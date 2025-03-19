using UnityEngine;

[RequireComponent(typeof(Light))]
public class SunShadowCaster : MonoBehaviour
{
    public Transform camToTrack;
    public bool trackCamera = true;

    void LateUpdate()
    {
        if (trackCamera && camToTrack != null )
        {
            transform.LookAt(-camToTrack.position);
        }
    }
}