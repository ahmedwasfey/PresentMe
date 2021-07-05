using System.IO;
using Syncfusion.Drawing;

namespace iPresenter
{
    public class PresenterImage : IPresenterItem
    {
        public Stream Stream;
        public int Width;
        public int Height;

        public PresenterImage(Stream stream, int width, int height)
        {
            Stream = stream;
            Width = width;
            Height = height;
        }

        public PresenterImage(ImagePart imagePart)
        {
            var bytes = File.ReadAllBytes(imagePart.Path);
            Image image = Image.FromStream(File.Open(imagePart.Path, FileMode.Open));

            Stream = new MemoryStream(bytes);
            Width = image.Width;
            Height = image.Height;
        }

        public int Weight => Height + 100;
    }
}
