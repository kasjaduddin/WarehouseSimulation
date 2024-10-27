using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using TMPro;

public class BinListManager : MonoBehaviour
{

    public GoogleSheetsConnector connector;
    private JArray bins;
    public GameObject recordTemplate;
    public Transform container;
    // Start is called before the first frame update
    void OnEnable()
    {
        bins = JArray.Parse(@"[[null, '', '']]");
        Invoke("GetData", 0.1f);
    }
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

    void ShowRecord()
    {
        float templateHigh = 91f;
        for (int i = 0; i < bins.Count; i++) 
        {
            GameObject newRow = Instantiate(recordTemplate, container);
            Transform newRowTransform = newRow.transform;
            RectTransform entryRectTransform = newRow.GetComponent<RectTransform>();
            Image entryImage = newRow.GetComponent<Image>();
            entryRectTransform.anchoredPosition = new Vector2(0f, -templateHigh * i);
            //entryImage.Find("Record Background").GetComponent<Image>().
            newRowTransform.Find("Id").GetComponent<TextMeshProUGUI>().text = (i + 1).ToString();
            newRowTransform.Find("Code").GetComponent<TextMeshProUGUI>().text = bins[i][1].ToString();
            newRowTransform.Find("Information").GetComponent<TextMeshProUGUI>().text = bins[i][2].ToString();
        }
        recordTemplate.SetActive(false);
    }
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