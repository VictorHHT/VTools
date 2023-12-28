using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Victor.Tools
{
    public static class VTGUIStates
    {
        // Button States
        public class SmartToggleButtonState
        {
            public enum SetDirection { Up, Down };
            public SetDirection setDirection;
            // Set to what based on !hot control button state initial bool value
            public bool setTo;
            // We use this bool to show if the hot control is smart toggle too
            public bool hasSetDirection;
            public bool hotControlIsSmartToggleButton;
            // Set in mouse down to hot control's bot boundary
            public float setDownBotBoundary;
            public float setUpTopBoundary;
        }
    }
}

