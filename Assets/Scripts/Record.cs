using System;
using UnityEngine.Rendering.LookDev;

namespace Record
{
    public struct BinRecord
    {
        public string Id;
        public string Code;
        public string Information;
        public int NumberOfTags;
        public bool Active;

        public BinRecord(string code, string information)
        {
            Id = string.Empty;
            Code = code;
            Information = information;
            NumberOfTags = 0;
            Active = true;
        }

        public BinRecord(string id, string code, string information)
        {
            Id = id;
            Code = code;
            Information = information;
            NumberOfTags = 0;
            Active = true;
        }
    }

    public struct ItemRecord
    {
        public string Id;
        public string Sku;
        public string ItemName;
        public string BinCode;
        public int Quantity;
        public int NumberOfTags;
        public bool Active;
        public string UOM;

        public ItemRecord(string sku, string itemName, string binCode, string uom)
        {
            Id = string.Empty;
            Sku = sku;
            ItemName = itemName;
            BinCode = binCode;
            Quantity = 0;
            NumberOfTags = 0;
            Active = true;
            UOM = uom;
        }

        public ItemRecord(string id, string sku, string itemName, string binCode, string uom)
        {
            Id = id;
            Sku = sku;
            ItemName = itemName;
            BinCode = binCode;
            Quantity = 0;
            NumberOfTags = 0;
            Active = true;
            UOM = uom;
        }
    }

    public struct TransactionRecord
    {
        public string Id;
        public string Code;
        public string InvoiceNumber;
        public string InvoiceDate;
        public string Vendor;

        public TransactionRecord(string invoiceNumber, string invoiceDate, string vendor)
        {
            string date = $"{DateTime.Now.Year}{DateTime.Now.Month:D2}{DateTime.Now.Day:D2}";
            string time = $"{DateTime.Now.Hour:D2}{DateTime.Now.Minute:D2}{DateTime.Now.Second:D2}";

            Id = string.Empty;
            Code = $"TRANS-{date}-{time}";
            InvoiceNumber = invoiceNumber;
            InvoiceDate = invoiceDate;
            Vendor = vendor;
        }
    }
}