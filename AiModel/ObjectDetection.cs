using KdTree;
using KdTree.Math;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using NezurAimbot.AiModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

#pragma warning disable CS1998
#pragma warning disable CS8618

namespace NezurAimbot
{
    public class ObjectDetection : IDisposable
    {
        private RunOptions Options;
        private InferenceSession Model;
        private List<string> OutputNames;

        private MemoryStream Stream = new MemoryStream(GlobalSettings.ImageSize * GlobalSettings.ImageSize * 4);

        public static ObjectDetection Instance { get; private set; }

        public ObjectDetection(string modelPath)
        {
            Options = new RunOptions();

            var sessionOptions = new SessionOptions
            {
                EnableCpuMemArena = true,
                EnableMemoryPattern = true,
                GraphOptimizationLevel = GraphOptimizationLevel.ORT_ENABLE_ALL,
                ExecutionMode = ExecutionMode.ORT_PARALLEL
            };

            try
            {
                LoadViaDirectML(sessionOptions, modelPath);
            }
            catch (Exception ex)
            {
                LoadViaCPU(sessionOptions, modelPath);
            }
        }

        private void LoadViaDirectML(SessionOptions sessionOptions, string modelPath)
        {
            sessionOptions.AppendExecutionProvider_DML();
            Model = new InferenceSession(modelPath, sessionOptions);
            OutputNames = Model.OutputMetadata.Keys.ToList();
        }

        private void LoadViaCPU(SessionOptions sessionOptions, string modelPath)
        {
            try
            {
                sessionOptions.AppendExecutionProvider_CPU();
                Model = new InferenceSession(modelPath, sessionOptions);
                OutputNames = Model.OutputMetadata.Keys.ToList();
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show($"Error starting the model via CPU: {e}");
                System.Windows.Application.Current.Shutdown();
            }
        }

        public class Prediction
        {
            public RectangleF Rectangle { get; set; }
            public float Confidence { get; set; }
        }

        public static Bitmap Capture(Rectangle detectionBox)
        {
            using (var bmp = new Bitmap(detectionBox.Width, detectionBox.Height))
            {
                using (var g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen(detectionBox.Location, System.Drawing.Point.Empty, detectionBox.Size);
                }

                return new Bitmap(bmp);
            }
        }

        public static float[] FloatArray(Bitmap image)
        {
            int height = image.Height;
            int width = image.Width;
            float[] result = new float[3 * height * width];
            Rectangle rect = new Rectangle(0, 0, width, height);

            BitmapData bmpData = image.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            IntPtr ptr = bmpData.Scan0;
            int bytes = Math.Abs(bmpData.Stride) * height;
            byte[] rgbValues = new byte[bytes];

            Marshal.Copy(ptr, rgbValues, 0, bytes);

            Parallel.For(0, rgbValues.Length / 3, i =>
            {
                int index = i * 3;
                int counter = i;
                result[counter] = rgbValues[index + 2] / 255.0f;
                result[height * width + counter] = rgbValues[index + 1] / 255.0f;
                result[2 * height * width + counter] = rgbValues[index] / 255.0f;
            });

            image.UnlockBits(bmpData);

            return result;
        }

        public async Task<Prediction?> ProcessOutputs()
        {
            try
            {
                var fovSize = GlobalSettings.FOVSize;

                var halfScreenWidth = Screen.PrimaryScreen.Bounds.Width / 2;
                var halfScreenHeight = Screen.PrimaryScreen.Bounds.Height / 2;
                var detectionBox = new Rectangle((int)(halfScreenWidth - 640 / 2), (int)(halfScreenHeight - 640 / 2), 640, 640);

                var frame = Capture(detectionBox);

                if (MiscSettings["CollectData"] == true)
                    await ImageUtils.SaveFrameAsync(frame);

                var inputArray = FloatArray(frame);
                if (inputArray == null) return null;

                var inputTensor = new DenseTensor<float>(inputArray, new int[] { 1, 3, frame.Height, frame.Width });
                var inputs = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor("images", inputTensor) };
                var results = Model.Run(inputs, OutputNames, Options);

                var fovMin = (GlobalSettings.ImageSize - GlobalSettings.FOVSize) / 2.0f;
                var fovMax = (GlobalSettings.ImageSize + GlobalSettings.FOVSize) / 2.0f;

                var outputTensor = results[0].AsTensor<float>();

                var tree = new KdTree<float, Prediction>(2, new FloatMath());

                var filteredIndices = Enumerable.Range(0, GlobalSettings.Detections).AsParallel().Where(i => outputTensor[0, 4, i] >= GlobalSettings.ConfidenceThreshold).ToList();

                object treeLock = new object();

                Parallel.ForEach(filteredIndices, i =>
                {
                    var objectness = outputTensor[0, 4, i];

                    var xCenter = outputTensor[0, 0, i];
                    var yCenter = outputTensor[0, 1, i];

                    var width = outputTensor[0, 2, i];
                    var height = outputTensor[0, 3, i];

                    var xMin = xCenter - width / 2;
                    var yMin = yCenter - height / 2;
                    var xMax = xCenter + width / 2;
                    var yMax = yCenter + height / 2;

                    if (xMin >= fovMin && xMax <= fovMax && yMin >= fovMin && xMax <= fovMax)
                    {
                        var prediction = new Prediction
                        {
                            Rectangle = new RectangleF(xMin, yMin, xMax - xMin, yMax - yMin),
                            Confidence = objectness
                        };

                        var centerX = (xMin + xMax) / 2.0f;
                        var centerY = (yMin + yMax) / 2.0f;

                        lock (treeLock)
                        {
                            tree.Add(new[] { centerX, centerY }, prediction);
                        }
                    }
                });

                var enemies = tree.GetNearestNeighbours(new[] { fovSize / 2.0f, fovSize / 2.0f }, 1);

                return enemies.Length > 0 ? enemies[0].Value : null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void Dispose()
        {
            Model?.Dispose();
            Stream?.Dispose();
        }
    }
}