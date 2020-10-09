using System.Collections.Generic;
using System.Linq;

namespace DevBook.Services
{
    public static class CollectionStringBuilder
    {
        public static string BuildString(IList<string> list, string separator)
        {
            if (list.Count == 0)
                return "";

            string s = "";

            for (int i = 0; i < list.Count() - 1; i++)
                s += list[i] + separator;

            s += list[list.Count() - 1];

            return s;
        }
    }
}
