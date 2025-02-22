﻿using System;
using sysIO = System.IO;
using Cosmos.Debug.Kernel;
using Cosmos.HAL;
using Cosmos.System.ScanMaps;

namespace Cosmos.System
{
    // MtW: if the fullname (namespace + name) of this class changes, please also change IL2CPU msbuild task
    /// <summary>
    /// Provides a base kernel class for a Cosmos-based system
    /// </summary>
    public abstract class Kernel
    {
        /// <summary>
        /// User ring debugger instance, with the tag "Kernel".
        /// </summary>
        public readonly Debugger mDebugger = new Debugger("User", "Kernel");

        /// <summary>
        /// Clear screen.
        /// </summary>
        public bool ClearScreen = true;

        // Set after initial start. Can be started and stopped at same time
        /// <summary>
        /// Kernel started.
        /// </summary>
        protected bool mStarted = false;
        // Set to signal stopped
        /// <summary>
        /// Kernel stopped.
        /// </summary>
        protected bool mStopped = false;

        /// <summary>
        /// Get text screen device.
        /// </summary>
        /// <returns>null</returns>
        protected virtual TextScreenBase GetTextScreen()
        {
            // null means use default
            return null;
        }

        /// <summary>
        /// Get keyboard key layout.
        /// </summary>
        /// <returns>Keyboard key layout.</returns>
        protected ScanMapBase GetKeyboardScanMap()
        {
            return KeyboardManager.GetKeyLayout();
        }

        /// <summary>
        /// Set keyboard key layout.
        /// </summary>
        /// <param name="ScanMap">Keyboard key layout.</param>
        protected void SetKeyboardScanMap(ScanMapBase ScanMap)
        {
            KeyboardManager.SetKeyLayout(ScanMap);
        }

        /// <summary>
        /// Start the system up using the properties for configuration.
        /// </summary>
        /// <exception cref="sysIO.IOException">Thrown on IO error.</exception>
        public virtual void Start()
        {
            try
            {
                Global.mDebugger.Send("Starting kernel");
                if (mStarted)
                {
                    Global.mDebugger.Send("ERROR: Kernel Already Started");
                    throw new Exception("Kernel has already been started. A kernel cannot be started twice.");
                }
                mStarted = true;

                if (string.Empty == null)
                {
                    throw new Exception("Compiler didn't initialize System.String.Empty!");
                }

                Global.mDebugger.Send("HW Bootstrap Init");
                HAL.Bootstrap.Init();

                Global.mDebugger.Send("Global Init");
                Global.Init(GetTextScreen());

                // Provide the user with a clear screen if they requested it
                if (ClearScreen)
                {
                    Global.mDebugger.Send("Cls");
                    //Global.Console.Clear();
                }

                Global.mDebugger.Send("Before Run");
                BeforeRun();

                // now enable interrupts:
                HAL.Global.EnableInterrupts();

                Global.mDebugger.Send("Run");
                if (mStopped)
                {
                    Global.mDebugger.Send("Already stopped");
                }
                else
                {
                    Global.mDebugger.Send("Not yet stopped");
                }
                while (!mStopped)
                {
                    //Network.NetworkStack.Update();
                    Run();
                }
                Global.mDebugger.Send("AfterRun");
                AfterRun();
                //bool xTest = 1 != 3;
                //while (xTest) {
                //}
            }
            catch (Exception E)
            {
                // todo: better ways to handle?
                global::System.Console.WriteLine("Exception occurred while running kernel:");
                global::System.Console.WriteLine(E.ToString());
            }
        }

        /// <summary>
        /// Pre-run events
        /// </summary>
        protected virtual void BeforeRun() { }

        /// <summary>
        /// Main kernel loop
        /// </summary>
        protected abstract void Run();

        /// <summary>
        /// After the Run() method is exited (?)
        /// </summary>
        protected virtual void AfterRun() { }

        /// <summary>
        /// Shut down the system and power off
        /// </summary>
        public void Stop()
        {
            mStopped = true;
        }

        /// <summary>
        /// Kernal object constructor.
        /// </summary>
        public Kernel()
        {
            Global.mDebugger.Send("In Cosmos.System.Kernel..ctor");
        }

        // Shutdown and restart
        /// <summary>
        /// Shutdown and restart.
        /// Not implemented.
        /// </summary>
        public void Restart()
        {
        }

        /// <summary>
        /// Print message to the debbuger at system ring with "Global"-tag.
        /// </summary>
        /// <param name="message">A message to print.</param>
        public static void PrintDebug(string message)
        {
            Global.mDebugger.Send(message);
        }

        /// <summary>
        /// Get interrupts status.
        /// </summary>
        public static bool InterruptsEnabled
        {
            get
            {
                return HAL.Global.InterruptsEnabled;
            }
        }
    }
}
