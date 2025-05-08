using System.Collections;
using TMPro;
using UniTools;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultScreen : AnimatedWindowBase
{
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private Button tapBtn;

    public void Show(string result)
    {
        resultText.text = result;

        connections += tapBtn.Subscribe(() =>
        {
            ReloadScene();
        });
    }

    private void ReloadScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }
}
