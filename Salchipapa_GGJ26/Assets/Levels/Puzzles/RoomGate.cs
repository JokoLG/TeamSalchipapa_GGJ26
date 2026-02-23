using System.Collections.Generic;
using UnityEngine;

public class RoomGate : MonoBehaviour
{
    public List<Spotlight> spotlightList = new List<Spotlight>();

    public bool closed = true;

    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Check if ALL spotlights are active
        bool allActive = true;

        foreach (Spotlight s in spotlightList)
        {
            if (!s.isActive)
            {
                allActive = false;
                break;
            }
        }

        // Open the gate if all are active
        closed = !allActive;

        boxCollider.enabled = closed;
        spriteRenderer.enabled = closed;
    }
}
