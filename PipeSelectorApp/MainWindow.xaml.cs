using Microsoft.EntityFrameworkCore;
using PipeSelectorApp.ViewModels;
using System.Windows;

namespace PipeSelectorApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void InitializeDataContext(PipesDbContext context)
        {
            DataContext = new PipesPerHourViewModel(context);
        }
    }
}