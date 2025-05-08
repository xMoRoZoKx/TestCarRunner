using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TurretController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float fireRate = 0.2f;
    [SerializeField] private int poolSize = 20;
    [SerializeField] private float recoilDistance = 0.2f;  // Сила отдачи
    [SerializeField] private float recoilDuration = 0.1f;  // Длительность отдачи
    private List<Bullet> activeBullets = new();
    private float fireTimer;
    private ObjectPool bulletPool;
    private Vector3 initialPosition;
    public bool Lock = false;

    private void Start()
    {
        bulletPool = new ObjectPool(projectilePrefab, poolSize);
        initialPosition = transform.localPosition;
    }

    private void Update()
    {
        if (Lock) return;

        Vector3? aimPosition = GetTouchWorldPosition();
        if (aimPosition.HasValue)
        {
            RotateToTarget(aimPosition.Value);
            TryShoot();
        }
        UpdateBullets();
    }

    private void RotateToTarget(Vector3 targetPos)
    {
        Vector3 dir = targetPos - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 15f);
    }

    private void TryShoot()
    {
        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0f)
        {
            fireTimer = fireRate;

            GameObject bulletObj = bulletPool.Get();
            Bullet bullet = bulletObj.GetComponent<Bullet>();
            bullet.Activate(firePoint.position, firePoint.rotation);
            activeBullets.Add(bullet);

            AnimateRecoil();
        }
    }

    private void AnimateRecoil()
    {
        // Останавливаем текущие анимации, если они есть
        transform.DOKill();

        Vector3 recoilOffset = -transform.right * recoilDistance;

        // Анимация отката
        transform.DOLocalMove(initialPosition + recoilOffset, recoilDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                // Возвращение обратно
                transform.DOLocalMove(initialPosition, recoilDuration).SetEase(Ease.InQuad);
            });
    }

    private void UpdateBullets()
    {
        for (int i = activeBullets.Count - 1; i >= 0; i--)
        {
            var bullet = activeBullets[i];
            bullet.Tick(Time.deltaTime);

            if (!bullet.IsAlive)
                activeBullets.RemoveAt(i);
        }
    }

    private Vector3? GetTouchWorldPosition()
    {
#if UNITY_EDITOR
        if (!Input.GetMouseButton(0)) return null;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
#else
        if (Input.touchCount == 0) return null;
        Ray ray = mainCamera.ScreenPointToRay(Input.GetTouch(0).position);
#endif
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Ground")))
            return hit.point;

        return null;
    }
}
