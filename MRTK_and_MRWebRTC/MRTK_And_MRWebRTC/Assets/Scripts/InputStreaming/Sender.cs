using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

using Microsoft.MixedReality.WebRTC;


    using InputSystem = UnityEngine.InputSystem.InputSystem;

    class Sender : InputManager, IDisposable
    {
        public override event Action<InputEventPtr, InputDevice> onEvent;
        public override event Action<InputDevice, InputDeviceChange> onDeviceChange;
        public override event Action<string, InputControlLayoutChange> onLayoutChange;

        private InputPositionCorrector _corrector;
        private Action<InputEventPtr, InputDevice> _onEvent;

        public Sender()
        {
            InputSystem.onEvent += OnEvent;
            InputSystem.onDeviceChange += OnDeviceChange;
            InputSystem.onLayoutChange += OnLayoutChange;

            _onEvent = (InputEventPtr ptr, InputDevice device) => { onEvent?.Invoke(ptr, device); };
            _corrector = new InputPositionCorrector(_onEvent);
        }

        ~Sender()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            InputSystem.onEvent -= OnEvent;
            InputSystem.onDeviceChange -= OnDeviceChange;
            InputSystem.onLayoutChange -= OnLayoutChange;
        }

        /// <summary>
        /// 
        /// </summary>
        public override ReadOnlyArray<InputDevice> devices
        {
            get
            {
                return InputSystem.devices;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override IEnumerable<string> layouts
        {
            get
            {
                // todo(kazuki):: filter layout
                return InputSystem.ListLayouts();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool EnableInputPositionCorrection { set; get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputRegion"></param>
        /// <param name="outputRegion"></param>
        public void SetInputRange(Rect inputRegion, Rect outputRegion)
        {
            _corrector.inputRegion = inputRegion;
            _corrector.outputRegion = outputRegion;
        }

        private void OnEvent(InputEventPtr ptr, InputDevice device)
        {
            // mapping sender coordinate system to receiver one.
            if (EnableInputPositionCorrection && device is Pointer && ptr.IsA<StateEvent>())
            {
                _corrector.Invoke(ptr, device);
            }
            else
            {
                onEvent?.Invoke(ptr, device);
            }
        }

        private void OnDeviceChange(InputDevice device, InputDeviceChange change)
        {
            onDeviceChange?.Invoke(device, change);
        }
        private void OnLayoutChange(string name, InputControlLayoutChange change)
        {
            onLayoutChange?.Invoke(name, change);
        }
    }

    /// <summary>
    ///
    /// </summary>
    class Observer : IObserver<InputRemoting.Message>
    {
        private DataChannel _channel;
        public Observer(DataChannel channel)
        {
            _channel = channel ?? throw new ArgumentNullException("channel is null");
        }
        public void OnNext(InputRemoting.Message value)
        {
            if (_channel.State != DataChannel.ChannelState.Open)
                return;
        //Debug.Log("Sender input message");
            byte[] bytes = MessageSerializer.Serialize(ref value);
            _channel.SendMessage(bytes);
        }

        public void OnCompleted()
        {
        }
        public void OnError(Exception error)
        {
        }
    }

// #endif
