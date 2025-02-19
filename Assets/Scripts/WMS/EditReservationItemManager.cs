using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CompanySystem
{
    public class EditReservationItemManager : MonoBehaviour
    {
        public TMP_Dropdown itemDropdown;
        public TMP_InputField quantityInputField;

        public GameObject itemTable;

        public GameObject popup;
        private GameObject warningPanel;

        private void OnEnable()
        {
            // Invoke GetData method after a short delay
            Invoke("GetItems", 0.1f);
        }

        private void OnDisable()
        {
            itemDropdown.options.Clear();
        }

        public void GetItems()
        {
            StartCoroutine(FirebaseServices.ReadData("items", data =>
            {
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        itemDropdown.options.Add(new TMP_Dropdown.OptionData($"{item["sku"]} - {item["item_name"]}"));
                    }
                    LoadItem();
                }
                else
                {
                    Debug.LogError("Failed to retrieve data.");
                }
            }));
        }

        private void LoadItem()
        {
            foreach (var option in itemDropdown.options)
            {
                if (option.text == $"{ReservationDetailManager.selectedRecord.Sku} - {ReservationDetailManager.selectedRecord.ItemName}")
                {
                    itemDropdown.value = itemDropdown.options.IndexOf(option);
                    break;
                }
            }
            quantityInputField.text = ReservationDetailManager.selectedRecord.Quantity.ToString();
        }

        // Edit item to reservation
        public void EditItem()
        {
            string[] selectedItem = itemDropdown.captionText.text.Split('-');
            string sku = selectedItem[0].Trim();

            StartCoroutine(FirebaseServices.ReadData("items", "sku", sku, data =>
            {
                if (data != null)
                {
                    StartCoroutine(UpdateData(data));
                }
                else
                {
                    Debug.LogError("Failed to retrieve data.");
                }
            }));
        }

        private IEnumerator UpdateData(JObject data)
        {
            string code = ReservationListManager.selectedRecord.Code;
            int quantity = int.Parse(quantityInputField.text);
            string oldSku = ReservationDetailManager.selectedRecord.Sku;
            bool reservationSuccess = false;

            var newReservationItem = new Dictionary<string, object>
            {
                { "sku", data["sku"].ToString() },
                { "item_name", data["item_name"].ToString() },
                { "quantity", quantity },
                { "information", "Unprocessed" },
                { "packed", false }
            };

            StartCoroutine(FirebaseServices.ModifyData("reservations", "code", code, "items", newReservationItem, oldSku, "sku", message =>
            {

                if (message.Contains("successfully"))
                {
                    HidePopup();
                    ResetInput();
                    reservationSuccess = true;
                }
                else if (message.Contains("registered"))
                {
                    ShowPopup();
                    ItemRegisteredHandler(message, code, newReservationItem["sku"].ToString());
                }
                else
                {
                    Debug.LogError("Failed to add item to reservation.");
                }
            }));

            yield return new WaitUntil(() => reservationSuccess);
            if (reservationSuccess)
            {
                yield return new WaitForSeconds(0.1f);
                RefreshTable();
                gameObject.SetActive(false);
            }
        }

        // Reset input field
        public void ResetInput()
        {
            if (quantityInputField.text.Length > 0)
                quantityInputField.text = quantityInputField.text.Remove(0);
        }
        private void RefreshTable()
        {
            itemTable.SetActive(false);
            itemTable.SetActive(true);
        }

        private void ShowPopup()
        {
            popup.SetActive(true);
        }

        private void HidePopup()
        {
            if (warningPanel != null && warningPanel.activeSelf)
            {
                warningPanel.transform.parent.gameObject.SetActive(false);
                warningPanel.SetActive(false);
            }
        }

        private void ItemRegisteredHandler(string message, string code, string sku)
        {
            warningPanel = popup.transform.Find("Item Registered").gameObject;
            warningPanel.SetActive(true);

            TextMeshProUGUI warningText = warningPanel.GetComponentInChildren<TextMeshProUGUI>();
            warningText.text = message + "\r\nReplace data item?";

            GameObject replaceItemButton = warningPanel.transform.Find("Buttons").Find("Yes Button").gameObject;

            // Add event trigger to replace bin button
            EventTrigger trigger = replaceItemButton.GetComponent<EventTrigger>() ?? replaceItemButton.AddComponent<EventTrigger>();
            trigger.triggers.Clear();

            // Create entry for click/ pointer down event
            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerDown
            };

            entry.callback.AddListener((eventData) =>
            {
                StartCoroutine(FirebaseServices.DeleteData("reservations", "code", code, "items", "sku", sku, deleteResult =>
                {
                    if (deleteResult.Contains("successfully"))
                    {
                        EditItem();
                    }
                }));
            });
            trigger.triggers.Add(entry);
        }
    }
}
