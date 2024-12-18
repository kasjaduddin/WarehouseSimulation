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
}