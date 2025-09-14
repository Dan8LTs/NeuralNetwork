using System;
using System.Collections.Generic;
using System.Linq;

namespace NeuralNetwork
{
    public class NeuralNetwork
    {
        public Topology Topology { get; }
        public List<Layer> Layers { get; }

        private double[] inputMeans;
        private double[] inputStds;

        private static readonly Random Rnd = new Random();

        public NeuralNetwork(Topology topology)
        {
            Topology = topology;
            Layers = new List<Layer>();

            CreateInputLayer();
            CreateHiddenLayers();
            CreateOutputLayer();
        }

        public Neuron Predict(params double[] inputSignals)
        {
            if (inputMeans != null && inputStds != null && inputSignals != null && inputSignals.Length == inputMeans.Length)
            {
                var scaled = new double[inputSignals.Length];
                for (int i = 0; i < inputSignals.Length; i++)
                {
                    var mean = inputMeans[i];
                    var std = inputStds[i];
                    if (Math.Abs(std) < Double.Epsilon)
                    {
                        scaled[i] = 0.0;
                    }
                    else
                    {
                        scaled[i] = (inputSignals[i] - mean) / std;
                    }
                }
                return PredictInternal(scaled);
            }
            else
            {
                return PredictInternal(inputSignals);
            }
        }

        private Neuron PredictInternal(params double[] inputSignals)
        {
            SendSignalsToInputNeurons(inputSignals);
            FeedForwardLayers();
            if (Topology.OutputCount == 1)
            {
                return Layers.Last().Neurons[0];
            }
            else
            {
                return Layers.Last().Neurons.OrderByDescending(n => n.Output).First();
            }
        }

        private void FeedForwardLayers()
        {
            for (int i = 1; i < Layers.Count; i++)
            {
                var layer = Layers[i];
                var previousLayerSignals = Layers[i - 1].GetSignals();
                foreach (var neuron in layer.Neurons)
                {
                    neuron.FeedForward(previousLayerSignals);
                }
            }
        }

        private void SendSignalsToInputNeurons(params double[] inputSignals)
        {
            for (int i = 0; i < inputSignals.Length; i++)
            {
                var signal = new List<double>() { inputSignals[i] };
                var neuron = Layers[0].Neurons[i];
                neuron.FeedForward(signal);
            }
        }

        public double Learn(double[] expected, double[,] inputs, int epoch)
        {
            var rowCount = inputs.GetLength(0);
            var colCount = inputs.GetLength(1);

            inputMeans = new double[colCount];
            inputStds = new double[colCount];
            for (int c = 0; c < colCount; c++)
            {
                double sum = 0.0;
                for (int r = 0; r < rowCount; r++)
                {
                    sum += inputs[r, c];
                }
                var mean = sum / rowCount;
                inputMeans[c] = mean;

                double varSum = 0.0;
                for (int r = 0; r < rowCount; r++)
                {
                    var d = inputs[r, c] - mean;
                    varSum += d * d;
                }
                var std = Math.Sqrt(varSum / rowCount);
                inputStds[c] = std;
            }

            var scaledInputs = new double[rowCount, colCount];
            for (int r = 0; r < rowCount; r++)
            {
                for (int c = 0; c < colCount; c++)
                {
                    var mean = inputMeans[c];
                    var std = inputStds[c];
                    var v = inputs[r, c];
                    if (Math.Abs(std) < Double.Epsilon)
                        scaledInputs[r, c] = 0.0;
                    else
                        scaledInputs[r, c] = (v - mean) / std;
                }
            }

            var error = 0.0;
            var sampleCount = expected.Length;

            var indices = new int[sampleCount];
            for (int i = 0; i < sampleCount; i++) indices[i] = i;

            const double decayFactor = 0.0001;

            for (int e = 0; e < epoch; e++)
            {
                var learningRate = Topology.LearningRate / (1.0 + decayFactor * e);

                for (int i = sampleCount - 1; i > 0; i--)
                {
                    int j = Rnd.Next(i + 1);
                    var tmp = indices[i];
                    indices[i] = indices[j];
                    indices[j] = tmp;
                }

                for (int k = 0; k < sampleCount; k++)
                {
                    var idx = indices[k];
                    var output = expected[idx];
                    var input = GetRow(scaledInputs, idx);
                    error += Backpropagation(output, learningRate, input);
                }
            }

            var result = error / (epoch * sampleCount);
            return result;
        }
        public static double[] GetRow(double[,] matrix, int row)
        {
            var columns = matrix.GetLength(1);
            var array = new double[columns];
            for (int i = 0; i < columns; ++i)
                array[i] = matrix[row, i];
            return array;
        }
        private double Backpropagation(double expectedResult, double learningRate, params double[] inputs)
        {
            PredictInternal(inputs);

            double sumSquaredError = 0.0;
            var outputLayer = Layers.Last();
            foreach (var neuron in outputLayer.Neurons)
            {
                var neuronError = neuron.Output - expectedResult;
                sumSquaredError += neuronError * neuronError;
                neuron.ComputeDeltaCrossEntropy(neuronError);
            }

            for (int j = Layers.Count - 2; j >= 1; j--)
            {
                var layer = Layers[j];
                var nextLayer = Layers[j + 1];

                for (int i = 0; i < layer.NeuronsCount; i++)
                {
                    var neuron = layer.Neurons[i];
                    double totalError = 0.0;
                    for (int k = 0; k < nextLayer.NeuronsCount; k++)
                    {
                        var nextNeuron = nextLayer.Neurons[k];
                        totalError += nextNeuron.Weights[i] * nextNeuron.Delta;
                    }
                    neuron.ComputeDelta(totalError);
                }
            }

            for (int j = 1; j < Layers.Count; j++)
            {
                var layer = Layers[j];
                foreach (var neuron in layer.Neurons)
                {
                    neuron.UpdateWeights(learningRate);
                }
            }

            return sumSquaredError;
        }

        private void CreateInputLayer()
        {
            var inputNeurons = new List<Neuron>();
            for (int i = 0; i < Topology.InputCount; i++)
            {
                var neuron = new Neuron(1, NeuronSignal.Input);
                inputNeurons.Add(neuron);
            }
            var inputLayer = new Layer(inputNeurons, NeuronSignal.Input);
            Layers.Add(inputLayer);
        }

        private void CreateHiddenLayers()
        {
            for (int j = 0; j < Topology.HiddenLayers.Count; j++)
            {
                var hiddenNeurons = new List<Neuron>();
                var lastLayer = Layers.Last();
                for (int i = 0; i < Topology.HiddenLayers[j]; i++)
                {
                    var neuron = new Neuron(lastLayer.NeuronsCount);
                    hiddenNeurons.Add(neuron);
                }
                var hiddenLayer = new Layer(hiddenNeurons);
                Layers.Add(hiddenLayer);
            }
        }

        private void CreateOutputLayer()
        {
            var outputNeurons = new List<Neuron>();
            var lastLayer = Layers.Last();
            for (int i = 0; i < Topology.OutputCount; i++)
            {
                var neuron = new Neuron(lastLayer.NeuronsCount, NeuronSignal.Output);
                outputNeurons.Add(neuron);
            }
            var outputLayer = new Layer(outputNeurons, NeuronSignal.Output);
            Layers.Add(outputLayer);
        }
    }
}