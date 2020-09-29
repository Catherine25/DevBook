using System;

namespace DevBook.Data
{
    public class Command
    {
        public string Name;

        public override string ToString() => Name;

        public Action Action;
    }
}
