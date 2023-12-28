using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace Victor.Tools
{
    [System.Serializable]
    public class VTCounter
    {
        // If CountMode is Infinite and Countmethod is TimeDuration, the speed will be the absolute value of the difference between CountFrom and 0 multiplied by CountDirection
        public enum CountModes { Once, Reset, PingPong, Inifinite};
        public enum CountMethods { TimeDuration, FixedSpeed};
        public enum CountElements { Time, Number}
        public enum TimeModes { DeltaTime, UnscaledDeltaTime };

        public float countFrom;
        [VTEnumCondition("CountMode", false, true, (int)CountModes.Inifinite)]
        public float countTo;

        [VTEnumCondition("CountMethod", (int)CountMethods.FixedSpeed)]
        public float countSpeed = 1f;
        [VTEnumCondition("CountMethod", (int)CountMethods.TimeDuration)]
        public float countPeriod = 10f;

        public bool paused = true;
        // Auto initialize on start
        public bool autoInitialize = true;
        // Auto start on initialization
        public bool autoStartOnInitialize = false;
        
        [VTReadOnly]
        public float currentCount;
        [VTReadOnly]
        [SerializeField]
        protected float m_CountLeft;

        [VTEnumCondition("CountElement", (int)CountElements.Time)]
        public bool displayHours = true;
        [VTEnumCondition("CountElement", (int)CountElements.Time)]
        public bool displayMinutes = true;
        [VTEnumCondition("CountElement", (int)CountElements.Time)]
        public bool displaySeconds = true;
        [VTEnumCondition("CountElement", (int)CountElements.Time)]
        public bool displayMilliSeconds = false;
        // Only works when descending
        [VTEnumCondition("CountElement", (int)CountElements.Time)]
        public bool activateMilliLess = false;
        [VTEnumCondition("CountElement", (int)CountElements.Time)]
        public bool activateMilliGreater = false;
        [VTEnumCondition("CountElement", (int)CountElements.Time)]
        [VTCondition("ActivateMilliLess")]
        public float activateLessTime = 1f;
        [VTEnumCondition("CountElement", (int)CountElements.Time)]
        [VTCondition("ActivateMilliGreater")]
        public float activateGreaterTime = 30f;
        [VTReadOnly]
        public string currentCountString;

        [Header("Counter Enums")]
        public CountModes countMode;
        public CountMethods countMethod;
        public CountElements countElement;
        public TimeModes timeMode;

        protected enum CountDirections { Ascending, Descending };

        //[VTReadOnly]
        [SerializeField]
        protected CountDirections m_CountDirection;

        [Header("Counter Events")]
        public UnityEvent onTargetReached;
        // For Count with duration
        protected float m_IncrementPerSec;
       
        protected float m_LastTimeStringUpdateTimestamp;

        public float countLeft
        {
            get
            {
                return m_CountLeft;
            }

            set
            {
                m_CountLeft = value;
            }
        }

        protected float relativeDeltaTime
        {
            get
            {
                int direction = m_CountDirection == CountDirections.Ascending ? 1 : -1;
                return timeMode == TimeModes.DeltaTime ? Time.deltaTime * direction : Time.unscaledDeltaTime * direction;
            }
        }

        protected float totalCount
        {
            get
            {
                return Mathf.Abs(countFrom - countTo);
            }
        }

        public virtual void Initialize()
        {
            paused = true;

            DetermineCountDownDirection();
            currentCount = countFrom;
            m_CountLeft = countMode == CountModes.Inifinite ? 0 : totalCount;
            m_IncrementPerSec = totalCount / countPeriod;
            currentCountString = currentCount.ToString();

            if (autoStartOnInitialize)
            {
                Resume();
            }
        }

        public virtual void Start()
        {
            if (autoInitialize)
            {
                Initialize();
            }

            if (countMode != CountModes.Inifinite && m_CountLeft == 0)
            {
                Debug.LogError("Can not start before initialization");
                return;
            }

            Resume();
        }

        public virtual void Update()
        {
            UpdateCountDown();
            CheckReachTarget();
        }

        // This is different than reset, user expect the timer gets resumed after calling this function
        public virtual void Restart()
        {
            Initialize();

            Resume();
        }

        public virtual void Pause()
        {
            paused = true;
        }

        public virtual void Resume()
        {
            if (countMode != CountModes.Inifinite && m_CountLeft == 0)
            {
                Debug.LogError("Can not resume before initialization");
                return;
            }

            paused = false;
        }

        protected virtual void UpdateCountDown()
        {
            if (paused == true)
            {
                return;
            }

            if (m_CountDirection == CountDirections.Ascending)
            {
                if (activateMilliGreater && displayMilliSeconds == false && currentCount >= activateGreaterTime)
                {
                    displayMilliSeconds = true;
                }
                
            }
            else if (m_CountDirection == CountDirections.Descending)
            {
                if (activateMilliLess && displayMilliSeconds == false && currentCount <= activateLessTime)
                {
                    displayMilliSeconds = true;
                }
            }

            if (countMethod == CountMethods.FixedSpeed)
            {
                currentCount += relativeDeltaTime * countSpeed;
            }
            else
            {
                currentCount += relativeDeltaTime * m_IncrementPerSec;
            }

            if (countMode != CountModes.Inifinite)
            {
                m_CountLeft -= relativeDeltaTime * countSpeed;
            }

            if (countElement == CountElements.Time)
            {
                currentCountString = VTime.FloatToTime(currentCount, displayHours, displayMinutes, displaySeconds, displayMilliSeconds);
            }
            else
            {
                currentCountString = ((int)currentCount).ToString();
            }
        }

        protected virtual void CheckReachTarget()
        {
            if (paused == true)
            {
                return;
            }

            // If count down mode is infinite, we never reach the target and will keep going even if it exceeds the end
            if (countMode == CountModes.Inifinite)
            {
                return;
            }

            bool targetReached = false;
            if (m_CountDirection == CountDirections.Ascending && (currentCount >= countTo) || m_CountDirection == CountDirections.Descending && (currentCount <= countTo))
            {
                targetReached = true;
            }

            if (targetReached)
            {
                displayMilliSeconds = false;
                if (countMode == CountModes.Once)
                {
                    currentCount = countTo;
                    m_CountLeft = 0;

                    if (countElement == CountElements.Time)
                    {
                        currentCountString = VTime.FloatToTime(currentCount, displayHours, displayMinutes, displaySeconds, displayMilliSeconds);
                    }
                    else
                    {
                        // We only clamp current count if count mode is once
                        currentCount = countTo;
                        currentCountString = ((int)currentCount).ToString();
                    }

                    paused = true;
                }

                if (countMode == CountModes.Reset)
                {
                    currentCount = countFrom;
                    m_CountLeft = totalCount;

                    if (countElement == CountElements.Time)
                    {
                        currentCountString = VTime.FloatToTime(currentCount, displayHours, displayMinutes, displaySeconds, displayMilliSeconds);
                    }
                    else
                    {
                        currentCountString = ((int)currentCount).ToString();
                    }

                }

                if (countMode == CountModes.PingPong)
                {
                    SwapCountDownDirection();

                    if (countElement == CountElements.Time)
                    {
                        currentCountString = VTime.FloatToTime(currentCount, displayHours, displayMinutes, displaySeconds, displayMilliSeconds);
                    }
                    else
                    {
                        currentCountString = ((int)currentCount).ToString();
                    }
                }

                onTargetReached.Invoke();
            }
        }

        protected virtual void DetermineCountDownDirection()
        {
            if (countMode == CountModes.Inifinite)
            {
                return;
            }

            if (countFrom > countTo)
            {
                m_CountDirection = CountDirections.Descending;
            }
            else if (countFrom < countTo)
            {
                m_CountDirection = CountDirections.Ascending;
            }
            else
            {
                paused = true;
            }

        }

        protected virtual void SwapCountDownDirection()
        {
            // Before swap from and to values
            // When reach target value, set CurrentTime to CountDownTo
            currentCount = countTo;

            float temp = countFrom;
            countFrom = countTo;
            countTo = temp;

            countLeft = totalCount;

            DetermineCountDownDirection();
        }

    }

}
