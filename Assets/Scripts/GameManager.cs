using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Merkez (Base) Can Ayarlarý")]
    [SerializeField] private float maxBaseHealth = 500f; // Inspector'dan kolayca ayarlanabilir
    private float currentBaseHealth;

    [Header("Para (Altýn) Ayarlarý")]
    [SerializeField] private int totalGold = 0;

    [Header("UI (Arayüz) Baðlantýlarý")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI goldText;

    private bool isGameOver = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        currentBaseHealth = maxBaseHealth;
    }

    void Start()
    {
        UpdateUI();
    }

    // Merkeze hasar geldiðinde burasý tetiklenir
    public void DamageBase(float amount)
    {
        if (isGameOver) return;

        currentBaseHealth -= amount;
        currentBaseHealth = Mathf.Clamp(currentBaseHealth, 0f, maxBaseHealth);

        UpdateUI();

        if (currentBaseHealth <= 0f)
        {
            GameOver();
        }
    }

    // Altýn ekleme fonksiyonu
    public void AddGold(int amount)
    {
        totalGold += amount;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (healthText != null)
        {
            healthText.text = "Merkez Caný: " + currentBaseHealth.ToString("F0");
        }

        if (goldText != null)
        {
            goldText.text = "Altýn: " + totalGold.ToString();
        }
    }

    private void GameOver()
    {
        isGameOver = true;
        Debug.Log("Oyun Bitti! Merkez Yýkýldý.");

        // Ýleride buraya Game Over Paneli açma, zamaný durdurma vb. ekleyebilirsin
        // Time.timeScale = 0f;
    }
}