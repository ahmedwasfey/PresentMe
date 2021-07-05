using Syncfusion.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iPresenter
{
    /* public class Presenter_BigFirstSmallSecond : IPresenter
     {
         float minFontSize = 11.0f;
         float maxFontSize = 40.0f;

         float CalcualteFontForMaxWeight(string text, int maxWeight)
         {
             var sentence = new PresenterSentence { Text = text, FontSize = maxFontSize, };
             for (; sentence.FontSize > 11; sentence.FontSize--)
             {
                 if (sentence.Weight < maxWeight)
                     return sentence.FontSize;
             }
             return minFontSize;
         }

         public IPresentation ToPresentation(List<SummarizedPart> parts)
         {
             var pptxDoc = Presentation.Create();
             foreach (var part in parts)
             {
                 var sentencesText = part.Text.Split(".").Where(t => !string.IsNullOrWhiteSpace(t)).Select(t => t.Trim()).ToList();
                 var presenterSlide = new PresenterSlide();

                 foreach (var sentenceText in sentencesText)
                 {
                     var sentence = new PresenterSentence
                     {
                         Text = sentenceText,
                         FontSize = CalcualteFontForMaxWeight(sentenceText, presenterSlide.RemainingWeight),
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
                         sentence.FontSize = CalcualteFontForMaxWeight(sentenceText, presenterSlide.RemainingWeight);
                         presenterSlide.Items.Add(sentence);
                     }
                 }

                 if (presenterSlide.Items.Any())
                 {
                     presenterSlide.AddToPresentation(pptxDoc);
                 }
             }
             return pptxDoc;
         }
     }*/
}
