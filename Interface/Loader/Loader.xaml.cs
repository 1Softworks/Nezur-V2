using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using Newtonsoft.Json;

namespace NezurAimbot.Interface.Loader
{
    public partial class Loader : Window
    {
        public Loader() => InitializeComponent();

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (WebClient Http = new WebClient { })
            {
                if (LocalVersion != Http.DownloadString("https://nezur.net/version.txt"))
                {
                    MessageBox.Show("It Appears You're Using An Older Version Of Nezur, Please Visit Our Discord Or https://nezur.net To Download Our Latest Release.", "Nezur",
                        MessageBoxButton.OK, MessageBoxImage.Error);

                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "https://nezur.net",
                        UseShellExecute = true
                    });

                    this.Close();
                }
            }

            KeyStep.Visibility = Visibility.Visible;
            DownloadStep.Visibility = Visibility.Hidden;

            string[] Directories = new string[] { "Bin", "Logs", "Bin\\Config", "Bin\\Images", "Bin\\Aim", };

            for (int i = 0; i < Directories.Length; ++i)
                Directory.CreateDirectory(Directories[i]);

            Process.Start(new ProcessStartInfo
            {
                FileName = "https://1cheats.com/store/product/41-nezur-key-bypass-lifetime-license/",
                UseShellExecute = true
            });

            Main.Visibility = Visibility.Hidden;
            await LoadAnimations();
        }

        private async Task LoadAnimations()
        {
            await Task.Delay(500);
            Main.Visibility = Visibility.Visible;
            await Animations.AnimateVisibility(Main, Visibility.Visible);
            await Animations.AnimateVisibility(Main, Visibility.Visible);
        }

        public async Task DownloadFileAsync(string Url, string Destination, string Name)
        {
            if (File.Exists(Destination) == true)
                return;

            using (WebClient Http = new WebClient { Proxy = null })
            {
                Http.DownloadProgressChanged += (s, e) => DW.Content = $"Downloading {Name} - {e.ProgressPercentage}%";
                
                await Http.DownloadFileTaskAsync(
                    new Uri(Url),
                    Destination);
            }
        }

        private async void Enter_Click(object sender, RoutedEventArgs e)
        {
            string Key = "53aDhasjS39S_Djk9UASD9uWJI_sDJgWSU9sDGHW_sdg";
            string Key2 = "(53aDhasjS)39S)_D(jk)#9UASD9uWJI$)#(s)DJg)WS#%U9sDGHW)_#sdg";

            string UserInput = Input.Password;

            if (UserInput == Key || UserInput == Key2)
            {
                KeyStep.Visibility = Visibility.Hidden;
                DownloadStep.Visibility = Visibility.Visible;

                await DownloadFileAsync("https://raw.githubusercontent.com/0x9374698765254342/Models/main/Resources/DirectML.dll", "DirectML.dll", "DirectML.dll");
                await DownloadFileAsync("https://raw.githubusercontent.com/0x9374698765254342/Models/main/Resources/onnxruntime.dll", "onnxruntime.dll", "onnxruntime.dll");

                if (Directory.Exists($"{Environment.CurrentDirectory}\\Bin\\Models"))
                    Directory.Delete($"{Environment.CurrentDirectory}\\Bin\\Models", true);

                await DownloadFileAsync("https://nezur.net/immuneishot.zip", $"{Environment.CurrentDirectory}\\Bin\\Models.zip", "Models");
                ZipFile.ExtractToDirectory($"{Environment.CurrentDirectory}\\Bin\\Models.zip", $"{Environment.CurrentDirectory}\\Bin\\Models");

                File.Delete($"{Environment.CurrentDirectory}\\Bin\\Models.zip");

                if (File.Exists($"{Environment.CurrentDirectory}\\Bin\\Models\\Universal.onnx") != true)
                    MessageBox.Show("It Appears Your Isp Might Have Blocked Downloading Our Models, Please Use A Vpn And Try Again.", "Nezur",
                        MessageBoxButton.OK, MessageBoxImage.Error);

                new MainInterface().Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Your key is incorrect!");
            }
        }

        private void Get_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://nezur.net/NEZURV2",
                UseShellExecute = true
            });
        }

        private void Discord_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://discord.gg/nezurai",
                UseShellExecute = true
            });
        }

        private void Exit_Click(object sender, RoutedEventArgs e) => Close();

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = isMaximized ? previousState : WindowState.Maximized;
            isMaximized = !isMaximized;
        }

        private void Minimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void Drag(object sender, RoutedEventArgs e) => DragMove();
    }
}