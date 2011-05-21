using System;
using System.IO;
using System.Text;
using PlagueEngine.TimeControlSystem;
using System.Diagnostics;

namespace PlagueEngine
{

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
                _logFile = (directory == null ? String.Empty : directory + "\\") + DateTime.Now.ToString(@"HH-mm-ss dd-MM-yy") + ".txt";

                try
                {
                    _textWriter = new StreamWriter(_logFile);
                }
                catch (IOException)
                {
                    if (directory != null)
                    {
                        Directory.CreateDirectory(directory + "\\");
                        _textWriter = new StreamWriter(_logFile);
                    }
                    else
                    {
                        return false;
                    }
                }
                
                _textWriter.WriteLine(_game.Title);
                _textWriter.WriteLine(DateTime.Now.ToString());
                _textWriter.WriteLine(_lineBrake);
                _textWriter.Flush();

                if (_logWindow != null)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine(_game.Title);
                    sb.AppendLine(DateTime.Now.ToString());
                    sb.AppendLine(_lineBrake);
                    _logWindow.TextBox.Text += sb.ToString();
                }

                return true;
            }

            return false;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Push Log
        /****************************************************************************/
        public static void PushLog(String text)
        {
            if (_textWriter == null) return;
            if (_logWindow == null) return;

            var sb = new StringBuilder();
            sb.Append(">> ");
            sb.Append(DateTime.Now.ToString(@"HH\:mm\:ss"));
            sb.Append(" | ");
            sb.Append(_totalElapsedTime.ToString(@"hh\:mm\:ss"));
            sb.Append(" >> ");
            sb.Append(text);
            sb.AppendLine();
            _logWindow.TextBox.Text += sb.ToString();
                
            _logWindow.TextBox.SelectionStart = _logWindow.TextBox.TextLength;
            _logWindow.TextBox.ScrollToCaret();
        }

        public static void PushLog(Object obj, String text)
        {
            if (_textWriter == null) return;
            if (_logWindow == null) return;

            var sb = new StringBuilder();
            sb.Append(">> ");
            sb.Append(DateTime.Now.ToString(@"HH\:mm\:ss"));
            sb.Append(" | ");
            sb.Append(_totalElapsedTime.ToString(@"hh\:mm\:ss"));
            sb.Append(" >> ");
            sb.Append(obj.GetType().Name);
            sb.Append(":");
            sb.Append(text);
            sb.AppendLine();
            _logWindow.TextBox.Text += sb.ToString();

            _logWindow.TextBox.SelectionStart = _logWindow.TextBox.TextLength;
            _logWindow.TextBox.ScrollToCaret();
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

                if (_logWindow != null)
                {
                    _logWindow.Close();
                    _logWindow       = null;
                    _showLogWindow   = false;
                }
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