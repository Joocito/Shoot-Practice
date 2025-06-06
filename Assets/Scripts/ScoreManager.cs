using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public TextMeshProUGUI scoreText;
    private int currentScore = 0;
    private int speedIncreaseThreshold = 100;
    private int nextThreshold = 100;

    // Evento para notificar aumento de velocidad
    public delegate void SpeedIncreaseHandler(int speedIncreaseCount);
    public static event SpeedIncreaseHandler OnSpeedIncrease;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        InitializeScore();
    }

    void InitializeScore()
    {
        currentScore = 0;
        nextThreshold = speedIncreaseThreshold;
        UpdateScoreText();
    }

    public void AddScore(int points)
    {
        currentScore += points;
        CheckForSpeedIncrease();
        UpdateScoreText();
    }

    void CheckForSpeedIncrease()
    {
        if (currentScore >= nextThreshold)
        {
            nextThreshold += speedIncreaseThreshold;
            int speedIncreaseCount = currentScore / speedIncreaseThreshold;

            // Disparar evento
            OnSpeedIncrease?.Invoke(speedIncreaseCount);

            Debug.Log($"Velocidad aumentada! Nivel: {speedIncreaseCount}");
        }
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Puntos: {currentScore}\nNivel Velocidad: {currentScore / speedIncreaseThreshold}";
        }
    }
}