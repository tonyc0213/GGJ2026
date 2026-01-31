using System;
using TMPro;
using UnityEngine;

namespace GameFlow
{
    public class Timer : MonoBehaviour
    {
        public event Action OnTimesUp;
        public TextMeshProUGUI timeText;
        
        public float CurrentTime => timer;
        private float timer;
        
        public void StartTimer(float time)
        {
            timer = time;
        }

        public void StopTimer()
        {
            timer = -1;
        }
        
        private void Update()
        {
            if (timer <= 0)
            {
                return;
            }
            
            timer -= Time.deltaTime;
            timeText.text = string.Format($"{timer:0.0}");
            
            if (timer <= 0)
            {
                OnTimesUp?.Invoke();
            }
        }
    }
}