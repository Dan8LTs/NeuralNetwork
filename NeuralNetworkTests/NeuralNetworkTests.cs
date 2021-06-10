﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace NeuralNetwork.Tests
{
    [TestClass()]
    public class NeuralNetworkTests
    {
        [TestMethod()]
        public void FeedForwardTest()
        {
            var dataset = new List<Tuple<double, double[]>>
            {
                //  Result
                //  The patient is sick - 1
                //  The patient is healthy - 0

                // Normal temperature - T
                // Good age - A
                // Smokes - S
                // Eats right - K 
                //                                                                     T  A  S  K
                new Tuple<double, double[]> (0, new double[] { 0, 0, 0, 0 }),
                new Tuple<double, double[]> (0, new double[] { 0, 0, 0, 1 }),
                new Tuple<double, double[]> (1, new double[] { 0, 0, 1, 0 }),
                new Tuple<double, double[]> (0, new double[] { 0, 0, 1, 1 }),
                new Tuple<double, double[]> (0, new double[] { 0, 1, 0, 0 }),
                new Tuple<double, double[]> (0, new double[] { 0, 1, 0, 1 }),
                new Tuple<double, double[]> (1, new double[] { 0, 1, 1, 0 }),
                new Tuple<double, double[]> (0, new double[] { 0, 1, 1, 1 }),
                new Tuple<double, double[]> (1, new double[] { 1, 0, 0, 0 }),
                new Tuple<double, double[]> (1, new double[] { 1, 0, 0, 1 }),
                new Tuple<double, double[]> (1, new double[] { 1, 0, 1, 0 }),
                new Tuple<double, double[]> (1, new double[] { 1, 0, 1, 1 }),
                new Tuple<double, double[]> (1, new double[] { 1, 1, 0, 0 }),
                new Tuple<double, double[]> (0, new double[] { 1, 1, 0, 1 }),
                new Tuple<double, double[]> (1, new double[] { 1, 1, 1, 0 }),
                new Tuple<double, double[]> (1, new double[] { 1, 1, 1, 1 })
            };

            var topology = new Topology(4, 1, 0.01, 4);
            var neuralNetwork = new NeuralNetwork(topology);
            var difference = neuralNetwork.Learn(dataset, 800000);

            var results = new List<double>();
            foreach (var data in dataset)
            {
                var res = neuralNetwork.FeedForward(data.Item2).Output;
                results.Add(res);
            }

            for (int i = 0; i < results.Count; i++)
            {
                var expected = Math.Round(dataset[i].Item1, 3);
                var actual = Math.Round(results[i], 3);
                Assert.AreEqual(expected, actual);
            }
        }
    }
}