﻿using System.Collections;
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

        CompositeDisposable disposables;

        public BrainBase() {
            Debug.Log("Creating brain.");
            disposables = new CompositeDisposable();

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
        protected void ConnectSwitchToLED(short switchID, short led) {
            BrainInterface.OnSwitchActive(switchID).Subscribe(e => BrainInterface.ActivateLED(led)).AddTo(this);
            BrainInterface.OnSwitchInactive(switchID).Subscribe(e => BrainInterface.DeactivateLED(led)).AddTo(this);
        }

        /// <summary>
        /// If switch is active, led will blink for blinkAmount of times. If blinkAmount is 0, led will blink until switch is inactive.
        /// </summary>
        /// <param name="switchID"></param>
        /// <param name="led"></param>
        /// <param name="interval"></param>
        /// <param name="blinkAmount"></param>
        protected void ConnectSwitchToLEDBlink(short switchID, short led, short interval, byte blinkAmount = 0) {
            if (blinkAmount == 0) {
                BrainInterface.OnSwitchActive(switchID).Subscribe(e => BrainInterface.BlinkLED(led, interval)).AddTo(this);
                BrainInterface.OnSwitchInactive(switchID).Subscribe(e => BrainInterface.DeactivateLED(led)).AddTo(this);
            } else {
                BrainInterface.OnSwitchActive(switchID).Subscribe(e => BrainInterface.BlinkLED(led, interval, blinkAmount)).AddTo(this);
            }
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