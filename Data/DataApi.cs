using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Data
{
    public abstract class DataAbstractApi
    {
        public abstract IBall createBall(int count);
        public abstract int width { get; }
        public abstract int height { get; }
        public abstract void stopLoggingTask();
        public abstract Task createLoggingTask(ConcurrentQueue<IBall> logQueue);
        public abstract void appendObjectToJsonFile(string filename, string newJsonObject);
        public static DataAbstractApi createApi(int width, int height)
        {
            return new DataApi(width, height);
        }
    }

    internal class DataApi : DataAbstractApi
    {
        private readonly Random random = new Random();
        private readonly Stopwatch stopwatch;
        private readonly string logPath = "Log.json";
        private bool newSession;
        private bool stop;
        public override int width { get; }
        public override int height { get; }
        public DataApi(int width, int height)
        {
            this.width = width;
            this.height = height;
            newSession = true;
            stopwatch = new Stopwatch();
        }
        public override IBall createBall(int count)
        {
            int radius = 30;
            double weight = radius;
            double x = random.Next(radius + 20, width - radius - 20);
            double y = random.Next(radius + 20, height - radius - 20);
            double newX = 0;
            double newY = 0;
            while (newX == 0)
            {
                newX = random.Next(-5, 5) + random.NextDouble();
            }
            while (newY == 0)
            {
                newY = random.Next(-5, 5) + random.NextDouble();
            }
            Ball ball = new Ball(count, radius, x, y, newX, newY, weight);
            return ball;
        }
        public override void stopLoggingTask()
        {
            stop = true;
        }
        public override Task createLoggingTask(ConcurrentQueue<IBall> logQueue)
        {
            stop = false;
            return callLogger(logQueue);
        }
        internal void FileMaker(string filename)
        {
            if (File.Exists(filename) && newSession)
            {
                newSession = false;
                File.Delete(filename);
            }
        }
        public override void appendObjectToJsonFile(string filename, string newJsonObject)
        {
            using (StreamWriter sw = new StreamWriter(filename, true))
            {
                sw.WriteLine("[]");
            }
            string content;
            using (StreamReader sr = File.OpenText(filename))
            {
                content = sr.ReadToEnd();
            }
            content = content.TrimEnd();
            content = content.Remove(content.Length - 1, 1);
            if (content.Length == 1)
            {
                content = String.Format("{0}\n{1}\n]\n", content.Trim(), newJsonObject);
            }
            else
            {
                content = String.Format("{0},\n{1}\n]\n", content.Trim(), newJsonObject);
            }
            using (StreamWriter sw = File.CreateText(filename))
            {
                sw.Write(content);
            }
        }
        internal async Task callLogger(ConcurrentQueue<IBall> logQueue)
        {
            FileMaker(logPath);
            string diagnostics;
            string date;
            string log;
            while (!stop)
            {
                stopwatch.Reset();
                stopwatch.Start();
                logQueue.TryDequeue(out IBall logObject);
                if (logObject != null)
                {
                    diagnostics = JsonSerializer.Serialize(logObject);
                    date = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff");
                    log = "{" + String.Format("\n\t\"Date\": \"{0}\",\n\t\"Info\":{1}\n", date, diagnostics) + "}";
                    lock (this)
                    {
                        File.AppendAllText(logPath, log);
                    }
                }
                else
                {
                    return;
                }
                stopwatch.Stop();
                await Task.Delay((int)(stopwatch.ElapsedMilliseconds));
            }
        }
    }
}
