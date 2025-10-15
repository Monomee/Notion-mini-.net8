using NotionMini.Models;
using NotionMini.Services;
using NotionMini.ViewModels;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NotionMini
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //DataContext = new MainViewModel();
            var vm = new MainViewModel();
            DataContext = vm;

            Loaded += async (s, e) =>
            {
                await vm.InitializeAsync();
            };
        }
  
    }
}