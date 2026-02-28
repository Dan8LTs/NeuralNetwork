using NeuralNetwork;

namespace MedicalSystem
{
    class SystemController
    {
        public NeuralNetwork.NeuralNetwork ImageNetwork { get; set; }
        public NeuralNetwork.NeuralNetwork DataNetwork { get; private set; }

        public bool IsDataNetworkTrained { get; set; } = false;
        public bool IsImageNetworkTrained { get; set; } = false;

        public SystemController()
        {
            ResetDataNetwork();
        }

        public void ResetDataNetwork()
        {
            var dataTopology = new Topology(13, 1, 0.006, 11, 6);
            DataNetwork = new NeuralNetwork.NeuralNetwork(dataTopology);
            IsDataNetworkTrained = false;
        }

        public void CreateImageNetwork(int inputCount)
        {
            var hidden = inputCount / 2;
            if (hidden < 1) hidden = 1;
            var imageTopology = new Topology(inputCount, 1, 0.1, hidden);
            ImageNetwork = new NeuralNetwork.NeuralNetwork(imageTopology);
            IsImageNetworkTrained = false;
        }
    }
}
