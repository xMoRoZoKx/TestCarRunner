using UniTools.Reactive;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class HealthProvider : ConnectableMonoBehaviour
{
    [SerializeField] private float maxValue;
    [SerializeField] private Reactive<float> value;

    [Header("Events")]
    [SerializeField] private UnityEvent onDeath;
    [SerializeField] private UnityEvent<float> onDamageTaken;

    public IReactive<float> Value => value;
    public float MaxValue => maxValue;
    public UnityEvent OnDeath => onDeath;
    public UnityEvent<float> OnDamageTaken => onDamageTaken;

    public bool IsAlive => value.value > 0;

    private Collider _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _collider.isTrigger = true;
        FullRestore();
    }

    public void Init(float maxHealthValue)
    {
        maxValue = maxHealthValue;
        FullRestore();
    }

    public void TakeDamage(float amount)
    {
        if (!IsAlive) return;

        value.value = Mathf.Max(value.value - amount, 0);

        onDamageTaken?.Invoke(amount);

        if (!IsAlive)
        {
            HandleDeath();
        }
    }

    public void Kill()
    {
        value.value = 0;
        HandleDeath();
    }

    public void Heal(float amount)
    {
        if (!IsAlive) return;

        value.value = Mathf.Min(value.value + amount, maxValue);
    }

    public void FullRestore()
    {
        value.value = maxValue;
    }

    private void HandleDeath()
    {
        onDeath?.Invoke();
        if (_collider != null)
        {
            _collider.enabled = false;
        }
    }
}
