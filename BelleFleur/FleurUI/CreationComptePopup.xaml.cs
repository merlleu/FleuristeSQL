using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FleurUI
{
    /// <summary>
    /// Interaction logic for CreationComptePopup.xaml
    /// </summary>
    public partial class CreationComptePopup : Window
    {
        public CreationComptePopup()
        {
            InitializeComponent();
        }

        public void BtnSeConnecter_Click(object sender, RoutedEventArgs e)
        {
            ConnexionPopup connexionPopup = new ConnexionPopup();
            connexionPopup.Show();
            this.Close();
        }

        public void BtnCreerCompte_Click(object sender, RoutedEventArgs e)
        {
            BelleFleurLib.Client c = null;
            try {
                c = new BelleFleurLib.Client(0, tbPrenom.Text, tbNom.Text, tbAdresseMail.Text, tbAdresse.Text, tbCodeCarteCredit.Text, tbMotDePasse.Password);
                c.InsertClient();
            } catch (Exception err) {
                tbMessageErreur.Text = err.ToString();
                tbMessageErreur.Visibility = Visibility.Visible;
                return;
            }

            var h = new ClientAccueilWindow(c);
            h.Show();
            this.Close();
        }
    }
}
