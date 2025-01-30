using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CompanySystem
{
    public class AddTransactionItemManager : MonoBehaviour
    {
        public TMP_Dropdown itemDropdown;
        public TMP_InputField quantityInputField;

        public GameObject itemTable;

        private void OnEnable()
        {
            // Invoke GetData method after a short delay
            Invoke("GetItems", 0.1f);
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
                    itemDropdown.captionText.text = itemDropdown.options[0].text;
                }
                else
                {
                    Debug.LogError("Failed to retrieve data.");
                }
            }));
        }

        // Add item to transaction
        public void AddNewItem()
        {
            string[] selectedItem = itemDropdown.captionText.text.Split('-');
            string sku = selectedItem[0].Trim();

            StartCoroutine(FirebaseServices.ReadData("items", "sku", sku, data =>
            {
                if (data != null)
                {
                    StartCoroutine(UpdateData(sku, data));
                }
                else
                {
                    Debug.LogError("Failed to retrieve data.");
                }
            }));
        }

        private IEnumerator UpdateData(string sku, JObject data)
        {
            string code = TransactionListManager.selectedRecord.Code;
            int quantity = int.Parse(quantityInputField.text);

            var newTransactionItem = new Dictionary<string, object>
            {
                { "sku", data["sku"].ToString() },
                { "item_name", data["item_name"].ToString() },
                { "quantity", quantity },
                { "information", "Unprocessed" }
            };

            var newItemQuantity = new Dictionary<string, object>
            {
                { "sku", data["sku"].ToString() },
                { "quantity", int.Parse(data["quantity"].ToString()) + quantity }
            };

            bool transactionSuccess = false;
            bool itemQuantitySuccess = false;

            yield return FirebaseServices.ModifyData("transactions", "code", code, "items", newTransactionItem, message =>
            {
                Debug.Log(message);
                transactionSuccess = message.Contains("successfully");
                if (transactionSuccess)
                {
                    Debug.Log("Item added to transaction.");
                    ResetInput();
                }
                else
                {
                    Debug.LogError("Failed to add item to transaction.");
                }
            });

            yield return FirebaseServices.ModifyData("items", newItemQuantity, sku, "sku", message =>
            {
                itemQuantitySuccess = message.Contains("successfully");
                if (itemQuantitySuccess)
                {
                    Debug.Log("Item quantity updated.");
                }
                else
                {
                    Debug.LogError("Failed to update item quantity.");
                }
            });

            if (transactionSuccess && itemQuantitySuccess)
            {
                gameObject.SetActive(false);
                RefreshTable();
            }
            else
            {
                Debug.LogError("Failed to update data.");
            }
        }

        // Reset input field
        public void ResetInput()
        {
            if (quantityInputField.text.Length > 0)
                quantityInputField.text = quantityInputField.text.Remove(0);

            itemDropdown.options.Clear();
        }
        private void RefreshTable()
        {
            itemTable.SetActive(false);
            itemTable.SetActive(true);
        }
    }
}