using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;

public class PadDetector : MonoBehaviour
{
    internal InputUser[] _users = new InputUser[4];

    internal Gamepad[] _gamepads = new Gamepad[4];

    void Awake()
    {
        InputUser.listenForUnpairedDeviceActivity = 4;

        InputUser.onChange += OnControlsChanged;
        InputUser.onUnpairedDeviceUsed += ListenForUnpairedGamepads;

        for (var i = 0; i < _users.Length; i++)
        {
            _users[i] = InputUser.CreateUserWithoutPairedDevices();
        }
    }

    void OnControlsChanged(InputUser user, InputUserChange change, InputDevice device)
    {
        if (change == InputUserChange.DevicePaired)
        {
            var playerId = Array.IndexOf(_users, user);
            _gamepads[playerId] = device as Gamepad;

            Debug.Log("User " + playerId + " got connected with a new pad");
        }
        else if (change == InputUserChange.DeviceUnpaired)
        {
            var playerId = Array.IndexOf(_users, user);
            _gamepads[playerId] = null;
        }
    }

    void ListenForUnpairedGamepads(InputControl control, InputEventPtr arg2)
    {
        if (control.device is Gamepad)
        {
            for (var i = 0; i < _users.Length; i++)
            {
                // find a user without a paired device
                if (_users[i].pairedDevices.Count == 0)
                {
                    // pair the new Gamepad device to that user
                    _users[i] = InputUser.PerformPairingWithDevice(control.device, _users[i]);
                    return;
                }
            }
        }
    }
}
