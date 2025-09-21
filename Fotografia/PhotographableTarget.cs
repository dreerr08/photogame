// PhotographableTarget.cs

using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PhotographableTarget : MonoBehaviour
{
    [Header("Dados da Descoberta")]
    [Tooltip("ID único para este TIPO de alvo. Ex: 'Capivara_Comum'. Vários objetos podem partilhar o mesmo ID.")]
    public string speciesID = "ID_da_Especie_Nao_Definido";

    [Tooltip("A descrição que aparece na tela de nomeação para ajudar o jogador.")]
    [TextArea(3, 5)]
    public string descriptionForNaming = "Descreva aqui o alvo para o jogador.";

    [Tooltip("Defina a categoria deste alvo para o sistema de descobertas.")]
    public DiscoveryCategory category = DiscoveryCategory.SeresVivos;

    [Tooltip("A distância máxima em que este alvo específico pode ser fotografado com sucesso.")]
    public float maxPhotographyDistance = 50f;

    [Header("Feedback de Proximidade (Áudio)")]
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
        else { Debug.LogError("Não foi possível encontrar o PlayerController na cena!", this.gameObject); }
    }

    void Update()
    {
        // CONDIÇÃO ADICIONADA: Se o jogador ou o som não existem, ou se a espécie JÁ FOI DESCOBERTA, não faz mais nada.
        if (playerTransform == null || proximitySound == null || CodexManager.instance.IsSpeciesDiscovered(speciesID))
        {
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        bool isInRange = (distanceToPlayer <= soundTriggerDistance);

        // Esta lógica agora só será executada para espécies não descobertas
        if (isInRange && !wasInRange)
        {
            audioSource.PlayOneShot(proximitySound);
        }
        wasInRange = isInRange;
    }
}