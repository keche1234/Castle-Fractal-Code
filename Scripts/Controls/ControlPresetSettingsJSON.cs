using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ControlPresetSettingsJSON
{
    //public int controlSchemeIndex;
    //public string nameOverride;
    public ControlPresetSettings.MeleeAim meleeAim;
    public ControlPresetSettings.RangedAim rangedAim;
    public int rangedAssist;
    public ControlPresetSettings.SignatureActivation signatureActivation;
}
