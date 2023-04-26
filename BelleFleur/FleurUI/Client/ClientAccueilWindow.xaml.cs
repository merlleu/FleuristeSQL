using System.Windows;

namespace FleurUI
{
    public partial class ClientAccueilWindow : Window
    {
        BelleFleurLib.Client client;
        public ClientAccueilWindow(BelleFleurLib.Client client)
        {
            InitializeComponent();
            this.client = client;
        }

        public void MesCommandes_Click(object sender, RoutedEventArgs e)
        {
            ClientCommandeListWindow clientCommandeList = new ClientCommandeListWindow(client);
            clientCommandeList.Show();
            this.Close();
        }

        public void NouvelleCommande_Click(object sender, RoutedEventArgs e)
        {
            // ClientNouvelleCommandeWindow clientNouvelleCommandeWindow = new ClientNouvelleCommandeWindow(client);
            // clientNouvelleCommandeWindow.ShowPopup();
            // this.Close();
        }

        public void Deconnexion_Click(object sender, RoutedEventArgs e)
        {
            ConnexionPopup connexionPopup = new ConnexionPopup();
            connexionPopup.Show();
            this.Close();
        }
    }
}
