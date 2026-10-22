using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ZombMovement : MonoBehaviour
{
    [Header("Hareket Ayarlarý")]
    public float moveSpeed = 3.5f;

    private Transform targetTransform;
    private float stoppingDistance = 3.0f;  // Merkezden otomatik alýnacak
    private bool isMoving = false;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    public void SetTarget(Transform newTarget)
    {
        targetTransform = newTarget;
        isMoving = true;

        if (targetTransform != null)
        {
            // MERKEZDEKÝ YARIÇAPI OTOMATÝK OKU
            CenterBase centerBase = targetTransform.GetComponent<CenterBase>();
            if (centerBase != null)
            {
                stoppingDistance = centerBase.Radius; // Artýk merkezde ne yazýyorsa o geçerli!
            }

            Vector3 direction = (targetTransform.position - transform.position).normalized;
            direction.y = 0;

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
        targetPosition.y = transform.position.y;

        // Hedefe doðru ilerlet
        Vector3 newPosition = Vector3.MoveTowards(rb.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPosition);

        // Merkezden gelen yarýçap mesafesine girip girmediðini kontrol et
        if (Vector3.Distance(rb.position, targetPosition) <= stoppingDistance)
        {
            ReachCenter();
        }
    }

    void ReachCenter()
    {
        isMoving = false;

        Target myTargetComponent = GetComponent<Target>();
        if (myTargetComponent != null)
        {
            myTargetComponent.ReachCenter();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}