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

        [TestMethod()]
        public void RecognizeImages()
        {
            var size = 1000;
            var parasitizedPath = @"D:\Desktop\Danil\Pictures\cell_images\Parasitized\";
            var uninfectedPath = @"D:\Desktop\Danil\Pictures\cell_images\Uninfected\";

            var converter = new PictureConverter();
            var testParasitizedImageInput = converter.Convert(@"D:\Desktop\Danil\Projects\Visual Studio\Dan8LTs\NeuralNetwork\NeuralNetworkTests\Images\Parasitized.png");
            var testUninfectedImageInput = converter.Convert(@"D:\Desktop\Danil\Projects\Visual Studio\Dan8LTs\NeuralNetwork\NeuralNetworkTests\Images\Uninfected.png");

            var topology = new Topology(testParasitizedImageInput.Count, 1, 0.1, testParasitizedImageInput.Count / 2);
            var neuralNetwork = new NeuralNetwork(topology);

            double[,] parasitizedInputs = GetData(parasitizedPath, converter, testParasitizedImageInput, size);
            neuralNetwork.Learn(new double[] { 1 }, parasitizedInputs, 1, (epoch, loss) =>
            {
                System.Diagnostics.Debug.WriteLine($"Image Test Parasitized - Epoch {epoch}: Loss = {loss:F6}");
            });

            double[,] uninfectedInputs = GetData(uninfectedPath, converter, testUninfectedImageInput, size);
            neuralNetwork.Learn(new double[] { 0 }, uninfectedInputs, 1, (epoch, loss) =>
            {
                System.Diagnostics.Debug.WriteLine($"Image Test Uninfected - Epoch {epoch}: Loss = {loss:F6}");
            });

            var par = neuralNetwork.Predict(testParasitizedImageInput.Select(t => (double)t).ToArray());
            var uninf = neuralNetwork.Predict(testUninfectedImageInput.Select(t => (double)t).ToArray());

            var predPar = par.Output >= 0.5 ? 1 : 0;
            var predUninf = uninf.Output >= 0.5 ? 1 : 0;
            Assert.AreEqual(1, predPar, par.Output.ToString());
            Assert.AreEqual(0, predUninf, uninf.Output.ToString());
        }

        //[TestMethod]
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

            // Split data: 70% training, 30% test
            var random = new Random();
            var indices = Enumerable.Range(0, rowCount).ToList();

            // Shuffle indices
            for (int i = indices.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                var temp = indices[i];
                indices[i] = indices[j];
                indices[j] = temp;
            }

            int trainCount = (int)(rowCount * 0.7);
            int testCount = rowCount - trainCount;

            var trainIndices = indices.Take(trainCount).ToArray();
            var testIndices = indices.Skip(trainCount).ToArray();

            var trainOutputs = trainIndices.Select(i => outputs[i]).ToArray();
            var trainInputs = new double[trainCount, 13];
            for (int i = 0; i < trainCount; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    trainInputs[i, j] = inputs[trainIndices[i], j];
                }
            }

            var testOutputs = testIndices.Select(i => outputs[i]).ToArray();
            var testInputs = new double[testCount, 13];
            for (int i = 0; i < testCount; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    testInputs[i, j] = inputs[testIndices[i], j];
                }
            }

            // Train network
            var topology = new Topology(13, 1, 0.1, 16, 8);
            var neuralNetwork = new NeuralNetwork(topology);
            neuralNetwork.Learn(trainOutputs, trainInputs, 10000, (epoch, loss) =>
            {
                System.Diagnostics.Debug.WriteLine($"[TEST] Epoch {epoch:D5}: Loss = {loss:E10}");
            });

            // Evaluate on training set
            int trainCorrect = 0;
            for (int i = 0; i < trainCount; i++)
            {
                var row = NeuralNetwork.GetRow(trainInputs, i);
                var pred = neuralNetwork.Predict(row).Output;
                int predicted = pred >= 0.5 ? 1 : 0;
                if (outputs[trainIndices[i]] == predicted)
                    trainCorrect++;
            }
            double trainAccuracy = (double)trainCorrect / trainCount;

            // Evaluate on test set
            int testCorrect = 0;
            for (int i = 0; i < testCount; i++)
            {
                var row = NeuralNetwork.GetRow(testInputs, i);
                var pred = neuralNetwork.Predict(row).Output;
                int predicted = pred >= 0.5 ? 1 : 0;
                if (outputs[testIndices[i]] == predicted)
                    testCorrect++;
            }
            double testAccuracy = (double)testCorrect / testCount;

            // Log results
            System.Diagnostics.Debug.WriteLine($"Training Accuracy: {trainAccuracy:P2} ({trainCorrect}/{trainCount})");
            System.Diagnostics.Debug.WriteLine($"Test Accuracy: {testAccuracy:P2} ({testCorrect}/{testCount})");

            // Assert minimum accuracy
            Assert.IsTrue(testAccuracy >= 0.6,
                $"Test accuracy too low: {testAccuracy:P2}. Train accuracy: {trainAccuracy:P2}");
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