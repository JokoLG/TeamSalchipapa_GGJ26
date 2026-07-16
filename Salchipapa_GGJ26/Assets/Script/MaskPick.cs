using UnityEngine;
public enum MaskType { Witch, Ody, Shark }

public class MaskPick : MonoBehaviour
{

    public MaskType type;
    public SFX_Manager sfx;
    public BGM_Manager bgm;

    void Awake()
    {
        sfx = FindObjectOfType<SFX_Manager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name != "Player") return;

        P_Movement player = other.GetComponent<P_Movement>();
        if (player == null) return;

        switch (type)
        {
            case MaskType.Witch:
                player.hasWitchMask = true;
                player.weapon = MaskWeapon.Fireball;
                player.UpdateMaskWeapon();
                sfx?.Play("Obt_Witch");
                bgm.PlayBGM("witch");
                break;

            case MaskType.Ody:
                player.hasOdyMask = true;
                player.weapon = MaskWeapon.Sword;
                player.UpdateMaskWeapon();
                sfx?.Play("Obt_Ody");
                bgm.PlayBGM("ody");
                break;

            case MaskType.Shark:
                player.hasSharkMask = true;
                player.weapon = MaskWeapon.Shark;
                player.UpdateMaskWeapon();
                FindObjectOfType<SFX_Manager>()?.Play("Obt_Shark");
                FindObjectOfType<BGM_Manager>()?.PlayBGM("shark");
                break;
        }

        Destroy(gameObject); 
    }
}