using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Spawn Ayarlarý")]
    public GameObject enemyPrefab;          // Düþman prefab'i
    public float spawnInterval = 3f;        // Kaç saniyede bir spawn olacaðý
    public float spawnRadius = 5f;          // Çemberin yarýçapý

    [Header("Hedef Ayarlarý")]
    public Transform targetTransform;       // Sahnede takip edilecek hedef (Örn: Oyuncu veya Merkez)

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefab == null) return;

        // Çember içinde rastgele pozisyon hesapla
        Vector2 randomPoint2D = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPosition = transform.position + new Vector3(randomPoint2D.x, 0f, randomPoint2D.y);

        // 1. Düþmaný sahneye oluþtur
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        // 2. Oluþan düþmanýn üzerindeki EnemyMovement script'ini al ve hedefini anýnda ata!
        ZombMovement movement = newEnemy.GetComponent<ZombMovement>();
        if (movement != null && targetTransform != null)
        {
            movement.SetNewTarget(targetTransform);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}