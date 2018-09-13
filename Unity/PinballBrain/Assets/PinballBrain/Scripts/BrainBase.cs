using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;

namespace PinballBrain {

    public interface IBrain {
        void AddDisposable(IDisposable disposable);
    }

    public abstract class BrainBase : IBrain, IDisposable {
        protected IBrainInterface BrainInterface { get; private set; }

        List<int> playerScore;
        int currentPlayer = 0;
        int maxPlayers = 4;
        public class PlayerScoreChangedEvent {
            public int playerID;
            public int currentScore;
        }
        ReactiveCommand<PlayerScoreChangedEvent> PlayerScoreChangedCmd;
        List<PlayerScoreChangedEvent> playerScoreChangedEventCache;

        CompositeDisposable disposables;

        public BrainBase() {
            Debug.Log("Creating brain.");
            disposables = new CompositeDisposable();

            playerScore = new List<int>(maxPlayers);
            playerScoreChangedEventCache = new List<PlayerScoreChangedEvent>(maxPlayers);

            for (int i=0; i<maxPlayers; ++i) {
                playerScoreChangedEventCache.Add(new PlayerScoreChangedEvent() { playerID = i });
            }

            BrainInterface = CreateBrainInterface();
            //TODO: Add code to check if interface is ready/had errors
            OnBrainInterfaceReady();
        }

        protected virtual void OnBrainInterfaceReady() {
            SetupRules();
        }

        protected virtual void OnBrainInterfaceFail(byte errorCode) { }

        protected abstract IBrainInterface CreateBrainInterface();
        protected abstract void SetupRules();

        public void AddDisposable(IDisposable disposable) {
            disposables.Add(disposable);
        }

        public void Dispose() {
            BrainInterface.Dispose();
            disposables.Dispose();
        }


        /// <summary>
        /// If switch is active, solenoid will be activated. If switch is inactive, solenoid will be deactivated
        /// </summary>
        /// <param name="switchID"></param>
        /// <param name="solenoid"></param>
        protected void ConnectSwitchToSolenoid(short switchID, byte solenoid) {
            BrainInterface.OnSwitchActive(switchID).Subscribe(e => BrainInterface.ActivateSolenoid(solenoid)).AddTo(this);
            BrainInterface.OnSwitchInactive(switchID).Subscribe(e => BrainInterface.DeactivateSolenoid(solenoid)).AddTo(this);
        }

        /// <summary>
        /// If switch is active, the solenoid will be activated for a set amount of time (in ms)
        /// </summary>
        /// <param name="switchID"></param>
        /// <param name="solenoid"></param>
        /// <param name="ms"></param>
        protected void ConnectSwitchToSolenoid(short switchID, byte solenoid, short time) {
            BrainInterface.OnSwitchActive(switchID).Subscribe(e => BrainInterface.ActivateSolenoid(solenoid, time)).AddTo(this);
        }

        /// <summary>
        /// If switch is active, led will be activated. If switch is inactive, led will be deactivated
        /// </summary>
        /// <param name="switchID"></param>
        /// <param name="led"></param>
        protected void ConnectSwitchToLED(short switchID, short led, byte red, byte green, byte blue) {
            BrainInterface.OnSwitchActive(switchID).Subscribe(e => BrainInterface.ActivateLED(led, red, green, blue)).AddTo(this);
            BrainInterface.OnSwitchInactive(switchID).Subscribe(e => BrainInterface.DeactivateLED(led)).AddTo(this);
        }

        /// <summary>
        /// If switch is active, led will blink for blinkAmount of times. If blinkAmount is 0, led will blink until switch is inactive.
        /// </summary>
        /// <param name="switchID"></param>
        /// <param name="led"></param>
        /// <param name="interval"></param>
        /// <param name="blinkAmount"></param>
        protected void ConnectSwitchToLEDBlink(short switchID, short led, byte red, byte green, byte blue, short interval, byte blinkAmount = 0) {
            if (blinkAmount == 0) {
                BrainInterface.OnSwitchActive(switchID).Subscribe(e => BrainInterface.BlinkLED(led, red, green , blue, interval)).AddTo(this);
                BrainInterface.OnSwitchInactive(switchID).Subscribe(e => BrainInterface.DeactivateLED(led)).AddTo(this);
            } else {
                BrainInterface.OnSwitchActive(switchID).Subscribe(e => BrainInterface.BlinkLED(led, red, green, blue, interval, blinkAmount)).AddTo(this);
            }
        }

        public class LEDBlinkData {
            public byte red;
            public byte green;
            public byte blue;
            public short interval;
            public byte amount;

            public LEDBlinkData(byte red, byte green, byte blue, short interval, byte amount) {
                this.red = red;
                this.green = green;
                this.blue = blue;
                this.interval = interval;
                this.amount = amount;
            }
        }

        /// <summary>
        /// Connects switch to led, using LEDBlinkData
        /// </summary>
        /// <param name="switchID"></param>
        /// <param name="ledBlinkData"></param>
        protected void ConnectSwitchToLEDBlink(short switchID, short led, LEDBlinkData ledBlinkData) {
            ConnectSwitchToLEDBlink(switchID, led, ledBlinkData.red, ledBlinkData.green, ledBlinkData.blue, ledBlinkData.interval, ledBlinkData.amount);
        }

        /// <summary>
        /// Gets the score of a player
        /// </summary>
        /// <param name="playerID"></param>
        /// <returns></returns>
        protected int GetPlayerScore(int playerID) {
            return playerScore[playerID];
        }

        /// <summary>
        /// Increase the score of the current player
        /// </summary>
        /// <param name="score"></param>
        protected void IncreaseCurrentPlayerScore(int score) {
            IncreasePlayerScore(currentPlayer, score);
        }

        /// <summary>
        /// Increase the score of a player
        /// </summary>
        /// <param name="player"></param>
        /// <param name="score"></param>
        protected void IncreasePlayerScore(int player, int score) {
            playerScore[player] += score;
            playerScoreChangedEventCache[player].currentScore = playerScore[player];
            PlayerScoreChangedCmd.Execute(playerScoreChangedEventCache[player]);
        }

        /// <summary>
        /// Increases the current player score when switch is triggered
        /// </summary>
        /// <param name="switchID"></param>
        /// <param name="score"></param>
        protected void ConnectSwitchToScoreIncrease(short switchID, int score) {
            BrainInterface.OnSwitchActive(switchID).Subscribe(e => IncreaseCurrentPlayerScore(score)).AddTo(this);
        }

        protected void ConnectSwitchToScoreIncrease(short switchID, int score, int player) {
            BrainInterface.OnSwitchActive(switchID).Subscribe(e => IncreasePlayerScore(player, score)).AddTo(this);
        }

        protected IObservable<PlayerScoreChangedEvent> OnPlayerScoreChanged(int playerID) {
            return PlayerScoreChangedCmd.Where(v => v.playerID == playerID);
        }

        protected IObservable<PlayerScoreChangedEvent> OnCurrentPlayerScoreChanged() {
            return PlayerScoreChangedCmd.Where(v => v.playerID == currentPlayer);
        }
    }
}

namespace UniRx {
    public static partial class DisposableExtensions {
        /// <summary>Add disposable(self) to CompositeDisposable(or other ICollection). Return value is self disposable.</summary>

        // BrainBase
        public static T AddTo<T>(this T disposable, PinballBrain.IBrain brain)
            where T : IDisposable {
            if (disposable == null) throw new ArgumentNullException("disposable");
            if (brain == null) throw new ArgumentNullException("brain");

            brain.AddDisposable(disposable);

            return disposable;
        }
    }
}