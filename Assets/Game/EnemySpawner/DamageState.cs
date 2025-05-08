using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "DamageState", menuName = "Configs/DamageState")]
public class DamageState : ScriptableObject, IStickmanState
{
    [SerializeField] private float damageDuration = 0.2f;
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private Material flashMaterial;

    private Material _originalMaterial;
    private float timer;

    public void Enter(StickmanView stickman)
    {
        _originalMaterial = stickman.MeshRenderer.material;

        stickman.StopAllCoroutines();
        stickman.StartCoroutine(FlashCoroutine(stickman));

        //stickman.Animator.SetBool("IsDamaged", true);
        timer = 0;
    }
    private IEnumerator FlashCoroutine(StickmanView stickman)
    {
        stickman.MeshRenderer.material = flashMaterial;
        yield return new WaitForSeconds(flashDuration);
        stickman.MeshRenderer.material = _originalMaterial;
    }

    public void UpdateState(StickmanView stickman)
    {
        timer += Time.deltaTime;
        if (timer >= damageDuration)
        {
            //stickman.Animator.SetBool("IsDamaged", false);
            if (stickman.HealthProvider.IsAlive)
            {
                stickman.TransitionToState(new IdleState());
            }
        }
    }

    public void Exit(StickmanView stickman)
    {
        stickman.Animator.SetBool("IsDamaged", false);
    }
}