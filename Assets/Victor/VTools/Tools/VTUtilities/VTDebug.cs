using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Victor.Tools
{
    public static class VTDebug
    {
        public enum DebugTypes { Log, Warning, Error };

        /// <summary>
        /// A helper function to log any variable formated value
        /// Example: VTDebug.LogVariable(someVariable, nameof(someVariable)); 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="variableToLog"></param>
        /// <param name="variableName"></param>
        /// <param name="debugType"></param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static void LogVariable<T>(T variableToLog, object variableName, DebugTypes debugType = DebugTypes.Log)
        {
            if (variableToLog == null)
            {
                throw new System.ArgumentNullException("variableToLog", "variableToLog can not be null");
            }

            // Note that if we use nameof(variableToLog) in this function, the name will be always "variableToLog"
            if (variableName == null || variableName != null && variableName.ToString().EmptyOrWhiteSpace())
            {
                switch (debugType)
                {
                    case DebugTypes.Log:
                        Debug.Log($"<color=white>[VTDebug]</color>: The name of the variable cannot be null, empty or all white spaces");
                        break;
                    case DebugTypes.Warning:
                        Debug.Log($"<color=#FFC009>[VTDebug]</color>: The name of the variable cannot be null, empty or all white spaces");
                        break;
                    case DebugTypes.Error:
                        Debug.Log($"<color=#FF6E3F>[VTDebug]</color>: The name of the variable cannot be null, empty or all white spaces");
                        break;
                    default:
                        break;
                }
                return;
            }

            switch (debugType)
            {
                case DebugTypes.Log:
                    Debug.Log($"Name: <color=white>{variableName}</color>, Type: <color=white>{typeof(T)}</color>, Value: <color=white>{variableToLog.ToString()}</color>");
                    break;
                case DebugTypes.Warning:
                    Debug.LogWarning($"Name: <color=#FFC009>{variableName}</color>, Type: <color=#FFC009>{typeof(T)}</color>, Value: <color=#FFC009>{variableToLog.ToString()}</color>");
                    break;
                case DebugTypes.Error:
                    Debug.LogError($"Name: <color=#FF6E3F>{variableName}</color>, Type: <color=#FF6E3F>{typeof(T)}</color>, Value: <color=#FF6E3F>{variableToLog.ToString()}</color>");
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Use reflection to clear the console window (Same effect as you click the clear button on console)
        /// </summary>
        public static void ClearConsole()
        {
            var logEntries = System.Type.GetType("UnityEditor.LogEntries, UnityEditor.dll");
            if (logEntries != null)
            {
                var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
                if (clearMethod != null)
                {
                    clearMethod.Invoke(null, null);
                }
            }
        }
    }
}

