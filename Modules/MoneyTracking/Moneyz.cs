namespace Modules.MoneyTracking
{
    using Raven.Imports.Newtonsoft.Json;

    public class Moneyz
    {

        public decimal Value { get; private set; }

        public Moneyz(decimal howMuch)
        {
            Value = howMuch;
        }

        public static Moneyz operator +(Moneyz lhs, Moneyz rhs)
        {
            return new Moneyz(lhs.Value + rhs.Value);
        }

        public static Moneyz operator -(Moneyz lhs, Moneyz rhs)
        {
            return new Moneyz(lhs.Value - rhs.Value);
        }

        public override bool Equals(object obj)
        {
            var moneyz = obj as Moneyz;

            if (moneyz == null)
            {
                return false;
            }

            return moneyz.Value == Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0:N2}", Value);
        }

        public string SignedString
        {
            get
            {
                string plus = Value > 0 ? "+" : string.Empty;
                return string.Format("{0}{1:N2}", plus, Value);
            }
        }

        public string UnsignedString
        {
            get
            {
                var value = Value < 0 ? -Value : Value;
                return string.Format("{0:N2}", value);
            }
        }

        public string SignString
        {
            get
            {
                return Value >= 0 ? "+" : "-";
            }
        }
        
        [JsonIgnore]
        public Moneyz Absolute
        {
            get
            {
                var value = (Value < 0) ? -Value : Value;

                return new Moneyz(value);
            }
        }
    }
}