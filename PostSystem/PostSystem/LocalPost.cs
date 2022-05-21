using System;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace PostSystem
{
    public class LocalPost
    {
        /// <summary>
        /// データタイプ
        /// 送られてきたデータをこの型に変換する
        /// </summary>
        public enum DataType
        {
            //通知だけ送りたい系
            DataTypeNone,
            //文字列
            DataTypeString,
            //数値
            DataTypeValue,
            //bool値
            DataTypeBoolean,
            //配列
            DataTypeArray,
        }
        private string m_DllName { get; set; }
        private string m_ParentAppName = "DefaultApp";
        LocalSharedMemory m_SharedMemory;
        LocalPipe m_LocalPipe;

        /// <summary>
        /// 
        /// </summary>
        /// DLL名の共有メモリを確認する
        /// メモリにアプリ名を追記（起動中のアプリを管理）
        /// アプリ名のサーバー作成
        /// <param name="l_ParentAppName">DLLが起動されたアプリの名前</param>
        /// <param name="continueAction">データ受信時に実行したい処理</param>
        public LocalPost(string l_ParentAppName ,Action<string> continueAction)
        {
            m_DllName = Assembly.GetExecutingAssembly().GetName().Name;
            if (!string.IsNullOrWhiteSpace(l_ParentAppName))
            {
                m_ParentAppName = l_ParentAppName;
            }
            m_SharedMemory = new LocalSharedMemory(l_ParentAppName, m_DllName);
            m_LocalPipe = new LocalPipe(m_ParentAppName, continueAction);
        }
        ~LocalPost()
        {
            m_SharedMemory = null;
            m_LocalPipe = null;
        }
        /// <summary>
        /// 送信データ作成
        /// </summary>
        /// 送信データの中身（string）
        /// 0.識別子(宛先※全クライアントに送信されるので自分のApp名を識別子として処理するかどうかを選択。ALLも対応したい)
        /// 1.送信元(このDLLを起動したアプリ名が入ってる想定※m_ParentAppName)
        /// 2.処理内容(実行したい処理内容=関数名GetMethod("処理内容"))
        /// 3.データ数
        /// 4_a.データ種別(Enum.Parse(typeof(DataType), tmp)で確認する)
        /// 5_a.データ
        /// End.優先度（省略可能）
        public string CreateSendData(string dataType, string data, string identifier="All", string functionName = "ping",int dataNum = 1)
        {
            List<string> tmp = new List<string>();
            //1.送信元
            tmp.Add(m_ParentAppName);
            // 2.処理内容
            tmp.Add(functionName);
            // 3.データ数
            tmp.Add(dataNum.ToString());
            /// 4_a.データ種別
            tmp.Add(dataType);
            /// 5_a.データ
            tmp.Add(data);
            return OrderUtility.CreateComOrderString(tmp, identifier);
        }

        /// <summary>
        /// データを送信する
        /// </summary>
        /// <param name="l_SendData">書き込みたいデータ</param>
        ///var receiveData_Argument = new object[] { l_SendData, l_SendName, overwriting };
        ///Type t = child.GetType();
        ///MethodInfo mi = t.GetMethod("SendData");
        ///return (bool) mi.Invoke(child, receiveData_Argument);
        /// <returns></returns>
        public bool SendData(string l_SendData)
        {
            //サーバーが立っているアプリリストを更新
            List<string> server_list = new List<string>();
            OrderUtility.ComOrderStringSplit(server_list, m_SharedMemory.GetSharedMemoryData(), m_DllName);
            //識別子はserverじゃないので一旦削除
            server_list.Remove(m_DllName);
            string l_SendServerName = "All";

            //l_SendDataから宛先サーバーを取り出す。
            //識別子は送信元（自分のアプリ名）で0番目に送信先が入っているはず
            if (!OrderUtility.ComOrderStringSplit(ref l_SendServerName, l_SendData, 0, m_ParentAppName))
            {
                //ToDo:宛先ははいってない時どうするか
                return false;
            }
            if (server_list.IndexOf(l_SendServerName) > -1)
            {
                //見つかった場合はそこにSend
                m_LocalPipe.SendData(l_SendData, l_SendServerName);
            }
            else if(l_SendServerName=="All")
            {
               
                foreach (var item in server_list)
                {
                    m_LocalPipe.SendData(l_SendData, item);
                }
            }
            else
            {
                //見つからなかった場合は。。。送信待ちで一旦send？
                m_LocalPipe.SendData(l_SendData, l_SendServerName);
            }
            return true;
        }
    }
}
