using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using PlagueEngine.TimeControlSystem;


/************************************************************************************/
/// PlagueEngine
/************************************************************************************/
namespace PlagueEngine
{

    /********************************************************************************/
    /// Diagnostics
    /********************************************************************************/
    static class Diagnostics
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private static long         fPS                 = 0;
        private static long         frames              = 0;
        private static TimeSpan     elapsedTime         = TimeSpan.Zero;
        private static TimeSpan     totalElapsedTime    = TimeSpan.Zero;
        
        private static bool         forceGCOnUpdate     = false;
        
        private static Game         game                = null;
        private static bool         showDiagnostics     = true;
        private static TextWriter   textWriter          = null;
        private static String       logFile             = String.Empty;
        private static LogWindow    logWindow           = null;
        private static bool         showLogWindow       = false;

        private static uint         timerID             = 0;
        /****************************************************************************/


        /****************************************************************************/
        /// Update
        /****************************************************************************/
        public static void Update(TimeSpan deltaTime)
        {
            ++frames;

            elapsedTime += deltaTime;
            totalElapsedTime += deltaTime;

            if (elapsedTime.Seconds >= 1)
            {
                fPS = frames / elapsedTime.Seconds;
                frames = 0;
                elapsedTime = TimeSpan.Zero;
            }

            if (forceGCOnUpdate) GC.Collect();
            if (showDiagnostics) game.Window.Title = game.Title + " | " + Diagnostics.ToString();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Open Log File
        /****************************************************************************/
        public static bool OpenLogFile(String directory = null)
        {
            if (textWriter == null)
            {
                logFile = (directory == null ? String.Empty : directory + "\\") + DateTime.Now.ToString(@"HH-mm-ss dd-MM-yy") + ".txt";

                try
                {
                    textWriter = new StreamWriter(logFile);
                }
                catch (IOException)
                {
                    if (directory != null)
                    {
                        Directory.CreateDirectory(directory + "\\");
                        textWriter = new StreamWriter(logFile);
                    }
                    else
                    {
                        return false;
                    }
                }
                
                textWriter.WriteLine(game.Title);
                textWriter.WriteLine(DateTime.Now.ToString());
                textWriter.WriteLine("-------------------------");
                textWriter.Flush();

                if (logWindow != null)
                {
                    logWindow.TextBox.Text += game.Title                  + System.Environment.NewLine;
                    logWindow.TextBox.Text += DateTime.Now.ToString()     + System.Environment.NewLine;
                    logWindow.TextBox.Text += "-------------------------" + System.Environment.NewLine;
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
            if (textWriter == null) return;

            textWriter.WriteLine(">> " + DateTime.Now.ToString(@"HH\:mm\:ss")     + " | "
                                       + totalElapsedTime.ToString(@"hh\:mm\:ss") + " >> "
                                       + text);
            textWriter.Flush();

            if (logWindow != null)
            {
                logWindow.TextBox.Text += ">> " + DateTime.Now.ToString(@"HH\:mm\:ss") + " | "
                                       + totalElapsedTime.ToString(@"hh\:mm\:ss")      + " >> "
                                       + text + System.Environment.NewLine;
                
                logWindow.TextBox.SelectionStart = logWindow.TextBox.TextLength;
                logWindow.TextBox.ScrollToCaret();
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Close Log File
        /****************************************************************************/
        public static void CloseLogFile()
        {
            if (textWriter != null)
            {
                textWriter.WriteLine("-------------------------");
                textWriter.WriteLine("Run Time: " + totalElapsedTime.ToString(@"hh\:mm\:ss"));
                textWriter.Flush();
                textWriter.Close();

                if (logWindow != null)
                {
                    logWindow.Close();
                    logWindow       = null;
                    showLogWindow   = false;
                }
            }
        }
        /****************************************************************************/
                     

        /****************************************************************************/
        /// FPS
        /****************************************************************************/
        public static long FPS
        {
            get
            {
                return fPS;                        
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Run Time
        /****************************************************************************/
        public static TimeSpan RunTime 
        {
            get
            {
                return totalElapsedTime;
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
                return GC.GetTotalMemory(false);
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
                forceGCOnUpdate = value;
            }

            get
            {
                return forceGCOnUpdate;
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
                game = value;
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
                game.IsFixedTimeStep = value;
            }

            get
            {
                return game.IsFixedTimeStep;
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
                showDiagnostics = value;
                if (!showDiagnostics) game.Window.Title = game.Title;
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
                showLogWindow = value;
                
                if (showLogWindow && logWindow == null && textWriter != null)
                {
                    logWindow = new LogWindow();
                    logWindow.Show();
                    textWriter.Close();

                    using (TextReader textReader = new StreamReader(logFile))
                    {
                        logWindow.TextBox.Text += textReader.ReadToEnd();
                    }

                    textWriter = new StreamWriter(logFile,true);
                }
                else if (showLogWindow && logWindow == null && textWriter == null)
                {
                    logWindow = new LogWindow();
                    logWindow.Show();                
                }
                else if (!showLogWindow && logWindow != null)
                {
                    logWindow.Close();
                    logWindow = null;
                }
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Diagnostic Snapshot
        /****************************************************************************/
        public static void DiagnosticSnapshot()
        { 
            Diagnostics.PushLog("FPS: " + fPS.ToString() + " | Allocated Managed Memory: "  
                                + (GC.GetTotalMemory(false)/1024).ToString() + " kb");               
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Start Diagnostic Snapshot
        /****************************************************************************/
        public static void StartDiagnosticSnapshots(TimeSpan time)
        {
            if (timerID == 0) timerID = TimeControl.CreateTimer(time, -1, TimerCallback);
            else TimeControl.ResetTimer(timerID,time,-1);            
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Stop Diagnostic Snapshot
        /****************************************************************************/
        public static void StopDiagnosticSnapshots()
        {
            TimeControl.ReleaseTimer(timerID);
            timerID = 0;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Timer Callback
        /****************************************************************************/
        private static void TimerCallback()
        {
            Diagnostics.DiagnosticSnapshot();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// To String
        /****************************************************************************/
        public static String ToString()
        {
            return "FPS: "                          + fPS.ToString()                             + 
                   " | Run Time: "                  + totalElapsedTime.ToString(@"hh\:mm\:ss")   +
                   " | Allocated Managed Memory: "  + (GC.GetTotalMemory(false)/1024).ToString() + " kb";
        }
        /****************************************************************************/


    }
    /********************************************************************************/
    
}
/************************************************************************************/