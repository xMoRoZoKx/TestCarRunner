using System;
using System.Collections;
using System.Collections.Generic;
using UniTools;
using UnityEngine;
using UnityEngine.UI;

public class MenuScreen : WindowBase
{
    [SerializeField] private Button startBtn;
    public void Show(Action onStartClick)
    {
        connections += startBtn.Subscribe(onStartClick);
    }
}
