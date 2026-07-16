using UnityEngine;
using UnityEngine.UI;

public class P_MaskUI : MonoBehaviour
{
    [Header("Player Reference")]
    [SerializeField] private P_Movement player;

    [Header("Mask UI Objects")]
    [SerializeField] private GameObject odyMaskUI;
    [SerializeField] private GameObject witchMaskUI;
    [SerializeField] private GameObject sharkMaskUI;

    [Header("Icon Colors")]
    [SerializeField] private Color activeColor = Color.white;

    [SerializeField]
    private Color inactiveColor = new Color(
        0.45f,
        0.45f,
        0.45f,
        1f
    );

    private Image odyMaskImage;
    private Image witchMaskImage;
    private Image sharkMaskImage;

    private void Awake()
    {
        // Find the Image component on each object or its children.
        odyMaskImage = GetImage(odyMaskUI);
        witchMaskImage = GetImage(witchMaskUI);
        sharkMaskImage = GetImage(sharkMaskUI);

        // Hidden by default until the player owns the masks.
        if (odyMaskUI != null)
            odyMaskUI.SetActive(false);

        if (witchMaskUI != null)
            witchMaskUI.SetActive(false);

        if (sharkMaskUI != null)
            sharkMaskUI.SetActive(false);
    }

    private void Update()
    {
        if (player == null)
            return;

        UpdateMaskIcon(
            odyMaskUI,
            odyMaskImage,
            player.hasOdyMask,
            player.weapon == MaskWeapon.Sword
        );

        UpdateMaskIcon(
            witchMaskUI,
            witchMaskImage,
            player.hasWitchMask,
            player.weapon == MaskWeapon.Fireball
        );

        UpdateMaskIcon(
            sharkMaskUI,
            sharkMaskImage,
            player.hasSharkMask,
            player.weapon == MaskWeapon.Shark
        );
    }

    private void UpdateMaskIcon(
        GameObject maskObject,
        Image maskImage,
        bool hasMask,
        bool isEquipped
    )
    {
        if (maskObject == null)
            return;

        // Show the icon only when the player has acquired the mask.
        if (maskObject.activeSelf != hasMask)
            maskObject.SetActive(hasMask);

        if (!hasMask || maskImage == null)
            return;

        // Normal color when equipped, dark gray when inactive.
        maskImage.color = isEquipped ? activeColor : inactiveColor;
    }

    private Image GetImage(GameObject target)
    {
        if (target == null)
            return null;

        Image image = target.GetComponent<Image>();

        if (image == null)
            image = target.GetComponentInChildren<Image>(true);

        return image;
    }
}