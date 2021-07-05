using Syncfusion.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iPresenter
{
    public class SmarterPresenter : IPresenter
    {
        const int PreferedFontSize = 26;
        public IPresentation ToPresentation(SummarizedText summarizedText, List<ImagePart> imageParts)
        {
            var pptxDoc = Presentation.Create();
            var parts = summarizedText.Metadata.Sections;
            foreach (var part in parts)
            {
                var sentencesText = part.Text.Split(".").Where(t => !string.IsNullOrWhiteSpace(t)).Select(t => t.Trim()).ToList();
                System.Console.Write(sentencesText)
                ImagePart image = imageParts.FirstOrDefault(imagePart => part.Heading.Equals(imagePart.Heading));//part.Image;

                var presenterSlide = new PresenterSlide();

                foreach (var sentenceText in sentencesText)
                {
                    var sentence = new PresenterSentence
                    {
                        Text = sentenceText,
                        FontSize = PreferedFontSize,
                    };
                    bool willFit = presenterSlide.WillFit(sentence);
                    if (willFit)
                    {
                        presenterSlide.Items.Add(sentence);
                        continue;
                    }
                    else
                    {
                        presenterSlide.AddToPresentation(pptxDoc);

                        presenterSlide = new PresenterSlide();
                        sentence.FontSize = PreferedFontSize;
                        presenterSlide.Items.Add(sentence);
                    }
                }

                if (image != null)
                {
                    var presenterImage = new PresenterImage(image);
                    bool willFit = presenterSlide.WillFit(presenterImage);
                    if (willFit)
                    {
                        presenterSlide.Items.Add(presenterImage);
                    }
                    else
                    {
                        presenterSlide.AddToPresentation(pptxDoc);

                        presenterSlide = new PresenterSlide();
                        presenterSlide.Items.Add(presenterImage);
                    }
                }

                if (presenterSlide.Items.Any())
                {
                    presenterSlide.AddToPresentation(pptxDoc);
                }
            }
            return pptxDoc;
        }
    }
}
