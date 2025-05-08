using UnityEngine;

public class PointView : MonoBehaviour
{
    [HideInInspector]
    public int id;

    private void OnValidate()
    {
        name = $"Point {id}";
    }
}
