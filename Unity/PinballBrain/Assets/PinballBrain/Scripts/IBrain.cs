﻿using System;

namespace PinballBrain {
    public interface IBrain {
        void AddDisposable(IDisposable disposable);

        void ActivateSolenoid(byte solenoid);
        void ActivateSolenoid(byte solenoid, short ms);
        /// <summary>
        /// Deactivate the led
        /// </summary>
        /// <param name="led"></param>
        void DeactivateLED(short led);

        IDisposable ConnectSwitchToAction(short switchID, Action<short> action);

        /// <summary>
        /// If switch is active, solenoid will be activated. If switch is inactive, solenoid will be deactivated. Be careful to not destroy the solenoid by overheating it.
        /// </summary>
        /// <param name="switchID"></param>
        /// <param name="solenoid"></param>
        IDisposable ConnectSwitchToSolenoid(short switchID, byte solenoid);

        /// <summary>
        /// If switch is active, the solenoid will be activated for a set amount of time (in ms)
        /// </summary>
        /// <param name="switchID"></param>
        /// <param name="solenoid"></param>
        /// <param name="ms"></param>
        IDisposable ConnectSwitchToSolenoid(short switchID, byte solenoid, short time);

        /// <summary>
        /// If switch is active, led will be activated. If switch is inactive, led will be deactivated
        /// </summary>
        /// <param name="switchID"></param>
        /// <param name="led"></param>
        IDisposable ConnectSwitchToLED(short switchID, short led, byte red, byte green, byte blue);

        /// <summary>
        /// If switch is triggered, led will be set to active, inactive or blink. If set to blink and blinkAmount is 0, led will blink forever. Otherwise it will blink "blinkAmount" of times
        /// </summary>
        /// <param name="switchID"></param>
        /// <param name="led"></param>
        /// <param name="red"></param>
        /// <param name="green"></param>
        /// <param name="blue"></param>
        /// <param name="ledAction"></param>
        /// <param name="blinkInterval"></param>
        /// <param name="blinkAmount"></param>
        IDisposable ConnectSwitchToLED(short switchID, short led, byte red, byte green, byte blue, LEDAction ledAction = LEDAction.Activate, short blinkInterval = 100, byte blinkAmount = 0);

        /// <summary>
        /// If switch is triggered, led will be set to active, inactive or blink. If set to blink and blinkAmount is 0, led will blink forever. Otherwise it will blink "blinkAmount" of times
        /// </summary>
        /// <param name="switchID"></param>
        /// <param name="led"></param>
        /// <param name="ledData"></param>
        /// <param name="ledAction"></param>
        /// <returns></returns>
        IDisposable ConnectSwitchToLED(short switchID, short led, LEDData ledData, LEDAction ledAction);

        /// <summary>
        /// If switch is active, led will blink for blinkAmount of times. If blinkAmount is 0, led will blink until switch is inactive.
        /// </summary>
        /// <param name="switchID"></param>
        /// <param name="led"></param>
        /// <param name="interval"></param>
        /// <param name="blinkAmount"></param>
        IDisposable ConnectSwitchToLEDBlink(short switchID, short led, byte red, byte green, byte blue, short interval, byte blinkAmount = 0);

        /// <summary>
        /// Connects switch to led, using LEDData. If ledData.amount is 0, led will blink until switch is inactive.
        /// </summary>
        /// <param name="switchID"></param>
        /// <param name="ledData"></param>
        IDisposable ConnectSwitchToLEDBlink(short switchID, short led, LEDData ledData);

        IDisposable ConnectSwitchToScoreIncrease(short switchID, int score);
        IDisposable ConnectSwitchToScoreIncrease(short switchID, int score, int player);

        IObservable<PlayerScoreChangedEvent> OnCurrentPlayerScoreChanged();
        IObservable<PlayerScoreChangedEvent> OnPlayerScoreChanged(int playerID);

        int GetPlayerScore(int playerID);
        void IncreaseCurrentPlayerScore(int score);
        void IncreasePlayerScore(int player, int score);
    }

    public class LEDData {
        public byte red;
        public byte green;
        public byte blue;
        public short interval;
        public byte amount;

        public LEDData(byte red, byte green, byte blue, short interval, byte amount) {
            this.red = red;
            this.green = green;
            this.blue = blue;
            this.interval = interval;
            this.amount = amount;
        }
    }

    public class PlayerScoreChangedEvent {
        public int playerID;
        public int currentScore;
    }

    public enum LEDAction {
        Activate,
        Deactivate,
        Blink
    }
}