using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json.Linq;

namespace ScreenMagic
{
    class TestStubOcrProvider : IOcrResultProvider
    {
        Task<JToken> IOcrResultProvider.MakeOCRRequest(byte[] jpegEncoded)
        {
            string responsePath = Path.Combine(Utils.GetAssemblyPath(), "Assets\\response.txt");
            StreamReader r = new StreamReader(responsePath);
            string contentString = r.ReadToEnd();
            var jtoken = JToken.Parse(contentString);
            Task<JToken> task = Task<JToken>.Factory.StartNew(() => { return jtoken; });
            return task;
        }
    }
}
