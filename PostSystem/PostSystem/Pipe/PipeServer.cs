using System;
using System.IO;
using System.IO.Pipes;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace PostSystem
{
    public class PipeServer
    {
        private bool m_ServerStatus { get; set; } = false;
        private bool m_StreamStatus { get; set; } = false;
        private string m_ServerName { get; set; } = "DefaultServer";
        public string m_ReceiveData { get; private set; }="";
        private Action<string> m_ContinueAction { get; set; }
        StreamUtil m_StreamUtil = new StreamUtil();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="add_ServerName"></param>
        public PipeServer(string add_ServerName="")
        {
            //空じゃなければ更新
            if (!string.IsNullOrWhiteSpace(add_ServerName))
            {
                m_ServerName = add_ServerName;
            }
            m_ServerStatus = true;
        }

        /// <summary>
        /// 開始処理
        /// </summary>
        public void ServerStart(Action<string> continueAction)
        {
            m_ContinueAction = continueAction;
            var testClientStream = new NamedPipeClientStream(m_ServerName);
            try
            {
                //ToDo.ローカルなので基本即時のはずだし一旦1
                testClientStream.Connect(1);
            }
            catch (TimeoutException)
            {
                //タイムアウトするってことはサーバーが起動してないので起動
                Program.ConsoleDebugLog("Created: " + m_ServerName + "Server", ConsoleColor.Green);
                Server().ContinueWith(task => {
                    ServerIfClose(); 
                }, TaskContinuationOptions.AttachedToParent);
            }
            catch (IOException)
            {
                //ToDo:他のクライアントが起動済みserverにアクセス中で接続できない時にここに来るはず
                //serverが足りないってことだから増やす？
                //Server().ContinueWith(task => ServerIfClose(), TaskContinuationOptions.AttachedToParent);
            }
            finally
            {
                testClientStream.Dispose();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task Server()
        {
            try
            {
                // サーバ作成
                using (var stream = new NamedPipeServerStream(m_ServerName, PipeDirection.InOut, 1))
                {
                    m_StreamStatus = true;
                    // クライアントからの接続を待つ
                    Program.ConsoleDebugLog("StreamCreated", ConsoleColor.Green);
                    Program.ConsoleDebugLog("ServerWait..." + m_ServerName, ConsoleColor.Green);
                    await stream.WaitForConnectionAsync();
                    // メッセージを受信（読み込み）
                    await m_StreamUtil.ReadStream(stream).ContinueWith(task => {
                        Program.ConsoleDebugLog("ServerRead: " + m_StreamUtil.m_ReadData);
                        //受け取ったorderをアプリ側で処理
                        m_ContinueAction(m_StreamUtil.m_ReadData);
                        // レスポンスを返す（書き込み）
                        m_StreamUtil.WriteStream(stream, "ServerSend"+ m_StreamUtil.m_ReadData).ContinueWith(tasks =>
                        {
                            Program.ConsoleDebugLog("ServerResponse: " + m_StreamUtil.m_ReadData);
                            stream.Dispose();
                        });
                    });
                    
                   
                }
            }
            catch (IOException)
            {
                Program.ConsoleDebugLog("Connection is closed.");
            }
        }

        /// <summary>
        /// Messageを確認して分岐
        /// 　""の場合はクラアントが閉じた？
        /// 　
        /// </summary>
        private void MessageCheck(in string readmessage)
        {
            m_StreamStatus = false;
            if (string.IsNullOrWhiteSpace(readmessage))
            {
                return;
            }
            //終了オーダーが届いた場合終了処理
            if (OrderUtility.IdentifierExist(readmessage, "ServerClose"))
            {
                m_ServerStatus = false;
                Program.ConsoleDebugLog("ServerClosed", ConsoleColor.Red);
            }
            else
            {
                Program.ConsoleDebugLog("StreamClosed", ConsoleColor.Yellow);
            }
        }
        /// <summary>
        /// サーバー閉じるか判定
        /// 閉じない場合再帰処理
        /// </summary>
        private void ServerIfClose()
        {
            MessageCheck(m_StreamUtil.m_ReadData);
            if (m_ServerStatus && !m_StreamStatus)
            {
                Server().ContinueWith(task => ServerIfClose(), TaskContinuationOptions.AttachedToParent);
            }
        }

    }
}
