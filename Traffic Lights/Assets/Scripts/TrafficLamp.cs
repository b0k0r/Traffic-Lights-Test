using UnityEngine;

public class TrafficLamp : MonoBehaviour
{
    public bool isOn = false;               // lamp works or no
       
    public Material materialOn;             // material for lamp if lamp works
    public Material materialOff;            // material for lamp if lamp not works

    public GameObject loghtObj;             // light for lamp

    MeshRenderer meshRenderer;              // mesh renderer of this lamp

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }
    
    // Turn on/off lamp
    public void TurnOn(bool _on)
    {
        isOn = _on;

        if (isOn)
        {
            meshRenderer.material = materialOn;
            loghtObj.SetActive(true);
        }
        else
        {
            meshRenderer.material = materialOff;
            loghtObj.SetActive(false);
        }
    }
}