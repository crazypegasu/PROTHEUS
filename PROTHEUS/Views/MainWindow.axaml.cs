using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using PROTHEUS.Models;
using PROTHEUS.Views; // já está, pode manter


namespace PROTHEUS.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            BtnSair.Click += (_, _) => Close(); // Sair

            BtnAtualizar.Click += (_, _) => CarregarStatusDasCameras();


            BtnConfiguracoes.Click += (_, _) =>
            {
                var config = new ConfigWindow();
                config.ShowDialog(this);
            };

            // Carregar status das câmeras
            CarregarStatusDasCameras();
        }

        private async void CarregarStatusDasCameras()
        {
            var config = ConfigManager.Carregar();

            for (int i = 0; i < 8; i++)
            {
                var border = this.FindControl<Border>($"BorderCam{i + 1}");

                if (i < config.Count)
                {
                    var camera = config[i];
                    string status;

                    try
                    {
                        var ping = new Ping();
                        var reply = await ping.SendPingAsync(camera.Ip ?? "", 1000);
                        status = reply.Status == IPStatus.Success ? "🟢 Online" : "🔴 Offline";
                    }
                    catch
                    {
                        status = "🔴 Offline";
                    }

                    border.Child = new StackPanel
                    {
                        Children =
                        {
                            new TextBlock
                            {
                                Text = $"Câmera {i + 1}",
                                FontWeight = FontWeight.Bold,
                                Foreground = Brushes.White,
                                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
                            },
                            new TextBlock
                            {
                                Text = $"{camera.Ip} - {status}",
                                Foreground = Brushes.Gray,
                                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
                            }
                        }
                    };

                    // Adiciona evento de duplo clique para abrir visualização
                    border.PointerPressed += (s, e) =>
                    {
                        if (e.ClickCount == 2)
                            AbrirCameraEmTelaCheia(i);
                    };
                }
                else
                {
                    border.Child = new TextBlock
                    {
                        Text = "Não configurada",
                        Foreground = Brushes.DarkGray,
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
                    };
                }
            }
        }

        private void AbrirCameraEmTelaCheia(int index)
        {
            var janela = new CameraWindow(index);
            janela.ShowDialog(this);
        }
    }
}
