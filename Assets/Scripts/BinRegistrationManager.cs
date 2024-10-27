using UnityEngine;
using TMPro;

public class BinRegistrationManager : MonoBehaviour
{
    public GoogleFormConnector connector;
    public TextMeshProUGUI codeInputField;
    public TextMeshProUGUI informationInputField;
    private string code;
    private string information;
    void Start()
    {

    }
    public void AddNewBin()
    {
        code = codeInputField.text;
        information = informationInputField.text;

        StartCoroutine(connector.WriteData(code, information));
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
