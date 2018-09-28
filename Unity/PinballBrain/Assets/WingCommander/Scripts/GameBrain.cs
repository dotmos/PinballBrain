using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PinballBrain;
using UniRx;
using System;

public class GameBrain : BrainBase {
    // Switches -------------------------------------------------------------------------------------------------
    const short SWITCH_FLIPPER_LEFT = 0;
    const short SWITCH_FLIPPER_RIGHT = 1;

    const short bumperSlingshotActiveTime = 200;
    const short SWITCH_SLINGSHOT_LEFT = 2;
    const short SWITCH_SLINGSHOT_RIGHT = 3;

    const short bumperSolenoidActiveTime = 200;
    const short SWITCH_BUMPER_1 = 2;
    const short SWITCH_BUMPER_2 = 3;
    const short SWITCH_BUMPER_3 = 4;

    const short SWITCH_TARGETBANK_LOWERLEFT_1 = 10;
    const short SWITCH_TARGETBANK_LOWERLEFT_2 = 11;
    const short SWITCH_TARGETBANK_LOWERLEFT_3 = 11;

    const short SWITCH_DROPTARGETBANK_LOWERLEFT_1 = 12;
    const short SWITCH_DROPTARGETBANK_LOWERLEFT_2 = 13;
    const short SWITCH_DROPTARGETBANK_LOWERLEFT_3 = 14;
    const short SWITCH_DROPTARGETBANK_LOWERLEFT_4 = 15;

    const short SWITCH_TARGETBANK_LOWERRIGHT_1 = 21;
    const short SWITCH_TARGETBANK_LOWERRIGHT_2 = 22;

    const short SWITCH_TARGETBANK_UPPERRIGHT_1 = 21;
    const short SWITCH_TARGETBANK_UPPERRIGHT_2 = 22;

    const short SWITCH_DROPTARGETBANK_UPPERRIGHT_1 = 23;
    const short SWITCH_DROPTARGETBANK_UPPERRIGHT_2 = 23;
    const short SWITCH_DROPTARGETBANK_UPPERRIGHT_3 = 23;

    const short SWITCH_TARGETBANK_UPPERCENTER_1 = 21;
    const short SWITCH_TARGETBANK_UPPERCENTER_2 = 22;

    // Solenoids --------------------------------------------------------------------------------------------------
    const byte SOLENOID_FLIPPER_LEFT = 0;
    const byte SOLENOID_FLIPPER_RIGHT = 1;

    const byte SOLENOID_SLINGSHOT_LEFT = 2;
    const byte SOLENOID_SLINGSHOT_RIGHT = 3;

    const byte SOLENOID_BUMPER_1 = 4;
    const byte SOLENOID_BUMPER_2 = 5;
    const byte SOLENOID_BUMPER_3 = 6;

    const byte SOLENOID_DROPTARGET_LOWERLEFT = 7;

    const byte SOLENOID_DROPTARGET_UPPERRIGHT = 8;

    // LEDS ---------------------------------------------------------------------------------------------------------
    LEDData LED_SLINGSHOT_DATA = new LEDData(255, 255, 255, 100, 1);
    const short LED_SLINGSHOT_LEFT = 10;
    const short LED_SLINGSHOT_RIGHT = 11;

    LEDData LED_BUMPER_DATA = new LEDData(0, 64, 255, 25, 3);
    const short LED_BUMPER_1 = 0;
    const short LED_BUMPER_2 = 1;
    const short LED_BUMPER_3 = 2;

    LEDData LED_TARGETBANK_LOWERLEFT_DATA = new LEDData(255, 64, 0, 25, 3);
    const short LED_TARGETBANK_LOWERLEFT_1 = 0;
    const short LED_TARGETBANK_LOWERLEFT_2 = 1;
    const short LED_TARGETBANK_LOWERLEFT_3 = 2;

    LEDData LED_TARGETBANK_LOWERRIGHT_DATA = new LEDData(0, 255, 64, 100, 1);
    const short LED_TARGETBANK_LOWERRIGHT_1 = 3;
    const short LED_TARGETBANK_LOWERRIGHT_2 = 4;

    LEDData LED_TARGETBANK_UPPERRIGHT_DATA = new LEDData(255, 64, 0, 25, 3);
    const short LED_TARGETBANK_UPPERRIGHT_1 = 0;
    const short LED_TARGETBANK_UPPERRIGHT_2 = 1;

    LEDData LED_TARGETBANK_UPPERCENTER_DATA = new LEDData(255, 0, 0, 100, 1);
    const short LED_TARGETBANK_UPPERCENTER_1 = 0;
    const short LED_TARGETBANK_UPPERCENTER_2 = 1;


    // TARGET BANKS -------------------------------------------------------------------------------------------------
    TargetBank lowerRightTargetBank;
    TargetBank lowerLeftDropTargetbank;
    TargetBank upperRightDropTargetbank;



    // Logic --------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Create brain interface
    /// </summary>
    /// <returns></returns>
    protected override IBrainInterface CreateBrainInterface() {
            return new ArduinoBrainInterface(4);
            //return new KeyboardBrainInterface();
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
        //Debug stuff
        BrainInterface.OnSwitchActive(1).Subscribe(e => Debug.Log("Switch " + e + " active")).AddTo(this);
        BrainInterface.OnSwitchInactive(1).Subscribe(e => Debug.Log("Switch " + e + " inactive")).AddTo(this);
        //ConnectSwitchToLED(0, 0, 64, 16, 0);
        //ConnectSwitchToLEDBlink(1, 1, 64, 0, 16, 40, 3);
        //ConnectSwitchToLEDBlink(3, 2, 0, 64, 16, 500, 0);
        ConnectSwitchToLEDBlink(0, 0, LED_SLINGSHOT_DATA);
        ConnectSwitchToLEDBlink(1, 1, LED_TARGETBANK_LOWERRIGHT_DATA);
        ConnectSwitchToLEDBlink(2, 2, LED_BUMPER_DATA);
        ConnectSwitchToLEDBlink(3, 3, LED_TARGETBANK_LOWERLEFT_DATA);

        BrainInterface.OnSwitchActive(0).Subscribe(e => {
            BrainInterface.SetTFTImage(0, e);
        }).AddTo(this);
        BrainInterface.OnSwitchActive(1).Subscribe(e => {
            BrainInterface.SetTFTImage(0, e);
        }).AddTo(this);
        BrainInterface.OnSwitchActive(2).Subscribe(e => {
            BrainInterface.SetTFTImage(0, e);
        }).AddTo(this);
        BrainInterface.OnSwitchActive(3).Subscribe(e => {
            BrainInterface.SetTFTImage(0, e);
        }).AddTo(this);

        
        //Setup flippers
        ConnectSwitchToSolenoid(SWITCH_FLIPPER_LEFT, SOLENOID_FLIPPER_LEFT);
        ConnectSwitchToSolenoid(SWITCH_FLIPPER_RIGHT, SOLENOID_FLIPPER_RIGHT);

        //Setup slingshots
        ConnectSwitchToSolenoid(SWITCH_SLINGSHOT_LEFT, SOLENOID_SLINGSHOT_LEFT, bumperSlingshotActiveTime);
        ConnectSwitchToSolenoid(SWITCH_SLINGSHOT_RIGHT, SOLENOID_SLINGSHOT_RIGHT, bumperSlingshotActiveTime);
        ConnectSwitchToLEDBlink(SWITCH_SLINGSHOT_LEFT, LED_SLINGSHOT_LEFT, LED_SLINGSHOT_DATA);
        ConnectSwitchToLEDBlink(SWITCH_SLINGSHOT_RIGHT, LED_SLINGSHOT_RIGHT, LED_SLINGSHOT_DATA);

        //Setup bumpers
        ConnectSwitchToSolenoid(SWITCH_BUMPER_1, SOLENOID_BUMPER_1, bumperSolenoidActiveTime);
        ConnectSwitchToSolenoid(SWITCH_BUMPER_2, SOLENOID_BUMPER_2, bumperSolenoidActiveTime);
        ConnectSwitchToSolenoid(SWITCH_BUMPER_3, SOLENOID_BUMPER_3, bumperSolenoidActiveTime);
        ConnectSwitchToLEDBlink(SWITCH_BUMPER_1, LED_BUMPER_1, LED_BUMPER_DATA);
        ConnectSwitchToLEDBlink(SWITCH_BUMPER_2, LED_BUMPER_2, LED_BUMPER_DATA);
        ConnectSwitchToLEDBlink(SWITCH_BUMPER_3, LED_BUMPER_3, LED_BUMPER_DATA);

        //Setup lower left targetbank
        ConnectSwitchToLED(SWITCH_TARGETBANK_LOWERLEFT_1, LED_TARGETBANK_LOWERLEFT_1, LED_TARGETBANK_LOWERLEFT_DATA, LEDAction.Activate);
        ConnectSwitchToLED(SWITCH_TARGETBANK_LOWERLEFT_2, LED_TARGETBANK_LOWERLEFT_2, LED_TARGETBANK_LOWERLEFT_DATA, LEDAction.Activate);
        ConnectSwitchToLED(SWITCH_TARGETBANK_LOWERLEFT_3, LED_TARGETBANK_LOWERLEFT_3, LED_TARGETBANK_LOWERLEFT_DATA, LEDAction.Activate);
        //Setup lower left drop targetbank
        lowerLeftDropTargetbank = new TargetBank(this, 
            new List<short>() {
                SWITCH_TARGETBANK_LOWERLEFT_1,
                SWITCH_TARGETBANK_LOWERLEFT_2,
                SWITCH_TARGETBANK_LOWERLEFT_3,
                SWITCH_DROPTARGETBANK_LOWERLEFT_1,
                SWITCH_DROPTARGETBANK_LOWERLEFT_2,
                SWITCH_DROPTARGETBANK_LOWERLEFT_3,
                SWITCH_DROPTARGETBANK_LOWERLEFT_4
            },
            true, 1000);
        lowerLeftDropTargetbank.OnReset().Subscribe(e => {
            ActivateSolenoid(SOLENOID_DROPTARGET_LOWERLEFT, 50);
            DeactivateLED(LED_TARGETBANK_LOWERLEFT_1);
            DeactivateLED(LED_TARGETBANK_LOWERLEFT_2);
            DeactivateLED(LED_TARGETBANK_LOWERLEFT_3);

            //TODO: Play animation + audio
        }).AddTo(this);

        //Setup lower right targetbank
        ConnectSwitchToLED(SWITCH_TARGETBANK_LOWERRIGHT_1, LED_TARGETBANK_LOWERRIGHT_1, LED_TARGETBANK_LOWERRIGHT_DATA, LEDAction.Activate);
        ConnectSwitchToLED(SWITCH_TARGETBANK_LOWERRIGHT_2, LED_TARGETBANK_LOWERRIGHT_2, LED_TARGETBANK_LOWERRIGHT_DATA, LEDAction.Activate);
        lowerRightTargetBank = new TargetBank(this,
            new List<short> {
                SWITCH_TARGETBANK_LOWERRIGHT_1,
                SWITCH_TARGETBANK_LOWERRIGHT_2
            },
            true, 100);
        lowerRightTargetBank.OnReset().Subscribe(e => {
            DeactivateLED(LED_TARGETBANK_LOWERRIGHT_1);
            DeactivateLED(LED_TARGETBANK_LOWERRIGHT_2);

            //TODO: Play animation + audio
        }).AddTo(this);

        //Setup upper right targetbank
        ConnectSwitchToLED(SWITCH_TARGETBANK_UPPERRIGHT_1, LED_TARGETBANK_UPPERRIGHT_1, LED_TARGETBANK_UPPERRIGHT_DATA, LEDAction.Activate);
        ConnectSwitchToLED(SWITCH_TARGETBANK_UPPERRIGHT_2, LED_TARGETBANK_UPPERRIGHT_2, LED_TARGETBANK_UPPERRIGHT_DATA, LEDAction.Activate);
        //Setup upper right drop targetbank
        upperRightDropTargetbank = new TargetBank(this,
            new List<short>() {
                SWITCH_TARGETBANK_UPPERRIGHT_1,
                SWITCH_TARGETBANK_UPPERRIGHT_2,
                SWITCH_DROPTARGETBANK_UPPERRIGHT_1,
                SWITCH_DROPTARGETBANK_UPPERRIGHT_2,
                SWITCH_DROPTARGETBANK_UPPERRIGHT_3
            },
            true, 1000);
        upperRightDropTargetbank.OnReset().Subscribe(e => {
            ActivateSolenoid(SOLENOID_DROPTARGET_UPPERRIGHT, 50);
            DeactivateLED(LED_TARGETBANK_UPPERRIGHT_1);
            DeactivateLED(LED_TARGETBANK_UPPERRIGHT_2);

            //TODO: Play animation + audio
        }).AddTo(this);

        //Setup upper center targetbank
        ConnectSwitchToLEDBlink(SWITCH_TARGETBANK_UPPERCENTER_1, LED_TARGETBANK_UPPERCENTER_1, LED_TARGETBANK_UPPERCENTER_DATA);
        ConnectSwitchToLEDBlink(SWITCH_TARGETBANK_UPPERCENTER_2, LED_TARGETBANK_UPPERCENTER_2, LED_TARGETBANK_UPPERCENTER_DATA);
        
    }
}