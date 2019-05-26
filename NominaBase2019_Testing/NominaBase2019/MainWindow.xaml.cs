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

namespace NominaBase2019
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (txtUsuario.Text != null && txtPass.Password != null)
            {
                if (txtUsuario.Text == "admin" && txtPass.Password == "admin")
                {
                    MenuPrincipal mp = new MenuPrincipal();
                    mp.ShowDialog();
                }
                else
                    MessageBox.Show("El usuario o la contraseña son incorrectos");
            }
            else
                MessageBox.Show("Los campos de usuario y contraseña son obligatorios");


        }
    }
}
