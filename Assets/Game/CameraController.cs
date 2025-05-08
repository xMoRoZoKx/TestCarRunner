using UnityEngine;
using DG.Tweening;
using System;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform menuPosition;
    [SerializeField] private Transform carPosition;
    [SerializeField] private Camera cameraView;
    [SerializeField] private float transitionDuration = 1.5f;
    [SerializeField] private Ease transitionEase = Ease.InOutQuad;

    private Tween moveTween;
    private Tween rotateTween;

    public void SwitchToMenu(Action onComplete = null)
    {
        SmoothTransition(menuPosition, onComplete);
    }

    public void SwitchToCar(Action onComplete = null)
    {
        SmoothTransition(carPosition, onComplete);
    }

    private void SmoothTransition(Transform target, Action onComplete)
    {
        moveTween?.Kill();
        rotateTween?.Kill();

        moveTween = cameraView.transform.DOMove(target.position, transitionDuration)
            .SetEase(transitionEase);

        rotateTween = cameraView.transform.DORotateQuaternion(target.rotation, transitionDuration)
            .SetEase(transitionEase)
            .OnComplete(() =>
            {
                onComplete?.Invoke();
            });
    }
}
