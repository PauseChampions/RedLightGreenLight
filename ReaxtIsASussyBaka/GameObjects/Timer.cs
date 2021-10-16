using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ReaxtIsASussyBaka.GameObjects
{
    internal class Timer : MonoBehaviour
    {
        public float InitialTime;
        public float CurrentTime { get; private set; }

        public void Start() => enabled = false;

        public void Update()
        {
            CurrentTime -= Time.deltaTime;
            if (CurrentTime <= 0)
            {
                StopTimer();
                TimerStoppedEvent?.Invoke();
            }
        }

        internal void ResetTime() => CurrentTime = InitialTime;

        internal void StartTimer(float time)
        {
            InitialTime = CurrentTime = time;
            enabled = true;
        }

        internal void StopTimer()
        {
            enabled = false;
            CurrentTime = InitialTime;
        }
        internal void ResumeTimer() => enabled = true;
        internal void PauseTimer() => enabled = false;
        internal void ToggleTimer() => enabled = !enabled;

        public event Action TimerStoppedEvent;
    }
}
