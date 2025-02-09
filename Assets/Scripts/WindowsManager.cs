using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowsManager : MonoBehaviour
{
    [SerializeField]
    private GameObject window; // Window game object
    [SerializeField]
    private GameObject companySystem; // Company system game object
    [SerializeField]
    private GameObject rfid; // RFID game object
    [SerializeField]
    private GameObject pomQm; // POM QM game object

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenWindow()
    {
        window.SetActive(true);
    }

    public void CloseWindow()
    {
        GameObject windowPanel = window.transform.Find("Background").gameObject;
        foreach (Transform child in windowPanel.transform)
        {
            if (child.gameObject.name != "Power Button")
                child.gameObject.SetActive(false);
        }
        window.SetActive(false);
    }
}
