using UnityEngine;

public class CenterBase : MonoBehaviour
{
    [Header("Etki Alaný Ayarlarý")]
    [Tooltip("Zombilerin merkeze ulaþtý sayýlacaðý mesafe (Yarýçap)")]
    [SerializeField] private float radius = 3.0f;
    [SerializeField] private Color gizmoColor = Color.yellow;

    // Dýþarýdan yarýçap deðerini almak için property
    public float Radius => radius;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, radius);

        // Þeffaf iç dolgu
        Color fillColor = gizmoColor;
        fillColor.a = 0.15f;
        Gizmos.color = fillColor;
        Gizmos.DrawSphere(transform.position, radius);
    }
}