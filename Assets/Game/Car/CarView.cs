using System.Collections.Generic;
using UniTools;
using UnityEngine;

public class CarView : ConnectableMonoBehaviour
{
    [SerializeField, Range(-1f, 1f)] private float horizontalOffset = 0f;
    [SerializeField] private TileGenerator tileGenerator;
    [SerializeField] private float maxTiltAngle = 15f;
    [SerializeField] private float tiltSmoothness = 5f;
    [SerializeField] private List<WheelRotator> wheels;
    [SerializeField] private HealthProvider healthProvider;
    [SerializeField] private ParticleSystem deadEffect, driveEffect;
    [SerializeField] private TurretController turretController;
    [SerializeField] private RotationLocker rotationLocker;
    public HealthProvider HealthProvider => healthProvider;
    private void Awake()
    {
        connections += healthProvider.OnDeath.Subscribe(() => deadEffect.Play());
        turretController.Lock = true;
    }
    public void StartDrive()
    {
        turretController.Lock = false;
        driveEffect.Play();
    }
    public void StopDrive()
    {
        turretController.Lock = true;
        driveEffect.Stop();
    }
    private void FixedUpdate()
    {
        if (tileGenerator == null) return;

        TileView closestTile = GetClosestTile();
        if (closestTile?.PatchSystem == null) return;

        var (backPoint, frontPoint) = GetBackAndFrontPoints(closestTile);
        if (backPoint == null || frontPoint == null) return;

        UpdatePosition(backPoint, frontPoint);
        UpdateRotation(frontPoint);

        foreach (var wheel in wheels)
        {
            wheel.Rotate(tileGenerator.TileSpeed);
        }

        rotationLocker.Tick(Time.fixedDeltaTime);
    }

    private TileView GetClosestTile()
    {
        TileView closest = null;
        float minDist = float.MaxValue;
        foreach (var tile in tileGenerator.ActiveTiles)
        {
            float dist = Mathf.Abs(tile.transform.position.x - transform.position.x);
            if (dist < minDist)
            {
                minDist = dist;
                closest = tile;
            }
        }
        return closest;
    }

    private (Transform backPoint, Transform frontPoint) GetBackAndFrontPoints(TileView tile)
    {
        var points = tile.PatchSystem.GetPoints();
        if (points.Count < 2) return (null, null);

        float carX = transform.position.x;
        Transform back = null, front = null;
        float minBackDist = float.MaxValue, minFrontDist = float.MaxValue;

        foreach (var point in points)
        {
            float dx = point.transform.position.x - carX;
            float absDx = Mathf.Abs(dx);

            if (dx <= 0 && absDx < minBackDist)
            {
                minBackDist = absDx;
                back = point.transform;
            }
            else if (dx > 0 && absDx < minFrontDist)
            {
                minFrontDist = absDx;
                front = point.transform;
            }
        }

        if (back == null)
        {
            var prevPoints = tileGenerator.GetTileByIndex(tile.TileIndex - 1)?.PatchSystem?.GetPoints();
            if (prevPoints != null && prevPoints.Count > 0)
                back = prevPoints[0].transform;
        }

        if (front == null)
        {
            var nextPoints = tileGenerator.GetTileByIndex(tile.TileIndex + 1)?.PatchSystem?.GetPoints();
            if (nextPoints != null && nextPoints.Count > 0)
                front = nextPoints[^1].transform;
        }

        return (back, front);
    }

    private void UpdatePosition(Transform back, Transform front)
    {
        float carX = transform.position.x;
        float t = Mathf.InverseLerp(back.position.x, front.position.x, carX);
        Vector3 target = Vector3.Lerp(back.position, front.position, t);
        Vector3 dir = (front.position - back.position).normalized;
        Vector3 offset = Vector3.Cross(Vector3.forward, dir) * horizontalOffset;

        transform.position = new Vector3(transform.position.x, transform.position.y, target.z + offset.z);
    }

    private void UpdateRotation(Transform front)
    {
        Vector3 lookDir = front.position - transform.position;
        lookDir.y = 0f;

        if (lookDir.sqrMagnitude < 0.0001f) return;

        Quaternion targetRot = Quaternion.LookRotation(lookDir, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * tiltSmoothness);
    }
}
