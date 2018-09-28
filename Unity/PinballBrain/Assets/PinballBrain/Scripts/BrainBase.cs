using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;

namespace PinballBrain {

    public abstract class BrainBase : IBrain, IDisposable {
        protected IBrainInterface BrainInterface { get; private set; }

        List<int> playerScore;
        int currentPlayer = 0;
        int maxPlayers = 4;
        
        ReactiveCommand<PlayerScoreChangedEvent> PlayerScoreChangedCmd;
        List<PlayerScoreChangedEvent> playerScoreChangedEventCache;
        CompositeDisposable disposables;

        public BrainBase() {
            Debug.Log("Creating brain.");
            disposables = new CompositeDisposable();

            playerScore = new List<int>(maxPlayers) { 0, 0, 0, 0 };
            playerScoreChangedEventCache = new List<PlayerScoreChangedEvent>(maxPlayers);
            PlayerScoreChangedCmd = new ReactiveCommand<PlayerScoreChangedEvent>();

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
        /// Activate a solenoid. Be careful to not destroy the solenoid by overheating it.
        /// </summary>
        /// <param name="solenoid"></param>
        public void ActivateSolenoid(byte solenoid) {
            BrainInterface.ActivateSolenoid(solenoid);
        }

        /// <summary>
        /// Activate a solenoid for a specific amount of time (in milliseconds)
        /// </summary>
        /// <param name="solenoid"></param>
        /// <param name="ms"></param>
        public void ActivateSolenoid(byte solenoid, short ms) {
            BrainInterface.ActivateSolenoid(solenoid, ms);
        }

        /// <summary>
        /// Deactivate the led
        /// </summary>
        /// <param name="led"></param>
        public void DeactivateLED(short led) {
            BrainInterface.DeactivateLED(led);
        }

        /// <summary>
        /// Deactivate solenoid
        /// </summary>
        /// <param name="solenoid"></param>
        public void DeactivateSolenoid(byte solenoid) {
            BrainInterface.DeactivateSolenoid(solenoid);
        }

        /// <summary>
        /// If switch is active, action will be triggered
        /// </summary>
        /// <param name="switchID"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public IDisposable ConnectSwitchToAction(short switchID, Action<short> action) {
            return BrainInterface.OnSwitchActive(switchID).Subscribe(e => action(e)).AddTo(this);
        }

        /// <summary>
        /// If switch is active, solenoid will be activated. If switch is inactive, solenoid will be deactivated. Be careful to not destroy the solenoid by overheating it.
        /// </summary>
        /// <param name="switchID"></param>
        /// <param name="solenoid"></param>
        public IDisposable ConnectSwitchToSolenoid(short switchID, byte solenoid) {
            CompositeDisposable disposable = new CompositeDisposable();
            disposable.Add( BrainInterface.OnSwitchActive(switchID).Subscribe(e => BrainInterface.ActivateSolenoid(solenoid)).AddTo(this));
            disposable.Add( BrainInterface.OnSwitchInactive(switchID).Subscribe(e => BrainInterface.DeactivateSolenoid(solenoid)).AddTo(this));

            return disposable;
        }

        /// <summary>
        /// If switch is active, the solenoid will be activated for a set amount of time (in ms)
        /// </summary>
        /// <param name="switchID"></param>
        /// <param name="solenoid"></param>
        /// <param name="ms"></param>
        public IDisposable ConnectSwitchToSolenoid(short switchID, byte solenoid, short time) {
            return BrainInterface.OnSwitchActive(switchID).Subscribe(e => BrainInterface.ActivateSolenoid(solenoid, time)).AddTo(this);
        }

        /// <summary>
        /// If switch is active, led will be activated. If switch is inactive, led will be deactivated
        /// </summary>
        /// <param name="switchID"></param>
        /// <param name="led"></param>
        public IDisposable ConnectSwitchToLED(short switchID, short led, byte red, byte green, byte blue) {
            CompositeDisposable disposable = new CompositeDisposable();
            disposable.Add( BrainInterface.OnSwitchActive(switchID).Subscribe(e => BrainInterface.ActivateLED(led, red, green, blue)).AddTo(this));
            disposable.Add( BrainInterface.OnSwitchInactive(switchID).Subscribe(e => BrainInterface.DeactivateLED(led)).AddTo(this));

            return disposable;
        }



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
        /// <returns></returns>
        public IDisposable ConnectSwitchToLED(short switchID, short led, byte red, byte green, byte blue, LEDAction ledAction = LEDAction.Activate, short blinkInterval = 100, byte blinkAmount = 0) {
            if (ledAction == LEDAction.Activate) {
                return BrainInterface.OnSwitchActive(switchID).Subscribe(e => BrainInterface.ActivateLED(led, red, green, blue)).AddTo(this);
            } else if (ledAction == LEDAction.Blink) {
                //return BrainInterface.OnSwitchActive(switchID).Subscribe(e => BrainInterface.BlinkLED(led, red, green, blue, blinkInterval)).AddTo(this);
                if (blinkAmount == 0) {
                    return BrainInterface.OnSwitchActive(switchID).Subscribe(e => BrainInterface.BlinkLED(led, red, green, blue, blinkInterval)).AddTo(this);
                } else {
                    return BrainInterface.OnSwitchActive(switchID).Subscribe(e => BrainInterface.BlinkLED(led, red, green, blue, blinkInterval, blinkAmount)).AddTo(this);
                }
            } else {
                return BrainInterface.OnSwitchActive(switchID).Subscribe(e => BrainInterface.DeactivateLED(led)).AddTo(this);
            }
        }

        /// <summary>
        /// If switch is triggered, led will be set to active, inactive or blink. If set to blink and blinkAmount is 0, led will blink forever. Otherwise it will blink "blinkAmount" of times
        /// </summary>
        /// <param name="switchID"></param>
        /// <param name="led"></param>
        /// <param name="ledData"></param>
        /// <param name="ledAction"></param>
        /// <returns></returns>
        public IDisposable ConnectSwitchToLED(short switchID, short led, LEDData ledData, LEDAction ledAction) {
            return ConnectSwitchToLED(switchID, led, ledData.red, ledData.green, ledData.blue, ledAction, ledData.interval, ledData.amount);
        }



        /// <summary>
        /// If switch is active, led will blink for blinkAmount of times. If blinkAmount is 0, led will blink until switch is inactive.
        /// </summary>
        /// <param name="switchID"></param>
        /// <param name="led"></param>
        /// <param name="interval"></param>
        /// <param name="blinkAmount"></param>
        public IDisposable ConnectSwitchToLEDBlink(short switchID, short led, byte red, byte green, byte blue, short interval, byte blinkAmount = 0) {
            if (blinkAmount == 0) {
                CompositeDisposable disposable = new CompositeDisposable();
                disposable.Add( BrainInterface.OnSwitchActive(switchID).Subscribe(e => BrainInterface.BlinkLED(led, red, green , blue, interval)).AddTo(this));
                disposable.Add( BrainInterface.OnSwitchInactive(switchID).Subscribe(e => BrainInterface.DeactivateLED(led)).AddTo(this));
                return disposable;
            } else {
                return BrainInterface.OnSwitchActive(switchID).Subscribe(e => BrainInterface.BlinkLED(led, red, green, blue, interval, blinkAmount)).AddTo(this);
            }
        }

        /// <summary>
        /// Connects switch to led, using LEDData. If ledData.amount is 0, led will blink until switch is inactive.
        /// </summary>
        /// <param name="switchID"></param>
        /// <param name="ledBlinkData"></param>
        public IDisposable ConnectSwitchToLEDBlink(short switchID, short led, LEDData ledData) {
            return ConnectSwitchToLEDBlink(switchID, led, ledData.red, ledData.green, ledData.blue, ledData.interval, ledData.amount);
        }

        /// <summary>
        /// Gets the score of a player
        /// </summary>
        /// <param name="playerID"></param>
        /// <returns></returns>
        public int GetPlayerScore(int playerID) {
            return playerScore[playerID];
        }

        /// <summary>
        /// Increase the score of the current player
        /// </summary>
        /// <param name="score"></param>
        public void IncreaseCurrentPlayerScore(int score) {
            IncreasePlayerScore(currentPlayer, score);
        }

        /// <summary>
        /// Increase the score of a player
        /// </summary>
        /// <param name="player"></param>
        /// <param name="score"></param>
        public void IncreasePlayerScore(int player, int score) {
            playerScore[player] += score;
            playerScoreChangedEventCache[player].currentScore = playerScore[player];
            PlayerScoreChangedCmd.Execute(playerScoreChangedEventCache[player]);
        }

        /// <summary>
        /// Increases the current player score when switch is triggered
        /// </summary>
        /// <param name="switchID"></param>
        /// <param name="score"></param>
        public IDisposable ConnectSwitchToScoreIncrease(short switchID, int score) {
            return BrainInterface.OnSwitchActive(switchID).Subscribe(e => IncreaseCurrentPlayerScore(score)).AddTo(this);
        }

        public IDisposable ConnectSwitchToScoreIncrease(short switchID, int score, int player) {
            return BrainInterface.OnSwitchActive(switchID).Subscribe(e => IncreasePlayerScore(player, score)).AddTo(this);
        }

        public IObservable<PlayerScoreChangedEvent> OnPlayerScoreChanged(int playerID) {
            return PlayerScoreChangedCmd.Where(v => v.playerID == playerID);
        }

        public IObservable<PlayerScoreChangedEvent> OnCurrentPlayerScoreChanged() {
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