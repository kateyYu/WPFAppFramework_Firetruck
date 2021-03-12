/*
 * Dabas, Anil
 * Saxena, Ritesh
 * Seo, Olivia
 * Yu, Katey
 */
using System.Windows;

namespace Firetruck
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly VM vm = new VM();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
