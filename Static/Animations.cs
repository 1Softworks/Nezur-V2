using System.Windows.Media.Animation;

namespace NezurAimbot.Static;

public static class Animations
{
    public static DoubleAnimation DoubleAnimate(int From, int To, double Time)
    {
        return new DoubleAnimation
        {
            To = To,
            From = From,
            Duration = TimeSpan.FromSeconds(Time),
            EasingFunction = new QuarticEase(),
        };
    }
    public static ThicknessAnimation MarginThicknessAnimate(Thickness fromMargin, Thickness toMargin, double timeInSeconds)
    {
        return new ThicknessAnimation
        {
            From = fromMargin,
            To = toMargin,
            Duration = TimeSpan.FromSeconds(timeInSeconds),
            EasingFunction = new QuarticEase(),
        };
    }


    public async static Task AnimateVisibility(UIElement element, Visibility visibility)
    {
        if (visibility == Visibility.Visible)
        {
            element.Visibility = Visibility.Hidden;
            await Task.Delay(500);
            var animationVis = DoubleAnimate(0, 1, 0.5);
            element.BeginAnimation(UIElement.OpacityProperty, animationVis);
            element.Visibility = visibility;
        }
        else
        {
            element.Visibility = visibility;
            var animationDis = DoubleAnimate(1, 0, 0.5);
            element.BeginAnimation(UIElement.OpacityProperty, animationDis);
            await Task.Delay(500);
        }
    }

}
