using UnityEngine;

public class BossPhase1Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 12;

    [Header("References")]
    [SerializeField] private SpriteRenderer bodyRenderer;
    [SerializeField] private BossPhase1Controller controller;

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color stunnedColor = Color.blue;

    private int currentHealth;
    private bool isStunned = false;
    private bool isDead = false;

    public bool IsDead => isDead;
    public bool IsStunned => isStunned;
    public int CurrentHealth => currentHealth;

    void Awake()
    {
        currentHealth = maxHealth;

        if (bodyRenderer != null)
            bodyRenderer.color = normalColor;
    }

    public void SetStunned(bool stunned)
    {
        isStunned = stunned;

        if (bodyRenderer != null)
            bodyRenderer.color = stunned ? stunnedColor : normalColor;
    }

    public void TakeSwordHit(int damage = 1)
    {
        if (isDead || !isStunned)
            return;

        currentHealth -= damage;

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        currentHealth = 0;

        if (controller != null)
            controller.Die();

        gameObject.SetActive(false);
    }
}