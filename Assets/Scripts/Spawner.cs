using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Spawn Ayarlarý")]
    public GameObject enemyPrefab;
    public float spawnInterval = 3f;
    public float spawnRadius = 5f;

    [Header("Havuz ve Performans Sýnýrlarý")]
    public int initialPoolSize = 20;        // Baþlangýçta havuzda kaç zombi olsun?
    public int maxActiveZombies = 50;       // Ekranda ayný anda bulunabilecek maksimum zombi sýnýrý
    public bool allowDynamicGrowth = true;  // Havuz tamamen dolarsa yeni zombi üretilsin mi?

    [Header("Hedef Ayarlarý")]
    public Transform targetTransform;

    private List<GameObject> enemyPool;
    private float spawnTimer;

    void Start()
    {
        InitializePool();
    }

    void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            TrySpawnEnemy();
        }
    }

    void InitializePool()
    {
        enemyPool = new List<GameObject>();

        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab);
            enemy.SetActive(false);
            enemyPool.Add(enemy);
        }
    }

    void TrySpawnEnemy()
    {
        // 1. ADIM: Ekranda þu an aktif olan zombi sayýsýný hesapla
        int currentActiveCount = GetActiveZombieCount();

        // Eðer maksimum sýnýra ulaþýldýysa yeni zombi spawn etme (Performansý korur)
        if (currentActiveCount >= maxActiveZombies)
        {
            return;
        }

        // 2. ADIM: Havuzda boþta (pasif) zombi ara
        GameObject enemyToSpawn = null;

        foreach (GameObject enemy in enemyPool)
        {
            if (!enemy.activeInHierarchy)
            {
                enemyToSpawn = enemy;
                break;
            }
        }

        // 3. ADIM: Havuzda boþ yoksa ve dinamik büyüme açýksa yeni üret
        if (enemyToSpawn == null)
        {
            if (allowDynamicGrowth)
            {
                enemyToSpawn = Instantiate(enemyPrefab);
                enemyPool.Add(enemyToSpawn);
            }
            else
            {
                return; // Dinamik büyüme kapalýysa çýk
            }
        }

        // Pozisyonu belirle ve aktif et
        Vector2 randomPoint2D = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPosition = transform.position + new Vector3(randomPoint2D.x, 0f, randomPoint2D.y);

        enemyToSpawn.transform.position = spawnPosition;
        enemyToSpawn.SetActive(true);

        // Hedefi bildir
        ZombMovement movement = enemyToSpawn.GetComponent<ZombMovement>();
        if (movement != null && targetTransform != null)
        {
            movement.SetTarget(targetTransform);
        }
    }

    // Sahnedeki o an aktif olan zombileri sayan yardýmcý fonksiyon
    int GetActiveZombieCount()
    {
        int count = 0;
        foreach (GameObject enemy in enemyPool)
        {
            if (enemy.activeInHierarchy)
            {
                count++;
            }
        }
        return count;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}