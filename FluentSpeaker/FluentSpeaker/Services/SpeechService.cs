using System;
using System.Threading.Tasks;

using FluentSpeaker.Classes;
using FluentSpeaker.Helpers;
using FluentSpeaker.Models;

using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.PronunciationAssessment;

using Xamarin.Forms;

namespace FluentSpeaker.Services
{
    internal class SpeechService
    {
        string language;
        IMicrophoneService micService;
        SpeechRecognizer recognizer;
        PronunciationAssessmentConfig pronunciationConfig;

        public SpeechService()
        {
            language = "en-US";
            micService = DependencyService.Resolve<IMicrophoneService>();

            pronunciationConfig = new PronunciationAssessmentConfig(String.Empty, 
                GradingSystem.HundredMark, Granularity.Phoneme, true);
        }

        public async Task<PronunciationResult> RecognizeSpeech(string text)
        {
            var pronunciationResult = new PronunciationResult();
            bool isMicEnabled = await micService.GetPermissionAsync();

            if (isMicEnabled)
            {
                if (recognizer == null)
                {
                    var config = SpeechConfig.FromSubscription(Constants.CognitiveKey, Constants.CognitiveRegion);
                    config.SetProperty(PropertyId.SpeechServiceConnection_EndSilenceTimeoutMs, "2000");

                    recognizer = new SpeechRecognizer(config, language);
                }

                pronunciationConfig.ReferenceText = text;
                pronunciationConfig.ApplyTo(recognizer);

                var result = await recognizer.RecognizeOnceAsync().ConfigureAwait(false);

                switch (result.Reason)
                {
                    case ResultReason.NoMatch:
                        pronunciationResult.Error = "NO MATCH! Speech could not be recognized.";
                        break;
                    case ResultReason.Canceled:
                        pronunciationResult.Error = "Canceled!";
                        break;
                    case ResultReason.RecognizedSpeech:
                        pronunciationResult.Text = result.Text;
                        pronunciationResult.Assessment = PronunciationAssessmentResult.FromResult(result);
                        pronunciationResult.Success = true;
                        break;
                }
            }
            else
            {
                pronunciationResult.Error = "Please grant access to the microphone";
            }

            return pronunciationResult;
        }
    }
}
