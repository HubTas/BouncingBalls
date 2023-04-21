using Data;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Logic
{
    public abstract class LogicAbstractApi
    {
        public abstract IList createBalls(int count);
        public abstract IList deleteBalls(int count);
        public abstract void start();
        public abstract void stop();
        public abstract int width { get; }
        public abstract int height { get; }
        public static LogicAbstractApi createApi(int width, int height)
        {
            return new LogicApi(width, height);
        }
    }
    internal class LogicApi : LogicAbstractApi
    {
        private readonly DataAbstractApi dataLayer;
        private ObservableCollection<IBall> balls { get; }
        private readonly ConcurrentQueue<IBall> queue;
        public LogicApi(int width, int height)
        {
            dataLayer = DataAbstractApi.createApi(width, height);
            this.width = width;
            this.height = height;
            balls = new ObservableCollection<IBall>();
            queue = new ConcurrentQueue<IBall>();
        }
        public override int width { get; }
        public override int height { get; }
        public override void start()
        {
            for (int i = 0; i < balls.Count; i++)
            {
                balls[i].PropertyChanged += ballPositionChanged;
                balls[i].ballCreateMovementTask(30, queue);
            }
            dataLayer.createLoggingTask(queue);
        }
        public override void stop()
        {
            for (int i = 0; i < balls.Count; i++)
            {
                balls[i].stopBall();
                balls[i].PropertyChanged -= ballPositionChanged;
            }
        }
        public override IList createBalls(int count)
        {
            int liczba = balls.Count;
            for (int i = liczba; i < liczba + count; i++)
            {
                bool contain = true;
                bool licz;
                while (contain)
                {
                    balls.Add(dataLayer.createBall(i + 1));
                    licz = false;
                    for (int j = 0; j < i; j++)
                    {
                        if (balls[i].ballX <= balls[j].ballX + balls[j].ballSize && balls[i].ballX + balls[i].ballSize >= balls[j].ballX)
                        {
                            if (balls[i].ballY <= balls[j].ballY + balls[j].ballSize && balls[i].ballY + balls[i].ballSize >= balls[j].ballY)
                            {
                                licz = true;
                                balls.Remove(balls[i]);
                                break;
                            }
                        }
                    }
                    if (!licz)
                    {
                        contain = false;
                    }
                }
            }
            return balls;
        }
        public override IList deleteBalls(int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (balls.Count > 0)
                {
                    balls.Remove(balls[balls.Count - 1]);
                };
            }
            return balls;
        }
        internal void wallCollision(IBall ball)
        {
            double diameter = ball.ballSize;
            double right = width - diameter;
            double down = height - diameter;
            if (ball.ballX <= 5)
            {
                if (ball.ballNewX <= 0)
                {
                    ball.ballChangeSpeed(-ball.ballNewX, ball.ballNewY);
                }
            }
            else if (ball.ballX >= right - 5)
            {
                if (ball.ballNewX > 0)
                {
                    ball.ballChangeSpeed(-ball.ballNewX, ball.ballNewY);
                }
            }
            if (ball.ballY <= 5)
            {
                if (ball.ballNewY <= 0)
                {
                    ball.ballChangeSpeed(ball.ballNewX, -ball.ballNewY);
                }
            }
            else if (ball.ballY >= down - 5)
            {
                if (ball.ballNewY > 0)
                {
                    ball.ballChangeSpeed(ball.ballNewX, -ball.ballNewY);
                }
            }
        }
        internal void ballBounce(IBall ball)
        {
            lock (ball)
            {
                for (int i = 0; i < balls.Count; i++)
                {
                    IBall secondBall = balls[i];
                    if (ball.ballID == secondBall.ballID)
                    {
                        continue;
                    }
                    lock (secondBall)
                    {
                        if (collision(ball, secondBall))
                        {
                            double relativeX = ball.ballX - secondBall.ballX;
                            double relativeY = ball.ballY - secondBall.ballY;
                            double relativeNewX = ball.ballNewX - secondBall.ballNewX;
                            double relativeNewY = ball.ballNewY - secondBall.ballNewY;
                            if (relativeX * relativeNewX + relativeY * relativeNewY > 0)
                            {
                                return;
                            }
                            double u1x;
                            double u1y;
                            double m1 = ball.ballWeight;
                            double v1x = ball.ballNewX;
                            double v1y = ball.ballNewY;

                            double m2 = secondBall.ballWeight;
                            double v2x = secondBall.ballNewX;
                            double v2y = secondBall.ballNewY;

                            u1x = (m1 - m2) * v1x / (m1 + m2) + (2 * m2) * v2x / (m1 + m2);
                            u1y = (m1 - m2) * v1y / (m1 + m2) + (2 * m2) * v2y / (m1 + m2);

                            double u2x = 2 * m1 * v1x / (m1 + m2) + (m2 - m1) * v2x / (m1 + m2);
                            double u2y = 2 * m1 * v1y / (m1 + m2) + (m2 - m1) * v2y / (m1 + m2);

                            secondBall.ballChangeSpeed(u2x, u2y);
                            ball.ballChangeSpeed(u1x, u1y);
                        }
                    }
                }
            }
            return;
        }
        internal bool collision(IBall a, IBall b)
        {
            if (a == null || b == null)
            {
                return false;
            }
            return distance(a, b) <= (a.ballSize / 2 + b.ballSize / 2);
        }
        internal double distance(IBall a, IBall b)
        {
            double x1 = a.ballX + a.ballSize / 2;
            double y1 = a.ballY + a.ballSize / 2;
            double x2 = b.ballX + b.ballSize / 2;
            double y2 = b.ballY + b.ballSize / 2;
            return Math.Sqrt((Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2)));
        }
        internal void ballPositionChanged(object sender, PropertyChangedEventArgs args)
        {
            IBall ball = (IBall)sender;
            wallCollision(ball);
            ballBounce(ball);
        }
    }
}
