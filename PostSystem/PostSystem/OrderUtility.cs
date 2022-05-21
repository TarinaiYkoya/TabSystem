using System.Collections.Generic;
using System.Linq;

namespace PostSystem
{
    /// <summary>
    /// 文字列を送受信する時に使うあれこれ
    /// </summary>
    static public class OrderUtility
    {
        /// <summary>
        /// 識別子が存在するか確認
        /// </summary>
        /// <param name="str"></param>
        /// <param name="identifier"></param>
        /// <param name="split_key"></param>
        /// <returns></returns>
        static public bool IdentifierExist(in string str,in string identifier = "ComOrderString", in char split_key = '@')
        {
            if (str.IndexOf(identifier + split_key) == -1)
            {
                return false;
            }
            return true;
        }
        /*!
       * @brief  識別子が存在すれば指定したキーで分割して格納
       * @author koyama_yohei
       * @date   [3/5/2020]
       * @param  ret_str       分割後
       * @param  str           分割する文字列(order)
       * @param  identifier    識別子
       * @param  split_key     区切りとなる文字（指定なしは'@'）
       * @retval Bool          成功可否
       */
        static public bool ComOrderStringSplit(in List<string> ret_str, in string str, in string identifier = "ComOrderString", in char split_key = '@')
        {
            //識別子
            if (!IdentifierExist(str, identifier, split_key))
            {
                return false;
            }
            foreach (var add_str in str.Split(split_key))
            {
                ret_str.Add(add_str);
            }
            //最初は識別子なので削除
            //ret_str.Remove(identifier);
            return true;
        }

        /*!
        * @brief  識別子が存在すれば指定したキーで分割して指定した要素番目の文字列を返してくれる
        * @author koyama_yohei
        * @date   [3/5/2020]
        * @param  ret_str       要素
        * @param  str           分割する文字列(order)
        * @param  element_num   欲しい要素番号（指定なしは0）
        * @param  identifier    識別子
        * @param  split_key     区切りとなる文字（指定なしは'@'）
        * @retval Bool          成功可否
        */
        static public bool ComOrderStringSplit(ref string ret_str, in string str, in int element_num = 0, in string identifier = "ComOrderString", in char split_key = '@')
        {
            //識別子
            if (!IdentifierExist(str, identifier, split_key))
            {
                return false;
            }
            List<string> tmp = new List<string>();
            foreach (var add_str in str.Split(split_key))
            {
                tmp.Add(add_str);
            }
            //最初は識別子なので削除
            //tmp.erase(tmp.begin());
            //tmp.Remove(identifier);
            if (tmp.Count() <= element_num)
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(tmp[element_num]))
            {
                return false;
            }
            ret_str = tmp[element_num];
            return true;
        }

        /*!
        * @brief  識別子の後ろに区切り文字ごとにコンテナの中身を詰めてくれる
        * @author koyama_yohei
        * @date   [3/5/2020]
        * @param  str           送りたい情報のリスト
        * @param  identifier    識別子(送信先が識別子・受信時は自分の名前が識別子)
        * @param  split_key     区切りとなる文字（指定なしは'@'）
        * @retval String        送るべき文字列(order)
        */
        static public string CreateComOrderString(in List<string> str, in string identifier = "ComOrderString", in char split_key = '@')
        {
            string ret_str = identifier;
            for (int i = 0; i < str.Count(); i++)
            {
                ret_str += split_key + str[i];
            }
            return ret_str;
        }

        /*!
        * @brief  識別子の後ろに区切り文字と要素を付けてくれる
        * @author koyama_yohei
        * @date   [3/5/2020]
        * @param  str           送りたい情報
        * @param  identifier    識別子(送信先が識別子・受信時は自分の名前が識別子)
        * @param  split_key     区切りとなる文字（指定なしは'@'）
        * @retval String        送るべき文字列(order)
        */
        static public string CreateComOrderString(in string str, in string identifier = "ComOrderString", in char split_key = '@')
        {
            string ret_str = identifier;
            ret_str += split_key + str;
            return ret_str;
        }
    }
}
