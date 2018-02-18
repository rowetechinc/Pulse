/*
 * Copyright © 2013 
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
 * Date            Initials    Version    Comments
 * -----------------------------------------------------------------
 * 10/03/2013      RC          3.2.0      Initial coding
 * 08/07/2014      RC          4.0.0      Updated ReactiveCommand to 6.0.
 * 
 * 
 */

namespace RTI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Caliburn.Micro;
    using ReactiveUI;

    /// <summary>
    /// Ask the user if they want to create a new project or load a previous project.
    /// 
    /// </summary>
    public class ProjectViewModel : PulseViewModel
    {
        #region Variables

        /// <summary>
        ///  Setup logger
        /// </summary>
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Event aggregator.
        /// </summary>
        private readonly IEventAggregator _events;

        #endregion


        #region Commands

        /// <summary>
        /// Command to create a new project.
        /// </summary>
        public ReactiveCommand<object> NewProjectCommand { get; protected set; }

        /// <summary>
        /// Command to load a project.
        /// </summary>
        public ReactiveCommand<object> LoadProjectCommand { get; protected set; }

        #endregion

        /// <summary>
        /// Initalize the values.
        /// </summary>
        public ProjectViewModel()
            : base("Projects")
        {
            // Set Event Aggregator
            _events = IoC.Get<IEventAggregator>();

            // New Project command
            NewProjectCommand = ReactiveCommand.Create();
            NewProjectCommand.Subscribe(_ => NewProject());

            // Load Project command
            LoadProjectCommand = ReactiveCommand.Create();
            LoadProjectCommand.Subscribe(_ => LoadProject());

        }

        /// <summary>
        /// Shutdown this object.
        /// </summary>
        public override void Dispose()
        {

        }


        #region New Project

        /// <summary>
        /// Go to the new project view.
        /// </summary>
        private void NewProject()
        {
            _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.NewProjectView));
        }

        #endregion

        #region Load Project

        /// <summary>
        /// Go to the load project view.
        /// </summary>
        private void LoadProject()
        {
            _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.LoadProjectsView));
        }

        #endregion
    }
}
