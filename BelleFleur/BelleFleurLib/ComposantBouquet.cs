using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

public class ComposantBouquet
{
    public Produit Produit { get; set; }
    public int Quantite { get; set; }
    public int StockQte { get; set; }

    public ComposantBouquet(Produit produit, int quantite, int stockQte)
    {
        Produit = produit;
        Quantite = quantite;
        StockQte = stockQte;
    }

    public static List<ComposantBouquet> GetComposantsBouquet(int magasinId, int bouquetId)
    {
        var composantsBouquet = new List<ComposantBouquet>();
        var command = DB.GetCommand();
        command.CommandText = "SELECT compositionbouquet.*, produit.*, stocks.* FROM compositionbouquet INNER JOIN produit ON compositionbouquet.produit_id = produit.produit_id LEFT JOIN stocks ON stocks.magasin_id = @MagasinId AND stocks.produit_id = produit.produit_id WHERE compositionbouquet.bouquet_id = @BouquetId";
        command.Parameters.AddWithValue("@MagasinId", magasinId);
        command.Parameters.AddWithValue("@BouquetId", bouquetId);

        var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var produit = new Produit(reader);
            var quantite = Convert.ToInt32(reader["composition_quantite"]);
            var stockQte = Convert.ToInt32(reader["stock_qte"]);

            var composantBouquet = new ComposantBouquet(produit, quantite, stockQte);
            composantsBouquet.Add(composantBouquet);
        }

        reader.Close();

        return composantsBouquet;
    }

    public static void AjouterComposant(int magasinId, int bouquetId, int produitId, int quantite)
    {
        if (quantite <= 0)
        {
            SupprimerComposant(magasinId, bouquetId, produitId);
        }
        var command = DB.GetCommand();
        command.CommandText = "INSERT INTO compositionbouquet (bouquet_id, produit_id, composition_quantite) VALUES (@BouquetId, @ProduitId, @Quantite) ON DUPLICATE KEY UPDATE composition_quantite = @Quantite";
        command.Parameters.AddWithValue("@BouquetId", bouquetId);
        command.Parameters.AddWithValue("@ProduitId", produitId);
        command.Parameters.AddWithValue("@Quantite", quantite);

        command.ExecuteNonQuery();
    }

    public static void SupprimerComposant(int magasinId, int bouquetId, int produitId)
    {
        var command = DB.GetCommand();
        command.CommandText = "DELETE FROM compositionbouquet WHERE bouquet_id = @BouquetId AND produit_id = @ProduitId";
        command.Parameters.AddWithValue("@BouquetId", bouquetId);
        command.Parameters.AddWithValue("@ProduitId", produitId);

        command.ExecuteNonQuery();
    }

}
