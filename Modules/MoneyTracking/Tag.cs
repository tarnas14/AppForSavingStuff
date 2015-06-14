namespace Modules.MoneyTracking
{
    using System.Collections.Generic;

    public class Tag
    {
        private string _value;

        public string Value
        {
            get { return _value; }
            set { _value = IsTagName(value) ? value : "#" + value; }
        }

        public Tag()
        {
            
        }

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

        public static bool IsTagName(string sourceName)
        {
            return sourceName.StartsWith("#");
        }

        public class Comparer : IEqualityComparer<Tag>
        {
            public bool Equals(Tag x, Tag y)
            {
                return x.Value == y.Value;
            }

            public int GetHashCode(Tag obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}