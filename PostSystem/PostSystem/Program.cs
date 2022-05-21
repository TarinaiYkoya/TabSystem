using System;
using System.Reflection;

namespace PostSystem
{
    /// <summary>
    /// 挙動確認テスト用クラス
    /// </summary>
    public class Program
    {
        static void Main(string[] args)
        {
            ConsoleDebugLog("PostSystem TestMode Start");
            Console.ReadLine();
            LocalPostTest();
            ConsoleDebugLog("PostSystem TestMode End");
            Console.ReadLine();
        }

        static void LocalPostTest()
        {
            LocalPost l_post = new LocalPost(Assembly.GetExecutingAssembly().GetName().Name, ValueChangedTest);
            SendTest(l_post, l_post.CreateSendData((LocalPost.DataType.DataTypeString).ToString(), "送信テスト", "PostTest"));
        }

        /// <summary>
        /// 送信テスト
        /// </summary>
        /// <param name="l_post"></param>
        /// <param name="sendStr"></param>
        /// <returns></returns>
        static bool SendTest(LocalPost l_post,string sendStr)
        {
            if (!l_post.SendData(sendStr))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 関数渡しテスト用
        /// </summary>
        /// <param name="input"></param>
        static　void ValueChangedTest(string input)
        {
            ConsoleDebugLog("関数渡しテスト: " + input);
        }

        /// <summary>
        /// デバック用ログ出力＿色指定
        /// </summary>
        /// <param name="logtext"></param>
        /// <param name="clor"></param>
        static public void ConsoleDebugLog(string logtext, ConsoleColor clor = ConsoleColor.Gray)
        {
            ConsoleColor nowclor = Console.ForegroundColor;
            Console.ForegroundColor = clor;
            Console.WriteLine(logtext);
            Console.ForegroundColor = nowclor;
        }
    }
}
