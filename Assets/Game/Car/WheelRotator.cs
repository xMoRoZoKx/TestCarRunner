using UnityEngine;

public class WheelRotator : MonoBehaviour
{
    [SerializeField] private float speedOffset = 1f;
    [SerializeField] private Vector3 rotationAxis = Vector3.right;

    public void Rotate(float speed)
    {
        float rotationSpeed = speedOffset;
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime, Space.Self);
    }
}
