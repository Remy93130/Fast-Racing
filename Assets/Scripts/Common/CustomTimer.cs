﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CustomTimer : MonoBehaviour
{

    static Dictionary<string, CustomTimer> dicoCustomTimers;
    public static CustomTimer GetCustomTimer(string name)
    {
        dicoCustomTimers.TryGetValue(name, out CustomTimer customTimer);
        return customTimer;
    }

    public bool IsRunning { get; private set; }

    float m_Time = 0;

    private float m_TimeScale;

    public float TimeScale
    {
        get
        {
            return m_TimeScale;
        }
        set
        {
            m_TimeScale = value;
        }
    }
    public float DeltaTime
    {
        get
        {

            return IsRunning ? UnityEngine.Time.deltaTime * m_TimeScale : 0;
        }
    }

    public float FixedDeltaTime
    {
        get
        {

            return IsRunning ? UnityEngine.Time.fixedDeltaTime * m_TimeScale : 0;
        }
    }


    public float Time
    {
        get
        {
            return m_Time;
        }
        private set
        {
            m_Time = value;
        }
    }

    public void StopTimer()
    {
        IsRunning = false;
    }

    public void StartTimer()
    {
        IsRunning = true;
    }

    public void Reset()
    {
        Time = 0;
    }

    public void ResetWithTimer(bool startTimer)
    {
        Time = 0;
        IsRunning = startTimer;
    }

    public void ResetAndStart()
    {
        ResetWithTimer(true);
    }

    public void ResetAndStop()
    {
        ResetWithTimer(false);
    }

    private void Awake()
    {
        if (dicoCustomTimers == null)
        {
            dicoCustomTimers = new Dictionary<string, CustomTimer>();
            CustomTimer[] customTimers = GameObject.FindObjectsOfType<CustomTimer>();
            foreach (var item in customTimers)
            {
                dicoCustomTimers.Add(item.name, item);
            }
        }
    }
    // Use this for initialization
    void Start()
    {
        ResetWithTimer(false);
    }

    // Update is called once per frame
    void Update()
    {
        Time += DeltaTime;
    }
}
