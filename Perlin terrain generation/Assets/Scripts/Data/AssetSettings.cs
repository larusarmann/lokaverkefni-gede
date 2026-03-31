using UnityEngine;

[System.Serializable]
public class AssetSettings
{
    public GameObject[] prefabs;
    [Range(0, 100)]
    public int density = 10;
    
    [Header("Height Constraints")]
    [Range(0, 1)]
    public float minHeight = 0.3f;  // Set this to ~0.3 to stay out of water
    [Range(0, 1)]
    public float maxHeight = 1.0f;

    [Header("Size")]
    public float minScale = 1f;
    public float maxScale = 5f;     // For "bigga" trees
}