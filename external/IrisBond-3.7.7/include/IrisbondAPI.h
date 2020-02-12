/*! \IrisbondAPI 
 * 
 * http://www.irisbond.com 
 *  
*/

#ifndef IRISBONDAPI_H
#define IRISBONDAPI_H

#ifdef WIN32
    #ifdef IrisbondAPI_STATIC
        #define API
    #elif defined IrisbondAPI_EXPORTS
        #define API __declspec(dllexport)
    #else
        #define API __declspec(dllimport)
    #endif
#endif

#include "defines.h"

extern "C" {
    namespace IrisbondAPI
    {
        //! \name Eye tracker identification
        //@{

        /** \brief Check that there is an eye tracker connected to the computer.
        \return True if the eye tracker is connected. False if the eye tracker is not connected or the drivers are not installed
        */
        API bool trackerIsPresent();

        /** \brief Get the eye tracker serial number in the format IRIS12345678. 
        This ID is unique for each eye tracker. 
        \return the hardware serial number
        */
        API const char* getHardwareNumber();

        /** \brief Get the eye tracker model.
        This ID is common for all eye trackers of the same type (i.e. Primma, Duo).
        This is only required for troubleshooting
        \return the hardware model
        */
        API const char* getCameraKeyword();

        /** \brief Get the eye tracker focus distance in centimeters.
        \return the optimal distance between the tracker and the user for the best experience
        */
        API int getCameraFocusDistance();

        //@}



        //! \name Basic functions
        //@{

        /** \brief Start the API: Start the eye tracker and the processing loop
        \return START_STATUS code that tells whether start process is correct or there is some error
        */
        API START_STATUS start();

        /** \brief Stop the API: Stop the eye tracker and the processing loop
        */
        API void stop();

        //@}


        //! \name User positioning
        //@{

        /** \brief Show a fullscreen window that helps the user know the optimal location for the best tracking experience.
        It can be closed by clicking/tapping of by calling hidePositioningWindow()
        */
        API void showPositioningWindow();

        /** \brief Close the positioning window.
        */
        API void hidePositioningWindow();

        //@}
        

        //! \name User calibration
        //@{

        /** \brief Configure the calibration target travel and fixation time
        \param[in] travelTime    Time that takes the target to go from one calibration point to another one. In seconds. Default value is 2s
        \param[in] fixationTime  Time that takes the target to stay fixed in one calibration point. In seconds. Default value is 1s
        */
        API void setCalibrationParameters(double travelTime, double fixationTime);

        /** \brief Configure the calibration GUI colors
        \param[in] targetColor   The color of the target in 32 bit unsigned integer format (uint32_t), RGBA channels.
        \param[in] backgroundColor  The color of the background in 32 bit unsigned integer format (uint32_t), RGBA channels.
        */
        API void setCalibrationColors(uint32_t targetColor, uint32_t backgroundColor);

        /** \brief Configure the calibration GUI target size.
        \param[in] targetSize   The size of the target from 1 to 100. 25 is equivalent to 3% of the height of the screen.
        */
        API void setTargetSize(int targetSize);

        /** \brief Configure the calibration GUI changing the image that will be shown as the target.
        \param[in] index  Index of the image: "0 = Android icon"
        */
        API void setLocalTargetImage(int index);

        /** \brief Load a .png image to be shown as the target.
        \param[in] pathToImage  Absolut path to the image. Please, when refering to paths, avoid using single backslashes ("\") as they will 
        be intepreted as special characters. You can use both double back slashes ("\\") or forward slashes ("/") instead.
        \return true if the image have been loaded successfully. False if not.
        */
        API bool loadTargetImage(const char * pathToImage);

        /** \brief Configure the calibration GUI displaying the walking duck animation.
        */
        API void setLocalTargetAnimation();

        /** \brief Load a .png animation sprite sheet to be shown as the target (frames in horizontal).
        \param[in] pathToAnimation  Absolut path to the image. Please, when refering to paths, avoid using single backslashes ("\") as they will 
        be intepreted as special characters. You can use both double back slashes ("\\") or forward slashes ("/") instead.
        \param[in] frames  Total number of frames.
        \param[in] loopTime  The time it should take the animation to complete a loop in seconds.
        \return true if the sprite sheet have been loaded successfully. False if not.
        */
        API bool loadTargetAnimationSpriteSheet(const char * pathToAnimation, int frames, float loopTime);
        
        /** \brief Start a calibration process to adapt the eye tracker to the user's eyes.
        waitForCalibrationToEnd() must be called immediately after this function.
        \param[in] numCalibPoints  Number of calibration points. Can be 2, 3, 5, 9 or 16.
        */
        API void startCalibration(int numCalibPoints);

        /** \brief Cancel the calibration process
        */
        API void cancelCalibration();

        /** \brief Start a 3-point calibration rectification to improve the worst 3 calibration points in a 5, 9 or 16-point calibration.
        waitForCalibrationToEnd() must be called immediately after this function.
        */
        API void startImproveCalibration();

        /** \brief Start a 1-point calibration rectification to improve the calibration
        waitForCalibrationToEnd() must be called immediately after this function.
        */
        API void startCalibrationRectification();

        /** \brief Wait during the calibration processes until they finish
        \param[in] timeoutInMinutes Time to force the calibration process to end in case the user can't do it properly. In minutes.
        \return CALIBRATION_STATUS Code to know the reason why the calibration ends
        */
        API CALIBRATION_STATUS waitForCalibrationToEnd(int timeoutInMinutes);

        /** \brief Enable or disable the API calibration GUI
        \param[in] show True when you want to show the API calibration GUI, false when you want to show your own calibration GUI. False by default.
        */
        API void showCalibrationGUI(bool show);

        /** \brief Enable or disable the API calibration error GUI when the user detection fails
        \param[in] show True when you want to show the API error GUI, false when you want to show your own error GUI. False by default.
        */
        API void showCalibrationPointErrorGUI(bool show);

        /** \brief Enable or disable the API results GUI when the calibration ends
        \param[in] show True when you want to show the API results GUI, false when you want to show your own results GUI. False by default.
        */
        API void showCalibrationResultsGUI(bool show);

        /** \brief Enable or disable the step-by-step calibration mode.
        \param[in] step True when you want to perform step-by-step calibration, false when you want a calibration without any pause. False by default.
        */
        API void setStepByStepCalibration(bool step);

        /** \brief Start recording on a paused fixed point on step-by-step calibration mode. Then, it will automatically advance to the next point.
        */
        API void resumeCalibration();

        /** \brief Get default calibration string for your screen.
        \return a pointer to the first character of the string.
        */
        API const char * getDefaultCalibrationChar();

        /** \brief Get current calibration string. If no calibration has been performed, the default one will be returned.
        \return a pointer to the first character of the string.
        */
        API const char * getCalibrationChar();

        /** \brief Load a calibration string to the system. Current calibration will be overwritten
        */
        API void loadCalibrationChar(char * calibrationChar);

        /** \brief Restore the default calibration, optimized for the current screen size.
        */
        API void loadDefaultCalibrationChar();




        //@}

        //! \name Tests
        //@{

        API void startTesting(int userDistance);
        API void setTestingRecording(bool record);
        //@}


        //! \name Eye control
        //@{

        /** \brief Configure the eye(s) to be taken into account for the eye tracker. It can be either eyes or both
        \param[in] controlMode  The controlling eye in CONTROLLING_EYE format. Default value is CONTROLLING_EYE::CONTROL_ANY.
        \return Deprecated, always true.
        */
        API bool setUserEyeControlMode(CONTROLLING_EYE controlMode);

        /** \brief Configure the smoothness of the mouse cursor. Low smoothing implies high speed, and viceversa.
        \param[in] smooth Smoothing amount. Can take values between 0 and 14, where 0 means a fast mouse, and 14 a very slow but steady mouse. Default value is 2.
        */
        API void setSmoothValue(int smooth);

        /** \brief Configure the blink detector. A blink is detected when one or two eyes are closed in a time interval defined between a minimmum and a maximum blink times
        \param[in] singleTime         The minimum time to be considered a blink. In seconds. Default value is 0.3s.
        \param[in] cancelTime         The maximum time to be considered a blink. In seconds. Default value is 1.0s.
        \param[in] bothEyesRequired   True when the blink must be done by the two eyes at the same time. False when only one eye is enough. False by default.
        */
        API void setBlinkConfiguration(double singleTime, double cancelTime, bool bothEyesRequired);

        /** \brief Configure the dwell detector. A dwell is detected when the mouse is located within an area for a specific time.
        \param[in] areaPixels         The distance the mouse can move within a fixation. In pixels. Default value is 30px.
        \param[in] time               The minumum time to be considered a fixation. In seconds. Default value is 1.0s.
        \param[in] bothEyesRequired   True when the dwell must be done by the two eyes at the same time. False when only one eye is enough. False by default.
        */
        API void setDwellConfiguration(int areaPixels, double time, bool bothEyesRequired);

        //@}



        //! \name Performance
        //@{

        /** \brief Configure the tracker to run in high performance mode.
        \param[in] enable True to activate high performance mode, false to disable it. True by default
        */
        API void setHighPerformanceMode(bool enable);

        /** \brief Get the tracker high performance status
        \return True when the API is in high performance mode. False when it is not.
        */
        API bool isHighPerformanceMode();

        //@}



        //! \name Callback setters
        //@{

        /** \brief Set the callback to be called when a new image is processed and a new gaze point is received.
        \param[in] theCallback The pointer to the function to be executed
        */
        API void setDataCallback(DATA_CALLBACK theCallback);

        /** \brief Set the callback to be called when the calibration finishes, and the global calibration results are received.
        \param[in] theCallback The pointer to the function to be executed
        */
        API void setCalibrationResultsCallback(CALIBRATION_RESULTS_CALLBACK theCallback);

        /** \brief Set the callback to be called when the calibration finishes, and the detailed calibration results (point by point) are received.
        \param[in] theCallback The pointer to the function to be executed
        */
        API void setCalibrationResultsPointsCallback(CALIBRATION_RESULTS_POINTS_CALLBACK theCallback);

        /** \brief Set the callback to be called when the calibration is cancelled.
        \param[in] theCallback The pointer to the function to be executed
        */
        API void setCalibrationCancelledCallback(CALIBRATION_CANCELLED_CALLBACK theCallback);

        /** \brief Set the callback to be called during the calibration to know the calibration target position and state
        \param[in] theCallback The pointer to the function to be executed
        */
        API void setCalibrationTargetCallback(CALIBRATION_TARGET_CALLBACK theCallback);

        /** \brief Set the callback to be called during the calibration when the user is not detected in one calibration point 
        \param[in] theCallback The pointer to the function to be executed
        */
        API void setCalibrationPointErrorCallback(CALIBRATION_POINT_ERROR_CALLBACK theCallback);

        /** \brief Set the callback to be called when a new frame is acquired from the camera and the image is received.
        \param[in] theCallback The pointer to the function to be executed
        */
        API void setImageCallback(IMAGE_CALLBACK theCallback);

        /** \brief Set the callback to be called when a blink is done by the user
        \param[in] theCallback The pointer to the function to be executed
        */
        API void setBlinkCallback(BLINK_CALLBACK theCallback);

        /** \brief Set the callback to be called when a dwell is done by the user
        \param[in] theCallback The pointer to the function to be executed
        */
        API void setDwellCallback(DWELL_CALLBACK theCallback);
        //@}



        //! \name Deprecated functions
        //@{

        /** \brief Check that the API ends after calling stop()
        \return True when the API stops properly. False when the API is still stopping.
        */
        API bool isApplicationEnded();

        /** \brief Switch on the eye tracker leds
        \return True when the action is done properly. False when it's not.
        */
        API bool switchOnLedLights();

        /** \brief Switch off the eye tracker leds
        \return True when the action is done properly. False when it's not.
        */
        API bool switchOffLedLights();

        /** \brief Check the eye tracker leds status
        \return True when the leds are on, false when the leds are off.
        */
        API bool areLedLightsOn();

        /** \brief Set the license code when the license system is not based on an activation file
        \param[in] license license code 
        */
        API void setLicense(const char* license);
        
        //@}









    }
}
#endif //IRISBONDAPI_H
