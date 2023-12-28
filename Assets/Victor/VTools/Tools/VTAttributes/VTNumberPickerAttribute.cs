using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Victor.Tools
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
    public class VTNumberPickerAttribute : PropertyAttribute
    {
        public int[] intOptions;
        public float[] floatOptions;

        public VTNumberPickerAttribute(int[] options)
        {
            intOptions = options;
        }

        public VTNumberPickerAttribute(float[] options)
        {
            floatOptions = options;
        }
    }

}
