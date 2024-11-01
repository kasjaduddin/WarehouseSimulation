using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using dotenv.net;
using System;

public class GoogleSheetsConnector : MonoBehaviour
{
    private string apiKey; // Variable to hold the API Key
    private string sheetId; // Variable to hold the sheet ID
    private string baseUrl; // Variable to hold the base URL of the Google Sheets

    void Start()
    {
        // Initialize base URL
        DotEnv.Load();
        apiKey = Environment.GetEnvironmentVariable("API_KEY");
        sheetId = Environment.GetEnvironmentVariable("SHEET_ID");
        baseUrl = $"https://sheets.googleapis.com/v4/spreadsheets/{sheetId}/values/{{0}}?key={apiKey}";
    }

    public IEnumerator ReadData(string sheetName, System.Action<string> callback)
    {
        string url = string.Format(baseUrl, sheetName);

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            // Send the GET request to the server and wait for a response
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                callback(www.downloadHandler.text);
            }
        }
    }
}