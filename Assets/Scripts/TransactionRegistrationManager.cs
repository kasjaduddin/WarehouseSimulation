using Record;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace CompanySystem
{
    public class TransactionRegistrationManager : MonoBehaviour
    {
        public TMP_InputField invoiceNumberInputField;
        public TextMeshProUGUI invoiceDate;
        public TMP_InputField vendorInputField;

        private struct Date
        {
            public string day;
            public string month;
            public string year;
        }
        private Date selectedDate;
        public GameObject calendarPicker;

        public GameObject transactionTable;

        public GameObject popup;
        private GameObject warningPanel;

        private void OnEnable()
        {
            LoadDate();   
        }

        private void Update()
        {
            invoiceDate.text = $"{selectedDate.day}/{selectedDate.month}/{selectedDate.year}";
        }

        public void AddNewTransaction()
        {
            TransactionRecord newTransaction = new TransactionRecord(invoiceNumberInputField.text, invoiceDate.text, vendorInputField.text);
            var transactionData = new Dictionary<string, object>
            {
                { "code", newTransaction.Code },
                { "invoice_number", newTransaction.InvoiceNumber },
                { "invoice_date", newTransaction.InvoiceDate },
                { "vendor", newTransaction.Vendor }
            };

            StartCoroutine(FirebaseServices.WriteData("transactions", transactionData, "invoice_number", message =>
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
                    //BinRegisteredHandler(message, newBin.Code);
                }
                else
                {
                    Debug.LogError(message);
                }
            }));
        }

        private void LoadDate()
        {
            selectedDate.day = DateTime.Now.Day.ToString();
            selectedDate.month = DateTime.Now.Month.ToString();
            selectedDate.year = DateTime.Now.Year.ToString();
        }

        // Reset input field
        public void ResetInput()
        {
            if (invoiceNumberInputField.text.Length > 0)
                invoiceNumberInputField.text = invoiceNumberInputField.text.Remove(0);
            if (vendorInputField.text.Length > 0)
                vendorInputField.text = vendorInputField.text.Remove(0);
        }
        private void RefreshTable()
        {
            transactionTable.SetActive(false);
            transactionTable.SetActive(true);
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
    }
}