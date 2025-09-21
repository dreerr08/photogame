using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NamingUI : MonoBehaviour
{
    [Header("Referências da UI")]
    public GameObject panelObject;
    public TextMeshProUGUI descriptionText;
    public TMP_InputField nameInputField;
    public Button confirmButton;

    private System.Action<string> onNameConfirmed;
    private PlayerController playerController;

    void Awake()
    {
        confirmButton.onClick.AddListener(OnConfirm);
        panelObject.SetActive(false);

        playerController = FindObjectOfType<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("NamingUI não conseguiu encontrar o PlayerController na cena!");
        }
    }

    public void ShowNamingScreen(string description, System.Action<string> callback)
    {
        playerController.ChangeState(PlayerController.PlayerState.InUI);

        this.onNameConfirmed = callback;
        descriptionText.text = description;
        nameInputField.text = "";
        panelObject.SetActive(true);
    }

    private void OnConfirm()
    {
        string chosenName = nameInputField.text;
        if (string.IsNullOrWhiteSpace(chosenName))
        {
            return;
        }

        HideNamingScreen();
        onNameConfirmed?.Invoke(chosenName);
    }

    private void HideNamingScreen()
    {
        panelObject.SetActive(false);
        playerController.ChangeState(PlayerController.PlayerState.CameraActive);
    }
}