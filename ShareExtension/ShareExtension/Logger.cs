namespace ShareExtension;
public static class Logger
    {
        // Log dosyasının yolu
        private static string logFilePath ;

        // Log seviyeleri
        public enum LogLevel
        {
            Info,
            Warning,
            Error
        }

    static Logger()
    {
        string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            logFilePath = Path.Combine(documentsPath, "share_extension_log.txt");
    }


    // Log kaydetme
    public static void Log(LogLevel level, string message)
        {
            string logMessage = $"[{DateTime.Now}] [{level}] {message}";

            // Konsola yazdırma (isteğe bağlı)
            Console.WriteLine(logMessage);

            // Log mesajını dosyaya kaydetme
            AppendToFile(logMessage);
        }

        // Dosyaya ekleme
        private static void AppendToFile(string logMessage)
        {
            try
            {
                if (!File.Exists(logFilePath))
                {
                    File.Create(logFilePath).Dispose();
                }

                File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Log dosyasına yazılamadı: {ex.Message}");
            }
        }

        // Log dosyasının yolunu almak
        public static string GetLogFilePath()
        {
            return logFilePath;
        }

        // Logları temizleme
        public static void ClearLog()
        {
            try
            {
                File.WriteAllText(logFilePath, string.Empty); // Dosyayı temizle
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Log dosyası temizlenemedi: {ex.Message}");
            }
        }
    }