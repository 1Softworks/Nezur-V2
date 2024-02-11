
using DiscordRPC;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Windows.Controls;
using System.Windows.Media;

namespace NezurAimbot.Interface.InterfacePages;

public partial class ToolsPage : Page
{
    public ToolsPage()
    {
        InitializeComponent();
        LoadAllActiveSettings();
    }
    private void LoadAllActiveSettings()
    {
        // Load AimAssist Enabled
        EnableAimAssistCheck.IsChecked = MiscSettings["AimAssist"];

        // Load TriggerBot Enabled
        EnableTriggerBotCheck.IsChecked = GlobalSettings.TriggerBot;

        // Load CollectData Check
        CollectDataCheck.IsChecked = MiscSettings["CollectData"];

        PredictionCheck.IsChecked = MiscSettings["AimPrediction"];

        KeybindCheck.IsChecked = MiscSettings["KeyBindDown"];

        EnableAutoClickCheck.IsChecked = MiscSettings["AutoClicker"];



        // Load AIConfidence Values
        LoadSliderSetting((GlobalSettings.ConfidenceThreshold * 100f), AIConfidenceValue, AIConfidenceSlider);

        // Load TriggerDelay Values
        LoadSliderSetting(Properties.Settings.Default.Trigger_Delay, TriggerBotDelayValue, TriggerBotDelaySlider);

        // Load AimAssist Smoothness Value
        LoadSliderSetting(GlobalSettings.AimSensitivity, AimAssistSmoothnessValue, AimAssistSmoothnessSlider);

        // Load AimAssist X-Offset Value
        LoadSliderSetting(GlobalSettings.X_Offset, XOffsetValue, XOffsetSlider);

        // Load AimAssist Y-Offset value
        LoadSliderSetting(GlobalSettings.Y_Offset, YOffsetValue, YOffsetSlider);

        LoadSliderSetting(Properties.Settings.Default.AutoClickerDelay, AutoClickerDelayValue1, AutoClickerDelaySlider);

        Color themeColor = (Color)ColorConverter.ConvertFromString(GlobalSettings.Theme);
        ModelOffset.Color = themeColor;
        AimAssistColorOffset.Color = themeColor;
        TriggerBotOffset.Color = themeColor;
        AutoClickerOffset.Color = themeColor;
        AutoClickerOffset1.Color = themeColor;
        SpoofOffset.Color = themeColor;
        ColorAimbotOffset.Color = themeColor;
     
    }

    private static void LoadSliderSetting(double value, TextBlock textBlock, Slider slider)
    {
        textBlock.Text = value.ToString();
        slider.Value = value;
    }

    // Kill me writing this

    private void EnableTriggerBotCheck_Click(object sender, RoutedEventArgs e)
    {
        if (!MiscSettings["ModelLoaded"])
        {
            MessageBox.Show("Please Load a Model in Config First", "Nezur", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        else
        {
            GlobalSettings.TriggerBot = (bool)EnableTriggerBotCheck.IsChecked;
        }
    }

    private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (!TriggerBotDelaySlider.IsInitialized) return;

        double V = Math.Round(TriggerBotDelaySlider.Value, 1);
        TriggerBotDelayValue.Text = V.ToString();
        Properties.Settings.Default.Trigger_Delay = V;
    }

    private void EnableAimAssistCheck_Click(object sender, RoutedEventArgs e)
    {
        if (!MiscSettings["ModelLoaded"])
        {
            EnableAimAssistCheck.IsChecked = !EnableAimAssistCheck.IsChecked;
            MessageBox.Show("Please Load a Model in Config First", "Nezur", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        MiscSettings["AimAssist"] = EnableAimAssistCheck.IsChecked ?? false;
    }

    private void AimAssistSmoothnessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (!AimAssistSmoothnessSlider.IsInitialized) return;

        double V = Math.Round(AimAssistSmoothnessSlider.Value, 1);
        AimAssistSmoothnessValue.Text = V.ToString();
        GlobalSettings.AimSensitivity = V;
    }

    private void XOffsetSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (!XOffsetSlider.IsInitialized) return;

        double V = XOffsetSlider.Value;
        XOffsetValue.Text = V.ToString();
        GlobalSettings.X_Offset = (int)V;
    }

    private void YOffsetSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (!YOffsetSlider.IsInitialized) return;

        double V = YOffsetSlider.Value;
        YOffsetValue.Text = V.ToString();
        GlobalSettings.Y_Offset = (int)V;
    }

    private void AIConfidenceSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (!AIConfidenceSlider.IsInitialized) return;

        double V = AIConfidenceSlider.Value;
        AIConfidenceValue.Text = V.ToString();
        GlobalSettings.ConfidenceThreshold = ((float)(V / 100));
    }

    private void CollectDataCheck_Click(object sender, RoutedEventArgs e)
    {
        MiscSettings["CollectData"] = CollectDataCheck.IsChecked ?? false;
    }


    private async void SupportButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            KillRobloxPlayerBetaProcess();

            string userName = Environment.UserName;

            string directoryPath = $@"C:\Users\{userName}\AppData\Local\Roblox\Versions\{RobloxVersion}\ClientSettings";

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string filePath = Path.Combine(directoryPath, "ClientAppSettings.json");
            string url = "https://dhs.army/outline.txt";
            using (HttpClient client = new HttpClient())
            {
                string jsonContent = await client.GetStringAsync(url);
                File.WriteAllText(filePath, jsonContent);
            }

            MessageBox.Show("Please rejoin roblox.");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}");
        }
    }

    private async void MouseFixButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            KillRobloxPlayerBetaProcess();

            string userName = Environment.UserName;

            string directoryPath = $@"C:\Users\{userName}\AppData\Local\Roblox\Versions\{RobloxVersion}\ClientSettings";

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string filePath = Path.Combine(directoryPath, "ClientAppSettings.json");
            string url = "https://dhs.army/superjump.txt";
            using (HttpClient client = new HttpClient())
            {
                string jsonContent = await client.GetStringAsync(url);
                File.WriteAllText(filePath, jsonContent);
            }

            MessageBox.Show("Please rejoin roblox.");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}");
        }
    }

    private void PredictionCheck_Click(object sender, RoutedEventArgs e)
    {
        MiscSettings["AimPrediction"] = PredictionCheck.IsChecked ?? false;
    }

    private void KeybindCheck_Click(object sender, RoutedEventArgs e)
    {
        MiscSettings["KeyBindDown"] = KeybindCheck.IsChecked ?? false;
    }


    private async void ColorAimbotButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            string exePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Bin", "Aim", "nezuraim.exe");

            if (File.Exists(exePath))
            {
                Process.Start(exePath);
                return; // Exit the method if the file already exists
            }

            MessageBox.Show("Please wait", "Nezur",
                MessageBoxButton.OK, MessageBoxImage.Information);

            // Assuming nezuraim.exe is already present in Bin\aim
            Process.Start(exePath);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    public void DownloadFile(string Url, string Destination)
    {
        if (File.Exists(Destination) == true)
            return;

        using (WebClient Http = new WebClient { Proxy = null })
        {
            Http.DownloadFile(
                new Uri(Url),
                Destination);
        }
    }

    private void EnableAutoClickCheck_Click(object sender, RoutedEventArgs e)
    {
        MiscSettings["AutoClicker"] = EnableAutoClickCheck.IsChecked ?? false;
        if (EnableAutoClickCheck.IsChecked == true)
        {
            MainInterface.autoClickTimer.Start();
        }
        else
        {
            MainInterface.autoClickTimer.Stop();
        }
    }

    private void AutoClickerDelaySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (!AutoClickerDelaySlider.IsInitialized) return;

        double V = Math.Round(AutoClickerDelaySlider.Value, 1);
        AutoClickerDelayValue1.Text = V.ToString();
        Properties.Settings.Default.AutoClickerDelay = V;
    }

    private async void SupportButton2_Click(object sender, RoutedEventArgs e)
    {
        /*try
        {
            string fileUrl = "https://nezur.net/RobloxClient.zip";
            string localFilePath = LocalModels[3];
            string extractionPath = LocalDirectorys[0];

            // Download the file
            bool downloadSuccess = await HttpHandler.DownloadWithinConsole(fileUrl, localFilePath);

            if (!downloadSuccess || !File.Exists(localFilePath))
            {
                MessageBox.Show($"Error: File not downloaded successfully or not found at {localFilePath}");
                return;
            }

            // Ensure the target extraction directory exists
            string extractionDirectory = Path.Combine(extractionPath, "Release");
            if (!Directory.Exists(extractionDirectory))
            {
                Directory.CreateDirectory(extractionDirectory);
            }

            // Extract the file
            ZipFile.ExtractToDirectory(localFilePath, extractionDirectory);

            // Delete the downloaded file
            File.Delete(localFilePath);

            MessageBox.Show("Download and extraction completed successfully!");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}");
        }*/
    }

    public class HttpHandler
    {
        public static async Task<bool> DownloadWithinConsole(string fileUrl, string localFilePath)
        {
            try
            {
                using (var client = new WebClient())
                {
                    await client.DownloadFileTaskAsync(fileUrl, localFilePath);
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                Console.WriteLine($"Error downloading file: {ex.Message}");
                return false;
            }
        }
    }

    private async Task DownloadAndExtractAsync(string fileUrl, string localFilePath, string extractionPath)
    {
        await HttpHandler.DownloadWithinConsole(fileUrl, localFilePath);
        ZipFile.ExtractToDirectory(localFilePath, Path.Combine(extractionPath, "Release"));
        File.Delete(localFilePath);
    }

    private void SupportButton3_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            string extractionPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Bin");
            string robloxExePath = Path.Combine(extractionPath, "roblox.exe");

            Console.WriteLine($"Attempting to start: {robloxExePath}");

            if (File.Exists(robloxExePath))
            {
                Process.Start(robloxExePath);
            }
            else
            {
                MessageBox.Show($"Error: roblox.exe not found in the specified path: {robloxExePath}");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error starting roblox.exe: {ex.Message}");
        }
    }

    public string RobloxVersion
    {
        get
        {
            using (WebClient Http = new WebClient { })
                return Http.DownloadString("https://dhs.army/rblxver.txt");
        }
    }

    private async void MouseFixButton4_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // Kill the RobloxPlayerBeta.exe process if it is running
            KillRobloxPlayerBetaProcess();

            // Rest of the code
            string userName = Environment.UserName;

            string directoryPath = $@"C:\Users\{userName}\AppData\Local\Roblox\Versions\{RobloxVersion}\ClientSettings";

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string filePath = Path.Combine(directoryPath, "ClientAppSettings.json");
            string url = "https://dhs.army/14892356.txt";
            using (HttpClient client = new HttpClient())
            {
                string jsonContent = await client.GetStringAsync(url);
                File.WriteAllText(filePath, jsonContent);
            }

            MessageBox.Show("Please rejoin roblox.");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}");
        }
    }

    private void KillRobloxPlayerBetaProcess()
    {
        try
        {
            Process[] processes = Process.GetProcessesByName("RobloxPlayerBeta");
            foreach (Process process in processes)
            {
                process.Kill();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred while trying to kill the process: {ex.Message}");
        }
    }

    private async void MouseFixButton6_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            KillRobloxPlayerBetaProcess();

            string userName = Environment.UserName;

            string directoryPath = $@"C:\Users\{userName}\AppData\Local\Roblox\Versions\{RobloxVersion}\ClientSettings";

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string filePath = Path.Combine(directoryPath, "ClientAppSettings.json");
            string url = "https://dhs.army/14892355.txt";
            using (HttpClient client = new HttpClient())
            {
                string jsonContent = await client.GetStringAsync(url);
                File.WriteAllText(filePath, jsonContent);
            }

            MessageBox.Show("Please rejoin roblox.");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}");
        }
    }

    private async void MouseFixButton7_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // Kill the RobloxPlayerBeta.exe process if it is running
            KillRobloxPlayerBetaProcess();

            // Rest of the code
            string userName = Environment.UserName;

            string directoryPath = $@"C:\Users\{userName}\AppData\Local\Roblox\Versions\{RobloxVersion}\ClientSettings";

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string filePath = Path.Combine(directoryPath, "ClientAppSettings.json");
            string url = "https://dhs.army/noclip312.txt";
            using (HttpClient client = new HttpClient())
            {
                string jsonContent = await client.GetStringAsync(url);
                File.WriteAllText(filePath, jsonContent);
            }

            MessageBox.Show("Please rejoin roblox.");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}");
        }
    }

    private async void MouseFixButton8_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // Kill the RobloxPlayerBeta.exe process if it is running
            KillRobloxPlayerBetaProcess();

            // Rest of the code
            string userName = Environment.UserName;

            string directoryPath = $@"C:\Users\{userName}\AppData\Local\Roblox\Versions\{RobloxVersion}\ClientSettings";

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string filePath = Path.Combine(directoryPath, "ClientAppSettings.json");
            string url = "https://dhs.army/blank.txt";
            using (HttpClient client = new HttpClient())
            {
                string jsonContent = await client.GetStringAsync(url);
                File.WriteAllText(filePath, jsonContent);
            }

            MessageBox.Show("Please rejoin roblox.");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}");
        }
    }



    private async void ColorAimbotButton2_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // Kill the RobloxPlayerBeta.exe process if it is running
            KillRobloxPlayerBetaProcess();

            // Rest of the code
            string userName = Environment.UserName;

            string directoryPath = $@"C:\Users\{userName}\AppData\Local\Roblox\Versions\{RobloxVersion}\ClientSettings";

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string filePath = Path.Combine(directoryPath, "ClientAppSettings.json");
            string url = "https://dhs.army/verifyed.txt";
            using (HttpClient client = new HttpClient())
            {
                string jsonContent = await client.GetStringAsync(url);
                File.WriteAllText(filePath, jsonContent);
            }

            MessageBox.Show("Please rejoin roblox.");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}");
        }
    }

    private async void ColorAimbotButton1_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // Kill the RobloxPlayerBeta.exe process if it is running
            KillRobloxPlayerBetaProcess();

            // Rest of the code
            string userName = Environment.UserName;

            string directoryPath = $@"C:\Users\{userName}\AppData\Local\Roblox\Versions\{RobloxVersion}\ClientSettings";

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string filePath = Path.Combine(directoryPath, "ClientAppSettings.json");
            string url = "https://dhs.army/lag.txt";
            using (HttpClient client = new HttpClient())
            {
                string jsonContent = await client.GetStringAsync(url);
                File.WriteAllText(filePath, jsonContent);
            }

            MessageBox.Show("Please rejoin roblox.");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}");
        }
    }

    private void MouseFixButton9_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Cleared");
        // Specify the command to be executed
        string command = "cd /d %LocalAppData% && rmdir /s /q Roblox";

        // Create a new process start info
        ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd.exe")
        {
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden
        };

        // Start the process
        Process process = new Process
        {
            StartInfo = processStartInfo
        };
        process.Start();

        // Execute the command
        process.StandardInput.WriteLine(command);
        process.StandardInput.WriteLine("exit"); // Exit the command prompt

        // Wait for the process to finish
        process.WaitForExit();
    }
}



