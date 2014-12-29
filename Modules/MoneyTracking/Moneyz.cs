namespace Modules.MoneyTracking
{
    using System;

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
            return Convert.ToString(Value);
        }
    }
}