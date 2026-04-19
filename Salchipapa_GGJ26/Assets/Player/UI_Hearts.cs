using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Hearts : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private P_Health playerHealth;
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private Transform heartContainer;

    [Header("Sprites")]
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;

    private List<Image> hearts = new List<Image>();
    private int maxHealth;

    void Start()
    {
        // Uses the player's starting remaining hits as max health
        maxHealth = playerHealth.HitsRemaining;

        for (int i = 0; i < maxHealth; i++)
        {
            GameObject heart = Instantiate(heartPrefab, heartContainer);

            RectTransform rt = heart.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(-380f + i * 35f, 200f);

            Image heartImage = heart.GetComponent<Image>();
            hearts.Add(heartImage);
        }

        UpdateHearts();
    }

    void Update()
    {
        UpdateHearts();
    }

    void UpdateHearts()
    {
        int currentHealth = playerHealth.HitsRemaining;

        for (int i = 0; i < hearts.Count; i++)
        {
            if (i < currentHealth)
                hearts[i].sprite = fullHeart;
            else
                hearts[i].sprite = emptyHeart;
        }
    }
}