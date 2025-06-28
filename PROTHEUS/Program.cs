using Avalonia;
using LibVLCSharp.Shared;
using System;

namespace PROTHEUS;

sealed class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        Core.Initialize();  // LibVLCSharp initialize aqui

        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
