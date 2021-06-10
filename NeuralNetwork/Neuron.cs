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
        public Neuron(int inputCount, NeuronSignal signal = NeuronSignal.Normal)
        {
            //if (int.TryParse(inputCount.ToString(), out int input) == false)
            //{
            //    throw new ArgumentException("InputCount does not match the type", inputCount.ToString());
            //}
            //if (NeuronSignal.TryParse(signal.ToString(), out NeuronSignal neuronSignal) == false)
            //{
            //    throw new ArgumentException("Signal does not match the type", signal.GetType().Name);
            //}
            Signal = signal;
            Weights = new List<double>();
            Inputs = new List<double>();
            InitializeRandomWeightsValue(inputCount);
        }

        private void InitializeRandomWeightsValue(int input)
        {
            var rnd = new Random();

            for (int i = 0; i < input; i++)
            {
                if (Signal == NeuronSignal.Input)
                {
                    Weights.Add(1);
                }
                else
                {
                    Weights.Add(rnd.NextDouble());
                }
                Inputs.Add(0);
            }
        }

        public double FeedForward(List<double> inputs)
        {
            for (int i = 0; i < inputs.Count; i++)
            {
                Inputs[i] = inputs[i];
            }

            //for (int i = 0; i < inputs.Count - 1; i++)
            //{
            //    if (double.TryParse(inputs[i].ToString(), out double result) == false)
            //    {
            //        throw new ArgumentException("Signal does not match the type", inputs[i].ToString());
            //    }
            //}

            var sum = 0.0;
            for (int i = 0; i < inputs.Count; i++)
            {
                sum += inputs[i] * Weights[i];
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
            var result = 1.0 / (1.0 + Math.Exp(-x));
            return result;
        }

        private double SigmoidX(double x)
        {
            var sigmoid = Sigmoid(x);
            var result = sigmoid / (1 - sigmoid);
            return result;
        }

        public void Balancing(double error, double learningRate)
        {
            if (Signal == NeuronSignal.Input)
            {
                return;
            }
            Delta = error * SigmoidX(Output);
            for (int i = 0; i < Weights.Count; i++)
            {
                var weight = Weights[i];
                var input = Inputs[i];

                var newWeight = weight - input * Delta * learningRate;
                Weights[i] = newWeight;
            }
        }

        public override string ToString()
        {
            return Output.ToString();
        }
    }
}
