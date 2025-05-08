using UnityEngine;

public class LockRotator : MonoBehaviour
{
    private Quaternion _initialLocalRotation;

    private void Awake()
    {
        _initialLocalRotation = transform.rotation;
    }

    private void LateUpdate()
    {
        transform.rotation = _initialLocalRotation;
    }
}
