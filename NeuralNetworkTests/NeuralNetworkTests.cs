using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace NeuralNetwork.Tests
{
    [TestClass()]
    public class NeuralNetworkTests
    {
        [TestMethod()]
        public void FeedForwardTest()
        {
            var topology = new Topology(4, 1, 2);
            var neuralNetwork = new NeuralNetwork(topology);
            neuralNetwork.Layers[1].Neurons[0].SetWeight(0, 6, -0.2, 0.2, -0.3);
            neuralNetwork.Layers[1].Neurons[1].SetWeight(0.2, -0.3, 0.6, -0,3);
            neuralNetwork.Layers[2].Neurons[0].SetWeight(1.5, 0.9);

            var result = neuralNetwork.FeedForward(new List<double> { 1, 0, 0, 0 });
        }
    }
}