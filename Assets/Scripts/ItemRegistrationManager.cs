using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Record;

namespace CompanySystem
{
    public class ItemRegistrationManager : MonoBehaviour
    {
        public TMP_InputField skuInputField;
        public TMP_InputField itemNameInputField;
        public TMP_InputField binCodeInputField;
        public TMP_InputField quantityInputField;
        public TMP_InputField uomInputField;

        public GameObject itemTable;
        private GameObject warningPanel;

        // Register new item to system
        public void AddNewItem()
        {
            ItemRecord newItem = new ItemRecord(skuInputField.text, itemNameInputField.text, binCodeInputField.text, quantityInputField.text, uomInputField.text);
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

            StartCoroutine(FirebaseServices.WriteData("items", itemData, false, null, message =>
            {
                if (message.Contains("successfully"))
                {
                    HidePopup();
                    ResetInput();
                    gameObject.SetActive(false);
                    RefreshTable();
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
            if (uomInputField.text.Length > 0)
                uomInputField.text = uomInputField.text.Remove(0);
        }
        private void RefreshTable()
        {
            itemTable.SetActive(false);
            itemTable.SetActive(true);
        }

        private void HidePopup()
        {
            if (warningPanel != null && warningPanel.activeSelf)
            {
                warningPanel.transform.parent.gameObject.SetActive(false);
                warningPanel.SetActive(false);
            }
        }
    }
}