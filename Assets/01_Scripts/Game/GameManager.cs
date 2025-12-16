using UnityEngine;
using System;
using UnityEngine.SceneManagement;

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
    public int money = 0; // Dinero actual del jugador

    // === EVENTOS PARA UIManager ===
    public event Action<int> OnLivesChanged;
    public event Action<int> OnWaveChanged;
    public event Action<bool> OnStrategyPhaseChanged;
    public event Action<float> OnStrategyTimeTick;
    public event Action OnGameOver;
    public event Action<int> OnMoneyChanged; // Evento para UI del dinero

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        currentLives = maxLives;
        currentWave = 1;
        OnLivesChanged?.Invoke(currentLives);
        OnWaveChanged?.Invoke(currentWave);
        OnMoneyChanged?.Invoke(money);
    }

    // -------------------------------------------------------
    // Daño al jugador
    // -------------------------------------------------------
    public void TakeDamage(int amount = 1)
    {
        currentLives -= amount;
        if (currentLives < 0) currentLives = 0;

        OnLivesChanged?.Invoke(currentLives);

        if (currentLives <= 0)
            TriggerGameOver();
    }

    void TriggerGameOver()
    {
        OnGameOver?.Invoke();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
    // Manejo de dinero
    // -------------------------------------------------------
    public void AddMoney(int amount)
    {
        money += amount;
        OnMoneyChanged?.Invoke(money);
    }

    public bool SpendMoney(int amount)
    {
        if (money >= amount)
        {
            money -= amount;
            OnMoneyChanged?.Invoke(money);
            return true;
        }
        return false; // No hay suficiente dinero
    }
    public bool TryBuy(int cost)
    {
        return SpendMoney(cost);
    }

}
