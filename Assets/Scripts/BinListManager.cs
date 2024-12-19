using UnityEngine;
using Newtonsoft.Json.Linq;
using TMPro;
using Record;


namespace CompanySystem
{
    public class BinListManager : MonoBehaviour
    {
        private JArray bins; // Array to store bins data
        public GameObject recordTemplate; // Template for displaying each record
        public Transform container; // Container to hold the instantiated records

        public static BinRecord selectedRecord; // Variabel to hold selected record data

        public GameObject deleteButton; // Button to delete selected record data

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
                    recordTemplate.SetActive(false);
                    Debug.LogError("Failed to retrieve data.");
                }
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
                newRowTransform.Find("Code").GetComponent<TextMeshProUGUI>().text = bins[i]["code"].ToString();
                newRowTransform.Find("Information").GetComponent<TextMeshProUGUI>().text = bins[i]["information"].ToString();
                newRowTransform.Find("Number of Tag").GetComponent<TextMeshProUGUI>().text = bins[i]["numberoftags"].ToString();
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

        // Edit selected record
        public void ShowDeleteButton(Transform recordTransform)
        {
            selectedRecord = GetRecord(recordTransform);

            // Show delete button under expand button
            deleteButton.SetActive(true);
            float recordY = recordTransform.GetComponent<RectTransform>().anchoredPosition.y;
            RectTransform deleteButtonRectTransform = deleteButton.GetComponent<RectTransform>();
            deleteButtonRectTransform.anchoredPosition = new Vector2(deleteButtonRectTransform.anchoredPosition.x, recordY - 46f);
            deleteButton.transform.SetAsLastSibling();
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