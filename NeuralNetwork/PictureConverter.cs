using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace NeuralNetwork
{
    /// <summary>Конвертор изображений в массив пикселей (0/1 или 0-255)</summary>
    public class PictureConverter
    {
        public int Bondary { get; set; } = 128;       // Порог яркости для бинаризации
        public bool GrayscaleMode { get; set; } = false; // true — возвращать 0-255, false — бинарный 0/1
        public int Width { get; set; }                // Ширина изображения после обработки
        public int Height { get; set; }               // Высота изображения после обработки

        /// <summary>Загрузить изображение, изменить размер на 30x30 и преобразовать в массив пикселей</summary>
        public List<int> Convert(string path)
        {
            var result = new List<int>();
            var image = new Bitmap(path);

            var resizeImage = new Bitmap(30, 30);
            using (var g = Graphics.FromImage(resizeImage))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.DrawImage(image, 0, 0, 30, 30);
            }

            Height = resizeImage.Height;
            Width = resizeImage.Width;

            for (int y = 0; y < resizeImage.Height; y++)
            {
                for (int x = 0; x < resizeImage.Width; x++)
                {
                    var pixel = resizeImage.GetPixel(x, y);
                    result.Add(Brightness(pixel));
                }
            }
            return result;
        }

        /// <summary>Яркость пикселя: 0-255 (оттенки серого) или 0/1 (бинарный)</summary>
        public int Brightness(Color pixel)
        {
            var brightness = 0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B;
            if (GrayscaleMode)
                return (int)brightness;
            return brightness < Bondary ? 0 : 1;
        }

        /// <summary>Сохранить массив пикселей как изображение (чёрный/белый)</summary>
        public void Save(string path, List<int> pixels)
        {
            var image = new Bitmap(Width, Height);
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    var rawValue = pixels[y * Width + x];
                    var gray = GrayscaleMode ? rawValue : (rawValue == 1 ? 255 : 0);
                    image.SetPixel(x, y, Color.FromArgb(gray, gray, gray));
                }
            }
            image.Save(path);
        }
    }
}
