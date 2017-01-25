/*
 * Copyright © 2011 
 * Rowe Technology Inc.
 * All rights reserved.
 * http://www.rowetechinc.com
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification is NOT permitted.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
 * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE 
 * COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
 * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES INCLUDING,
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
 * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT 
 * LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
 * ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
 * POSSIBILITY OF SUCH DAMAGE.
 * 
 * HISTORY
 * -----------------------------------------------------------------
 * Date            Initials    Comments
 * -----------------------------------------------------------------
 * 09/01/2011      RC          Initial coding
 * 10/18/2011      RC          Add Shutdown command    
 * 
 */

using System.Linq;
using System.Windows;

using System.ComponentModel;
using System.Diagnostics;
using System;
using MahApps.Metro.Controls;

namespace RTI
{
    ///// <summary>
    ///// Class representing the shell of the application.
    ///// The shell is a window.
    ///// </summary>
    //public partial class Shell : Window
    //{
    //    /// <summary>
    //    ///  Initialize the shell.
    //    ///  Setup the shutdown command.
    //    /// </summary>
    //    public Shell()
    //    {
    //        InitializeComponent();
    //        AppCommands.ShutdownCommand.CanExecuteChanged += OnShutdownCanExecuteChanged;
    //    }

    //    /// <summary>
    //    /// Called when shutdown has been started.
    //    /// This will send a command to all that a 
    //    /// shutdown is about to occur.
    //    /// </summary>
    //    /// <param name="sender">Object sending command.</param>
    //    /// <param name="e">Arguments for the command.</param>
    //    private void OnWindowClosing(object sender, CancelEventArgs e)
    //    {
    //        //Debug.WriteLine("Shell.xaml.cs Shutting Down");

    //        // Send to all view that a shutdown is about to occur
    //        if (AppCommands.ShutdownCommand.CanExecute(e))
    //        {
    //            AppCommands.ShutdownCommand.Execute(e);
    //        }

    //        if (e.Cancel)
    //        {
    //            // If someone cancels the shutdown
    //        }
    //    }

    //    /// <summary>
    //    /// Check if shutdown can occur.
    //    /// Not implemented.
    //    /// </summary>
    //    /// <param name="sender">Sender of command.</param>
    //    /// <param name="e">Arguments for the command.</param>
    //    private void OnShutdownCanExecuteChanged(object sender, EventArgs e)
    //    {
    //        // Shutdown is occuring
    //    }
    //}

    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : MetroWindow
    {
        /// <summary>
        /// Initialize the Shell View.
        /// </summary>
        public ShellView()
        {
            InitializeComponent();
        }
    }
}
