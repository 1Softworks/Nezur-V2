using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Concurrent;

namespace NezurAimbot.AiModel;

public static class ImageUtils
{
    public static async Task SaveFrameAsync(Bitmap frame) => await Task.Run(() => frame.Save(Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Bin", "Images"), $"{Guid.NewGuid()}.jpg")));

    public static Bitmap ScreenGrab(Rectangle detectionBox)
    {
        using Bitmap bmp = new(detectionBox.Width, detectionBox.Height);
        using (Graphics g = Graphics.FromImage(bmp))
        {
            g.CopyFromScreen(detectionBox.Left, detectionBox.Top, 0, 0, detectionBox.Size);
        }
        return new Bitmap(bmp); // Create a copy to avoid disposing the original bitmap
    }

    public static float[] BitmapToFloatArray(Bitmap img)
    {
        int imgHeight = img.Height;
        int imgWidth = img.Width;
        float[] outputArray = new float[3 * imgHeight * imgWidth];
        Rectangle rectangle = new(0, 0, imgWidth, imgHeight);

        BitmapData bitmapData = img.LockBits(rectangle, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

        try
        {
            IntPtr pointer = bitmapData.Scan0;
            int totalBytes = bitmapData.Stride * imgHeight;
            byte[] rgbArray = new byte[totalBytes];

            Marshal.Copy(pointer, rgbArray, 0, totalBytes);

            Parallel.ForEach(Partitioner.Create(0, rgbArray.Length / 3), range =>
            {
                for (int j = range.Item1; j < range.Item2; j++)
                {
                    int rgbIndex = j * 3;
                    int counter = j;
                    outputArray[counter] = rgbArray[rgbIndex + 2] / 255.0f; // R
                    outputArray[imgHeight * imgWidth + counter] = rgbArray[rgbIndex + 1] / 255.0f; // G
                    outputArray[2 * imgHeight * imgWidth + counter] = rgbArray[rgbIndex] / 255.0f; // B
                }
            });
        }
        finally
        {
            img.UnlockBits(bitmapData);
        }

        return outputArray;
    }
}