using NeuralNetwork;

namespace MedicalSystem
{
    class SystemController
    {
        public NeuralNetwork.NeuralNetwork ImageNetwork { get; }
        public NeuralNetwork.NeuralNetwork DataNetwork { get; }

        public SystemController()
        {
            var imageTopology = new Topology(400, 1, 0.1, 200);
            ImageNetwork = new NeuralNetwork.NeuralNetwork(imageTopology);

            var dataTopology = new Topology(13, 1, 0.1, 7);
            DataNetwork = new NeuralNetwork.NeuralNetwork(dataTopology);
        }
    }
}
