using Microsoft.CognitiveServices.Speech.PronunciationAssessment;

namespace FluentSpeaker.Models
{
    internal class PronunciationResult
    {

        public string Error { get; set; }
        public string Text { get; set; }
        public PronunciationAssessmentResult Assessment { get; set; }
        public bool Success { get; set; }
    }
}
