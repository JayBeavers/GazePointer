using QuickLink2DotNet;
using QuickStart;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Microsoft.HandsFree.Sensors
{
    internal class EyeTechDSSensor : IGazeDataProvider
    {
        DispatcherTimer _timer;

        int _mouseMoveCount;

        event EventHandler<GazeEventArgs> IGazeDataProvider.GazeEvent
        {
            add
            {
                _gazeEvent += value;
            }
            remove
            {
                _gazeEvent -= value;
            }
        }
        event EventHandler<GazeEventArgs> _gazeEvent;

        public Sensors Sensor { get { return Sensors.EyeTechDS; } }

        IntPtr deviceId;
        QLFrameData frameData = new QLFrameData();
        void Tick(object sender, EventArgs e)
        {
            try
            {
                QuickLink2API.QLDevice_GetFrame(deviceId, (IntPtr)10000, ref frameData);
                if (frameData.WeightedGazePoint.Valid)
                {
                    var eventData = new GazeEventArgs(
                        frameData.WeightedGazePoint.x * System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width / 100,
                        frameData.WeightedGazePoint.y * System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height / 100,
                        Environment.TickCount,
                        Fixation.Unknown,
                        false
                    );

                    _gazeEvent?.Invoke(this, eventData);
                }
            }
            catch
            {
                // A Win32Exception is sometimes thrown Mouse.GetPosition; this try-catch block just lessens the impact of that.
            }
        }

        void IGazeDataProvider.BeginAddCalibrationPoint(int x, int y)
        {
        }

        void IGazeDataProvider.EndAddCalibrationPoint()
        {
        }

        bool IGazeDataProvider.Initialize()
        {
            deviceId = Initialize.QL2Initialize("QL2Passwords.txt");
            QLError error = QuickLink2API.QLDevice_Start(deviceId);
            if (error != QLError.QL_ERROR_OK)
            {
                return false;
            }

            IntPtr calibrationId = IntPtr.Zero;
            //Calibrate.AutoCalibrate(deviceId, QLCalibrationType.QL_CALIBRATION_TYPE_16, ref calibrationId);
            //QuickLink2API.QLDevice_ApplyCalibration(deviceId, calibrationId);

            _timer = new DispatcherTimer(TimeSpan.FromSeconds(1.0 / 15), DispatcherPriority.Normal, Tick, Dispatcher.CurrentDispatcher);

            return true;
        }

        Task<bool> IGazeDataProvider.CreateProfileAsync()
        {
            ((IGazeDataProvider)this).LaunchRecalibration();
            return Task.FromResult(true);
        }

        void IGazeDataProvider.LaunchRecalibration()
        {
        }

        void IGazeDataProvider.Terminate()
        {
            _timer?.Stop();
        }
    }
}
