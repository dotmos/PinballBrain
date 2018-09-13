using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PinballBrain;
using UniRx;
using System;

namespace Example1 {
    public class GameBrain : BrainBase {
        protected override IBrainInterface CreateBrainInterface() {
            //Setup arduino brain interface on COM4
            return new ArduinoBrainInterface(4);
        }

        protected override void OnBrainInterfaceReady() {
            base.OnBrainInterfaceReady();

            Debug.Log("Brain ready!");
        }

        protected override void OnBrainInterfaceFail(byte errorCode) {
            base.OnBrainInterfaceFail(errorCode);

            Debug.Log("Brain failure. Code " + errorCode);
        }

        protected override void SetupRules() {
            //Some debug messages when pressing switch 1
            BrainInterface.OnSwitchActive(1).Subscribe(e => Debug.Log("Switch " + e + " active")).AddTo(this);
            BrainInterface.OnSwitchInactive(1).Subscribe(e => Debug.Log("Switch " + e + " inactive")).AddTo(this);
            //Activate LED 0 while switch 0 is active
            ConnectSwitchToLED(0, 0, 64, 16, 0);
            //Blink LED 1 three times with an interval of 40ms when switch 1 is triggered
            ConnectSwitchToLEDBlink(1, 1, 64, 0, 16, 40, 3);
            //Blink LED 2 with an interval of 500ms while switch 2 is active
            ConnectSwitchToLEDBlink(2, 2, 0, 64, 16, 500, 0);
            //Activate solenoid 0 for 200ms when switch 10 is triggered
            ConnectSwitchToSolenoid(10, 0, 200);
            //Activate solenoid 1 while switch 11 is active. Warning: Make sure you don't fry your solenoid!
            ConnectSwitchToSolenoid(11, 1);
        }
    }
}