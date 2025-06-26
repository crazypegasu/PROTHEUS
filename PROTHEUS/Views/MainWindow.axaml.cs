using Avalonia.Controls;
using PROTHEUS.Views;

namespace PROTHEUS.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        BtnSair.Click += (_, _) => Close(); // quando pressio o botao sair, fecha a interface

        BtnConfiguracoes.Click += (_, _) =>
        {
            var config = new ConfigWindow();
            config.ShowDialog(this);
        };

    }
}