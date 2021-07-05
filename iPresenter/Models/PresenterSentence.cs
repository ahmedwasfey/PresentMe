using System;

namespace iPresenter
{
    public class PresenterSentence : IPresenterItem
    {
        const int MaxCharactersPerLine = 50;

        public string Text;
        public float FontSize;
        public int Weight
        {
            get => (int)Math.Ceiling((Text.Length / MaxCharactersPerLine) * (FontSize * 1.3));
        }
    }
}
