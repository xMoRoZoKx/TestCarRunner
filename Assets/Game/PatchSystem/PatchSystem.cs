using System.Collections.Generic;
using UnityEngine;

public class PatchSystem : MonoBehaviour
{
    private void Start()
    {

        List<Transform> points = GetPoints();
        points.ForEach(p => p.SetActive(false));
    }
    private void OnValidate()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            var point = child.GetComponent<PointView>();
            if (point != null)
            {
                point.id = i;
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(point);
#endif
            }
        }
    }
    public List<Transform> GetPoints()
    {

        List<Transform> points = new List<Transform>();
        foreach (Transform child in transform)
        {
            points.Add(child);
        }

        return points;
    }
    private void OnDrawGizmos()
    {
        List<Transform> points = GetPoints();

        if (points.Count < 2) return;

        Gizmos.color = Color.green;
        for (int i = 0; i < points.Count - 1; i++)
        {
            if (points[i] != null && points[i + 1] != null)
            {
                Gizmos.DrawLine(points[i].position, points[i + 1].position);
            }
        }
    }
}
