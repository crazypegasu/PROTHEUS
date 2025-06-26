using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PROTHEUS.Models;
using System.Collections.Generic;
using PROTHEUS.Models;

namespace PROTHEUS.Views
{
    public partial class ConfigWindow : Window
    {
        private readonly List<TextBox> _ipBoxes = new();
        private readonly List<TextBox> _usuarioBoxes = new();
        private readonly List<TextBox> _senhaBoxes = new();

        public ConfigWindow()
        {
            InitializeComponent();
            CarregarCampos();
        }

        private void CarregarCampos()
        {
            var painel = this.FindControl<StackPanel>("PainelConfiguracoes");

            var config = ConfigManager.Carregar();
            for (int i = 0; i < 8; i++)
            {
                var dados = i < config.Count ? config[i] : new CameraConfig();

                painel.Children.Add(new TextBlock { Text = $"Câmera {i + 1}" });

                var ipBox = new TextBox { Watermark = "Endereço IP", Text = dados.Ip };
                var usuarioBox = new TextBox { Watermark = "Usuário", Text = dados.Usuario };
                var senhaBox = new TextBox { Watermark = "Senha", Text = dados.Senha };

                painel.Children.Add(ipBox);
                painel.Children.Add(usuarioBox);
                painel.Children.Add(senhaBox);

                _ipBoxes.Add(ipBox);
                _usuarioBoxes.Add(usuarioBox);
                _senhaBoxes.Add(senhaBox);
            }

            var botaoSalvar = new Button { Content = "Salvar", Margin = new Thickness(0, 20, 0, 0), HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right };
            botaoSalvar.Click += SalvarConfiguracoes;
            painel.Children.Add(botaoSalvar);
        }

        private void SalvarConfiguracoes(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var lista = new List<CameraConfig>();
            for (int i = 0; i < 8; i++)
            {
                lista.Add(new CameraConfig
                {
                    Ip = _ipBoxes[i].Text ?? "",
                    Usuario = _usuarioBoxes[i].Text ?? "",
                    Senha = _senhaBoxes[i].Text ?? ""
                });
            }

            ConfigManager.Salvar(lista);
            Close(); // fecha a janela depois de salvar
        }
    }
}
