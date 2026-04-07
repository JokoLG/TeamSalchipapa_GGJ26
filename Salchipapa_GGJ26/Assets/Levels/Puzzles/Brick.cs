using UnityEngine;

public class Brick : MonoBehaviour
{
    public void BreakBrick()
    {
        // play sound
        this.GetComponent<SpriteRenderer>().enabled = false;
        this.GetComponent<BoxCollider2D>().enabled = false;
        Destroy(this);
    }
}
