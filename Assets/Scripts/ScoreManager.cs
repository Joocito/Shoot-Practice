using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public TextMeshProUGUI scoreText;
    private int currentScore = 0;

    void Awake()
    {
        // Mejor inicialización del Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persiste entre escenas si es necesario
        }

        InitializeScore();
    }

    void InitializeScore()
    {
        currentScore = 0;
        UpdateScoreText();
    }

    public void AddScore(int points)
    {
        currentScore += points;
        Debug.Log($"Adding {points} points. Total: {currentScore}"); // Debug
        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        if (scoreText == null)
        {
            Debug.LogError("ScoreText reference is missing!");
            return;
        }

        scoreText.text = $"Puntos: {currentScore}";
    }
}