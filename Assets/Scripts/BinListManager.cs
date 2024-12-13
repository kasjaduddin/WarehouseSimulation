using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using TMPro;

public class BinListManager : MonoBehaviour
{
    public GoogleSheetsConnector connector; // Reference to the Google Sheets connector
    private JArray bins; // Array to store bins data
    public GameObject recordTemplate; // Template for displaying each record
    public Transform container; // Container to hold the instantiated records

    // Start is called before the first frame update
    void OnEnable()
    {
        // Invoke GetData method after a short delay
        bins = JArray.Parse(@"[[null, '', '']]");
        Invoke("GetData", 0.1f);
    }

    // Get all bin data from Google Sheets
    public void GetData()
    {
        StartCoroutine(connector.ReadData("Bins", (data) =>
        {
            // Parse JSON response
            JObject parsedData = JObject.Parse(data);
            bins = (JArray)parsedData["values"];
            bins.RemoveAt(0);
            ShowRecord();
        }));
    }

    // Display all bin data to bin table
    void ShowRecord()
    {
        float templateHigh = 91f;
        for (int i = 0; i < bins.Count; i++) 
        {
            GameObject newRow = Instantiate(recordTemplate, container);
            Transform newRowTransform = newRow.transform;
            RectTransform entryRectTransform = newRow.GetComponent<RectTransform>();

            // Fill the UI elements with data
            entryRectTransform.anchoredPosition = new Vector2(0f, 228f + (-templateHigh * i));
            newRowTransform.Find("Id").GetComponent<TextMeshProUGUI>().text = (i + 1).ToString();
            newRowTransform.Find("Code").GetComponent<TextMeshProUGUI>().text = bins[i][1].ToString();
            newRowTransform.Find("Information").GetComponent<TextMeshProUGUI>().text = bins[i][2].ToString();
        }
        recordTemplate.SetActive(false);
    }

    private void GetRecordData(Transform recordTransform)
    {
        string id = recordTransform.Find("Id").GetComponent<TextMeshProUGUI>().text;
        string code = recordTransform.Find("Code").GetComponent<TextMeshProUGUI>().text;
        string information = recordTransform.Find("Information").GetComponent<TextMeshProUGUI>().text;

        // Output the data to the console
        Debug.Log($"Id: {id}, Code: {code}, Information: {information}");
    }
    // Edit selected record
    public void EditRecord(Transform recordTransform)
    {
        GetRecordData(recordTransform);
    }

    // Delete shows all bin data in the bin table
    public void DestroyRecord()
    {
        foreach (Transform child in container)
        {
            if (child != recordTemplate.transform)
            {
                Destroy(child.gameObject);
            }
        }
        recordTemplate.SetActive(true);
    }
}