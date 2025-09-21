using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Referências")]
    public Transform playerTarget;
    public Transform firstPersonPoint;
    public SkinnedMeshRenderer playerMeshRenderer;

    [Header("Configurações de Terceira Pessoa")]
    public float distance = 5.0f;
    public float mouseSensitivity = 200f;
    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;

    private float rotationX = 0.0f;
    private float rotationY = 0.0f;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        rotationY = angles.y;
        rotationX = angles.x;
    }

    void LateUpdate()
    {
        if (transform.parent == null && playerTarget)
        {
            rotationY += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            rotationX -= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
            rotationX = Mathf.Clamp(rotationX, yMinLimit, yMaxLimit);
            Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0);
            Vector3 desiredPosition = playerTarget.position - (rotation * Vector3.forward * distance);
            transform.rotation = rotation;
            transform.position = desiredPosition;
        }
    }

    public void SwitchView(bool toFirstPerson)
    {
        if (toFirstPerson)
        {
            transform.SetParent(firstPersonPoint);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
        else
        {
            transform.SetParent(null);
            LateUpdate();
        }

        if (playerMeshRenderer != null)
        {
            playerMeshRenderer.enabled = !toFirstPerson;
        }
    }

    public void SyncOrbitWithFPSTarget()
    {
        rotationY = playerTarget.eulerAngles.y;
        rotationX = transform.localEulerAngles.x;
        if (rotationX > 180f)
        {
            rotationX -= 360f;
        }
    }
}