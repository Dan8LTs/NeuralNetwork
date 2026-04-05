using System;

namespace NeuralNetwork
{
    /// <summary>Глобальный генератор случайных чисел с фиксированным seed для воспроизводимости</summary>
    public static class RandomProvider
    {
        private static Random _instance;
        private const int DefaultSeed = 42;

        static RandomProvider()
        {
            _instance = new Random(DefaultSeed);
        }

        public static Random Instance => _instance;

        /// <summary>Переинициализировать генератор с новым seed</summary>
        public static void ResetSeed(int seed = DefaultSeed)
        {
            _instance = new Random(seed);
        }
    }
}
