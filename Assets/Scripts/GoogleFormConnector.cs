using dotenv.net;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class GoogleFormConnector : MonoBehaviour
{
    private string formId;
    private string baseUrl;

    void Start()
    {
        // Initialize base URL
        DotEnv.Load();
        formId = Environment.GetEnvironmentVariable("FORM_ID");
        baseUrl = $"https://docs.google.com/forms/u/0/d/e/{formId}/formResponse";
    }
    public IEnumerator WriteData(string code, string information)
    {
        WWWForm form = new WWWForm();
        form.AddField("entry.1212823721", code);
        form.AddField("entry.1610947420", information);

        using (UnityWebRequest www = UnityWebRequest.Post(baseUrl, form))
        {
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
