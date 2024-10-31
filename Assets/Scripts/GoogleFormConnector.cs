using dotenv.net;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class GoogleFormConnector : MonoBehaviour
{
    private string formId; // Variable to hold the form ID
    private string baseUrl; // Variable to hold the base URL of the Google Form

    void Start()
    {
        // Initialize base URL
        DotEnv.Load();
        formId = Environment.GetEnvironmentVariable("FORM_ID");
        baseUrl = $"https://docs.google.com/forms/u/0/d/e/{formId}/formResponse";
    }

    public IEnumerator WriteData(string code, string information)
    {
        WWWForm form = new WWWForm(); // Create a new form
        form.AddField("entry.1212823721", code); // Add the code field to the form
        form.AddField("entry.1610947420", information); // Add the code field to the form

        using (UnityWebRequest www = UnityWebRequest.Post(baseUrl, form))
        {
            // Send the form data to the server and wait for a response
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Debug.Log("Form successfully submitted");
            }
        }
    }
}
