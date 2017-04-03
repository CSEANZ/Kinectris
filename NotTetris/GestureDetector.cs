using System;
using System.Collections.Generic;
using Microsoft.Kinect;
using Microsoft.Kinect.VisualGestureBuilder;


namespace NotTetris
{
    public class GestureDetector : IDisposable
    {
        //public RoutedEventHandler GestureRecognized { get; set; }

        //Path to the gesture database
        private readonly string gestureDatabase = @"Database\NotTetris.gbd";



        private VisualGestureBuilderFrameSource vgbFrameSource = null;
        private VisualGestureBuilderFrameReader vgbFrameReader = null;

        public GestureDetector(KinectSensor kinectSensor, GestureResultView gestureResultView)
        {
            if (kinectSensor == null)
            {
                throw new ArgumentNullException("Kinect");
            }

            if (gestureResultView == null)
            {
                throw new ArgumentNullException("gestureResultView null");
            }

            this.GestureResultView = gestureResultView;

            this.vgbFrameSource = new VisualGestureBuilderFrameSource(kinectSensor, 0);
            this.vgbFrameSource.TrackingIdLost += this.Source_TrackingIdLost;

            // open the reader for the vgb frames
            this.vgbFrameReader = this.vgbFrameSource.OpenReader();
            if (this.vgbFrameReader != null)
            {
                this.vgbFrameReader.IsPaused = true;
                this.vgbFrameReader.FrameArrived += this.Reader_GestureFrameArrived;
            }

            // load the gestures from database
            using (VisualGestureBuilderDatabase database = new VisualGestureBuilderDatabase(this.gestureDatabase))
            {
                vgbFrameSource.AddGestures(database.AvailableGestures);
            }
        }

        /// <summary> Gets the GestureResultView object which stores the detector results for display in the UI </summary>
        public GestureResultView GestureResultView { get; private set; }

        /// <summary>
        /// Gets or sets the body tracking ID associated with the current detector
        /// The tracking ID can change whenever a body comes in/out of scope
        /// </summary>
        public ulong TrackingId
        {
            get
            {
                return this.vgbFrameSource.TrackingId;
            }

            set
            {
                if (this.vgbFrameSource.TrackingId != value)
                {
                    this.vgbFrameSource.TrackingId = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the detector is currently paused
        /// If the body tracking ID associated with the detector is not valid, then the detector should be paused
        /// </summary>
        public bool IsPaused
        {
            get
            {
                return this.vgbFrameReader.IsPaused;
            }

            set
            {
                if (this.vgbFrameReader.IsPaused != value)
                {
                    this.vgbFrameReader.IsPaused = value;
                }
            }
        }

        public bool GestureRecognized
        {
            get
            {
                return this.GestureRecognized;
            }

            set
            {
                if (this.GestureRecognized != value)
                {
                    this.GestureRecognized = value;
                }
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.vgbFrameReader != null)
                {
                    this.vgbFrameReader.FrameArrived -= this.Reader_GestureFrameArrived;
                    this.vgbFrameReader.Dispose();
                    this.vgbFrameReader = null;
                }

                if (this.vgbFrameSource != null)
                {
                    this.vgbFrameSource.TrackingIdLost -= this.Source_TrackingIdLost;
                    this.vgbFrameSource.Dispose();
                    this.vgbFrameSource = null;
                }
            }
        }

        /// <summary>
        /// Handles gesture detection results arriving from the sensor for the associated body tracking Id
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Reader_GestureFrameArrived(object sender, VisualGestureBuilderFrameArrivedEventArgs e)
        {
            VisualGestureBuilderFrameReference frameReference = e.FrameReference;
            using (VisualGestureBuilderFrame frame = frameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    IReadOnlyDictionary<Gesture, DiscreteGestureResult> discreteResults = frame.DiscreteGestureResults;

                    if (discreteResults != null)
                    {
                        foreach (Gesture gesture in this.vgbFrameSource.Gestures)
                        {

                            switch (gesture.Name)
                            {


                                case "L":
                                    {
                                        DiscreteGestureResult result = null;
                                        discreteResults.TryGetValue(gesture, out result);

                                        if (result != null)
                                        {
                                            // update the GestureResultView object with new gesture result values
                                            this.GestureResultView.UpdateGestureResult(true, result.Detected, result.Confidence, "L");
                                        }
                                        break;
                                    }

                                case "Tee":
                                    {
                                        DiscreteGestureResult result = null;
                                        discreteResults.TryGetValue(gesture, out result);

                                        if (result != null)
                                        {
                                            // update the GestureResultView object with new gesture result values
                                            this.GestureResultView.UpdateGestureResult(true, result.Detected, result.Confidence, "Tee");
                                        }
                                        break;
                                    }

                                case "Line":
                                    {
                                        DiscreteGestureResult result = null;
                                        discreteResults.TryGetValue(gesture, out result);

                                        if (result != null)
                                        {
                                            // update the GestureResultView object with new gesture result values
                                            this.GestureResultView.UpdateGestureResult(true, result.Detected, result.Confidence, "Line");
                                        }
                                        break;
                                    }

                                case "Z":
                                    {
                                        DiscreteGestureResult result = null;
                                        discreteResults.TryGetValue(gesture, out result);

                                        if (result != null)
                                        {
                                            // update the GestureResultView object with new gesture result values
                                            this.GestureResultView.UpdateGestureResult(true, result.Detected, result.Confidence, "Z");
                                        }
                                        break;
                                    }

                                case "Square":
                                    {
                                        DiscreteGestureResult result = null;
                                        discreteResults.TryGetValue(gesture, out result);

                                        if (result != null)
                                        {
                                            // update the GestureResultView object with new gesture result values
                                            this.GestureResultView.UpdateGestureResult(true, result.Detected, result.Confidence, "Square");
                                        }
                                        break;
                                    }
                            }

                        }

                    }
                }
            }
        }

        private void Source_TrackingIdLost(object sender, TrackingIdLostEventArgs e)
        {
            this.GestureResultView.UpdateGestureResult(false, false, 0.0f, "");
        }
    }

}
