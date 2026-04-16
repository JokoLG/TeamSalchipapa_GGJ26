using UnityEngine;

public class noRotate : MonoBehaviour
{
    void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}
