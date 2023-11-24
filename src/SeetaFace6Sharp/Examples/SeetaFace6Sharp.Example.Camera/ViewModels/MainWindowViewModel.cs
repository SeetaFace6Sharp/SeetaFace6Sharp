using System.Windows.Input;
using Epoxy;

namespace SeetaFace6Sharp.Example.Camera.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public CaptureControlViewModel CaptureControl { get; } = new CaptureControlViewModel();

        public void OnWindowOpened()
        {
            
        }
    }
}
