namespace FleurConsole;

using BelleFleurLib;


public class ModuleConnect {
    Connexion connexion = null;

    public ModuleConnect() {}

    public void Menu() {
        Console.Clear();
        Console.WriteLine("Bienvenue dans BelleFleur !");
        Console.WriteLine("Que voulez-vous faire ?");
        Console.WriteLine("1. Se connecter");
        Console.WriteLine("2. Créer un compte");
        Console.WriteLine("3. Quitter");
        Console.WriteLine();

        int choice = InputsHelper.Int("Votre choix", 1, 3);

        switch (choice) {
            case 1:
                LoginPage();
                break;
            case 2:
                CreerCompte();
                break;
            case 3:
                Environment.Exit(0);
                break;
        }
    }

    public void LoginPage() {
        Console.Clear();
        Console.WriteLine("Bienvenue dans BelleFleur !");
        Console.WriteLine("Veuillez vous connecter pour continuer.");
        Console.WriteLine();

        // Read the email and password from the user
        string email = InputsHelper.Text("Adresse Email", 3, 50);
        string password = InputsHelper.Text("Mot de passe", 3, 50, true);

        try {
            connexion = new Connexion(email, password);
            try {
                Connected();
            } catch (Exception e) {
                Console.WriteLine("\nErreur : " + e.Message);
                Console.WriteLine("Appuyez sur une touche pour réessayer.");
                Console.ReadKey();
                Connected();
            }
        } catch (Exception e) {
            Console.WriteLine("\nErreur de connexion : " + e.Message);
            Console.WriteLine("Appuyez sur une touche pour réessayer.");
            Console.ReadKey();
            LoginPage();
        }
    }

    public void Connected() {
        Console.Clear();
        if (connexion.isEmploye) {
            Console.WriteLine("Bonjour " + connexion.employe.Nom + " " + connexion.employe.Prenom + " !");
            
            Console.WriteLine("0. Quitter");
            Console.WriteLine("1. Gérer les clients");
            Console.WriteLine("2. Gérer les commandes");
            Console.WriteLine("3. Gérer les produits");

            if (connexion.employe.Proprietaire) {
                Console.WriteLine("4. Afficher les statistiques");
            }

            int choice = InputsHelper.Int("Choix", 0, connexion.employe.Proprietaire ? 4 : 3);

            switch (choice) {
                case 0:
                    new ModuleConnect().LoginPage();
                    return;
                case 1:
                    new ModuleClients(connexion).ListClients();
                    break;
                case 2:
                    new ModuleCommandes(connexion).ListCommandes();
                    break;
                case 3:
                    new ModuleProduits().AfficherProduitMagasins();
                    break;
                case 4:
                    new ModuleStatistiques().Menu();
                    break;
                default:
                    break;
            }

        } else {
            Console.WriteLine("Bonjour " + connexion.client.Nom + " " + connexion.client.Prenom + " !");

            Console.WriteLine("0. Quitter");
            Console.WriteLine("1. Mes Commandes");
            Console.WriteLine("2. Nouvelle Commande");
            
            int choice = InputsHelper.Int("Choix", 0, 2);

            switch (choice) {
                case 0:
                    new ModuleConnect().LoginPage();
                    return;
                case 1:
                    new ModuleCommandes(connexion).ListCommandes();
                    break;
                case 2:
                    new ModuleCommandes(connexion).NewCommande(connexion.client);
                    break;
            }
        }

        Connected();
    }

    public void CreerCompte() {
        Console.Clear();
        Console.WriteLine("Bienvenue dans BelleFleur !");
        Console.WriteLine("Veuillez créer un compte pour continuer.");
        Console.WriteLine();

        // Read the email and password from the user
        string email = InputsHelper.Text("Adresse Email", 3, 50);
        string motDePasse = InputsHelper.Text("Mot de passe", 3, 50, true);
        string nom = InputsHelper.Text("Nom", 3, 50);
        string prenom = InputsHelper.Text("Prénom", 3, 50);
        string adresse = InputsHelper.Text("Adresse", 3, 255);
        string telephone = InputsHelper.Text("Téléphone", 3, 50);
        string carteDeCredit = InputsHelper.Text("Carte de crédit", 3, 50);

        try {
            Client client = new Client(0, prenom, nom, email, adresse, carteDeCredit, motDePasse);
            client.InsertClient();
            Console.WriteLine("\nCompte créé avec succès !");
            Console.WriteLine("Appuyez sur une touche pour vous connecter.");
            Console.ReadKey();
            LoginPage();
        } catch (Exception e) {
            Console.WriteLine("\nErreur de création de compte : " + e.Message);
            Console.WriteLine("Appuyez sur une touche pour réessayer.");
            Console.ReadKey();
            CreerCompte();
        }
    }
}