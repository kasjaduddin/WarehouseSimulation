using Keyboard;
using TMPro;
using UnityEngine;

public class InputFieldManager : MonoBehaviour
{
    public KeyboardManager keyboardManager; // Reference to the keyboard manager
    public TMP_InputField codeInputField; // Input field for bin code
    public TMP_InputField informationInputField; // Input field for bin information
    public TMP_InputField usernameInputField; // Input field for user name
    public TMP_InputField passwordInputField; // Input field for account password
    public TMP_InputField numberOfPeriodsField; // Input field for number of periods

    // Set the output field of the keyboard manager to the bin code input field
    public void InputCode()
    {
        keyboardManager.outputField = codeInputField;
    }

    // Set the output field of the keyboard manager to the bin information input field
    public void InputInformation()
    {
        keyboardManager.outputField = informationInputField;
    }

    // Set the output field of the keyboard manager to the user name input field
    public void InputUsername()
    {
        keyboardManager.outputField = usernameInputField;
    }

    // Set the output field of the keyboard manager to the account password input field
    public void InputPassword()
    {
        keyboardManager.outputField = passwordInputField;
    }
    public void InputNumberOfPeriods()
    {
        keyboardManager.outputField = numberOfPeriodsField;
    }

    public void InputQuantityRecord(TMP_InputField inputField) 
    { 
        keyboardManager.outputField = inputField;
    }
}
