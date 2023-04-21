using Logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace TestLogic
{
    [TestClass]
    public class CreationTest
    {

        private LogicAbstractApi LApi;


        [TestMethod]
        public void testCreateApi()
        {
            LApi = LogicAbstractApi.createApi(800, 600);
            Assert.IsNotNull(LApi);
        }

        [TestMethod]
        public void getWidthHeightTest()
        {
            LApi = LogicAbstractApi.createApi(800, 600);
            Assert.AreEqual(800, LApi.width);
            Assert.AreEqual(600, LApi.height);

        }

        [TestMethod]
        public void createDeleteTest()
        {
            LApi = LogicAbstractApi.createApi(800, 600);
            Assert.AreEqual(5, LApi.createBalls(5).Count);
            Assert.AreEqual(2, LApi.deleteBalls(3).Count);
            Assert.AreEqual(0, LApi.deleteBalls(3).Count);
        }

    }
}