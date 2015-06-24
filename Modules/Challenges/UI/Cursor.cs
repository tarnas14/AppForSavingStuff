namespace Modules.Challenges.UI
{
    public class Cursor
    {
        public Cursor(int left, int top)
        {
            Left = left;
            Top = top;
        }

        public int Left { get; private set; }
        public int Top { get; private set; }
    }
}
