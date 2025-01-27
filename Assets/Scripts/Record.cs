using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using UnityEngine.Rendering.LookDev;

namespace Record
{
    public struct BinRecord
    {
        public string Code;
        public string Information;
        public int NumberOfTags;
        public bool Active;

        public BinRecord(string code, string information)
        {
            Code = code;
            Information = information;
            NumberOfTags = 0;
            Active = true;
        }

        public BinRecord(string code, string information, int numberOfTags, bool active)
        {
            Code = code;
            Information = information;
            NumberOfTags = numberOfTags;
            Active = active;
        }
    }

    public struct ItemRecord
    {
        public string Sku;
        public string ItemName;
        public string BinCode;
        public int Quantity;
        public int NumberOfTags;
        public bool Active;
        public string UOM;

        public ItemRecord(string sku, string itemName, string binCode, string uom)
        {
            Sku = sku;
            ItemName = itemName;
            BinCode = binCode;
            Quantity = 0;
            UOM = uom;
            Active = true;
            NumberOfTags = 0;
        }

        public ItemRecord(string sku, string itemName, string binCode, int quantity, string uom, bool active, int numberOfTags)
        {
            Sku = sku;
            ItemName = itemName;
            BinCode = binCode;
            Quantity = quantity;
            UOM = uom;
            Active = active;
            NumberOfTags = numberOfTags;
        }
    }

    public struct TransactionRecord
    {
        public struct TransactionItem
        {
            public string Sku;
            public string ItemName;
            public int Quantity;
            public string Information;
            public string Status;
        }

        public string Code;
        public string InvoiceNumber;
        public string InvoiceDate;
        public string Vendor;
        public List<TransactionItem> Items;
        public TransactionRecord(string invoiceNumber, string invoiceDate, string vendor)
        {
            string date = $"{DateTime.Now.Year}{DateTime.Now.Month:D2}{DateTime.Now.Day:D2}";
            string time = $"{DateTime.Now.Hour:D2}{DateTime.Now.Minute:D2}{DateTime.Now.Second:D2}";

            Code = $"TRANS-{date}-{time}";
            InvoiceNumber = invoiceNumber;
            InvoiceDate = invoiceDate;
            Vendor = vendor;
            Items = new List<TransactionItem>();
        }

        public TransactionRecord(string code, string invoiceNumber, string invoiceDate, string vendor)
        {
            Code = code;
            InvoiceNumber = invoiceNumber;
            InvoiceDate = invoiceDate;
            Vendor = vendor;
            Items = new List<TransactionItem>();
        }

        public TransactionRecord(JObject record)
        {
            Code = record["code"].ToString();
            InvoiceNumber = record["invoice_number"].ToString();
            InvoiceDate = record["invoice_date"].ToString();
            Vendor = record["vendor"].ToString();
            Items = new List<TransactionItem>();
        }
    }
}