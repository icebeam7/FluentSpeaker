using System;

namespace FluentSpeaker.Services
{
    internal class TextService
    {
        private string[] texts = {
            "Good morning everybody, how are you doing today?",
            "I want to practice my English speaking skills",
            "The next exam will be easy if I study a lot",
            "What did you do yesterday at home?",
            "What is your favorite color? Mine is blue",
            "Could you please give me a cookie?"
        };

        Random random;

        public TextService()
        {
            random = new Random();
        }

        public string GetRandomText()
        {
            var index = random.Next(texts.Length);
            return texts[index];
        }
    }
}
