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
            var inputs = converter.Convert(@"D:\Desktop\Danil\Projects\Visual Studio\Dan8LTs\NeuralNetwork\NeuralNetworkTests\Images\Parasitized.png");
            converter.Save("image.png", inputs);
        }
    }
}