using System;
using System.Windows;
using System.Windows.Controls;
using System.Reflection;

namespace TabSystem
{
    public class AddTabItem
    {
        // [+]ボタンクリック時の処理
        public void AddTab(TabItem parameter)
        {
            PostSystem.LocalPost l_post = new PostSystem.LocalPost(Assembly.GetExecutingAssembly().GetName().Name, test);
            System.Windows.Controls.TabControl tabCtrl = parameter.Parent as System.Windows.Controls.TabControl;
            if (tabCtrl != null)
            {
                TabItem addtab = new TabItem();
                var type = Type.GetType(GetTypeName(parameter.Name));
                var instance = Activator.CreateInstance(type) as Window;
                //l_post.SendData(l_post.CreateSendData(PostSystem.LocalPost.DataType.DataTypeString.ToString(),"AddTab", "PostTest"));
                l_post.SendData(l_post.CreateSendData(PostSystem.LocalPost.DataType.DataTypeString.ToString(), "AddTab", "LogWindow"));

                addtab.Header = instance.Name + (tabCtrl.Items.Count ).ToString();
                addtab.Name = instance.Name + (tabCtrl.Items.Count).ToString();
                addtab.Style = tabCtrl.TryFindResource("Menu") as Style;
                addtab.Content = instance.Content;
                addtab.MouseRightButtonDown += new System.Windows.Input.MouseButtonEventHandler(closeTabItem_Click);
                tabCtrl.Items.Insert(tabCtrl.Items.Count-1, addtab);
            }
        }
        static string GetTypeName(string dllName)
        {
            string typeName = dllName + "." + dllName + "Main" + "," + dllName;
            return typeName;
        }

        private void closeTabItem_Click(object sender, RoutedEventArgs e)
        {
            TabItem _tab = sender as TabItem;
            TabControl tabControl = _tab.Parent as TabControl;
            tabControl.Items.Remove(_tab);
            e.Handled = true;
        }
        void test(string temp)
        {

        }
    }
}
