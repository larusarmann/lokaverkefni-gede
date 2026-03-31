using UnityEngine;

[System.Serializable]
public class AssetSettings
{
    public GameObject[] prefabs;
    [Range(0, 100)]
    public int density = 10;
    
    [Header("Height Constraints")]
    [Range(0, 1)]
    public float minHeight = 0.3f; 
    [Range(0, 1)]
    public float maxHeight = 1.0f;

    [Header("Size Settings")]
    public float minScale = 0.8f;
    public float maxScale = 2.5f; // Set this to ~2 or 3 to avoid the "goofy" look
}