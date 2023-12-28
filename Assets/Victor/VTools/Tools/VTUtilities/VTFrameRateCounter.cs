using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Victor.Tools
{
    // Based on the solution by csofranz here https://forum.unity.com/threads/solved-this-fps-counter-method-is-right.756389/#:~:text=If%20you%20divide%20One%20by%20that%20number%2C%20you,average%20at%20the%20end%20of%20every%20n%20iterations.
    public class VTFrameRateCounter : MonoBehaviour
    {
        [VTReadOnly]
        public float frameRate;
        [VTRangeStep(5, 100, 5)]
        public int samplesCount = 20;
        [VTRangeStep(0, 2, 1)]
        public int frameRateDecimalPoint = 0;
        private float m_FrameRateCount;
        private float m_AccumulatedTime;

        private void Start()
        {
            m_FrameRateCount = samplesCount;
            m_AccumulatedTime = 0f;
        }

        private void Update()
        {
            UpdateFrameRate();
        }

        protected void UpdateFrameRate()
        {
            m_FrameRateCount -= 1;
            m_AccumulatedTime += Time.deltaTime;

            if (m_FrameRateCount <= 0)
            {
                frameRate = samplesCount / m_AccumulatedTime;
                frameRate = VTMath.Round(frameRate, frameRateDecimalPoint);
                m_AccumulatedTime = 0f;
                m_FrameRateCount = samplesCount;
            }
        }
    }
}

