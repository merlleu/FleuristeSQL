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
        var command = DB.GetCommand();
        command.CommandText = "get_reduction_client";
        command.CommandType = System.Data.CommandType.StoredProcedure;
        command.Parameters.AddWithValue("client_id_param", clientId);

        var reader = command.ExecuteReader();

        Reduction reduction = null;

        if (reader.Read())
        {
            reduction = new Reduction(reader);
        }

        reader.Close();

        return reduction;
    }

}
