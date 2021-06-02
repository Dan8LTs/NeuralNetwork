using System;
using System.Collections.Generic;

namespace NeuralNetwork
{
    public class Neuron
    {
        public List<double> Weights { get; }
        public NeuronSignal Signal { get; }
        public double Output { get; private set; }
        public Neuron(int inputCount, NeuronSignal signal = NeuronSignal.Normal)
        {
            if(int.TryParse(inputCount.ToString(), out int input) == false)
            {
                throw new ArgumentException("InputCount does not match the type", inputCount.ToString());
            }
            if (NeuronSignal.TryParse(signal.ToString(), out NeuronSignal neuronSignal) == false)
            {
                throw new ArgumentException("Signal does not match the type", signal.GetType().Name);
            }
            Signal = neuronSignal;
            Weights = new List<double>();
            for(int i = 0; i < input;  i++)
            {
                Weights.Add(1);
            }
        }

        public double FeedForward(List<double> inputs)
        {
            for(int i = 0; i < inputs.Count; i++)
            {
                if (double.TryParse(inputs[i - 1].ToString(), out  double result) == false)
                {
                    throw new ArgumentException("Signal does not match the type", inputs[i].ToString());
                }
            }
            var sum = 0.0;
            for(int i = 0; i < inputs.Count; i++)
            {
                sum += inputs[i] * Weights[i];
            }

            Output = Sigmoid(sum);
            return Output;
        }

        private double Sigmoid(double x)
        {
            var result = 1.0 / (1.0 + Math.Exp(-x));
            return result;
        }
        public void SetWeight(params double[] weights)
        {
            // TODO: Remove after adding neural network training capability.
            for (int i = 0; i < weights.Length; i++)
            {
                Weights[i] = weights[i];
            }
        }
        public override string ToString()
        {
            return Output.ToString();
        }
    }
}
