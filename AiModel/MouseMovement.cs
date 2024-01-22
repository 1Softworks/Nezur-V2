using NezurAimbot.AiModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace NezurAimbot
{
    public class MouseMovement
    {
        private const int MOUSEEVENTF_MOVE = 0x0001;

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        public void MoveViewTo(int detectedX, int detectedY, bool triggerBot)
        {
            var screenBounds = Screen.PrimaryScreen.Bounds;
            var halfScreenWidth = screenBounds.Width / 2;
            var halfScreenHeight = screenBounds.Height / 2;

            int targetX = detectedX - halfScreenWidth;
            int targetY = detectedY - halfScreenHeight;

            double arC = (double)screenBounds.Width / screenBounds.Height;
            targetY = (int)(targetY * arC);

            System.Drawing.Point start = new(0, 0);
            System.Drawing.Point end = new(targetX, targetY);

            System.Drawing.Point c1 = new(start.X + (end.X - start.X) / 2, start.Y + (end.Y - start.Y) / 2);
            System.Drawing.Point c2 = new(start.X + (end.X - start.X) / 2, start.Y + (end.Y - start.Y) / 2);

            System.Drawing.Point newPosition = CubicBezier(start, end, c1, c2, 1 - GlobalSettings.AimSensitivity);

            mouse_event(MOUSEEVENTF_MOVE, newPosition.X, newPosition.Y, 0, 0);

            if (triggerBot)
            {
                TriggerBot();
            }
        }

        private void TriggerBot()
        {
            const int MOUSEEVENTF_LEFTDOWN = 0x0002;
            const int MOUSEEVENTF_LEFTUP = 0x0004;

            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }

        private static System.Drawing.Point CubicBezier(System.Drawing.Point start, System.Drawing.Point end, System.Drawing.Point c1, System.Drawing.Point c2, double t)
        {
            double u = 1 - t;
            double tt = t * t;
            double uu = u * u;
            double uuu = uu * u;
            double ttt = tt * t;

            double x = uuu * start.X + 3 * uu * t * c1.X + 3 * u * tt * c2.X + ttt * end.X;
            double y = uuu * start.Y + 3 * uu * t * c1.Y + 3 * u * tt * c2.Y + ttt * end.Y;

            return new System.Drawing.Point((int)x, (int)y);
        }
    }
}