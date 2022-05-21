using System;
using System.IO;
using System.IO.Pipes;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace PostSystem
{
    public class PipeClient
    {

        private string m_ClientName { get; set; } = "DefaultClient";
        public string m_ReceiveMessage { get; private set; } = "";
        private Action<string> m_ContinueAction { get; set; }
        private StreamUtil m_StreamUtil = new StreamUtil();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="continueAction"></param>
        /// <param name="sendservername"></param>
        public PipeClient(Action<string> continueAction, string sendservername = "")
        {
            //空じゃなければ更新
            if (!string.IsNullOrWhiteSpace(sendservername))
            {
                m_ClientName = sendservername;
            }
            m_ContinueAction = continueAction;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="l_SendData"></param>
        /// <returns></returns>
        public async Task Client(string l_SendData = "")
        {
            using (var stream = new NamedPipeClientStream(m_ClientName))
            {
                await stream.ConnectAsync();
                //空じゃなければ読み書き
                if (!string.IsNullOrWhiteSpace(l_SendData))
                {
                    // メッセージを送信（書き込み）
                    await m_StreamUtil.WriteStream(stream, "ClientSend: " + l_SendData);
                }
                await m_StreamUtil.ReadStream(stream).ContinueWith(task => {
                    //空じゃなければ更新
                    if (!string.IsNullOrWhiteSpace(m_StreamUtil.m_ReadData))
                    {
                        m_ReceiveMessage = m_StreamUtil.m_ReadData;
                        Program.ConsoleDebugLog("ServerResponse: " + m_ReceiveMessage);
                    }
                });
                m_ContinueAction(m_ReceiveMessage);
                stream.Dispose();
            }
        }
    }

 
}
