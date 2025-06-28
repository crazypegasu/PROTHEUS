using Emgu.CV;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using PROTHEUS.Models;

// Alias para Bitmaps
using SDBitmap = System.Drawing.Bitmap;
using AvaloniaBitmap = Avalonia.Media.Imaging.Bitmap;

namespace PROTHEUS.Views
{
    public partial class CameraWindow : Window
    {
        private VideoCapture? _capture;
        private CancellationTokenSource? _cts;

        public CameraWindow() // Construtor padrão (obrigatório para Avalonia)
        {
            InitializeComponent();
        }

        public CameraWindow(int index) : this()
        {
            LoadCamera(index);
        }

        private void LoadCamera(int index)
        {
            var config = ConfigManager.Carregar();

            if (index < 0 || index >= config.Count)
            {
                TxtCameraInfo.Text = "Câmera não configurada.";
                return;
            }

            var camera = config[index];

            // Ping para status
            string status;
            try
            {
                var ping = new Ping();
                var reply = ping.Send(camera.Ip ?? "", 1000);
                status = reply.Status == IPStatus.Success ? "🟢 Online" : "🔴 Offline";
            }
            catch
            {
                status = "🔴 Offline";
            }

            TxtCameraInfo.Text = $"Visualizando: {camera.Ip} - {status}";

            string rtspUrl = $"rtsp://{camera.Usuario}:{camera.Senha}@{camera.Ip}:554/cam/realmonitor?channel=1&subtype=0";

            StartStream(rtspUrl);
        }

        private void StartStream(string rtspUrl)
        {
            _cts?.Cancel();
            _capture?.Dispose();

            _cts = new CancellationTokenSource();

            try
            {
                _capture = new VideoCapture(rtspUrl);
            }
            catch (Exception ex)
            {
                TxtCameraInfo.Text = $"Erro ao iniciar stream: {ex.Message}";
                return;
            }

            Task.Run(async () =>
            {
                try
                {
                    while (!_cts.Token.IsCancellationRequested)
                    {
                        using var frame = _capture.QueryFrame();

                        if (frame != null && !frame.IsEmpty)
                        {
                            using SDBitmap bitmap = frame.ToBitmap();

                            using var ms = new MemoryStream();
                            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                            ms.Position = 0;

                            var avaloniaBitmap = await Dispatcher.UIThread.InvokeAsync(() => new AvaloniaBitmap(ms));

                            await Dispatcher.UIThread.InvokeAsync(() =>
                            {
                                VideoImage.Source = avaloniaBitmap;
                            });
                        }

                        await Task.Delay(33, _cts.Token); // Aproximadamente 30 FPS
                    }
                }
                catch (OperationCanceledException)
                {
                    // Cancelamento esperado
                }
                catch (Exception ex)
                {
                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        TxtCameraInfo.Text = $"Erro durante streaming: {ex.Message}";
                    });
                }
            }, _cts.Token);
        }

        protected override void OnClosed(EventArgs e)
        {
            _cts?.Cancel();
            _capture?.Dispose();
            base.OnClosed(e);
        }
    }
}
