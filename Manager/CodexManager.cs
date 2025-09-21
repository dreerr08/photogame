// CodexManager.cs

using System.Collections.Generic;
using UnityEngine;

public class CodexManager : MonoBehaviour
{
    // Padr�o Singleton para acesso f�cil de qualquer script
    public static CodexManager instance;

    // Dicion�rio para mapear o ID da esp�cie ao nome dado pelo jogador
    private Dictionary<string, string> discoveredSpecies = new Dictionary<string, string>();

    void Awake()
    {
        // Configura��o do Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Opcional: mant�m o c�dice entre cenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Verifica se uma esp�cie j� foi descoberta.
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
            Debug.Log($"Nova esp�cie descoberta e registada! ID: {speciesID}, Nome: {playerName}");
        }
    }

    /// <summary>
    /// Obt�m o nome que o jogador deu a uma esp�cie.
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