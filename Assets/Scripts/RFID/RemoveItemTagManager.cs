using CompanySystem;
using Newtonsoft.Json.Linq;
using Record;
using System.Collections;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Record.ReservationRecord;
using static UnityEngine.Rendering.DebugUI;

namespace Rfid
{
    public class RemoveItemTagManager : MonoBehaviour
    {
        private JArray reservations; // Array to store reservations data
        private GameObject reservationTable;
        private Transform reservationContainer; // Container to hold the instantiated records
        private GameObject reservationRecordTemplate; // Template for displaying each record
        private ReservationRecord reservationRecord;

        private GameObject itemTable;
        private Transform itemContainer; // Container to hold the instantiated records
        private GameObject itemRecordTemplate; // Template for displaying each record
        //private ReservationRecord itemRecord;

        private void OnEnable()
        {
            reservationTable = transform.Find("Reservation List").gameObject;
            reservationContainer = reservationTable.transform.Find("Table Container");
            reservationRecordTemplate = reservationContainer.Find("Record Template").gameObject;

            itemTable = transform.Find("Item List").gameObject;
            itemContainer = itemTable.transform.Find("Table Container");
            itemRecordTemplate = itemContainer.Find("Record Template").gameObject;

            Invoke("LoadReservationData", 0.1f);
        }

        private void OnDisable()
        {
            DestroyReservationRecord();
            DestroyItemRecord();
        }

        public void LoadReservationData()
        {
            DestroyReservationRecord();
            StartCoroutine(FirebaseServices.ReadData("reservations", data =>
            {
                if (data != null)
                {
                    reservations = data;
                    ShowReservation();
                }
                else
                {
                    Debug.LogError("Failed to retrieve data.");
                }
            }));
        }

        public void SelectReservation(TextMeshProUGUI code)
        {
            UnselectRecord();
            code.transform.parent.GetComponent<Image>().color = new Color32(4, 83, 221, 255);
            StartCoroutine(GetItems(code.text));
        }

        private void ShowReservation()
        {
            float templateHigh = 32f;
            for (int i = 0; i < reservations.Count; i++)
            {
                GameObject newRow = Instantiate(reservationRecordTemplate, reservationContainer);
                Transform newRowTransform = newRow.transform;
                RectTransform entryRectTransform = newRow.GetComponent<RectTransform>();

                // Fill the UI elements with data
                entryRectTransform.anchoredPosition = new Vector2(0f, 46f + (-templateHigh * i));
                newRowTransform.Find("Button").Find("Code").GetComponent<TextMeshProUGUI>().text = reservations[i]["code"].ToString();
                
                reservationTable.GetComponent<DynamicTableManager>().enabled = true;
            }
            reservationRecordTemplate.SetActive(false);
        }

        private IEnumerator GetItems(string code)
        {
            transform.Find("Reservation Code").GetComponent<TextMeshProUGUI>().text = code;

            StartCoroutine(FirebaseServices.ReadData("reservations", "code", code, data =>
            {
                if (data != null)
                {
                    ReservationRecord record = new ReservationRecord(data);
                    reservationRecord = record;
                }
                else
                {
                    Debug.LogError("Failed to retrieve data.");
                }
            }));

            yield return new WaitForSeconds(0.2f);
            StartCoroutine(ShowItems());
        }

        private IEnumerator ShowItems()
        {
            float templateHigh = 32f;
            int i = 0;
            for (int j = 0; j < reservationRecord.Items.Count; j++)
            {
                if (reservationRecord.Items[j].Information.Equals("approved") && !reservationRecord.Items[j].Packed)
                {
                    ItemRecord item = new ItemRecord();
                    StartCoroutine(FirebaseServices.ReadData("items", "sku", reservationRecord.Items[j].Sku, data =>
                    {
                        if (data != null)
                        {
                            ItemRecord record = new ItemRecord(data);
                            item = record;
                        }
                        else
                        {
                            Debug.LogError("Failed to retrieve data.");
                        }
                    }));

                    yield return new WaitUntil(() => item.Sku == reservationRecord.Items[j].Sku);

                    GameObject newRow = Instantiate(itemRecordTemplate, itemContainer);
                    Transform newRowTransform = newRow.transform;
                    RectTransform entryRectTransform = newRow.GetComponent<RectTransform>();

                    // Fill the UI elements with data
                    entryRectTransform.anchoredPosition = new Vector2(0f, 46f + (-templateHigh * i));
                    newRowTransform.Find("No").GetComponent<TextMeshProUGUI>().text = (i + 1).ToString();
                    newRowTransform.Find("Bin Code").GetComponent<TextMeshProUGUI>().text = item.BinCode;
                    newRowTransform.Find("SKU").GetComponent<TextMeshProUGUI>().text = item.Sku;
                    newRowTransform.Find("Item Name").GetComponent<TextMeshProUGUI>().text = item.ItemName;
                    newRowTransform.Find("Quantity").GetComponent<TextMeshProUGUI>().text = reservationRecord.Items[i].Quantity.ToString();
                    newRowTransform.Find("Stock").GetComponent<TextMeshProUGUI>().text = item.Quantity.ToString();
                    i++;
                }
                itemTable.GetComponent<DynamicTableManager>().enabled = true;
            }
            itemRecordTemplate.SetActive(false);
        }

        private void DestroyReservationRecord()
        {
            foreach (Transform child in reservationContainer)
            {
                if (child != reservationRecordTemplate.transform)
                {
                    Destroy(child.gameObject);
                }
            }
            reservationRecordTemplate.SetActive(true);
        }

        private void DestroyItemRecord()
        {
            foreach (Transform child in itemContainer)
            {
                if (child != itemRecordTemplate.transform)
                {
                    Destroy(child.gameObject);
                }
            }
            itemRecordTemplate.SetActive(true);
        }

        private void UnselectRecord()
        {
            foreach (Transform child in reservationContainer)
            {
                child.Find("Button").GetComponent<Image>().color = new Color32(255, 255, 255, 0);
            }

            DestroyItemRecord();
        }
    }
}