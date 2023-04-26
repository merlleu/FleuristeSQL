namespace FleurConsole;

using System;
using BelleFleurLib;

public class ModuleCommandes {
    Connexion connexion = null;
    Client client = null;

    Commande commande = null;
    public ModuleCommandes(Connexion c) {
        connexion = c;
    }

    public void NewCommande(Client client) {
        Console.Clear();
        this.client = client;
        Console.WriteLine("Nouvelle commande pour " + client.Nom + " " + client.Prenom + " :");
        Console.WriteLine();

        this.commande = new Commande();
        
        commande.DateLivraisonDesiree = InputsHelper.Date("Date de livraison", DateTime.Now, DateTime.Now.AddYears(1));
        commande.AdresseLivraison = InputsHelper.Text("Adresse de livraison", 1, 255);

        var magasins = Magasin.GetMagasins();
        for (int i = 0; i < magasins.Count; i++) {
            Console.WriteLine((i + 1) + ". " + magasins[i].Nom + " (" + magasins[i].Adresse + ")");
        }

        int choice = InputsHelper.Int("Magasin", 1, magasins.Count);
        commande.Magasin = magasins[choice - 1];

        Console.WriteLine();

        Console.WriteLine("1. Commande Standard");
        Console.WriteLine("2. Commande Personnalisée");
        
        choice = InputsHelper.Int("Type Commande", 0, 2);

        switch (choice) {
            case 0:
                return;
            case 1:
                CommandeStandard();
                break;
            case 2:
                CommandePersonnalisee();
                break;
        }
    }

    public void CommandeStandard() {
        Console.Clear();
        Console.WriteLine("Commande standard");

        Console.WriteLine("0. Annuler");

        var bouquets = Bouquet.GetBouquets();

        for (int i = 0; i < bouquets.Count; i++) {
            Console.WriteLine((i + 1) + ". " + bouquets[i].Nom + " (" + bouquets[i].Prix + " euros)");
        }

        int choice = InputsHelper.Int("Choix", 0, bouquets.Count);
        
        if (choice == 0) {
            return;
        }

        commande.Bouquet = bouquets[choice - 1];

        ConfirmationCommande();
    }

    public void CommandePersonnalisee() {
        Console.Clear();
        Console.WriteLine("Commande personnalisée");
        var message = InputsHelper.Text("Message",0, 255);
        if (message == "") {
            return;
        }
        var prix_max = InputsHelper.Decimal("Prix maximum", 0, 999.99m);
        if (prix_max == 0) {
            return;
        }
        
        commande.Bouquet = Bouquet.NouveauPersonnalise(message, prix_max);

        ConfirmationCommande();
    }

    public void ConfirmationCommande() {
        Console.Clear();
        commande.Reduction = Reduction.GetReductionClient(client.Id);
        commande.Client = client;

        Console.WriteLine();
        
        Console.WriteLine("Récapitulatif de la commande :");
        Console.WriteLine("Bouquet : " + commande.Bouquet.Nom);
        Console.WriteLine("Prix : " + commande.Bouquet.Prix + " euros");
        Console.WriteLine("Date de livraison : " + commande.DateLivraisonDesiree);
        Console.WriteLine("Adresse de livraison : " + commande.AdresseLivraison);
        Console.WriteLine("Réduction : " + commande.Reduction.Valeur*100 + "%");
        Console.WriteLine("Prix final : " + commande.Bouquet.Prix*(1-commande.Reduction.Valeur) + " euros");

        Console.WriteLine("0. Annuler");
        Console.WriteLine("1. Confirmer");

        int choice = InputsHelper.Int("Choix", 0, 1);

        switch (choice) {
            case 0:
                return;
            case 1:
                commande.Etat = "VINV";
                if (commande.Bouquet.IsCustom()) {
                    Bouquet.InsertBouquet(commande.Bouquet);
                    commande.Etat = "CPAV";
                }

                commande.Insert();

                Console.WriteLine("Commande enregistrée !");
                Console.WriteLine("Appuyez sur une touche pour continuer.");
                Console.ReadKey();
                break;
        }
    }

    public void ListCommandes() {
        Console.Clear();
        Console.WriteLine("Liste des commandes :");
        Console.WriteLine();
        List<Commande> commandes = null;
        if (connexion.isEmploye) {
            List<Magasin> magasins = Magasin.GetMagasins();
            // si pas propriétaire, on ne peut voir que les commandes du magasin de l'employé
            if (!connexion.employe.Proprietaire) {
                magasins = magasins.FindAll(m => m.Id == connexion.employe.MagasinId);
            }

            

            Console.WriteLine("0. Tous");
            
            for (int i = 0; i < magasins.Count; i++) {
                Console.WriteLine((i + 1) + ". " + magasins[i].Nom);
            }

            int choice = InputsHelper.Int("Magasin", 0, magasins.Count);
            
            string etat = InputsHelper.Text("Etat - VINV/CC/CPAV/CAL/CL", 0, 10);

            Console.Clear();
            Console.WriteLine("Magasin : " + (choice != 0 ? magasins[choice - 1].Nom : "Tous"));

            if (etat != "VINV" && etat != "CC" && etat != "CPAV" && etat != "CAL" && etat != "CL") {
                etat = null;
            } else {
                Console.WriteLine("Etat : " + etat);
            }

            Console.WriteLine();
            commandes = Commande.GetCommandes(MagasinId: choice != 0 ? magasins[choice - 1].Id: -1, etat: etat);
        }  else {
            commandes = Commande.GetCommandes(ClientId: connexion.client.Id);
        }

        Console.WriteLine("Liste des " + commandes.Count + " commandes :");
        Console.WriteLine("0. Retour");
        
        for (int i = 0; i < commandes.Count; i++) {
            string etat = (connexion.isEmploye) ? commandes[i].Etat :commandes[i].GetEtatClient();
            Console.WriteLine((i + 1) + ". " + commandes[i].Bouquet.Nom + " (" + commandes[i].Bouquet.Prix + " euros) - " + etat + " - " + commandes[i].DateCreation);
        }

        int choice2 = InputsHelper.Int("Choix", 0, commandes.Count);
        if (choice2 == 0) {
            return;
        }

        AfficherCommande(commandes[choice2 - 1]);
    }

    public void ListCommandesClient(Client cli) {
        Console.Clear();
        Console.WriteLine("Liste des commandes :");
        Console.WriteLine();
        List<Commande> commandes = Commande.GetCommandes(ClientId: cli.Id);

        Console.WriteLine("Liste des " + commandes.Count + " commandes :");
        Console.WriteLine("0. Retour");
        
        for (int i = 0; i < commandes.Count; i++) {
            string etat = (connexion.isEmploye) ? commandes[i].Etat :commandes[i].GetEtatClient();
            Console.WriteLine((i + 1) + ". " + commandes[i].Bouquet.Nom + " (" + commandes[i].Bouquet.Prix + " euros) - " + etat + " - " + commandes[i].DateCreation);
        }

        int choice2 = InputsHelper.Int("Choix", 0, commandes.Count);
        if (choice2 == 0) {
            return;
        }

        AfficherCommande(commandes[choice2 - 1]);
    }
    public void AfficherCommande(Commande comm) {
        comm.Bouquet.GetComposants(comm.Magasin.Id);
        Console.Clear();

        Console.WriteLine("Commande " + comm.Id + " :");

        Console.WriteLine("Bouquet : " + comm.Bouquet.Nom);
        Console.WriteLine("Prix : " + comm.Bouquet.Prix + " euros");
        Console.WriteLine("Date de livraison : " + comm.DateLivraisonDesiree);
        Console.WriteLine("Adresse de livraison : " + comm.AdresseLivraison);
        Console.WriteLine("Magasin : " + comm.Magasin.Nom);

        if (comm.Reduction != null) {
            Console.WriteLine("Réduction : " + comm.Reduction.Valeur*100 + "%");
            Console.WriteLine("Prix final : " + comm.Bouquet.Prix*(1-comm.Reduction.Valeur) + " euros");
        }

        Console.WriteLine("Date de création : " + comm.DateCreation);
        Console.WriteLine("Date de livraison : " + comm.DateLivraisonDesiree);
        Console.WriteLine("Etat : " + (connexion.isEmploye ? comm.Etat : comm.GetEtatClient()));
        
        for (int i = 0; i < comm.Bouquet.Composants.Count; i++) {
            Console.WriteLine(comm.Bouquet.Composants[i].Produit.Nom + " x" + comm.Bouquet.Composants[i].Quantite + " - " + comm.Bouquet.Composants[i].Produit.Prix + " euros");
        }


        Console.WriteLine("0. Retour");
        int max = 0;
        if (connexion.isEmploye) {
            if (comm.Etat == "VINV") {
                Console.WriteLine("1. Vérifier l'inventaire");
                max = 1;
            } else if (comm.Etat == "CC") {
                Console.WriteLine("1. Marquer comme prête");
                max = 1;
            } else if (comm.Etat == "CPAV") {
                Console.WriteLine("1. Verifier la commande personnalisée");
                max = 1;
            } else if (comm.Etat == "CAL") {
                Console.WriteLine("1. Marquer comme livrée");
                max = 1;
            }
        }

        int choice = InputsHelper.Int("Choix", 0, max);

        switch (choice) {
            case 0:
                return;
            case 1:
                if (comm.Etat == "VINV") {
                    VerifierEtat(comm);
                } else if (comm.Etat == "CC") {
                    comm.SetEtat("CAL");
                } else if (comm.Etat == "CPAV") {
                    VerifierEtat(comm);
                } else if (comm.Etat == "CAL") {
                    comm.SetEtat("CL");
                }
                break;
        }

        AfficherCommande(comm);
    }

    public void VerifierEtat(Commande com) {
        Console.Clear();
        Console.WriteLine("Commande " + com.Id + " :");
        Console.WriteLine("Magasin : " + com.Magasin.Nom);

        Console.WriteLine("Produits: ");
        bool isOk = true;
        for (int i = 0; i < com.Bouquet.Composants.Count; i++) {
            var comp = com.Bouquet.Composants[i];
            if (comp.StockQte < comp.Quantite) {
                isOk = false;
            }
            Console.WriteLine(comp.Produit.Nom + " [" + comp.Quantite + "/" + comp.StockQte + "]");
        }

        Console.WriteLine("0. Retour");
        int max = 1;
        Console.WriteLine("1. Marquer comme prête");
        if (com.Bouquet.IsCustom()) {
            Console.WriteLine("2. Ajouter/modifier une quantité");
            max = 2;
        }

        int choice = InputsHelper.Int("Choix", 0, max);

        switch (choice) {
            case 0:
                return;
            case 1:
                if (isOk) {
                    com.SetEtat("CAL");
                    return;
                }
                else {
                    Console.WriteLine("Impossible de marquer la commande comme prête, il manque des produits");
                    Console.ReadKey();
                }
                break;
            case 2:
                AjouterQuantite(com);
                return;
        }

        VerifierEtat(com);
    }

    public void AjouterQuantite(Commande com) {
        List<ComposantBouquet> produits = ComposantBouquet.GetAllProduits(com.Magasin.Id);
        Console.Clear();

        Console.WriteLine("Commande " + com.Id + " :");
        Console.WriteLine("Magasin : " + com.Magasin.Nom);

        
        Console.WriteLine("Produits: ");

        Console.WriteLine("0. Retour");
        for (int i = 0; i < produits.Count; i++) {
            Console.WriteLine((i + 1) + ". " + produits[i].Produit.Nom + " [" + produits[i].StockQte + "]");
        }

        int choice = InputsHelper.Int("Choix", 0, produits.Count);
        if (choice == 0) {
            return;
        }

        ComposantBouquet comp = produits[choice - 1];

        Console.WriteLine("0 pour retirer");
        int qte = InputsHelper.Int("Quantité", 0, 100);

        ComposantBouquet.AjouterComposant(com.Magasin.Id, com.Bouquet.Id, comp.Produit.Id, qte);
    }
}