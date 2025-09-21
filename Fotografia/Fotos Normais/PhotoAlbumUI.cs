using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PhotoAlbumUI : MonoBehaviour
{
    public GameObject albumPanel;
    public Transform photoGridContainer;
    public GameObject photoThumbnailPrefab;

    private PhotoStorageManager photoStorageManager;
    private bool isAlbumOpen = false;

    void Start()
    {
        photoStorageManager = FindObjectOfType<PhotoStorageManager>();
        albumPanel.SetActive(false);
    }

    public void ToggleAlbum()
    {
        isAlbumOpen = !isAlbumOpen;
        albumPanel.SetActive(isAlbumOpen);

        if (isAlbumOpen)
        {
            RefreshPhotoGrid();
        }
    }

    void RefreshPhotoGrid()
    {
        foreach (Transform child in photoGridContainer)
        {
            Destroy(child.gameObject);
        }

        List<PhotoData> photos = photoStorageManager.StoredPhotos;

        foreach (PhotoData photo in photos)
        {
            GameObject thumbnailGO = Instantiate(photoThumbnailPrefab, photoGridContainer);
            Image thumbnailImage = thumbnailGO.GetComponent<Image>();

            if (thumbnailImage != null)
            {
                Sprite photoSprite = Sprite.Create(photo.PhotoTexture, new Rect(0, 0, photo.PhotoTexture.width, photo.PhotoTexture.height), new Vector2(0.5f, 0.5f));
                thumbnailImage.sprite = photoSprite;
            }
        }
    }
}