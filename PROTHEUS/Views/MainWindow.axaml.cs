using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Controls.Primitives; // Para ToolTip
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using PROTHEUS.Models;
using LibVLCSharp.Shared;
using LibVLCSharp.Avalonia;
using System;

namespace PROTHEUS.Views
{
    public partial class MainWindow : Window
    {
        private LibVLC _libVLC;
        private Dictionary<int, MediaPlayer> _mediaPlayers = new();

        public MainWindow()
        {
            InitializeComponent();

            // NÃO chame Core.Initialize() aqui — faça isso no Program.cs antes de BuildAvaloniaApp()
            _libVLC = new LibVLC();

            BtnSair.Click += (_, _) => Close();

            BtnAtualizar.Click += async (_, _) => await CarregarStatusDasCamerasAsync();

            BtnConfiguracoes.Click += (_, _) =>
            {
                var config = new ConfigWindow();
                config.ShowDialog(this);
            };

            // Carregamento inicial das câmeras, sem await para não travar a UI
            _ = CarregarStatusDasCamerasAsync();
        }

        private async Task CarregarStatusDasCamerasAsync()
        {
            var config = ConfigManager.Carregar();

            for (int i = 0; i < 8; i++)
            {
                var border = this.FindControl<Border>($"BorderCam{i + 1}");
                var videoView = this.FindControl<VideoView>($"VideoView{i + 1}");

                if (border == null || videoView == null)
                {
                    Console.WriteLine($"[WARN] Border ou VideoView não encontrado para índice {i + 1}");
                    continue;
                }

                if (i < config.Count)
                {
                    CameraConfig camera = config[i];
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

                    // Define tooltip corretamente (attached property)
                    ToolTip.SetTip(border, $"Câmera {i + 1} - {camera.Ip} - {status}");

                    // Limpa conteúdo anterior e insere só o VideoView para garantir que vídeo apareça
                    border.Child = videoView;

                    IniciarPreviewCamera(i, camera, videoView);
                }
                else
                {
                    ToolTip.SetTip(border, "Não configurada");

                    if (_mediaPlayers.ContainsKey(i))
                    {
                        _mediaPlayers[i].Stop();
                        _mediaPlayers[i].Dispose();
                        _mediaPlayers.Remove(i);
                    }

                    videoView.MediaPlayer = null;

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

        private void IniciarPreviewCamera(int index, CameraConfig camera, VideoView videoView)
        {
            if (_mediaPlayers.ContainsKey(index))
            {
                _mediaPlayers[index].Stop();
                _mediaPlayers[index].Dispose();
                _mediaPlayers.Remove(index);
            }

            try
            {
                var mediaPlayer = new MediaPlayer(_libVLC);
                videoView.MediaPlayer = mediaPlayer;

                string rtspUrl = $"rtsp://{camera.Usuario}:{camera.Senha}@{camera.Ip}:554/cam/realmonitor?channel=1&subtype=0";

                Console.WriteLine($"[INFO] Tentando iniciar stream câmera {index + 1}: {rtspUrl}");

                var media = new Media(_libVLC, rtspUrl, FromType.FromLocation);
                mediaPlayer.Play(media);

                _mediaPlayers[index] = mediaPlayer;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao iniciar preview da câmera {index + 1}: {ex.Message}");
            }
        }
    }
}
