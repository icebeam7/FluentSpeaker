using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using SkiaSharp;
using Microcharts;

using Xamarin.Forms;
using Xamarin.Essentials;

using FluentSpeaker.Models;
using FluentSpeaker.Services;

namespace FluentSpeaker.ViewModels
{
    internal class LearnViewModel : BaseViewModel
    {
        private string textToRecognize;
        public string TextToRecognize
        {
            get { return textToRecognize; }
            set { SetProperty(ref textToRecognize, value); }
        }

        private string recognizedText;
        public string RecognizedText
        {
            get { return recognizedText; }
            set { SetProperty(ref recognizedText, value); }
        }

        private Chart metricsChart;
        public Chart MetricsChart
        {
            get { return metricsChart; }
            set { SetProperty(ref metricsChart, value); }
        }

        public ObservableCollection<PronunciationWord> Words { get; }
        public ObservableCollection<PronunciationPhoneme> Phonemes { get; }
        public ObservableCollection<PronunciationSyllable> Syllables { get; }

        public ICommand SpeakCommand { get; set; }
        public ICommand ListenCommand { get; set; }
        public ICommand GetTextCommand { get; set; }

        private SpeechService speechService;
        private TextService textService;

        private void Clear()
        {
            RecognizedText = string.Empty;
            Words.Clear();
            Phonemes.Clear();
            Syllables.Clear();

            MetricsChart = new RadialGaugeChart()
            {
                IsAnimated = true,
                MinValue = 0,
                MaxValue = 100,
                LabelTextSize = 16
            };
        }

        private async Task Speak()
        {
            Clear();
            IsBusy = true;
            var pronunciationResult = await speechService.RecognizeSpeech(textToRecognize);

            if (pronunciationResult != null && pronunciationResult.Success)
            {
                RecognizedText = pronunciationResult.Text;
                var assessment = pronunciationResult.Assessment;

                var pronunciationMetrics = new List<PronunciationMetrics>()
                {
                    new PronunciationMetrics("Accuracy", assessment.AccuracyScore, SKColors.Blue),
                    new PronunciationMetrics("Fluency", assessment.FluencyScore, SKColors.Purple),
                    new PronunciationMetrics("Completeness", assessment.CompletenessScore, SKColors.Orange),
                    new PronunciationMetrics("Pronunciation", assessment.PronunciationScore, SKColors.Green),
                };

                MetricsChart.Entries = pronunciationMetrics.Select(
                    (pm, index) => new ChartEntry(pm.Score)
                    {
                        Label = pm.Metric,
                        ValueLabel = $"{pm.Score} %",
                        Color = pm.Color,
                        TextColor = pm.Color,
                        ValueLabelColor = pm.Color
                    });

                foreach (var word in assessment.Words)
                {
                    var pronunciationWord = new PronunciationWord()
                    {
                        AccuracyScore = word.AccuracyScore,
                        Word = word.Word,
                        Error = word.ErrorType,
                    };

                    if (word.Phonemes != null)
                    {
                        foreach (var phoneme in word.Phonemes)
                        {
                            var pronunciationPhoneme = new PronunciationPhoneme()
                            {
                                Text = phoneme.Phoneme,
                                AccuracyScore = phoneme.AccuracyScore
                            };

                            Phonemes.Add(pronunciationPhoneme);
                        }
                    }

                    if (word.Syllables != null)
                    {
                        foreach (var syllable in word.Syllables)
                        {
                            var pronunciationSyllable = new PronunciationSyllable()
                            {
                                Text = syllable.Syllable,
                                AccuracyScore = syllable.AccuracyScore,
                                Grapheme = syllable.Grapheme
                            };

                            Syllables.Add(pronunciationSyllable);
                        }
                    }

                    Words.Add(pronunciationWord);
                }
            }

            IsBusy = false;
        }

        private async Task Listen()
        {
            var locales = await TextToSpeech.GetLocalesAsync();
            Locale locale = locales.FirstOrDefault(x => x.Language == "en-US");

            if (locale == null)
                locale = locales.FirstOrDefault(x => x.Language.StartsWith("en"));

            if (locale != null)
            {
                var settings = new SpeechOptions()
                {
                    Locale = locale
                };

                await TextToSpeech.SpeakAsync(TextToRecognize, settings);
            }
            else
                await TextToSpeech.SpeakAsync(TextToRecognize);
        }

        private void GetText()
        {
            Clear();
            TextToRecognize = textService.GetRandomText();
        }

        public LearnViewModel()
        {
            speechService = new SpeechService();
            textService = new TextService();

            Words = new ObservableCollection<PronunciationWord>();
            Phonemes = new ObservableCollection<PronunciationPhoneme>();
            Syllables = new ObservableCollection<PronunciationSyllable>();

            MetricsChart = new RadialGaugeChart()
            {
                IsAnimated = true,
                MinValue = 0,
                MaxValue = 100,
                LabelTextSize = 16
            };

            SpeakCommand = new Command(async () => await Speak());
            ListenCommand = new Command(async () => await Listen());
            GetTextCommand = new Command(GetText);

            GetTextCommand.Execute(null);
        }
    }
}
