using UnityEngine;
using StarterAssets; // Allows us to access the ThirdPersonController

public class WaterSystem : MonoBehaviour
{
    [Header("Water Settings")]
    [Tooltip("Set this to the exact Y height where your blue terrain starts.")]
    public float waterLevel = 15f;

    [Tooltip("Multiplier for speed. 0.3 means a 70% reduction in speed.")]
    public float speedMultiplier = 0.3f;

    [Header("Underwater Visuals")]
    public Color underwaterColor = new Color(0f, 0.4f, 0.7f, 0.6f);
    public float underwaterFogDensity = 0.15f;

    private ThirdPersonController tpc;
    private Camera mainCamera;

    // Cache original settings so we can restore them when leaving the water
    private float originalMoveSpeed;
    private float originalSprintSpeed;

    private bool defaultFogEnabled;
    private Color defaultFogColor;
    private float defaultFogDensity;

    private bool isPlayerUnderwater = false;
    private bool isCameraUnderwater = false;

    void Start()
    {
        tpc = GetComponent<ThirdPersonController>();
        mainCamera = Camera.main;

        // Save the original speeds set in the inspector
        if (tpc != null)
        {
            originalMoveSpeed = tpc.MoveSpeed;
            originalSprintSpeed = tpc.SprintSpeed;
        }

        // Save the environment's default fog settings
        defaultFogEnabled = RenderSettings.fog;
        defaultFogColor = RenderSettings.fogColor;
        defaultFogDensity = RenderSettings.fogDensity;
    }

    void Update()
    {
        CheckPlayerUnderwater();
        CheckCameraUnderwater();
    }

    void CheckPlayerUnderwater()
    {
        // Check if the player's pivot (usually at their feet) is below the water line
        bool currentlyUnderwater = transform.position.y < waterLevel;

        if (currentlyUnderwater && !isPlayerUnderwater)
        {
            isPlayerUnderwater = true;
            if (tpc != null)
            {
                // Slow down the player
                tpc.MoveSpeed = originalMoveSpeed * speedMultiplier;
                tpc.SprintSpeed = originalSprintSpeed * speedMultiplier;
            }
        }
        else if (!currentlyUnderwater && isPlayerUnderwater)
        {
            isPlayerUnderwater = false;
            if (tpc != null)
            {
                // Restore original speed
                tpc.MoveSpeed = originalMoveSpeed;
                tpc.SprintSpeed = originalSprintSpeed;
            }
        }
    }

    void CheckCameraUnderwater()
    {
        if (mainCamera == null) return;

        // Check if the camera itself dips below the water surface
        bool currentlyUnderwater = mainCamera.transform.position.y < waterLevel;

        if (currentlyUnderwater && !isCameraUnderwater)
        {
            isCameraUnderwater = true;

            // Turn on heavy blue fog to simulate being underwater
            RenderSettings.fog = true;
            RenderSettings.fogColor = underwaterColor;
            RenderSettings.fogDensity = underwaterFogDensity;
        }
        else if (!currentlyUnderwater && isCameraUnderwater)
        {
            isCameraUnderwater = false;

            // Restore normal air visuals
            RenderSettings.fog = defaultFogEnabled;
            RenderSettings.fogColor = defaultFogColor;
            RenderSettings.fogDensity = defaultFogDensity;
        }
    }
}