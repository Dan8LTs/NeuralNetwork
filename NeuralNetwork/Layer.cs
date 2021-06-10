using System;
using System.Collections.Generic;

namespace NeuralNetwork
{
    public class Layer
    {
        public List<Neuron> Neurons { get; }
        public int NeuronsCount => Neurons?.Count ?? 0;
        public NeuronSignal Signal;
        public Layer(List<Neuron> neurons, NeuronSignal signal = NeuronSignal.Normal)
        {
            //var testNS = new NeuronSignal();
            //var testNeuron = new Neuron(0, testNS);
            //for (int i = 0; i < neurons.Count - 1; i++)
            //{
            //    if (neurons[i].GetType() != testNeuron.GetType())
            //    {
            //        throw new ArgumentException("Neuron does not match the type", neurons[i - 1].GetType().Name);
            //    }
            //}

            //if (NeuronSignal.TryParse(signal.ToString(), out NeuronSignal neuronSignal) == false)
            //{
            //    throw new ArgumentException("Signal does not match the type", signal.GetType().Name);
            //}

            Neurons = neurons;
            Signal = signal;
        }

        public List<double> GetSignals()
        {
            var result = new List<double>();
            foreach (var neuron in Neurons)
            {
                result.Add(neuron.Output);
            }
            return result;
        }
        public override string ToString()
        {
            return Signal.ToString();
        }
    }
}
