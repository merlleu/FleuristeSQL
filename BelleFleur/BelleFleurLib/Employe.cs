namespace BelleFleurLib;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

public class Employe
{
    public int EmployeId { get; set; }
    public bool Proprietaire { get; set; }
    public string Email { get; set; }
    public string MotDePasse { get; set; }
    public string Prenom { get; set; }
    public string Nom { get; set; }
    public int MagasinId { get; set; }

    public Employe(int employeId, bool proprietaire, string email, string motDePasse, string prenom, string nom, int magasinId)
    {
        EmployeId = employeId;
        Proprietaire = proprietaire;
        Email = email;
        MotDePasse = motDePasse;
        Prenom = prenom;
        Nom = nom;
        MagasinId = magasinId;
    }

    public Employe(MySqlDataReader reader)
    {
        EmployeId = Convert.ToInt32(reader["employe_id"]);
        Proprietaire = Convert.ToBoolean(reader["employe_proprietaire"]);
        Email = Convert.ToString(reader["employe_email"]);
        MotDePasse = Convert.ToString(reader["employe_pass"]);
        Prenom = Convert.ToString(reader["employe_prenom"]);
        Nom = Convert.ToString(reader["employe_nom"]);
        MagasinId = Convert.ToInt32(reader["magasin_id"]);
    }

    public static Employe GetEmploye(int employeId)
    {
        var command = DB.GetCommand();
        command.CommandText = "SELECT * FROM employe WHERE employe_id = @EmployeId";
        command.Parameters.AddWithValue("@EmployeId", employeId);

        var reader = command.ExecuteReader();

        Employe employe = null;

        if (reader.Read())
        {
            employe = new Employe(reader);
        }

        reader.Close();

        return employe;
    }

    public static Employe GetEmploye(string email)
    {
        var command = DB.GetCommand();
        command.CommandText = "SELECT * FROM employe WHERE employe_email = @Email";
        command.Parameters.AddWithValue("@Email", email);

        var reader = command.ExecuteReader();

        Employe employe = null;

        if (reader.Read())
        {
            employe = new Employe(reader);
        }

        reader.Close();

        return employe;
    }

    public List<Magasin> GetMagasins()
    {
        var magasins = new List<Magasin>();
        

        if (this.Proprietaire)
        {
            magasins = Magasin.GetMagasins();
        }
        else
        {
            var magasin = Magasin.GetMagasin(this.MagasinId);
            magasins.Add(magasin);
        }

        return magasins;
    }

    public bool CheckMotDePasse(string password)
    {
        // Vérifier si le mot de passe fourni correspond au mot de passe de l'employé
        return MotDePasse == password;
    }

}
