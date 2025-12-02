using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    [Header("Vidas")]
    public Image[] lifeImages;       // Asignar desde el inspector (todas las imágenes de vida)
    public Sprite fullLifeSprite;    // Sprite de vida llena
    public Sprite emptyLifeSprite;   // Sprite de vida vacía

    [Header("Oleadas")]
    public TextMeshProUGUI waveText; // Usar TextMeshPro

    [Header("Estrategia")]
    public GameObject strategyPanel;
    public TextMeshProUGUI strategyTimerText; // TextMeshPro

    [Header("Game Over")]
    public GameObject gameOverPanel;

    void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnLivesChanged += UpdateLives;
            GameManager.Instance.OnWaveChanged += SetWaveText;
            GameManager.Instance.OnStrategyPhaseChanged += OnStrategyPhaseChanged;
            GameManager.Instance.OnStrategyTimeTick += UpdateStrategyTimer;
            GameManager.Instance.OnGameOver += ShowGameOver;
        }
    }

    void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnLivesChanged -= UpdateLives;
            GameManager.Instance.OnWaveChanged -= SetWaveText;
            GameManager.Instance.OnStrategyPhaseChanged -= OnStrategyPhaseChanged;
            GameManager.Instance.OnStrategyTimeTick -= UpdateStrategyTimer;
            GameManager.Instance.OnGameOver -= ShowGameOver;
        }
    }

    void UpdateLives(int lives)
    {
        for (int i = 0; i < lifeImages.Length; i++)
        {
            lifeImages[i].sprite = (i < lives) ? fullLifeSprite : emptyLifeSprite;
        }
    }

    void SetWaveText(int waveNumber)
    {
        if (waveText != null)
            waveText.text = $"Oleada: {waveNumber}";
    }

    void OnStrategyPhaseChanged(bool active)
    {
        if (strategyPanel != null)
            strategyPanel.SetActive(active);
    }

    void UpdateStrategyTimer(float secondsLeft)
    {
        if (strategyTimerText != null)
            strategyTimerText.text = $"Estrategia: {Mathf.CeilToInt(secondsLeft)}s";
    }

    void ShowGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }
}
