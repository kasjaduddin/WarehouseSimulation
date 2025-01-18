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
        public string Quantity;
        public int NumberOfTags;
        public bool Active;
        public string UOM;

        public ItemRecord(string sku, string itemName, string binCode, string quantity, string uom)
        {
            Id = string.Empty;
            Sku = sku;
            ItemName = itemName;
            BinCode = binCode;
            Quantity = quantity;
            NumberOfTags = 0;
            Active = true;
            UOM = uom;
        }

        public ItemRecord(string id, string sku, string itemName, string binCode, string quantity, string uom)
        {
            Id = id;
            Sku = sku;
            ItemName = itemName;
            BinCode = binCode;
            Quantity = quantity;
            NumberOfTags = 0;
            Active = true;
            UOM = uom;
        }
    }
}