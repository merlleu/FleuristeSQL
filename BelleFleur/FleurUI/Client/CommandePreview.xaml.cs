using System.Windows;
using System.Collections.Generic;
using BelleFleurLib;

namespace FleurUI
{
    public partial class CommandePreview : Window
    {
        Employe employe = null;
        public CommandePreview(int id, Employe employe=null)
        {
            InitializeComponent();

            var commandes = Commande.GetCommandes(CommandeId: id);
            if (commandes.Count == 0)
            {
                MessageBox.Show("Commande introuvable");
                this.Close();
                return;
            }
            var commande = commandes[0];
            commande.Bouquet.GetComposants(commande.Magasin.Id);

            
            this.Table.ItemsSource = commande.Bouquet.Composants;

            IdCommandeTextBlock.Text = commande.Id.ToString();
            ClientTextBlock.Text = $"{commande.Client.Prenom} {commande.Client.Nom}";
            AdresseLivraisonTextBlock.Text = commande.AdresseLivraison;
            DateCommandeTextBlock.Text = commande.DateCreation.ToString("dd/MM/yyyy");
            DateLivraisonTextBlock.Text = commande.DateLivraisonDesiree.ToString("dd/MM/yyyy");
            NomBouquetTextBlock.Text = commande.Bouquet.Nom;
            DescriptionBouquetTextBlock.Text = commande.Bouquet.Description;
            CategorieBouquetTextBlock.Text = commande.Bouquet.Categorie;
        }
    }
}
