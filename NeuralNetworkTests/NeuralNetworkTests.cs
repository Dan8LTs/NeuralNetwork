using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NeuralNetwork.Tests
{
    /// <summary>
    /// Набор тестов для проверки функциональности нейронной сети
    /// </summary>
    [TestClass()]
    public class NeuralNetworkTests
    {
        /// <summary>
        /// Тест: проверка прямого распространения и обучения на простом наборе данных
        /// </summary>
        [TestMethod()]
        public void FeedForwardTest()
        {
            var outputs = new double[] { 0, 0, 1, 0, 0, 0, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1 };
            var inputs = new double[,]
            {
                //  Result
                //  The patient is sick - 1
                //  The patient is healthy - 0

                // Normal temperature - T
                // Good age - A
                // Smokes - S
                // Eats right - K 
                //T  A  S  K
                { 0, 0, 0, 0 },
                { 0, 0, 0, 1 },
                { 0, 0, 1, 0 },
                { 0, 0, 1, 1 },
                { 0, 1, 0, 0 },
                { 0, 1, 0, 1 },
                { 0, 1, 1, 0 },
                { 0, 1, 1, 1 },
                { 1, 0, 0, 0 },
                { 1, 0, 0, 1 },
                { 1, 0, 1, 0 },
                { 1, 0, 1, 1 },
                { 1, 1, 0, 0 },
                { 1, 1, 0, 1 },
                { 1, 1, 1, 0 },
                { 1, 1, 1, 1 }
            };

            var topology = new Topology(4, 1, 0.01, 2);
            var neuralNetwork = new NeuralNetwork(topology);
            neuralNetwork.Learn(outputs, inputs, 10000, (epoch, loss) =>
            {
                if (epoch % 2000 == 0)
                {
                    System.Diagnostics.Debug.WriteLine($"FeedForward Test - Epoch {epoch}: Loss = {loss:F6}");
                }
            });

            var results = new List<double>();
            for (int i = 0; i < outputs.Length; i++)
            {
                var row = NeuralNetwork.GetRow(inputs, i);
                results.Add(neuralNetwork.Predict(row).Output);
            }

            for (int i = 0; i < results.Count; i++)
            {
                var expected = Math.Round(outputs[i], 0);
                var actual = Math.Round(results[i], 0);
                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        /// Тест: классификация изображений клеток (зараженные vs незараженные)
        /// </summary>
        [TestMethod()]
        public void RecognizeImages()
        {
            RandomProvider.ResetSeed(42);
            var size = 500;

            // Пути к обучающим данным
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

            var parasitizedPath = Path.Combine(projectPath, @"Images\Parasitized\");
            var uninfectedPath = Path.Combine(projectPath, @"Images\Uninfected\");
            var testParasitizedImagePath = Path.Combine(projectPath, @"Images\Parasitized.png");
            var testUninfectedImagePath = Path.Combine(projectPath, @"Images\Uninfected.png");

            // Проверяем наличие нужных файлов и папок
            Assert.IsTrue(File.Exists(testParasitizedImagePath), $"Файл не найден: {testParasitizedImagePath}");
            Assert.IsTrue(File.Exists(testUninfectedImagePath), $"Файл не найден: {testUninfectedImagePath}");
            Assert.IsTrue(Directory.Exists(parasitizedPath), $"Папка не найдена: {parasitizedPath}");
            Assert.IsTrue(Directory.Exists(uninfectedPath), $"Папка не найдена: {uninfectedPath}");

            var converter = new PictureConverter();
            converter.GrayscaleMode = true;
            var testParasitizedImageInput = converter.Convert(testParasitizedImagePath);
            var testUninfectedImageInput = converter.Convert(testUninfectedImagePath);

            int diffCount = 0;
            for (int i = 0; i < testParasitizedImageInput.Count; i++)
                if (testParasitizedImageInput[i] != testUninfectedImageInput[i]) diffCount++;
            System.Diagnostics.Trace.WriteLine($"Pixel differences between test images: {diffCount}/{testParasitizedImageInput.Count}");

            var inputSize = testParasitizedImageInput.Count;
            var topology = new Topology(inputSize, 1, 0.01, inputSize / 30);
            var neuralNetwork = new NeuralNetwork(topology);

            // Загрузка изображений обоих классов
            double[,] parasitizedInputs = GetData(parasitizedPath, converter, testParasitizedImageInput, size);
            double[,] uninfectedInputs = GetData(uninfectedPath, converter, testUninfectedImageInput, size);

            // Объединение данных в один набор
            var totalSize = size * 2;
            var inputs = new double[totalSize, inputSize];
            var outputs = new double[totalSize];

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < inputSize; j++)
                    inputs[i, j] = parasitizedInputs[i, j];
                outputs[i] = 1; // заражённая
            }
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < inputSize; j++)
                    inputs[size + i, j] = uninfectedInputs[i, j];
                outputs[size + i] = 0; // здоровая
            }

            // Обучение на объединённом наборе
            neuralNetwork.Learn(outputs, inputs, 50, (epoch, loss) =>
            {
                if (epoch % 10 == 0)
                    System.Diagnostics.Debug.WriteLine($"Image Test - Epoch {epoch}: Loss = {loss:F6}");
            });

            var parOutput = neuralNetwork.Predict(testParasitizedImageInput.Select(t => (double)t).ToArray()).Output;
            var uninfOutput = neuralNetwork.Predict(testUninfectedImageInput.Select(t => (double)t).ToArray()).Output;

            System.Diagnostics.Trace.WriteLine($"RecognizeImages: par.Output={parOutput:F6}, uninf.Output={uninfOutput:F6}");

            var predPar = parOutput >= 0.5 ? 1 : 0;
            var predUninf = uninfOutput >= 0.5 ? 1 : 0;
            Assert.AreEqual(1, predPar, parOutput.ToString());
            Assert.AreEqual(0, predUninf, uninfOutput.ToString());
        }

        /// <summary>
        /// Вспомогательный метод: загружает и преобразует набор изображений в матрицу входных данных
        /// </summary>
        private static double[,] GetData(string path, PictureConverter converter, System.Collections.Generic.List<int> testImageInput, int size)
        {
            var allFiles = Directory.GetFiles(path);

            // Перемешиваем для репрезентативной случайной выборки (детерминировано через фиксированный seed)
            var rng = new Random(42);
            for (int i = allFiles.Length - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                var tmp = allFiles[i];
                allFiles[i] = allFiles[j];
                allFiles[j] = tmp;
            }

            var result = new double[size, testImageInput.Count];
            for (int i = 0; i < size; i++)
            {
                var image = converter.Convert(allFiles[i]);
                for (int j = 0; j < image.Count; j++)
                {
                    result[i, j] = image[j];
                }
            }
            return result;
        }
    }
}