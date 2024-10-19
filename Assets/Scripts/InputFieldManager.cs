using Keyboard;
using TMPro;
using UnityEngine;

public class InputFieldManager : MonoBehaviour
{
    public KeyboardManager keyboardManager;
    public TMP_InputField codeInputField;
    public TMP_InputField informationInputField;
    public TMP_InputField usernameInputField;
    public TMP_InputField passwordInputField;

    public void InputCode()
    {
        keyboardManager.outputField = codeInputField;
    }
    public void InputInformation()
    {
        keyboardManager.outputField = informationInputField;
    }
    public void InputUsername()
    {
        keyboardManager.outputField = usernameInputField;
    }
    public void InputPassword()
    {
        keyboardManager.outputField = passwordInputField;
    }
}
