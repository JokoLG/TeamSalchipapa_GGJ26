using UnityEngine;

public class CollisionSensor2D : MonoBehaviour
{
    public LayerMask obstacleMask;
    private BoxCollider2D col;

    public bool blocked { get; private set; }

    void Awake()
    {
        col = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        blocked = col.IsTouchingLayers(obstacleMask);
    }
}