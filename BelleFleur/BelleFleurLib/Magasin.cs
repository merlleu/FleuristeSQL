namespace BelleFleurLib;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

public class Magasin
{
    public int Id { get; set; }
    public string Nom { get; set; }
    public string Adresse { get; set; }

    public Magasin(MySqlDataReader reader)
    {
        Id = Convert.ToInt32(reader["magasin_id"]);
        Nom = reader["magasin_nom"].ToString();
        Adresse = reader["magasin_adresse"].ToString();
    }

    public static Magasin GetMagasin(int id)
    {
        var command = DB.GetCommand();
        command.CommandText = "SELECT * FROM magasin WHERE magasin_id = @Id";
        command.Parameters.AddWithValue("@Id", id);

        var reader = command.ExecuteReader();

        Magasin magasin = null;

        if (reader.Read())
        {
            magasin = new Magasin(reader);
        }

        reader.Close();

        return magasin;
    }

    public static List<Magasin> GetMagasins()
    {
        var magasins = new List<Magasin>();
        var command = DB.GetCommand();
        command.CommandText = "SELECT * FROM magasin";

        var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var magasin = new Magasin(reader);
            magasins.Add(magasin);
        }

        reader.Close();

        return magasins;
    }
}
