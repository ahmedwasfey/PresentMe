using Syncfusion.Presentation;
using System.Collections.Generic;

namespace iPresenter
{
    public interface IPresenter
    {
        IPresentation ToPresentation(SummarizedText summarizedText, List<ImagePart> imageParts);
    }
}