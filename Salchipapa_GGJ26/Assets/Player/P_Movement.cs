using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEngine;

public enum FacingDirection { Up, Down, Left, Right }
public enum MaskWeapon { None, Sword, Fireball, Shark }

[RequireComponent(typeof(Rigidbody2D))]
public class P_Movement : MonoBehaviour
{
    [Header("Sound Manager")]
    public P_SoundHandler soundPlayer;

    public Transform weaponPivot;

    public MaskWeapon weapon = MaskWeapon.None;

    public bool hasWitchMask = false;
    public bool hasSharkMask = false;
    public bool hasOdyMask = false;

    [Header("Weapon References")]
    public P_Sword sword;
    public FBSpawner fireball;
    public P_SharkRush sharkRush;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public bool isMoving = false;

    public FacingDirection facing = FacingDirection.Right;

    [Header("Locks")]
    public bool controlsLocked = false;

    private Rigidbody2D rb;
    private Vector2 movement;

    private FacingDirection lastPressed = FacingDirection.Right;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // If locked: no input updates, no attacks, keep facing/pivot as-is
        if (controlsLocked)
        {
            movement = Vector2.zero;
            isMoving = false;

            UpdateWeaponPivot();
            if (sword != null) sword.SetFacing(facing);
            return;
        }

        // Track last pressed movement key
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) lastPressed = FacingDirection.Right;
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))  lastPressed = FacingDirection.Left;
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))    lastPressed = FacingDirection.Up;
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))  lastPressed = FacingDirection.Down;

        // Decide actual move direction (4-way only)
        FacingDirection? moveDir = GetMoveDirection(lastPressed);

        if (moveDir.HasValue)
        {
            movement = DirToVector(moveDir.Value);
            isMoving = true;
            facing = moveDir.Value; // facing matches motion direction

            if (soundPlayer != null)
                soundPlayer.PlayLoop("Walk");
        }
        else
        {
            movement = Vector2.zero;

            if (isMoving && soundPlayer != null && soundPlayer.srcLoop.isPlaying)
                soundPlayer.StopLoop();

            isMoving = false;
        }

        UpdateWeaponPivot();
        if (sword != null) sword.SetFacing(facing);

        switch (weapon)
        {
            case MaskWeapon.Sword:
                sword.isActive = true;
                fireball.isActive = false;
                sharkRush.isActive = false;
                if (Input.GetKeyDown(KeyCode.J)) sword.TryAttack();
                else UpdateMaskWeapon();
                break;

            case MaskWeapon.Fireball:
                sword.isActive = false;
                fireball.isActive = true;
                sharkRush.isActive = false;
                if (Input.GetKeyDown(KeyCode.K)) fireball.Shoot();
                else UpdateMaskWeapon();
                break;

            case MaskWeapon.Shark:
                sword.isActive = false;
                fireball.isActive = false;
                sharkRush.isActive = true;
                if (sharkRush.state == SharkState.Idle && Input.GetKeyDown(KeyCode.L))
                    sharkRush.StartCharge();
                else UpdateMaskWeapon();
                break;

            default:
                UpdateMaskWeapon();
                break;
        }
    }

    public void UpdateMaskWeapon()
    {
        if (Input.GetKeyDown(KeyCode.J) && hasOdyMask)
        {
            weapon = MaskWeapon.Sword;
            if (soundPlayer != null) soundPlayer.Play("SwitchWeapon");
        }

        if (Input.GetKeyDown(KeyCode.L) && hasSharkMask)
        {
            weapon = MaskWeapon.Shark;
            if (soundPlayer != null) soundPlayer.Play("SwitchWeapon");
        }

        if (Input.GetKeyDown(KeyCode.K) && hasWitchMask)
        {
            weapon = MaskWeapon.Fireball;
            if (soundPlayer != null) soundPlayer.Play("SwitchWeapon");
        }
    }

    void FixedUpdate()
    {
        if (controlsLocked) return;
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    public void SetControlsLocked(bool locked)
    {
        controlsLocked = locked;
        if (locked)
        {
            movement = Vector2.zero;
            isMoving = false;
            rb.linearVelocity = Vector2.zero;
        }
    }

    public Vector2 GetFacingVector()
    {
        return DirToVector(facing);
    }

    FacingDirection? GetMoveDirection(FacingDirection preferred)
    {
        if (IsHeld(preferred)) return preferred;

        if (IsHeld(FacingDirection.Right)) return FacingDirection.Right;
        if (IsHeld(FacingDirection.Left))  return FacingDirection.Left;
        if (IsHeld(FacingDirection.Up))    return FacingDirection.Up;
        if (IsHeld(FacingDirection.Down))  return FacingDirection.Down;

        return null;
    }

    bool IsHeld(FacingDirection dir)
    {
        switch (dir)
        {
            case FacingDirection.Right: return Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
            case FacingDirection.Left:  return Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
            case FacingDirection.Up:    return Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
            case FacingDirection.Down:  return Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
            default: return false;
        }
    }

    Vector2 DirToVector(FacingDirection dir)
    {
        switch (dir)
        {
            case FacingDirection.Right: return Vector2.right;
            case FacingDirection.Left:  return Vector2.left;
            case FacingDirection.Up:    return Vector2.up;
            case FacingDirection.Down:  return Vector2.down;
            default: return Vector2.zero;
        }
    }

    void UpdateWeaponPivot()
    {
        if (weaponPivot == null) return;

        switch (facing)
        {
            case FacingDirection.Right: weaponPivot.localRotation = Quaternion.Euler(0, 0, 0); break;
            case FacingDirection.Up:    weaponPivot.localRotation = Quaternion.Euler(0, 0, 90); break;
            case FacingDirection.Left:  weaponPivot.localRotation = Quaternion.Euler(0, 0, 180); break;
            case FacingDirection.Down:  weaponPivot.localRotation = Quaternion.Euler(0, 0, 270); break;
        }
    }
}