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
 * 07/09/2013      RC          3.0.3      Initial coding
 * 08/07/2013      RC          3.0.7      Made ProjectImage a BitmapImage so it can be disposed correctly and image can be refreshed.
 * 08/28/2013      RC          3.0.8      Fixed bug with DEFAULT_IMAGE_PATH where the path was not known when installed.
 * 12/02/2013      RC          3.2.0      Removed the project object so it would not be leftover when shutdowning.
 * 01/02/2014      RC          3.2.2      Added IsSelected property.
 * 08/07/2014      RC          4.0.0      Updated ReactiveCommand to 6.0.
 * 
 */

namespace RTI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.IO;
    using ReactiveUI;
    using System.Windows.Media.Imaging;
    using System.Reflection;
    using System.Windows;
    using Caliburn.Micro;
    using System.Diagnostics;
    using System.Threading.Tasks;

    /// <summary>
    /// Display the Project summary to the user.
    /// </summary>
    public class ProjectListItemViewModel : PulseViewModel
    {
        #region Variables

        /// <summary>
        /// Default file image.
        /// It is blank so no image will be displayed.
        /// </summary>
        private string DEFAULT_IMAGE_PATH = @"pack://application:,,,/"
                                                 + Assembly.GetExecutingAssembly().GetName().Name
                                                 + ";component/"
                                                 + "Images/blank_projectimage.png";

        /// <summary>
        /// Parent VM.
        /// </summary>
        private LoadProjectsViewModel _VM;

        /// <summary>
        /// Pulse manager.
        /// </summary>
        private readonly PulseManager _pm;

        #endregion

        #region Properties

        /// <summary>
        /// Project name.
        /// </summary>
        private string _ProjectName;
        /// <summary>
        /// Project name.
        /// </summary>
        public string ProjectName
        {
            get { return _ProjectName; }
            set
            {
                _ProjectName = value;
                this.NotifyOfPropertyChange(() => this.ProjectName);
            }
        }

        /// <summary>
        /// Project last modified date.
        /// </summary>
        private DateTime _LastDateModified;
        /// <summary>
        /// Project last modified date.
        /// </summary>
        public DateTime LastDateModified
        {
            get { return _LastDateModified; }
            set
            {
                _LastDateModified = value;
                this.NotifyOfPropertyChange(() => this.LastDateModified);
            }
        }

        /// <summary>
        /// Project Folder Path.
        /// </summary>
        private string _ProjectFolderPath;
        /// <summary>
        /// Project Folder Path.
        /// </summary>
        public string ProjectFolderPath
        {
            get { return _ProjectFolderPath; }
            set
            {
                _ProjectFolderPath = value;
                this.NotifyOfPropertyChange(() => this.ProjectFolderPath);
            }
        }

        /// <summary>
        /// Image for the Project.
        /// </summary>
        private BitmapImage _ProjectImage;
        /// <summary>
        /// Image for the Project.
        /// </summary>
        public BitmapImage ProjectImage
        {
            get { return _ProjectImage; }
            set
            {
                _ProjectImage = value;
                this.NotifyOfPropertyChange(() => this.ProjectImage);
            }
        }

        /// <summary>
        /// Number of ensembles in the project.
        /// </summary>
        private int _NumEnsembles;
        /// <summary>
        /// Number of ensembles in the project.
        /// </summary>
        public int NumEnsembles
        {
            get { return _NumEnsembles; }
            set
            {
                _NumEnsembles = value;
                this.NotifyOfPropertyChange(() => this.NumEnsembles);
            }
        }

        /// <summary>
        /// Loading Flag.
        /// </summary>
        private bool _IsLoading;
        /// <summary>
        /// Loading Flag.
        /// </summary>
        public bool IsLoading
        {
            get { return _IsLoading; }
            set
            {
                _IsLoading = value;
                this.NotifyOfPropertyChange(() => this.IsLoading);
            }
        }

        /// <summary>
        /// First Date/Time Ensemble.
        /// </summary>
        private string _FirstEnsDate;
        /// <summary>
        /// First Date/Time Ensemble.
        /// </summary>
        public string FirstEnsDate
        {
            get { return _FirstEnsDate; }
            set
            {
                _FirstEnsDate = value;
                this.NotifyOfPropertyChange(() => this.FirstEnsDate);
            }
        }

        /// <summary>
        /// Last Date/Time Ensemble.
        /// </summary>
        private string _LastEnsDate;
        /// <summary>
        /// Last Date/Time Ensemble.
        /// </summary>
        public string LastEnsDate
        {
            get { return _LastEnsDate; }
            set
            {
                _LastEnsDate = value;
                this.NotifyOfPropertyChange(() => this.LastEnsDate);
            }
        }

        /// <summary>
        /// IsSelected flag for the project.
        /// </summary>
        private bool _IsSelected;
        /// <summary>
        /// IsSelected flag for the project.
        /// </summary>
        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                _IsSelected = value;
                this.NotifyOfPropertyChange(() => this.IsSelected);
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Refresh the project image.
        /// </summary>
        public ReactiveCommand<System.Reactive.Unit> RefreshProjectImageCommand { get; protected set; }

        /// <summary>
        /// Delete this project.
        /// </summary>
        public ReactiveCommand<object> DeleteCommand { get; protected set; }

        #endregion

        /// <summary>
        /// Initialize the object with the project.
        /// </summary>
        /// <param name="project">Project for this VM.</param>
        /// <param name="vm">Load Project view model.</param>
        public ProjectListItemViewModel(Project project, LoadProjectsViewModel vm)
            : base(project.ProjectName)
        {
            // Initialize the project
            //_Project = project;
            ProjectName = project.ProjectName;
            LastDateModified = project.LastDateModified;
            ProjectFolderPath = project.ProjectFolderPath;
            _VM = vm;
            _pm = IoC.Get<PulseManager>();
            IsLoading = false;
            Task.Run(() => GetProjectImage(project));

            // Refresh the image command
            RefreshProjectImageCommand = ReactiveCommand.CreateAsyncTask(_ => RefreshProjectImage());

            // Delete the project command
            DeleteCommand = ReactiveCommand.Create();
            DeleteCommand.Subscribe(_ => DeleteProject());

            NumEnsembles = project.GetNumberOfEnsembles();
            GetFirstLastDate(project);
        }

        /// <summary>
        /// Dispose of this object.
        /// </summary>
        public override void Dispose()
        {
            //Project.Dispose();
            IsSelected = false;
        }


        #region Project Image

        /// <summary>
        /// Get the project async.
        /// </summary>
        /// <param name="project">Project to generate the image.</param>
        public async void GetProjectImage(Project project)
        {
            await Task.Run(() => GetProjectImageExecute(project));
        }

        /// <summary>
        /// Display the project image.  If the project image exist,
        /// it will display it.  If it does not exist the default image will be used.
        /// 
        /// Read the Image into a memorystream.  This way the image
        /// can be disposed.
        /// http://stackoverflow.com/questions/2423584/deleting-an-image-that-has-been-used-by-a-wpf-control
        /// 
        /// </summary>
        private void GetProjectImageExecute(Project project)
        {
            // By default display the default image
            string path = DEFAULT_IMAGE_PATH;

            // Get the Project image path
            string projectPath = project.GetProjectImagePath();

            try
            {
                // If the project contains an image
                // display the project image
                if (File.Exists(projectPath))
                {
                    path = projectPath;
                }

                // Load the Image
                BitmapImage bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.CreateOptions = BitmapCreateOptions.IgnoreImageCache;                         // To allow the image to refresh
                bmp.EndInit();                                                                    // Required for full initialization to complete at this time
                bmp.Freeze();                                                                     // Freeze the image source, used to move it across the thread

                System.Windows.Application.Current.Dispatcher.BeginInvoke(new System.Action(() =>
                {
                    _ProjectImage = bmp;
                }));
            }
            catch (IOException)
            {
                // The project image is in use
                // Most likely the background thread is still creating the image
                Debug.WriteLine("Exception GetProjectImage()");
            }

            //this.NotifyOfPropertyChange(() => this.ProjectImage);
        }

        /// <summary>
        /// Reproduce the project image.
        /// </summary>
        public async Task RefreshProjectImage()
        {
            await Task.Run(() => RefreshProjectImageExecute());
        }

        /// <summary>
        /// Reproduce the project image.
        /// </summary>
        public void RefreshProjectImageExecute()
        {
            IsLoading = true;

            // Refresh the project image
            _pm.RefreshProjectImage(ProjectName);

            // Get the Project from the PM
            Project prj = _pm.GetProject(_ProjectName);

            // Display the new image
            System.Windows.Application.Current.Dispatcher.BeginInvoke(new System.Action(() => 
            {
                GetProjectImage(prj);
                NumEnsembles = prj.GetNumberOfEnsembles();
                GetFirstLastDate(prj);
                prj.Dispose();

                IsLoading = false;
            }));
        }

        #endregion

        #region First Last Date

        /// <summary>
        /// Get the first and last ensemble date and time from the project.
        /// </summary>
        /// <param name="project">Project to get the date and time.</param>
        private void GetFirstLastDate(Project project)
        {
            DataSet.Ensemble firstEns = project.GetFirstEnsemble();
            DataSet.Ensemble lastEns = project.GetLastEnsemble();
            
            // Get first ensemble date/time
            if (firstEns != null && firstEns.IsEnsembleAvail)
            {
                FirstEnsDate = firstEns.EnsembleData.EnsDateTime.ToString();
            }

            // Get last ensemble date/time
            if (lastEns != null && lastEns.IsEnsembleAvail)
            {
                LastEnsDate = lastEns.EnsembleData.EnsDateTime.ToString();
            }
        }

        #endregion

        #region Delete Project

        /// <summary>
        /// Delete this project.
        /// </summary>
        private void DeleteProject()
        {
            _VM.DeleteProject(this);
        }

        #endregion

        #region Refresh

        /// <summary>
        /// Refresh the display.  This will refresh the image
        /// and all the properties.
        /// </summary>
        public async void RefreshDisplay()
        {
            await RefreshProjectImage();

            // Refresh the display
            this.NotifyOfPropertyChange(() => this.ProjectImage);
        }

        #endregion
    }
}
