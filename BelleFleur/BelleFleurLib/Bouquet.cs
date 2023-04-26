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
            command.CommandText = "SELECT * FROM bouquet";
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
        ComposantBouquet.GetComposantsBouquet(magasinId, this.Id);
    }

    public static Bouquet NouveauPersonnalise(string message, int prix_max)
    {
        var bouquet = new Bouquet();

        bouquet.Nom = "Bouquet personnalisé";
        bouquet.Description = message;
        bouquet.Prix = prix_max;
        
        
        return bouquet;
    }
}
