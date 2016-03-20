﻿using ProjectWerner.Contracts.API;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Kinect;
using SharpSenses;
using SharpSenses.RealSense.Capabilities;
using System.Management;
using Microsoft.Kinect.Face;
using System.Speech.Synthesis;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using SharpSenses.RealSense;


namespace ProjectWerner.API
{
    public class Camera3D : ICamera3D
    {
        #region Actions

        /// <summary>
        /// when face is not or not anymore visible
        /// </summary>
        public event Action FaceLost;

        /// <summary>
        /// when face is found
        /// </summary>
        public event Action FaceVisible;

        /// <summary>
        /// when mouth is opened
        /// </summary>
        public event Action MouthOpened;

        /// <summary>
        /// when mouth is closed
        /// </summary>
        public event Action MouthClosed;

        #endregion

        #region public variables

        /// <summary>
        /// mouth open is set when event is fired
        /// </summary>
        public bool IsFaceMouthOpen { get; set; }

        /// <summary>
        /// Only works with intel real sense
        /// how much the mouth has to be open to fire open event
        /// 0 closed - 100 open
        /// </summary>
        public int MouthOpenValue { get; set; }

        #endregion

        #region Kinect Variablen

        //bool if availiable
        private bool _kinectAvailiable;

        // The sensor objects.
        private KinectSensor _kinectSensor;

        // The face frame source
        private FaceFrameSource _faceSource;

        // The face frame reader
        private FaceFrameReader _faceReader;

        // The body frame reader is used to identify the bodies
        private BodyFrameReader _bodyReader = null;

        // The list of bodies identified by the sensor
        private IList<Body> _bodies = null;

        #endregion

        #region RealSense Variablen

        //bool if availiable
        private bool _realSenseAvailiable;

        //camera instance for realsense
        private ICamera _camera;

        #endregion

        /// <summary>
        /// Set MouthOpeValue to default 30
        /// Check for kinect
        /// check for camera
        /// Setup camera
        /// </summary>
        public Camera3D()
        {
            MouthOpenValue = 30;
            CheckKinect();
            CheckRealSense();
            SetupCamera();
            Usb();
        }

        /// <summary>
        /// check if kinect senosr is availble
        /// </summary>
        private void CheckKinect()
        {
         
            // one sensor is currently supported
            this._kinectSensor = KinectSensor.GetDefault();

            // set IsAvailableChanged event notifier
            this._kinectSensor.IsAvailableChanged += this.Sensor_IsAvailableChanged;

            Sensor_IsAvailableChanged(null, null);
        }

        /// <summary>
        /// check for Realsense in device manager
        /// searches for DeviceId VID_8086&PID_0A66
        /// </summary>
        private void CheckRealSense()
        {

            //list all devices on usb
            ManagementObjectCollection collection;
            using (var searcher = new ManagementObjectSearcher(@"Select * From Win32_USBHub"))
                collection = searcher.Get();
            bool found = false;
            foreach (var device in collection)
            {
                if (device == null) return;
                //check usb devices for intel realsense
                //VID_8086&PID_0A66
                if (((string) device.GetPropertyValue("DeviceID")).Contains("VID_8086&PID_0A66"))
                {
                    found = true;
                    break;
                }
            }
            collection.Dispose();
            _realSenseAvailiable = found;
           
            if (!_realSenseAvailiable)
            {
               _camera.SilentlyDispose();
            }
            GC.Collect();
        }

        /// <summary>
        /// open kamera and add events
        /// preferes kinect if realsense and kinect are both connected
        /// </summary>
        private void SetupCamera()
        {
           
            if (_kinectAvailiable)
            {
                SetupKinect();
            }

            if (this._realSenseAvailiable && !_kinectAvailiable)
            {
                SetupRealSense();
            }
        }

        /// <summary>
        /// create and open real sense camera
        /// uses Capability.FaceTracking, Capability.FacialExpressionTracking
        /// </summary>
        private void SetupRealSense()
        {
            if (_camera == null)
            {
                try
                {
                    _camera = Camera.Create(Capability.FaceTracking, Capability.FacialExpressionTracking);
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            if (_camera == null) return;

            _camera.Face.Visible += OnFaceVisible;
            _camera.Face.NotVisible += OnFaceLost;
            _camera.Face.Mouth.Opened += OnMouthOpened;
            _camera.Face.Mouth.Closed += OnMouthClosed;

            FacialExpressionCapability.MonthOpenThreshold = MouthOpenValue;

            _camera.Start();
        }

        /// <summary>
        /// setup kinect
        /// start search for bodies
        /// start bodyframereader to add body to facereader
        /// start facereader
        /// adds faceframereader to track mouth
        /// </summary>
        private void SetupKinect()
        {
            if (_kinectSensor == null) return;

            _bodies = new Body[_kinectSensor.BodyFrameSource.BodyCount];
            
            _bodyReader = _kinectSensor.BodyFrameSource.OpenReader();

            _bodyReader.FrameArrived += BodyReader_FrameArrived; 

            // Initialize the face source with the desired features
            _faceSource = new FaceFrameSource(_kinectSensor, 0, FaceFrameFeatures.MouthOpen);

            _faceReader = _faceSource.OpenReader();
            
            _faceReader.FrameArrived += FaceReader_FrameArrived;
           
        }

        /// <summary>
        /// is called when camera sees sth
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BodyReader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
         
            using (var frame = e.FrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                  
                    //when body is found
                    frame.GetAndRefreshBodyData(_bodies);

                    Body body = _bodies.Where(b => b.IsTracked).FirstOrDefault();

                    if (!_faceSource.IsTrackingIdValid)
                    {
                        if (body != null)
                        {
                            // Assign a tracking ID to the face source
                            _faceSource.TrackingId = body.TrackingId;
                        }
                    }
                }
             
            }
        }


        /// <summary>
        /// when face is found calls this event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FaceReader_FrameArrived(object sender, FaceFrameArrivedEventArgs e)
        {
         
            using (var frame = e.FrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    FaceFrameResult result = frame.FaceFrameResult;

                    if (result != null)
                    {
                        //face is visible
                        OnFaceVisible(null, null);

                        // Get the face characteristics
                        var mouthOpen = result.FaceProperties[FaceProperty.MouthOpen];
                        //result.FaceFrameFeatures 
                        //fire events

                        CallAction(mouthOpen, OnMouthOpened, OnMouthClosed);
                    }
                    else
                    {
                        //face is invisible (lost)
                        OnFaceLost(null, null);
                    }
                }
            }
        }

        /// <summary>
        /// Calls Action depending on DetectionResult
        /// </summary>
        /// <param name="detectionResult"></param>
        /// <param name="resultYes">call when DetectionResult.Yes</param>
        /// <param name="resultNo">call when DetectionResult.No</param>
        private void CallAction(DetectionResult detectionResult, Action<object, EventArgs> resultYes,
            Action<object, EventArgs> resultNo)
        {
            switch (detectionResult)
            {
                case DetectionResult.Yes:
                    resultYes(null, null);
                    break;
                case DetectionResult.No:
                    resultNo(null, null);
                    break;
            }
        }

        /// <summary>
        /// Handles the event which the sensor becomes unavailable (E.g. paused, closed, unplugged).
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Sensor_IsAvailableChanged(object sender, IsAvailableChangedEventArgs e)
        {
         
            if (this._kinectSensor != null)
            {
              
                if (!_kinectSensor.IsOpen)
                {
                    // open the sensor
                    this._kinectSensor.Open();
                }
                // on failure, set the status text
                this._kinectAvailiable = this._kinectSensor.IsAvailable;

                //setup kinect events etc
                if (_kinectAvailiable)
                {
                    SetupKinect();
                }
            }
        }

        /// <summary>
        /// fired when mouth is closed
        /// sets IsFaceMouthOpen to false
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouthClosed(object sender, EventArgs e)
        {
            IsFaceMouthOpen = false;
            MouthClosed?.Invoke();
        }

        /// <summary>
        /// fired when mouth is opened
        /// sets IsFaceMouthOpen to true
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouthOpened(object sender, EventArgs e)
        {
            IsFaceMouthOpen = true;
            MouthOpened?.Invoke();
        }

        /// <summary>
        /// fires when face is lost
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFaceLost(object sender, EventArgs e)
        {
            FaceLost?.Invoke();
        }

        /// <summary>
        /// fires when face is lost
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFaceVisible(object sender, EventArgs e)
        {
            FaceVisible?.Invoke();
        }

        /// <summary>
        /// output string as speech on speaker
        /// german for realsense
        /// windows system language for kinect
        /// </summary>
        /// <param name="message"></param>
        public void Speech(string message)
        {
            //realsense
            if (_camera != null)
            {
                //deutsch
                _camera.Speech.Say(message);
            }
            //kinect
            else
            {
                // Initialize a new instance of the SpeechSynthesizer.
                using (SpeechSynthesizer synth = new SpeechSynthesizer())
                {
                    //system sprache
                    synth.SelectVoiceByHints(VoiceGender.NotSet, VoiceAge.NotSet, 0,
                        Thread.CurrentThread.CurrentUICulture);

                    // Configure the audio output. 
                    synth.SetOutputToDefaultAudioDevice();

                    // Speak a string synchronously.
                    synth.Speak(message);
                }
            }


        }


        /// <summary>
        /// setup event handler that recognizes ubs device (dis)connected
        /// </summary>
        private void Usb()
        {//From Win32_USBHub  Win32_USBControllerDevice
            WqlEventQuery q = new WqlEventQuery("__InstanceOperationEvent",
                "TargetInstance ISA 'Win32_USBHub' ");

            q.WithinInterval = TimeSpan.FromSeconds(1);

            ManagementEventWatcher w = new ManagementEventWatcher(q);

            w.EventArrived += new EventArrivedEventHandler(OnEventArrived);

            w.Start();
        }

        /// <summary>
        /// event is fired when usb device is (dis)connected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnEventArrived(object sender, EventArrivedEventArgs e)
        {
        
            //USB Device Changed
            CheckKinect();
            CheckRealSense();
               SetupCamera();
        }
    }
}