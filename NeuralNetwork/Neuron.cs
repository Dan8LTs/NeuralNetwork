using System;
using System.Collections.Generic;

namespace NeuralNetwork
{
    public class Neuron
    {
        public List<double> Weights { get; }
        public List<double> Inputs { get; }
        public NeuronSignal Signal { get; }
        public double Output { get; private set; }
        public double Delta { get; private set; }

        private static readonly Random Rnd = new Random();

        public Neuron(int inputCount, NeuronSignal signal = NeuronSignal.Normal)
        {
            Signal = signal;
            Weights = new List<double>();
            Inputs = new List<double>();
            InitializeRandomWeightsValue(inputCount);
        }

        private void InitializeRandomWeightsValue(int input)
        {
            for (int i = 0; i < input; i++)
            {
                if (Signal == NeuronSignal.Input)
                {
                    Weights.Add(1);
                }
                else
                {
                    Weights.Add((Rnd.NextDouble() - 0.5));
                }
                Inputs.Add(0);
            }

            if (Signal != NeuronSignal.Input)
            {
                Weights.Add((Rnd.NextDouble() - 0.5));
                Inputs.Add(1);
            }
        }

        public double FeedForward(List<double> inputs)
        {
            for (int i = 0; i < inputs.Count; i++)
            {
                Inputs[i] = inputs[i];
            }

            if (Signal != NeuronSignal.Input && Inputs.Count > inputs.Count)
            {
                Inputs[inputs.Count] = 1.0;
            }

            var sum = 0.0;
            for (int i = 0; i < Inputs.Count; i++)
            {
                sum += Inputs[i] * Weights[i];
            }

            if (Signal != NeuronSignal.Input)
            {
                Output = Sigmoid(sum);
            }
            else
            {
                Output = sum;
            }

            return Output;
        }

        private double Sigmoid(double x)
        {
            return 1.0 / (1.0 + Math.Exp(-x));
        }
        private double SigmoidX(double output)
        {
            return output * (1 - output);
        }

        public void ComputeDelta(double error)
        {
            if (Signal == NeuronSignal.Input)
                return;
            Delta = error * SigmoidX(Output);
        }

        public void ComputeDeltaCrossEntropy(double error)
        {
            if (Signal == NeuronSignal.Input)
                return;
            Delta = error;
        }

        public void UpdateWeights(double learningRate)
        {
            if (Signal == NeuronSignal.Input)
                return;
            for (int i = 0; i < Weights.Count; i++)
            {
                var weight = Weights[i];
                var input = Inputs[i];
                var newWeight = weight - input * Delta * learningRate;
                Weights[i] = newWeight;
            }
        }

        public void Balancing(double error, double learningRate)
        {
            ComputeDelta(error);
            UpdateWeights(learningRate);
        }

        public override string ToString()
        {
            return Output.ToString();
        }
    }
}
