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

            // Получаем путь к выходной директории (bin\Debug\net10.0-windows для .NET 10)
            var outputPath = AppDomain.CurrentDomain.BaseDirectory;

            // Поднимаемся до корня проекта NeuralNetworkTests
            // .NET 10: bin\Debug\net10.0-windows -> up 3 levels
            // .NET Framework: bin\Debug -> up 2 levels
            var projectPath = Path.GetFullPath(Path.Combine(outputPath, "..", "..", ".."));

            // Если путь не содержит Images (значит, не в корне NeuralNetworkTests), пробуем вариант для .NET Framework
            if (!Directory.Exists(Path.Combine(projectPath, "Images")))
            {
                projectPath = Path.GetFullPath(Path.Combine(outputPath, "..", ".."));
            }

            var parasitizedPath = Path.Combine(projectPath, @"Images\Parasitized.png");
            var uninfectedPath = Path.Combine(projectPath, @"Images\Uninfected.png");

            // Проверяем наличие файлов перед использованием
            Assert.IsTrue(File.Exists(parasitizedPath), $"Файл не найден: {parasitizedPath}");
            Assert.IsTrue(File.Exists(uninfectedPath), $"Файл не найден: {uninfectedPath}");

            var inputs1 = converter.Convert(parasitizedPath);
            var inputs2 = converter.Convert(uninfectedPath);
            converter.Save("PictConvParasitized.png", inputs1);
            converter.Save("PictConvUninfected.png", inputs2);
        }
    }
}