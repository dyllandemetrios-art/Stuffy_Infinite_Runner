/// <summary>
/// Classe de données sérialisable représentant la sauvegarde du joueur.
/// Contient les ressources, statistiques et niveaux de compétences
/// définis dans le Game Design Document (pages 27-28).
/// Cette classe est convertie en JSON pour la persistance entre les sessions.
/// </summary>
[System.Serializable]
public class GameData
{
    // =============================================
    // RESSOURCES
    // =============================================

    /// <summary>
    /// Nombre total de composants électroniques collectés.
    /// Utilisés comme monnaie pour acheter/améliorer les compétences.
    /// Gain : +1 par composant collecté en run (GDD - Mécanique 3.3)
    /// </summary>
    public int composants;

    // =============================================
    // STATISTIQUES DE PERFORMANCE
    // =============================================

    /// <summary>
    /// Meilleure distance atteinte par le joueur en mètres.
    /// Mise à jour uniquement si la distance de la run dépasse ce record.
    /// Références GDD : débutant 300-600m / intermédiaire 800-2000m / avancé 2000-5000m
    /// </summary>
    public float meilleureDistance;

    /// <summary>
    /// Meilleur score atteint par le joueur.
    /// Le score est égal à la distance parcourue en mètres (GDD - Mécanique 7.1)
    /// </summary>
    public int scoreMax;

    // =============================================
    // NIVEAUX DE COMPETENCES DU COEUR ARTIFICIEL
    // Valeurs possibles : 0 (verrouillé), 1, 2, 3
    // (GDD pages 27-28 - Système Compétences)
    // =============================================

    /// <summary>
    /// Niveau de la compétence Renforcement Structurel.
    /// Augmente la santé maximale du robot.
    /// N0 : 100 HP | N1 : 110 HP (coût 10) | N2 : 125 HP (coût 20) | N3 : 150 HP (coût 40)
    /// </summary>
    public int niveauSante;

    /// <summary>
    /// Niveau de la compétence Système de Récupération.
    /// Augmente les HP récupérés lors de la collecte d'un déchet.
    /// N0 : +8 HP | N1 : +10 HP (coût 10) | N2 : +12 HP (coût 20) | N3 : +15 HP (coût 40)
    /// </summary>
    public int niveauSoin;

    /// <summary>
    /// Niveau de la compétence Blindage.
    /// Réduit les dégâts reçus lors des collisions avec les obstacles.
    /// N1 : -10% (coût 15) | N2 : -20% (coût 30) | N3 : -30% (coût 50)
    /// </summary>
    public int niveauBlindage;

    /// <summary>
    /// Niveau de la compétence Stabilisateur.
    /// Augmente la durée d'invincibilité après une collision.
    /// N0 : 1.5s | N1 : 2.0s (coût 10) | N2 : 2.5s (coût 20) | N3 : 3.0s (coût 40)
    /// </summary>
    public int niveauStabilisateur;

    /// <summary>
    /// Niveau de la compétence Optimisation.
    /// Réduit la perte de santé passive au fil du temps.
    /// N0 : -2/s | N1 : -1.8/s (coût 15) | N2 : -1.5/s (coût 30) | N3 : -1.2/s (coût 50)
    /// </summary>
    public int niveauOptimisation;
}