using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ZombMovement : MonoBehaviour
{
    [Header("Hareket Ayarlarý")]
    public float moveSpeed = 3.5f;
    public bool stopAtTarget = true;
    public float stoppingDistance = 0.2f;

    private Transform targetTransform;
    private bool isMoving = false;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Zombinin çarpýþmada devrilmesini (yere kapaklanmasýný) önler
        rb.freezeRotation = true;

        // Yerçekiminden etkilenmesini istemiyorsanýz (örneðin düz zeminde yürüyorlarsa) kapatabilirsiniz:
        // rb.useGravity = false; 
    }

    public void SetTarget(Transform newTarget)
    {
        targetTransform = newTarget;
        isMoving = true;

        if (targetTransform != null)
        {
            Vector3 direction = (targetTransform.position - transform.position).normalized;
            direction.y = 0; // Sadece yatayda dön

            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }

    void FixedUpdate()
    {
        if (!isMoving || targetTransform == null) return;

        Vector3 targetPosition = targetTransform.position;
        targetPosition.y = transform.position.y; // Yükseklik farkýndan etkilenmemesi için

        // Rigidbody.MovePosition kullanarak fizik motorunu ezmeden güvenle ilerletiyoruz
        // Bu sayede hem çarpýþmalar (colliders) aktif çalýþýr hem de titreme olmaz.
        Vector3 newPosition = Vector3.MoveTowards(rb.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPosition);

        // Hedefe varýþ kontrolü
        if (stopAtTarget && Vector3.Distance(rb.position, targetPosition) <= stoppingDistance)
        {
            OnReachedTarget();
        }
    }

    void OnReachedTarget()
    {
        isMoving = false;
        gameObject.SetActive(false); // Object Pooling için pasife al
    }
}