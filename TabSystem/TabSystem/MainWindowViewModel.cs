using System.ComponentModel;
using System.Windows.Controls;

namespace TabSystem
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// View Modelのルールとして実装しておくイベントハンドラ
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// コンストラクタ
        /// ここでコマンドのインスタンスを生成し、プロパティに格納しておく
        /// </summary>
        public MainWindowViewModel()
        {
            addTabItemCommand = new AddTabItemCommand(this);
            addTabItem = new AddTabItem();

            closeTabItemCommand = new CloseTabItemCommand(this);
            closeTabItem = new CloseTabItem();
        }

        #region AddTab
        //タブ増加Modelのインスタンスを保持するプロパティ
        public AddTabItem addTabItem { get; set; }

        /// <summary>
        /// タブ増加コマンドを格納するプロパティ
        /// </summary>
        public AddTabItemCommand addTabItemCommand { get; private set; }
        /// <summary>
        /// 他のクラスから参照／設定が行えるようにするためのプロパティの定義
        /// View のXAMLに記述したバインドにより、View Modelからアクセスされる
        /// </summary>
        public void _AddTabItem(TabItem parameter)
        {
            addTabItem.AddTab((TabItem)parameter);
        }
        #endregion

        #region TabClose
        //Modelのインスタンスを保持するプロパティ
        public CloseTabItem closeTabItem { get; set; }

        /// <summary>
        /// コマンドを格納するプロパティ
        /// </summary>
        public CloseTabItemCommand closeTabItemCommand { get; private set; }

        /// <summary>
        /// 他のクラスから参照／設定が行えるようにするためのプロパティの定義
        /// View のXAMLに記述したバインドにより、View Modelからアクセスされる
        /// </summary>
        public void _CloseTab(object parameter)
        {
            closeTabItem.CloseTab(parameter);
        }
        #endregion
     
    }
}
