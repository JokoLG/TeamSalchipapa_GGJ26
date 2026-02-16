using UnityEngine;

public class Spotlight : MonoBehaviour
{
    public bool isActive = false;

    private SpriteRenderer sr;

    public Sprite spotOff;
    public Sprite spotOn;

    public void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void TurnLightOn()
    {
        isActive = true;
        sr.color = Color.green;
    }

    public void Update()
    {
        if (isActive) sr.sprite = spotOn;
        else sr.sprite = spotOff;
    }
}