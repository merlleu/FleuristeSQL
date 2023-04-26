namespace FleurConsole;

using BelleFleurLib;
using System;

public class ModuleClients {
    Connexion connexion;
    public ModuleClients(Connexion c) {
        connexion = c;
    }

    public void ListClients() {
        Console.Clear();

        Console.WriteLine("LISTE DES CLIENTS");

        var clients = Client.GetClients();

        int i = 1;
        Console.WriteLine("0. Retour");
        foreach (var client in clients) {
            Console.WriteLine($"{i}. {client.Nom} {client.Prenom} ({client.Email})");
            i++;
        }

        int choice = InputsHelper.Int("Choix", 0, clients.Count);

        if (choice == 0) {
            return;
        }
        
        var clientSelected = clients[choice - 1];

        AfficherClient(clientSelected);
    }

    public void AfficherClient(Client client) {
        Console.Clear();

        Console.WriteLine("CLIENT");

        Console.WriteLine($"Id: {client.Id}");
        Console.WriteLine($"Nom: {client.Nom}");
        Console.WriteLine($"Prénom: {client.Prenom}");
        Console.WriteLine($"Email: {client.Email}");
        Console.WriteLine($"Adresse: {client.Adresse}");
        Reduction red = Reduction.GetReductionClient(client.Id);
        Console.WriteLine($"Statut Fidélité: {red.Nom}");
        Console.WriteLine($"Carte de crédit: {client.CarteDeCredit}");

        Console.WriteLine();

        Console.WriteLine("0. Retour");
        Console.WriteLine("1. Liste des commandes");
        Console.WriteLine("2. Créer une commande");

        int choice = InputsHelper.Int("Choix", 0, 2);

        switch (choice) {
            case 0:
                return;
            case 1:
                new ModuleCommandes(connexion).ListCommandesClient(client);
                break;
            case 2:
                new ModuleCommandes(connexion).NewCommande(client);
                break;
        }

    }
}