using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace RealSenseWrapper.Core
{
    public class RealSenseSensor
    {
        #region RealSenseVariables

        /// <summary>
        /// The RealSense session
        /// </summary>
        private PXCMSession session;

        /// <summary>
        /// The RealSense sensor manager
        /// </summary>
        private PXCMSenseManager manager;

        private PXCMHandModule handAnalyzer;
        private PXCMHandConfiguration config;
        private PXCMHandData outputData;

        private string[] EmotionLabels = { "ANGER", "CONTEMPT", "DISGUST", "FEAR", "JOY", "SADNESS", "SURPRISE" };
        private string[] SentimentLabels = { "NEGATIVE", "POSITIVE", "NEUTRAL" };

        public int NUM_EMOTIONS = 10;
        public int NUM_PRIMARY_EMOTIONS = 7;

        #endregion RealSenseVariables

        #region streamTask

        private Task streamTask;
        private CancellationTokenSource streamCancellationTokenSource;
        private CancellationToken streamCancellationToken;

        private bool isColorStreamEnabled;
        private bool isDepthStreamEnabled;
        private bool isGestureRecognitionEnabled;
        private bool isHandsStreamEnabled;
        private bool is3DSegEnabled;
        private bool isEmotionEnabled;

        #endregion streamTask



        private SynchronizationContext parentContext;

        public RealSenseSensor()
        {
            session = PXCMSession.CreateInstance();
            
            this.manager = session.CreateSenseManager(); ;

            this.isColorStreamEnabled = false;
            this.isDepthStreamEnabled = false;
            this.isGestureRecognitionEnabled = false;
            this.isHandsStreamEnabled = false;
            this.is3DSegEnabled = false;
            this.isEmotionEnabled = false;

            parentContext = SynchronizationContext.Current;
        }

        /// <summary>
        /// Start the enabled frame-acquisition processes
        /// </summary>
        public void Start()
        {
            streamCancellationTokenSource = new CancellationTokenSource();
            streamCancellationToken = streamCancellationTokenSource.Token;

            if (streamTask != null && streamTask.Status == TaskStatus.Running)
            {
                throw new RealSenseException("Sensor already started");
            }
            if (streamTask != null && streamTask.Status == TaskStatus.RanToCompletion)
            {
                streamTask.Dispose();
            }


            //Thread thread = new Thread(() =>
            //{
            //    AcquireFrames();
            //});

            //thread.SetApartmentState(ApartmentState.STA);
            //thread.Start();


            

            streamTask = new Task(new Action(AcquireFrames), streamCancellationToken);

            streamTask.Start();

            Console.WriteLine("??? ID: " + System.Threading.Thread.CurrentThread.ManagedThreadId);

            //streamTask = Task.Factory.StartNew(new Action(AcquireFrames), streamCancellationToken);
        }

        /// <summary>
        /// Stop the enabled frame-acquisition processes
        /// </summary>
        public void Stop()
        {
            if (streamTask != null && streamTask.Status == TaskStatus.Running)
            {
                streamCancellationTokenSource.Cancel();
            }
        }

        #region ColorStream

        /// <summary>
        /// Enable the RealSense sensor color stream with the specified color format
        /// </summary>
        /// <param name="format">The color format to enable</param>
        public void InitializeColorStrem(RealSenseColorFormat format)
        {
            switch (format)
            {
                case RealSenseColorFormat.Color320x240F30:
                    manager.EnableStream(PXCMCapture.StreamType.STREAM_TYPE_COLOR, 320, 240, 30);
                    break;
                case RealSenseColorFormat.Color640x360F30:
                    manager.EnableStream(PXCMCapture.StreamType.STREAM_TYPE_COLOR, 640, 360, 30);
                    break;
                case RealSenseColorFormat.Color640x480F30:
                    manager.EnableStream(PXCMCapture.StreamType.STREAM_TYPE_COLOR, 640, 480, 30);
                    break;
                case RealSenseColorFormat.Color960x540F30:
                    manager.EnableStream(PXCMCapture.StreamType.STREAM_TYPE_COLOR, 960, 540, 30);
                    break;
                case RealSenseColorFormat.Color1280x720F30:
                    manager.EnableStream(PXCMCapture.StreamType.STREAM_TYPE_COLOR, 1280, 720, 30);
                    break;
                default:
                    break;
            }

            var re = manager.Init();

            if (re != pxcmStatus.PXCM_STATUS_NO_ERROR)
                throw new RealSenseException("error: " + re, re);

            this.isColorStreamEnabled = true;
        }

        public void EnableColorStream()
        {
            if (streamTask == null || streamTask.Status != TaskStatus.Running)
            {
                throw new RealSenseException("Sensor not started");
            }
            else
            {
                this.isColorStreamEnabled = true;
            }
        }

        public void DisableColorStream()
        {
            if (streamTask == null || streamTask.Status != TaskStatus.Running)
            {
                throw new RealSenseException("Sensor not started");
            }
            else
            {
                this.isColorStreamEnabled = false;
            }
        }

        #endregion ColorStream

        #region DepthStream

        /// <summary>
        /// Enable the RealSense sensor depth stream with the specified depth format
        /// </summary>
        /// <param name="format">The depth format to enable</param>
        public void InitializeDepthStrem(RealSenseDepthFormat format)
        {
            switch (format)
            {
                case RealSenseDepthFormat.Depth640x480F30:
                    manager.EnableStream(PXCMCapture.StreamType.STREAM_TYPE_DEPTH, 640, 480, 30);
                    break;
                default:
                    break;
            }

            var re = manager.Init();

            if (re != pxcmStatus.PXCM_STATUS_NO_ERROR)
                throw new RealSenseException("error: " + re, re);

            this.isDepthStreamEnabled = true;

        }

        public void EnableDepthStream()
        {
            if (streamTask == null || streamTask.Status != TaskStatus.Running)
            {
                throw new RealSenseException("Sensor not started");
            }
            else
            {
                this.isDepthStreamEnabled = true;
            }
        }

        public void DisableDepthStream()
        {
            if (streamTask == null || streamTask.Status != TaskStatus.Running)
            {
                throw new RealSenseException("Sensor not started");
            }
            else
            {
                this.isDepthStreamEnabled = false;
            }
        }

        #endregion DepthStream

        #region Gestures

        public void InitializeGestureRecognition(List<String> gestures = null)
        {
            manager.EnableStream(PXCMCapture.StreamType.STREAM_TYPE_DEPTH, 640, 480, 30);
            var s = manager.EnableHand();

            this.handAnalyzer = manager.QueryHand();


            var re = manager.Init();


            this.outputData = handAnalyzer.CreateOutput();
            this.config = handAnalyzer.CreateActiveConfiguration();

            if (gestures == null)
                this.config.EnableAllGestures();

            else
                foreach (var g in gestures) this.config.EnableGesture(g);
                    

            this.config.ApplyChanges();


            if (re != pxcmStatus.PXCM_STATUS_NO_ERROR)
                throw new RealSenseException("error: " + re, re);

            this.isGestureRecognitionEnabled = true;

        }

        public void EnableGestureRecognition()
        {
            if (streamTask == null || streamTask.Status != TaskStatus.Running)
            {
                throw new RealSenseException("Sensor not started");
            }
            else
            {
                this.isGestureRecognitionEnabled = true;
            }
        }

        public void DisableGestureRecognition()
        {
            if (streamTask == null || streamTask.Status != TaskStatus.Running)
            {
                throw new RealSenseException("Sensor not started");
            }
            else
            {
                this.isGestureRecognitionEnabled = false;
            }
        }

        #endregion Gestures

        #region Hands

        public void InitializeHandsStream()
        {
            manager.EnableStream(PXCMCapture.StreamType.STREAM_TYPE_DEPTH, 640, 480, 30);
            var s = manager.EnableHand();

            this.handAnalyzer = manager.QueryHand();


            var re = manager.Init();


            this.outputData = handAnalyzer.CreateOutput();

            if (re != pxcmStatus.PXCM_STATUS_NO_ERROR)
                throw new RealSenseException("error: " + re, re);

            this.isHandsStreamEnabled = true;

        }

        public void EnableHandsStream()
        {
            if (streamTask == null || streamTask.Status != TaskStatus.Running)
            {
                throw new RealSenseException("Sensor not started");
            }
            else
            {
                this.isHandsStreamEnabled = true;
            }
        }

        public void DisableHandsStream()
        {
            if (streamTask == null || streamTask.Status != TaskStatus.Running)
            {
                throw new RealSenseException("Sensor not started");
            }
            else
            {
                this.isHandsStreamEnabled = false;
            }
        }

        #endregion Hands

        #region 3DSeg

        public void Inizialize3DSeg(){

            manager.Enable3DSeg();

            var re = manager.Init();

            if (re != pxcmStatus.PXCM_STATUS_NO_ERROR)
                throw new RealSenseException("error: " + re, re);

            this.is3DSegEnabled = true;
        }

        public void Enable3DSeg()
        {
            if (streamTask == null || streamTask.Status != TaskStatus.Running)
            {
                throw new RealSenseException("Sensor not started");
            }
            else
            {
                this.is3DSegEnabled = true;
            }
        }

        public void Disable3DSeg()
        {
            if (streamTask == null || streamTask.Status != TaskStatus.Running)
            {
                throw new RealSenseException("Sensor not started");
            }
            else
            {
                this.is3DSegEnabled = false;
            }
        }
        #endregion



        #region Emotion

        public void InizializeEmotion()
        {

            manager.EnableEmotion();

            var re = manager.Init();

            if (re != pxcmStatus.PXCM_STATUS_NO_ERROR)
                throw new RealSenseException("error: " + re, re);

            this.isEmotionEnabled = true;
        }

        public void EnableEmotion()
        {
            if (streamTask == null || streamTask.Status != TaskStatus.Running)
            {
                throw new RealSenseException("Sensor not started");
            }
            else
            {
                this.isEmotionEnabled = true;
            }
        }

        public void DisableEmotion()
        {
            if (streamTask == null || streamTask.Status != TaskStatus.Running)
            {
                throw new RealSenseException("Sensor not started");
            }
            else
            {
                this.isEmotionEnabled = false;
            }
        }
        #endregion



        #region Private Methods

        /// <summary>
        /// Acquire the frames from streams
        /// </summary>
        private void AcquireFrames()
        {
            Console.WriteLine("RealSense THREAD ID: " + System.Threading.Thread.CurrentThread.ManagedThreadId);


            PXCMImage.ImageData segmented_image_data;
            PXCMImage segmentedImage;
            PXCM3DSeg pSeg;
            PXCMCapture.Sample sample;
            PXCMImage.ImageData colorData;
            PXCMImage.ImageData depthData;
            //PXCMEmotion pEmo;


            while (manager.AcquireFrame(true) >= pxcmStatus.PXCM_STATUS_NO_ERROR)
            {
                if (streamCancellationToken.IsCancellationRequested)
                {
                    break;
                }

                sample = manager.QuerySample();

                //Color analysis
                if (isColorStreamEnabled && sample.color != null)
                {
                    if (sample.color.AcquireAccess(PXCMImage.Access.ACCESS_READ, PXCMImage.PixelFormat.PIXEL_FORMAT_RGB32, out colorData) >= pxcmStatus.PXCM_STATUS_NO_ERROR)
                    {
                        parentContext.Post((object state) =>
                        {
                            if (ColorFrameReady != null) ColorFrameReady(this, new RealSenseEventArgs(colorData, sample.color.info));
                        }, "");
                    }
                    else
                    {
                        parentContext.Post((object state) =>
                        {
                            if (ColorFrameReady != null) ColorFrameReady(this, new RealSenseEventArgs());
                        }, "");
                    }

                    sample.color.ReleaseAccess(colorData);
                    sample.color.Dispose();

                }
                //Depth analysis
                if (isDepthStreamEnabled && sample.depth != null)
                {
                    if (sample.depth.AcquireAccess(PXCMImage.Access.ACCESS_READ, PXCMImage.PixelFormat.PIXEL_FORMAT_RGB32, out depthData) >= pxcmStatus.PXCM_STATUS_NO_ERROR)
                    {
                        parentContext.Post((object state) =>
                        {
                            if (DepthFrameReady != null) DepthFrameReady(this, new RealSenseEventArgs(depthData, sample.depth.info));
                        }, "");
                    }
                    else
                    {
                        parentContext.Post((object state) =>
                        {
                            if (DepthFrameReady != null) DepthFrameReady(this, new RealSenseEventArgs());
                        }, "");
                    }

                    sample.depth.ReleaseAccess(depthData);
                    sample.depth.Dispose();
                }
                //Gesture analysis
                if (isGestureRecognitionEnabled)
                {

                    this.outputData.Update();

                    int numberOfHands = this.outputData.QueryNumberOfHands();
                    PXCMHandData.IHand handData;




                    PXCMHandData.GestureData gestureData;

                    for (int i = 0; i < outputData.QueryFiredGesturesNumber(); i++)
                    {
                        try
                        {
                            if (outputData.QueryFiredGestureData(i, out gestureData) == pxcmStatus.PXCM_STATUS_NO_ERROR)
                            {

                                outputData.QueryHandDataById(gestureData.handId, out handData);
                                var bodySide = handData.QueryBodySide();

                                //Raise event from the parent context, not from the new started thread
                                parentContext.Post((object state) =>
                                {
                                    if (GestureRecognized != null) GestureRecognized(this, new RealSenseGestureEventArgs(gestureData, bodySide));
                                }, "");
                            }
                        }
                        catch (Exception e) { }
                    }
                }
                //Hand analysis
                if (isHandsStreamEnabled)
                {
                    this.outputData.Update();
                    Dictionary<PXCMHandData.JointType, PXCMHandData.JointData>[] hands = new Dictionary<PXCMHandData.JointType, PXCMHandData.JointData>[4];
                    PXCMHandData.IHand[] totalHands = new PXCMHandData.IHand[4];

                    int numberOfHands = outputData.QueryNumberOfHands();
                    if (numberOfHands > 0)
                    {
                        for (int i = 0; i < numberOfHands; i++)
                        {
                            Dictionary<PXCMHandData.JointType, PXCMHandData.JointData> handDictionary = new Dictionary<PXCMHandData.JointType, PXCMHandData.JointData>();
                            PXCMHandData.JointData temp;

                            PXCMHandData.IHand handData;
                            if (outputData.QueryHandData(PXCMHandData.AccessOrderType.ACCESS_ORDER_BY_ID, i, out handData) == pxcmStatus.PXCM_STATUS_NO_ERROR)
                            {
                                // iterate through Joints
                                for (int j = 0; j < PXCMHandData.NUMBER_OF_JOINTS; j++)
                                {
                                    handData.QueryTrackedJoint((PXCMHandData.JointType)j, out temp);
                                    handDictionary.Add((PXCMHandData.JointType)j, temp);
                                }
                                hands[i] = handDictionary;
                                totalHands[i] = handData;
                            }
                        }

                        //Raise event from the parent context, not from the new started thread
                        parentContext.Post((object state) =>
                        {
                            if (HandsFrameReady != null) HandsFrameReady(this, new RealSenseHandsEventArgs(hands, totalHands));
                        }, "");
                    }
                }
                if (is3DSegEnabled)
                {
                    pSeg = manager.Query3DSeg();
                    segmentedImage = pSeg.AcquireSegmentedImage();

                    if (segmentedImage != null)
                    {

                        segmentedImage.AcquireAccess(
                            PXCMImage.Access.ACCESS_READ_WRITE,
                            PXCMImage.PixelFormat.PIXEL_FORMAT_RGB32,
                            out segmented_image_data);

                        int height = segmentedImage.QueryInfo().height;
                        int width = segmentedImage.QueryInfo().width;

                        for (int y = 0; y < height; y++)
                        {
                            unsafe
                            {
                                byte* p = (byte*)segmented_image_data.planes[0] + y * segmented_image_data.pitches[0];
                                const byte grey = 0x7f;
                                for (int x = 0; x < width; x++)
                                {
                                    if (p[3] > 0)
                                    {
                                        // When the user moves into the near/far extent, the alpha values will drop from 255 to 1.
                                        // This can be used to fade the user in/out as a cue to move into the ideal operating range.
                                        float blend_factor = 1;//(float)p[3] / (float)255;
                                        for (int ch = 0; ch < 3; ch++)
                                        {
                                            byte not_visible = (byte)((p[ch] >> 4) + grey);
                                            p[ch] = (byte)(p[ch] * blend_factor + not_visible * (1.0f - blend_factor));
                                        }

                                    }
                                    else
                                    {
                                        for (int ch = 0; ch < 3; ch++) p[ch] = (byte)(0xFF);   // p[ch] = (byte)((p[ch] >> 4) + grey);


                                    }

                                    p += 4;
                                }
                            }
                        }


                        parentContext.Post((object state) =>
                        {
                            if (SegmentedFrameReady != null) SegmentedFrameReady(this, new RealSenseEventArgs(segmented_image_data, segmentedImage.info));
                        }, "");


                        // release the image
                        segmentedImage.ReleaseAccess(segmented_image_data);
                        segmentedImage.Dispose();
                    }
                }


                    //if (isEmotionEnabled)
                    //{
                    //    pEmo = manager.QueryEmotion();

                    //    int numFaces = pEmo.QueryNumFaces();
                    //    for (int i = 0; i < numFaces; i++)
                    //    {
                    //        /* Retrieve emotionDet location data */
                    //        PXCMEmotion.EmotionData[] data = new PXCMEmotion.EmotionData[10];
                    //        if (pEmo.QueryAllEmotionData(i, out data) >= pxcmStatus.PXCM_STATUS_NO_ERROR)
                    //        {
                    //            bool emotionPresent = false;
                    //            int epidx = -1;
                    //            int maxscoreE = -3;
                    //            float maxscoreI = 0;
                    //            for (int e = 0; e < NUM_EMOTIONS; e++)
                    //            {
                    //                if (data[e].evidence < maxscoreE) continue;
                    //                if (data[e].intensity < maxscoreI) continue;
                    //                maxscoreE = data[e].evidence;
                    //                maxscoreI = data[e].intensity;
                    //                epidx = e;
                    //            }
                    //            if ((epidx != -1) && (maxscoreI > 0.4))
                    //            {
                    //                emotionPresent = true;
                    //            }

                    //            int spidx = -1;
                    //            if (emotionPresent)
                    //            {
                    //                maxscoreE = -3;
                    //                maxscoreI = 0;
                    //                for (int e = 0; e < (NUM_EMOTIONS - NUM_PRIMARY_EMOTIONS); e++)
                    //                {
                    //                    if (data[NUM_PRIMARY_EMOTIONS + e].evidence < maxscoreE) continue;
                    //                    if (data[NUM_PRIMARY_EMOTIONS + e].intensity < maxscoreI) continue;
                    //                    maxscoreE = data[NUM_PRIMARY_EMOTIONS + e].evidence;
                    //                    maxscoreI = data[NUM_PRIMARY_EMOTIONS + e].intensity;
                    //                    spidx = e;
                    //                }
                    //                if ((spidx != -1))
                    //                {
                    //                    //SizeF line1Size = g.MeasureString(EmotionLabels[epidx], font);
                    //                    //SizeF line2Size = g.MeasureString(SentimentLabels[spidx], font);

                    //                    //float width = Math.Max(line1Size.Width, line2Size.Width);
                    //                    //float offset = Math.Max(line2Size.Height, font.GetHeight());
                    //                    //float height = line1Size.Height + offset;

                    //                    //int x = data[0].rectangle.x + data[0].rectangle.w;
                    //                    //int y = data[0].rectangle.y > 0 ? data[0].rectangle.y : data[0].rectangle.h - (int)height;

                    //                    //if (x + Math.Max(line1Size.Width, line2Size.Width) > bitmap.Width)
                    //                    //{
                    //                    //    x = data[0].rectangle.x - (int)width;
                    //                    //}

                    //                    //g.DrawString(EmotionLabels[epidx], font, brushTxt, x, y);
                    //                    //g.DrawString(SentimentLabels[spidx], font, brushTxt, x, data[0].rectangle.y > 0 ? y + offset : y - offset);
                    //                }
                    //            }
                    //        }
                    //    }


                    //    // release objects

                    //    pEmo.Dispose();                  
                        

                    //}

                    manager.ReleaseFrame();
                    GC.Collect();
                


            }
        }

        #endregion Private Methods

        #region RealSenseSensor Events

        public delegate void ColorFrameReadyHadler(object sender, RealSenseEventArgs e);
        public event ColorFrameReadyHadler ColorFrameReady;

        public delegate void DepthFrameReadyHadler(object sender, RealSenseEventArgs e);
        public event DepthFrameReadyHadler DepthFrameReady;

        public delegate void IRFrameReadyHadler(object sender, RealSenseEventArgs e);
        public event IRFrameReadyHadler IRFrameReady;

        public delegate void GestureRecognizedHadler(object sender, RealSenseGestureEventArgs e);
        public event GestureRecognizedHadler GestureRecognized;

        public delegate void HandsFrameReadyHadler(object sender, RealSenseHandsEventArgs e);
        public event HandsFrameReadyHadler HandsFrameReady;

        public delegate void SegmentedFrameReadyHadler(object sender, RealSenseEventArgs e);
        public event SegmentedFrameReadyHadler SegmentedFrameReady;

        public delegate void EmotionFrameReadyHadler(object sender, RealSenseEventArgs e);
        public event EmotionFrameReadyHadler EmotionFrameReady;


        #endregion RealSenseSensor Events


    }
}
