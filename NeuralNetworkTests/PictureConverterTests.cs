using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NeuralNetwork.Tests
{
    [TestClass()]
    public class PictureConverterTests
    {
        [TestMethod()]
        public void ConvertTest()
        {
            var converter = new PictureConverter();
            var inputs = converter.Convert(@"C:\Users\lotus\source\repos\NeuralNetwork\NeuralNetworkTests\Images\Parasitized.png");
            converter.Save("E:\\image.png", inputs);
        }
    }
}