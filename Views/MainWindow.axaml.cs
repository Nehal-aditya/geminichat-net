using Avalonia.Controls;
using Avalonia.Input;
using GeminiChat.ViewModels;

namespace GeminiChat.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            // Enter sends, Shift+Enter adds newline
            if (e.Key == Key.Return
                && !e.KeyModifiers.HasFlag(KeyModifiers.Shift)
                && DataContext is MainViewModel vm
                && !vm.IsLoading)
            {
                e.Handled = true;
                vm.SendMessageCommand.Execute(null);
            }
        }
    }
}