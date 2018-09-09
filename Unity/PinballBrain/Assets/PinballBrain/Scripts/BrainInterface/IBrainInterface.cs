using System;
using UniRx;

namespace PinballBrain {
    public interface IBrainInterface : IDisposable {

        /// <summary>
        /// Subscribe to a switch being activated (Button pressed, rollover switch active, etc.) .
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="action"></param>
        IObservable<short> OnSwitchActive(short switchID);

        /// <summary>
        /// Subscribe to a switch being deactivated (Button no longer pressed, rollover switch no longer hit, etc.)
        /// </summary>
        /// <param name="switchID"></param>
        /// <returns></returns>
        IObservable<short> OnSwitchInactive(short switchID);


        /// <summary>
        /// Activate a solenoid for a specific amount of time (in ms). Time is send to the arduino, so if the pc/program crashes, the arduino will still deactivate the solenoid.
        /// </summary>
        /// <param name="solenoid"></param>
        /// <param name="ms"></param>
        void ActivateSolenoid(byte solenoid, short ms);

        /// <summary>
        /// Activate a solenoid. You have to manually deactivate it again. Be careful to not destroy the solenoid by overheating it.
        /// </summary>
        /// <param name="solenoid"></param>
        void ActivateSolenoid(byte solenoid);

        /// <summary>
        /// Deactivate a solenoid.
        /// </summary>
        /// <param name="solenoid"></param>
        void DeactivateSolenoid(byte solenoid);

        /// <summary>
        /// Activate an LED
        /// </summary>
        /// <param name="led"></param>
        void ActivateLED(short led);

        /// <summary>
        /// Deactivate an LED
        /// </summary>
        /// <param name="led"></param>
        void DeactivateLED(int led);

        /// <summary>
        /// Set an LED to blink mode until it is deactivated again. Interval is in ms.
        /// </summary>
        /// <param name="led"></param>
        void BlinkLED(short led, short interval);

        /// <summary>
        /// Set an LED to blink mode until it is deactivated again. LED will blink for blinkAmount times, then deactivate. Interval is in ms.
        /// </summary>
        /// <param name="led"></param>
        /// <param name="interval"></param>
        void BlinkLED(short led, short interval, byte blinkAmount);

        /// <summary>
        /// Set the image of a display, loading an image from SDCard on the arduino side. Will stop animation on that display.
        /// </summary>
        /// <param name="tft"></param>
        /// <param name="image"></param>
        void SetTFTImage(byte display, short image);

        /// <summary>
        /// Clear the image of a display, making it black
        /// </summary>
        /// <param name="display"></param>
        void ClearTFTImage(byte display);

        /// <summary>
        /// Start an animation on a TFT, loading the animation from a SDCard on the arduino side.
        /// </summary>
        /// <param name="tft"></param>
        /// <param name="animation"></param>
        void LoopTFTAnimation(byte display, short animation);

        /// <summary>
        /// Stop a tft animation.
        /// </summary>
        /// <param name="tft"></param>
        void StopTFTAnimation(byte display);

        /// <summary>
        /// Play the animation once on the display.
        /// </summary>
        /// <param name="display"></param>
        /// <param name="animation"></param>
        void PlayTFTAnimation(byte display, short animation);
    }
}
