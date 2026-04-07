using UnityEngine;

public class Spotlight : MonoBehaviour
{
    public bool isActive = false;

    private SpriteRenderer sr;

    public Sprite spotOff;
    public Sprite spotOn;

    public SFX_Manager sfx;
    public bool PlaySound = true;

    public void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Update()
    {
        if (isActive)
        {
            sr.sprite = spotOn;
            if (PlaySound) 
            {
                sfx.Play("SpotLightON"); 
                PlaySound = false;
            }
        }
        else sr.sprite = spotOff;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        Brick brick = collision.gameObject.GetComponent<Brick>();
        if (brick != null)
        {
            isActive = false;
        }
    }
}