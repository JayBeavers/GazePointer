#ifndef DEFINITIONS_H
#define DEFINITIONS_H

namespace IrisbondAPI
{
/** \brief Result of the API initialization returned by start()
*/
enum class START_STATUS{ 
    START_OK,       ///< The API starts properly.
    NO_CAMERA,      ///< The camera is not detected on the computer. It can be not connected or the drivers are missing.
    CAMERA_IN_USE,  ///< The camera is being used by other program and the API can't access it.
    CAMERA_ERROR,   ///< The camera is connected but can't start because of a different reason.
    QT_GUI_ERROR,   ///< Deprecated, the API is not based on Qt anymore.
    INVALID_LICENSE ///< Deprecated.
};

/** \brief Result of the calibration process returned by waitForCalibrationToEnd()
*/
enum class CALIBRATION_STATUS{ 
    CALIBRATION_TIMEOUT,            ///< The calibration has not finished but the timeout is reached
    CALIBRATION_ENDED_ALT_F4,       ///< The calibration is cancelled by the user with Alt+F4
    CALIBRATION_ENDED_ESCAPE,       ///< The calibration is cancelled by the user with Esc key
    CALIBRATION_FINISHED,           ///< The calibration has finished
    CALIBRATION_CANCELLED,          ///< The calibration is cancelled programmatically
    CALIBRATION_FAILED_NO_CAMERA    ///< The calibration has found a camera error
};

/** \brief Eye codification used by setUserEyeControlMode()
*/
enum class CONTROLLING_EYE{ 
    CONTROL_LEFT,       ///< Only the left eye is required
    CONTROL_RIGHT,      ///< Only the right eye is required
    CONTROL_ANY,        ///< One eye is required, any of them
    CONTROL_BOTH_ALWAYS ///< Both eyes are required
};

#ifndef DOXYGEN_SHOULD_SKIP_THIS
/** \brief Action selected by the user in the calibration review panel returned by showCalibrationResultsReviewPanel
    Not public yet, skip documentation
*/
enum class CALIBRATION_ACTION{ 
    RESTORE = 0,    ///< Restore previous calibration
    ACCEPT = 1,     ///< Accept current calibration
    IMPROVE = 2     ///< Improve current calibration
};

#endif /* DOXYGEN_SHOULD_SKIP_THIS */



//! \name Callbacks
//@{

/** \brief Function to be called when a new image is processed and a new gaze point is received.
    \param timestamp            The timestamp that identifies the current gaze point. In milliseconds, from epoch.
    \param mouseX               X coordinate of the gaze point. In pixels, where 0 is the left side of the image.
    \param mouseY               Y coordinate of the gaze point. In pixels, where 0 is the top of the image.
    \param mouseRawX            X coordinate of the raw gaze point, this means, without any filtering. In pixels. For research only.
    \param mouseRawY            Y coordinate of the raw gaze point, this means, without any filtering. In pixels. For research only.
    \param screenWidth          Screen width considered by the API to estimate the gaze point. In pixels.
    \param screenHeight         Screen height considered by the API to estimate the gaze point. In pixels.
    \param leftEyeDetected      True when the left eye is detected in the current gaze point. False when it is not.
    \param rightEyeDetected     True when the right eye is detected in the current gaze point. False when it is not.
    \param imageWidth           Camera image width. In pixels.
    \param imageHeight          Camera image height. In pixels.
    \param leftEyeX             X coordinate of the left eye in the camera image. Normalized between 0 and 1, where 0 is the left side of the image, and 1 is the right side.
    \param leftEyeY             Y coordinate of the left eye in the camera image. Normalized between 0 and 1, where 0 is the top of the image, and 1 is the bottom side.
    \param leftEyeSize          Left eye pupil size. In pixels.
    \param rightEyeX            X coordinate of the right eye in the camera image. Normalized between 0 and 1, where 0 is the left side of the image, and 1 is the right side.
    \param rightEyeY            Y coordinate of the right eye in the camera image. Normalized between 0 and 1, where 0 is the top of the image, and 1 is the bottom side.
    \param rightEyeSize         Right eye pupil size. In pixels.
    \param distanceFactor       Tells the distance between the eye tracker and the user. Between -1 and 1, where -1 means too far, 1 is too close, and 0 is the ideal distance. Recommended [-0.5, 0.5]
*/
typedef void(*DATA_CALLBACK)(
    long long timestamp, 
    float mouseX, float mouseY,
    float mouseRawX, float mouseRawY,
    int screenWidth, int screenHeight,
    bool leftEyeDetected, bool rightEyeDetected,
    int imageWidth, int imageHeight,
    float leftEyeX, float leftEyeY, float leftEyeSize,
    float rightEyeX, float rightEyeY, float rightEyeSize,
    float distanceFactor);


/** \brief Function to be called when the calibration finishes, and the global calibration results are received.
    \param leftPrecisionError       Left eye precision, in pixels
    \param leftAccuracyError        Left eye accuracy, in pixels
    \param rightPrecisionError      Right eye precision, in pixels
    \param rightAccuracyError       Right eye accuracy, in pixels
    \param combinedPrecisionError   Both eyes precision, in pixels
    \param combinedAccuracyError    Both eyes accuracy, in pixels
    \param cancelled                True when the calibration is cancelled. Results must be ignored when cancelled is true.
*/
typedef void(*CALIBRATION_RESULTS_CALLBACK)(
    const double leftPrecisionError,
    const double leftAccuracyError,
    const double rightPrecisionError,
    const double rightAccuracyError,
    const double combinedPrecisionError,
    const double combinedAccuracyError,
    bool cancelled);

/** \brief Function to be called when the calibration finishes, and the detailed calibration results (point by point) are received.
    \param nPoints              The number of calibration points
    \param xCoords              A vector with the X coordinates of the calibration points. Normalized between 0 (left of the screen) and 1 (right of the screen)
    \param yCoords              A vector with the Y coordinates of the calibration points. Normalized between 0 (top of the screen) and 1 (bottom of the screen)
    \param leftErrorsPx         A vector with the left eye errors for all the points. In pixels.
    \param rightErrorsPx        A vector with the right eye errors for all the points. In pixels.
    \param combinedErrorsPx     A vector with the combined eye errors for all the points. In pixels.
    \param leftErrorsNorm       A vector with the left eye errors for all the points. Normalized between 0 (perfect point) and 1 (should be improved)
    \param rightErrorsNorm      A vector with the right eye errors for all the points. Normalized between 0 (perfect point) and 1 (should be improved)
    \param combinedErrorsNorm   A vector with the combined eye errors for all the points. Normalized between 0 (perfect point) and 1 (should be improved)
*/
typedef void(*CALIBRATION_RESULTS_POINTS_CALLBACK)(
    int nPoints, double* xCoords, double* yCoords, 
    double* leftErrorsPx, double* rightErrorsPx, double* combinedErrorsPx,
    double* leftErrorsNorm, double* rightErrorsNorm, double* combinedErrorsNorm);

/** \brief Function to be called when the calibration is cancelled.
*/
typedef void(*CALIBRATION_CANCELLED_CALLBACK)();

/** \brief Function to be called during the calibration to know the calibration target position and state
    \param pointX           X coordinate of the target. In pixels.
    \param pointY           Y coordinate of the target. In pixels.
    \param screenWidth      Screen width considered by the API to locate the target. In pixels.
    \param screenHeight     Screen heigth considered by the API to locate the target. In pixels.
    \param fixated          True when the target is fixated in the point (pointX, pointY), false when it is travelling towards that point.
*/
typedef void(*CALIBRATION_TARGET_CALLBACK)(double pointX, double pointY, int screenWidth, int screenHeight, bool fixated);

/** \brief Function to be called during the calibration when the user is not detected in one calibration point and decide to retry that point or cancel calibration
    \return True to try again, false to cancel calibration
*/
typedef bool(*CALIBRATION_POINT_ERROR_CALLBACK)();

/** \brief Function to be called during the calibration when a new frame is acquired from the camera and the image is received.
    \param data         A pointer to the image raw data
    \param rows         The number of rows in the image
    \param cols         The number of columns in the image
    \param channels     The number of channels in the image. Typically 1.
    \param timestamp    The timestamp that identifies the current gaze point. In milliseconds, from epoch.
*/
typedef void(*IMAGE_CALLBACK)(char* data, int rows, int cols, int channels, long long timestamp);

/** \brief Function to be called during the calibration when a blink is done by the user
    \param mouseX           X coordinate of the blink event. In pixels.
    \param mouseY           Y coordinate of the blink event. In pixels.
    \param screenWidth      Screen width considered by the API to locate the target. In pixels.
    \param screenHeight     Screen heigth considered by the API to locate the target. In pixels.
*/
typedef void(*BLINK_CALLBACK)(int mouseX, int mouseY, int screenWidth, int screenHeight);

/** \brief Function to be called during the calibration when a dwell is done by the user
    \param mouseX           X coordinate of the dwell event. In pixels.
    \param mouseY           Y coordinate of the dwell event. In pixels.
    \param screenWidth      Screen width considered by the API to locate the target. In pixels.
    \param screenHeight     Screen heigth considered by the API to locate the target. In pixels.
*/
typedef void(*DWELL_CALLBACK)(int mouseX, int mouseY, int screenWidth, int screenHeight);


}
#endif //DEFINITIONS_H
