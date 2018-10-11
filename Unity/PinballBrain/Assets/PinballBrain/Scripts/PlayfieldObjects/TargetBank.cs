using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;

namespace PinballBrain {
    public class TargetBank : IDisposable{
        readonly IBrain brain;
        /// <summary>
        /// Switches to check
        /// </summary>
        List<short> switches;

        Dictionary<short, IDisposable> switchCheckDisposables;

        /// <summary>
        /// Switches that have been pressed. List will be cleared when targetbank is resetted
        /// </summary>
        List<short> switchesPressed;

        bool autoReset;
        float autoResetTime;

        IDisposable autoResetDisposable;

        ReactiveCommand<List<short>> onAllTargetsHit;
        ReactiveCommand<List<short>> onReset;
        ReactiveCommand<short> onTargetHit;
        ReactiveCommand<short> onUniqueTargetHit;

        public TargetBank(IBrain brain, List<short> switches = null, bool enableAutoReset = true, float autoResetTime = 1000) {
            this.brain = brain;
            this.switches = new List<short>();
            this.onAllTargetsHit = new ReactiveCommand<List<short>>();
            this.onReset = new ReactiveCommand<List<short>>();
            this.onTargetHit = new ReactiveCommand<short>();
            this.onUniqueTargetHit = new ReactiveCommand<short>();
            this.switchesPressed = new List<short>();
            this.switchCheckDisposables = new Dictionary<short, IDisposable>();

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
                IDisposable switchCheck = brain.ConnectSwitchToAction(switchID, TargetDown);
                switchCheckDisposables[switchID] = switchCheck;
            }
        }

        void TargetDown(short switchID) {
            //Add switch to pressed switches list
            if (!switchesPressed.Contains(switchID)) {
                switchesPressed.Add(switchID);

                this.onUniqueTargetHit.Execute(switchID);

                //Check if all targets are down
                if (switches.TrueForAll(v => switchesPressed.Contains(v))) {
                    onAllTargetsHit.Execute(switches);
                }
            }

            this.onTargetHit.Execute(switchID);
        }

        /// <summary>
        /// Remove a switch from the targetbank
        /// </summary>
        /// <param name="switchID"></param>
        public void RemoveSwitch(short switchID) {
            if (this.switches.Contains(switchID)) {
                this.switches.Remove(switchID);
                if (this.switchCheckDisposables.ContainsKey(switchID) && this.switchCheckDisposables[switchID] != null) {
                    this.switchCheckDisposables[switchID].Dispose();
                    this.switchCheckDisposables[switchID] = null;
                }
            }
        }

        /// <summary>
        /// Observable for a target hit. Called everytime a target is hit. Will also be called, if target was already hit, but targetbank was not resetted.
        /// </summary>
        /// <param name="switchID"></param>
        /// <returns></returns>
        public IObservable<short> OnTargetHit(short switchID) {
            return this.onTargetHit.Where(v => v == switchID);
        }

        /// <summary>
        /// Observable for a target hit. Called ONCE for each target. Will not be called until targetbank is resetted.
        /// </summary>
        /// <param name="switchID"></param>
        /// <returns></returns>
        public IObservable<short> OnUniqueTargetHit(short switchID) {
            return this.onUniqueTargetHit.Where(v => v == switchID);
        }

        /// <summary>
        /// Observable for all targets down
        /// </summary>
        /// <returns></returns>
        public IObservable<List<short>> OnAllTargetsHit() {
            return onAllTargetsHit;
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
            this.switchesPressed.Clear();
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
                autoResetDisposable = OnAllTargetsHit().Delay(TimeSpan.FromMilliseconds(autoResetTime)).Subscribe(e => ResetBank());
            }

            this.autoReset = enable;
            this.autoResetTime = autoResetTime;
        }

        public void Dispose() {
            if(autoResetDisposable != null) autoResetDisposable.Dispose();
            this.onAllTargetsHit.Dispose();
            this.onTargetHit.Dispose();
            this.onUniqueTargetHit.Dispose();
            this.onReset.Dispose();
            foreach(var kv in switchCheckDisposables) {
                if(kv.Value != null) kv.Value.Dispose();
            }
            switchCheckDisposables.Clear();
        }
    }
}