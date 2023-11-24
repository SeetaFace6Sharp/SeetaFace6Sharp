using Avalonia.Controls;
using SeetaFace6Sharp.Example.Camera.ViewModels;

namespace SeetaFace6Sharp.Example.Camera.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.Opened += (sender, e) =>
            {
                if (DataContext is MainWindowViewModel viewModel)
                {
                    viewModel.OnWindowOpened();
                }
            };
        }
    }
}