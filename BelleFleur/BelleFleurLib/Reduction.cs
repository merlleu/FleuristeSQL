namespace BelleFleurLib;
using MySql.Data.MySqlClient;
using System;

public class Reduction
{
    public int Id { get; set; }
    public string Nom { get; set; }
    public int CommandesMois { get; set; }
    public decimal Valeur { get; set; }

    public Reduction(MySqlDataReader reader)
    {
        Id = Convert.ToInt32(reader["reduction_id"]);
        Nom = reader["reduction_nom"].ToString();
        CommandesMois = Convert.ToInt32(reader["reduction_commandes_mois"]);
        Valeur = Convert.ToDecimal(reader["reduction_valeur"]);
    }

    public static Reduction GetReductionClient(int clientId)
    {
        var q= DB.GetCommand();
        q.CommandText = "SELECT * FROM reduction WHERE reduction_commandes_mois <= (SELECT COUNT(*) FROM commande WHERE commande.client_id = @ClientId AND commande_date_creation >= DATE_SUB(NOW(), INTERVAL 1 MONTH)) ORDER BY reduction_commandes_mois DESC LIMIT 1";
            
        q.Parameters.AddWithValue("@ClientId", clientId);
        var r = q.ExecuteReader();
        if (r.Read())
        {
            var reduction = new Reduction(r);
            r.Close();
            return reduction;
        }
        else
        {
            r.Close();
            return null;
        }
    }
}
