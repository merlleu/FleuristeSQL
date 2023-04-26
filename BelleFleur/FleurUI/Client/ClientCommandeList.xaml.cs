using System.Windows;
using System.Collections.Generic;
using BelleFleurLib;

namespace FleurUI
{
    public partial class ClientCommandeListWindow : Window
    {
        BelleFleurLib.Client client;
        List<BelleFleurLib.Commande> commandes;
        public ClientCommandeListWindow(BelleFleurLib.Client client)
        {
            InitializeComponent();
            this.client = client;

            commandes = Commande.GetCommandes(ClientId: client.Id);

            this.Table.ItemsSource = commandes;
            this.HeaderText.Text = $"Commandes de {client.Prenom} {client.Nom}";
        }
    }
}
