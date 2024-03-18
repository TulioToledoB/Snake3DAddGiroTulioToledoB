using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bestScoreText;
    public GameObject gameOverPanel;
    public static int score = 0;
    public static int bestScore = 0;


    [SerializeField] private GameObject foodPrefab;
    [SerializeField] private Transform floorTransform;
    private float areaWidth;
    private float areaLength;
    public AudioSource audioSource;
    public AudioClip backgroundMusic;
    public AudioClip gameOverClip;


    void Start()
    {
        SpawnFood();
        score = 0;
        bestScore = PlayerPrefs.GetInt("BestScore", 0);
        audioSource.clip = backgroundMusic;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.Play();

        areaWidth = floorTransform.localScale.x;
        areaLength = floorTransform.localScale.z;
        UpdateScoreUI();
    }

    private void SpawnFood()
    {
        float randomX = UnityEngine.Random.Range(-areaWidth / 2, areaWidth / 2);
        float randomZ = UnityEngine.Random.Range(-areaLength / 2, areaLength / 2);
        Vector3 randomPosition = new Vector3(randomX, 0.5f, randomZ) + floorTransform.position;

        Instantiate(foodPrefab, randomPosition, Quaternion.identity);
    }

    public void GenerateFood()
    {
        SpawnFood();
    }
    public void AddScore(int pointsToAdd)
    {
        score += pointsToAdd;
        if (score > bestScore)
        {
            bestScore = score;
            PlayerPrefs.SetInt("BestScore", bestScore);
            PlayerPrefs.Save();
        }
        UpdateScoreUI();
    }


    public void ResetScore()
    {
        score = 0;
        UpdateScoreUI();
    }
    public void EndGame()
    {
        Time.timeScale = 0;
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        audioSource.Stop();
        audioSource.PlayOneShot(gameOverClip);
    }
    public void RestartGame()
    {
        ResetScore();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
        Time.timeScale = 1; 
    }
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
        if (bestScoreText != null)
        {
            bestScoreText.text = "Best Score: " + bestScore;
        }
    }

}
