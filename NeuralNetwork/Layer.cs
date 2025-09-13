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
