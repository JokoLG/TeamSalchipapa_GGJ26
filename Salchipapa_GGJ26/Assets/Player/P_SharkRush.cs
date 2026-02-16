using UnityEngine;

public enum SharkState { Idle, Charging, Rushing, Recovering }

[RequireComponent(typeof(Rigidbody2D))]
public class P_SharkRush : MonoBehaviour
{
    public bool isActive = false;

    [Header("Sound Manager")]
    public P_SoundHandler soundPlayer;

    [Header("References")]
    public P_Movement movement;

    [Header("Input")]
    public KeyCode rushKey = KeyCode.Q;

    [Header("Rush Settings")]
    public float chargeTime = 0.6f;
    public float rushSpeed = 10f;
    public float recoverDelay = 0.35f;

    private Rigidbody2D rb;

    public SharkState state = SharkState.Idle;

    private float timer = 0f;
    private Vector2 rushDir = Vector2.right;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (movement == null) movement = GetComponent<P_Movement>();

        // keep 2D rotation stable (optional, but nice)
        rb.freezeRotation = true;
    }

    void Update()
    {
        switch (state)
        {
            case SharkState.Idle:
                //if (Input.GetKeyDown(rushKey))
                    //StartCharge();
                break;

            case SharkState.Charging:
                timer -= Time.deltaTime;
                if (timer <= 0f)
                    StartRush();
                break;

            case SharkState.Recovering:
                timer -= Time.deltaTime;
                if (timer <= 0f)
                    EndRecover();
                break;
        }
    }

    void FixedUpdate()
    {
        if (state == SharkState.Rushing)
        {
            // Keep rushing forward constantly until collision stops it
            rb.linearVelocity = rushDir * rushSpeed;
        }
    }

    public void StartCharge()
    {
        // lock player control + "rotation" (your facing stays frozen)
        if (movement != null) movement.SetControlsLocked(true);

        // store the direction we will rush in (direction player is looking)
        rushDir = (movement != null) ? movement.GetFacingVector() : Vector2.right;
        if (rushDir == Vector2.zero) rushDir = Vector2.right;

        rb.linearVelocity = Vector2.zero;

        timer = chargeTime;
        state = SharkState.Charging;
        soundPlayer.Play("SharkCharge", 1f);
    }

    private void StartRush()
    {
        // begin the actual rush
        state = SharkState.Rushing;
        rb.linearVelocity = rushDir * rushSpeed;
        soundPlayer.Play("SharkCast", 1f);
        soundPlayer.PlayLoop("SharkLoop", 1f);
    }

    private void StopRushAndRecover()
    {
        if (state != SharkState.Rushing) return;

        rb.linearVelocity = Vector2.zero;

        timer = recoverDelay;
        state = SharkState.Recovering;
        soundPlayer.StopLoop();
        soundPlayer.Play("SharkCollide", 1f);
    }

    private void EndRecover()
    {
        // unlock controls after delay
        if (movement != null) movement.SetControlsLocked(false);

        state = SharkState.Idle;
    }

    // Stop when colliding with something solid
    private void OnCollisionEnter2D(Collision2D collision)
    {
        StopRushAndRecover();
    }

    // Optional: if you use triggers for walls/hitboxes, also stop on trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        StopRushAndRecover();
    }
}
