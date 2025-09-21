// PhotoData.cs

using UnityEngine;

[System.Serializable]
public class PhotoData
{
    public Texture2D PhotoTexture { get; private set; }
    public bool IsDiscovery { get; private set; }
    public string DiscoveryName { get; private set; }
    public DiscoveryCategory Category { get; private set; }
    public int InstanceID { get; private set; } // NOVO CAMPO

    // Construtor atualizado para incluir o InstanceID
    public PhotoData(Texture2D photoTexture, bool isDiscovery, string discoveryName, DiscoveryCategory category, int instanceID)
    {
        this.PhotoTexture = photoTexture;
        this.IsDiscovery = isDiscovery;
        this.DiscoveryName = discoveryName;
        this.Category = category;
        this.InstanceID = instanceID; // NOVO CAMPO
    }
}