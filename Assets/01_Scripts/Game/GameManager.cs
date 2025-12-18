using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player")]
    public int maxLives = 3;
    public int currentLives;

    [Header("Wave")]
    public int currentWave = 1;

    [Header("Estrategia")]
    public bool strategyPhaseActive = false;
    public float strategyTime = 5f;
    private float strategyTimer = 0f;

    [Header("Economía")]
    public int money = 0;

    [Header("UI")]
    public TMP_Text moneyText;
    public TMP_Text livesText; // ✅ NUEVO

    // === EVENTOS ===
    public event Action<int> OnLivesChanged;
    public event Action<int> OnWaveChanged;
    public event Action<bool> OnStrategyPhaseChanged;
    public event Action<float> OnStrategyTimeTick;
    public event Action OnGameOver;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    void Start()
    {
        currentLives = maxLives;
        currentWave = 1;

        OnLivesChanged?.Invoke(currentLives);
        OnWaveChanged?.Invoke(currentWave);

        UpdateMoneyText();
        UpdateLivesText(); // ✅
    }

    // -------------------------------------------------------
    // Daño al jugador
    // -------------------------------------------------------
    public void TakeDamage(int amount = 1)
    {
        currentLives -= amount;
        if (currentLives < 0) currentLives = 0;

        OnLivesChanged?.Invoke(currentLives);
        UpdateLivesText(); // ✅

        if (currentLives <= 0)
            TriggerGameOver();
    }

    void TriggerGameOver()
    {
        OnGameOver?.Invoke();
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    // -------------------------------------------------------
    // Waves
    // -------------------------------------------------------
    public void SetWave(int wave)
    {
        currentWave = wave;
        OnWaveChanged?.Invoke(currentWave);
    }

    // -------------------------------------------------------
    // Estrategia
    // -------------------------------------------------------
    public void StartStrategyPhase(float time)
    {
        strategyPhaseActive = true;
        strategyTimer = time;

        OnStrategyPhaseChanged?.Invoke(true);
        StartCoroutine(StrategyCountdown());
    }

    System.Collections.IEnumerator StrategyCountdown()
    {
        while (strategyTimer > 0f)
        {
            strategyTimer -= Time.deltaTime;
            OnStrategyTimeTick?.Invoke(strategyTimer);
            yield return null;
        }

        strategyPhaseActive = false;
        OnStrategyPhaseChanged?.Invoke(false);
    }

    // -------------------------------------------------------
    // Dinero
    // -------------------------------------------------------
    public void AddMoney(int amount)
    {
        money += amount;
        UpdateMoneyText();
    }

    public bool SpendMoney(int amount)
    {
        if (money >= amount)
        {
            money -= amount;
            UpdateMoneyText();
            return true;
        }
        return false;
    }

    public bool TryBuy(int cost) => SpendMoney(cost);

    void UpdateMoneyText()
    {
        if (moneyText != null)
            moneyText.text = money.ToString() + "$";
    }

    void UpdateLivesText()
    {
        if (livesText != null)
            livesText.text = currentLives.ToString(); // o: $"{currentLives}/{maxLives}"
    }
}
