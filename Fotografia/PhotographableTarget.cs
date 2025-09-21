// PhotographableTarget.cs

using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PhotographableTarget : MonoBehaviour
{
    [Header("Dados da Descoberta")]
    [Tooltip("ID �nico para este TIPO de alvo. Ex: 'Capivara_Comum'. V�rios objetos podem partilhar o mesmo ID.")]
    public string speciesID = "ID_da_Especie_Nao_Definido";

    [Tooltip("A descri��o que aparece na tela de nomea��o para ajudar o jogador.")]
    [TextArea(3, 5)]
    public string descriptionForNaming = "Descreva aqui o alvo para o jogador.";

    [Tooltip("Defina a categoria deste alvo para o sistema de descobertas.")]
    public DiscoveryCategory category = DiscoveryCategory.SeresVivos;

    [Tooltip("A dist�ncia m�xima em que este alvo espec�fico pode ser fotografado com sucesso.")]
    public float maxPhotographyDistance = 50f;

    [Header("Feedback de Proximidade (�udio)")]
    public AudioClip proximitySound;
    public float soundTriggerDistance = 20f;

    private AudioSource audioSource;
    private Transform playerTransform;
    private bool wasInRange = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1.0f;
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null) { playerTransform = player.transform; }
        else { Debug.LogError("N�o foi poss�vel encontrar o PlayerController na cena!", this.gameObject); }
    }

    void Update()
    {
        // CONDI��O ADICIONADA: Se o jogador ou o som n�o existem, ou se a esp�cie J� FOI DESCOBERTA, n�o faz mais nada.
        if (playerTransform == null || proximitySound == null || CodexManager.instance.IsSpeciesDiscovered(speciesID))
        {
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        bool isInRange = (distanceToPlayer <= soundTriggerDistance);

        // Esta l�gica agora s� ser� executada para esp�cies n�o descobertas
        if (isInRange && !wasInRange)
        {
            audioSource.PlayOneShot(proximitySound);
        }
        wasInRange = isInRange;
    }
}