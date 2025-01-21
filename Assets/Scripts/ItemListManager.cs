using Newtonsoft.Json.Linq;
using Record;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.LookDev;

namespace CompanySystem
{
    public class ItemListManager : MonoBehaviour
    {
        private JArray items; // Array to store items data
        public GameObject Table;
        public Transform container; // Container to hold the instantiated records
        public GameObject recordTemplate; // Template for displaying each record
        public static ItemRecord selectedRecord; // Variabel to hold selected record data

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

        // Get all bin data from Database
        public void GetData()
        {
            StartCoroutine(FirebaseServices.ReadData("items", data =>
            {
                if (data != null)
                {
                    items = data;
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
            for (int i = 0; i < items.Count; i++)
            {
                GameObject newRow = Instantiate(recordTemplate, container);
                Transform newRowTransform = newRow.transform;
                RectTransform entryRectTransform = newRow.GetComponent<RectTransform>();

                // Fill the UI elements with data
                entryRectTransform.anchoredPosition = new Vector2(0f, 228f + (-templateHigh * i));
                newRowTransform.Find("Id").GetComponent<TextMeshProUGUI>().text = (i + 1).ToString();
                newRowTransform.Find("SKU").GetComponent<TextMeshProUGUI>().text = items[i]["sku"].ToString();
                newRowTransform.Find("Item Name").GetComponent<TextMeshProUGUI>().text = items[i]["itemname"].ToString();
                newRowTransform.Find("Bin Code").GetComponent<TextMeshProUGUI>().text = items[i]["bincode"].ToString();
                newRowTransform.Find("Quantity").GetComponent<TextMeshProUGUI>().text = items[i]["quantity"].ToString();
                newRowTransform.Find("Number of Tag").GetComponent<TextMeshProUGUI>().text = items[i]["numberoftags"].ToString();
                newRowTransform.Find("UOM").GetComponent<TextMeshProUGUI>().text = items[i]["uom"].ToString();

                Table.GetComponent<DynamicTableManager>().enabled = true;
            }
            recordTemplate.SetActive(false);
        }

        private ItemRecord GetRecord(Transform recordTransform)
        {
            string id = recordTransform.Find("Id").GetComponent<TextMeshProUGUI>().text;
            string sku = recordTransform.Find("SKU").GetComponent<TextMeshProUGUI>().text;
            string itemName = recordTransform.Find("Item Name").GetComponent<TextMeshProUGUI>().text;
            string binCode = recordTransform.Find("Bin Code").GetComponent<TextMeshProUGUI>().text;
            string uom = recordTransform.Find("UOM").GetComponent<TextMeshProUGUI>().text;

            // Create a ItemRecord struct and return it
            ItemRecord record = new ItemRecord(id, sku, itemName, binCode, uom);
            return record;
        }

        public static void ResetSelectedRecord()
        {
            ItemRecord emptyItem = new ItemRecord(null, null, null, null, null);
            selectedRecord = emptyItem;
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
                if (child != recordTemplate.transform && child != deleteButton.transform)
                {
                    Destroy(child.gameObject);
                }
            }
            recordTemplate.SetActive(true);
            deleteButton.SetActive(false);
        }
    }
}