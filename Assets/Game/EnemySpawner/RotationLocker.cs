using UnityEngine;

public class RotationLocker : MonoBehaviour
{
    private Quaternion _initialLocalRotation;

    private void Awake()
    {
        _initialLocalRotation = transform.rotation;
    }

    public void Tick(float dt)
    {
        transform.rotation = _initialLocalRotation;
    }
}
