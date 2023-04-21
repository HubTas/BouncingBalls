using Model;
using System.Collections;
using System.Windows.Input;

namespace ViewModel
{

    public class MainViewModel : ViewModel
    {
        private readonly ModelAbstractApi modelLayer;
        private int ballValue = 1;
        private int width;
        private int height;
        private bool stop = false;
        private bool start = false;
        private bool add = true;
        private bool delete = false;
        private int size = 0;
        private IList balls;
        public ICommand addCommand { get; }
        public ICommand runCommand { get; }
        public ICommand stopCommand
        { get; }
        public ICommand deleteCommand { get; }
        public MainViewModel()
        {
            width = 600;
            height = 480;
            modelLayer = ModelAbstractApi.createApi(width, height);
            stopCommand = new RelayCommand(Stop);
            addCommand = new RelayCommand(AddBalls);
            runCommand = new RelayCommand(Start);
            deleteCommand = new RelayCommand(DeleteBalls);

        }
        public bool isStopEnabled
        {
            get { return stop; }
            set
            {
                stop = value;
                RaisePropertyChanged();
            }
        }
        public bool isRunEnabled
        {
            get { return start; }
            set
            {
                start = value;
                RaisePropertyChanged();
            }
        }
        public bool isAddEnabled
        {
            get
            {
                return add;
            }
            set
            {
                add = value;
                RaisePropertyChanged();
            }
        }
        public bool isDeleteEnabled
        {
            get
            {
                return delete;
            }
            set
            {
                delete = value;
                RaisePropertyChanged();
            }
        }
        public int ballAmount
        {
            get
            {
                return ballValue;
            }
            set
            {
                ballValue = value;
                RaisePropertyChanged();
            }
        }
        public int Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
                RaisePropertyChanged();
            }
        }
        public int Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
                RaisePropertyChanged();
            }
        }
        private void AddBalls()
        {
            size += ballAmount;
            if (size > 0 && size <= 25)
            {
                isRunEnabled = true;
                isDeleteEnabled = true;
                Balls = modelLayer.create(ballAmount);
                ballAmount = 1;
            }
            if (size <= 0)
            {
                size = 0;
                isRunEnabled = false;
                isDeleteEnabled = false;
                ballAmount = 1;
            }
            if (size == 25)
            {
                isAddEnabled = false;
                ballAmount = 1;
            }
            if (size > 25)
            {
                size -= ballAmount;
                ballAmount = 25 - size;
            }
        }
        private void DeleteBalls()
        {
            size -= ballAmount;
            Balls = modelLayer.delete(ballAmount);
            if (size >= 0 && size <= 25)
            {
                isRunEnabled = true;
                isAddEnabled = true;
            }
            if (size <= 0)
            {
                size = 0;
                isAddEnabled = true;
                isRunEnabled = false;
                isDeleteEnabled = false;
            }
            ballAmount = 1;
        }
        private void Stop()
        {
            isStopEnabled = false;
            isAddEnabled = true;
            isRunEnabled = true;
            isDeleteEnabled = true;
            modelLayer.stop();
        }
        private void Start()
        {
            isStopEnabled = true;
            isRunEnabled = false;
            isAddEnabled = false;
            isDeleteEnabled = false;
            modelLayer.startMoving();
        }
        public IList Balls
        {
            get => balls;
            set
            {
                if (value.Equals(balls))
                {
                    return;
                }
                balls = value;
                RaisePropertyChanged();
            }
        }
    }
}
