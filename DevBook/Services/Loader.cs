using System.IO;
using System.Text;

namespace DevBook.Services
{
    public class Loader
    {
        private readonly string _path;
        private readonly int _bytesToLoad;

        public Loader(string path, int bytesToLoad)
        {
            _path = path;
            _bytesToLoad = bytesToLoad;
        }

        public string LoadText()
        {
            string text = "";

            using (FileStream fs = File.OpenRead(_path))
            {
                byte[] b = new byte[_bytesToLoad];

                UTF8Encoding temp = new UTF8Encoding();
                //Encoding temp = Encoding.GetEncoding(932);

                while (fs.Read(b, 0, b.Length) > 0)
                    text += temp.GetString(b);
            }

            return text;
        }
    }
}
