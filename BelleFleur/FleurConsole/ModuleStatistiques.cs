namespace FleurConsole;

using System;
using BelleFleurLib;

public class ModuleStatistiques {
    public ModuleStatistiques() {

    }

    public void Menu() {
        Console.Clear();

        Console.WriteLine("STATISTIQUES");

        Console.WriteLine("0. Retour");
        Console.WriteLine("1. Calcul du prix moyen du bouquet acheté");
        Console.WriteLine("2. Quel est le meilleur client ?");
        Console.WriteLine("3. Quel est le bouquet standard qui a eu le plus de succès ?");
        Console.WriteLine("4. Quel est le magasin qui a généré le plus de chiffre d'affaires ?");
        Console.WriteLine("5. Quelle est la fleur exotique la moins vendue ?");

        int choice = InputsHelper.Int("Choix", 0, 5);

        switch (choice) {
            case 0:
                return;
            case 1:
                PrixMoyenBouquet();
                break;
            case 2:
                MeilleurClient();
                break;
            case 3:
                BouquetStandard();
                break;
            case 4:
                MagasinChiffreAffaire();
                break;
            case 5:
                FleurExotique();
                break;
            default:
                break;
        }

        Console.ReadKey();
        Menu();
    }

    void PrixMoyenBouquet() {
        Console.Clear();

        Console.WriteLine("PRIX MOYEN DU BOUQUET ACHETE");

        var q = DB.GetCommand();
        q.CommandText = @"
            SELECT AVG(bouquet_prix) AS prix_moyen_bouquet
            FROM commande
            JOIN bouquet ON commande.bouquet_id = bouquet.bouquet_id;";
        
        var r = q.ExecuteReader();
        if (r.Read()) Console.WriteLine("Prix moyen du bouquet acheté : " + r["prix_moyen_bouquet"]);
        else Console.WriteLine("Aucun résultat trouvé.");
        Console.WriteLine("Appuyez sur une touche pour continuer.");
    }

    void MeilleurClient() {
        Console.Clear();

        Console.WriteLine("MEILLEUR CLIENT");

        var q = DB.GetCommand();
        q.CommandText = @"
            SELECT client.client_id, client.client_prenom, client.client_nom, COUNT(commande_id) AS nb_commandes
            FROM client
            JOIN commande ON client.client_id = commande.client_id
            WHERE MONTH(commande_date_creation) = MONTH(CURRENT_DATE()) AND YEAR(commande_date_creation) = YEAR(CURRENT_DATE())
            GROUP BY client.client_id
            ORDER BY nb_commandes DESC
            LIMIT 1;";

        var r = q.ExecuteReader();

        if (r.Read()) Console.WriteLine("Meilleur client : " + r["client_nom"] + " " + r["client_prenom"] + " (" + r["nb_commandes"] + " commandes)");
        else Console.WriteLine("Aucun résultat trouvé.");

        Console.WriteLine("Appuyez sur une touche pour continuer.");
    }

    void BouquetStandard() {
        Console.Clear();

        Console.WriteLine("BOUQUET STANDARD LE PLUS VENDU");

        var q = DB.GetCommand();
        q.CommandText = @"
            SELECT bouquet.bouquet_id, bouquet.bouquet_nom, COUNT(commande_id) AS nb_commandes
            FROM bouquet
            JOIN commande ON bouquet.bouquet_id = commande.bouquet_id
            WHERE bouquet.bouquet_categorie = 'Toute occasion'
            GROUP BY bouquet.bouquet_id
            ORDER BY nb_commandes DESC
            LIMIT 1;
            ";

        var r = q.ExecuteReader();

        if (r.Read()) Console.WriteLine("Bouquet standard le plus vendu : " + r["bouquet_nom"] + " (" + r["nb_commandes"] + " commandes)");
        else Console.WriteLine("Aucun résultat trouvé.");

        Console.WriteLine("Appuyez sur une touche pour continuer.");
    }

    void MagasinChiffreAffaire() {
        Console.Clear();

        Console.WriteLine("MAGASIN AVEC LE PLUS DE CHIFFRE D'AFFAIRES");

        var q = DB.GetCommand();
        q.CommandText = @"
            SELECT magasin.magasin_id, magasin.magasin_nom, SUM(bouquet.bouquet_prix) AS chiffre_daffaires
            FROM magasin
            JOIN commande ON magasin.magasin_id = commande.magasin_id
            JOIN bouquet ON commande.bouquet_id = bouquet.bouquet_id
            GROUP BY magasin.magasin_id
            ORDER BY chiffre_daffaires DESC
            LIMIT 1;
            ";

        var r = q.ExecuteReader();

        if (r.Read()) Console.WriteLine("Magasin avec le plus de chiffre d'affaires : " + r["magasin_nom"] + " (" + r["chiffre_daffaires"] + " euros)");
        else Console.WriteLine("Aucun résultat trouvé.");
        Console.WriteLine("Appuyez sur une touche pour continuer.");
    }

    void FleurExotique() {
        Console.Clear();

        Console.WriteLine("FLEUR EXOTIQUE LA MOINS VENDUE");

        var q = DB.GetCommand();
        q.CommandText = @"SELECT produit.produit_id, produit.produit_nom, COUNT(compositionbouquet.bouquet_id) AS nb_ventes
                FROM produit
                JOIN compositionbouquet ON produit.produit_id = compositionbouquet.produit_id
                JOIN commande ON compositionbouquet.bouquet_id = commande.bouquet_id
                WHERE produit.produit_categorie = 'Fleur exotique'
                GROUP BY produit.produit_id
                ORDER BY nb_ventes ASC
                LIMIT 1;
                ";
        
        var r = q.ExecuteReader();

        if (r.Read()) Console.WriteLine("Fleur exotique la moins vendue : " + r["produit_nom"] + " (" + r["nb_ventes"] + " ventes)");
        else Console.WriteLine("Aucun résultat trouvé.");

        Console.WriteLine("Appuyez sur une touche pour continuer.");
    }
}