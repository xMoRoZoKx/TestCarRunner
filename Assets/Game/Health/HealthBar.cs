using UniTools.Reactive;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : ConnectableMonoBehaviour
{
    [SerializeField] private HealthProvider healthProvider;
    [SerializeField] private Slider slider;

    private void Awake()
    {
        if (healthProvider != null)
        {
            Initialize(healthProvider);
        }
    }

    public void Initialize(HealthProvider provider)
    {
        healthProvider = provider;
        slider.maxValue = healthProvider.MaxValue;
        slider.value = healthProvider.Value.Value;

        connections += healthProvider.Value.SubscribeAndInvoke(OnHealthChanged);
    }

    private void OnHealthChanged(float newValue)
    {
        slider.SetActive(healthProvider.MaxValue > newValue);
        slider.value = newValue;
    }
}
