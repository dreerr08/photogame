// PhotoStorageManager.cs

using System.Collections.Generic;
using UnityEngine;
using System.Linq; // Necess�rio para usar FindIndex

public class PhotoStorageManager : MonoBehaviour
{
    public List<PhotoData> StoredPhotos { get; private set; } = new List<PhotoData>();

    /// <summary>
    /// Adiciona uma nova foto ao �lbum. Se j� existir uma foto do mesmo
    /// objeto (mesmo InstanceID), a antiga � substitu�da.
    /// </summary>
    public void AddPhoto(PhotoData newPhotoData)
    {
        // Procura na lista se j� existe uma foto com o mesmo InstanceID
        int existingIndex = StoredPhotos.FindIndex(photo => photo.InstanceID == newPhotoData.InstanceID);

        if (existingIndex != -1)
        {
            // Encontrou uma foto existente: substitui-a
            StoredPhotos[existingIndex] = newPhotoData;
            Debug.Log($"Foto do objeto {newPhotoData.DiscoveryName} (ID: {newPhotoData.InstanceID}) foi ATUALIZADA.");
        }
        else
        {
            // N�o encontrou: adiciona a nova foto
            StoredPhotos.Add(newPhotoData);
            Debug.Log($"Nova foto do objeto {newPhotoData.DiscoveryName} (ID: {newPhotoData.InstanceID}) foi ADICIONADA.");
        }
    }
}