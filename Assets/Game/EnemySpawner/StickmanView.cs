using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using System.Collections;

public class StickmanView : RandomizedTileObject
{
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float damageAmount = 1f;
    [SerializeField] private Animator animator;
    [SerializeField] private HealthProvider healthProvider;
    [SerializeField] private SkinnedMeshRenderer meshRenderer;
    [SerializeField] private DamageState damageState;
    [SerializeField] private DeathState deathState;
    [SerializeField] private RotationLocker rotationLocker;

    private IStickmanState currentState;
    private Transform target;

    public Animator Animator => animator;
    public HealthProvider HealthProvider => healthProvider;
    public CarView Car { get; private set; }
    public SkinnedMeshRenderer MeshRenderer => meshRenderer;
    public float DetectionRadius => detectionRadius;
    private void Awake()
    {
        Car = ServiceLocator.Resolve<CarView>();
        TransitionToState(new IdleState());
        connections += healthProvider.OnDeath.Subscribe(() => TransitionToState(deathState));
        connections += healthProvider.OnDamageTaken.Subscribe(value =>
        {
            TransitionToState(damageState);
        });

    }

    public override void Tick(float dt)
    {
        currentState?.UpdateState(this);
        rotationLocker?.Tick(dt);
    }

    public void TransitionToState(IStickmanState newState)
    {
        currentState?.Exit(this);
        currentState = newState;
        currentState?.Enter(this);
    }

    public void MoveTowards(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
        transform.forward = direction;
    }

    public void SetTarget(Transform car) => target = car;
    public Transform GetTargetCar() => target;

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<CarView>(out var car))
        {
            var carHealthProvider = car.HealthProvider;
            if (carHealthProvider != null && carHealthProvider.IsAlive)
            {
                carHealthProvider.TakeDamage(damageAmount);
                healthProvider.Kill();
            }
        }
    }

}
