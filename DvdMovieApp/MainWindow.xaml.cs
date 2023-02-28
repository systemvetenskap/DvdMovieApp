using DvdMovieApp.DAL;
using DvdMovieApp.Models;
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

namespace DvdMovieApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void btnOk_Click(object sender, RoutedEventArgs e)
        {
            //int value = int.Parse(textBox.Text);
            
            /* 
            {
                "ConnectionStrings": {
                    "develop": "Server=localhost;Port=5432;User Id=dvd_user;Password=hemligt;Database=dvd;"
                }
            }
            */
            DbRepository db = new();
            var films = await db.GetFilms();
            listBox.ItemsSource = films;
            listBox.DisplayMemberPath="Title";
            //var film = await db.GetFilmById();
            
            var category = new Category()
            {
                 Name="Barn"
            };
            try
            {
               //category = await db.AddCategory2(category);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void listBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listBox.SelectedItem is Film select)
            {
                MessageBox.Show(select.Film_id.ToString());
            }
        }
    }
}
