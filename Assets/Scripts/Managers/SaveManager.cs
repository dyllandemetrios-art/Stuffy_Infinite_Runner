using UnityEngine;
using System.IO;

/// <summary>
/// Gestionnaire de sauvegarde du jeu Robot Clean.
/// Gère la lecture et l'écriture des données de progression
/// dans un fichier JSON sur le disque du joueur.
/// 
/// Ce script utilise le pattern Singleton avec DontDestroyOnLoad
/// pour persister entre toutes les scènes du jeu.
/// 
/// Utilisation depuis n'importe quel script :
/// SaveManager.Instance.data.composants += 1;
/// SaveManager.Instance.Sauvegarder();
/// </summary>
public class SaveManager : MonoBehaviour
{
    // =============================================
    // SINGLETON
    // =============================================

    /// <summary>
    /// Instance unique accessible depuis n'importe quel script.
    /// </summary>
    public static SaveManager Instance;

    // =============================================
    // DONNEES
    // =============================================

    /// <summary>
    /// Données de sauvegarde du joueur.
    /// Accessible en lecture/écriture depuis tous les scripts.
    /// </summary>
    public GameData data = new GameData();

    /// <summary>
    /// Chemin complet vers le fichier de sauvegarde JSON.
    /// Application.persistentDataPath est géré automatiquement par Unity
    /// selon le système d'exploitation.
    /// </summary>
    private string savePath;

    // =============================================
    // INITIALISATION
    // =============================================

    /// <summary>
    /// Initialisation du Singleton et chargement de la sauvegarde.
    /// Si une instance existe déjà, ce GameObject est détruit.
    /// </summary>
    private void Awake()
    {
        // Mise en place du Singleton
        if (Instance == null)
        {
            Instance = this;

            // Survit aux changements de scène
            DontDestroyOnLoad(gameObject);

            // Définition du chemin de sauvegarde
            savePath = Application.persistentDataPath + "/savegame.json";

            // Chargement automatique au démarrage
            Charger();
        }
        else
        {
            // Une instance existe déjà, on détruit le doublon
            Destroy(gameObject);
        }
    }

    // =============================================
    // SAUVEGARDE
    // =============================================

    /// <summary>
    /// Sérialise les données du joueur en JSON et les écrit sur le disque.
    /// À appeler à chaque fin de run (Game Over) et lors des achats
    /// de compétences dans l'écran de progression.
    /// </summary>
    public void Sauvegarder()
    {
        // Conversion de GameData en JSON formaté (true = lisible)
        string json = JsonUtility.ToJson(data, true);

        // Écriture dans le fichier
        File.WriteAllText(savePath, json);

        Debug.Log("[SaveManager] Sauvegarde réussie : " + savePath);
    }

    // =============================================
    // CHARGEMENT
    // =============================================

    /// <summary>
    /// Lit le fichier JSON et désérialise les données dans GameData.
    /// Si aucun fichier n'existe, une nouvelle GameData vide est utilisée
    /// (première partie du joueur).
    /// </summary>
    public void Charger()
    {
        if (File.Exists(savePath))
        {
            // Lecture du fichier JSON
            string json = File.ReadAllText(savePath);

            // Conversion du JSON en objet GameData
            data = JsonUtility.FromJson<GameData>(json);

            Debug.Log("[SaveManager] Chargement réussi");
        }
        else
        {
            // Première lancement : pas de fichier, on part sur des valeurs vides
            data = new GameData();
            Debug.Log("[SaveManager] Aucune sauvegarde trouvée, nouvelle partie");
        }
    }

    // =============================================
    // UTILITAIRES
    // =============================================

    /// <summary>
    /// Remet toutes les données à zéro et écrase la sauvegarde.
    /// À utiliser uniquement si tu veux ajouter un bouton
    /// "Réinitialiser la progression" dans les options.
    /// </summary>
    public void ResetSauvegarde()
    {
        data = new GameData();
        Sauvegarder();
        Debug.Log("[SaveManager] Sauvegarde réinitialisée");
    }

    /// <summary>
    /// Met à jour le meilleur score si le score actuel le dépasse.
    /// À appeler à chaque Game Over.
    /// </summary>
    /// <param name="scoreActuel">Score obtenu durant la run qui vient de se terminer</param>
    /// <param name="distanceActuelle">Distance parcourue durant la run</param>
    public void MettreAJourRecord(int scoreActuel, float distanceActuelle)
    {
        bool nouveauRecord = false;

        if (scoreActuel > data.scoreMax)
        {
            data.scoreMax = scoreActuel;
            nouveauRecord = true;
        }

        if (distanceActuelle > data.meilleureDistance)
        {
            data.meilleureDistance = distanceActuelle;
            nouveauRecord = true;
        }

        if (nouveauRecord)
        {
            Debug.Log("[SaveManager] Nouveau record enregistré !");
        }

        // Sauvegarde dans tous les cas à la fin d'une run
        Sauvegarder();
    }
}