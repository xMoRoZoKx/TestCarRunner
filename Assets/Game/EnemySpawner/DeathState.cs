

using UnityEngine;

[CreateAssetMenu(fileName = "DeathState", menuName = "Configs/DeathState")]
public class DeathState : ScriptableObject, IStickmanState
{
    [SerializeField] private float deathDuration = 0.1f;
    [SerializeField] private Material flashMaterial;
    public void Enter(StickmanView stickman)
    {
        //stickman.Animator.SetBool("IsDead", true);

        stickman.MeshRenderer.material = flashMaterial;
        Object.Destroy(stickman.gameObject, deathDuration);
    }

    public void UpdateState(StickmanView stickman) { }

    public void Exit(StickmanView stickman)
    {
    }
}