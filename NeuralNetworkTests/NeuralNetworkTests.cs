using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
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
            var difference = neuralNetwork.Learn(outputs, inputs, 80000);

            var results = new List<double>();
            for (int i = 0; i < outputs.Length; i++)
            {
                var row = NeuralNetwork.GetRow(inputs, i);
                var res = neuralNetwork.Predict(row).Output;
                results.Add(res);
            }

            for (int i = 0; i < results.Count; i++)
            {
                var expected = Math.Round(outputs[i], 3);
                var actual = Math.Round(results[i], 3);
                Assert.AreEqual(expected, actual);
            }
        }

        public void RecognizeImages()
        {
            var size = 1000;
            var parasitizedPath = @"E:\cell_images\Parasitized\";
            var unparasitizedPath = @"E:\cell_images\Unparasitized\";

            var converter = new PictureConverter();
            var testParasitizedImageInput = converter.Convert(@"C: \Users\lotus\source\repos\NeuralNetwork\NeuralNetworkTests\Images\Parasitized.png");
            var testUnparasitizedImageInput = converter.Convert(@"C: \Users\lotus\source\repos\NeuralNetwork\NeuralNetworkTests\Images\Unarasitized.png");

            var topology = new Topology(testParasitizedImageInput.Count, 1, 0.1, testParasitizedImageInput.Count / 2);
            var neuralNetwork = new NeuralNetwork(topology);

            double[,] parasitizedInputs = GetData(parasitizedPath, converter, testParasitizedImageInput, size);
            neuralNetwork.Learn(new double[] { 1 }, parasitizedInputs, 1);

            double[,] unparasitizedInputs = GetData(unparasitizedPath, converter, testParasitizedImageInput, size);
            neuralNetwork.Learn(new double[] { 0 }, unparasitizedInputs, 1);

            var par = neuralNetwork.Predict(testParasitizedImageInput.Select(t => (double)t).ToArray());
            var unpar = neuralNetwork.Predict(testUnparasitizedImageInput.Select(t => (double)t).ToArray());

            Assert.AreEqual(1, Math.Round(par.Output, 2));
            Assert.AreEqual(0, Math.Round(unpar.Output, 2));
        }
        private static double[,] GetData(string parasitizedPath, PictureConverter converter, List<int> testImageInput, int size)
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