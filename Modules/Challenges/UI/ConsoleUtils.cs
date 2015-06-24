using System;
using System.Text;

namespace Modules.Challenges.UI
{
    static class ConsoleUtils
    {
        public static void Utf8Display(Action displayStuff)
        {
            DisplayAndReturn(() =>
            {
                var encoding = Console.OutputEncoding;
                Console.OutputEncoding = Encoding.UTF8;

                displayStuff();

                Console.OutputEncoding = encoding; 
            });
        }

        public static void DisplayAndReturn(Action displayStuff)
        {
            var cursor = new Cursor(Console.CursorLeft, Console.CursorTop);

            displayStuff();

            Console.CursorLeft = cursor.Left;
            Console.CursorTop = cursor.Top;
        }

        public static void ClearLine()
        {
            var clearString = string.Format("\r{0}\r", new String(' ', Console.WindowWidth));
            Console.Write(clearString);
        }
    }
}
