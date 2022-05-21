using System.Windows.Controls;

namespace TabSystem
{
    public class CloseTabItem
    {
        // [X]ボタンクリック時の処理
        public void CloseTab(object parameter)
        {
            TabItem tabItem = parameter as TabItem;
            if (tabItem != null)
            {
                System.Windows.Controls.TabControl tabCtrl = tabItem.Parent as System.Windows.Controls.TabControl;
                if (tabCtrl != null)
                {
                    tabCtrl.Items.Remove(parameter);
                }
            }
        }
    }
}
