using System.Collections.Generic;

namespace NeuralNetwork
{
    /// <summary>Конфигурация архитектуры нейронной сети</summary>
    public class Topology
    {
        public int InputCount { get; }           // Количество входных нейронов
        public int OutputCount { get; }          // Количество выходных нейронов
        public List<int> HiddenLayers { get; }   // Размеры скрытых слоев
        public double LearningRate { get; }      // Скорость обучения
        public double Regularization { get; set; } = 0.0001; // L2-регуляризация
        public double Momentum { get; set; } = 0.9;           // Коэффициент момента

        /// <param name="inputCount">Входы</param>
        /// <param name="outputCount">Выходы</param>
        /// <param name="learningRate">Скорость обучения</param>
        /// <param name="layers">Размеры скрытых слоев</param>
        public Topology(int inputCount, int outputCount, double learningRate, params int[] layers)
        {
            InputCount = inputCount;
            OutputCount = outputCount;
            LearningRate = learningRate;
            HiddenLayers = new List<int>(layers);
        }
    }
}
