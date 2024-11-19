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
    public TMP_InputField binCodeInputField; // Input field for bin code
    public TMP_InputField tagDescriptionInputField; // Input field for tag description
    public TMP_InputField numberOfPeriodsField; // Input field for number of periods

    public void InputNumberOfPeriods()
    {
        keyboardManager.outputField = numberOfPeriodsField;
    }
    // Set the output field of the keyboard manager to the input field
    public void KeyboardInput(TMP_InputField inputField) 
    { 
        keyboardManager.outputField = inputField;
    }
}
