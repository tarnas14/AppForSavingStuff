namespace Modules.MoneyTracking
{
    public class Tag
    {

        public string Value { get; private set; }

        public Tag(string tagString)
        {
            Value = tagString;
        }

        public override bool Equals(object obj)
        {
            var tag = obj as Tag;

            if (tag == null)
            {
                return false;
            }

            return Value == tag.Value;
        }

        public override int GetHashCode()
        {
            return (Value != null ? Value.GetHashCode() : 0);
        }
    }
}