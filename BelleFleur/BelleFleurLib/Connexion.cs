namespace BelleFleurLib;

using System;

public class Connexion {
    public bool isEmploye = false;
    public Client client = null;
    public Employe employe = null;

    public Connexion(string email, string password) {
        // si @bellefleur.fr, employe
        // sinon, client
        email = email.ToLower();
        isEmploye = email.EndsWith("@bellefleur.fr");

        if (isEmploye) {
            ConnectEmployee(email, password);
        } else {
            ConnectClient(email, password);
        }
    }

    private void ConnectClient(string email, string password) {
        var clients = Client.GetClients(email: email);
        if (clients == null || clients.Count == 0) {
            throw new Exception("Client non trouvé");
        }

        client = clients[0];

        if (!client.CheckMotDePasse(password)) {
            throw new Exception("Mot de passe incorrect");
        }
    }

    private void ConnectEmployee(string email, string password) {
        employe = Employe.GetEmploye(email: email);
        if (employe == null) {
            throw new Exception("Employé non trouvé");
        }

        if (!employe.CheckMotDePasse(password)) {
            throw new Exception("Mot de passe incorrect");
        }
    }
}