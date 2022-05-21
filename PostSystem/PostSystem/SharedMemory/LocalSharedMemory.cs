using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Reflection;

namespace PostSystem
{

    public class LocalSharedMemory
    {
        private MemoryMappedFile m_share_mem = null;
        private MemoryMappedViewAccessor m_accessor = null;
        private string m_SharedMemoryName { get; set; }
        private string m_ParentAppName { get; set; }
        public string m_SharedMemData { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// 起動
        /// 　指定された名前の共有メモリを確認する
        /// 　　無ければ確保する
        /// 　　あれば親の名前を追記する
        /// </summary>
        public LocalSharedMemory(string parentAppName, string sharedMemoryName = "")
        {
            m_ParentAppName = parentAppName;
            m_SharedMemoryName = Assembly.GetExecutingAssembly().GetName().Name;
            if (!string.IsNullOrWhiteSpace(sharedMemoryName))
            {
                m_SharedMemoryName = sharedMemoryName;
            }
            CreateSharedMemoryData();
        }
        //デストラクタ
        ~LocalSharedMemory()
        {
            CreateSharedMemoryData(false);
            m_accessor?.Dispose();
            m_share_mem?.Dispose();
        }

        public string GetSharedMemoryData()
        {
            return ReadSharedMemory(m_SharedMemoryName);
        }

        private void CreateSharedMemoryData(bool dataAdd = true)
        {
            string l_SharedMemData = ReadSharedMemory(m_SharedMemoryName);
            //データ取得できた場合追記
            if (OrderUtility.IdentifierExist(l_SharedMemData, m_SharedMemoryName))
            {
                List<string> tmp = new List<string>();
                OrderUtility.ComOrderStringSplit(tmp, l_SharedMemData, m_SharedMemoryName);
                //識別子を一旦削除
                tmp.Remove(m_SharedMemoryName);
                if(dataAdd)
                {
                    tmp.Add(m_ParentAppName);
                }
                else
                {
                    tmp.Remove(m_ParentAppName);
                }
                //再度データ作成
                l_SharedMemData = OrderUtility.CreateComOrderString(tmp, m_SharedMemoryName);
            }
            //データがない場合新規作成
            else
            {
                if (dataAdd)
                {
                    l_SharedMemData = OrderUtility.CreateComOrderString(m_ParentAppName, m_SharedMemoryName);
                }
            }
            WriteSharedMemory(m_SharedMemoryName, l_SharedMemData);
        }
        // 共有メモリを読み取る
        private string ReadSharedMemory(string sharedMemoryName)
        {
            try
            {
                m_share_mem = MemoryMappedFile.OpenExisting(sharedMemoryName);
                m_accessor = m_share_mem.CreateViewAccessor();
                if (m_accessor.CanRead)
                {
                    int size = m_accessor.ReadInt32(0);
                    char[] result = new char[size];
                    m_accessor.ReadArray<char>(sizeof(int), result, 0, size);
                    return new string(result);
                }
                return ("Failure! MemoryMappedFile Can not Read");
            }
            catch (FileNotFoundException ex)
            {
                return ("Failure! MemoryMappedFile Not found");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return ("Failure!");
            }
        }
        // 共有メモリに書き込む
        private bool WriteSharedMemory(string sharedMemoryName, string l_SendData)
        {
            try
            {
                m_share_mem = MemoryMappedFile.CreateOrOpen(sharedMemoryName, l_SendData.Length);
                m_accessor = m_share_mem.CreateViewAccessor();
                if (m_accessor.CanWrite)
                {
                    m_accessor.Write(0, l_SendData.Length);
                    m_accessor.WriteArray<char>(sizeof(int), l_SendData.ToCharArray(), 0, l_SendData.Length);
                    return true;
                }
                Console.WriteLine("Failure! MemoryMappedFile Can not Write");
                return false;
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("Failure! MemoryMappedFile Not found");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
