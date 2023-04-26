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
    /// Interaction logic for ConnexionPopup.xaml
    /// </summary>
    public partial class ConnexionPopup : Window
    {
        public ConnexionPopup()
        {
            InitializeComponent();
        }

        public void BtnSeConnecter_Click(object sender, RoutedEventArgs e)
        {
            try {
                var c = new BelleFleurLib.Connexion(tbEmail.Text, tbMotDePasse.Password);
            }
            catch (Exception err) {
                tbMessageErreur.Text = err.ToString();
                tbMessageErreur.Visibility = Visibility.Visible;
            }
        }

        public void BtnCreerCompte_Click(object sender, RoutedEventArgs e)
        {
            CreationComptePopup creationComptePopup = new CreationComptePopup();
            creationComptePopup.Show();
            this.Close();
        }
    }
}
