

namespace ArabazonPlatform.Generic.Generic
{
    public class DDLDataSource
    {
        public string Label { get; set; }
        public int Value { get; set; }
        public object Attributes { get; set; }
        public bool Disabled { get; set; }
        public Guid ValueGuid { get; set; }

        public DDLDataSource()
        {

        }

        public DDLDataSource(string label, int value)
        {
            this.Label = label;
            this.Value = value;
        }

        public DDLDataSource(string label, Guid valueGuid)
        {
            this.Label = label;
            this.ValueGuid = valueGuid;
        }
    }
}
