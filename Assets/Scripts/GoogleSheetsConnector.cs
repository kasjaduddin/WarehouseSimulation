using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using dotenv.net;
using System;

public class GoogleSheetsConnector : MonoBehaviour
{
    private string apiKey;
    private string sheetId;
    private string baseUrl;

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