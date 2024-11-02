using UnityEngine;
using TMPro;

public class BinRegistrationManager : MonoBehaviour
{
    public GoogleFormConnector connector; // Reference to the Google Form connector
    public TextMeshProUGUI codeInputField; // Input field for the bin code
    public TextMeshProUGUI informationInputField; // Input field for additional information
    private string code; // Variable to store the bin code
    private string information; // Variable to store the additional information

    // Register new bin to system
    public void AddNewBin()
    {
        code = codeInputField.text;
        information = informationInputField.text;

        // Write data to the Google Form
        StartCoroutine(connector.WriteData(code, information));
    }
}
