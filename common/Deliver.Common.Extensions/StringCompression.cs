using System.IO;
using System.IO.Compression;
using System.Text;

namespace System
{
    public static class StringCompression
    {
        public static string Compress(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);

            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionLevel.Optimal))
                {
                    msi.CopyTo(gs);
                }

                var arr = mso.ToArray();
                return Convert.ToBase64String(arr);
            }
        }

        public static string Decompress(string base64)
        {
            var bytes = Convert.FromBase64String(base64);
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    gs.CopyTo(mso);
                }

                return Encoding.UTF8.GetString(mso.ToArray());
            }
        }
    }
}
