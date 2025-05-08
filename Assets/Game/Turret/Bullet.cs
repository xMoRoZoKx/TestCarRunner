using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private float damage = 10f;

    private float timeAlive;

    private void Awake()
    {
        var collider = GetComponent<Collider>();
        collider.isTrigger = true;
    }

    public void Activate(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
        gameObject.SetActive(true);
        timeAlive = 0f;
    }

    public void Tick(float deltaTime)
    {
        transform.position += -transform.up * speed * deltaTime;
        timeAlive += deltaTime;

        if (timeAlive > lifetime)
            gameObject.SetActive(false);
    }

    public bool IsAlive => gameObject.activeSelf;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsAlive) return;

        var healthProvider = other.GetComponent<HealthProvider>();
        if (healthProvider != null)
        {
            healthProvider.TakeDamage(damage);
            gameObject.SetActive(false);
        }
    }
}
