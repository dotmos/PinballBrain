using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;

namespace PinballBrain {
    public class TargetBank : IDisposable{
        readonly IBrain brain;
        List<short> switches;

        bool autoReset;
        float autoResetTime;

        IDisposable autoResetDisposable;

        ReactiveCommand<List<short>> onAllTargetsDown;
        ReactiveCommand<List<short>> onReset;

        public TargetBank(IBrain brain, List<short> switches = null, bool enableAutoReset = true, float autoResetTime = 1000) {
            this.brain = brain;
            this.switches = new List<short>();
            this.onAllTargetsDown = new ReactiveCommand<List<short>>();
            this.onReset = new ReactiveCommand<List<short>>();

            if(switches != null) {
                foreach (short s in switches) {
                    AddSwitch(s);
                }
            }

            EnableAutoReset(enableAutoReset, autoResetTime);
        }

        /// <summary>
        /// Add a switch to the targetbank
        /// </summary>
        /// <param name="switchID"></param>
        public void AddSwitch(short switchID) {
            if (!this.switches.Contains(switchID)) {
                this.switches.Add(switchID);
            }
        }

        /// <summary>
        /// Remove a switch from the targetbank
        /// </summary>
        /// <param name="switchID"></param>
        public void RemoveSwitch(short switchID) {
            if (this.switches.Contains(switchID)) {
                this.switches.Remove(switchID);
            }
        }

        /// <summary>
        /// Observable for all targets down
        /// </summary>
        /// <returns></returns>
        public IObservable<List<short>> OnAllTargetsDown() {
            return onAllTargetsDown;
        }

        /// <summary>
        /// Observable for reset of targetbank
        /// </summary>
        /// <returns></returns>
        public IObservable<List<short>> OnReset() {
            return onReset;
        }

        /// <summary>
        /// Resets the target bank
        /// </summary>
        public void ResetBank() {
            this.onReset.Execute(this.switches);
        }

        /// <summary>
        /// Enable/Disable autoreset for this targetbank and set time before reset is triggered
        /// </summary>
        /// <param name="enable"></param>
        /// <param name="autoResetTime"></param>
        public void EnableAutoReset(bool enable, float autoResetTime) {
            if (autoResetDisposable != null) autoResetDisposable.Dispose();

            if (enable) {
                autoResetDisposable = OnAllTargetsDown().Delay(TimeSpan.FromMilliseconds(autoResetTime)).Subscribe(e => ResetBank());
            }

            this.autoReset = enable;
            this.autoResetTime = autoResetTime;
        }

        public void Dispose() {
            if(autoResetDisposable != null) autoResetDisposable.Dispose();
        }
    }
}