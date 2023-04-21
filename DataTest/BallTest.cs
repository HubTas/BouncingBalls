using Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Concurrent;

namespace TestData
{


    [TestClass]
    public class BallTest
    {
        private DataAbstractApi DApi;

        [TestMethod]
        public void createIBallTest()
        {
            DApi = DataAbstractApi.createApi(800, 600);
            IBall b = DApi.createBall(1);
            Assert.AreEqual(1, b.ballID);

            Assert.IsTrue(b.ballX >= b.ballSize);
            Assert.IsTrue(b.ballX <= (DApi.width - b.ballSize));
            Assert.IsTrue(b.ballY >= b.ballSize);
            Assert.IsTrue(b.ballY <= (DApi.height - b.ballSize));

            Assert.AreEqual(30, b.ballSize);
            Assert.IsTrue(b.ballWeight == b.ballSize);
            Assert.IsTrue(b.ballNewX >= -5 && b.ballNewX <= 6);
            Assert.IsTrue(b.ballNewY >= -5 && b.ballNewY <= 6);
        }

        [TestMethod]
        public void moveTest()
        {
            DApi = DataAbstractApi.createApi(800, 600);
            IBall b = DApi.createBall(1);
            double x = b.ballX;
            double y = b.ballY;
            b.ballChangeSpeed(5, 5);
            ConcurrentQueue<IBall> queue = new ConcurrentQueue<IBall>();
            b.moveBall(1, queue);
            Assert.AreNotEqual(x, b.ballX);
            Assert.AreNotEqual(y, b.ballY);
            ;
        }




    }
}