using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace CryptoCorefx
{
    public class ImageTest
    {
        public ImageTest() { }

        public async Task Process()
        {
            HttpClient client = new HttpClient();
            var stream = await client.GetStreamAsync("https://ss0.baidu.com/6ONWsjip0QIZ8tyhnq/it/u=2933773599,3484489584&fm=58");

            int size = 150;
            int quality = 75;

            string path = Path.Combine(Directory.GetCurrentDirectory(), "image/test.jpg");

            using (var imgSource = Image.FromStream(stream))
            {
                var format = imgSource.RawFormat;
                int width, height;
                if (imgSource.Width > imgSource.Height)
                {
                    width = size;
                    height = imgSource.Height * size / imgSource.Width;
                }
                else
                {
                    width = imgSource.Width * size / imgSource.Height;
                    height = size;
                }

                using (Bitmap outMap = new Bitmap(width, height))
                {
                    using (Graphics g = Graphics.FromImage(outMap))
                    {
                        g.Clear(Color.Transparent);
                        g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.DrawImage(imgSource, new Rectangle(0, 0, width, height), 0, 0, imgSource.Width, imgSource.Height, GraphicsUnit.Pixel);

                        outMap.Save(path, ImageFormat.Jpeg);
                    }
                }
            }
        }
    }
}
