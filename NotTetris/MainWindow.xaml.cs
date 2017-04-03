using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using Microsoft.Kinect.Input;
using System.ComponentModel;
using System.Windows.Threading;
using SharpDX.XInput;


namespace NotTetris
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        /// <summary> Game Board </summary>
        private DispatcherTimer Timer;
        private Board GameBoard;
        private Label GameOverLabel;

        private Controller GP = new Controller(UserIndex.One);
        private State stateNew;
        private State stateOld;

        /// <summary> init images </summary>
        BitmapImage creeper = new BitmapImage(new Uri("pack://application:,,,/Assets/creeper.jpg"));       
        BitmapImage torch = new BitmapImage(new Uri("pack://application:,,,/Assets/torch.jpg"));      
        BitmapImage axe = new BitmapImage(new Uri("pack://application:,,,/Assets/axe.jpg"));      
        BitmapImage dirt= new BitmapImage(new Uri("pack://application:,,,/Assets/dirt.jpg"));        
        BitmapImage flower = new BitmapImage(new Uri("pack://application:,,,/Assets/flower.jpg"));       
        BitmapImage spider = new BitmapImage(new Uri("pack://application:,,,/Assets/spider.jpg"));      
        BitmapImage squid = new BitmapImage(new Uri("pack://application:,,,/Assets/squid.jpg"));


        /// <summary> Active Kinect sensor </summary>
        private KinectSensor kinectSensor = null;

        /// <summary> Array for the bodies (Kinect will track up to 6 people simultaneously) </summary>
        private Body[] bodies = null;

        /// <summary> Reader for body frames </summary>
        private BodyFrameReader bodyFrameReader = null;

        /// <summary> Current status text to display </summary>
        private string statusText = null;

        /// <summary> KinectBodyView object which handles drawing the Kinect bodies to a View box in the UI </summary>
        private BodyView kinectBodyView = null;

        /// <summary> List of gesture detectors, there will be one detector created for each potential body (max of 6) </summary>
        private List<GestureDetector> gestureDetectorList = null;

        /// <summary>
        /// Initializes a new instance of the MainWindow class
        /// </summary>
        public MainWindow()
        {
            // only one sensor is currently supported
            this.kinectSensor = KinectSensor.GetDefault();

            // set IsAvailableChanged event notifier
            this.kinectSensor.IsAvailableChanged += this.Sensor_IsAvailableChanged;

            // open the sensor
            this.kinectSensor.Open();

            // set the status text
            this.StatusText = null;

            // open the reader for the body frames
            this.bodyFrameReader = this.kinectSensor.BodyFrameSource.OpenReader();

            // set the BodyFramedArrived event notifier
            this.bodyFrameReader.FrameArrived += this.Reader_BodyFrameArrived;

            // initialize the BodyViewer object for displaying tracked bodies in the UI
            this.kinectBodyView = new BodyView(this.kinectSensor);

            // initialize the gesture detection objects for our gestures
            this.gestureDetectorList = new List<GestureDetector>();

            // initialize the MainWindow
            this.InitializeComponent();

            // set our data context objects for display in UI
            this.DataContext = this;
            this.kinectBodyViewbox.DataContext = this.kinectBodyView;

            // create a gesture detector for each body (6 bodies => 6 detectors) and create content controls to display results in the UI
            int col0Row = 0;
            int col1Row = 0;
            int maxBodies = this.kinectSensor.BodyFrameSource.BodyCount;
            for (int i = 0; i < maxBodies; ++i)
            {
                GestureResultView result = new GestureResultView(i, false, false, 0.0f, "");
                GestureDetector detector = new GestureDetector(this.kinectSensor, result);
                this.gestureDetectorList.Add(detector);

                // split gesture results across the first two columns of the content grid
                ContentControl contentControl = new ContentControl();
                contentControl.Content = this.gestureDetectorList[i].GestureResultView;

                if (i % 2 == 0)
                {
                    // Gesture results for bodies: 0, 2, 4
                    Grid.SetColumn(contentControl, 0);
                    Grid.SetRow(contentControl, col0Row);
                    ++col0Row;
                }
                else
                {
                    // Gesture results for bodies: 1, 3, 5
                    Grid.SetColumn(contentControl, 1);
                    Grid.SetRow(contentControl, col1Row);
                    ++col1Row;
                }

                this.contentGrid.Children.Add(contentControl);

            }
        }

        /// <summary>
        /// INotifyPropertyChangedPropertyChanged event to allow window controls to bind to changeable data
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the current status text to display
        /// </summary>
        public string StatusText
        {
            get
            {
                return this.statusText;
            }

            set
            {
                if (this.statusText != value)
                {
                    this.statusText = value;

                    // notify any bound elements that the text has changed
                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs("StatusText"));
                    }
                }
            }
        }

        void InitGame()
        {
            Timer = new DispatcherTimer();
            Timer.Tick += new EventHandler(GameTick);
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 400);
            GameStart();
        }

        private void GameStart()
        {
            MainGrid.Children.Clear();
            GameBoard = new Board(MainGrid);
            if (GameStatsPanel.Children.Contains(GameOverLabel))
            {
                GameStatsPanel.Children.Remove(GameOverLabel);
            }

            Timer.Start();
        }

        private void Start_BTN_Click(object sender, RoutedEventArgs e)
        {
            InitGame();
        }

        void GameTick(object sender, EventArgs e)
        {
            stateNew = GP.GetState();
            Score.Content = GameBoard.getScore().ToString("000000000");
            Lines.Content = GameBoard.getLines().ToString("000000000");
            GameBoard.CurrTetraminoMovDown();
            CheckGameState();
            foreach(var foo in gestureDetectorList)
            {
                if(foo.GestureResultView.Name != "" || foo.GestureResultView.Name == null)
                {
                    GameBoard.SpawnTetramino(foo.GestureResultView.Name);
                }
            }

            Next_LBL.Content = GameBoard.NextTetraminoName();
            
            switch(GameBoard.NextTetraminoName())
            {
                case "I":
                    NextTetraminoImage.Source = creeper;
                    break;
                case "J":
                    NextTetraminoImage.Source = torch;
                    break;
                case "L":
                    NextTetraminoImage.Source = axe;
                    break;
                case "[ ]":
                    NextTetraminoImage.Source = dirt;
                    break;
                case "S":
                    NextTetraminoImage.Source = flower;
                    break;
                case "T":
                    NextTetraminoImage.Source = spider;
                    break;
                case "Z":
                    NextTetraminoImage.Source = squid;
                    break;
            }

            CheckButtons();
        }

        private void CheckGameState()
        {
            if (GameBoard.GameOver())
            {
                GamePause();
                GameOverLabel = UiHelper.MakeLabel("Game Over", Brushes.DarkRed, Brushes.White, FontWeights.Bold, 24);
                GameStatsPanel.Children.Add(GameOverLabel);
            }
        }

        private void CheckButtons()
        {

            //a  
            if (stateOld.Gamepad.Buttons == GamepadButtonFlags.A && stateNew.Gamepad.Buttons == GamepadButtonFlags.A)
            {
                
            }

            //b 
            if (stateOld.Gamepad.Buttons == GamepadButtonFlags.B && stateNew.Gamepad.Buttons == GamepadButtonFlags.B)
            {
                GameBoard.CurrTetraminoMovRotate();
            }

            //dpad left button
            if (stateOld.Gamepad.Buttons == GamepadButtonFlags.DPadLeft && stateNew.Gamepad.Buttons == GamepadButtonFlags.DPadLeft)
            {
                GameBoard.CurrTetraminoMovLeft();
            }

            //dpad right button
            if (stateOld.Gamepad.Buttons == GamepadButtonFlags.DPadRight && stateNew.Gamepad.Buttons == GamepadButtonFlags.DPadRight)
            {
                GameBoard.CurrTetraminoMovRight();
            }
            stateOld = stateNew;
        }


        private void HandleKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {

                case Key.Left: if (Timer.IsEnabled) { GameBoard.CurrTetraminoMovLeft(); } break;
                case Key.Right: if (Timer.IsEnabled) { GameBoard.CurrTetraminoMovRight(); } break;
                case Key.Down: if (Timer.IsEnabled) { GameBoard.CurrTetraminoMovDown(); } break;
                case Key.Up: if (Timer.IsEnabled) { GameBoard.CurrTetraminoMovRotate(); } break;
                case Key.F2: GameStart(); break;
                case Key.F3:
                    if (!GameBoard.GameOver()) GamePause();
                    else GameStart();
                    break;

                default: break;
            }
        }

        private void GamePause()
        {
            if (Timer.IsEnabled) { Timer.Stop(); }
            else { Timer.Start(); }
        }

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (this.bodyFrameReader != null)
            {
                // BodyFrameReader is IDisposable
                this.bodyFrameReader.FrameArrived -= this.Reader_BodyFrameArrived;
                this.bodyFrameReader.Dispose();
                this.bodyFrameReader = null;
            }

            if (this.gestureDetectorList != null)
            {
                // The GestureDetector contains disposable members (VisualGestureBuilderFrameSource and VisualGestureBuilderFrameReader)
                foreach (GestureDetector detector in this.gestureDetectorList)
                {
                    detector.Dispose();
                }

                this.gestureDetectorList.Clear();
                this.gestureDetectorList = null;
            }

            if (this.kinectSensor != null)
            {
                this.kinectSensor.IsAvailableChanged -= this.Sensor_IsAvailableChanged;
                this.kinectSensor.Close();
                this.kinectSensor = null;
            }
        }

        /// <summary>
        /// Handles the event when the sensor becomes unavailable (e.g. paused, closed, unplugged).
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Sensor_IsAvailableChanged(object sender, IsAvailableChangedEventArgs e)
        {
           
        }

        /// <summary>
        /// Handles the body frame data arriving from the sensor and updates the associated gesture detector object for each body
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Reader_BodyFrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            bool dataReceived = false;

            using (BodyFrame bodyFrame = e.FrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {
                    if (this.bodies == null)
                    {
                        // creates an array of 6 bodies, which is the max number of bodies that Kinect can track simultaneously
                        this.bodies = new Body[bodyFrame.BodyCount];
                    }

                    // The first time GetAndRefreshBodyData is called, Kinect will allocate each Body in the array.
                    // As long as those body objects are not disposed and not set to null in the array,
                    // those body objects will be re-used.
                    bodyFrame.GetAndRefreshBodyData(this.bodies);
                    dataReceived = true;
                }
            }

            if (dataReceived)
            {
                // visualize the new body data
                this.kinectBodyView.UpdateBodyFrame(this.bodies);

                // we may have lost/acquired bodies, so update the corresponding gesture detectors
                if (this.bodies != null)
                {
                    // loop through all bodies to see if any of the gesture detectors need to be updated
                    int maxBodies = this.kinectSensor.BodyFrameSource.BodyCount;
                    for (int i = 0; i < maxBodies; ++i)
                    {
                        Body body = this.bodies[i];
                        ulong trackingId = body.TrackingId;

                        // if the current body TrackingId changed, update the corresponding gesture detector with the new value
                        if (trackingId != this.gestureDetectorList[i].TrackingId)
                        {
                            this.gestureDetectorList[i].TrackingId = trackingId;

                            // if the current body is tracked, unpause its detector to get VisualGestureBuilderFrameArrived events
                            // if the current body is not tracked, pause its detector so we don't waste resources trying to get invalid gesture results
                            this.gestureDetectorList[i].IsPaused = trackingId == 0;
                        }
                    }
                }
            }
        }
    }
}
