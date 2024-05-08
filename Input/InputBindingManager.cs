using Gma.System.MouseKeyHook;
using NezurAimbot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

public class InputBindingManager : IDisposable
{
    private IKeyboardMouseEvents _mEvents;
    private bool isSettingBinding = false;

    public string CurrentBinding { get; private set; }

    public event Action<string> OnBindingSet;
    public event Action<string> OnBindingPressed;
    public event Action<string> OnBindingReleased;

    public AntiRecoilManager arManager = new();

    public void SetupDefault(string KeyCode)
    {
        CurrentBinding = KeyCode.ToString();
        OnBindingSet?.Invoke(CurrentBinding);
        SetupEventHandlers();
    }

    public void StartListeningForBinding()
    {
        isSettingBinding = true;
        SetupEventHandlers();
    }

    private void SetupEventHandlers()
    {
        _mEvents ??= Hook.GlobalEvents();
        _mEvents.KeyDown += GlobalHookKeyDown;
        _mEvents.MouseDown += GlobalHookMouseDown;
        _mEvents.KeyUp += GlobalHookKeyUp;
        _mEvents.MouseUp += GlobalHookMouseUp;
    }

    private void GlobalHookKeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.OemCloseBrackets && GlobalSettings.AntiRecoil)
        {
            GlobalSettings.AntiRecoil = false;
            System.Windows.MessageBox.Show("Anti-Recoil was disabled using keybind.");
        }
        else
        {
            HandleInputEvent(e.KeyCode.ToString());
        }
    }

    private void GlobalHookMouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            if (GlobalSettings.AntiRecoil)
            {
                arManager.MousePress = 0;
                _ = arManager.StartAntiRecoil();
            }
            HandleInputEvent("LeftMouse");
        }
        else
        {
            HandleInputEvent(e.Button.ToString());
        }
    }

    private void HandleInputEvent(string input)
    {
        if (isSettingBinding)
        {
            CurrentBinding = input;
            OnBindingSet?.Invoke(CurrentBinding);
            isSettingBinding = false;
        }
        else if (CurrentBinding == input || (CurrentBinding == "LeftMouse" && input == "Left"))
        {
            OnBindingPressed?.Invoke(CurrentBinding);
        }
    }

    private void GlobalHookKeyUp(object sender, KeyEventArgs e)
    {
        if (CurrentBinding == e.KeyCode.ToString())
        {
            OnBindingReleased?.Invoke(CurrentBinding);
        }
    }

    private void GlobalHookMouseUp(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            if (CurrentBinding != "LeftMouse")
            {
                if (GlobalSettings.AntiRecoil)
                {
                    arManager.StopAntiRecoil();
                    arManager.MousePress = 0;
                }
                OnBindingReleased?.Invoke(CurrentBinding);
            }
        }
        else
        {
            if (CurrentBinding == e.Button.ToString())
            {
                OnBindingReleased?.Invoke(CurrentBinding);
            }
        }
    }

    public void StopListening()
    {
        if (_mEvents == null) return;

        _mEvents.KeyDown -= GlobalHookKeyDown;
        _mEvents.MouseDown -= GlobalHookMouseDown;
        _mEvents.KeyUp -= GlobalHookKeyUp;
        _mEvents.MouseUp -= GlobalHookMouseUp;
        _mEvents.Dispose();
        _mEvents = null;
    }

    public void Dispose()
    {
        StopListening();
    }
}