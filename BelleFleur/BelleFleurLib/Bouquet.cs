namespace BelleFleurLib;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

public class Bouquet
{
    public int Id { get; set; }
    public string Nom { get; set; }
    public decimal Prix { get; set; }
    public string Description { get; set; }
    public string Categorie { get; set; }
    public List<ComposantBouquet> Composants { get; set; }

    static public string CAT_PERSO = "Personnalisé";

    public Bouquet(MySqlDataReader reader)
    {
        Id = Convert.ToInt32(reader["bouquet_id"]);
        Nom = reader["bouquet_nom"].ToString();
        Prix = Convert.ToDecimal(reader["bouquet_prix"]);
        Description = reader["bouquet_description"].ToString();
        Categorie = reader["bouquet_categorie"].ToString();
    }

    public Bouquet(){}

    public static List<Bouquet> GetBouquets(string categorie = null)
    {
        var bouquets = new List<Bouquet>();
        var command = DB.GetCommand();

        if (categorie != null)
        {
            command.CommandText = "SELECT * FROM bouquet WHERE bouquet_categorie = @Categorie";
            command.Parameters.AddWithValue("@Categorie", categorie);
        }
        else
        {
            command.CommandText = "SELECT * FROM bouquet WHERE bouquet_categorie != @Categorie";
            command.Parameters.AddWithValue("@Categorie", CAT_PERSO);
        }

        var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var bouquet = new Bouquet(reader);
            bouquets.Add(bouquet);
        }

        reader.Close();

        return bouquets;
    }


    public void GetComposants(int magasinId)
    {
        this.Composants = ComposantBouquet.GetComposantsBouquet(magasinId, this.Id);
    }

    public static Bouquet NouveauPersonnalise(string message, decimal prix_max)
    {
        var bouquet = new Bouquet();

        bouquet.Nom = "Bouquet personnalisé";
        bouquet.Description = message;
        bouquet.Prix = prix_max;
        bouquet.Categorie = CAT_PERSO;
        
        
        return bouquet;
    }

    public static void InsertBouquet(Bouquet bouquet) {
        // on ne veut inserer que les bouquets customs
        if (bouquet.Id != 0) return;

        var command = DB.GetCommand();
        command.CommandText = @"
            INSERT INTO bouquet (bouquet_nom, bouquet_prix, bouquet_description, bouquet_categorie)
            VALUES (@Nom, @Prix, @Description, @Categorie)
        ";

        command.Parameters.AddWithValue("@Nom", bouquet.Nom);
        command.Parameters.AddWithValue("@Prix", bouquet.Prix);
        command.Parameters.AddWithValue("@Description", bouquet.Description);
        command.Parameters.AddWithValue("@Categorie", bouquet.Categorie);

        command.ExecuteNonQuery();

        bouquet.Id = (int)command.LastInsertedId;
    }

    public bool IsCustom()
    {
        return Categorie == CAT_PERSO;
    }
}
