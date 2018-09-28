using System;
using UnityEngine;
using System.Collections.Generic;
using UniRx;
using System.Collections;

namespace PinballBrain {
    
    public class KeyboardBrainInterface : IBrainInterface {

        Subject<short> switchActive = new Subject<short>();
        Subject<short> switchInactive = new Subject<short>();

        const byte SOLENOID_ACTIVATE = 1; // + solenoid (byte)
        byte[] solenoidActivateMsg = new byte[2];
        const byte SOLENOID_DEACTIVATE = 2; // + solenoid (byte)
        byte[] solenoidDeactivateMsg = new byte[2];
        const byte SOLENOID_TRIGGER = 3; // + solenoid (byte) + time (2 bytes)
        byte[] solenoidTriggerMsg = new byte[4];

        const byte LED_ACTIVATE = 10; // + led (2 bytes) + color (3 bytes)
        byte[] ledActivateMsg = new byte[6];
        const byte LED_DEACTIVATE = 11; // + led (2 bytes)
        byte[] ledDeactivateMsg = new byte[3];
        const byte LED_BLINK = 12; // + led (2 bytes) + color (3 bytes) +  interval (2 bytes)
        byte[] ledBlinkMsg = new byte[8];
        const byte LED_BLINK_AMOUNT = 13; // + led (2 bytes) + color (3 bytes) + interval (2 bytes) + times (byte)
        byte[] ledBlinkAmountMsg = new byte[9];

        const byte DISPLAY_SET_IMAGE = 20; // + display (byte) + image (2 byte)
        byte[] displaySetImageMsg = new byte[4];
        const byte DISPLAY_CLEAR_IMAGE = 21; // + display (byte)
        byte[] displayClearImageMsg = new byte[2];
        const byte DISPLAY_ANIMATION_LOOP = 22; // + display (byte) + animation (2 bytes)
        byte[] displayAnimationLoopMsg = new byte[4];
        const byte DISPLAY_ANIMATION_STOP = 23; // + display (byte)
        byte[] displayAnimationStopMsg = new byte[2];
        const byte DISPLAY_ANIMATION_PLAY_ONCE = 24; // + display (byte) + animation (2 bytes)
        byte[] displayAnimationPlayOnceMsg = new byte[4];

        const byte SWITCH_ACTIVE = 30; // 2 bytes switch id
        const byte SWITCH_INACTIVE = 31; // 2 bytes switch id


        AsyncSerial serialPort;

        public KeyboardBrainInterface() {
            Debug.Log("Creating keyboard brain interface.");

            /*
            //Setup serial port
            this.serialPort = new GodSerialPort("COM" + serialPort, 9600);
            //this.serialPort.UseDataReceived(true, OnSerialDataReceived); //Not working in unity since there is a bug in the ports event not being called.
            //this.serialPort.
            Observable.EveryUpdate().Subscribe(e => this.serialPort.)
            
            //Open serial port
            bool flag = this.serialPort.Open();
            if (!flag) {
                //Could not open port
                Debug.LogError("Could not open serial port "+serialPort);
                this.serialPort.Close();
                return;
            }
            */
            
            Observable.EveryUpdate().Subscribe(e => {
                CheckInput();
            });
        }

        void CheckInput() {
            if (Input.GetKeyDown(KeyCode.LeftControl)) {
                switchActive.OnNext(0);
            }
            if (Input.GetKeyDown(KeyCode.RightControl)) {
                switchActive.OnNext(1);
            }
            if (Input.GetKeyDown(KeyCode.Space)) {
                switchActive.OnNext(2);
            }
            if (Input.GetKeyDown(KeyCode.Alpha1)) {
                switchActive.OnNext(3);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2)) {
                switchActive.OnNext(4);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3)) {
                switchActive.OnNext(5);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4)) {
                switchActive.OnNext(6);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5)) {
                switchActive.OnNext(7);
            }


            if (Input.GetKeyUp(KeyCode.LeftControl)) {
                switchInactive.OnNext(0);
            }
            if (Input.GetKeyUp(KeyCode.RightControl)) {
                switchInactive.OnNext(1);
            }
            if (Input.GetKeyUp(KeyCode.Space)) {
                switchInactive.OnNext(2);
            }
            if (Input.GetKeyUp(KeyCode.Alpha1)) {
                switchInactive.OnNext(3);
            }
            if (Input.GetKeyUp(KeyCode.Alpha2)) {
                switchInactive.OnNext(4);
            }
            if (Input.GetKeyUp(KeyCode.Alpha3)) {
                switchInactive.OnNext(5);
            }
            if (Input.GetKeyUp(KeyCode.Alpha4)) {
                switchInactive.OnNext(6);
            }
            if (Input.GetKeyUp(KeyCode.Alpha5)) {
                switchInactive.OnNext(7);
            }

        }
        

        void Write(byte[] bytes) {
            Debug.Log(bytes[0]);
        }

        /// <summary>
        /// Subscribe to a switch being activated (Button pressed, rollover switch active, etc.) .
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="action"></param>
        public IObservable<short> OnSwitchActive(short switchID) {
            return switchActive.Where(v => v == switchID);
        }

        /// <summary>
        /// Subscribe to a switch being deactivated (Button no longer pressed, rollover switch no longer hit, etc.)
        /// </summary>
        /// <param name="switchID"></param>
        /// <returns></returns>
        public IObservable<short> OnSwitchInactive(short switchID) {
            return switchInactive.Where(v => v == switchID);
        }


        /// <summary>
        /// Activate a solenoid for a specific amount of time (in ms). Time is send to the arduino, so if the pc/program crashes, the arduino will still deactivate the solenoid.
        /// </summary>
        /// <param name="solenoid"></param>
        /// <param name="ms"></param>
        public void ActivateSolenoid(byte solenoid, short ms) {
            solenoidTriggerMsg[0] = SOLENOID_TRIGGER;
            solenoidTriggerMsg[1] = solenoid;
            byte[] msBytes = ByteHelper.ToBytes(ms);
            solenoidTriggerMsg[2] = msBytes[0];
            solenoidTriggerMsg[3] = msBytes[1];

            Write(solenoidTriggerMsg);
        }

        /// <summary>
        /// Activate a solenoid. You have to manually deactivate it again. Be careful to not destroy the solenoid by overheating it.
        /// </summary>
        /// <param name="solenoid"></param>
        public void ActivateSolenoid(byte solenoid) {
            solenoidActivateMsg[0] = SOLENOID_ACTIVATE;
            solenoidActivateMsg[1] = solenoid;

            Write(solenoidActivateMsg);
        }

        /// <summary>
        /// Deactivate a solenoid.
        /// </summary>
        /// <param name="solenoid"></param>
        public void DeactivateSolenoid(byte solenoid) {
            solenoidDeactivateMsg[0] = SOLENOID_DEACTIVATE;
            solenoidDeactivateMsg[0] = solenoid;

            Write(solenoidDeactivateMsg);
        }

        /// <summary>
        /// Activate an LED
        /// </summary>
        /// <param name="led"></param>
        public void ActivateLED(short led, byte red, byte green, byte blue) {
            byte[] ledByte = ByteHelper.ToBytes(led);

            ledActivateMsg[0] = LED_ACTIVATE;
            ledActivateMsg[1] = ledByte[0];
            ledActivateMsg[2] = ledByte[1];
            ledActivateMsg[3] = red;
            ledActivateMsg[4] = green;
            ledActivateMsg[5] = blue;

            Write(ledActivateMsg);
        }

        /// <summary>
        /// Deactivate an LED
        /// </summary>
        /// <param name="led"></param>
        public void DeactivateLED(int led) {
            byte[] ledByte = ByteHelper.ToBytes(led);

            ledDeactivateMsg[0] = LED_DEACTIVATE;
            ledDeactivateMsg[1] = ledByte[0];
            ledDeactivateMsg[2] = ledByte[1];

            Write(ledDeactivateMsg);
        }

        /// <summary>
        /// Set an LED to blink mode until it is deactivated again. Interval is in ms.
        /// </summary>
        /// <param name="led"></param>
        public void BlinkLED(short led, byte red, byte green, byte blue, short interval) {
            byte[] ledByte = ByteHelper.ToBytes(led);
            byte[] intervalByte = ByteHelper.ToBytes(interval);

            ledBlinkMsg[0] = LED_BLINK;
            ledBlinkMsg[1] = ledByte[0];
            ledBlinkMsg[2] = ledByte[1];
            ledBlinkMsg[3] = red;
            ledBlinkMsg[4] = green;
            ledBlinkMsg[5] = blue;
            ledBlinkMsg[6] = intervalByte[0];
            ledBlinkMsg[7] = intervalByte[1];

            Write(ledBlinkMsg);
        }

        /// <summary>
        /// Set an LED to blink mode until it is deactivated again. LED will blink for blinkAmount times, then deactivate. Interval is in ms.
        /// </summary>
        /// <param name="led"></param>
        /// <param name="interval"></param>
        public void BlinkLED(short led, byte red, byte green, byte blue, short interval, byte blinkAmount) {
            byte[] ledByte = ByteHelper.ToBytes(led);
            byte[] intervalByte = ByteHelper.ToBytes(interval);

            ledBlinkAmountMsg[0] = LED_BLINK_AMOUNT;
            ledBlinkAmountMsg[1] = ledByte[0];
            ledBlinkAmountMsg[2] = ledByte[1];
            ledBlinkAmountMsg[3] = red;
            ledBlinkAmountMsg[4] = green;
            ledBlinkAmountMsg[5] = blue;
            ledBlinkAmountMsg[6] = intervalByte[0];
            ledBlinkAmountMsg[7] = intervalByte[1];
            ledBlinkAmountMsg[8] = blinkAmount;

            Write(ledBlinkAmountMsg);

        }

        /// <summary>
        /// Set the image of a display, loading an image from SDCard on the arduino side. Will stop animation on that display.
        /// </summary>
        /// <param name="tft"></param>
        /// <param name="image"></param>
        public void SetTFTImage(byte display, short image) {
            byte[] imageByte = ByteHelper.ToBytes(image);

            displaySetImageMsg[0] = DISPLAY_SET_IMAGE;
            displaySetImageMsg[1] = display;
            displaySetImageMsg[2] = imageByte[0];
            displaySetImageMsg[3] = imageByte[1];

            Write(displaySetImageMsg);
        }

        /// <summary>
        /// Clear the image of a display, making it black
        /// </summary>
        /// <param name="display"></param>
        public void ClearTFTImage(byte display) {
            displayClearImageMsg[0] = DISPLAY_CLEAR_IMAGE;
            displayClearImageMsg[1] = display;

            Write(displayClearImageMsg);
        }

        /// <summary>
        /// Start an animation on a TFT, loading the animation from a SDCard on the arduino side.
        /// </summary>
        /// <param name="tft"></param>
        /// <param name="animation"></param>
        public void LoopTFTAnimation(byte display, short animation) {
            byte[] animationByte = ByteHelper.ToBytes(animation);

            displayAnimationLoopMsg[0] = DISPLAY_ANIMATION_LOOP;
            displayAnimationLoopMsg[1] = display;
            displayAnimationLoopMsg[2] = animationByte[0];
            displayAnimationLoopMsg[3] = animationByte[1];

            Write(displayAnimationLoopMsg);
        }

        /// <summary>
        /// Stop a tft animation.
        /// </summary>
        /// <param name="tft"></param>
        public void StopTFTAnimation(byte display) {
            displayAnimationStopMsg[0] = DISPLAY_ANIMATION_STOP;
            displayAnimationStopMsg[1] = display;

            Write(displayAnimationStopMsg);
        }

        /// <summary>
        /// Play the animation once on the display.
        /// </summary>
        /// <param name="display"></param>
        /// <param name="animation"></param>
        public void PlayTFTAnimation(byte display, short animation) {
            byte[] animationByte = ByteHelper.ToBytes(animation);

            displayAnimationPlayOnceMsg[0] = DISPLAY_ANIMATION_PLAY_ONCE;
            displayAnimationPlayOnceMsg[1] = display;
            displayAnimationPlayOnceMsg[2] = animationByte[0];
            displayAnimationPlayOnceMsg[3] = animationByte[1];

            Write(displayAnimationPlayOnceMsg);
        }

        public void Dispose() {
            this.serialPort.Dispose();
        }
    }
}