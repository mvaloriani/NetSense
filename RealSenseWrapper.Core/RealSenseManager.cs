using RealSenseWrapper.Core.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace RealSenseWrapper.Core
{
    public enum PrimaryUserHandState
    {
        NoHandUser,
        Proximity
    }

    public class RealSenseManager : INotifyPropertyChanged
    {

        #region INotify
        public void RaisePropertyChanged<T>(Expression<Func<T>> expression)
        {
            var property = (MemberExpression)expression.Body;
            this.RaisePropertyChanged(property.Member.Name);
        }

        public void RaisePropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion


        #region RealSenseManager Variables

        private readonly IRealSenseDataService _dataService;

        private RealSenseSensor realSenseSensor;

        /// <summary>
        /// Format we will use for the color stream
        /// </summary>
        private RealSenseColorFormat ColorFormat;

        /// <summary>
        /// Format we will use for the depth stream
        /// </summary>
        private const RealSenseDepthFormat DepthFormat = RealSenseDepthFormat.Depth640x480F30;

        /// <summary>
        /// The color bitmap taken from the RealSense camera
        /// </summary>
        private Bitmap colorBitmap;

        private HandCursorPosition handCursorPosition;

        /// <summary>
        /// Indicates whether a user Hand was tracked from the RealSense sensor in the previous frame -> start: false;
        /// </summary>
        private bool userHandPresenceAtPreviousFrame;

        #endregion RealSenseManager Variables

        #region RealSenseManager Property

        public const string ColorBitmapPropertyName = "ColorBitmap";
        public Bitmap ColorBitmap
        {
            get { return colorBitmap; }
            set
            {
                if (colorBitmap == value)
                {
                    return;
                }
                colorBitmap = value;
                RaisePropertyChanged(ColorBitmapPropertyName);
                RaisePropertyChanged(ColorBitmapSourcePropertyName);
            }
        }
        public const string ColorBitmapSourcePropertyName = "ColorBitmapSource";
        public BitmapSource ColorBitmapSource
        {
            get { return colorBitmap.ToBitmapSource(); }
           
        }


        private Bitmap segBitmap;
        public const string SegBitmapPropertyName = "SegBitmap";
        public Bitmap SegBitmap
        {
            get { return segBitmap; }
            set
            {
                if (segBitmap == value)
                {
                    return;
                }
                segBitmap = value;
                RaisePropertyChanged(SegBitmapPropertyName);
                RaisePropertyChanged(SegBitmapSourcePropertyName);

            }
        }
        public const string SegBitmapSourcePropertyName = "SegBitmapSource";
        public BitmapSource SegBitmapSource
        {
            get { return segBitmap.ToBitmapSource(); }

        }



        public const string HandCursorPositionPropertyName = "HandCursorPosition";
        public HandCursorPosition HandCursorPosition
        {
            get { return handCursorPosition; }
            set
            {
                if (handCursorPosition == value)
                {
                    return;
                }
                handCursorPosition = value;
                RaisePropertyChanged(HandCursorPositionPropertyName);
            }
        }

        #endregion RealSenseManager Property

        #region Costructor & Inizialize
        public RealSenseManager(IRealSenseDataService dataService)
        {
            _dataService = dataService;

        }

        public void Initialize()
        {
            ColorFormat = RealSenseColorFormat.Color1280x720F30;

            ColorFormat = RealSenseColorFormat.Color640x480F30;


            var sensor = new RealSenseSensor();

            sensor.InitializeColorStrem(ColorFormat);
            sensor.InitializeDepthStrem(DepthFormat);
            sensor.InitializeGestureRecognition();
            sensor.InitializeHandsStream();
            sensor.Inizialize3DSeg();

            Initialize(sensor);
        }

        public void Initialize(RealSenseSensor sensor)
        {
            this.realSenseSensor = sensor;
            this.userHandPresenceAtPreviousFrame = false;
            this.realSenseSensor.HandsFrameReady += realSenseSensor_HandsFrameReady;

            this.realSenseSensor.Start();

        }

        public void Uninitialize()
        {
            realSenseSensor.HandsFrameReady -= realSenseSensor_HandsFrameReady;

            this.realSenseSensor.Stop();
        }
        
        #endregion

        #region Public Methods

        /// <summary>
        /// Enable color stream and add handlers
        /// </summary>
        public void EnableColorStream()
        {
            try
            {
                this.realSenseSensor.ColorFrameReady += realSenseSensor_ColorFrameReady;
                //    this.realSenseSensor.EnableColorStream();
            }
            catch (Exception e)
            {
                Console.WriteLine("-----------------------------------------------------------------");
                Console.WriteLine("Error in RealSenseManager.EnableColorStream(): " + e.Message);
                Console.WriteLine("");
            }
        }

        /// <summary>
        /// Remove handlers and disable color stream
        /// </summary>
        public void DisableColorStream()
        {
            try
            {
                //disable 'ColorStream' and remove Handlers to Color Streams
                this.ColorBitmap.Dispose();
                this.realSenseSensor.ColorFrameReady -= realSenseSensor_ColorFrameReady;
                //this.realSenseSensor.DisableColorStream();
            }
            catch (Exception e)
            {
                Console.WriteLine("-----------------------------------------------------------------");
                Console.WriteLine("Error in RealSenseManager.DisableColorStream(): " + e.Message);
                Console.WriteLine("");
            }
        }

        /// <summary>
        /// Add handler to Hands event
        /// </summary>
        public void EnableHandsStream()
        {
            this.realSenseSensor.HandsFrameReady += realSenseSensor_HandsFrameReady;
        }

        /// <summary>
        /// Remove handler to Hands event
        /// </summary>
        public void DisableHandsStream()
        {
            this.realSenseSensor.HandsFrameReady -= realSenseSensor_HandsFrameReady;
        }
        /// <summary>
        /// Add handler to GestureRecognized event
        /// </summary>
        public void EnableGestureStream()
        {
            realSenseSensor.GestureRecognized += realSenseSensor_GestureRecognized;
        }

        /// <summary>
        /// Remove handler to GestureRecognized event
        /// </summary>
        public void DisableGestureStream()
        {
            realSenseSensor.GestureRecognized -= realSenseSensor_GestureRecognized;
        }

        public void EnableSegmentedStream()
        {
            try
            {
                // this.realSenseSensor.Enable3DSeg();
                this.realSenseSensor.SegmentedFrameReady += realSenseSensor_SegmentedFrameReady;
            }
            catch (Exception e)
            {
                Console.WriteLine("-----------------------------------------------------------------");
                Console.WriteLine("Error in RealSenseManager.EnableSegmentedStream(): " + e.Message);
                Console.WriteLine("");
            }
        }

        public void DisableSegmentedStream()
        {
            try
            {

                this.SegBitmap.Dispose();
                //   this.realSenseSensor.Disable3DSeg();
                this.realSenseSensor.SegmentedFrameReady -= realSenseSensor_SegmentedFrameReady;
            }
            catch (Exception e)
            {
                Console.WriteLine("-----------------------------------------------------------------");
                Console.WriteLine("Error in RealSenseManager.DisableSegmentedStream(): " + e.Message);
                Console.WriteLine("");
            }
        }




        /// <summary>
        /// Take photo ID of facing users
        /// </summary>
        /// <returns>the photo ID of users, stored in Folder</returns>
        public string takeUsersPhotoID()
        {
            string result = "";

            BitmapSource faceImageSource = ColorBitmap.ToBitmapSource();

            string fileName = DateTime.Now.Minute + ".jpg";

            //Store user image in folder. In case of errors do not save image into list
            _dataService.StoreUserImage((item, error) =>
            {
                if (error != null)
                {
                    // if there is an error should create a property and bind to it for better practices
                    System.Diagnostics.Debug.WriteLine(error.ToString());
                }
                else
                {
                    result = fileName;
                }
            }, faceImageSource, fileName);

            return result;
        }

        #endregion Public Methods


        #region RealSense Events

        public delegate void PrimaryUserHandHandler(object sender, PrimaryUserHandEventArgs e);
        public event PrimaryUserHandHandler PrimaryUserHandRaiseEvent;

        public delegate void GeneralGestureRecognizedHandler(object sender, GestureEventArgs e);
        public event GeneralGestureRecognizedHandler GeneralGestureRecognizedRaiseEvent;

        #endregion RealSense Events


        #region Handlers

        void realSenseSensor_HandsFrameReady(object sender, RealSenseHandsEventArgs e)
        {
            if (e.TotalHands != null && e.TotalHands.Count() != 0)
            {

                foreach (Dictionary<PXCMHandData.JointType, PXCMHandData.JointData> hand in e.Hands)
                {
                    if (hand != null)
                    {
                        PXCMHandData.JointData value;
                        bool hasValue = hand.TryGetValue(PXCMHandData.JointType.JOINT_CENTER, out value);
                        if (hasValue)
                        {
                            HandCursorPosition = new HandCursorPosition(640 - value.positionImage.x, value.positionImage.y);
                            break;
                        }
                    }
                }

                if (!this.userHandPresenceAtPreviousFrame)
                {
                    this.userHandPresenceAtPreviousFrame = true;
                    if (PrimaryUserHandRaiseEvent != null) PrimaryUserHandRaiseEvent(this, new PrimaryUserHandEventArgs(0, PrimaryUserHandState.Proximity));
                }
                else
                {
                    //Raise event No User Hand in front of the RealSense sensor
                    this.userHandPresenceAtPreviousFrame = false;
                    if (PrimaryUserHandRaiseEvent != null) PrimaryUserHandRaiseEvent(this, new PrimaryUserHandEventArgs(0, PrimaryUserHandState.NoHandUser));
                }
            }
        }

        void realSenseSensor_ColorFrameReady(object sender, RealSenseEventArgs e)
        {
            ColorBitmap = e.Source.ToBitmap(0, e.Info.width, e.Info.height);
        }

        void realSenseSensor_SegmentedFrameReady(object sender, RealSenseEventArgs e)
        {
            using (System.Drawing.Bitmap source = e.Source.ToBitmap(0, e.Info.width, e.Info.height))
            {
                try { source.MakeTransparent(System.Drawing.Color.White);

                    this.SegBitmap = source;
                    // IntPtr ptr = source.GetHbitmap();

                    //this.SegBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ptr, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                    // //release the HBitmap
                    // DeleteObject(ptr);
                    source.Dispose();
                }
                catch(Exception ex) { }
                GC.Collect();

            }


        }


        void realSenseSensor_GestureRecognized(object sender, RealSenseGestureEventArgs e)
        {
            if (GeneralGestureRecognizedRaiseEvent != null) GeneralGestureRecognizedRaiseEvent(this, new GestureEventArgs(e.GestureData.name, e.BodySide));
        }

        #endregion Handlers
    }






    /// <summary>
    /// Used to return a variable value for the Primary User Hand State
    /// </summary>
    public class PrimaryUserHandEventArgs : System.EventArgs
    {
        public string text { get { return user_state.ToString(); } }
        public int user_id;
        public PrimaryUserHandState user_state;
        public PrimaryUserHandEventArgs(int user_id, PrimaryUserHandState user_state)
        {
            this.user_id = user_id;
            this.user_state = user_state;
        }
    }

    public class GestureEventArgs : System.EventArgs
    {
        public String gestureName;
        public String bodySideType;
        public GestureEventArgs(String gestureName)
            : base()
        {
            this.gestureName = gestureName;
        }

        public GestureEventArgs(String gestureName, PXCMHandData.BodySideType bodySideType)
            : base()
        {
            this.gestureName = gestureName;
            this.bodySideType = bodySideType == PXCMHandData.BodySideType.BODY_SIDE_LEFT ? "left" : "right";
        }
    }

 
}
