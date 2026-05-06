using Com.IsartDigital.HealerSurvivor.Manager;
using System;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.HealerSurvivor.Manager
{
    
    public class TimeManager : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        private const int NEXT_DAY = 1;

        private DateTime _LastCheck;
        public DateTime CurrentTime => DateTime.Now;
        public TimeSpan TimeUntilMidnight => DateTime.Today.AddDays(NEXT_DAY) - CurrentTime;

        public string TimeUntilMidnightString => TimeUntilMidnight.ToString(@"hh\:mm\:ss");

        private GameManager _GameManager => GameManager.Instance;
        
        public event Action OnWeeklyReset;

        public static TimeManager Instance { get; private set; }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        private void Awake()
        {
            _LastCheck = DateTime.Now;

            #region SINGLETON
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            #endregion
        }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // PROCESS
        private void Update() => CheckMidnight();
        
        private void CallMidnightEvent()
        {
            //TODO : Call Events in shop for daily reward + quests
        }

        private void CheckMidnight()
        {
            if (CurrentTime.Date > _LastCheck.Date)
                _GameManager.onMidNight?.Invoke();
            
            _LastCheck = CurrentTime; 
            
            if (GetIso8601WeekOfYear(CurrentTime) != GetIso8601WeekOfYear(_LastCheck))
                OnWeeklyReset?.Invoke();
        }
        
        private int GetIso8601WeekOfYear(DateTime pTime)
        {
            return System.Globalization.CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
                pTime, 
                System.Globalization.CalendarWeekRule.FirstFourDayWeek, 
                DayOfWeek.Monday
            );
        }

        private void OnEnable()
        {
            if (_GameManager == null)
                return;

            _GameManager.onMidNight += CallMidnightEvent;
        }

        private void OnDisable()
        {
            if (_GameManager == null)
                return;

            _GameManager.onMidNight -= CallMidnightEvent;
        }
    }
}