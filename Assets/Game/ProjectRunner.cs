using System.Collections;
using System.Collections.Generic;
using UniTools;
using UnityEngine;

public class ProjectRunner : ConnectableMonoBehaviour
{
    [SerializeField] private TileGenerator tileGenerator;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private LevelData levelData;
    [SerializeField] private CarView carView;
    private void Awake()
    {
        var windowManager = WindowManager.Instance;

        ServiceLocator.RegisterSingleton(windowManager);
        ServiceLocator.RegisterSingleton(carView);
        

        tileGenerator.SetSeed(levelData.seed);
        tileGenerator.Init();

        connections += tileGenerator.CurrentCenterIndex.Subscribe(idx =>
        {
            if (idx >= levelData.levelLength)
            {
                tileGenerator.StopGeneration();
                carView.StopDrive();
                windowManager.Show<ResultScreen>(inst => inst.Show("You win"));
            }
        });
        connections += carView.HealthProvider.OnDeath.Subscribe(() =>
        {
            tileGenerator.StopGeneration();
            carView.StopDrive();
            windowManager.Show<ResultScreen>(inst => inst.Show("You lose"));
        });

        windowManager.Show<MenuScreen>(inst => inst.Show(() =>
        {
            inst.Close();
            cameraController.SwitchToCar(() =>
            {
                tileGenerator.StartGeneration();
                carView.StartDrive();
            });
        }));
    }
}
