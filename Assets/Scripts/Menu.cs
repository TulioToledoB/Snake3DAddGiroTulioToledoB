using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Menu : MonoBehaviour
{
    [SerializeField] private string mainSceneName = "Snake3D";
    public TextMeshProUGUI bestScoreText;
    private void Start()
    {
        BestScore();
    }
    public void StartGame()
    {
        SceneManager.LoadScene(mainSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    private void BestScore()
    {
        int bestScore = PlayerPrefs.GetInt("BestScore", 0);
        bestScoreText.text = "Best Score: " + bestScore;
    }

    public void SetControlType (bool useGyro)
    {
        PlayerPrefs.SetInt("UseGyroControl", useGyro ? 1 : 0);
        PlayerPrefs.Save();
    }
}