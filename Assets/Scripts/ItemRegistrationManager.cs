using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Record;
using UnityEngine.EventSystems;

namespace CompanySystem
{
    public class ItemRegistrationManager : MonoBehaviour
    {
        public TMP_InputField skuInputField;
        public TMP_InputField itemNameInputField;
        public TMP_InputField binCodeInputField;
        public TMP_InputField quantityInputField;
        public TMP_Dropdown uomInputField;

        public GameObject itemTable;
        public GameObject popup;
        private GameObject warningPanel;

        // Register new item to system
        public void AddNewItem()
        {
            ItemRecord newItem = new ItemRecord(skuInputField.text, itemNameInputField.text, binCodeInputField.text, quantityInputField.text, uomInputField.captionText.text);
            var itemData = new Dictionary<string, object>
            {
                { "sku", newItem.Sku },
                { "itemname", newItem.ItemName },
                { "bincode", newItem.BinCode },
                { "quantity", newItem.Quantity },
                { "uom", newItem.UOM },
                { "active", newItem.Active },
                { "numberoftags", newItem.NumberOfTags }
            };

            StartCoroutine(FirebaseServices.WriteData("items", itemData, true, "sku", message =>
            {
                if (message.Contains("successfully"))
                {
                    HidePopup();
                    ResetInput();
                    gameObject.SetActive(false);
                    RefreshTable();
                }
                else if (message.Contains("registered"))
                {
                    ShowPopup();
                    ItemRegisteredHandler(message, newItem.Sku);
                }
                else
                {
                    Debug.LogError(message);
                }
            }));
        }
        // Reset input field
        public void ResetInput()
        {
            if (skuInputField.text.Length > 0)
                skuInputField.text = skuInputField.text.Remove(0);
            if (itemNameInputField.text.Length > 0)
                itemNameInputField.text = itemNameInputField.text.Remove(0);
            if (binCodeInputField.text.Length > 0)
                binCodeInputField.text = binCodeInputField.text.Remove(0);
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

        private void ItemRegisteredHandler(string message, string sku)
        {
            warningPanel = popup.transform.Find("Item Registered").gameObject;
            warningPanel.SetActive(true);

            TextMeshProUGUI warningText = warningPanel.GetComponentInChildren<TextMeshProUGUI>();
            warningText.text = message;

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
                StartCoroutine(FirebaseServices.DeleteData("items", "sku", sku, deleteResult =>
                {
                    if (deleteResult.Contains("successfully"))
                    {
                        AddNewItem();
                    }
                }));
            });
            trigger.triggers.Add(entry);
        }
    }
}