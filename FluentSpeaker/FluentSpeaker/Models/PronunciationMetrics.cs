using SkiaSharp;

namespace FluentSpeaker.Models
{
    internal class PronunciationMetrics
    {
        public string Metric { get; set; }
        public float Score { get; set; }
        public SKColor Color { get; set; }

        public PronunciationMetrics(string metric, double score, SKColor color)
        {
            Metric = metric;
            Score = (float)score;
            Color = color;
        }
    }
}
