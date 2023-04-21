using Logic;
using System.Collections;



namespace Model
{
    public abstract class ModelAbstractApi
    {
        public abstract int width { get; }
        public abstract int height { get; }
        public abstract void startMoving();
        public abstract IList create(int ballVal);
        public abstract IList delete(int ballVal);
        public abstract void stop();
        public static ModelAbstractApi createApi(int Weight, int Height)
        {
            return new ModelApi(Weight, Height);
        }
    }

    internal class ModelApi : ModelAbstractApi
    {
        public override int width { get; }
        public override int height { get; }
        private readonly LogicAbstractApi logicLayer;
        public ModelApi(int width, int height)
        {
            this.width = width;
            this.height = height;
            logicLayer = LogicAbstractApi.createApi(this.width, this.height);
        }
        public override void startMoving()
        {
            logicLayer.start();
        }
        public override void stop()
        {
            logicLayer.stop();
        }
        public override IList create(int ballVal) => logicLayer.createBalls(ballVal);
        public override IList delete(int ballVal) => logicLayer.deleteBalls(ballVal);
    }
}
