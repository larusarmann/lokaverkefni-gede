using UnityEngine;

public class WaterPlaneFollow : MonoBehaviour
{
    public Transform player;
    public float waterLevel = 5f;

    void LateUpdate()
    {
        if (player != null)
        {
            // Lock the Y axis to the water level, but follow the player on X and Z
            transform.position = new Vector3(player.position.x, waterLevel, player.position.z);
        }
    }
}