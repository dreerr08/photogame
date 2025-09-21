// PhotographyManager.cs

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PhotographyManager : MonoBehaviour
{
    // --- Referências e variáveis ---
    private PlayerController playerController;
    private PhotoStorageManager photoStorageManager;
    private Camera playerCamera;
    private AudioSource playerAudioSource;
    private NamingUI namingUI;

    [Header("Feedback de Captura (Áudio)")]
    public AudioClip shutterSound;
    public AudioClip newDiscoverySound;

    [Header("Efeito de Obturador")]
    public Image shutterEffectUI;
    public float shutterCloseDuration = 0.1f;
    public float shutterOpenDuration = 0.3f;

    [Header("Lógica de Descoberta")]
    public float newDiscoveryDelay = 1.0f;

    // --- VARIÁVEL DE DISTÂNCIA GLOBAL REMOVIDA DAQUI ---
    // public float maxPhotographyDistance = 50f; // REMOVIDO

    private bool canTakePhoto = true;

    // O método Start() permanece o mesmo
    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        photoStorageManager = FindObjectOfType<PhotoStorageManager>();
        playerCamera = Camera.main;
        namingUI = FindObjectOfType<NamingUI>();
        if (playerController != null) { playerAudioSource = playerController.GetComponent<AudioSource>(); }
        if (namingUI == null) { Debug.LogError("PhotographyManager: NamingUI não encontrada na cena!"); }
        if (shutterEffectUI != null)
        {
            Color tempColor = shutterEffectUI.color;
            tempColor.a = 0f;
            shutterEffectUI.color = tempColor;
        }
    }

    // O método Update() permanece o mesmo
    void Update()
    {
        if (playerController.CurrentState == PlayerController.PlayerState.CameraActive
            && Input.GetMouseButtonDown(0)
            && canTakePhoto)
        {
            StartCoroutine(CaptureAndAnimateRoutine());
        }
    }

    // O método CaptureAndAnimateRoutine() permanece o mesmo
    private IEnumerator CaptureAndAnimateRoutine()
    {
        canTakePhoto = false;
        yield return new WaitForEndOfFrame();
        Texture2D screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenshot.Apply();
        PhotographableTarget discoveredTarget = IdentifyTargetInView();
        bool isNewDiscovery = discoveredTarget != null && !CodexManager.instance.IsSpeciesDiscovered(discoveredTarget.speciesID);
        if (playerAudioSource != null)
        {
            if (shutterSound != null) { playerAudioSource.PlayOneShot(shutterSound); }
            if (isNewDiscovery && newDiscoverySound != null) { playerAudioSource.PlayOneShot(newDiscoverySound); }
        }
        yield return AnimateShutter(0f, 1f, shutterCloseDuration);
        yield return AnimateShutter(1f, 0f, shutterOpenDuration);
        if (discoveredTarget != null)
        {
            string speciesID = discoveredTarget.speciesID;
            int instanceID = discoveredTarget.gameObject.GetInstanceID();
            PhotoData newPhotoData = new PhotoData(screenshot, true, discoveredTarget.gameObject.name, discoveredTarget.category, instanceID);
            if (isNewDiscovery)
            {
                StartCoroutine(ShowNamingUIAfterDelay(discoveredTarget, newPhotoData));
            }
            else
            {
                photoStorageManager.AddPhoto(newPhotoData);
                canTakePhoto = true;
            }
        }
        else
        {
            Destroy(screenshot);
            canTakePhoto = true;
        }
    }

    // O método ShowNamingUIAfterDelay() permanece o mesmo
    private IEnumerator ShowNamingUIAfterDelay(PhotographableTarget target, PhotoData data)
    {
        yield return new WaitForSeconds(newDiscoveryDelay);
        namingUI.ShowNamingScreen(target.descriptionForNaming, (playerName) => {
            CodexManager.instance.RegisterNewDiscovery(target.speciesID, playerName);
            photoStorageManager.AddPhoto(data);
            canTakePhoto = true;
        });
    }

    // O método AnimateShutter() permanece o mesmo
    private IEnumerator AnimateShutter(float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        Color shutterColor = shutterEffectUI.color;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            shutterColor.a = newAlpha;
            shutterEffectUI.color = shutterColor;
            yield return null;
        }
        shutterColor.a = endAlpha;
        shutterEffectUI.color = shutterColor;
    }

    // --- MÉTODO ATUALIZADO PARA USAR A DISTÂNCIA DO ALVO ---
    private PhotographableTarget IdentifyTargetInView()
    {
        PhotographableTarget closestTarget = null;
        float minDistance = float.MaxValue;

        PhotographableTarget[] allTargets = FindObjectsOfType<PhotographableTarget>();

        foreach (var target in allTargets)
        {
            float distanceToTarget = Vector3.Distance(playerCamera.transform.position, target.transform.position);

            // ALTERADO: Agora usa a distância máxima do próprio alvo
            if (distanceToTarget > target.maxPhotographyDistance)
            {
                continue;
            }

            Vector3 viewportPos = playerCamera.WorldToViewportPoint(target.transform.position);
            bool isInViewport = viewportPos.x > 0 && viewportPos.x < 1 && viewportPos.y > 0 && viewportPos.y < 1 && viewportPos.z > 0;

            if (isInViewport)
            {
                Vector3 directionToTarget = target.transform.position - playerCamera.transform.position;

                // ALTERADO: O Raycast também usa a distância máxima do alvo
                if (Physics.Raycast(playerCamera.transform.position, directionToTarget, out RaycastHit hit, target.maxPhotographyDistance))
                {
                    if (hit.transform == target.transform)
                    {
                        if (distanceToTarget < minDistance)
                        {
                            minDistance = distanceToTarget;
                            closestTarget = target;
                        }
                    }
                }
            }
        }

        return closestTarget;
    }
}