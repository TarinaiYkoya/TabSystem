using System;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace PostSystem
{
    public class LocalPipe
    {
        private PipeServer m_LocalServer { get; set; }
        private PipeClient m_LocalClient { get; set; }
        private string m_DefaultPipeName { get; set; } = "DefaultApp";
        private Action<string> m_ContinueAction { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="l_CreatePipe"></param>
        /// <param name="serverCreate"></param>
        /// <param name="continueAction"></param>
        public LocalPipe(string l_CreatePipe,Action<string> continueAction)
        {
            m_ContinueAction = continueAction;
            m_DefaultPipeName = l_CreatePipe;
            //DLLを起動したアプリ名のサーバーを作成
            m_LocalServer = new PipeServer(m_DefaultPipeName);
            m_LocalServer.ServerStart(continueAction);
        }

        /// <summary>
        /// データを送受信する
        /// </summary>
        /// <param name="l_SharedMemoryName">書き込みたいメモリ名</param>
        /// <param name="l_SendData">書き込みたいデータ</param>
        /// <param name="overwriting">上書き許可</param>
        /// <returns></returns>
        /// 
        public bool SendData(string l_SendData, string sendserver)
        {
            string l_SendServerName = sendserver;
            m_LocalClient = new PipeClient(m_ContinueAction, l_SendServerName);
            m_LocalClient.Client(l_SendData).ContinueWith(task => {  });
            return true;
        }

    }
}
