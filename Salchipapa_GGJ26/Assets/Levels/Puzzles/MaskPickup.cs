using UnityEngine;

public class MaskPickup : MonoBehaviour
{
    public bool isActive = true;

    public string maskType;
    public string pickupSound;
    public string BGM;

    public SFX_Manager sfx;
    public BGM_Manager bgm;
    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!isActive) 
        {
            return;
        }
        else
        {
            return;
        }

        sfx.Play(pickupSound);
        bgm.PlayBGM(BGM);
    }
}
