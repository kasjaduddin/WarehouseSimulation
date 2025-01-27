using System.Transactions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace CompanySystem
{
    public class TransactionItemListManager : MonoBehaviour
    {
        public TextMeshProUGUI transactionCode;
        public TextMeshProUGUI transactionInvoiceNumber;
        public TextMeshProUGUI transactionInvoiceDate;
        public TextMeshProUGUI transactionVendor;

        public GameObject Table;
        public Transform container; // Container to hold the instantiated records
        public GameObject recordTemplate; // Template for displaying each record

        void OnEnable()
        {
            // Invoke GetData method after a short delay
            Invoke("LoadData", 0.1f);
        }

        private void OnDisable()
        {
            DestroyRecord();
        }

        private void LoadData()
        {
            transactionCode.text = TransactionListManager.selectedRecord.Code;
            transactionInvoiceNumber.text = TransactionListManager.selectedRecord.InvoiceNumber;
            transactionInvoiceDate.text = TransactionListManager.selectedRecord.InvoiceDate;
            transactionVendor.text = TransactionListManager.selectedRecord.Vendor;

            ShowRecord();
        }

        void ShowRecord()
        {
            float templateHigh = 91f;
            for (int i = 0; i < TransactionListManager.selectedRecord.Items.Count; i++)
            {
                GameObject newRow = Instantiate(recordTemplate, container);
                Transform newRowTransform = newRow.transform;
                RectTransform entryRectTransform = newRow.GetComponent<RectTransform>();

                // Fill the UI elements with data
                entryRectTransform.anchoredPosition = new Vector2(0f, 228f + (-templateHigh * i));
                newRowTransform.Find("SKU").GetComponent<TextMeshProUGUI>().text = TransactionListManager.selectedRecord.Items[i].Sku;
                newRowTransform.Find("Item Name").GetComponent<TextMeshProUGUI>().text = TransactionListManager.selectedRecord.Items[i].ItemName;
                newRowTransform.Find("Quantity").GetComponent<TextMeshProUGUI>().text = TransactionListManager.selectedRecord.Items[i].Quantity.ToString();
                newRowTransform.Find("Information").GetComponent<TextMeshProUGUI>().text = TransactionListManager.selectedRecord.Items[i].Information;

                if (TransactionListManager.selectedRecord.Items[i].Status.Equals("Approved"))
                {
                    newRowTransform.Find("Record Background").GetComponent<Image>().color = new Color32(111, 191, 177, 255);
                }
                else if (TransactionListManager.selectedRecord.Items[i].Status.Equals("Not approved"))
                {
                    newRowTransform.Find("Record Background").GetComponent<Image>().color = new Color32(210, 102, 90, 255);
                }
                if (i % 2 != 0)
                {
                    newRowTransform.Find("Record Background").GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                }

                Table.GetComponent<DynamicTableManager>().enabled = true;
            }
            recordTemplate.SetActive(false);
        }

        public void DestroyRecord()
        {
            foreach (Transform child in container)
            {
                //if (child != recordTemplate.transform && child != editButton.transform)
                //{
                //    Destroy(child.gameObject);
                //}
            }
            recordTemplate.SetActive(true);
            //editButton.SetActive(false);
        }
    }
}
