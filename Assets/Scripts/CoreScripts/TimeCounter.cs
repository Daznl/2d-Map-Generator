using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeCounter : MonoBehaviour
{
    public Text timeDisplay;
    public int daysInYear = 12 * 30; // 12 months * 30 days
    public int daysInMonth = 30; // Each month has 30 days

    private float elapsedTime;
    public int totalDays;
    private bool isCounting = false;
    public int currentYear;
    public int currentMonth;

    // Declare the event
    public event Action OnYearIncremented;
    public event Action OnMonthIncremented;
    public event Action OnDayIncremented;


    // A dictionary to hold RaceManagers and their raceStarts
    private Dictionary<int, List<Action<TimeCounter>>> raceStartActions = new Dictionary<int, List<Action<TimeCounter>>>();
    public void SubscribeToRaceStart(int raceStart, Action<TimeCounter> action)
    {
        if (!raceStartActions.ContainsKey(raceStart))
        {
            raceStartActions[raceStart] = new List<Action<TimeCounter>>();
        }
        raceStartActions[raceStart].Add(action);
    }

    private void Start()
    {
        elapsedTime = 0;
        totalDays = 0;
        currentMonth = 0;
        UpdateTimeDisplay();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isCounting = !isCounting;
        }

        if (isCounting)
        {
            elapsedTime += Time.fixedDeltaTime * daysInYear;
            int newTotalDays = Mathf.FloorToInt(elapsedTime);

            if (newTotalDays != totalDays)
            {
                totalDays = newTotalDays;
                UpdateTimeDisplay();
                // Trigger NewDayStarted event for each new day
                OnDayIncremented?.Invoke();

                // Check if a month has passed
                int lastMonth = (totalDays - 1) / daysInMonth;
                currentMonth = totalDays / daysInMonth;

                if (lastMonth != currentMonth)
                {
                    // Invoke the month incremented event
                    OnMonthIncremented?.Invoke();
                }

                // Check if a year has passed
                int lastYear = (totalDays - 1) / daysInYear;
                currentYear = totalDays / daysInYear;

                if (lastYear != currentYear)
                {
                    // Invoke the year incremented event
                    OnYearIncremented?.Invoke();

                    if (raceStartActions.ContainsKey(currentYear))
                    {
                        foreach (var action in raceStartActions[currentYear])
                        {
                            action(this);
                        }
                        raceStartActions.Remove(currentYear); // Consider if you want to keep this
                    }
                }
            }
        }
    }

    private void UpdateTimeDisplay()
    {
        int days = totalDays % daysInYear % daysInMonth; // Days in the current month
        int years = totalDays / daysInYear; // Total years passed
        int months = (totalDays % daysInYear) / daysInMonth; // Months in the current year

        timeDisplay.text = string.Format("<color=#FFFFFF>{0} Years, {1} Months, {2} Days</color>", years, months, days);
    }
}