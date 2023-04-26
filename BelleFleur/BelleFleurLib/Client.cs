namespace BelleFleurLib;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

public class Client
{
    public int Id { get; set; }
    public string Prenom { get; set; }
    public string Nom { get; set; }
    public string Email { get; set; }
    public string Adresse { get; set; }
    public string CarteDeCredit { get; set; }
    public string MotDePasse { get; set; }

    public Client(MySqlDataReader reader)
    {
        Id = Convert.ToInt32(reader["client_id"]);
        Prenom = reader["client_prenom"].ToString();
        Nom = reader["client_nom"].ToString();
        Email = reader["client_email"].ToString();
        Adresse = reader["client_adresse"].ToString();
        CarteDeCredit = reader["client_carte_de_credit"].ToString();
        MotDePasse = reader["client_pass"].ToString();
    }

    public Client(int id, string prenom, string nom, string email, string adresse, string carteDeCredit, string motDePasse)
    {
        Id = id;
        Prenom = prenom;
        Nom = nom;
        Email = email;
        Adresse = adresse;
        CarteDeCredit = carteDeCredit;
        MotDePasse = motDePasse;
    }

    public static List<Client> GetClients(string email = null, int id = 0)
    {
        var clients = new List<Client>();
        var command = DB.GetCommand();

        if (email != null)
        {
            command.CommandText = "SELECT * FROM client WHERE client_email = @Email";
            command.Parameters.AddWithValue("@Email", email);
        }
        else if (id != 0)
        {
            command.CommandText = "SELECT * FROM client WHERE client_id = @Id";
            command.Parameters.AddWithValue("@Id", id);
        }
        else
        {
            command.CommandText = "SELECT * FROM client";
        }

        var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var client = new Client(reader);
            clients.Add(client);
        }

        reader.Close();

        return clients;
    }

    public bool CheckMotDePasse(string password)
    {
        // Vérifier si le mot de passe fourni correspond au mot de passe du client
        return MotDePasse == password;
    }

    public static void InsertClient(Client client)
    {
        var command = DB.GetCommand();
        command.CommandText = "INSERT INTO client (client_prenom, client_nom, client_email, client_adresse, client_carte_de_credit, client_pass) VALUES (@Prenom, @Nom, @Email, @Adresse, @CarteDeCredit, @MotDePasse)";
        command.Parameters.AddWithValue("@Prenom", client.Prenom);
        command.Parameters.AddWithValue("@Nom", client.Nom);
        command.Parameters.AddWithValue("@Email", client.Email);
        command.Parameters.AddWithValue("@Adresse", client.Adresse);
        command.Parameters.AddWithValue("@CarteDeCredit", client.CarteDeCredit);
        command.Parameters.AddWithValue("@MotDePasse", client.MotDePasse);

        command.ExecuteNonQuery();
    }
}
