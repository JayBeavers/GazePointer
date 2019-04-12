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
                mouseX,
                mouseY,
                Environment.TickCount, Fixation.Unknown, false);

            _gazeEvent?.Invoke(this, eventData);
        }

        IrisbondDuo.DATA_CALLBACK fndataCallback;
        public bool Initialize()
        {
            fndataCallback = new IrisbondDuo.DATA_CALLBACK(dataCallback);
            var status = IrisbondDuo.start();
            IrisbondDuo.setDataCallback(fndataCallback);
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
            IrisbondDuo.waitForCalibrationToEnd(2);
        }

        public void Terminate()
        {
            IrisbondDuo.stop();
        }

    }
}
