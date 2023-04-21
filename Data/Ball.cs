using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Data
{
    public interface IBall : INotifyPropertyChanged
    {
        int ballID { get; }
        int ballSize { get; }
        double ballWeight { get; }
        double ballX { get; }
        double ballY { get; }
        double ballNewX { get; }
        double ballNewY { get; }
        void ballChangeSpeed(double Vx, double Vy);
        void moveBall(double time, ConcurrentQueue<IBall> queue);
        Task ballCreateMovementTask(int interval, ConcurrentQueue<IBall> queue);
        void saveBall(ConcurrentQueue<IBall> queue);
        void stopBall();
    }

    internal class Ball : IBall
    {
        private readonly int size;
        private readonly int id;
        private double x;
        private double y;
        private double newX;
        private double newY;
        private readonly double weight;
        private readonly Stopwatch stopwatch;
        private bool stop;
        private readonly object locker = new object();
        public Ball(int idBall, int size, double x, double y, double newX, double newY, double weight)
        {
            id = idBall;
            this.size = size;
            this.x = x;
            this.y = y;
            this.newX = newX;
            this.newY = newY;
            this.weight = weight;
            stop = false;
            stopwatch = new Stopwatch();
        }
        public int ballID { get => id; }
        public int ballSize { get => size; }
        public double ballWeight { get => weight; }
        public void ballChangeSpeed(double Vx, double Vy)
        {
            lock (locker)
            {
                ballNewX = Vx;
                ballNewY = Vy;
            }
        }
        public double ballNewX
        {
            get
            {
                lock (locker) { return newX; }
            }
            private set
            {
                if (value.Equals(newX))
                {
                    return;
                }
                newX = value;
            }
        }
        public double ballNewY
        {
            get
            {
                lock (locker) { return newY; }
            }
            private set
            {
                if (value.Equals(newY))
                {
                    return;
                }
                newY = value;
            }
        }
        public double ballX
        {
            get
            {
                lock (locker) { return x; }
            }
            private set
            {
                if (value.Equals(x))
                {
                    return;
                }
                x = value;
            }
        }
        public double ballY
        {
            get
            {
                lock (locker) { return y; }
            }
            private set
            {
                if (value.Equals(y))
                {
                    return;
                }
                y = value;
            }
        }
        public void saveBall(ConcurrentQueue<IBall> queue)
        {
            queue.Enqueue(new Ball(ballID, ballSize, ballX, ballY, ballNewX, ballNewY, ballWeight));
        }
        public void moveBall(double time, ConcurrentQueue<IBall> queue)
        {
            lock (locker)
            {
                ballX += ballNewX * time;
                ballY += ballNewY * time;
                RaisePropertyChanged(nameof(ballX));
                RaisePropertyChanged(nameof(ballY));
                saveBall(queue);
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        internal void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public Task ballCreateMovementTask(int interval, ConcurrentQueue<IBall> queue)
        {
            stop = false;
            return Run(interval, queue);
        }
        private async Task Run(int interval, ConcurrentQueue<IBall> queue)
        {
            while (!stop)
            {
                stopwatch.Reset();
                stopwatch.Start();
                if (!stop)
                {
                    moveBall(((interval - stopwatch.ElapsedMilliseconds) / 16), queue);
                }
                stopwatch.Stop();

                await Task.Delay((int)(interval - stopwatch.ElapsedMilliseconds));
            }
        }
        public void stopBall()
        {
            stop = true;
        }
    }
}

