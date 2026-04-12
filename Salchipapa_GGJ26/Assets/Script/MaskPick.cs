using UnityEngine;
public enum MaskType { Witch, Ody, Shark }

public class MaskPick : MonoBehaviour
{

        public MaskType type;
        public SFX_Manager sfx;

    private void OnTriggerEnter2D(Collider2D other)
        {
        if (other.gameObject.name != "Player") return;
        void Awake()
        {
            sfx = FindObjectOfType<SFX_Manager>();
        }

        P_Movement player = other.GetComponent<P_Movement>();
            if (player == null) return;

            switch (type)
            {
                case MaskType.Witch:
                    player.hasWitchMask = true;
                    sfx?.Play("Obt_Witch");
                break;

                case MaskType.Ody:
                    player.hasOdyMask = true;
                    sfx?.Play("Obt_Ody");
                break;

                case MaskType.Shark:
                    player.hasSharkMask = true;
                FindObjectOfType<SFX_Manager>()?.Play("Obt_Shark");
                break;
            }

            Destroy(gameObject); 
        }
    }

