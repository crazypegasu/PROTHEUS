using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using PROTHEUS.ViewModels;
using PROTHEUS.Views;
using System;

namespace PROTHEUS
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            Console.WriteLine(">> Inicializando AvaloniaXamlLoader...");
            AvaloniaXamlLoader.Load(this);
            Console.WriteLine(">> AvaloniaXamlLoader concluído.");
        }

        public override void OnFrameworkInitializationCompleted()
        {
            Console.WriteLine(">> Entrou em OnFrameworkInitializationCompleted");

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                Console.WriteLine(">> ApplicationLifetime identificado como Desktop");

                DisableAvaloniaDataAnnotationValidation();
                Console.WriteLine(">> Validações duplicadas desativadas");

                // Remova temporariamente o ViewModel para testar se é ele que trava
                desktop.MainWindow = new MainWindow();
                Console.WriteLine(">> MainWindow atribuída");
            }
            else
            {
                Console.WriteLine(">> ApplicationLifetime NÃO é IClassicDesktopStyleApplicationLifetime");
            }

            base.OnFrameworkInitializationCompleted();
            Console.WriteLine(">> Finalizou OnFrameworkInitializationCompleted");
        }

        private void DisableAvaloniaDataAnnotationValidation()
        {
            var dataValidationPluginsToRemove =
                BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

            foreach (var plugin in dataValidationPluginsToRemove)
            {
                BindingPlugins.DataValidators.Remove(plugin);
            }
        }
    }
}
