using System;
using System.IO;
using System.Text;
using PlagueEngine.TimeControlSystem;
using System.Diagnostics;
using System.Drawing;

namespace PlagueEngine
{
    public enum LoggingLevel
    {
        OFF = 150,
        NONE = 125,
        INFO = 100,
        DEBUG = 75,
        WARN = 50,
        ERROR = 25,
        FATAL = 0
    };
    /********************************************************************************/
    /// Diagnostics
    /********************************************************************************/
    static class Diagnostics
    {

        /****************************************************************************/
        private static long _frames;
        private static TimeSpan _elapsedTime = TimeSpan.Zero;
        private static TimeSpan _totalElapsedTime = TimeSpan.Zero;

        private static bool _forceGCOnUpdate;
        private static int _level = 150;
        private static Game _game;
        private static bool _showDiagnostics = true;
        private static TextWriter _textWriter;
        private static String _logFile = String.Empty;
        private static LogWindow _logWindow;
        private static bool _showLogWindow;
        private static long _allocatedMemory = -1;
        private static readonly ExpireClock MemoryClock = ExpireClock.FromSeconds(10);
        private const string LineBrake = "-------------------------";
        private static uint _timerId;
        private delegate void UpdaterDelegate();
        /****************************************************************************/


        /****************************************************************************/
        /// Update
        /****************************************************************************/
        public static void Update(TimeSpan deltaTime)
        {
            ++_frames;
            if (_allocatedMemory == -1)
            {
                AllocatedManagedMemoryUpdate();
            }

            _elapsedTime += deltaTime;
            _totalElapsedTime += deltaTime;

            if (_elapsedTime.Seconds >= 1)
            {
                FPS = _frames / _elapsedTime.Seconds;
                _frames = 0;
                _elapsedTime = TimeSpan.Zero;
            }

            if (MemoryClock.isExpired())
            {
                AllocatedManagedMemoryUpdate();
            }

            if (_forceGCOnUpdate) GC.Collect();
            if (_showDiagnostics) _game.Window.Title = _game.Title + " | " + ToString();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Open Log File
        /****************************************************************************/
        public static bool OpenLogFile(String directory = null)
        {
            if (_textWriter == null)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_game.Title);
                sb.AppendLine(DateTime.Now.ToString());
                sb.AppendLine(LineBrake);
                if (_logWindow != null && !_logWindow.TextBox.IsDisposed)
                {
                    PushLog(LoggingLevel.NONE, sb.ToString());
                }
                if (!String.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory + "\\");
                }

                _logFile = (directory == null ? String.Empty : directory + "\\") + DateTime.Now.ToString(@"HH-mm-ss dd-MM-yy") + ".txt";
                try
                {
                    _textWriter = new StreamWriter(_logFile);
                }
                catch (IOException e)
                {
                    PushLog(LoggingLevel.ERROR, "There was an error while crating log file " + _logFile + ". Exception:"+ e.InnerException.Message);
                }
                if (_textWriter != null)
                {
                    _textWriter.Write(sb.ToString());
                }
                return (_logWindow != null && !_logWindow.TextBox.IsDisposed) || _textWriter != null;
            }
            PushLog(LoggingLevel.WARN, "You can not create new log file while the old one is in use.");
            return false;
        }
        /****************************************************************************/

        public static void Debug(String message)
        {
            PushLog(LoggingLevel.DEBUG, message);
        }

        public static void Info(String message)
        {
            PushLog(LoggingLevel.INFO, message);
        }

        public static void Warn(String message)
        {
            PushLog(LoggingLevel.WARN, message);
        }

        public static void Error(String message)
        {
            PushLog(LoggingLevel.ERROR, message);
        }

        public static void Fatal(String message)
        {
            PushLog(LoggingLevel.FATAL, message);
        }

        private static Color LogColor(LoggingLevel logginglevel)
        {
            switch (logginglevel)
            {
                case LoggingLevel.NONE:
                    return Color.Black;
                case LoggingLevel.DEBUG:
                    return Color.Green;
                case LoggingLevel.ERROR:
                    return Color.Red;
                case LoggingLevel.WARN:
                    return Color.OrangeRed;
                case LoggingLevel.INFO:
                    return Color.Blue;
                case LoggingLevel.FATAL:
                    return Color.DarkRed;
                default:
                    return Color.Green;
            }
        }
        public static void PushMessage(String text)
        {
            PushLog(LoggingLevel.NONE, text);
        }
        /****************************************************************************/
        /// Push Log
        /****************************************************************************/
        public static void PushLog(String text)
        {
            PushLog(LoggingLevel.DEBUG, text);
        }

        public static void PushLog(Object obj, String text)
        {
            PushLog(LoggingLevel.DEBUG, obj, text);
        }
        public static void PushLog(LoggingLevel logginglevel, String text)
        {
            if ((int)logginglevel >= _level)
            {
                if (_textWriter == null && _logWindow == null) return;

                var sb = new StringBuilder();
                sb.Append(">> ");
                sb.Append(DateTime.Now.ToString(@"HH\:mm\:ss"));
                sb.Append(" | ");
                sb.Append(_totalElapsedTime.ToString(@"hh\:mm\:ss"));
                sb.Append(" >> ");
                sb.Append("[");
                sb.Append(logginglevel.ToString());
                sb.Append("] ");
                sb.Append(text);
                sb.AppendLine();

                if (_logWindow != null && !_logWindow.TextBox.IsDisposed)
                {
                    if (_logWindow.TextBox.InvokeRequired)
                    {
                        _logWindow.TextBox.Invoke(new UpdaterDelegate(() => UpdateTextBox(logginglevel, sb.ToString())));
                    }
                    else
                    {
                        UpdateTextBox(logginglevel, sb.ToString());
                    }

                }
                if (_textWriter != null)
                {
                    lock (_textWriter)
                    {
                        _textWriter.Write(sb.ToString());
                    }
                }
            }

        }
        private static void UpdateTextBox(LoggingLevel logginglevel, string text)
        {
            lock (_logWindow.TextBox)
            {
                _logWindow.TextBox.SuspendLayout();
                _logWindow.TextBox.SelectionColor = LogColor(logginglevel);
                _logWindow.TextBox.AppendText(text);
                _logWindow.TextBox.ResumeLayout();
            }
        }
        public static void PushLog(LoggingLevel logginglevel, Object obj, String text)
        {
            var sb = new StringBuilder();
            sb.Append(obj.GetType().Name);
            sb.Append(":");
            sb.Append(text);
            PushLog(logginglevel, sb.ToString());
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Close Log File
        /****************************************************************************/
        public static void CloseLogFile()
        {
            if (_textWriter != null)
            {
                _textWriter.WriteLine(LineBrake);
                _textWriter.WriteLine("Run Time: " + _totalElapsedTime.ToString(@"hh\:mm\:ss"));
                _textWriter.Flush();
                _textWriter.Close();
            }
            if (_logWindow != null)
            {
                _logWindow.Close();
                _logWindow = null;
                _showLogWindow = false;
            }
        }
        /****************************************************************************/


        /****************************************************************************/

        /// FPS
        /****************************************************************************/
        public static long FPS { get; private set; }

        /****************************************************************************/
        public static LoggingLevel Level
        {
            get { return (LoggingLevel)_level; }
            set { _level = (int)value; }
        }

        /****************************************************************************/
        /// Run Time
        /****************************************************************************/
        public static TimeSpan RunTime
        {
            get
            {
                return _totalElapsedTime;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Allocated Managed Memory
        /****************************************************************************/
        public static long AllocatedManagedMemory
        {
            get
            {
                return _allocatedMemory;
            }
            set
            {
                _allocatedMemory = value;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Force GC On Update
        /****************************************************************************/
        public static bool ForceGCOnUpdate
        {
            set
            {
                _forceGCOnUpdate = value;
            }

            get
            {
                return _forceGCOnUpdate;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Game
        /****************************************************************************/
        public static Game Game
        {
            set
            {
                _game = value;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Limit Update Time Step
        /****************************************************************************/
        public static bool LimitUpdateTimeStep
        {
            set
            {
                _game.IsFixedTimeStep = value;
            }

            get
            {
                return _game.IsFixedTimeStep;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Show Diagnostics
        /****************************************************************************/
        public static bool ShowDiagnostics
        {
            set
            {
                _showDiagnostics = value;
                if (!_showDiagnostics) _game.Window.Title = _game.Title;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Show Log Window
        /****************************************************************************/
        public static bool ShowLogWindow
        {
            set
            {
                _showLogWindow = value;

                if (_showLogWindow && _logWindow == null && _textWriter != null)
                {
                    _logWindow = new LogWindow();
                    _logWindow.Show();
                    _textWriter.Close();

                    using (TextReader textReader = new StreamReader(_logFile))
                    {
                        _logWindow.TextBox.Text += textReader.ReadToEnd();
                    }

                    _textWriter = new StreamWriter(_logFile, true);
                }
                else if (_showLogWindow && _logWindow == null && _textWriter == null)
                {
                    _logWindow = new LogWindow();
                    _logWindow.Show();
                }
                else if (!_showLogWindow && _logWindow != null)
                {
                    _logWindow.Close();
                    _logWindow = null;
                }
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Diagnostic Snapshot
        /****************************************************************************/
        public static void DiagnosticSnapshot()
        {
#if DEBUG
            PushLog(string.Format("FPS: {0} | Allocated Managed Memory: {1} kb", FPS, _allocatedMemory));
#endif
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Start Diagnostic Snapshot
        /****************************************************************************/
        public static void StartDiagnosticSnapshots(TimeSpan time)
        {
            if (_timerId == 0) _timerId = TimeControl.CreateTimer(time, -1, TimerCallback);
            else TimeControl.ResetTimer(_timerId, time, -1);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Stop Diagnostic Snapshot
        /****************************************************************************/
        public static void StopDiagnosticSnapshots()
        {
            TimeControl.ReleaseTimer(_timerId);
            _timerId = 0;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Timer Callback
        /****************************************************************************/
        private static void TimerCallback()
        {
            DiagnosticSnapshot();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Memory Update
        /****************************************************************************/
        private static void AllocatedManagedMemoryUpdate()
        {
            _allocatedMemory = Process.GetCurrentProcess().PrivateMemorySize64 / 1024;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// To String
        /****************************************************************************/
        public new static String ToString()
        {
            return string.Format("FPS: {0} | Run Time: {1} | Allocated Memory: {2} kb", FPS, _totalElapsedTime.ToString(@"hh\:mm\:ss"), _allocatedMemory);
        }
        /****************************************************************************/


    }
    /********************************************************************************/

}
/************************************************************************************/