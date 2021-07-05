using System.Collections.Generic;

namespace iPresenter
{
    public class SummarizedText
    {
        public string name { get; set; }
        public SummarizedMetaData Metadata { get; set; }
    }

    public class SummarizedMetaData
    {
        public List<SummarizedSection> Sections { get; set; }
    }

    public class SummarizedSection
    {
        public string Heading { get; set; }
        public string Text { get; set; }
    }
}
