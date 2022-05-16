using System.Collections.Generic;

namespace FluentSpeaker.Models
{
    internal class PronunciationWord
    {
        public string Word { get; set; }
        public double AccuracyScore { get; set; }
        public string Error { get; set; }
    }
}
