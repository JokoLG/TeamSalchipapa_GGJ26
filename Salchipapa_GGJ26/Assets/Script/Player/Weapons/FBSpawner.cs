using UnityEngine;

public class FBSpawner : MonoBehaviour
{
    [Header("Sound Manager")]
    public P_SoundHandler soundPlayer;
    
    [Header("Fireball")]
    [SerializeField] private Fireball fireballPrefab;
    [SerializeField] private Transform fireballSpawn;
    [SerializeField] private Transform aimTransform; // object with Z rotation

    [Header("Cooldown")]
    [SerializeField] private float cooldownTime = 0.35f;

    private float cooldownTimer = 0f;
    private bool isOnCooldown = false;

    void Update()
    {
        // Cooldown ticking
        if (isOnCooldown)
        {
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= cooldownTime)
            {
                isOnCooldown = false;
                cooldownTimer = 0f;
            }
        }

        // Input
        if (Input.GetKeyDown(KeyCode.F) && !isOnCooldown)
        {
            SpawnFireball();
            StartCooldown();
            soundPlayer.Play("WitchFireBall", 1f);
        }
    }

    void SpawnFireball()
    {
        Fireball fb = Instantiate(
            fireballPrefab,
            fireballSpawn.position,
            aimTransform.rotation
        );

        fb.Init(aimTransform);
    }

    void StartCooldown()
    {
        isOnCooldown = true;
        cooldownTimer = 0f;
    }
}
