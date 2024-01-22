using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace NezurAimbot.Interface;

public partial class FovOverlay : Window
{
    public FovOverlay()
    {
        InitializeComponent();
        GlobalSettings.PropertyChanged += HandlePropertyChanged;
    }

    private void HandlePropertyChanged(string propertyName)
    {
        double screenWidth = SystemParameters.PrimaryScreenWidth;
        double screenHeight = SystemParameters.PrimaryScreenHeight;

        switch (propertyName)
        {
            case "FOVSize":
                OverlayRectangle.Width = GlobalSettings.FOVSize;
                OverlayRectangle.Height = GlobalSettings.FOVSize;
                break;

            case "FOVThickness":
                OverlayRectangle.StrokeThickness = GlobalSettings.FOVThickness;
                break;

            case "FOVOpacity":
                OverlayRectangle.Opacity = GlobalSettings.FOVOpacity;
                break;

            case "Theme":
                OverlayRectangle.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(GlobalSettings.Theme));
                break;


            default:
                break;
        }

        OverlayCanvas.Width = screenWidth;
        OverlayCanvas.Height = screenHeight;

        Canvas.SetLeft(OverlayRectangle, (screenWidth - GlobalSettings.FOVSize) / 2);
        Canvas.SetTop(OverlayRectangle, (screenHeight - GlobalSettings.FOVSize) / 2);

        Left = 0;
        Top = 0;

        Width = screenWidth;
        Height = screenHeight;
    }
}