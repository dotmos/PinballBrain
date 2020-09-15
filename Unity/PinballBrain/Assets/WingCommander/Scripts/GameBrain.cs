using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PinballBrain;
using UniRx;
using System;


public class GameBrain : BrainBase {

    // Switches -------------------------------------------------------------------------------------------------
    public class Switches {
        public const short FLIPPER_LEFT = 800;
        public const short FLIPPER_RIGHT = 900;
        
        public const short SLINGSHOT_LEFT = 8;
        public const short SLINGSHOT_RIGHT = 7;

        public const short BUMPER_1 = 510;
        public const short BUMPER_2 = 511;
        public const short BUMPER_3 = 512;

        public const short TARGETBANK_LOWERLEFT_1 = 1;
        public const short TARGETBANK_LOWERLEFT_2 = 2;
        public const short TARGETBANK_LOWERLEFT_3 = 0;
        public const short DROPTARGETBANK_LOWERLEFT_1 = 4;
        public const short DROPTARGETBANK_LOWERLEFT_2 = 3;
        public const short DROPTARGETBANK_LOWERLEFT_3 = 6;
        public const short DROPTARGETBANK_LOWERLEFT_4 = 5;

        public const short TARGETBANK_LOWERRIGHT_1 = 16;
        public const short TARGETBANK_LOWERRIGHT_2 = 17;

        public const short TARGETBANK_UPPERRIGHT_1 = 14;
        public const short TARGETBANK_UPPERRIGHT_2 = 13;
        public const short DROPTARGETBANK_UPPERRIGHT_1 = 12;
        public const short DROPTARGETBANK_UPPERRIGHT_2 = 11;
        public const short DROPTARGETBANK_UPPERRIGHT_3 = 15;

        public const short TARGETBANK_UPPERCENTER_1 = 9;
        public const short TARGETBANK_UPPERCENTER_2 = 10;

        public const short KICKER_UPPERRIGHT = 400;
        public const short KICKER_CENTERRAMP = 600;

        public const short KICKBACK_UPPERLEFT = 300;

        public const short LEFTTUNNEL = 255;

        public const short BUMPERLANE_1 = 255;
        public const short BUMPERLANE_2 = 255;
        public const short BUMPERLANE_3 = 255;

        public const short PLUNGER_KICK = 60;
        public const short PLUNGER_LANE = 255;
        public const short PLUNGER_LANE_EXIT = 255;

        public const short BALL_OUT = 255;

        public const short DEBUG_PLUNGER_FEED = 500;
    }

    // Solenoids --------------------------------------------------------------------------------------------------
    public class Solenoids {
        /// <summary>
        /// Trigger time for solenoids in ms. 1-5 seem to be good values. Even the big ones are firing with that
        /// </summary>
        public const byte solenoidTriggerTime = 2;


        public const byte FLIPPER_LEFT = 0;
        public const byte FLIPPER_RIGHT = 5;

        public const byte SLINGSHOT_LEFT = 3;
        public const byte SLINGSHOT_RIGHT = 2;
        public const short slingshotActiveTime = 2;

        public const byte BUMPER_1 = 10; //top left
        public const byte BUMPER_2 = 9; //top right
        public const byte BUMPER_3 = 11; //bottom
        public const short bumperSolenoidActiveTime = 2;

        public const byte DROPTARGET_LOWERLEFT = 14;

        public const byte DROPTARGET_UPPERRIGHT = 12;

        public const byte KICKER_UPPERRIGHT = 8;
        public const byte KICKER_CENTERRAMP = 15; // DEPRECATE?

        public const byte PLUNGER_FEED = 4; //4
        public const byte PLUNGER_KICKER = 7;
        public const byte plungerKickerSolenoidActiveTime = 25;

        public const byte KICKBACK_UPPERLEFT = 13;

        //1 = n/a (after left flipper)
        //6 = n/a (after right flipper)
    }

    // LEDS ---------------------------------------------------------------------------------------------------------
    public class LEDs {
        public LEDData SLINGSHOT_DATA = new LEDData(255, 255, 255, 100, 1);
        public const short SLINGSHOT_LEFT = 10;
        public const short SLINGSHOT_RIGHT = 11;

        public LEDData BUMPER_DATA = new LEDData(0, 64, 255, 25, 3);
        public const short BUMPER_1 = 10;
        public const short BUMPER_2 = 11;
        public const short BUMPER_3 = 12;

        public LEDData TARGETBANK_LOWERLEFT_DATA = new LEDData(255, 64, 0, 25, 3);
        public LEDData TARGETBANK_LOWERLEFT_ALLHIT_DATA = new LEDData(255, 128, 64, 25, 10);
        public const short TARGETBANK_LOWERLEFT_1 = 0;
        public const short TARGETBANK_LOWERLEFT_2 = 1;
        public const short TARGETBANK_LOWERLEFT_3 = 2;

        public LEDData TARGETBANK_LOWERRIGHT_ALLHIT_DATA = new LEDData(64, 255, 128, 100, 3);
        public LEDData TARGETBANK_LOWERRIGHT_DATA = new LEDData(0, 255, 64, 100, 1);
        public const short TARGETBANK_LOWERRIGHT_1 = 3;
        public const short TARGETBANK_LOWERRIGHT_2 = 4;

        public LEDData TARGETBANK_UPPERRIGHT_DATA = new LEDData(255, 64, 0, 25, 3);
        public LEDData TARGETBANK_UPPERRIGHT_ALLHIT_DATA = new LEDData(255, 128, 64, 25, 10);
        public const short TARGETBANK_UPPERRIGHT_1 = 10;
        public const short TARGETBANK_UPPERRIGHT_2 = 11;

        public LEDData TARGETBANK_UPPERCENTER_DATA = new LEDData(255, 0, 0, 100, 1);
        public const short TARGETBANK_UPPERCENTER_1 = 10;
        public const short TARGETBANK_UPPERCENTER_2 = 11;

        public LEDData KICKER_UPPERRIGHT_BALLIN_DATA = new LEDData(255, 128, 128, 0, 0);
        public LEDData KICKER_UPPERRIGHT_BALLRELEASE_DATA = new LEDData(255, 255, 255, 100, 2);
        public const short KICKER_UPPERRIGHT = 31;

        public LEDData KICKER_CENTERRAMP_BALLIN_DATA = new LEDData(255, 128, 128, 0, 0);
        public LEDData KICKER_CENTERRAMP_BALLRELEASE_DATA = new LEDData(255, 255, 255, 100, 2);
        public const short KICKER_CENTERRAMP = 32;


        public LEDData LEFTTUNNEL_DATA = new LEDData(64, 255, 64, 100, 3);
        public const short LEFTTUNNEL = 40;

        public LEDData BUMPERLANE_DATA = new LEDData(255, 0, 0, 0, 0);
        public const short BUMPERLANE_1 = 50;
        public const short BUMPERLANE_2 = 51;
        public const short BUMPERLANE_3 = 52;

        public LEDData KICKBACK_UPPERLEFT_BALLIN_DATA = new LEDData(255, 128, 128, 0, 0);
        public LEDData KICKBACK_UPPERLEFT_BALLRELEASE_DATA = new LEDData(255, 255, 255, 100, 2);
        public const short KICKBACK_UPPERLEFT = 60;
    }
    LEDs ledData = new LEDs();


    // TARGET BANKS -------------------------------------------------------------------------------------------------
    public TargetBank lowerRightTargetBank { get; private set; }
    public TargetBank lowerLeftDropTargetbank { get; private set; }
    public TargetBank upperRightDropTargetbank { get; private set; }
    public TargetBank upperCenterTargetbank { get; private set; }
    public TargetBank bumperLaneTargetbank { get; private set; }

    // DISPLAYS -----------------------------------------------------------------------------------------------------
    /// <summary>
    /// Display near lower left target bank.
    /// </summary>
    const byte DISPLAY_TARGETBANK_LOWERLEFT = 0;
    /// <summary>
    /// Explosion animation ID
    /// </summary>
    const short DISPLAY_ANIMATION_EXPLODE = 5;

    // EVENTS -------------------------------------------------------------------------------------------------------
    public class Event_StartVideoMode {
        public enum VideoModeEnum {
            AsteroidEvade,
            KilrathiKill
        }
        public VideoModeEnum videoMode;
    }
    Event_StartVideoMode event_StartVideoMode = new Event_StartVideoMode();


    // Logic variables -----------------------------------------------------------------------------------------------
    /// <summary>
    /// Current number of balls in play
    /// </summary>
    ReactiveProperty<int> BallsInPlay = new ReactiveProperty<int>(0);

    /// <summary>
    /// Current bumper multiülicator
    /// </summary>
    int currentBumperMultiplicator = 1;
    /// <summary>
    /// Maximum bumper multiplicator
    /// </summary>
    readonly int maxBumperMultiplicator = 32;

    /// <summary>
    /// Whether or not there is currently a ball in the plungerlane, ready to get kicked
    /// </summary>
    bool ballInPlungerLane = false;


    // Logic --------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Create brain interface
    /// </summary>
    /// <returns></returns>
    protected override IBrainInterface CreateBrainInterface() {
            return new ArduinoBrainInterface(3);
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
        /*
        //Just for testing: Start videomode once player reaches score of 10000
        OnCurrentPlayerScoreChanged().Subscribe(e => {
            if (e.currentScore >= 10000) {
                PublishBrainEvent(event_StartVideoMode);
            }
        }).AddTo(this);
        */

        //Debug stuff
        //BrainInterface.OnSwitchActive(1).Subscribe(e => Debug.Log("Switch " + e + " active")).AddTo(this);
        //BrainInterface.OnSwitchInactive(1).Subscribe(e => Debug.Log("Switch " + e + " inactive")).AddTo(this);
        //ConnectSwitchToLED(0, 0, 64, 16, 0);
        //ConnectSwitchToLEDBlink(1, 1, 64, 0, 16, 40, 3);
        //ConnectSwitchToLEDBlink(3, 2, 0, 64, 16, 500, 0);
        //ConnectSwitchToLEDBlink(0, 0, SLINGSHOT_DATA);
        //ConnectSwitchToLEDBlink(1, 1, TARGETBANK_LOWERRIGHT_DATA);
        //ConnectSwitchToLEDBlink(2, 2, BUMPER_DATA);
        //ConnectSwitchToLEDBlink(3, 3, TARGETBANK_LOWERLEFT_DATA);

        /*
        BrainInterface.OnSwitchActive(0).Subscribe(e => {
            //BrainInterface.SetTFTImage(0, e);
            BrainInterface.PlayTFTAnimation(0, 4);
        }).AddTo(this);
        BrainInterface.OnSwitchActive(1).Subscribe(e => {
            //BrainInterface.SetTFTImage(0, e);
            BrainInterface.PlayTFTAnimation(0, 5);
        }).AddTo(this);
        BrainInterface.OnSwitchActive(2).Subscribe(e => {
            //BrainInterface.SetTFTImage(0, e);
            BrainInterface.LoopTFTAnimation(0, 6);
        }).AddTo(this);
        BrainInterface.OnSwitchActive(3).Subscribe(e => {
            //BrainInterface.SetTFTImage(0, e);
            BrainInterface.LoopTFTAnimation(0, 3);
        }).AddTo(this);
        */





        
        //Setup flippers ----------------------------------------------------------------------------------------------------------------------
        //Left
        OnSwitchActive(Switches.FLIPPER_LEFT, (switchID) => {
            if (BallsInPlay.Value > 0) {
                ActivateSolenoid(Solenoids.FLIPPER_LEFT);
            }
        }).AddTo(this);
        OnSwitchInactive(Switches.FLIPPER_LEFT, (switchID) => {
            DeactivateSolenoid(Solenoids.FLIPPER_LEFT);
        }).AddTo(this);
        //Right
        OnSwitchActive(Switches.FLIPPER_RIGHT, (switchID) => {
            if (BallsInPlay.Value > 0) {
                ActivateSolenoid(Solenoids.FLIPPER_RIGHT);
            }
        }).AddTo(this);
        OnSwitchInactive(Switches.FLIPPER_RIGHT, (switchID) => {
            DeactivateSolenoid(Solenoids.FLIPPER_RIGHT);
        }).AddTo(this);

        
        //Setup slingshots ----------------------------------------------------------------------------------------------------------------------
        ConnectSwitchToSolenoid(Switches.SLINGSHOT_LEFT, Solenoids.SLINGSHOT_LEFT, Solenoids.slingshotActiveTime);
        ConnectSwitchToSolenoid(Switches.SLINGSHOT_RIGHT, Solenoids.SLINGSHOT_RIGHT, Solenoids.slingshotActiveTime);

        ConnectSwitchToLED(Switches.SLINGSHOT_LEFT, LEDs.SLINGSHOT_LEFT, ledData.SLINGSHOT_DATA, LEDAction.Blink);
        ConnectSwitchToLED(Switches.SLINGSHOT_RIGHT, LEDs.SLINGSHOT_RIGHT, ledData.SLINGSHOT_DATA, LEDAction.Blink);

        ConnectSwitchToScoreIncrease(Switches.SLINGSHOT_LEFT, 100).AddTo(this);
        ConnectSwitchToScoreIncrease(Switches.SLINGSHOT_RIGHT, 100).AddTo(this);
        
        //Setup bumpers ----------------------------------------------------------------------------------------------------------------------
        ConnectSwitchToSolenoid(Switches.BUMPER_1, Solenoids.BUMPER_1, Solenoids.bumperSolenoidActiveTime);
        ConnectSwitchToSolenoid(Switches.BUMPER_2, Solenoids.BUMPER_2, Solenoids.bumperSolenoidActiveTime);
        ConnectSwitchToSolenoid(Switches.BUMPER_3, Solenoids.BUMPER_3, Solenoids.bumperSolenoidActiveTime);
        
        ConnectSwitchToLED(Switches.BUMPER_1, LEDs.BUMPER_1, ledData.BUMPER_DATA, LEDAction.Blink);
        ConnectSwitchToLED(Switches.BUMPER_2, LEDs.BUMPER_2, ledData.BUMPER_DATA, LEDAction.Blink);
        ConnectSwitchToLED(Switches.BUMPER_3, LEDs.BUMPER_3, ledData.BUMPER_DATA, LEDAction.Blink);

        ConnectSwitchToScoreIncrease(Switches.BUMPER_1, () => { return 100 * currentBumperMultiplicator; }).AddTo(this);
        ConnectSwitchToScoreIncrease(Switches.BUMPER_2, () => { return 100 * currentBumperMultiplicator; }).AddTo(this);
        ConnectSwitchToScoreIncrease(Switches.BUMPER_3, () => { return 100 * currentBumperMultiplicator; }).AddTo(this);
        
        //Setup lower left droptarget targetbank ----------------------------------------------------------------------------------------------------------------------
        lowerLeftDropTargetbank = new TargetBank(this, 
            new List<short>() {
                Switches.TARGETBANK_LOWERLEFT_1,
                Switches.TARGETBANK_LOWERLEFT_2,
                Switches.TARGETBANK_LOWERLEFT_3,
                Switches.DROPTARGETBANK_LOWERLEFT_1,
                Switches.DROPTARGETBANK_LOWERLEFT_2,
                Switches.DROPTARGETBANK_LOWERLEFT_3,
                Switches.DROPTARGETBANK_LOWERLEFT_4
            },
            true, 2000);
        
        //Setup bank LEDs
        lowerLeftDropTargetbank.OnUniqueTargetHit(Switches.TARGETBANK_LOWERLEFT_1).Subscribe(e => SetLED(LEDs.TARGETBANK_LOWERLEFT_1, LEDAction.Activate, ledData.TARGETBANK_LOWERLEFT_DATA)).AddTo(this);
        lowerLeftDropTargetbank.OnUniqueTargetHit(Switches.TARGETBANK_LOWERLEFT_2).Subscribe(e => SetLED(LEDs.TARGETBANK_LOWERLEFT_2, LEDAction.Activate, ledData.TARGETBANK_LOWERLEFT_DATA)).AddTo(this);
        lowerLeftDropTargetbank.OnUniqueTargetHit(Switches.TARGETBANK_LOWERLEFT_3).Subscribe(e => SetLED(LEDs.TARGETBANK_LOWERLEFT_3, LEDAction.Activate, ledData.TARGETBANK_LOWERLEFT_DATA)).AddTo(this);
        //Setup bank scoring
        lowerLeftDropTargetbank.OnTargetHit(Switches.TARGETBANK_LOWERLEFT_1).Subscribe(e => IncreaseCurrentPlayerScore(250)).AddTo(this);
        lowerLeftDropTargetbank.OnTargetHit(Switches.TARGETBANK_LOWERLEFT_2).Subscribe(e => IncreaseCurrentPlayerScore(250)).AddTo(this);
        lowerLeftDropTargetbank.OnTargetHit(Switches.TARGETBANK_LOWERLEFT_3).Subscribe(e => IncreaseCurrentPlayerScore(250)).AddTo(this);
        lowerLeftDropTargetbank.OnTargetHit(Switches.DROPTARGETBANK_LOWERLEFT_1).Subscribe(e => IncreaseCurrentPlayerScore(250)).AddTo(this);
        lowerLeftDropTargetbank.OnTargetHit(Switches.DROPTARGETBANK_LOWERLEFT_2).Subscribe(e => IncreaseCurrentPlayerScore(250)).AddTo(this);
        lowerLeftDropTargetbank.OnTargetHit(Switches.DROPTARGETBANK_LOWERLEFT_3).Subscribe(e => IncreaseCurrentPlayerScore(250)).AddTo(this);
        lowerLeftDropTargetbank.OnTargetHit(Switches.DROPTARGETBANK_LOWERLEFT_4).Subscribe(e => IncreaseCurrentPlayerScore(250)).AddTo(this);
        
        //On all bank targets hit
        lowerLeftDropTargetbank.OnAllTargetsHit().DelayFrame(1).Subscribe(e => {
            //Play explosion animation on TFT near targetbank
            //TODO: Display animation takes A LOT of performance on the arduino. (LED flashing takes A LOT longer when animation is playing.)
            //BrainInterface.PlayTFTAnimation(DISPLAY_TARGETBANK_LOWERLEFT, DISPLAY_ANIMATION_EXPLODE);

            //Flash targetbank LEDS
            SetLED(LEDs.TARGETBANK_LOWERLEFT_1, LEDAction.Blink, ledData.TARGETBANK_LOWERLEFT_ALLHIT_DATA);
            SetLED(LEDs.TARGETBANK_LOWERLEFT_2, LEDAction.Blink, ledData.TARGETBANK_LOWERLEFT_ALLHIT_DATA);
            SetLED(LEDs.TARGETBANK_LOWERLEFT_3, LEDAction.Blink, ledData.TARGETBANK_LOWERLEFT_ALLHIT_DATA);

            //Add points
            IncreaseCurrentPlayerScore(2500);

            //TODO: Play audio
        }).AddTo(this);

        //On bank reset
        lowerLeftDropTargetbank.OnReset().Subscribe(e => {
            //Reset physical droptargets by activating the solenoid
            ActivateSolenoid(Solenoids.DROPTARGET_LOWERLEFT, Solenoids.solenoidTriggerTime);
        }).AddTo(this);

        /*
        //Setup lower right targetbank -----------------------------------------------------------------------------------------------------------------------
        lowerRightTargetBank = new TargetBank(this,
            new List<short> {
                Switches.TARGETBANK_LOWERRIGHT_1,
                Switches.TARGETBANK_LOWERRIGHT_2
            },
            true, 300);
        //Setup targets
        lowerRightTargetBank.OnUniqueTargetHit(Switches.TARGETBANK_LOWERRIGHT_1).Subscribe(e => SetLED(LEDs.TARGETBANK_LOWERRIGHT_1, LEDAction.Activate, ledData.TARGETBANK_LOWERRIGHT_DATA)).AddTo(this);
        lowerRightTargetBank.OnTargetHit(Switches.TARGETBANK_LOWERRIGHT_1).Subscribe(e => IncreaseCurrentPlayerScore(100)).AddTo(this);
        lowerRightTargetBank.OnUniqueTargetHit(Switches.TARGETBANK_LOWERRIGHT_2).Subscribe(e => SetLED(LEDs.TARGETBANK_LOWERRIGHT_2, LEDAction.Activate, ledData.TARGETBANK_LOWERRIGHT_DATA)).AddTo(this);
        lowerRightTargetBank.OnTargetHit(Switches.TARGETBANK_LOWERRIGHT_2).Subscribe(e => IncreaseCurrentPlayerScore(100)).AddTo(this);

        lowerRightTargetBank.OnAllTargetsHit().Subscribe(e => {
            SetLED(LEDs.TARGETBANK_LOWERRIGHT_1, LEDAction.Blink, ledData.TARGETBANK_LOWERRIGHT_ALLHIT_DATA);
            SetLED(LEDs.TARGETBANK_LOWERRIGHT_2, LEDAction.Blink, ledData.TARGETBANK_LOWERRIGHT_ALLHIT_DATA);
            IncreaseCurrentPlayerScore(400);

            //TODO: Play animation & audio
        }).AddTo(this);
        */


        //Setup upper right droptarget targetbank -------------------------------------------------------------------------------------------------------------------------
        upperRightDropTargetbank = new TargetBank(this,
            new List<short>() {
                Switches.TARGETBANK_UPPERRIGHT_1,
                Switches.TARGETBANK_UPPERRIGHT_2,
                Switches.DROPTARGETBANK_UPPERRIGHT_1,
                Switches.DROPTARGETBANK_UPPERRIGHT_2,
                Switches.DROPTARGETBANK_UPPERRIGHT_3
            },
            true, 500);
        //Setup leds
        upperRightDropTargetbank.OnUniqueTargetHit(Switches.TARGETBANK_UPPERRIGHT_1).Subscribe(e => SetLED(LEDs.TARGETBANK_UPPERRIGHT_1, LEDAction.Activate, ledData.TARGETBANK_UPPERRIGHT_DATA)).AddTo(this);
        upperRightDropTargetbank.OnUniqueTargetHit(Switches.TARGETBANK_UPPERRIGHT_2).Subscribe(e => SetLED(LEDs.TARGETBANK_UPPERRIGHT_2, LEDAction.Activate, ledData.TARGETBANK_UPPERRIGHT_DATA)).AddTo(this);
        //Setup scoring
        upperRightDropTargetbank.OnTargetHit(Switches.TARGETBANK_UPPERRIGHT_1).Subscribe(e => IncreaseCurrentPlayerScore(250)).AddTo(this);
        upperRightDropTargetbank.OnTargetHit(Switches.TARGETBANK_UPPERRIGHT_2).Subscribe(e => IncreaseCurrentPlayerScore(250)).AddTo(this);
        upperRightDropTargetbank.OnTargetHit(Switches.DROPTARGETBANK_UPPERRIGHT_1).Subscribe(e => IncreaseCurrentPlayerScore(250)).AddTo(this);
        upperRightDropTargetbank.OnTargetHit(Switches.DROPTARGETBANK_UPPERRIGHT_2).Subscribe(e => IncreaseCurrentPlayerScore(250)).AddTo(this);
        upperRightDropTargetbank.OnTargetHit(Switches.DROPTARGETBANK_UPPERRIGHT_3).Subscribe(e => IncreaseCurrentPlayerScore(250)).AddTo(this);

        upperRightDropTargetbank.OnAllTargetsHit().Subscribe(e => {
            //Flash targetbank LEDS
            SetLED(LEDs.TARGETBANK_UPPERRIGHT_1, LEDAction.Blink, ledData.TARGETBANK_UPPERRIGHT_ALLHIT_DATA);
            SetLED(LEDs.TARGETBANK_UPPERRIGHT_1, LEDAction.Blink, ledData.TARGETBANK_UPPERRIGHT_ALLHIT_DATA);

            //Add points
            IncreaseCurrentPlayerScore(2500);

            //TODO: Play animation + audio
        }).AddTo(this);

        upperRightDropTargetbank.OnReset().Subscribe(e => {
            //Reset physical droptargets by activating the solenoid
            ActivateSolenoid(Solenoids.DROPTARGET_UPPERRIGHT, Solenoids.solenoidTriggerTime);
        }).AddTo(this);

        /*
        //Setup upper center targetbank --------------------------------------------------------------------------------------------------------------------------
        upperCenterTargetbank = new TargetBank(this,
            new List<short>() {
                Switches.TARGETBANK_UPPERCENTER_1,
                Switches.TARGETBANK_UPPERCENTER_2
            },
            true, 100);
        //Activate LEDS
        upperCenterTargetbank.OnUniqueTargetHit(Switches.TARGETBANK_UPPERCENTER_1).Subscribe(e => SetLED(LEDs.TARGETBANK_UPPERCENTER_1, LEDAction.Activate, ledData.TARGETBANK_UPPERCENTER_DATA)).AddTo(this);
        upperCenterTargetbank.OnUniqueTargetHit(Switches.TARGETBANK_UPPERCENTER_2).Subscribe(e => SetLED(LEDs.TARGETBANK_UPPERCENTER_2, LEDAction.Activate, ledData.TARGETBANK_UPPERCENTER_DATA)).AddTo(this);
        upperCenterTargetbank.OnReset().Subscribe(e => {
            //Deactivate LEDS
            DeactivateLED(LEDs.TARGETBANK_UPPERCENTER_1);
            DeactivateLED(LEDs.TARGETBANK_UPPERCENTER_2);
        }).AddTo(this);
        */

        // Center ramp kicker -------------------------------------------------------------------------------------------------------------------------------------
        OnSwitchActive(Switches.KICKER_CENTERRAMP, (s) => {
            //Play sound

            //Add points
            IncreaseCurrentPlayerScore(1000);

            //Activate LED
            SetLED(LEDs.KICKER_CENTERRAMP, LEDAction.Activate, ledData.KICKER_CENTERRAMP_BALLIN_DATA);

            //activate kicker solenoid after a short amount of time
            Observable.Timer(TimeSpan.FromMilliseconds(1000)).Subscribe(e => {
                //Flash LED
                SetLED(LEDs.KICKER_CENTERRAMP, LEDAction.Blink, ledData.KICKER_CENTERRAMP_BALLRELEASE_DATA);

                //Solenoid
                ActivateSolenoid(Solenoids.KICKER_CENTERRAMP, Solenoids.solenoidTriggerTime);
            }).AddTo(this);
        }).AddTo(this);

        
        // Upperright kicker -------------------------------------------------------------------------------------------------------------------------------------
        OnSwitchActive(Switches.KICKER_UPPERRIGHT, (s) => {
            //Play sound

            //Add points
            IncreaseCurrentPlayerScore(1500);

            //Activate LED
            SetLED(LEDs.KICKER_UPPERRIGHT, LEDAction.Activate, ledData.KICKER_UPPERRIGHT_BALLIN_DATA);

            //activate kicker solenoid after a short amount of time
            Observable.Timer(TimeSpan.FromMilliseconds(1000)).Subscribe(e => {
                //Flash LED
                SetLED(LEDs.KICKER_UPPERRIGHT, LEDAction.Blink, ledData.KICKER_UPPERRIGHT_BALLRELEASE_DATA);

                //Solenoid
                ActivateSolenoid(Solenoids.KICKER_UPPERRIGHT, Solenoids.solenoidTriggerTime);
            }).AddTo(this);
        }).AddTo(this);

        /*
        // Setup left tunnel ----------------------------------------------------------------------------------------------------------------------
        ConnectSwitchToLED(Switches.LEFTTUNNEL, LEDs.LEFTTUNNEL, ledData.LEFTTUNNEL_DATA, LEDAction.Blink).AddTo(this);
        ConnectSwitchToScoreIncrease(Switches.LEFTTUNNEL, 500).AddTo(this);


        // Bumper lane targetbank ---------------------------------------------------------------------------------------------------------------------------------
        bumperLaneTargetbank = new TargetBank(this,
            new List<short>() {
                Switches.BUMPERLANE_1,
                Switches.BUMPERLANE_2,
                Switches.BUMPERLANE_3,
            }, true, 10);

        bumperLaneTargetbank.OnUniqueTargetHit(Switches.BUMPERLANE_1).Subscribe(e => SetLED(LEDs.BUMPERLANE_1, LEDAction.Activate, ledData.BUMPERLANE_DATA)).AddTo(this);
        bumperLaneTargetbank.OnUniqueTargetHit(Switches.BUMPERLANE_2).Subscribe(e => SetLED(LEDs.BUMPERLANE_1, LEDAction.Activate, ledData.BUMPERLANE_DATA)).AddTo(this);
        bumperLaneTargetbank.OnUniqueTargetHit(Switches.BUMPERLANE_3).Subscribe(e => SetLED(LEDs.BUMPERLANE_1, LEDAction.Activate, ledData.BUMPERLANE_DATA)).AddTo(this);
        bumperLaneTargetbank.OnTargetHit(Switches.BUMPERLANE_1).Subscribe(e => IncreaseCurrentPlayerScore(150)).AddTo(this);
        bumperLaneTargetbank.OnTargetHit(Switches.BUMPERLANE_2).Subscribe(e => IncreaseCurrentPlayerScore(150)).AddTo(this);
        bumperLaneTargetbank.OnTargetHit(Switches.BUMPERLANE_3).Subscribe(e => IncreaseCurrentPlayerScore(150)).AddTo(this);

        bumperLaneTargetbank.OnAllTargetsHit().Subscribe(e => {
            //Increase multiplicator
            currentBumperMultiplicator = Mathf.Min(currentBumperMultiplicator*2, maxBumperMultiplicator);

            IncreaseCurrentPlayerScore(500 * currentBumperMultiplicator);
        }).AddTo(this);
        */



        // Upper left kickback -------------------------------------------------------------------------------------------------------------------------------------
        OnSwitchActive(Switches.KICKBACK_UPPERLEFT, (s) => {
            //Play sound

            //Add points
            IncreaseCurrentPlayerScore(1000);

            //Activate LED
            SetLED(LEDs.KICKBACK_UPPERLEFT, LEDAction.Activate, ledData.KICKBACK_UPPERLEFT_BALLIN_DATA);

            //activate kicker solenoid after a short amount of time
            Observable.Timer(TimeSpan.FromMilliseconds(1000)).Subscribe(e => {
                //Flash LED
                SetLED(LEDs.KICKBACK_UPPERLEFT, LEDAction.Blink, ledData.KICKBACK_UPPERLEFT_BALLRELEASE_DATA);

                //Solenoid
                ActivateSolenoid(Solenoids.KICKBACK_UPPERLEFT, Solenoids.solenoidTriggerTime);
            }).AddTo(this);
        }).AddTo(this);


        //Setup ball feed and plunger ----------------------------------------------------------------------------------------------------------

        //Plunger feed debug
        OnSwitchActive(Switches.DEBUG_PLUNGER_FEED, (switchID) => {
            ActivateSolenoid(Solenoids.PLUNGER_FEED, Solenoids.solenoidTriggerTime);
        });

        //Plunger
        OnSwitchActive(Switches.PLUNGER_LANE, (switchID) => ballInPlungerLane = true).AddTo(this);
        OnSwitchInactive(Switches.PLUNGER_LANE, (switchID) => ballInPlungerLane = false).AddTo(this);
        OnSwitchActive(Switches.PLUNGER_KICK, (switchID) => {
            //if (ballInPlungerLane) ActivateSolenoid(Solenoids.PLUNGER_KICKER, Solenoids.plungerKickerSolenoidActiveTime);
            ActivateSolenoid(Solenoids.PLUNGER_KICKER, Solenoids.plungerKickerSolenoidActiveTime); 
        }).AddTo(this);

        
        //Next player -> reset playfield and variables. Feed new ball into plunger lane
        OnCurrentPlayerChanged().Subscribe(e => {
            //Reset targetbanks
            //bumperLaneTargetbank.ResetBank();
            lowerLeftDropTargetbank.ResetBank();
            //lowerRightTargetBank.ResetBank();
            //upperCenterTargetbank.ResetBank();
            upperRightDropTargetbank.ResetBank();

            //Reset variables
            currentBumperMultiplicator = 1;
            
            //trigger next player once all balls are out again
            BallsInPlay.Skip(1).Where(v => v == 0).Take(1).Subscribe(nextPlayerEvent => {
                NextPlayer();
            }).AddTo(this);
            
            //Feed ball to plunger lane
            ActivateSolenoid(Solenoids.PLUNGER_FEED, Solenoids.solenoidTriggerTime);
            BallsInPlay.Value++;
            

            //TODO: Change music

        }).AddTo(this);
        

        //New ball on playfield
        OnSwitchActive(Switches.PLUNGER_LANE_EXIT, (switchID) => {
            //TODO: Change music
        }).AddTo(this);

        //Ball out
        OnSwitchActive(Switches.BALL_OUT, (switchID) => {
            //TODO: Play ball out sound
            BallsInPlay.Value = Mathf.Max(BallsInPlay.Value - 1, 0);
        }).AddTo(this);
        
    }

    public override void StartNewGame() {
        base.StartNewGame();

        //lowerLeftDropTargetbank.ResetBank();
        //upperRightDropTargetbank.ResetBank();
    }
}