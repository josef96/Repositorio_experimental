using Microsoft.Win32;
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
using System.Windows.Shapes;

namespace wpfABMCExample
{
    /// <summary>
    /// Interaction logic for w_Artesania.xaml
    /// </summary>
    public partial class w_Artesania : Window
    {
        //Creamos el objeto 
        ConexionBD datos;

        public w_Artesania()
        {
            InitializeComponent();
            //Se instancia el objeto conexion
            datos = new ConexionBD();
        }

        private void CargarDatosGrilla()
        {            
            try
            {
                //Con una sola linea de código, cargamos la grilla con las Artesanias
                dgArtesanias.ItemsSource = datos.Artesania.ToList();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CargarDatosGrilla();

            //Cargamos el combo de Materiales
            cboMaterialPrincipal.ItemsSource = datos.Material.ToList();
            cboMaterialPrincipal.DisplayMemberPath = "Nombre";
            cboMaterialPrincipal.SelectedValuePath = "Id_Material";

            //En el caso de las categorias, vamos a crear los controles en base a los datos.
            //Khe? si, asi mismo.. xD

            //Obtenemos las categorias
            var categorias = datos.Categoria.ToList();

            //Por cada categoria, creamos un checkbox y agregamos al contenedor
            foreach (var c in categorias)
            {
                CheckBox chk = new CheckBox();
                chk.Content = c.Nombre;
                chk.Name = "chk" + c.Nombre;
                chk.Tag = c.Id_Categoria;

                //Agregamos al contenedor de los checkboxes
                chksCategorias.Children.Add(chk);
            }            
        }

        private void btnImagen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Elegir una imagen";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                imgPhoto.Source = new BitmapImage(new Uri(op.FileName));
            }
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            //Validaciones de ingreso de datos!
            //Try Catch!


            //Crear un objeto de Tipo Artesania, agregarlo al modelo... Y el insert into..?
            Artesania arte = new Artesania();
            //arte.Id_Artesania = ? Es autogenerado! ish!
            arte.Descripcion = txtDescripcion.Text;
            arte.Material = (Material)cboMaterialPrincipal.SelectedItem;

            if (rdbNacional.IsChecked == true)
                arte.Procedencia = rdbNacional.Content.ToString();
            else
                arte.Procedencia = rdbImportado.Content.ToString();

            arte.PathImagen = imgPhoto.Source.ToString();

            //Guardamos la artesania
            datos.Artesania.Add(arte);
            datos.SaveChanges();

            //Como Artesania/Categoria se guarda en una tabla aparte (correlación), entonces a continuación hacemos eso...            
            //Que categorias checkeó?
            foreach (var chk in chksCategorias.Children.OfType<CheckBox>())
            {
                //Obtenemos la categoria de acuerdo al id, guardado en el tag:
                if(chk.IsChecked == true)
                {
                    int idCategoria = int.Parse(chk.Tag.ToString());
                    Categoria c = datos.Categoria.Find(idCategoria);

                    Artesania_Categoria ac = new Artesania_Categoria();
                    ac.Artesania = arte;
                    ac.Categoria = c;

                    datos.Artesania_Categoria.Add(ac);
                    datos.SaveChanges();
                }                
            }
            CargarDatosGrilla();
        }

        private void dgArtesanias_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(dgArtesanias.SelectedItem != null)
            {
                Artesania a = (Artesania)dgArtesanias.SelectedItem;

                txtId.Text = a.Id_Artesania.ToString();
                txtDescripcion.Text = a.Descripcion;
                cboMaterialPrincipal.SelectedItem = a.Material;

                foreach (var chk in chksCategorias.Children.OfType<CheckBox>())
                {
                    chk.IsChecked = false;
                }

                if (a.Procedencia.Equals("Nacional"))
                    rdbNacional.IsChecked = true;
                else
                    rdbImportado.IsChecked = true;

                //Cargamos la imagen
                String stringPath = a.PathImagen;
                Uri imageUri = new Uri(stringPath);
                BitmapImage imageBitmap = new BitmapImage(imageUri);                
                imgPhoto.Source = imageBitmap;

                //Vemos si tiene categorias asociadas...
                var artesaniasCategorias = a.Artesania_Categoria;
                
                if(artesaniasCategorias.Count > 0)
                {
                    foreach(var ac in artesaniasCategorias)
                    {
                        foreach (var chk in chksCategorias.Children.OfType<CheckBox>())
                        {
                            if (chk.Content.Equals(ac.Categoria.Nombre))                            
                                chk.IsChecked = true;                                                                                                                         
                        }
                    }
                }            
            }    
        }

        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            txtId.Text = string.Empty;
            txtDescripcion.Text = string.Empty;
            cboMaterialPrincipal.SelectedIndex = -1;
            rdbNacional.IsChecked = true;

            foreach (var chk in chksCategorias.Children.OfType<CheckBox>())
            {                           
                    chk.IsChecked = false;
            }

            imgPhoto.Source = null;
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (dgArtesanias.SelectedItem != null)
            {
                Artesania a = (Artesania)dgArtesanias.SelectedItem;

                //Eliminamos las categorias que tenga asociada actualmente..y todas las que seleccione (van a asociarse nuevamente)
                var arteCategorias = a.Artesania_Categoria;
                if (arteCategorias.Count > 0)
                {
                    datos.Artesania_Categoria.RemoveRange(arteCategorias);                    
                    datos.SaveChanges();
                }

                datos.Artesania.Remove(a);
                datos.SaveChanges();
                CargarDatosGrilla();
            }
            else
                MessageBox.Show("Debe seleccionar una Artesania de la grilla para eliminar!");
        }

        private void btnModificar_Click(object sender, RoutedEventArgs e)
        {
            if (dgArtesanias.SelectedItem != null)
            {
                Artesania a = (Artesania)dgArtesanias.SelectedItem;

                a.Descripcion = txtDescripcion.Text;
                a.Material = (Material)cboMaterialPrincipal.SelectedItem;

                if (rdbNacional.IsChecked == true)
                    a.Procedencia = rdbNacional.Content.ToString();
                else
                    a.Procedencia = rdbImportado.Content.ToString();

                a.PathImagen = imgPhoto.Source.ToString();
                
                //Le ponemos una banderita de que se modicaron datos en la entidad..
                datos.Entry(a).State = System.Data.Entity.EntityState.Modified;                
                datos.SaveChanges();

                //Eliminamos las categorias que tenga asociada actualmente..y todas las que seleccione (van a asociarse nuevamente)
                var arteCategorias = a.Artesania_Categoria;
                if(arteCategorias.Count > 0)
                {
                    datos.Artesania_Categoria.RemoveRange(arteCategorias);                    
                    datos.SaveChanges();
                }
  
                //Finalmente, actualizamos las categorias asociadas
                foreach (var chk in chksCategorias.Children.OfType<CheckBox>())
                {
                    //Obtenemos la categoria de acuerdo al id, guardado en el tag:
                    if (chk.IsChecked == true)
                    {
                        int idCategoria = int.Parse(chk.Tag.ToString());
                        Categoria c = datos.Categoria.Find(idCategoria);
                        Artesania_Categoria ac = new Artesania_Categoria();
                        ac.Artesania = a;
                        ac.Categoria = c;
                        datos.Artesania_Categoria.Add(ac);
                        datos.SaveChanges();
                    }                                       
                }                

                CargarDatosGrilla();
            }
            else
                MessageBox.Show("Debe seleccionar una Artesania de la grilla para modificar!");
        }
    }
}
