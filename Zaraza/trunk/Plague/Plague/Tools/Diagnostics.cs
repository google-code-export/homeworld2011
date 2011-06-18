using System;
using System.IO;
using System.Text;
using PlagueEngine.TimeControlSystem;
using System.Diagnostics;
using System.Drawing;

namespace PlagueEngine
{
    public enum LogingLevel{INFO, DEBUG, WARN, ERROR};
    /********************************************************************************/
    /// Diagnostics
    /********************************************************************************/
    static class Diagnostics
    {

        /****************************************************************************/
        private static long         _frames;
        private static TimeSpan     _elapsedTime         = TimeSpan.Zero;
        private static TimeSpan     _totalElapsedTime    = TimeSpan.Zero;
        
        private static bool         _forceGCOnUpdate;
        
        private static Game         _game;
        private static bool         _showDiagnostics     = true;
        private static TextWriter   _textWriter;
        private static String       _logFile             = String.Empty;
        private static LogWindow    _logWindow;
        private static bool         _showLogWindow;
        private static long         _allocatedMemory     = -1;
        private static TimeSpan     _memoryElapsedTime   = TimeSpan.Zero;
        private static string       _lineBrake           = "-------------------------";
        private static uint         _timerId;
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
            _memoryElapsedTime += deltaTime;

            if (_elapsedTime.Seconds >= 1)
            {
                FPS = _frames / _elapsedTime.Seconds;
                _frames = 0;
                _elapsedTime = TimeSpan.Zero;
            }

            if (_memoryElapsedTime.Seconds >= 10)
            {
                AllocatedManagedMemoryUpdate();
                _memoryElapsedTime = TimeSpan.Zero;
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
                sb.AppendLine(_lineBrake);
                if (_logWindow != null && !_logWindow.TextBox.IsDisposed)
                {
                    _logWindow.TextBox.AppendText(sb.ToString());
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
                catch (IOException)
                {
                    PushLog(LogingLevel.ERROR, "Nie udało się utworzyć pliku logu o nazwie "+_logFile);
                }
                if (_textWriter != null)
                {
                    _textWriter.Write(sb.ToString());
                    _textWriter.Flush();
                }
                return (_logWindow != null && !_logWindow.TextBox.IsDisposed) || _textWriter != null;
            }
            else{
                PushLog(LogingLevel.WARN, "Próba utworzenia nowego pliku logu w czasie gdy jest używany inny plik.");
            }
            return false;
        }
        /****************************************************************************/
        private static Color LogColor(LogingLevel loginglevel)
        {
            switch (loginglevel)
            {
                case LogingLevel.DEBUG:
                    return Color.Green;
                    break;
                case LogingLevel.ERROR:
                    return Color.Red;
                    break;
                case LogingLevel.WARN:
                    return Color.LightSalmon;
                    break;
                case LogingLevel.INFO:
                    return Color.Blue;
                    break;
                default:
                    return Color.Green;
                    break;
            }
        }

        /****************************************************************************/
        /// Push Log
        /****************************************************************************/
        public static void PushLog(String text)
        {
            PushLog(LogingLevel.DEBUG, text);
        }

        public static void PushLog(Object obj, String text)
        {
            PushLog(LogingLevel.DEBUG, obj, text);
        }
        public static void PushLog(LogingLevel loginglevel, String text)
        {
            if (_textWriter == null && _logWindow == null) return;
 
            var sb = new StringBuilder();
            sb.Append(">> ");
            sb.Append(DateTime.Now.ToString(@"HH\:mm\:ss"));
            sb.Append(" | ");
            sb.Append(_totalElapsedTime.ToString(@"hh\:mm\:ss"));
            sb.Append(" >> ");
            sb.Append("[");
            sb.Append(loginglevel.ToString());
            sb.Append("] ");
            sb.Append(text);
            sb.AppendLine();

            if (_logWindow != null && !_logWindow.TextBox.IsDisposed)
            {
                _logWindow.TextBox.SuspendLayout();
                _logWindow.TextBox.SelectionColor = LogColor(loginglevel);
                _logWindow.TextBox.AppendText(sb.ToString());
                _logWindow.TextBox.ResumeLayout();
            }
            if (_textWriter != null) 
            {
                _textWriter.Write(sb.ToString());
            }
            
        }

        public static void PushLog(LogingLevel loginglevel, Object obj, String text)
        {
            var sb = new StringBuilder();
            sb.Append(obj.GetType().Name);
            sb.Append(":");
            sb.Append(text);
            PushLog(loginglevel, sb.ToString());
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Close Log File
        /****************************************************************************/
        public static void CloseLogFile()
        {
            if (_textWriter != null)
            {
                _textWriter.WriteLine(_lineBrake);
                _textWriter.WriteLine("Run Time: " + _totalElapsedTime.ToString(@"hh\:mm\:ss"));
                _textWriter.Flush();
                _textWriter.Close();
            }
            if (_logWindow != null)
                {
                    _logWindow.Close();
                    _logWindow       = null;
                    _showLogWindow   = false;
                }
        }
        /****************************************************************************/
                     

        /****************************************************************************/

        /// FPS
        /****************************************************************************/
        public static long FPS { get; private set; }

        /****************************************************************************/


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

                    _textWriter = new StreamWriter(_logFile,true);
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
            else TimeControl.ResetTimer(_timerId,time,-1);
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
            _allocatedMemory=Process.GetCurrentProcess().PrivateMemorySize64 / 1024;
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