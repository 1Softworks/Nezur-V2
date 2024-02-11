using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

using Newtonsoft.Json;

namespace NezurAimbot.Static;

public class Data
{
    public static IntPtr WindowHandle = IntPtr.Zero;

    public static string LocalVersion = "2.0.7";
    public static WindowState previousState;
    public static bool isMaximized = false;
    public static int TimeSinceLastClick { get; set; }
    public static DateTime LastClickTime { get; set; } = DateTime.MinValue;

    public const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
    public const uint MOUSEEVENTF_LEFTUP = 0x0004;
    public const uint MOUSEEVENTF_MOVE = 0x0001;

    // Default Settings
    public static Dictionary<string, double> DefaultSettings = new()
    {
        ["FOV_Size"] = 640,
        ["Y_Offset"] = 0,
        ["X_Offset"] = 0,
        ["Trigger_Delay"] = 0.1,
        ["AI_Min_Conf"] = 50,
        ["AimAssist_Smoothness"] = 0.8,
        ["AutoClickerDelay"] = 5,
    };

    public static Dictionary<string, bool> MiscSettings = new()
    {
        ["AimAssist"] = false,
        ["TriggerBot"] = false,
        ["ModelLoaded"] = false,
        ["CollectData"] = false,
        ["ShowFOV"] = false,
        ["AimPrediction"] = false,
        ["SpecialEducation"] = false,
        ["KeyBindDown"] = true,
        ["AutoClicker"] = false,
        ["Streamable"] = false,
        ["TopMost"] = false,
        ["VC"] = false
    };

    public static string DefaultBinding = "Right";
    public static string DefaultThemeColor = "#ff1231";

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;
    }

    public class Prediction
    {
        public RectangleF Rectangle { get; set; }
        public float Confidence { get; set; }
    }

    public struct Detection
    {
        public int X;
        public int Y;
        public DateTime Timestamp;
    }

    [Flags]
    public enum DisplayAffinity : uint
    {
        None = 0x00000000,
        Monitor = 0x00000001,
        Exclude = 0x00000011
    }

    public static string ProgramFolder = $"{Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)}\\Nezur";

    public static string[] LocalDirectorys =
    {
        $"{ProgramFolder}\\bin",
        $"{ProgramFolder}\\bin\\models",
        $"{ProgramFolder}\\bin\\images",
        $"{ProgramFolder}\\bin\\config"
    };

    public static string[] LocalModels =
    {
        "onnxruntime.dll",
        "DirectML.dll",
        Path.Combine(LocalDirectorys[0], "Temp.zip"),
        Path.Combine(LocalDirectorys[0], "Release\\Temp2.zip"),
        Path.Combine(LocalDirectorys[0], "Release\\Nezur Mouse Fix.exe"),
        Path.Combine(LocalDirectorys[0], "Release\\nezuraim.exe")
    };
}
