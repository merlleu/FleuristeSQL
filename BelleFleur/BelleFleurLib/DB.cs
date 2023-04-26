namespace BelleFleurLib;

using System;
using MySql.Data.MySqlClient;

public class DB
{
    public static MySqlConnection GetConnection()
    {
        try {
            string connString = "Server=localhost;Database=bellefleur;Uid=root;Pwd=root;Port=3306;";

            MySqlConnection conn = new MySqlConnection(connString);
            conn.Open();
            return conn;
        } catch (Exception err) { // si db pas trouvée
            BootStrapDB(); // erreur si mysql pas lancé
            return GetConnection(); // sinon on retry la connection
        }
    }

    public static MySqlCommand GetCommand() 
    {
        MySqlCommand cmd = GetConnection().CreateCommand();

        return cmd;
    }

    public static void BootStrapDB() {
        string connString = "Server=localhost;Uid=root;Pwd=root;Port=3306;"; 

        MySqlConnection conn = new MySqlConnection(connString);
        conn.Open();

        MySqlCommand cmd = conn.CreateCommand();

        // open Bellefleur.sql and execute it 
        string sql = System.IO.File.ReadAllText("Bellefleur.sql");
        cmd.CommandText = sql;

        cmd.ExecuteNonQuery();
    }
}