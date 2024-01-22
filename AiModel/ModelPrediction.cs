using Accord.Statistics.Running;

namespace NezurAimbot.AiModel
{
    internal class ModelPrediction
    {
        private KalmanFilter2D kalmanFilter;

        public void InitializeKalmanFilter() => kalmanFilter = new KalmanFilter2D();

        public void UpdateKalmanFilter(double detectedX, double detectedY) => kalmanFilter.Push(detectedX, detectedY);

        public Detection GetEstimatedPosition() => new Detection
        {
            X = (int)(kalmanFilter.X + kalmanFilter.XAxisVelocity * 0.01),
            Y = (int)(kalmanFilter.Y + kalmanFilter.YAxisVelocity * 0.01)
        };
    }
}