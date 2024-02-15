using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.CognitiveServices.Speech;

namespace NezurAimbot.Interface.InterfacePages;

public partial class SettingsPage : Page
{

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);
    private const uint SWP_NOMOVE = 0x0002;
    private const uint SWP_NOSIZE = 0x0001;
    private const uint TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

    public SettingsPage()
    {
        InitializeComponent();
        LoadActiveSettings();
    }

    private void LoadActiveSettings()
    {
        EnableFOVCheck.IsChecked = MiscSettings["ShowFOV"];
        EnableStreamable.IsChecked = MiscSettings["Streamable"];
        EnableTopMost.IsChecked = MiscSettings["TopMost"];
        
        //EnableDevMode.IsChecked = MiscSettings["DevMode"];
        LoadSliderSetting(GlobalSettings.FOVSize, FOVSizeValue, FOVSizeSlider);
        LoadSliderSetting(GlobalSettings.FOVThickness, FOVThickessValue, FOVThickness);
        LoadSliderSetting(GlobalSettings.FOVOpacity, FOVOpacityValue, FOVOpacity);

        InputButton.Content = Properties.Settings.Default.Current_Binding;

        Color themeColor = (Color)ColorConverter.ConvertFromString(GlobalSettings.Theme);

        FOVOffset.Color = themeColor;
        InputOffset.Color = themeColor;
        ThemeColorOffset.Color = themeColor;
        ConfigOffset.Color = themeColor;
        DiscordOffset.Color = themeColor;
        ColorThemeBorder.Background = new SolidColorBrush(themeColor);
    }

    private static void LoadSliderSetting(double value, TextBlock textBlock, Slider slider)
    {
        textBlock.Text = value.ToString();
        slider.Value = value;
    }

    private void EnableFOVCheck_Click(object sender, RoutedEventArgs e)
    {
        if (EnableFOVCheck.IsChecked == true) MainInterface.ShowFOV();
        if (EnableFOVCheck.IsChecked == false) MainInterface.HideFOV();
    }

    private void FOVSizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (!FOVSizeSlider.IsInitialized) return;

        double V = FOVSizeSlider.Value;
        FOVSizeValue.Text = V.ToString();
        GlobalSettings.FOVSize = (int)V;
    }

    private async void InputButton_Click(object sender, RoutedEventArgs e)
    {
        InputButton.Content = "Listening..";

        string binding = await MainInterface.GetNewInputBindingAsync();

        InputButton.Content = binding;
        Properties.Settings.Default.Current_Binding = binding;
    }

    private void ResetButton_Click(object sender, RoutedEventArgs e)
    {
        GlobalSettings.FOVSize = (int)DefaultSettings["FOV_Size"];
        GlobalSettings.Y_Offset = (int)DefaultSettings["Y_Offset"];
        GlobalSettings.X_Offset = (int)DefaultSettings["X_Offset"];
        Properties.Settings.Default.Trigger_Delay = DefaultSettings["Trigger_Delay"];
        GlobalSettings.ConfidenceThreshold = (float)DefaultSettings["AI_Min_Conf"] / 100;
        GlobalSettings.AimSensitivity = DefaultSettings["AimAssist_Smoothness"];
        Properties.Settings.Default.Current_Binding = DefaultBinding;
        GlobalSettings.Theme = DefaultThemeColor;
        Properties.Settings.Default.AutoClickerDelay = DefaultSettings["AutoClicker"];
        GlobalSettings.Streamable = false;
        GlobalSettings.TopMost = false;
        GlobalSettings.VC = false;
        //GlobalSettings.DevMode = false;
        LoadActiveSettings();
        MainInterface.ResetBinding();
    }

    private void ChangeColorButton_Click(object sender, RoutedEventArgs e)
    {
        System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
        System.Windows.Forms.DialogResult result = colorDialog.ShowDialog();

        if (result == System.Windows.Forms.DialogResult.OK)
        {
            System.Drawing.Color? selectedColor = colorDialog.Color;

            if (selectedColor.HasValue)
            {
                GlobalSettings.Theme = ConvertColorToHexString(selectedColor.Value);
                LoadActiveSettings();
            }
        }
    }

    private static string ConvertColorToHexString(System.Drawing.Color color)
    {
        return $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
    }
    private void LoadConfigButton_Click(object sender, RoutedEventArgs e)
    {
        // Prompt the user to select a JSON file using OpenFileDialog
        Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "JSON Files (*.json)|*.json",
            Title = "Load Configuration",
            InitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Bin", "Config") // Set the initial directory
        };

        if (openFileDialog.ShowDialog() == true)
        {
            string filePath = openFileDialog.FileName;

            ConfigSave loadedConfig = LoadConfigFromJson(filePath);

            GlobalSettings.FOVSize = (int)loadedConfig.FovSize;
            GlobalSettings.AimSensitivity = loadedConfig.Smoothness;
            GlobalSettings.X_Offset = (int)loadedConfig.XOffset;
            GlobalSettings.Y_Offset = (int)loadedConfig.YOffset;
            Properties.Settings.Default.Trigger_Delay = loadedConfig.TriggerDelay;
            GlobalSettings.ConfidenceThreshold = (float)loadedConfig.Confidence / 100;
            GlobalSettings.Streamable = loadedConfig.Streamable;
            GlobalSettings.TopMost = loadedConfig.TopMost;
       
            //GlobalSettings.DevMode = loadedConfig.DevMode;
        }
        LoadActiveSettings();
    }
    private static ConfigSave LoadConfigFromJson(string filePath)
    {
        string json = File.ReadAllText(filePath);

        // Deserialize the JSON content to a ConfigSave object
        ConfigSave loadedConfig = JsonConvert.DeserializeObject<ConfigSave>(json);

        return loadedConfig;
    }

    private void SaveConfigButton_Click(object sender, RoutedEventArgs e)
    {
        ConfigSave save = new();
        {
            save.FovSize = GlobalSettings.FOVSize;
            save.Smoothness = GlobalSettings.AimSensitivity;
            save.XOffset = GlobalSettings.X_Offset;
            save.YOffset = GlobalSettings.Y_Offset;
            save.TriggerDelay = Properties.Settings.Default.Trigger_Delay;
            save.Confidence = GlobalSettings.ConfidenceThreshold * 100;
            save.Streamable = GlobalSettings.Streamable;
            save.TopMost = GlobalSettings.TopMost;
            //save.DevMode = GlobalSettings.DevMode;
        }

        // Prompt the user for a file name using a SaveFileDialog
        Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog
        {
            Filter = "JSON Files (*.json)|*.json",
            DefaultExt = "json",
            Title = "Save Configuration",
            InitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Bin", "Config") // Set the initial directory
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            string filePath = saveFileDialog.FileName;

            // Save the configuration to the specified file
            SaveConfigToJson(filePath, save);
        }
    }

    private static async void SaveConfigToJson(string filePath, ConfigSave config)
    {
        string json = JsonConvert.SerializeObject(config, Newtonsoft.Json.Formatting.Indented);
        File.WriteAllText(filePath, json);
    }

    /*private NezurAimbot.ESP.ESP espOverlay;

    private void EnableESP_Click(object sender, RoutedEventArgs e)
    {
        if (EnableESP.IsChecked == true)
        {
            if (espOverlay == null)
            {
                espOverlay = new NezurAimbot.ESP.ESP();
                espOverlay.Closed += (s, args) => espOverlay = null;
                espOverlay.Show();
            }
        }
        else
        {
            espOverlay?.Close();
        }
    }*/

    private void FOVThickness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (!FOVThickness.IsInitialized) return;

        double V = FOVThickness.Value;
        FOVThickessValue.Text = V.ToString();
        GlobalSettings.FOVThickness = (int)V;
    }

    private void FOVOpacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (!FOVOpacity.IsInitialized) return;

        float V = (float)FOVOpacity.Value;
        FOVOpacityValue.Text = V.ToString();
        GlobalSettings.FOVOpacity = V;
    }

    private void SaveConfigButton1_Click(object sender, RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = "https://discord.gg/nezurai",
            UseShellExecute = true
        });
    }

    private void EnableStreamable_Click(object sender, RoutedEventArgs e)
    {
        if (EnableStreamable.IsChecked == true)
        {
            Imports.SetWindowDisplayAffinity(Data.WindowHandle, DisplayAffinity.Exclude);
            GlobalSettings.Streamable = true;
            MiscSettings["Streamable"] = true;
        }
        else if (EnableStreamable.IsChecked == false)
        {
            Imports.SetWindowDisplayAffinity(Data.WindowHandle, DisplayAffinity.None);
            GlobalSettings.Streamable = false;
            MiscSettings["Streamable"] = false;
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
    public string RobloxVersion
    {
        get
        {
            using (WebClient Http = new WebClient { })
                return Http.DownloadString("http://setup.roblox.com/version");
        }
    }
    private async void ResetButton1_Click(object sender, RoutedEventArgs e)
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
            string url = "https://dhs.army/fps.txt";
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

    private void EnableTopMost_Click(object sender, RoutedEventArgs e)
    {
        if (EnableTopMost.IsChecked == true)
        {
            GlobalSettings.TopMost = true;
            MiscSettings["TopMost"] = true;
            IntPtr mainWindowHandle = Process.GetCurrentProcess().MainWindowHandle;
            SetWindowPos(mainWindowHandle, new IntPtr(-1), 0, 0, 0, 0, TOPMOST_FLAGS);
        }
        else if (EnableTopMost.IsChecked == false)
        {
            GlobalSettings.TopMost = false;
            MiscSettings["TopMost"] = false;
            IntPtr mainWindowHandle = Process.GetCurrentProcess().MainWindowHandle;
            SetWindowPos(mainWindowHandle, new IntPtr(-2), 0, 0, 0, 0, TOPMOST_FLAGS);
        }
    }

    //private SpeechRecognizer speechRecognizer;

    //private void EnableVC_Click(object sender, RoutedEventArgs e)
    //{
    //    if (EnableVC.IsChecked == true)
    //    {
    //        GlobalSettings.VC = true;
    //        MiscSettings["VC"] = true;
    //        InitializeSpeechRecognizer();
    //    }
    //    else if (EnableVC.IsChecked == false)
    //    {
    //        GlobalSettings.VC = false;
    //        MiscSettings["VC"] = false;
    //        DisposeSpeechRecognizer();
    //    }
    //}

    //private void InitializeSpeechRecognizer()
    //{
    //    try
    //    {
    //        var speechConfig = SpeechConfig.FromSubscription("YourAzureSubscriptionKey", "YourAzureServiceRegion");
    //        speechRecognizer = new SpeechRecognizer(speechConfig);

    //        // Handle speech recognition events
    //        speechRecognizer.Recognized += SpeechRecognizer_Recognized;

    //        // Start continuous recognition
    //        speechRecognizer.SessionStarted += SpeechRecognizer_SessionStarted;
    //        speechRecognizer.Recognizing += SpeechRecognizer_Recognizing;
    //        speechRecognizer.SessionStopped += SpeechRecognizer_SessionStopped;

    //        speechRecognizer.StartContinuousRecognitionAsync().Wait();
    //    }
    //    catch (Exception ex)
    //    {
    //        MessageBox.Show($"Error initializing speech recognizer: {ex.Message}");
    //    }
    //}

    //private void DisposeSpeechRecognizer()
    //{
    //    try
    //    {
    //        if (speechRecognizer != null)
    //        {
    //            // Stop recognition and close the recognizer
    //            speechRecognizer.StopContinuousRecognitionAsync().Wait();
    //            speechRecognizer.Dispose();
    //            speechRecognizer = null;
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        MessageBox.Show($"Error disposing speech recognizer: {ex.Message}");
    //    }
    //}

    //private void SpeechRecognizer_SessionStarted(object sender, SessionEventArgs e)
    //{
    //    // Handle session started event
    //}

    //private void SpeechRecognizer_Recognizing(object sender, SpeechRecognitionEventArgs e)
    //{
    //    // Handle recognizing event
    //}

    //private void SpeechRecognizer_SessionStopped(object sender, SessionEventArgs e)
    //{
    //    // Handle session stopped event
    //}

    //private void SpeechRecognizer_Recognized(object sender, SpeechRecognitionEventArgs e)
    //{
    //    try
    //    {
    //        // Use the Dispatcher to marshal the call to the UI thread
    //        Dispatcher.Invoke(() =>
    //        {
    //            // Handle the recognized command
    //            string command = e.Result.Text.ToLower();

    //            switch (command)
    //            {
    //                case "hi":
    //                    GlobalSettings.TopMost = true;
    //                    MiscSettings["TopMost"] = true;
    //                    EnableTopMost.IsChecked = true;
    //                    break;

    //                case "disable voice command":
    //                    // Handle the "disable voice command" command
    //                    // Add any additional logic specific to the "disable voice command" command
    //                    break;

    //                // Add more cases for other commands as needed

    //                default:
    //                    // Handle unrecognized commands if needed
    //                    break;
    //            }
    //        });
    //    }
    //    catch (Exception ex)
    //    {
    //        // Log or display the exception details
    //        MessageBox.Show($"Error handling speech command: {ex.Message}");
    //    }
    //}
}
public class ConfigSave
    {
        public double FovSize { get; set; }
        public double Smoothness { get; set; }
        public double XOffset { get; set; }
        public double YOffset { get; set; }
        public double TriggerDelay { get; set; }
        public double Confidence { get; set; }
        public bool Streamable { get; set; }
        public bool TopMost { get; set; }
        //public bool DevMode { get; set; }

    }
