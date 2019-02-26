using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace IrisbondAPI
{
    /// <summary>
    /// The main Irisbond Duo eye tracker class
    /// For a detailed documentation please go to Doc
    /// </summary>
    class IrisbondDuo
    {
        #region enums

        /// <summary>
        /// Result of the API initialization returned by start()
        /// </summary>
        public enum START_STATUS { START_OK, NO_CAMERA, CAMERA_IN_USE, CAMERA_ERROR, QT_GUI_ERROR, INVALID_LICENSE };

        /// <summary>
        /// Result of the calibration process returned by waitForCalibrationToEnd()
        /// </summary>
        public enum CALIBRATION_STATUS { CALIBRATION_TIMEOUT, CALIBRATION_ENDED_ALT_F4, CALIBRATION_ENDED_ESCAPE, CALIBRATION_FINISHED, CALIBRATION_CANCELLED, CALIBRATION_FAILED_NO_CAMERA };

        /// <summary>
        /// Eye codification used by setUserEyeControlMode()
        /// </summary>
        public enum CONTROLLING_EYE { CONTROL_LEFT, CONTROL_RIGHT, CONTROL_ANY, CONTROL_BOTH_ALWAYS };

        #endregion


        #region delegates

        /// <summary>
        /// Function to be called when a new image is processed and a new gaze point is received.
        /// </summary>
        /// <param name="timestamp">The timestamp that identifies the current gaze point. In milliseconds, from epoch.</param>
        /// <param name="mouseX">X coordinate of the gaze point. In pixels, where 0 is the left side of the image.</param>
        /// <param name="mouseY">Y coordinate of the gaze point. In pixels, where 0 is the top of the image.</param>
        /// <param name="mouseRawX">X coordinate of the raw gaze point, this means, without any filtering. In pixels. For research only.</param>
        /// <param name="mouseRawY">Y coordinate of the raw gaze point, this means, without any filtering. In pixels. For research only.</param>
        /// <param name="screenWidth">Screen width considered by the API to estimate the gaze point. In pixels.</param>
        /// <param name="screenHeight">Screen height considered by the API to estimate the gaze point. In pixels.</param>
        /// <param name="leftEyeDetected">True when the left eye is detected in the current gaze point. False when it is not.</param>
        /// <param name="rightEyeDetected">True when the right eye is detected in the current gaze point. False when it is not.</param>
        /// <param name="imageWidth">Camera image width. In pixels.</param>
        /// <param name="imageHeight">Camera image height. In pixels.</param>
        /// <param name="leftEyeX">X coordinate of the left eye in the camera image. Normalized between 0 and 1, where 0 is the left side of the image, and 1 is the right side.</param>
        /// <param name="leftEyeY">Y coordinate of the left eye in the camera image. Normalized between 0 and 1, where 0 is the top of the image, and 1 is the bottom side.</param>
        /// <param name="leftEyeSize">Left eye pupil size. In pixels.</param>
        /// <param name="rightEyeX">X coordinate of the right eye in the camera image. Normalized between 0 and 1, where 0 is the left side of the image, and 1 is the right side.</param>
        /// <param name="rightEyeY">Y coordinate of the right eye in the camera image. Normalized between 0 and 1, where 0 is the top of the image, and 1 is the bottom side.</param>
        /// <param name="rightEyeSize">Right eye pupil size. In pixels.</param>
        /// <param name="distanceFactor">Tells the distance between the eye tracker and the user. Between -1 and 1, where -1 means too far, 1 is too close, and 0 is the ideal distance. Recommended [-0.5, 0.5]</param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void DATA_CALLBACK(
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
            float distanceFactor);

        /// <summary>
        /// Function to be called when the calibration finishes, and the global calibration results are received.
        /// </summary>
        /// <param name="leftPrecisionError">Left eye precision, in pixels</param>
        /// <param name="leftAccuracyError">Left eye accuracy, in pixels</param>
        /// <param name="rightPrecisionError">Right eye precision, in pixels</param>
        /// <param name="rightAccuracyError">Right eye accuracy, in pixels</param>
        /// <param name="combinedPrecisionError">Both eyes precision, in pixels</param>
        /// <param name="combinedAccuracyError">Both eyes accuracy, in pixels</param>
        /// <param name="cancelled">True when the calibration is cancelled. Results must be ignored when cancelled is true.</param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void CALIBRATION_RESULTS_CALLBACK(
            double leftPrecisionError,
            double leftAccuracyError,
            double rightPrecisionError,
            double rightAccuracyError,
            double combinedPrecisionError,
            double combinedAccuracyError,
            [MarshalAs(UnmanagedType.U1)] bool cancelled);

        /// <summary>
        /// Function to be called when the calibration finishes, and the detailed calibration results (point by point) are received.
        /// </summary>
        /// <param name="nPoints">The number of calibration points</param>
        /// <param name="xCoords">A vector with the X coordinates of the calibration points. Normalized between 0 (left of the screen) and 1 (right of the screen)</param>
        /// <param name="yCoords">A vector with the Y coordinates of the calibration points. Normalized between 0 (top of the screen) and 1 (bottom of the screen)</param>
        /// <param name="leftErrorsPx">A vector with the left eye errors for all the points. In pixels.</param>
        /// <param name="rightErrorsPx">A vector with the right eye errors for all the points. In pixels.</param>
        /// <param name="combinedErrorsPx">A vector with the combined eye errors for all the points. In pixels.</param>
        /// <param name="leftErrorsNorm">A vector with the left eye errors for all the points. Normalized between 0 (perfect point) and 1 (should be improved)</param>
        /// <param name="rightErrorsNorm">A vector with the right eye errors for all the points. Normalized between 0 (perfect point) and 1 (should be improved)</param>
        /// <param name="combinedErrorsNorm">A vector with the combined eye errors for all the points. Normalized between 0 (perfect point) and 1 (should be improved)</param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void CALIBRATION_RESULTS_POINTS_CALLBACK(
            int nPoints,
            IntPtr xCoords,
            IntPtr yCoords,
            IntPtr leftErrorsPx,
            IntPtr rightErrorsPx,
            IntPtr combinedErrorsPx,
            IntPtr leftErrorsNorm,
            IntPtr rightErrorsNorm,
            IntPtr combinedErrorsNorm);

        /// <summary>
        /// Function to be called when the calibration is cancelled.
        /// </summary>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void CALIBRATION_CANCELLED_CALLBACK();

        /// <summary>
        /// Function to be called during the calibration to know the calibration target position and state
        /// </summary>
        /// <param name="pointX">X coordinate of the target. In pixels.</param>
        /// <param name="pointY">Y coordinate of the target. In pixels.</param>
        /// <param name="screenWidth">Screen width considered by the API to locate the target. In pixels.</param>
        /// <param name="screenHeight">Screen heigth considered by the API to locate the target. In pixels.</param>
        /// <param name="fixated">True when the target is fixated in the point (pointX, pointY), false when it is travelling towards that point.</param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void CALIBRATION_TARGET_CALLBACK(double pointX, double pointY, int screenWidth, int screenHeight, [MarshalAs(UnmanagedType.U1)] bool fixated);

        /// <summary>
        /// Function to be called during the calibration when the user is not detected in one calibration point and decide to retry that point or cancel calibration
        /// </summary>
        /// <returns>True to try again, false to cancel calibration</returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate bool CALIBRATION_POINT_ERROR_CALLBACK();

        /// <summary>
        /// Function to be called during the calibration when a new frame is acquired from the camera and the image is received.
        /// </summary>
        /// <param name="data">A pointer to the image raw data</param>
        /// <param name="rows">The number of rows in the image</param>
        /// <param name="cols">The number of columns in the image</param>
        /// <param name="channels">The number of channels in the image. Typically 1.</param>
        /// <param name="timestamp">The timestamp that identifies the current gaze point. In milliseconds, from epoch.</param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void IMAGE_CALLBACK(IntPtr data, int rows, int cols, int channels, long timestamp);

        /// <summary>
        /// Function to be called during the calibration when a blink is done by the user
        /// </summary>
        /// <param name="mouseX">X coordinate of the blink event. In pixels.</param>
        /// <param name="mouseY">Y coordinate of the blink event. In pixels.</param>
        /// <param name="screenWidth">Screen width considered by the API to locate the target. In pixels.</param>
        /// <param name="screenHeight">Screen heigth considered by the API to locate the target. In pixels.</param>        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void BLINK_CALLBACK(int x, int y, int screenWidth, int screenHeight);

        /// <summary>
        /// Function to be called during the calibration when a dwell is done by the user
        /// </summary>
        /// <param name="mouseX">X coordinate of the dwell event. In pixels.</param>
        /// <param name="mouseY">Y coordinate of the dwell event. In pixels.</param>
        /// <param name="screenWidth">Screen width considered by the API to locate the target. In pixels.</param>
        /// <param name="screenHeight">Screen heigth considered by the API to locate the target. In pixels.</param>   
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void DWELL_CALLBACK(int mouseX, int mouseY, int screenWidth, int screenHeight);

        #endregion


        #region functions

        /// <summary>
        /// Check that there is an eye tracker connected to the computer.
        /// </summary>
        /// <returns>True if the eye tracker is connected. False if the eye tracker is not connected or the drivers are not installed</returns>
        [DllImport("IrisbondAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool trackerIsPresent();


        [DllImport("IrisbondAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr getHardwareNumber();

        /// <summary>
        /// Get the eye tracker serial number in the format IRIS12345678. 
        /// This ID is unique for each eye tracker. 
        /// </summary>
        /// <returns>the hardware serial number</returns>
        public static string getHWNumber() {
            string s = Marshal.PtrToStringAnsi(getHardwareNumber());
            return s;
        }


        [DllImport("IrisbondAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr getCameraKeyword();

        /// <summary>
        /// Get the eye tracker model.
        /// This ID is common for all eye trackers of the same type (i.e. Primma, Duo).
        /// This is only required for troubleshooting
        /// </summary>
        /// <returns>the hardware model</returns>
        public static string getHWKeyword() {
            string s = Marshal.PtrToStringAnsi(getCameraKeyword());
            return s;
        }

        /// <summary>
        /// Get the eye tracker focus distance in centimeters.
        /// </summary>
        /// <returns>the optimal distance between the tracker and the user for the best experience</returns>
        [DllImport("IrisbondAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int getCameraFocusDistance();


        /// <summary>
        /// Start the API: Start the eye tracker and the processing loop
        /// </summary>
        /// <returns>START_STATUS code that tells whether start process is correct or there is some error</returns>
        [DllImport("IrisbondAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern START_STATUS start();

        /// <summary>
        /// Stop the API: Stop the eye tracker and the processing loop
        /// </summary>
        [DllImport("IrisbondAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void stop();





        /// <summary>
        /// Show a fullscreen window that helps the user know the optimal location for the best tracking experience.
        /// It can be closed by clicking/tapping of by calling hidePositioningWindow()
        /// </summary>
        [DllImport("IrisbondAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void showPositioningWindow();

        /// <summary>
        /// Close the positioning window.
        /// </summary>
        [DllImport("IrisbondAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void hidePositioningWindow();




        /// <summary>
        /// Start a testing process with 36 calibration points
        /// waitForCalibrationToEnd() must be called immediately after this function.
        /// </summary>
        /// <param name="userDistance">Accurate distance from the user to the screen in cm.</param>
        [DllImport("IrisbondAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void startTesting(int userDistance);

        /// <summary>
        /// Enable or disable the step-by-step calibration mode.
        /// </summary>
        /// <param name="record">True when you want to perform step-by-step calibration, false when you want a calibration without any pause. False by default.</param>
        [DllImport("IrisbondAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void setTestingRecording(bool record);





        /// <summary>
        /// Configure the calibration target travel and fixation time.
        /// </summary>
        /// <param name="travelTime">Time that takes the target to go from one calibration point to another one. In seconds. Default value is 2s</param>
        /// <param name="fixationTime">Time that takes the target to stay fixed in one calibration point. In seconds. Default value is 1s</param>
        [DllImport("IrisbondAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void setCalibrationParameters(double travelTime, double fixationTime);

        /// <summary>
        /// Configure the calibration target and background color.
        /// </summary>
        /// <param name="targetColor">The color of the target in 32 bit unsigned integer format (uint32_t), RGBA channels.</param>
        /// <param name="backgroundColor">The color of the background in 32 bit unsigned integer format (uint32_t), RGBA channels.</param>
        [DllImport("IrisbondAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void setCalibrationColors(uint targetColor, uint backgroundColor);

        /// <summary>
        /// Configure the calibration target size.
        /// </summary>
        /// <param name="targetSize">The size of the target in a range of 1 to 100. The default size value of 25 corresponds to 3% of the screen height.</param>
        [DllImport("IrisbondAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void setTargetSize(int targetSize);

        /// <summary>
        /// Configure the calibration GUI changing the image that will be shown as the target.
        /// </summary>
        /// <param name="index">Absolut path to the image.</param>
        [DllImport("IrisbondAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void setLocalTargetImage(int index);

        /// <summary>
        /// Load a .png image to be shown as the target. If the image is not found, the standard target will be shown.
        /// </summary>
        /// <param name="pathToImage">Absolut path to the image.</param>
        [DllImport("IrisbondAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool loadTargetImage(string pathToImage);

        /// <summary>
        /// Configure the calibration GUI displaying the walking duck animation.
        /// </summary>
        [DllImport("IrisbondAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void setLocalTargetAnimation();

        /// <summary>
        /// Load a .png image to be shown as the target. If the image is not found, the standard target will be shown.
        /// </summary>
        /// <param name="pathToAnimation">Absolut path to the animation.</param>
        /// <param name="frames">Total number of frames.</param>
        /// <param name="loopTime">The time it should take the animation to complete a loop in seconds.</param>
        [DllImport("IrisbondAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool loadTargetAnimationSpriteSheet(string pathToAnimation, int frames, float loopTime);

        /// <summary>
        /// Start a calibration process to adapt the eye tracker to the user's eyes.
        /// waitForCalibrationToEnd() must be called immediately after this function.
        /// </summary>
        /// <param name="numCalibPoints">Number of calibration points. Can be 2, 3, 5, 9 or 16.</param>
        [DllImport("IrisbondAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void startCalibration(int numCalibPoints);

        /// <summary>
        /// Cancel the calibration process
        /// </summary>
        [DllImport("IrisbondAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void cancelCalibration();

        /// <summary>
        /// Start a 3-point calibration rectification to improve the worst 3 calibration points
        /// waitForCalibrationToEnd() must be called immediately after this function.
        /// </summary>
        [DllImport("IrisbondAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void startImproveCalibration();

        /// <summary>
        /// Start a 1-point calibration rectification to improve the calibration
        /// waitForCalibrationToEnd() must be called immediately after this function.
        /// </summary>
        [DllImport("IrisbondAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void startCalibrationRectification();

        /// <summary>
        /// Wait during the calibration processes until they finish
        /// </summary>
        /// <param name="timeoutInMinutes">Time to force the calibration process to end in case the user can't do it properly. In minutes.</param>
        /// <returns>CALIBRATION_STATUS Code to know the reason why the calibration ends</returns>
        [DllImport("IrisbondAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern CALIBRATION_STATUS waitForCalibrationToEnd(int timeoutInMinutes);

        /// <summary>
        /// Enable or disable the API calibration GUI
        /// </summary>
        /// <param name="show">True when you want to show the API calibration GUI, false when you want to show your own calibration GUI. False by default.</param>
        [DllImport("IrisbondAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void showCalibrationGUI(bool show);

        /// <summary>
        /// Enable or disable the API calibration error GUI when the user detection fails
        /// </summary>
        /// <param name="show">True when you want to show the API error GUI, false when you want to show your own error GUI. False by default.</param>
        [DllImport("IrisbondAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void showCalibrationPointErrorGUI(bool show);

        /// <summary>
        /// Enable or disable the API results GUI when the calibration ends
        /// </summary>
        /// <param name="show">True when you want to show the API results GUI, false when you want to show your own results GUI. False by default.</param>
        [DllImport("IrisbondAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void showCalibrationResultsGUI(bool show);

        /// <summary>
        /// Enable or disable the step-by-step calibration mode.
        /// </summary>
        /// <param name="step">True when you want to perform step-by-step calibration, false when you want a calibration without any pause. False by default.</param>
        [DllImport("IrisbondAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void setStepByStepCalibration(bool step);

        /// <summary>
        /// Start recording on a paused fixed point on step-by-step calibration mode. Then, it will automatically advance to the next point.
        /// </summary>
        [DllImport("IrisbondAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void resumeCalibration();
        

        [DllImport("IrisbondAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr getDefaultCalibrationChar();
        /// <summary>
        /// Get default calibration string for your screen.
        /// </summary>
        /// <returns> the default calibration string.</returns>
        public static string getDefaultCalibrationString()
        {
            string s = Marshal.PtrToStringAnsi(getDefaultCalibrationChar());
            return s;
        }

        [DllImport("IrisbondAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr getCalibrationChar();
        /// <summary>
        /// Get current calibration string. If no calibration has been performed, the default one will be returned.
        /// </summary>
        /// <returns> the current calibration string.</returns>
        public static string getCalibrationString()
        {
            string s = Marshal.PtrToStringAnsi(getCalibrationChar());
            return s;
        }

        /// <summary>
        /// Load a calibration string to the system. This will overwrite the calibration data and will automatically reset the calibration.
        /// </summary>
        /// <param name="calibrationString">A string that contains the data for the calibration. This string can be obtained using either getCalibrationString() or getDefaultCalibrationString() functions.</param>
        [DllImport("IrisbondAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void loadCalibrationChar(string calibrationChar);

        /// <summary>
        /// Load a calibration string to the system. This will overwrite the calibration data and will automatically reset the calibration.
        /// </summary>
        [DllImport("IrisbondAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void loadDefaultCalibrationChar();





        /// <summary>
        /// Configure the eye(s) to be taken into account for the eye tracker. It can be either eyes or both
        /// </summary>
        /// <param name="controlMode">The controlling eye in CONTROLLING_EYE format. Default value is CONTROLLING_EYE::CONTROL_ANY.</param>
        /// <returns>Deprecated, always true.</returns>
        [DllImport("IrisbondAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool setUserEyeControlMode(CONTROLLING_EYE controlMode);

        /// <summary>
        /// Configure the smoothness of the mouse cursor. Low smoothing implies high speed, and viceversa.
        /// </summary>
        /// <param name="smooth">Smoothing amount. Can take values between 0 and 14, where 0 means a fast mouse, and 14 a very slow but steady mouse. Default value is 2.</param>
        [DllImport("IrisbondAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void setSmoothValue(int smooth);

        /// <summary>
        /// Configure the blink detector. A blink is detected when one or two eyes are closed in a time interval defined between a minimmum and a maximum blink times
        /// </summary>
        /// <param name="singleTime">The minimum time to be considered a blink. In seconds. Default value is 0.3s.</param>
        /// <param name="cancelTime">The maximum time to be considered a blink. In seconds. Default value is 1.0s.</param>
        /// <param name="bothEyesRequired">True when the blink must be done by the two eyes at the same time. False when only one eye is enough. False by default.</param>
        [DllImport("IrisbondAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void setBlinkConfiguration(double singleTime, double cancelTime, bool bothEyesRequired);

        /// <summary>
        /// Configure the dwell detector. A dwell is detected when the mouse is located within an area for a specific time.
        /// </summary>
        /// <param name="areaPixels">The distance the mouse can move within a fixation. In pixels. Default value is 30px.</param>
        /// <param name="time">The minumum time to be considered a fixation. In seconds. Default value is 1.0s.</param>
        /// <param name="bothEyesRequired">True when the dwell must be done by the two eyes at the same time. False when only one eye is enough. False by default.</param>
        [DllImport("IrisbondAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void setDwellConfiguration(int areaPixels, double time, bool bothEyesRequired);





        /// <summary>
        /// Configure the tracker to run in high performance mode.
        /// </summary>
        /// <param name="enable">True to activate high performance mode, false to disable it. True by default</param>
        [DllImport("IrisbondAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void setHighPerformanceMode(bool enable);

        /// <summary>
        /// Get the tracker high performance status
        /// </summary>
        /// <returns>True when the API is in high performance mode. False when it is not.</returns>
        [DllImport("IrisbondAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool isHighPerformanceMode();









        /// <summary>
        /// Set the callback to be called when a new image is processed and a new gaze point is received.
        /// </summary>
        /// <param name="theCallback">The pointer to the function to be executed</param>
        [DllImport("IrisbondAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void setDataCallback(DATA_CALLBACK theCallback);

        /// <summary>
        /// Set the callback to be called when the calibration finishes, and the global calibration results are received.
        /// </summary>
        /// <param name="theCallback">The pointer to the function to be executed</param>
        [DllImport("IrisbondAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void setCalibrationResultsCallback(CALIBRATION_RESULTS_CALLBACK theCallback);

        /// <summary>
        /// Set the callback to be called when the calibration finishes, and the detailed calibration results (point by point) are received.
        /// </summary>
        /// <param name="theCallback">The pointer to the function to be executed</param>
        [DllImport("IrisbondAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void setCalibrationResultsPointsCallback(CALIBRATION_RESULTS_POINTS_CALLBACK theCallback);

        /// <summary>
        /// Set the callback to be called when the calibration is cancelled.
        /// </summary>
        /// <param name="theCallback">The pointer to the function to be executed</param>
        [DllImport("IrisbondAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void setCalibrationCancelledCallback(CALIBRATION_CANCELLED_CALLBACK theCallback);

        /// <summary>
        /// Set the callback to be called during the calibration to know the calibration target position and state
        /// </summary>
        /// <param name="theCallback">The pointer to the function to be executed</param>
        [DllImport("IrisbondAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void setCalibrationTargetCallback(CALIBRATION_TARGET_CALLBACK theCallback);

        /// <summary>
        /// Set the callback to be called during the calibration when the user is not detected in one calibration point 
        /// </summary>
        /// <param name="theCallback">The pointer to the function to be executed</param>
        [DllImport("IrisbondAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void setCalibrationPointErrorCallback(CALIBRATION_POINT_ERROR_CALLBACK theCallback);

        /// <summary>
        /// Set the callback to be called when a new frame is acquired from the camera and the image is received.
        /// </summary>
        /// <param name="theCallback">The pointer to the function to be executed</param>
        [DllImport("IrisbondAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void setImageCallback(IMAGE_CALLBACK theCallback);

        /// <summary>
        /// Set the callback to be called when a blink is done by the user
        /// </summary>
        /// <param name="theCallback">The pointer to the function to be executed</param>
        [DllImport("IrisbondAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void setBlinkCallback(BLINK_CALLBACK theCallback);

        /// <summary>
        /// Set the callback to be called when a dwell is done by the user
        /// </summary>
        /// <param name="theCallback">The pointer to the function to be executed</param>
        [DllImport("IrisbondAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void setDwellCallback(DWELL_CALLBACK theCallback);






        /// <summary>
        /// Check that the API ends after calling stop()
        /// </summary>
        /// <returns>True when the API stops properly. False when the API is still stopping.</returns>
        [DllImport("IrisbondAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool isApplicationEnded();

        /// <summary>
        /// Switch on the eye tracker leds
        /// </summary>
        /// <returns>True when the action is done properly. False when it's not.</returns>
        [DllImport("IrisbondAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool switchOnLedLights();

        /// <summary>
        /// Switch off the eye tracker leds
        /// </summary>
        /// <returns>True when the action is done properly. False when it's not.</returns>
        [DllImport("IrisbondAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool switchOffLedLights();

        /// <summary>
        /// Check the eye tracker leds status
        /// </summary>
        /// <returns>True when the leds are on, false when the leds are off.</returns>
        [DllImport("IrisbondAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool areLedLightsOn();

        /// <summary>
        /// Set the license code when the license system is not based on an activation file
        /// </summary>
        /// <param name="license">license code </param>
        [DllImport("IrisbondAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void setLicense(string license);

        #endregion

    }
}
