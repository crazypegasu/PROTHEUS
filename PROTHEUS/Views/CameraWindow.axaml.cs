using Avalonia.Controls;
using PROTHEUS.Models;

namespace PROTHEUS.Views
{
    public partial class CameraWindow : Window
    {
        public CameraWindow(int index)
        {
            InitializeComponent();

            var config = ConfigManager.Carregar();
            if (index < config.Count)
            {
                var camera = config[index];
                TxtCameraInfo.Text = $"Visualizando: {camera.Ip}";
            }
            else
            {
                TxtCameraInfo.Text = "Câmera não configurada.";
            }
        }
    }
}
