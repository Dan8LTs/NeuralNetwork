using NeuralNetwork;
using System;

namespace MedicalSystem
{
    /// <summary>
    /// Контроллер для управления нейронными сетями медицинской системы
    /// </summary>
    class SystemController
    {
        /// <summary>
        /// Нейронная сеть для анализа изображений
        /// </summary>
        public NeuralNetwork.NeuralNetwork ImageNetwork { get; set; }

        /// <summary>
        /// Нейронная сеть для анализа медицинских данных (13 входов, 16-8 скрытых слоев)
        /// </summary>
        public NeuralNetwork.NeuralNetwork DataNetwork { get; private set; }

        /// <summary>
        /// Статус обучения сети для анализа медицинских данных
        /// </summary>
        public bool IsDataNetworkTrained { get; set; } = false;

        /// <summary>
        /// Статус обучения сети изображений
        /// </summary>
        public bool IsImageNetworkTrained { get; set; } = false;

        /// <summary>
        /// Инициализирует контроллер и создает новую сеть данных
        /// </summary>
        public SystemController()
        {
            ResetDataNetwork();
        }

        /// <summary>
        /// Переустанавливает сеть данных с фиксированным сидом для воспроизводимости
        /// </summary>
        public void ResetDataNetwork()
        {
            RandomProvider.ResetSeed(42);
            var dataTopology = new Topology(13, 1, 0.1, 16, 8);
            dataTopology.Regularization = 0.0018;
            dataTopology.Momentum = 0.9;
            DataNetwork = new NeuralNetwork.NeuralNetwork(dataTopology);
            IsDataNetworkTrained = false;
        }

        /// <summary>
        /// Создает сеть изображений с динамической топологией (скрытый слой = входы/2)
        /// </summary>
        public void CreateImageNetwork(int inputCount)
        {
            var hidden = Math.Max(1, inputCount / 50);
            var imageTopology = new Topology(inputCount, 1, 0.001, hidden);
            imageTopology.Regularization = 0.005;
            imageTopology.Momentum = 0.9;
            ImageNetwork = new NeuralNetwork.NeuralNetwork(imageTopology);
            IsImageNetworkTrained = false;
        }
    }
}
