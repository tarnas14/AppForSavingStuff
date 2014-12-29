namespace Ui
{
    using System.Collections.Generic;

    public class UserCommand
    {
        public string Name { get; set; }
        public IList<string> Params { get; set; }
    }
}