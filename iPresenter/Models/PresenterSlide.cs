using Syncfusion.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iPresenter
{
    public class PresenterSlide
    {
        public const int MaxWeight = 440;

        public List<IPresenterItem> Items = new();

        public ISlide AddToPresentation(IPresentation pptxDoc)
        {
            var sentences = Items.Where(i => i is PresenterSentence).Select(i => i as PresenterSentence).ToList();
            var images = Items.Where(i => i is PresenterImage).Select(i => i as PresenterImage).ToList();

            //Sentences only
            if (sentences.Any() && !images.Any())
            {
                ISlide slide = pptxDoc.Slides.Add(SlideLayoutType.Blank);

                IShape textShape = slide.AddTextBox(100, 50, 756, 500);
                IParagraph paragraph = textShape.TextBody.AddParagraph();
                paragraph.HorizontalAlignment = HorizontalAlignmentType.Center;

                for (int i = 0; i < sentences.Count; i++)
                {
                    var sentence = sentences[i];
                    var text = $"{sentence.Text}.";
                    if (i != sentences.Count - 1) text += "\n";

                    ITextPart textPart = paragraph.AddTextPart(text);
                    textPart.Font.FontSize = sentence.FontSize;
                    //textPart.Font.Bold = true;
                }

                paragraph.ListFormat.Type = ListType.Bulleted;
                paragraph.ListFormat.BulletCharacter = Convert.ToChar(183);
                paragraph.ListFormat.FontName = "Symbol";
                paragraph.FirstLineIndent = -20;

                return slide;
            }

            //Single Image only
            if (images.Count == 1 && sentences.Count == 0)
            {
                ISlide slide = pptxDoc.Slides.Add(SlideLayoutType.Blank);
                var image = images.First();
                var left = (slide.SlideSize.Width / 2) - (image.Width / 2);
                var top = (slide.SlideSize.Height / 2) - (image.Height / 2);
                IPicture picture = slide.Pictures.AddPicture(image.Stream, left, top, image.Width, image.Height);

                return slide;
            }

            //Two Images or more only
            if (images.Count > 1 && sentences.Count == 0)
            {
                ISlide slide = pptxDoc.Slides.Add(SlideLayoutType.Blank);
                return slide;
            }

            //Sentence or more && Single Image
            if (sentences.Count > 0 && images.Count == 1)
            {
                ISlide slide = pptxDoc.Slides.Add(SlideLayoutType.Blank);

                var paddingTop = 50;
                var paddingBottom = 50;

                var image = images.First();
                var left = (slide.SlideSize.Width / 2) - (image.Width / 2);
                var top = paddingTop;
                IPicture picture = slide.Pictures.AddPicture(image.Stream, left, top, image.Width, image.Height);

                var textPaddingTop = 50;

                var textTop = image.Height + paddingTop + textPaddingTop;

                IShape textShape = slide.AddTextBox(100, textTop, 756, slide.SlideSize.Height - (textTop + paddingBottom));
                IParagraph paragraph = textShape.TextBody.AddParagraph();
                paragraph.HorizontalAlignment = HorizontalAlignmentType.Center;

                for (int i = 0; i < sentences.Count; i++)
                {
                    var sentence = sentences[i];
                    var text = $"{sentence.Text}.";
                    if (i != sentences.Count - 1) text += "\n";

                    ITextPart textPart = paragraph.AddTextPart(text);
                    textPart.Font.FontSize = sentence.FontSize;
                    //textPart.Font.Bold = true;
                }

                paragraph.ListFormat.Type = ListType.Bulleted;
                paragraph.ListFormat.BulletCharacter = Convert.ToChar(183);
                paragraph.ListFormat.FontName = "Symbol";
                paragraph.FirstLineIndent = -20;

                return slide;
            }

            //Sentence or more && Two Images or more only
            if (sentences.Count > 0 && images.Count > 1)
            {
                ISlide slide = pptxDoc.Slides.Add(SlideLayoutType.Blank);
                return slide;
            }

            throw new Exception("Unsupported Slide Layout");

            //ISlide emptySlide = pptxDoc.Slides.Add(SlideLayoutType.Blank);
            //return emptySlide;
        }

        public bool WillFit(IPresenterItem item)
        {
            var remainingWeight = RemainingWeight;
            if (remainingWeight < item.Weight)
            {
                //Single sentence is too large, maybe truncate ?

                if (!Items.Any())
                    throw new Exception("Single sentence is too large, maybe truncate.");

                return false;
            }
            return true;
        }

        public int RemainingWeight => MaxWeight - Items.Sum(s => s.Weight);
        public int TotalWeight => Items.Sum(s => s.Weight);
        public bool IsOverWeight => Items.Sum(s => s.Weight) > MaxWeight;
    }
}
