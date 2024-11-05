using UnityEngine;
using Newtonsoft.Json.Linq;
using TMPro;

public class RfidLoginManager : MonoBehaviour
{
    public GoogleSheetsConnector connector;
    private JArray accounts;
    public TextMeshProUGUI usernameInputField;
    public TextMeshProUGUI passwordInputField;
    private JArray userInput;
    private string username;
    private string password;
    void Start()
    {
        StartCoroutine(connector.ReadData("Accounts", (data) =>
        {
            // Parse JSON response
            JObject parsedData = JObject.Parse(data);
            accounts = (JArray)parsedData["values"];
            accounts.RemoveAt(0);
        }));
    }

    public void CheckCredential()
    {
        username = usernameInputField.text;
        password = passwordInputField.text;
        username = username.Substring(0, username.Length - 1);
        password = password.Substring(0, password.Length - 1);

        foreach (JArray account in accounts)
        {
            if (username == account[0].ToString())
                if (password == account[1].ToString())
                {
                    Debug.Log($"Login as {username}");
                    break;
                }
                else
                    Debug.Log("Wrong password");
            else
                Debug.Log($"User with username {username} not found");
        }
    }
}