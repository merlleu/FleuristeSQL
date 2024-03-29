namespace BelleFleurLib;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

public class Commande
{
    public int Id { get; set; }
    public string AdresseLivraison { get; set; }
    public DateTime DateLivraisonDesiree { get; set; }
    public string Etat { get; set; }
    public DateTime DateCreation { get; set; }
    public Client Client { get; set; }
    public Bouquet Bouquet { get; set; }
    public Magasin Magasin { get; set; }
    public Reduction Reduction { get; set; }

    public Commande()
    {
    }

    public Commande(int commandeId, string adresseLivraison, DateTime dateLivraisonDesiree, string etat, DateTime dateCreation, decimal prix, Client client, Bouquet bouquet, Magasin magasin, Reduction reduction)
    {
        Id = commandeId;
        AdresseLivraison = adresseLivraison;
        DateLivraisonDesiree = dateLivraisonDesiree;
        Etat = etat;
        DateCreation = dateCreation;
        Client = client;
        Bouquet = bouquet;
        Magasin = magasin;
        Reduction = reduction;
    }

    public Commande(MySqlDataReader reader)
    {
        Id = Convert.ToInt32(reader["commande_id"]);
        AdresseLivraison = Convert.ToString(reader["commande_adresse_livraison"]);
        DateLivraisonDesiree = Convert.ToDateTime(reader["commande_date_livraison_desiree"]);
        Etat = Convert.ToString(reader["commande_etat"]);
        DateCreation = Convert.ToDateTime(reader["commande_date_creation"]);
        Client = new Client(reader);
        Bouquet = new Bouquet(reader);
        Magasin = new Magasin(reader);
        Reduction = new Reduction(reader);
    }

    public static List<Commande> GetCommandes(int MagasinId = -1, int ClientId = -1, string etat = null, int CommandeId = -1)
    {
        var commandes = new List<Commande>();
        var command = DB.GetCommand();
        command.CommandText = @"
        SELECT commande.*, client.*, bouquet.*, magasin.*, reduction.*
        FROM commande 
        JOIN client ON commande.client_id = client.client_id 
        JOIN bouquet ON commande.bouquet_id = bouquet.bouquet_id
        JOIN magasin ON commande.magasin_id = magasin.magasin_id
        JOIN reduction ON commande.reduction_id = reduction.reduction_id
        WHERE 1=1"; // on est obligé de mettre une condition toujours vraie pour pouvoir ajouter des AND

        if (MagasinId != -1)
        {
            command.CommandText += " AND commande.magasin_id = @MagasinId";
            command.Parameters.AddWithValue("@MagasinId", MagasinId);
        }

        if (ClientId != -1)
        {
            command.CommandText += " AND commande.client_id = @ClientId";
            command.Parameters.AddWithValue("@ClientId", ClientId);
        }

        if (etat != null)
        {
            command.CommandText += " AND commande.commande_etat = @Etat";
            command.Parameters.AddWithValue("@Etat", etat);
        }

        if (CommandeId != -1)
        {
            command.CommandText += " AND commande.commande_id = @CommandeId";
            command.Parameters.AddWithValue("@CommandeId", CommandeId);
        }

        var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var commande = new Commande(reader);
            commandes.Add(commande);
        }

        reader.Close();

        return commandes;
    }

    public void Insert()
    {
        var command = DB.GetCommand();
        command.CommandText = @"
        INSERT INTO commande (client_id, bouquet_id, magasin_id, reduction_id, commande_adresse_livraison, commande_date_livraison_desiree, commande_etat, commande_date_creation)
        VALUES (@ClientId, @BouquetId, @MagasinId, @ReductionId, @AdresseLivraison, @DateLivraisonDesiree, @Etat, NOW())";

        command.Parameters.AddWithValue("@ClientId", Client.Id);
        command.Parameters.AddWithValue("@BouquetId", Bouquet.Id);
        command.Parameters.AddWithValue("@MagasinId", Magasin.Id);
        command.Parameters.AddWithValue("@ReductionId", Reduction.Id);
        
        command.Parameters.AddWithValue("@AdresseLivraison", AdresseLivraison);
        command.Parameters.AddWithValue("@DateLivraisonDesiree", DateLivraisonDesiree);
        command.Parameters.AddWithValue("@Etat", Etat);

        command.ExecuteNonQuery();

        int commandeId = Convert.ToInt32(command.LastInsertedId);
    }

    public string GetEtatClient()
    {
        switch (Etat)
        {
            case "VINV":
            case "CPAV":
                return "En attente de validation";
            case "CC":
                return "En cours de préparation";
            case "CAL":
                return "En attente de livraison";
            case "CL":
                return "Livrée";
            default:
                return "Inconnu";
        }
    }

    public string EtatClient => GetEtatClient();

    public void SetEtat(string etat) {
        var command = DB.GetCommand();
        command.CommandText = @"
        UPDATE commande
        SET commande_etat = @Etat
        WHERE commande_id = @CommandeId";

        command.Parameters.AddWithValue("@Etat", etat);
        command.Parameters.AddWithValue("@CommandeId", Id);
        
        command.ExecuteNonQuery();
        Etat = etat;
    }
}
