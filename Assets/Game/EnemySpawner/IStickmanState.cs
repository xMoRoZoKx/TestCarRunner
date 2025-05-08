using DG.Tweening;
using System.Collections;
using UnityEngine;

public interface IStickmanState
{
    void Enter(StickmanView stickman);
    void UpdateState(StickmanView stickman);
    void Exit(StickmanView stickman);
}

public class IdleState : IStickmanState
{
    public void Enter(StickmanView stickman)
    {
        stickman.Animator.SetBool("IsRunning", false);
        stickman.Animator.SetBool("IsDamaged", false);
        //stickman.Animator.SetBool("IsDead", false);
    }

    public void UpdateState(StickmanView stickman)
    {
        var car = stickman.Car;
        if (car != null && Vector3.Distance(stickman.transform.position, car.transform.position) < stickman.DetectionRadius)
        {
            stickman.SetTarget(car.transform);
            stickman.TransitionToState(new MoveToCarState());
        }
    }

    public void Exit(StickmanView stickman) { }
}

public class MoveToCarState : IStickmanState
{
    public void Enter(StickmanView stickman)
    {
        stickman.Animator.SetBool("IsRunning", true);
    }

    public void UpdateState(StickmanView stickman)
    {
        var car = stickman.GetTargetCar();
        if (car == null)
        {
            stickman.TransitionToState(new IdleState());
            return;
        }

        stickman.MoveTowards(car.transform.position);
    }

    public void Exit(StickmanView stickman)
    {
        stickman.Animator.SetBool("IsRunning", false);
    }
}


