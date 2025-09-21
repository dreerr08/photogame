using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public enum PlayerState { Exploring, CameraActive, InUI }
    public PlayerState CurrentState { get; private set; }

    private CharacterController characterController;
    private PhotoAlbumUI photoAlbumUI;
    private Camera playerCamera;
    private ThirdPersonCamera thirdPersonCamera;

    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float rotationSpeed = 10f;
    private float currentSpeed;

    [Header("Camera Settings")]
    public float mouseSensitivity = 100f;
    private float xRotation = 0f;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        photoAlbumUI = FindObjectOfType<PhotoAlbumUI>();
        playerCamera = Camera.main;
        if (playerCamera != null)
        {
            thirdPersonCamera = playerCamera.GetComponent<ThirdPersonCamera>();
        }
        if (photoAlbumUI == null || thirdPersonCamera == null)
        {
            Debug.LogError("Dependências não encontradas! Verifique se PhotoAlbumUI e ThirdPersonCamera estão na cena.");
        }
    }

    void Start()
    {
        ChangeState(PlayerState.Exploring);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (CurrentState != PlayerState.InUI)
            {
                photoAlbumUI.ToggleAlbum();
                ChangeState(PlayerState.InUI);
            }
            else
            {
                photoAlbumUI.ToggleAlbum();
                ChangeState(PlayerState.Exploring);
            }
        }

        if (Input.GetKeyDown(KeyCode.E) && CurrentState != PlayerState.InUI)
        {
            if (CurrentState == PlayerState.Exploring)
            {
                Vector3 camForward = playerCamera.transform.forward;
                camForward.y = 0;
                camForward.Normalize();

                if (camForward != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(camForward);
                }

                ChangeState(PlayerState.CameraActive);
            }
            else if (CurrentState == PlayerState.CameraActive)
            {
                thirdPersonCamera.SyncOrbitWithFPSTarget();
                ChangeState(PlayerState.Exploring);
            }
        }

        if (CurrentState != PlayerState.InUI)
        {
            HandleMovement();
            HandleCameraAndRotation();
        }
    }

    private void HandleMovement()
    {
        if (currentSpeed <= 0) return;
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 moveInput = new Vector3(horizontal, 0, vertical);
        if (moveInput.magnitude < 0.1f) return;
        Vector3 camForward = playerCamera.transform.forward;
        Vector3 camRight = playerCamera.transform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();
        Vector3 moveDirection = (camForward * vertical + camRight * horizontal).normalized;
        characterController.Move(moveDirection * currentSpeed * Time.deltaTime);
        if (CurrentState == PlayerState.Exploring)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void HandleCameraAndRotation()
    {
        if (CurrentState == PlayerState.Exploring) return;
        if (CurrentState == PlayerState.CameraActive)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
            transform.Rotate(Vector3.up * mouseX);
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }

    public void ChangeState(PlayerState newState)
    {
        CurrentState = newState;
        switch (CurrentState)
        {
            case PlayerState.Exploring:
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                currentSpeed = walkSpeed;
                thirdPersonCamera.SwitchView(false);
                break;
            case PlayerState.CameraActive:
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                currentSpeed = 0f;
                thirdPersonCamera.SwitchView(true);
                xRotation = 0;
                break;
            case PlayerState.InUI:
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
        }
    }
}