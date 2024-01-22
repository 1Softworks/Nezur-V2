
using System.Runtime.InteropServices;

namespace NezurAimbot.AiModel;

public class TriggerBot
{
    public static void PerformMouseClick()
    {
        Imports.mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
        Task.Delay(2).Wait();
        Imports.mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
    }
}
