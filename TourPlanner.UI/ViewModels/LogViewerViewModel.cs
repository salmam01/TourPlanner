using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using TourPlanner.UI.Commands;

namespace TourPlanner.UI.ViewModels
{
    public class LogEntry
    {
        public string Message { get; set; }
        public string Type { get; set; }
        public Brush Color { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class LogViewerViewModel : BaseViewModel
    {
        private ObservableCollection<LogEntry> _logs;
        public ObservableCollection<LogEntry> Logs
        {
            get => _logs;
            set
            {
                _logs = value;
                OnPropertyChanged();
            }
        }

        public ICommand RefreshCommand { get; }

        public LogViewerViewModel()
        {
            _logs = new ObservableCollection<LogEntry>();
            RefreshCommand = new RelayCommand(_ => RefreshLogs());
            RefreshLogs();
        }

        private Brush GetColorForLogType(string logType)
        {
            return logType.ToLower() switch
            {
                "error" => new SolidColorBrush(Color.FromRgb(255, 107, 107)),    // #FF6B6B
                "warning" => new SolidColorBrush(Color.FromRgb(255, 184, 108)),  // #FFB86C
                "information" => new SolidColorBrush(Color.FromRgb(31, 225, 253)), // #8BE9FD
                "success" => new SolidColorBrush(Color.FromRgb(80, 250, 123)),   // #50FA7B
                _ => new SolidColorBrush(Color.FromRgb(51, 51, 51))             // #333333
            };
        }

        private string GetLogType(string logLine)
        {
            if (logLine.Contains("\"Level\":\"Error\"")) return "Error";
            if (logLine.Contains("\"Level\":\"Warning\"")) return "Warning";
            if (logLine.Contains("\"Level\":\"Information\"")) return "Information";
            if (logLine.Contains("\"Level\":\"Debug\"")) return "Debug";
            return "Unknown";
        }

        public void RefreshLogs()
        {
            try
            {
                var logFiles = Directory.GetFiles("Logs", "TourPlannerLog-*.txt")
                    .OrderByDescending(f => File.GetLastWriteTime(f));

                if (!logFiles.Any()) return;

                var latestLogFile = logFiles.First();
                var logLines = File.ReadLines(latestLogFile)
                    .Where(line => !string.IsNullOrWhiteSpace(line))
                    .TakeLast(50) // Show last 50 log entries
                    .ToList();

                Logs.Clear();
                foreach (var line in logLines)
                {
                    var logType = GetLogType(line);
                    Logs.Add(new LogEntry
                    {
                        Message = line,
                        Type = logType,
                        Color = GetColorForLogType(logType),
                        Timestamp = DateTime.Now // You might want to parse this from the log line
                    });
                }
            }
            catch (Exception ex)
            {
                Logs.Add(new LogEntry
                {
                    Message = $"Error reading logs: {ex.Message}",
                    Type = "Error",
                    Color = GetColorForLogType("Error"),
                    Timestamp = DateTime.Now
                });
            }
        }
    }
} 