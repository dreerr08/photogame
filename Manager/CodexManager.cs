// CodexManager.cs

using System.Collections.Generic;
using UnityEngine;

public class CodexManager : MonoBehaviour
{
    // Padrão Singleton para acesso fácil de qualquer script
    public static CodexManager instance;

    // Dicionário para mapear o ID da espécie ao nome dado pelo jogador
    private Dictionary<string, string> discoveredSpecies = new Dictionary<string, string>();

    void Awake()
    {
        // Configuração do Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Opcional: mantém o códice entre cenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Verifica se uma espécie já foi descoberta.
    /// </summary>
    public bool IsSpeciesDiscovered(string speciesID)
    {
        return discoveredSpecies.ContainsKey(speciesID);
    }

    /// <summary>
    /// Regista uma nova descoberta com o nome dado pelo jogador.
    /// </summary>
    public void RegisterNewDiscovery(string speciesID, string playerName)
    {
        if (!IsSpeciesDiscovered(speciesID))
        {
            discoveredSpecies.Add(speciesID, playerName);
            Debug.Log($"Nova espécie descoberta e registada! ID: {speciesID}, Nome: {playerName}");
        }
    }

    /// <summary>
    /// Obtém o nome que o jogador deu a uma espécie.
    /// </summary>
    public string GetPlayerNameForSpecies(string speciesID)
    {
        if (IsSpeciesDiscovered(speciesID))
        {
            return discoveredSpecies[speciesID];
        }
        return "Desconhecido";
    }
}