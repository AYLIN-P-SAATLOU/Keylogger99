using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using SharpHook;
using System.Threading.Tasks;

namespace Keylogger99;

public partial class MainWindow : Window
{
    private TaskPoolGlobalHook? _hook;

    public MainWindow()
    {
        InitializeComponent();
        SetupHook();
    }

    private void SetupHook()
    {
        // 1. Initialize the global hook
        _hook = new TaskPoolGlobalHook();

        // 2. Handle the KeyPressed event
        _hook.KeyPressed += (s, e) =>
        {
            // Get the name of the key (e.g., "A", "Space", "Enter")
            var keyText = e.Data.KeyCode.ToString();

            // Mac logic: We must update the UI on the main thread
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                KeyLogDisplay.Text += $"{keyText} ";
                StatusLabel.Text = "Status: Recording...";
                StatusLabel.Foreground = Avalonia.Media.Brushes.Green;
            });
        };

        // 3. Start the hook in the background so the window doesn't freeze
        Task.Run(() => _hook.Run());
    }

    private void OnClearClick(object sender, RoutedEventArgs e)
    {
        KeyLogDisplay.Text = "";
    }
}