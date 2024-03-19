using System.IO;

namespace NezurAimbot;

public partial class App : Application
{
    public ThemeManager ThemeManager { get; } = new ThemeManager();

    protected override void OnStartup(StartupEventArgs e)
    {
        Current.DispatcherUnhandledException += (s, e) =>
        {
            string FileName = $"./Logs/Crashlog-{DateTime.Now.ToLongTimeString().Replace(':', '-')}.txt";

            using (StreamWriter Writer = new StreamWriter(FileName))
                Writer.WriteLine(e.Exception);

            e.Handled = true;
        };
    }
}