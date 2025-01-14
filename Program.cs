namespace lab15_2
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.Json;

    namespace LoggerExample
    {
        // Репозиторий для управления логами
        public interface ILogRepository
        {
            void WriteLog(string message);
            IEnumerable<string> ReadLogs();
        }

        // Реализация для текстового файла
        public class TextFileLogRepository : ILogRepository
        {
            private readonly string _filePath;

            public TextFileLogRepository(string filePath)
            {
                _filePath = filePath;
            }

            public void WriteLog(string message)
            {
                File.AppendAllText(_filePath, message + Environment.NewLine);
            }

            public IEnumerable<string> ReadLogs()
            {
                if (File.Exists(_filePath))
                {
                    return File.ReadAllLines(_filePath);
                }
                return new List<string>();
            }
        }

        // Реализация для JSON-файла
        public class JsonFileLogRepository : ILogRepository
        {
            private readonly string _filePath;
            private List<string> _logEntries;

            public JsonFileLogRepository(string filePath)
            {
                _filePath = filePath;
                _logEntries = LoadLogs();
            }

            public void WriteLog(string message)
            {
                _logEntries.Add(message);
                SaveLogs();
            }

            public IEnumerable<string> ReadLogs()
            {
                return _logEntries;
            }

            private List<string> LoadLogs()
            {
                if (File.Exists(_filePath))
                {
                    string json = File.ReadAllText(_filePath);
                    return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
                }
                return new List<string>();
            }

            private void SaveLogs()
            {
                string json = JsonSerializer.Serialize(_logEntries, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_filePath, json);
            }
        }

        // Класс MyLogger, использующий репозиторий
        public class MyLogger
        {
            private readonly ILogRepository _repository;

            public MyLogger(ILogRepository repository)
            {
                _repository = repository;
            }

            public void Log(string message)
            {
                string timestampedMessage = $"[{DateTime.Now}] {message}";
                _repository.WriteLog(timestampedMessage);
            }

            public IEnumerable<string> GetLogs()
            {
                return _repository.ReadLogs();
            }
        }

        class Program
        {
            static void Main(string[] args)
            {
                // Пример использования

                // 1. Логгер с текстовым файлом
                var textLogger = new MyLogger(new TextFileLogRepository("logs.txt"));
                textLogger.Log("This is a log message to the text file.");
                textLogger.Log("Another message for the text file.");

                Console.WriteLine("Logs from text file:");
                foreach (var log in textLogger.GetLogs())
                {
                    Console.WriteLine(log);
                }

                // 2. Логгер с JSON-файлом
                var jsonLogger = new MyLogger(new JsonFileLogRepository("logs.json"));
                jsonLogger.Log("This is a log message to the JSON file.");
                jsonLogger.Log("Another message for the JSON file.");

                Console.WriteLine("\nLogs from JSON file:");
                foreach (var log in jsonLogger.GetLogs())
                {
                    Console.WriteLine(log);
                }
            }
        }
    }
}
