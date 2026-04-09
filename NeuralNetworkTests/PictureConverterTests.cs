using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace NeuralNetwork.Tests
{
    /// <summary>
    /// Набор тестов для преобразователя изображений
    /// </summary>
    [TestClass()]
    public class PictureConverterTests
    {
        /// <summary>
        /// Тест: проверка преобразования изображения в бинарный массив и сохранения обратно
        /// </summary>
        [TestMethod()]
        public void ConvertTest()
        {
            var converter = new PictureConverter();

            var outputPath = AppDomain.CurrentDomain.BaseDirectory;
            var projectPath = Path.GetFullPath(Path.Combine(outputPath, "..", "..", ".."));

            var parasitizedPath = Path.Combine(projectPath, @"Images\Parasitized.png");
            var uninfectedPath = Path.Combine(projectPath, @"Images\Uninfected.png");

            var inputs1 = converter.Convert(parasitizedPath);
            var inputs2 = converter.Convert(uninfectedPath);
            converter.Save("PictConvParasitized.png", inputs1);
            converter.Save("PictConvUninfected.png", inputs2);
        }
    }
}