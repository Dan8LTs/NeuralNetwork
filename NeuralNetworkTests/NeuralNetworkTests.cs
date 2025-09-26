using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace NeuralNetwork.Tests
{
    [TestClass()]
    public class NeuralNetworkTests
    {
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

            var topology = new Topology(4, 1, 0.1, 2);
            var neuralNetwork = new NeuralNetwork(topology);
            neuralNetwork.Learn(outputs, inputs, 80000);

            var results = new List<double>();
            for (int i = 0; i < outputs.Length; i++)
            {
                var row = NeuralNetwork.GetRow(inputs, i);
                results.Add(neuralNetwork.Predict(row).Output);
            }

            for (int i = 0; i < results.Count; i++)
            {
                var expected = Math.Round(outputs[i], 3);
                var actual = Math.Round(results[i], 3);
                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod()]
        public void RecognizeImages()
        {
            var size = 1000;
            var parasitizedPath = @"D:\Загрузки\cell_images\Parasitized\";
            var uninfectedPath = @"D:\Загрузки\cell_images\Uninfected\";

            var converter = new PictureConverter();
            var testParasitizedImageInput = converter.Convert(@"D:\Desktop\Danil\Projects\Visual Studio\Dan8LTs\NeuralNetwork\NeuralNetworkTests\Images\Parasitized.png");
            var testUninfectedImageInput = converter.Convert(@"D:\Desktop\Danil\Projects\Visual Studio\Dan8LTs\NeuralNetwork\NeuralNetworkTests\Images\Uninfected.png");

            var topology = new Topology(testParasitizedImageInput.Count, 1, 0.1, testParasitizedImageInput.Count / 2);
            var neuralNetwork = new NeuralNetwork(topology);

            double[,] parasitizedInputs = GetData(parasitizedPath, converter, testParasitizedImageInput, size);
            neuralNetwork.Learn(new double[] { 1 }, parasitizedInputs, 1);

            double[,] uninfectedInputs = GetData(uninfectedPath, converter, testUninfectedImageInput, size);
            neuralNetwork.Learn(new double[] { 0 }, uninfectedInputs, 1);

            var par = neuralNetwork.Predict(testParasitizedImageInput.Select(t => (double)t).ToArray());
            var uninf = neuralNetwork.Predict(testUninfectedImageInput.Select(t => (double)t).ToArray());

            var predPar = par.Output >= 0.5 ? 1 : 0;
            var predUninf = uninf.Output >= 0.5 ? 1 : 0;
            Assert.AreEqual(1, predPar, par.Output.ToString());
            Assert.AreEqual(0, predUninf, uninf.Output.ToString());
        }

        [TestMethod]
        public void HeartCsvPredictionTest()
        {
            var path = @"D:\Desktop\Danil\Projects\Visual Studio\Dan8LTs\NeuralNetwork\NeuralNetworkTests\heart.csv";
            if (!File.Exists(path))
            {
                Assert.Inconclusive("CSV file not found: " + path);
                return;
            }

            var rawLines = File.ReadAllLines(path);
            var lines = new List<string>(rawLines.Length);
            foreach (var ln in rawLines)
            {
                if (!string.IsNullOrWhiteSpace(ln))
                    lines.Add(ln.Trim());
            }

            int rowCount = lines.Count;
            var outputs = new double[rowCount];
            var inputs = new double[rowCount, 13];

            for (int i = 0; i < rowCount; i++)
            {
                var parts = lines[i].Split(',');
                if (parts.Length < 14)
                {
                    Assert.Fail($"Line {i} does not contain 14 values: '{lines[i]}'");
                }
                for (int j = 0; j < 13; j++)
                {
                    inputs[i, j] = Convert.ToDouble(parts[j].Trim(), new CultureInfo("en-US"));
                }
                outputs[i] = Convert.ToDouble(parts[13].Trim(), new CultureInfo("en-US"));
            }

            var topology = new Topology(13, 1, 0.1, 7);
            var neuralNetwork = new NeuralNetwork(topology);

            neuralNetwork.Learn(outputs, inputs, 80000);

            for (int i = 0; i < rowCount; i++)
            {
                var row = NeuralNetwork.GetRow(inputs, i);
                var pred = neuralNetwork.Predict(row).Output;
                int predicted = pred >= 0.5 ? 1 : 0;
                Assert.AreEqual(outputs[i], predicted, $"Row {i} mismatch: expected={outputs[i]}, predicted={pred}");
            }
        }

        private static double[,] GetData(string parasitizedPath, PictureConverter converter, System.Collections.Generic.List<int> testImageInput, int size)
        {
            var images = Directory.GetFiles(parasitizedPath);
            var result = new double[size, testImageInput.Count];
            for (int i = 0; i < size; i++)
            {
                var image = converter.Convert(images[i]);
                for (int j = 0; j < image.Count; j++)
                {
                    result[i, j] = image[j];
                }
            }
            return result;
        }
    }
}