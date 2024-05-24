using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ListInput
{
    /*****************************************************************************
     * Mode One: Returns true if GetKeyDown is true for at least one key in `keys`
     * Mode All: Returns true if GetKeyDown is true for every key in `keys`
     ****************************************************************************/
    public static bool GetKeyDown(List<KeyCode> keys, InputMode mode = InputMode.One)
    {
        if (keys.Count == 0)
            return false;
        switch (mode)
        {
            case InputMode.One:
                foreach (KeyCode k in keys)
                    if (Input.GetKeyDown(k))
                        return true;
                return false;
            case InputMode.All:
                foreach (KeyCode k in keys)
                    if (!Input.GetKeyDown(k))
                        return false;
                return true;
        }
        return false;
    }

    /*****************************************************************************
     * Mode One: Returns true if GetKey is true for at least one key in `keys`
     * Mode All: Returns true if GetKey is true for every key in `keys`
     ****************************************************************************/
    public static bool GetKey(List<KeyCode> keys, InputMode mode = InputMode.One)
    {
        if (keys.Count == 0)
            return false;
        switch (mode)
        {
            case InputMode.One:
                foreach (KeyCode k in keys)
                    if (Input.GetKey(k))
                        return true;
                return false;
            case InputMode.All:
                foreach (KeyCode k in keys)
                    if (!Input.GetKey(k))
                        return false;
                return true;
        }
        return false;
    }

    /*****************************************************************************
     * Mode One: Returns true if GetKeyUp is true for at least one key in `keys`
     * Mode All: Returns true if GetKeyUp is true for every key in `keys`
     ****************************************************************************/
    public static bool GetKeyUp(List<KeyCode> keys, InputMode mode = InputMode.One)
    {
        if (keys.Count == 0)
            return false;
        switch (mode)
        {
            case InputMode.One:
                foreach (KeyCode k in keys)
                    if (Input.GetKeyUp(k))
                        return true;
                return false;
            case InputMode.All:
                foreach (KeyCode k in keys)
                    if (!Input.GetKeyUp(k))
                        return false;
                return true;
        }
        return false;
    }

    /*******************************************************************************************
     * Mode One: Returns true if GetMouseButtonDown is true for at least one button in `buttons`
     * Mode All: Returns true if GetMouseButtonDown is true for every buttons in `buttons`
     ******************************************************************************************/
    public static bool GetMouseButtonDown(List<int> buttons, InputMode mode = InputMode.One)
    {
        if (buttons.Count == 0)
            return false;
        switch (mode)
        {
            case InputMode.One:
                foreach (int b in buttons)
                    if (Input.GetMouseButtonDown(b))
                        return true;
                return false;
            case InputMode.All:
                foreach (int b in buttons)
                    if (!Input.GetMouseButtonDown(b))
                        return false;
                return true;
        }
        return false;
    }

    /*******************************************************************************************
     * Mode One: Returns true if GetMouseButton is true for at least one button in `buttons`
     * Mode All: Returns true if GetMouseButton is true for every buttons in `buttons`
     ******************************************************************************************/
    public static bool GetMouseButton(List<int> buttons, InputMode mode = InputMode.One)
    {
        if (buttons.Count == 0)
            return false;
        switch (mode)
        {
            case InputMode.One:
                foreach (int b in buttons)
                    if (Input.GetMouseButton(b))
                        return true;
                return false;
            case InputMode.All:
                foreach (int b in buttons)
                    if (!Input.GetMouseButton(b))
                        return false;
                return true;
        }
        return false;
    }

    /*******************************************************************************************
     * Mode One: Returns true if GetMouseButtonUp is true for at least one button in `buttons`
     * Mode All: Returns true if GetMouseButtonUp is true for every buttons in `buttons`
     ******************************************************************************************/
    public static bool GetMouseButtonUp(List<int> buttons, InputMode mode = InputMode.One)
    {
        if (buttons.Count == 0)
            return false;
        switch (mode)
        {
            case InputMode.One:
                foreach (int b in buttons)
                    if (Input.GetMouseButtonUp(b))
                        return true;
                return false;
            case InputMode.All:
                foreach (int b in buttons)
                    if (!Input.GetMouseButtonUp(b))
                        return false;
                return true;
        }
        return false;
    }

    public enum InputMode
    {
        One,
        All
    }
}
