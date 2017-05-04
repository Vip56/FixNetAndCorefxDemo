using System;
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
            var stream = await client.GetStreamAsync("http://img1.cache.netease.com/catchpic/3/30/30082E5F12205745CAC3105B8764984B.jpg");

            int size = 1024;
            int quality = 75;

            string path = Path.Combine(Directory.GetCurrentDirectory(), "image");
            path = Path.Combine(path, "test.jpg");

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

                        Font font = new Font("WenQuanYi Zen Hei", 18);
                        SizeF fontSize = g.MeasureString("中文测试 test", font);

                        StringFormat stringFormat = new StringFormat { Alignment = StringAlignment.Center };
                        SolidBrush solodBrush = new SolidBrush(Color.FromArgb(220, 0, 0, 0));
                        g.DrawString("中文测试 test", font, solodBrush, width - fontSize.Width, height - fontSize.Height, stringFormat);
                        solodBrush.Dispose();
                    }

                    outMap.Save(path, ImageFormat.Jpeg);
                }
            }
        }

        public void DrawString()
        {

        }
    }
}
