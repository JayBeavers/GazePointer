using IrisbondAPI;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Microsoft.HandsFree.Sensors
{
    public class IrisBondSensor : IGazeDataProvider
    {
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

        public Sensors Sensor { get { return Sensors.IrisBond; } }

        public void BeginAddCalibrationPoint(int x, int y)
        {
        }

        public void EndAddCalibrationPoint()
        {
        }

        public void dataCallback( 
            long timestamp,
            float mouseX,
            float mouseY,
            float mouseRawX,
            float pogRawY,
            int screenWidth,
            int screenHeight,
            [MarshalAs(UnmanagedType.U1)] bool leftEyeDetected,
            [MarshalAs(UnmanagedType.U1)] bool rightEyeDetected,
            int imageWidth,
            int imageHeight,
            float leftEyeX,
            float leftEyeY,
            float leftEyeSize,
            float rightEyeX,
            float rightEyeY,
            float rightEyeSize,
            float distanceFactor)
        {
            var eventData = new GazeEventArgs(
                mouseX * System.Windows.SystemParameters.PrimaryScreenWidth / 100,
                mouseY * System.Windows.SystemParameters.PrimaryScreenHeight / 100,
                Environment.TickCount, Fixation.Unknown, false);

            _gazeEvent?.Invoke(this, eventData);
        }

        public bool Initialize()
        {
            IrisbondDuo.setDataCallback(dataCallback);

            var status = IrisbondDuo.start();

            return status == IrisbondDuo.START_STATUS.START_OK;
        }

        public Task<bool> CreateProfileAsync()
        {
            LaunchRecalibration();
            return Task.FromResult(true);
        }

        public void LaunchRecalibration()
        {
            IrisbondDuo.startCalibration(9);
        }

        public void Terminate()
        {
            IrisbondDuo.stop();
        }

    }
}
