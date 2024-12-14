using UnityEngine;
using TMPro;

public class BinRegistrationManager : MonoBehaviour
{
    public GoogleFormConnector connector; // Reference to the Google Form connector
    public TMP_InputField codeInputField;
    public TMP_InputField informationInputField;
    private string code; // Variable to store the bin code
    private string information; // Variable to store the additional information

    // Register new bin to system
    public void AddNewBin()
    {
        code = codeInputField.text;
        information = informationInputField.text;

        // Write data to the Google Form
        StartCoroutine(connector.WriteData(code, information));
        ResetInput();
    }

    // Reset input field
    public void ResetInput()
    {
        codeInputField.text = codeInputField.text.Remove(0);
        informationInputField.text = informationInputField.text.Remove(0);
    }
}
