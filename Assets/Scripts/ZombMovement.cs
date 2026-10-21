using UnityEngine;

public class ZombMovement : MonoBehaviour
{
    [Header("Hareket Ayarlarý")]
    public float moveSpeed = 3.5f;
    public bool stopAtTarget = true;
    public float stoppingDistance = 0.2f;

    private Transform targetTransform;
    private bool isMoving = false;

    // Hedef atandýðýnda veya deðiþtiðinde bir kez çalýþýr
    public void SetTarget(Transform newTarget)
    {
        targetTransform = newTarget;
        isMoving = true;

        if (targetTransform != null)
        {
            // Dönme iþlemini sadece hedef belirlendiðinde BÝR KEZ yapýyoruz
            Vector3 direction = (targetTransform.position - transform.position).normalized;
            direction.y = 0; // Y ekseninde dönmeyi sabitlemek için (isteðe baðlý)

            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }

    void Update()
    {
        if (!isMoving || targetTransform == null) return;

        Vector3 targetPosition = targetTransform.position;

        // Düþmaný direkt hedefe doðru ilerlet (Her karede dönme hesaplamasý yapýlmaz)
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Hedefe varýp varmadýðýný kontrol et
        if (stopAtTarget && Vector3.Distance(transform.position, targetPosition) <= stoppingDistance)
        {
            OnReachedTarget();
        }
    }

    void OnReachedTarget()
    {
        isMoving = false;

        // Object Pooling kullandýðýmýz için Destroy yerine objeyi kapatýyoruz (Pasif yapýyoruz)
        gameObject.SetActive(false);
    }
}