using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


public class Timer : MonoBehaviour
{
    public class TimerControl
    {
        public bool IsRunning {  get; set; } = true;
    }

    GameObject timer;

    static List<TimerControl> timers =  new List<TimerControl>();

    public TimerControl StartFuncCycle(Action func, int timeDelay)
    {
        var control = new TimerControl();
        TimerCoroutine(func, timeDelay, control);
        timers.Add(control);
        return control;
    }

    public async void TimerCoroutine(Action func, int timeDelay, TimerControl control)
    {
        while (control.IsRunning)
        {
            func();
            await Task.Delay(timeDelay);
        }
    }

    Task wait(int delay)
    {
        Thread.Sleep(delay);
        return Task.CompletedTask;
    }

    static async void StartFuncCycleInternal(Action func, int timeDelay, TimerControl control)
    {
        while (control.IsRunning)
        {
            func();

            await Task.Delay(timeDelay);
        }
    }

    void OnApplicationQuit()
    {
        foreach (var timer in timers)
        {
            timer.IsRunning = false;
        }
    }
}

