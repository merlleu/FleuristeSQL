namespace FleurConsole;

using BelleFleurLib;
using System;

public class ModuleProduits {

    public Magasin magasin = null;

    public ModuleProduits() {}

    public void AfficherAlertesStocks() {
        Console.Clear();

        Console.WriteLine("ALERTES STOCKS");
        Console.WriteLine("Sélectionnez un produit en alerte pour y accéder.");

        var alertes = StockAlerte.GetAll();

        int i = 1;
        Console.WriteLine("0. Retour");

        foreach (var alerte in alertes) {
            Console.WriteLine($"{i}. {alerte.Magasin.Nom} - {alerte.Produit.Nom} - {alerte.Quantite}/{alerte.QteMin}");
            i++;
        }

        int choice = InputsHelper.Int("Choix", 0, alertes.Count);

        if (choice == 0) {
            return;
        }

        var alerteSelected = alertes[choice - 1];
        this.magasin = alerteSelected.Magasin;
        
        // AfficherProduit(alerteSelected.Produit);
        AfficherAlertesStocks();
    }

    public void AfficherProduitMagasins() {
        Console.Clear();

        Console.WriteLine("LISTE DES PRODUITS");

        var magasins = Magasin.GetMagasins();
        
        int i = 1;
        Console.WriteLine("0. Retour");
        foreach (var magasin in magasins) {
            Console.WriteLine($"{i}. {magasin.Nom}");
            i++;
        }

        int choice = InputsHelper.Int("Choix", 0, magasins.Count);

        if (choice == 0) {
            return;
        }

        var magasinSelected = magasins[choice - 1];
        this.magasin = magasinSelected;

        AfficherProduitMagasin(magasinSelected);
    }

    public void AfficherProduitMagasin(Magasin magasin) {
        Console.Clear();

        Console.WriteLine("PRODUITS");

        var produits = ComposantBouquet.GetAllProduits(magasin.Id);


        int i = 1;
        Console.WriteLine("0. Retour");
        foreach (var produit in produits) {
            Console.WriteLine($"{i}. {produit.Produit.Nom} - {produit.StockQte} - {produit.QuantiteMin}");
            i++;
        }

        int choice = InputsHelper.Int("Choix", 0, produits.Count);

        if (choice == 0) {
            return;
        }

        var produitSelected = produits[choice - 1];

        AfficherProduit(produitSelected);
        AfficherProduitMagasin(magasin);
    }

    private void AfficherProduit(ComposantBouquet produit) {
        Console.Clear();

        Console.WriteLine("PRODUIT");

        Console.WriteLine($"Id: {produit.Produit.Id}");
        Console.WriteLine($"Nom: {produit.Produit.Nom}");
        Console.WriteLine($"Prix: {produit.Produit.Prix}");
        Console.WriteLine($"Quantité: {produit.StockQte}");
        Console.WriteLine($"Quantité minimum: {produit.QuantiteMin}");
        Console.WriteLine($"Magasin: {magasin.Nom}");

        Console.WriteLine();

        var qte = InputsHelper.Int("Quantité Actuelle", 0, 1000);
        var qteMin = InputsHelper.Int("Quantité Minimum", 0, 1000);
        ComposantBouquet.MajStocks(magasin.Id, produit.Produit.Id, qte, qteMin);
    }
}