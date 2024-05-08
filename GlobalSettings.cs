namespace NezurAimbot
{
    public static class GlobalSettings
    {
        private static Dictionary<string, object> properties = new Dictionary<string, object>();
        public static bool IsHoldingBinding = false;

        public static float ConfidenceThreshold
        {
            get => GetProperty<float>("ConfidenceThreshold", 0.5f);
            set => SetProperty("ConfidenceThreshold", value);
        }

        public static int FOVSize
        {
            get => GetProperty<int>("FOVSize", 640);
            set => SetProperty("FOVSize", value);
        }

        public static int FOVThickness
        {
            get => GetProperty<int>("FOVThickness", 1);
            set => SetProperty("FOVThickness", value);
        }

        public static float FOVOpacity
        {
            get => GetProperty<float>("FOVOpacity", 1f);
            set => SetProperty("FOVOpacity", value);
        }

        public static bool Streamable
        {
            get => GetProperty<bool>("Streamable", false);
            set => SetProperty("Streamable", value);
        }

        public static int Detections
        {
            get => GetProperty<int>("Detections", 8400);
            set => SetProperty("Detections", value);
        }

        public static int ImageSize
        {
            get => GetProperty<int>("ImageSize", 640);
            set => SetProperty("ImageSize", value);
        }

        public static int X_Offset
        {
            get => GetProperty<int>("X_Offset", 68);
            set => SetProperty("X_Offset", value);
        }

        public static int Y_Offset
        {
            get => GetProperty<int>("Y_Offset", 44);
            set => SetProperty("Y_Offset", value);
        }

        public static double AimSensitivity
        {
            get => GetProperty<double>("AimSensitivity", 0.8);
            set => SetProperty("AimSensitivity", value);
        }

        public static bool HoldingBinding
        {
            get => GetProperty<bool>("HoldingBinding", false);
            set => SetProperty("HoldingBinding", value);
        }

        public static bool TriggerBot
        {
            get => GetProperty<bool>("TriggerBot", false);
            set => SetProperty("TriggerBot", value);
        }

        public static bool TopMost
        {
            get => GetProperty<bool>("TopMost", false);
            set => SetProperty("TopMost", value);
        }

        public static bool RGB
        {
            get => GetProperty<bool>("RGB", false);
            set => SetProperty("RGB", value);
        }

        public static bool VC
        {
            get => GetProperty<bool>("VC", false);
            set => SetProperty("VC", value);
        }

        //public static bool DevMode
        //{
        //    get => GetProperty<bool>("DevMode", false);
        //    set => SetProperty("DevMode", value);
        //}

        public static string Theme
        {
            get => GetProperty<string>("Theme", "#fa2a38");
            set => SetProperty("Theme", value);
        }

        public static bool AntiRecoil
        {
            get => GetProperty<bool>("AntiRecoil", false);
            set => SetProperty("AntiRecoil", value);
        }

        public static int Recoil_Offset
        {
            get => GetProperty<int>("Recoil_Offset", 0);
            set => SetProperty("Recoil_Offset", value);
        }

        public static int Recoil_OffsetX
        {
            get => GetProperty<int>("Recoil_OffsetX", 0);
            set => SetProperty("Recoil_OffsetX", value);
        }

        public static int Recoil_Rate
        {
            get => GetProperty<int>("Recoil_Rate", 0);
            set => SetProperty("Recoil_Rate", value);
        }

        public static int Recoil_FireRate
        {
            get => GetProperty<int>("Recoil_FireRate", 0);
            set => SetProperty("Recoil_FireRate", value);
        }

        private static T GetProperty<T>(string propertyName, T defaultValue)
        {
            if (properties.ContainsKey(propertyName) && properties[propertyName] is T)
            {
                return (T)properties[propertyName];
            }
            else
            {
                SetProperty(propertyName, defaultValue);
                return defaultValue;
            }
        }

        private static void SetProperty<T>(string propertyName, T value)
        {
            if (properties.ContainsKey(propertyName))
            {
                properties[propertyName] = value;
            }
            else
            {
                properties.Add(propertyName, value);
            }

            NotifyPropertyChanged(propertyName);
        }

        public static event Action<string> PropertyChanged;
        public static event Action<string> ExitRecoil;

        private static void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(propertyName);
            ExitRecoil?.Invoke(propertyName);
        }
    }
}