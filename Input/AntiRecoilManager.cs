using System.Diagnostics;
using System.Windows.Threading;

namespace NezurAimbot
{
    public class AntiRecoilManager
    {
        private CancellationTokenSource _cancellationTokenSource;
        public int MousePress;

        public async Task StartAntiRecoil()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            await Task.Run(async () =>
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    await Task.Delay(1);

                    Interlocked.Increment(ref MousePress);
                    if (MousePress >= GlobalSettings.Recoil_Rate)
                    {
                        MouseMovement.OffsetMouse();
                        Interlocked.Exchange(ref MousePress, 0);
                    }
                }
            }, _cancellationTokenSource.Token);
        }

        public void StopAntiRecoil()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }
    }
}
