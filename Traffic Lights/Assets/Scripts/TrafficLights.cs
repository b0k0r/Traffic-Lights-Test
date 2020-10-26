using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TrafficLightsSignal: int
{
    Off = 1,
    Red = 2,
    RedAndYellow = 3,
    Green = 4,
    Yellow = 5,
    FlashingYellow = 6
}

public enum TrafficLightMode
{
    Off,
    Regulated,
    NotRegulated,
    Check
}

public class TrafficLights : MonoBehaviour
{
    private TrafficLightsSignal lightsSignal;  // current signal 
    private TrafficLightMode mode;             // current mode

    private bool isAutomaticControl = false;   // automatic control
    private bool isChange = false;             // change signal
    private bool isWork = false;               // traffic lights work
    private float maxTime = 0;                 // time for change to next signal
    private float timer;                       // current work time 
    private float startTime;                   // start time to count timer
    private int currentSignalNumber = 0;       // current signal in integer

    public float[] timeWorks = new float[4];       // array for set work time everyone signal in Regulated mode
                                                   // where timeWorks[0] - red
                                                   //       timeWorks[1] - red+yellow
                                                   //       timeWorks[2] - green
                                                   //       timeWorks[3] - yellow

    [SerializeField] private float delayFlash = 0.35f;      // frequency flash lights in NotRegulated and Check modes

    private readonly int maxSignals = System.Enum.GetValues(typeof(TrafficLightsSignal)).Length; // max signals in enum Signal

    public List<TrafficLamp> trLights = new List<TrafficLamp>();  // list of traffic lights lamps
    
           
    private void Update()
    {        
        if (!isWork)
            return;
        else
        {
            
            if (isChange)
            {
                StartTime();

                ChangeSignal(ChangeSignalNum());
            }

            // timer work if controller in Automatic mode is Regulated

            if (isAutomaticControl && mode == TrafficLightMode.Regulated)
            {                
                timer = Timer();

                if (timer <= 0)
                    isChange = true;
            }
        }

    }

    //Function for begin work
    public void StartWork(bool _on)
    {
        isWork = _on;
        isChange = _on;
    }

    // Return value if Controller works
    public bool IsWork()
    {
        return isWork;
    }

    // Return value if mode is Regulated
    public bool IsRegulated()
    {
        if (mode == TrafficLightMode.Regulated)
            return true;
        else
            return false;
    }

    // Function for calculate timer
    public float Timer()
    {
        float a = 0;
        float t = 0;
        if (isAutomaticControl && mode == TrafficLightMode.Regulated)
        {
            a = Time.time - startTime;
            t = maxTime - a;
        }

        return t;
    }

    // Change control
    public void AutomaticControl(bool _on)
    {
        isAutomaticControl = _on;
    }
    
    // Function for setup mode for traffic lights
    public void SetMode(TrafficLightMode _mode)
    {
        mode = _mode;
    }

    // Return current traffic light signal in string format
    public string CurrentLightSignal()
    {
        return lightsSignal.ToString();
    }
    
    // Decrease current signal numer
    public int DecreaseSignalNum()
    {
        if (mode == TrafficLightMode.Regulated ||
            mode == TrafficLightMode.Check)
        {
            if (currentSignalNumber > 2)
                currentSignalNumber--;
            else
                currentSignalNumber = maxSignals - 1;
        }

        return currentSignalNumber;
    }

    // Increase current signal numer
    public int IncreaseSignalNum()
    {
        if (mode == TrafficLightMode.Regulated || 
            mode == TrafficLightMode.Check)
        {
            if (currentSignalNumber < maxSignals - 1)
                currentSignalNumber++;
            else
                currentSignalNumber = 2;
        }

        return currentSignalNumber;
    }

    // Set manual current signal numer
    public void SetSignalNum(int _num)
    {
        currentSignalNumber = _num;
    }
    
    // Change signal of traffic lights
    public void ChangeSignal(int _value)
    {        
        int[] a;  // array for current signals

        lightsSignal = (TrafficLightsSignal)_value;
        Debug.Log("Traffic lights switch mode to " + lightsSignal);

        switch (lightsSignal)
        {
            case TrafficLightsSignal.Off:
                TurnOff();
                break;

            case TrafficLightsSignal.Red:
                a = new int[1];
                a[0] = 0;
                TurnOnOff(a);
                maxTime = timeWorks[0];
                break;

            case TrafficLightsSignal.RedAndYellow:
                a = new int[2];
                a[0] = 0;
                a[1] = 1;
                TurnOnOff(a);
                maxTime = timeWorks[1];
                break;

            case TrafficLightsSignal.Green:
                a = new int[1];
                a[0] = 2;
                TurnOnOff(a);
                maxTime = timeWorks[2];
                break;

            case TrafficLightsSignal.Yellow:
                a = new int[1];
                a[0] = 1;
                TurnOnOff(a);
                maxTime = timeWorks[3];
                break;

            case TrafficLightsSignal.FlashingYellow:
                TurnOff();
                FlashLight();
                break;
        }
    }

    // Return current signal numer depending on current mode
    private int ChangeSignalNum()
    {
        switch (mode)
        {
            case TrafficLightMode.Off:
                currentSignalNumber = 1;
                break;

            case TrafficLightMode.Regulated:

                IncreaseSignalNum();

                break;

            case TrafficLightMode.NotRegulated:
                currentSignalNumber = maxSignals;

                break;

            case TrafficLightMode.Check:
                TurnOff();
                currentSignalNumber = 2;
                CheckTrafficLights();
                break;
        }


        return currentSignalNumber;
    }

    // Turn off all lights
    private void TurnOff()
    {
        StopAllCoroutines();
        for (int i = 0; i < trLights.Count; i++)
        {
            trLights[i].TurnOn(false);
        }
    }

    // Turn on lights
    private void TurnOnOff(int[] _nums)
    {
        for (int i = 0; i < trLights.Count; i++)
        {
            trLights[i].TurnOn(false);
        }

        for (int i = 0; i < trLights.Count; i++)
        {
            for (int j = 0; j < _nums.Length; j++)
            {
                if (i == _nums[j])
                    trLights[i].TurnOn(true);
            }
        }
    }

    // Return start time for counting timer
    private float StartTime()
    {
        startTime = Time.time;

        isChange = false;
        return startTime;
    }

    // Flash yellow light
    private void FlashLight()
    {
        StartCoroutine(FlashingLight());
    }
    private IEnumerator FlashingLight()
    {
        while (isWork)
        {
            trLights[1].TurnOn(false);
            yield return new WaitForSeconds(delayFlash);

            trLights[1].TurnOn(true);
            yield return new WaitForSeconds(delayFlash);
        }
    }

    // Turn on/off all lights in turn
    private void CheckTrafficLights()
    {
        StartCoroutine(CheckingTrafficLight());
    }
    private IEnumerator CheckingTrafficLight()
    {
        while(isWork)
        {
            for (int i = 0; i < trLights.Count; i++)
            {
                trLights[i].TurnOn(true);
                yield return new WaitForSeconds(delayFlash);

                trLights[i].TurnOn(false);
                yield return new WaitForSeconds(delayFlash);
            }
        }
    }
}