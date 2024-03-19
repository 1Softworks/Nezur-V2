using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using Microsoft.Win32;

namespace NezurAimbot
{
    public class ThemeManager : INotifyPropertyChanged
    {
        private SolidColorBrush textColor;
        private Brush background;
        private VisualBrush backgroundImage;

        public SolidColorBrush TextColor
        {
            get => textColor;
            set
            {
                if (textColor != value)
                {
                    textColor = value;
                    OnPropertyChanged();
                }
            }
        }

        public Brush Background
        {
            get => background;
            set
            {
                if (background != value)
                {
                    background = value;
                    OnPropertyChanged();
                }
            }
        }

        public VisualBrush BackgroundImage
        {
            get => backgroundImage;
            set
            {
                if (backgroundImage != value)
                {
                    backgroundImage = value;
                    OnPropertyChanged();
                }
            }
        }

        public ThemeManager()
        {
            TextColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF414150"));
            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF0A0A0F"));
            BackgroundImage = null;
        }

        public void ImportTheme(string filePath)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filePath);

                XmlNode textColorNode = doc.SelectSingleNode("//TextColor");
                XmlNode backgroundNode = doc.SelectSingleNode("//Background");

                if (textColorNode != null)
                {
                    TextColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(textColorNode.InnerText));
                }

                if (backgroundNode != null)
                {
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(backgroundNode.InnerText));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while importing the theme: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ExportTheme(string filePath)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                XmlElement root = doc.CreateElement("Theme");
                doc.AppendChild(root);

                XmlElement textColorNode = doc.CreateElement("TextColor");
                textColorNode.InnerText = TextColor.Color.ToString();
                root.AppendChild(textColorNode);

                XmlElement backgroundNode = doc.CreateElement("Background");
                backgroundNode.InnerText = Background.ToString();
                root.AppendChild(backgroundNode);

                doc.Save(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while exporting the theme: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ApplyTheme(Window window)
        {
            if (window != null)
            {
                window.Resources["TextColor"] = TextColor;

                ApplyThemeVT(window);
            }
        }

        public void ApplyTheme(Page page)
        {
            if (page != null)
            {
                ApplyThemeVT(page);
            }
        }

        private void ApplyThemeVT(Visual element)
        {
            if (element == null)
                return;

            if (element is FrameworkElement frameworkElement)
            {
                if (frameworkElement is TextBlock textBlock && textBlock.Foreground == null)
                {
                    textBlock.Foreground = TextColor;
                }
            }

            int childrenCount = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < childrenCount; i++)
            {
                Visual child = VisualTreeHelper.GetChild(element, i) as Visual;
                ApplyThemeVT(child);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}