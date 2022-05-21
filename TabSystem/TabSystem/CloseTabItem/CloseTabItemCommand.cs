using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace TabSystem
{
    public class CloseTabItemCommand : ICommand
    {
        /// <summary>
        /// コマンドを読み出す側のクラス（View Model）を保持するプロパティ
        /// </summary>
        private MainWindowViewModel _view { get; set; }

        /// <summary>
        /// コンストラクタ
        /// コマンドで処理したいクラス(View Modeo)をここで受け取る
        /// </summary>
        /// <param name="view"></param>
        public CloseTabItemCommand(MainWindowViewModel view)
        {
            _view = view;
        }

        /// <summary>
        /// コマンドのルールとして必ず実装しておくイベントハンドラ
        /// 通常、このメソッドを丸ごとコピーすればOK
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// コマンドの有効／無効を判定するメソッド
        /// コマンドのルールとして必ず実装しておくメソッド
        /// 有効／無効を制御する必要が無ければ、無条件にTrueを返しておく
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// コマンドの動作を定義するメソッド
        /// コマンドのルールとして必ず実装しておくメソッド
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            _view._CloseTab(parameter);
        }
    }
}
