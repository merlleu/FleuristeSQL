namespace BelleFleurLib;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

public class Produit
{
    public int Id { get; set; }
    public string Nom { get; set; }
    public string Type { get; set; }
    public decimal Prix { get; set; }
    public string Categorie { get; set; }
    public string DisponibiliteMois { get; set; }

    public Produit(MySqlDataReader reader)
    {
        Id = Convert.ToInt32(reader["produit_id"]);
        Nom = reader["produit_nom"].ToString();
        Type = reader["produit_type"].ToString();
        Prix = Convert.ToDecimal(reader["produit_prix"]);
        Categorie = reader["produit_categorie"].ToString();
        DisponibiliteMois = reader["produit_disponibilite_mois"].ToString();
    }

    public static List<Produit> GetProduits(int produitId = 0, string categorie = null, bool onlyDisponible = true)
    {
        var produits = new List<Produit>();
        var command = DB.GetCommand();

        if (produitId != 0)
        {
            command.CommandText = "SELECT * FROM produit WHERE produit_id = @ProduitId";
            command.Parameters.AddWithValue("@ProduitId", produitId);
        }
        else if (categorie != null)
        {
            command.CommandText = "SELECT * FROM produit WHERE produit_categorie = @Categorie";
            command.Parameters.AddWithValue("@Categorie", categorie);
        }
        else
        {
            command.CommandText = "SELECT * FROM produit";
        }

        var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var produit = new Produit(reader);
            produits.Add(produit);
        }

        if (onlyDisponible)
        {
            produits = produits.Where(p => p.Disponible()).ToList();
        }

        reader.Close();

        return produits;
    }

    public bool Disponible() {
        if (DisponibiliteMois == "*") return true;
        var mois = DateTime.Now.Month;
        var moisDisponibilite = DisponibiliteMois.Split(',');
        foreach (var m in moisDisponibilite)
        {
            if (Convert.ToInt32(m) == mois)
            {
                return true;
            }
        }
        return false;
    }
}

public class StockParMagasin {
    public Magasin Magasin { get; set; }
    public int Quantite { get; set; }

    public StockParMagasin(MySqlDataReader reader)
    {
        Magasin = Magasin.GetMagasin(Convert.ToInt32(reader["magasin_id"]));
        Quantite = Convert.ToInt32(reader["stock_quantite"]);
    }

    
}