using System.IO;
using System.IO.Compression;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using NezurAimbot.AiModel;
using NezurAimbot.Interface.InterfacePages;
using Application = System.Windows.Application;
using DiscordRPC;
using System.Windows.Forms;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using Microsoft.Windows.Themes;


namespace NezurAimbot.Interface;

public partial class MainInterface : Window

{
    private bool AnimationActive = false;
    private int ActivePage;

    private static FileSystemWatcher? fileWatcher;
    public static DispatcherTimer autoClickTimer;
    public static string LoadedModel = "N/A";

    private const int ToolsPage = 1;
    private const int ConfigPage = 2;
    private const int SettingsPage = 3;
    private const int SpooferPage = 4;

    private static InputBindingManager bindingManager;
    private ModelPrediction predictionManager;
    private ConfigurationPage configurationPage;
    private static FovOverlay fovOverlay;
    private ObjectDetection AIModel;
    private MouseMovement mouseMovement;

    Color themeColor = (Color)ColorConverter.ConvertFromString(GlobalSettings.Theme);
   
    public static api KeyAuthApp = new api(
         name: "Nezur",
         ownerid: "2UWe8CI1m7",
         secret: "da0fa5c2ed5edc166033d7296f3343118d8401fd552aafae3b231d896b558dd9",
         version: "1.0"
     );
    public MainInterface()
    {
        InitializeComponent();
        mouseMovement = new MouseMovement();
        Username.Text = $"Hello\n{Environment.UserName}";
        SetThemeColors();
        fovOverlay = new FovOverlay();
        fovOverlay.Hide();
        System.Windows.Forms.Timer themeTimer = new System.Windows.Forms.Timer();
        themeTimer.Interval = 10;
        themeTimer.Tick += ThemeTimer_Tick;
        themeTimer.Start();
        InitializeRPC();
        KeyAuthApp.init();
        KeyAuthApp.check();
    }

    private void ThemeTimer_Tick(object sender, EventArgs e)
    {
        SetThemeColors();
    }

    private void SetThemeColors()
    {
        Color themeColor = (Color)ColorConverter.ConvertFromString(GlobalSettings.Theme);
        SolidColorBrush themeBrush = new SolidColorBrush(themeColor);
        GlowMain.Color = themeColor;
        NezurTitle.Foreground = themeBrush;
    }


    private async void Main_Loaded(object sender, RoutedEventArgs e)
    {
        WindowHandle = new WindowInteropHelper(this).Handle;
        await LoadStartup();
    }

    // Download Links
    // https://nezur.net/Spoofer.zip
    // https://nezur.net/NezurMouseFix.zip
    // https://github.com/skibbbitoiltisskibbbitoiltisskibbbitoi/Models/releases/download/NezurColorAimbot/NezurColorAimbot.zip
    // https://github.com/0x9374698765254342/Models

    private async Task LoadStartup()
    {
        Main.Visibility = Visibility.Hidden;
        await LoadAnimations();
        await FirstLoadupToolsPage();
        LoadBindingManager();
        InitializeFileWatcher();
        InitializeAutoClickTimer();
        predictionManager = new ModelPrediction();
        predictionManager.InitializeKalmanFilter();
        StartDetection();
    }

    public static void ShowFOV()
    {
        MiscSettings["ShowFOV"] = true;
        fovOverlay.Show();
    }

    public static void HideFOV()
    {
        MiscSettings["ShowFOV"] = false;
        fovOverlay.Hide();
    }

    private static void LoadBindingManager()
    {
        bindingManager = new InputBindingManager();
        bindingManager.SetupDefault(Properties.Settings.Default.Current_Binding);
        bindingManager.OnBindingPressed += (binding) => { GlobalSettings.IsHoldingBinding = true; };
        bindingManager.OnBindingReleased += (binding) => { GlobalSettings.IsHoldingBinding = false; };
    }



    public static void ResetBinding() => bindingManager.SetupDefault(Properties.Settings.Default.Current_Binding);

    public static Task<string> GetNewInputBindingAsync()
    {
        var taskCompletionSource = new TaskCompletionSource<string>();

        void onBindingSet(string binding)
        {
            taskCompletionSource.SetResult(binding);
            bindingManager.OnBindingSet -= onBindingSet; // Unsubscribe after handling once
        }

        bindingManager.StartListeningForBinding();
        bindingManager.OnBindingSet += onBindingSet;

        return taskCompletionSource.Task;
    }

    private async Task LoadAnimations()
    {
        await Task.Delay(500);
        Main.Visibility = Visibility.Visible;
        Foreground.Visibility = Visibility.Hidden;
        Background.Visibility = Visibility.Visible;
        await Animations.AnimateVisibility(Background, Visibility.Visible);
        await Animations.AnimateVisibility(Foreground, Visibility.Visible);
        MainArea.Content = new InterfacePages.ToolsPage();
    }

    private async Task FirstLoadupToolsPage()
    {
        Tools.Style = (Style)Application.Current.Resources["SelectedSideBarAnimatedButtonStyle"];
        Tools.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#dde6e8"));
        AnimationActive = true;
        ActivePage = 1;

        ButtonBarSelection.Margin = new Thickness(7, 65, 0, 0);
        await Animations.AnimateVisibility(ButtonBarSelection, Visibility.Visible);

        AnimationActive = false;
    }
    private async void Main_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        if (Client.IsInitialized)
            Client.ClearPresence();
        Client.Dispose();

        e.Cancel = true;
        bindingManager.StopListening();
        Properties.Settings.Default.Save();
        var dbo = Animations.DoubleAnimate(1, 0, 0.5);
        Foreground.BeginAnimation(OpacityProperty, dbo);
        await Task.Delay(500);
        Foreground.Visibility = Visibility.Hidden;
        Main.BeginAnimation(OpacityProperty, dbo);
        Background.BeginAnimation(WidthProperty, Animations.DoubleAnimate((int)Main.Width, 0, 0.5));
        Background.BeginAnimation(HeightProperty, Animations.DoubleAnimate((int)Main.Height, 0, 0.5));
        await Task.Delay(1500);
        Environment.Exit(0);
    }

    private void Drag(object sender, MouseButtonEventArgs e) => DragMove();

    private void Exit_Click(object sender, RoutedEventArgs e) => Close();

    private void Maximize_Click(object sender, RoutedEventArgs e)
    {
        WindowState = isMaximized ? previousState : WindowState.Maximized;
        isMaximized = !isMaximized;
    }

    private void Minimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

    private void Tools_Click(object sender, RoutedEventArgs e) 
    {
        SwitchPage(ToolsPage, 65);
    } 

    private void Configuration_Click(object sender, RoutedEventArgs e)
    {
        SwitchPage(ConfigPage, 105);
    }

    private void Settings_Click(object sender, RoutedEventArgs e)
    {
        SwitchPage(SettingsPage, 145);
    }

    private async void SwitchPage(int pageNumber, int buttonMarginTop)
    {
        // Basic checks making sure the current page is not the one trying to be switched too and if there is a current pageswitch happening mid animation
        if (ActivePage == pageNumber || AnimationActive)
            return;

        // Set Active Animation bool true and start Page switch to new page from current page
        await SwitchPageAsync(pageNumber, buttonMarginTop);
    }

    private async Task<bool> SwitchPageAsync(int pageNumber, int buttonMarginTop)
    {
        /* 
         * Selected Page Left Bar Margins respective to their button
         * 
         * Tools SelectionBar Margin = 7,65,0,0
         * Config SelectionBar Margin = 7,105,0,0
         * Settings SelectionBar Margin = 7,145,0,0
         * Spoofer SelectionBar Margin = 7,185,0,0
         */

        AnimationActive = true;
        ActivePage = pageNumber;

        MainAreaFrame.BeginAnimation(OpacityProperty, Animations.DoubleAnimate(1, 0, 0.25));

        await Task.Delay(250);

        switch (pageNumber)
        {
            case ToolsPage:
                MainArea.Content = new ToolsPage();
                break;
            case ConfigPage:
                MainArea.Content = new ConfigurationPage();
                break;
            case SettingsPage:
                MainArea.Content = new SettingsPage();
                break;
            case SpooferPage:
                MainArea.Content = new Spoofer();
                break;
        }

        MainAreaFrame.BeginAnimation(OpacityProperty, Animations.DoubleAnimate(0, 1, 0.25));
        ButtonBarSelection.BeginAnimation(MarginProperty, Animations.MarginThicknessAnimate(ButtonBarSelection.Margin, new Thickness(7, buttonMarginTop, 0, 0), 0.5));

        UpdateButtonStyles(pageNumber);

        AnimationActive = false;
        return true;
    }

    private void UpdateButtonStyles(int selectedPage)
    {
        // Set The Tools Button To Selected
        Tools.Style = selectedPage == 1 ? (Style)Application.Current.Resources["SelectedSideBarAnimatedButtonStyle"] : (Style)Application.Current.Resources["DefaultSideBarAnimatedButtonStyle"];
        Tools.Foreground = new SolidColorBrush(selectedPage == 1 ? (Color)ColorConverter.ConvertFromString("#dde6e8") : (Color)ColorConverter.ConvertFromString("#FF4B4B4B"));

        // Set The Configuration Button To Selected
        Configuration.Style = selectedPage == 2 ? (Style)Application.Current.Resources["SelectedSideBarAnimatedButtonStyle"] : (Style)Application.Current.Resources["DefaultSideBarAnimatedButtonStyle"];
        Configuration.Foreground = new SolidColorBrush(selectedPage == 2 ? (Color)ColorConverter.ConvertFromString("#dde6e8") : (Color)ColorConverter.ConvertFromString("#FF4B4B4B"));

        // Set The Settings Button To Selected
        Settings.Style = selectedPage == 3 ? (Style)Application.Current.Resources["SelectedSideBarAnimatedButtonStyle"] : (Style)Application.Current.Resources["DefaultSideBarAnimatedButtonStyle"];
        Settings.Foreground = new SolidColorBrush(selectedPage == 3 ? (Color)ColorConverter.ConvertFromString("#dde6e8") : (Color)ColorConverter.ConvertFromString("#FF4B4B4B"));

        SpooferButton.Style = selectedPage == 4 ? (Style)Application.Current.Resources["SelectedSideBarAnimatedButtonStyle"] : (Style)Application.Current.Resources["DefaultSideBarAnimatedButtonStyle"];
        SpooferButton.Foreground = new SolidColorBrush(selectedPage == 4 ? (Color)ColorConverter.ConvertFromString("#dde6e8") : (Color)ColorConverter.ConvertFromString("#FF4B4B4B"));
    }

    private void InitializeFileWatcher()
    {
        fileWatcher = new FileSystemWatcher
        {
            Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Bin", "Models"),
            Filter = "*.onnx",
            EnableRaisingEvents = true
        };
        fileWatcher.Created += FileWatcher_Reload;
        fileWatcher.Deleted += FileWatcher_Reload;
        fileWatcher.Renamed += FileWatcher_Reload;
    }
    private void FileWatcher_Reload(object sender, FileSystemEventArgs e)
    {
        base.Dispatcher.Invoke(delegate
        {
            if (configurationPage != null)
            {
                configurationPage.LoadListBox();
            }
        });
    }

    private async Task DetectEnemies()
    {
        var closestPrediction = await AIModel.ProcessOutputs();

        if (closestPrediction == null)
            return;

        var screenBounds = Screen.PrimaryScreen.Bounds;
        float scaleX = screenBounds.Width / 640f;
        float scaleY = screenBounds.Height / 640f;

        double YOffset = GlobalSettings.Y_Offset;
        double XOffset = GlobalSettings.X_Offset;
        int detectedX = (int)((closestPrediction.Rectangle.X * scaleX) + XOffset);
        int detectedY = (int)((closestPrediction.Rectangle.Y * scaleY) + YOffset);

        if (GlobalSettings.TriggerBot)
        {
            if (MiscSettings["AimPrediction"])
            {
                predictionManager.UpdateKalmanFilter(detectedX, detectedY);
                var predictedPosition = predictionManager.GetEstimatedPosition();
                mouseMovement.MoveViewTo(predictedPosition.X, predictedPosition.Y, true);
            }
            else
            {
                mouseMovement.MoveViewTo(detectedX, detectedY, true);
            }
        }
        else
        {
            if (MiscSettings["AimPrediction"])
            {
                predictionManager.UpdateKalmanFilter(detectedX, detectedY);
                var predictedPosition = predictionManager.GetEstimatedPosition();
                mouseMovement.MoveViewTo(predictedPosition.X, predictedPosition.Y, false);
            }
            else
            {
                mouseMovement.MoveViewTo(detectedX, detectedY, false);
            }
        }
    }

    private CancellationTokenSource cts;
    private Thread ModelCapture;

    private void StartDetection()
    {
        cts = new CancellationTokenSource();

        ModelCapture = new Thread(async () =>
        {
            while (!cts.Token.IsCancellationRequested)
            {
                if (MiscSettings["KeyBindDown"])
                {
                    if (GlobalSettings.IsHoldingBinding)
                    {
                        if (MiscSettings["AimAssist"]) await DetectEnemies();
                        else if (MiscSettings["TriggerBot"]) await DetectEnemies();
                    }
                }
                else
                {
                    if (MiscSettings["AimAssist"]) await DetectEnemies();
                    else if (MiscSettings["TriggerBot"]) await DetectEnemies();
                }

                Thread.Sleep(1);
            }
        });

        ModelCapture.Start();
    }
 
    private void StopDetection()
    {
        if (cts != null)
        {
            cts.Cancel();
        }
    }
   private void StartColorLoop()
    {
      
    }
    public async void LoadModel()
    {
        string modelPath = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Bin", "Models"), LoadedModel);

        AIModel?.Dispose();

        AIModel = new ObjectDetection(modelPath);
        MiscSettings["ModelLoaded"] = true;
        ModelStatusText.Text = "Online";
        ModelStatusText.Foreground = new SolidColorBrush(Colors.Green);
    }

    public static void InitializeAutoClickTimer()
    {
        autoClickTimer = new DispatcherTimer();
        autoClickTimer.Tick += AutoClickTimer_Tick;
        autoClickTimer.Interval = TimeSpan.FromMilliseconds(Properties.Settings.Default.AutoClickerDelay * 1000);
    }

    private static void AutoClickTimer_Tick(object sender, EventArgs e)
    {
        TriggerBot.PerformMouseClick();
    }

    private void Spoofer_Click(object sender, RoutedEventArgs e)
    {
        SwitchPage(SpooferPage, 185);
    }

    private readonly DiscordRpcClient Client = new("1153775951906349108");
    public void InitializeRPC()
    {
        if (Client.IsInitialized == false) Client.Initialize();

        Client.SetPresence(new RichPresence
        {
            Details = "Nezur",
            State = "Roblox's #1 External",
            Timestamps = Timestamps.Now,
            Buttons = new DiscordRPC.Button[] {
                new() { Label = "Download (NEZUR.NET)", Url = "https://nezur.net", },
                new() { Label = "Join the Discord", Url = "https://discord.gg/nezurai", },
            },
            Assets = new Assets
            {
                LargeImageKey = "nezur",
                LargeImageText = "NEZUR.NET",
            },
        });
    }
}