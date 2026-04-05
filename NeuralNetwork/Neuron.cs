using System;
using System.Collections.Generic;

namespace NeuralNetwork
{
    /// <summary>Нейрон с поддержкой Momentum оптимизации</summary>
    public class Neuron
    {
        public List<double> Weights { get; }          // Веса синапсов
        public List<double> WeightVelocities { get; } // Скорости для Momentum
        public List<double> Inputs { get; }           // Входные сигналы
        public NeuronSignal Signal { get; }           // Тип: Input/Normal
        public double Output { get; private set; }    // Выход после сигмоида
        public double Delta { get; private set; }     // Локальный градиент

        public Neuron(int inputCount, NeuronSignal signal = NeuronSignal.Normal)
        {
            Signal = signal;
            Weights = new List<double>();
            WeightVelocities = new List<double>();
            Inputs = new List<double>();
            InitializeRandomWeightsValue(inputCount);
        }

        /// <summary>Инициализация весов со случайными значениями</summary>
        private void InitializeRandomWeightsValue(int input)
        {
            double limit = Math.Sqrt(2.0 / (input + 1));

            for (int i = 0; i < input; i++)
            {
                if (Signal == NeuronSignal.Input)
                {
                    Weights.Add(1);
                }
                else
                {
                    Weights.Add((RandomProvider.Instance.NextDouble() - 0.5) * 2 * limit);
                }
                Inputs.Add(0);
                WeightVelocities.Add(0.0);
            }

            if (Signal != NeuronSignal.Input)
            {
                Weights.Add((RandomProvider.Instance.NextDouble() - 0.5) * 2 * limit);
                Inputs.Add(1);
                WeightVelocities.Add(0.0);
            }
        }

        /// <summary>Прямое распространение: output = sigmoid(сумма(вход * вес))</summary>
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

        /// <summary>Производная сигмоида: σ'(x) = σ(x) * (1 - σ(x))</summary>
        private double SigmoidX(double output)
        {
            return output * (1 - output);
        }

        /// <summary>Вычислить дельта для промежуточных нейронов</summary>
        public void ComputeDelta(double error)
        {
            if (Signal == NeuronSignal.Input)
                return;
            Delta = error * SigmoidX(Output);
        }

        /// <summary>Вычислить дельта для выходного слоя (кросс-энтропия)</summary>
        public void ComputeDeltaCrossEntropy(double error)
        {
            if (Signal == NeuronSignal.Input)
                return;
            Delta = error;
        }

        /// <summary>Обновить веса: SGD с Momentum и L2 регуляризацией</summary>
        public void UpdateWeights(double learningRate, double momentum = 0.9, double regularization = 0.0001)
        {
            if (Signal == NeuronSignal.Input)
                return;

            for (int i = 0; i < Weights.Count; i++)
            {
                var weight = Weights[i];
                var input = Inputs[i];
                double gradient = Delta * input;
                bool isBias = (i == Weights.Count - 1);
                double regTerm = isBias ? 0.0 : regularization * weight;
                WeightVelocities[i] = momentum * WeightVelocities[i] + gradient + regTerm;
                Weights[i] = weight - learningRate * WeightVelocities[i];
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
