using System;
using System.IO;
using System.IO.Pipes;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace PostSystem
{
     public class StreamUtil
     {
        public string m_ReadData { get; private set; }
        public async Task WriteStream(Stream stream, string l_WriteData = "")
        {
            var writer = new StreamWriter(stream, Encoding.UTF8, l_WriteData.Length, true);
            await writer.WriteLineAsync(l_WriteData);
            writer.Close();
            Console.WriteLine(l_WriteData);
        }

        public async Task ReadStream(Stream stream)
        {
            //stream.ReadTimeout = 100;
            var reader = new StreamReader(stream,Encoding.UTF8,false,1,true);
            // メッセージを読み込み
            m_ReadData = await reader.ReadLineAsync();
            reader.Close();
        }
    }
}
