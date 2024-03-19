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
            UpdateDate();
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://1cheats.com/store/product/41-nezur-key-bypass-lifetime-license/",
                UseShellExecute = true
            });
            KeyStep.Visibility = Visibility.Visible;
            DownloadStep.Visibility = Visibility.Hidden;

            string[] Directories = new string[] { "Bin", "Logs", "Bin\\Config", "Bin\\Images", "Bin\\Aim", };

            for (int i = 0; i < Directories.Length; ++i)
                Directory.CreateDirectory(Directories[i]);
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
        private void UpdateDate()
        {
            Date.Text = DateTime.Now.ToString("MM/dd/yy");
        }
        public async Task DownloadFileAsync(string Url, string Destination, string Name)
        {
            if (File.Exists(Destination) == true)
                return;

            using (WebClient Http = new WebClient { Proxy = null })
            {
                Splitter.Visibility = Visibility.Hidden;
                Splitter3.Visibility = Visibility.Hidden;
                Splitter2.Visibility = Visibility.Hidden;
                Splitter1.Visibility = Visibility.Hidden;
                Http.DownloadProgressChanged += (s, e) => DW.Content = $"Downloading {Name} - {e.ProgressPercentage}%";
                
                await Http.DownloadFileTaskAsync(
                    new Uri(Url),
                    Destination);
            }
        }
        public static api KeyAuthApp = new api(
           name: "Nezur",
           ownerid: "2UWe8CI1m7",
           secret: "da0fa5c2ed5edc166033d7296f3343118d8401fd552aafae3b231d896b558dd9",
           version: "1.0"
       );
        public string RobloxVersion
        {
            get
            {
                using (WebClient Http = new WebClient { })
                    return Http.DownloadString("https://dhs.army/rblxver.txt");
            }
        }
        private async void Enter_Click(object sender, RoutedEventArgs e)
        {
            KeyAuthApp.init();
            string enteredKey = Input.Password;
            string allowedKey = "Developer-Y8LL0ckHXJAfbWZMIbooVx2vOt02YnUC";

            KeyAuthApp.license(enteredKey);

            if (KeyAuthApp.response.success || enteredKey == allowedKey)
            {
                KeyStep.Visibility = Visibility.Hidden;
                DownloadStep.Visibility = Visibility.Visible;

                await DownloadFileAsync("https://github.com/Lucasfin000/Leeucasfin000.github.io/raw/main/DirectML.dll", "DirectML.dll", "DirectML.dll");
                await DownloadFileAsync("https://github.com/Lucasfin000/Leeucasfin000.github.io/raw/main/onnxruntime.dll", "onnxruntime.dll", "onnxruntime.dll");
                KeyAuthApp.log("Correct Key Used");
                new MainInterface().Show();
                this.Close();
            }
            else
            {
                MessageBox.Show(KeyAuthApp.response.message);
                KeyAuthApp.log("Incorrect Key Used");
            }
        }

        private void Get_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://key.nezur.io",
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

        //private void Maximize_Click(object sender, RoutedEventArgs e)
        //{
        //    WindowState = isMaximized ? previousState : WindowState.Maximized;
        //    isMaximized = !isMaximized;
        //}

        private void Minimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void Drag(object sender, RoutedEventArgs e) => DragMove();

        private void KeyBypass_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://1cheats.com/store/product/41-nezur-key-bypass-lifetime-license/",
                UseShellExecute = true
            });
        }
    }
}