using UnityEngine;
using Newtonsoft.Json.Linq;
using TMPro;
using Record;


namespace CompanySystem
{
    public class BinListManager : MonoBehaviour
    {
        //public GoogleSheetsConnector connector; // Reference to the Google Sheets connector
        private JArray bins; // Array to store bins data
        public GameObject recordTemplate; // Template for displaying each record
        public Transform container; // Container to hold the instantiated records
        public static BinRecord selectedRecord;

        // Start is called before the first frame update
        void OnEnable()
        {
            // Invoke GetData method after a short delay
            Invoke("GetData", 0.1f);
        }

        private void OnDisable()
        {
            DestroyRecord();
        }

        // Get all bin data from Google Sheets
        public void GetData()
        {
            StartCoroutine(FirebaseServices.ReadData("bins", data =>
            {
                if (data != null)
                {
                    bins = data;
                    ShowRecord();
                }
                else
                {
                    Debug.LogError("Failed to retrieve data.");
                }
            }));
        }

        // Display all bin data to bin table
        void ShowRecord()
        {
            float templateHigh = 91f;
            foreach (var bin in bins)
            {
                GameObject newRow = Instantiate(recordTemplate, container);
                Transform newRowTransform = newRow.transform;
                RectTransform entryRectTransform = newRow.GetComponent<RectTransform>();

                // Fill the UI elements with data
                entryRectTransform.anchoredPosition = new Vector2(0f, 228f + (-templateHigh * ((int)bin["id"] - 1)));
                newRowTransform.Find("Id").GetComponent<TextMeshProUGUI>().text = bin["id"].ToString();
                newRowTransform.Find("Code").GetComponent<TextMeshProUGUI>().text = bin["code"].ToString();
                newRowTransform.Find("Information").GetComponent<TextMeshProUGUI>().text = bin["information"].ToString();
                newRowTransform.Find("Number of Tag").GetComponent<TextMeshProUGUI>().text = bin["numberoftags"].ToString();
            }
            recordTemplate.SetActive(false);
        }

        private BinRecord GetRecord(Transform recordTransform)
        {
            string id = recordTransform.Find("Id").GetComponent<TextMeshProUGUI>().text;
            string code = recordTransform.Find("Code").GetComponent<TextMeshProUGUI>().text;
            string information = recordTransform.Find("Information").GetComponent<TextMeshProUGUI>().text;

            // Create a BinRecord struct and return it
            BinRecord record = new BinRecord(id, code, information);
            return record;
        }

        // Edit selected record
        public void EditRecord(Transform recordTransform)
        {
            selectedRecord = GetRecord(recordTransform);
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
}