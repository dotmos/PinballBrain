using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PinballBrain;
using UniRx;
using System;

public class GameBrain : BrainBase {

    //Solenoids
    const byte SOLENOID_FLIPPER_LEFT = 0;
    const byte SOLENOID_FLIPPER_RIGHT = 1;
    const byte SOLENOID_BUMPER_1 = 2;
    const byte SOLENOID_BUMPER_2 = 2;
    const byte SOLENOID_BUMPER_3 = 2;

    //Switches
    const short SWITCH_FLIPPER_LEFT = 0;
    const short SWITCH_FLIPPER_RIGHT = 1;

    const short bumperSwitchTime = 200;
    const short SWITCH_BUMPER_1 = 2;
    const short SWITCH_BUMPER_2 = 3;
    const short SWITCH_BUMPER_3 = 4;

    const short SWITCH_TARGET_1 = 10;
    const short SWITCH_TARGET_2 = 11;

    //LEDS
    const short targetLedBlinkInterval = 200;
    const byte targetLedBlinkAmount = 4;
    const short LED_TARGET_1 = 0;
    const short LED_TARGET_2 = 1;

    protected override IBrainInterface CreateBrainInterface() {
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
        BrainInterface.OnSwitchActive(1).Subscribe(e => Debug.Log("Switch " + e + " active")).AddTo(this);
        BrainInterface.OnSwitchInactive(1).Subscribe(e => Debug.Log("Switch " + e + " inactive")).AddTo(this);
        ConnectSwitchToLED(0, 0, 64, 16, 0);
        ConnectSwitchToLEDBlink(1, 1, 64, 0, 16, 40, 3);
        ConnectSwitchToLEDBlink(2, 2, 0, 64, 16, 500, 0);

        /*
        //Blink led 13
        bool isOn = false;
        Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(e => {
            isOn = !isOn;
            if (isOn) {
                BrainInterface.ActivateLED(13);
            } else {
                BrainInterface.DeactivateLED(13);
            }
        }).AddTo(this);
        */

        /*
        //Setup flippers
        ConnectSwitchToSolenoid(SWITCH_FLIPPER_LEFT, SOLENOID_FLIPPER_LEFT);
        ConnectSwitchToSolenoid(SWITCH_FLIPPER_RIGHT, SOLENOID_FLIPPER_RIGHT);

        //Setup bumpers
        ConnectSwitchToSolenoid(SWITCH_BUMPER_1, SOLENOID_BUMPER_1, bumperSwitchTime);
        ConnectSwitchToSolenoid(SWITCH_BUMPER_2, SOLENOID_BUMPER_2, bumperSwitchTime);
        ConnectSwitchToSolenoid(SWITCH_BUMPER_3, SOLENOID_BUMPER_3, bumperSwitchTime);

        //Setup targets
        ConnectSwitchToLEDBlink(SWITCH_TARGET_1, LED_TARGET_1, targetLedBlinkInterval, targetLedBlinkAmount);
        ConnectSwitchToLEDBlink(SWITCH_TARGET_2, LED_TARGET_2, targetLedBlinkInterval, targetLedBlinkAmount);
        */
    }
}