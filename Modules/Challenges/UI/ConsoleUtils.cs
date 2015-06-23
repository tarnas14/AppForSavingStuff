using System;
using System.Text;

namespace Modules.Challenges.UI
{
    static class ConsoleUtils
    {
        public static void Utf8Display(Action displayStuff)
        {
            var cursor = Tuple.Create(Console.CursorLeft, Console.CursorTop);
            var encoding = Console.OutputEncoding;
            Console.OutputEncoding = Encoding.UTF8;

            displayStuff();

            Console.OutputEncoding = encoding;
            Console.CursorLeft = cursor.Item1;
            Console.CursorTop = cursor.Item2;
        }
    }
}
