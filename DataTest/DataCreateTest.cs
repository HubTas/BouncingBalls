using Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace TestData
{
    [TestClass]
    public class DataCreationTest
    {
        private DataAbstractApi DApi;


        [TestMethod]
        public void testCreateApi()
        {
            DApi = DataAbstractApi.createApi(800, 600);
            Assert.IsNotNull(DApi);
        }

        [TestMethod]
        public void getWidtHeighTest()
        {
            DApi = DataAbstractApi.createApi(800, 600);
            Assert.AreEqual(800, DApi.width);
            Assert.AreEqual(600, DApi.height);
        }


    }
}