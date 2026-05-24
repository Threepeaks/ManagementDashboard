using ManagementDashboard.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;

namespace ManagementDashboard.Services
{
    public static class UserActionLogBuffer
    {
        private const int FlushIntervalMilliseconds = 5000;
        private static readonly object SyncRoot = new object();
        private static Timer flushTimer;
        private static bool isStarted;

        public static void Start()
        {
            lock (SyncRoot)
            {
                if (isStarted)
                {
                    return;
                }

                Directory.CreateDirectory(BufferDirectory);
                flushTimer = new Timer(_ => Flush(), null, FlushIntervalMilliseconds, FlushIntervalMilliseconds);
                isStarted = true;
            }

            Flush();
        }

        public static void Stop()
        {
            lock (SyncRoot)
            {
                flushTimer?.Dispose();
                flushTimer = null;
                isStarted = false;
            }

            Flush();
        }

        public static void Enqueue(UserActionLog userActionLog)
        {
            Start();

            var record = BufferedUserActionLog.FromUserActionLog(userActionLog);
            var line = JsonConvert.SerializeObject(record) + Environment.NewLine;
            var bytes = Encoding.UTF8.GetBytes(line);

            try
            {
                lock (SyncRoot)
                {
                    Directory.CreateDirectory(BufferDirectory);

                    using (var stream = new FileStream(PendingFilePath, FileMode.Append, FileAccess.Write, FileShare.Read, 4096, FileOptions.WriteThrough))
                    {
                        stream.Write(bytes, 0, bytes.Length);
                        stream.Flush(true);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Failed to persist user action log to buffer. Falling back to direct database write. {0}", ex);
                WriteDirectly(userActionLog);
            }
        }

        public static void Flush()
        {
            string[] files;

            lock (SyncRoot)
            {
                Directory.CreateDirectory(BufferDirectory);
                RotatePendingFile();
                files = Directory.GetFiles(BufferDirectory, "*.log")
                    .Where(x => !string.Equals(x, PendingFilePath, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(x => x)
                    .ToArray();
            }

            foreach (var file in files)
            {
                FlushFile(file);
            }
        }

        private static string BufferDirectory
        {
            get
            {
                var basePath = HttpRuntime.AppDomainAppPath ?? AppDomain.CurrentDomain.BaseDirectory;
                return Path.Combine(basePath, "App_Data", "UserActionLogBuffer");
            }
        }

        private static string PendingFilePath => Path.Combine(BufferDirectory, "pending.log");

        private static void RotatePendingFile()
        {
            if (!File.Exists(PendingFilePath) || new FileInfo(PendingFilePath).Length == 0)
            {
                return;
            }

            var processingPath = Path.Combine(BufferDirectory, "processing-" + DateTime.UtcNow.ToString("yyyyMMddHHmmssffff") + "-" + Guid.NewGuid().ToString("N") + ".log");
            File.Move(PendingFilePath, processingPath);
        }

        private static void FlushFile(string filePath)
        {
            List<UserActionLog> logs;

            try
            {
                logs = ReadLogs(filePath);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Failed to read buffered user action log file '{0}'. {1}", filePath, ex);
                return;
            }

            if (!logs.Any())
            {
                MoveBadFile(filePath);
                return;
            }

            try
            {
                using (var db = new ApplicationDbContext())
                {
                    var logsToInsert = logs.Where(log => !AlreadySaved(db, log)).ToList();

                    if (logsToInsert.Any())
                    {
                        db.UserActionLogs.AddRange(logsToInsert);
                        db.SaveChanges();
                    }
                }

                File.Delete(filePath);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Failed to flush buffered user action logs from '{0}'. The file will be retried. {1}", filePath, ex);
            }
        }

        private static List<UserActionLog> ReadLogs(string filePath)
        {
            var logs = new List<UserActionLog>();

            foreach (var line in File.ReadLines(filePath))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                try
                {
                    var record = JsonConvert.DeserializeObject<BufferedUserActionLog>(line);
                    if (record != null)
                    {
                        logs.Add(record.ToUserActionLog());
                    }
                }
                catch (JsonException ex)
                {
                    Trace.TraceError("Skipped corrupt user action log buffer line from '{0}'. {1}", filePath, ex);
                }
            }

            return logs;
        }

        private static bool AlreadySaved(ApplicationDbContext db, UserActionLog log)
        {
            return db.UserActionLogs.Any(existing =>
                existing.DateTimeUtc == log.DateTimeUtc &&
                existing.UserName == log.UserName &&
                existing.ControllerName == log.ControllerName &&
                existing.ActionName == log.ActionName &&
                existing.HttpMethod == log.HttpMethod &&
                existing.UrlAccessed == log.UrlAccessed &&
                existing.AccessedAt == log.AccessedAt);
        }

        private static void MoveBadFile(string filePath)
        {
            try
            {
                var badPath = Path.ChangeExtension(filePath, ".bad");

                if (File.Exists(badPath))
                {
                    badPath = Path.Combine(
                        Path.GetDirectoryName(filePath),
                        Path.GetFileNameWithoutExtension(filePath) + "-" + Guid.NewGuid().ToString("N") + ".bad");
                }

                File.Move(filePath, badPath);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Failed to move empty or invalid user action log buffer file '{0}'. {1}", filePath, ex);
            }
        }

        private static void WriteDirectly(UserActionLog userActionLog)
        {
            using (var db = new ApplicationDbContext())
            {
                db.UserActionLogs.Add(userActionLog);
                db.SaveChanges();
            }
        }

        private class BufferedUserActionLog
        {
            public DateTime DateTimeUtc { get; set; }
            public string UserName { get; set; }
            public string ControllerName { get; set; }
            public string ActionName { get; set; }
            public string HttpMethod { get; set; }
            public string UrlAccessed { get; set; }
            public DateTime AccessedAt { get; set; }

            public static BufferedUserActionLog FromUserActionLog(UserActionLog userActionLog)
            {
                return new BufferedUserActionLog
                {
                    DateTimeUtc = userActionLog.DateTimeUtc,
                    UserName = userActionLog.UserName,
                    ControllerName = userActionLog.ControllerName,
                    ActionName = userActionLog.ActionName,
                    HttpMethod = userActionLog.HttpMethod,
                    UrlAccessed = userActionLog.UrlAccessed,
                    AccessedAt = userActionLog.AccessedAt
                };
            }

            public UserActionLog ToUserActionLog()
            {
                return new UserActionLog
                {
                    DateTimeUtc = DateTimeUtc,
                    UserName = UserName,
                    ControllerName = ControllerName,
                    ActionName = ActionName,
                    HttpMethod = HttpMethod,
                    UrlAccessed = UrlAccessed,
                    AccessedAt = AccessedAt
                };
            }
        }
    }
}
