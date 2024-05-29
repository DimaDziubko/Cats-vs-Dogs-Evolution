using UnityEngine;

public class CrSpawnMarker : MonoBehaviour
{
    [SerializeField]
    private Color _color = Color.red;
    private void OnDrawGizmos()
    {
        Gizmos.color = _color;
        Gizmos.DrawSphere(transform.position, 0.1f);
    }
}
