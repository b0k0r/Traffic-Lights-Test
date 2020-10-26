using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrafficLightsUI : MonoBehaviour
{
    public TrafficLights trafficLights;         // traffic lights controller

    public Toggle autoToggle;                   // toggle for automatic mode
    public Toggle manualToggle;                 // toggle for manual mode

    public Dropdown modeDropdown;               // dropdown for Mode list

    public Button startWorkBtn;                 // button for start/stop work
    public Text startWorkBtnText;               // label for startBtn

    public Button previousSignalBtn;            // button for change signal on previous signal
    public Button nextSignalBtn;                // button for change signal on next signal

    public Text currentSignalText;              // label for current signal 

    public Text timerText;                      // label for current work timer

    public Button closeBtn;                     // button for quit application

    public Toggle hideToggle;                   // toggle for hide ui
    public Text hideToggleLabel;                // label for toggle hide ui
    public GameObject UIobj;                    // parent ui


    private void Start()
    {
        modeDropdown.onValueChanged.AddListener(delegate 
        {
            ModeDropdownIndexChanged();
        });

        startWorkBtn.onClick.AddListener(delegate
        {
            StartButton();
        });

        previousSignalBtn.onClick.AddListener(delegate
        {
            PreviousSignalButton();
        });

        nextSignalBtn.onClick.AddListener(delegate
        {
            NextSignalButton();
        });


        if (!manualToggle.isOn)
        {
            previousSignalBtn.interactable = false;
            nextSignalBtn.interactable = false;
        }


        closeBtn.onClick.AddListener(delegate
        {
            CloseButton();
        });
    }

    private void Update()
    {
        if (trafficLights.IsWork())
        {
            TimerText();
            currentSignalText.text = trafficLights.CurrentLightSignal();
            modeDropdown.interactable = false;
        }
        else
        {
            modeDropdown.interactable = true;
        }
    }

    // Function for auto toggle
    public void AutoToggle()
    {
        if (autoToggle.isOn)
        {
            ModePopulationDropdown();
            Debug.Log("Selecting .. Automatic control .. ");
            trafficLights.AutomaticControl(true);

            previousSignalBtn.interactable = false;
            nextSignalBtn.interactable = false;
        }
    }

    // Function for manual toggle
    public void ManualToggle()
    {
        if (manualToggle.isOn)
        {
            ModePopulationDropdown();
            Debug.Log("Selecting .. Manual control .. ");
            trafficLights.AutomaticControl(false);


            previousSignalBtn.interactable = true;
            nextSignalBtn.interactable = true;
        }
    }

    // Mode list in dropdown
    private void ModePopulationDropdown()
    {
        modeDropdown.ClearOptions();
        string[] enumNames = System.Enum.GetNames(typeof(TrafficLightMode));
        List<string> names = new List<string>(enumNames);

        if(autoToggle.isOn)
            modeDropdown.AddOptions(names);
        else if(manualToggle.isOn)
        {
            List<string> n = new List<string>();
            for (int i = 0; i < 2; i++)
                n.Add(names[i]);

            modeDropdown.AddOptions(n);
        }
    }

    // Fuction for dropdown
    public void ModeDropdownIndexChanged()
    {
        TrafficLightMode mode = (TrafficLightMode)modeDropdown.value;
        trafficLights.SetMode(mode);

        switch (mode)
        {
            case TrafficLightMode.Off:
                currentSignalText.text = "";
                trafficLights.SetSignalNum((int)TrafficLightsSignal.Off);
                break;

            case TrafficLightMode.Regulated:
                currentSignalText.text = TrafficLightsSignal.Red.ToString();
                trafficLights.SetSignalNum((int)TrafficLightsSignal.Off);
                break;

            case TrafficLightMode.NotRegulated:
                currentSignalText.text = TrafficLightsSignal.FlashingYellow.ToString();
                trafficLights.SetSignalNum((int)TrafficLightsSignal.FlashingYellow);
                break;

            case TrafficLightMode.Check:
                currentSignalText.text = TrafficLightsSignal.Red.ToString();
                trafficLights.SetSignalNum((int)TrafficLightsSignal.Red);
                break;

        }

        Debug.Log("Selecting " + modeDropdown.options[modeDropdown.value].text + ".. mode .. ");
    }

    // Start/stop work
    public void StartButton()
    {
        if (trafficLights.IsWork())
        {
            startWorkBtnText.text = "Start";
            trafficLights.StartWork(false);
        }
        else
        {
            startWorkBtnText.text = "Stop";

            trafficLights.StartWork(true);
        }
    }

    // Previous signal if manual control
    public void PreviousSignalButton()
    {
        TrafficLightsSignal signal = (TrafficLightsSignal)trafficLights.DecreaseSignalNum();
        currentSignalText.text = signal.ToString();
        trafficLights.ChangeSignal((int)signal);
    }

    // Next signal of automatic control
    public void NextSignalButton()
    {
        TrafficLightsSignal signal = (TrafficLightsSignal)trafficLights.IncreaseSignalNum();
        currentSignalText.text = signal.ToString();
        trafficLights.ChangeSignal((int)signal);
    }

    // Timer text
    public void TimerText()
    {
        timerText.text = trafficLights.Timer().ToString("0");
    }

    // Apllication Quit
    public void CloseButton()
    {
        Application.Quit();
    }

    // Hide/unhide UI
    public void HideToggle()
    {
        if (hideToggle.isOn)
        {
            UIobj.SetActive(false);
            hideToggleLabel.text = "";
        }
        else 
        {
            hideToggleLabel.text = "Hide UI";
            UIobj.SetActive(true);
        }
    }

}