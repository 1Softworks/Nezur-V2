using System.Diagnostics;
using System.IO;
using System.Windows.Controls;

namespace NezurAimbot.Interface.InterfacePages;

public partial class ConfigurationPage : Page
{
    public ConfigurationPage()
    {
        InitializeComponent();
    }
    private void SelectorListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        string Model = SelectorListBox.SelectedItem.ToString();
        MainInterface.LoadedModel = Model;
        LoadedModelLabel.Text = $"Loaded Model: {Model}";
        foreach (Window window in Application.Current.Windows)
        {
            if (window.GetType() == typeof(MainInterface)) ((MainInterface)window).LoadModel();
        }
    }

    public void LoadListBox()
    {
        string[] onnxFiles = Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Bin", "Models"), "*.onnx");
        SelectorListBox.Items.Clear();

        foreach (string filePath in onnxFiles)
        {
            string fileName = Path.GetFileName(filePath);
            SelectorListBox.Items.Add(fileName);
        }

        if (SelectorListBox.Items.Count > 0)
        {
            if (MainInterface.LoadedModel != "N/A" && SelectorListBox.Items.Contains(MainInterface.LoadedModel))
            {
                SelectorListBox.SelectedItem = MainInterface.LoadedModel;
            }
            LoadedModelLabel.Text = $"Loaded Model: {MainInterface.LoadedModel}";
        }
        else
        {
            MessageBox.Show("No models found, please put a .onnx model in bin/models.");
            Environment.Exit(0);
        }
    }

    private void OpenModelFolder_Click(object sender, RoutedEventArgs e) => Process.Start("explorer.exe", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Bin", "Models"));

    private void CreateOwnModel_Click(object sender, RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = "https://1cheats.com/forums/topic/79-how-to-create-a-custom-model/",
            UseShellExecute = true
        });
    }

    private void LoadConfig_Click(object sender, RoutedEventArgs e)
    {

    }

    private void SaveConfig_Click(object sender, RoutedEventArgs e)
    {

    }

    private void CreateOwnModel_Copy_Click(object sender, RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = "https://1cheats.com/files/",
            UseShellExecute = true
        });
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        LoadListBox();
        SelectorListBox.SelectionChanged += new SelectionChangedEventHandler(SelectorListBox_SelectionChanged);
    }
}
