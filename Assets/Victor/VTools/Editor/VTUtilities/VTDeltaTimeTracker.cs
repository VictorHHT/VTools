using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Victor.Tools
{
    public struct VTDeltaTimeTracker
    {
        private float m_DeltaTime;
        private long m_LastTime;

        public readonly float deltaTime => m_DeltaTime;

        public void Prepare()
        {
            m_LastTime = DateTime.Now.Ticks;
        }

        public void Update()
        {
            m_DeltaTime = (DateTime.Now.Ticks - m_LastTime) / 10000000.0f;
            m_LastTime = DateTime.Now.Ticks;
        }
    }
}
