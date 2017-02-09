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
 * 04/23/2013      RC          3.0.0       Initial coding
 * 05/07/2013      RC          3.0.0       Added ProjectManager as a singleton.
 * 05/09/2013      RC          3.0.0       Added ViewLocator.NameTransformer to Configure to allow navigation using ModernUI.
 * 06/17/2013      RC          3.0.1       Added ViewDataBaseViewModel.
 * 06/25/2013      RC          3.0.2       Added PlaybackViewModel.
 * 07/03/2013      RC          3.0.2       Added ValidationTestViewModel.
 * 07/11/2013      RC          3.0.4       Added CompassCalViewModel.
 * 07/15/2013      RC          3.0.4       Added ViewDataBaseTextViewModel and ViewDataGraphicalTextViewModel.
 * 07/23/2013      RC          3.0.4       Added ScreenDataBaseViewModel.
 * 07/25/2013      RC          3.0.4       Added ValidationTestBaseViewModel.
 * 07/29/2013      RC          3.0.6       Added DownloadDataViewModel.
 * 07/30/2013      RC          3.0.6       Added UpdateFirmwareViewModel.
 * 07/31/2013      RC          3.0.6       Added CompassUtilityViewModel.
 * 08/07/2013      RC          3.0.7       Added TerminalAdcpViewModel and TerminalGpsViewModel.
 * 08/23/2013      RC          3.0.8       Added AboutViewModel.
 * 08/26/2013      RC          3.0.8       Added ExportDataViewModel.
 * 10/03/2013      RC          3.2.0       Added ModeViewModel and ProjectViewModel.
 * 10/10/2013      RC          3.2.0       Added StorageViewModel and CommunicationViewModel.
 * 12/20/2013      RC          3.2.1       Added AdcpPredicitionViewModel.
 * 02/12/2014      RC          3.2.3       Added VesselMountViewModel.
 * 07/07/2014      RC          3.4.0       Changed AveragingViewModel to AveragingBaseViewModel and made it a singleton.
 * 08/06/2014      RC          4.0.0       Updated Bootstrapper for Caliburn.Micro 2.0.
 * 09/03/2014      RC          4.0.3       Added SelectPlaybackViewModel.
 * 09/17/2014      RC          4.1.0       Added DvlSetupViewModel.
 * 10/01/2014      RC          4.1.0       Added ViewDataBaseDvlViewModel.
 * 10/02/2014      RC          4.1.0       Added RtiCompassCalViewModel.
 * 11/03/2014      RC          4.1.0       Added WavesViewModel.
 * 10/26/2015      RC          4.3.1       Added DiagnosticsViewModel    
 * 
 */

//using System.Windows;
//using Microsoft.Practices.Prism.UnityExtensions;
//using Microsoft.Practices.Prism.Modularity;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using Caliburn.Micro;
using WPFLocalizeExtension.Engine;
using System.Globalization;

namespace RTI
{
    /// <summary>
    /// Setup the Bootstrapper for the Caliburn.Micro application.
    /// This sets the view model to first load.
    /// 
    /// Nuget:
    /// Caliburn.Micro.Container
    /// 
    /// Setup at:
    /// https://github.com/dbuksbaum/IOCBattle/blob/master/IocBattle.Benchmark/Tests/CaliburnContainer.cs
    /// http://www.jradley.co.uk/1/post/2013/02/24/CaliburnMicro-and-SimpleContainer
    /// 
    /// </summary>
    public class AppBootstrapper : BootstrapperBase
    {
        /// <summary>
        /// Container used to store the view/viewmodels and objects
        /// that will be used globally throughout the application.
        /// This container allows for dependency injections and a 
        /// global way to acess the objects.  It will create and 
        /// destroy the objects based on how they are registered.
        /// </summary>
        private SimpleContainer _container;

        /// <summary>
        /// Initialize the application.  This will then
        /// call Configure.  It will then call OnStartup.
        /// </summary>
        public AppBootstrapper()
        {
            Initialize();
        }

        /// <summary>
        /// Called at startup to display the first view model.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<IShellViewModel>();
        }

        /// <summary>
        /// Configure all singleton and objects that
        /// need to be stored in the container for 
        /// IoC and dependency injection.  This will allow
        /// you to put in the constructor an object that is
        /// stored within the container and it will automagically
        /// be retreived from the container without having to 
        /// pass value to the constructor.
        /// </summary>
        protected override void Configure()
        {

            // Found at https://github.com/gblmarquez/mui-sample-chat/blob/master/src/MuiChat.App/Bootstrapper.cs
            // Allows the CaliburnContentLoader to find the viewmodel based off the view string given
            // Used with ModernUI to navigate between views.
            ViewLocator.NameTransformer.AddRule(
                @"(?<nsbefore>([A-Za-z_]\w*\.)*)?(?<nsvm>ViewModels\.)(?<nsafter>([A-Za-z_]\w*\.)*)(?<basename>[A-Za-z_]\w*)(?<suffix>ViewModel$)",
                @"${nsbefore}Views.${nsafter}${basename}View",
                @"(([A-Za-z_]\w*\.)*)?ViewModels\.([A-Za-z_]\w*\.)*[A-Za-z_]\w*ViewModel$"
                );

            _container = new SimpleContainer();

            base.Configure();

            // Set the localization of the app to the culture of the environment
            //LocalizeDictionary.Instance.Culture = CultureInfo.CurrentCulture;
            //LocalizeDictionary.Instance.SetCurrentThreadCulture = true;
            //LocalizeDictionary.Instance.Culture = CultureInfo.GetCultureInfo("en-US");
            //Application.Current.Dispatcher.BeginInvoke(new System.Action(() => LocalizeDictionary.Instance.Culture = CultureInfo.CurrentCulture);

            ContainerExtensions.Singleton<IWindowManager, WindowManager>(_container);

            ContainerExtensions.Singleton<IEventAggregator, EventAggregator>(_container);

            ContainerExtensions.Singleton<IShellViewModel, ShellViewModel>(_container);

            // Singleton AdcpConnection
            ContainerExtensions.Singleton<AdcpConnection, AdcpConnection>(_container);

            // Singleton ProjectManager
            ContainerExtensions.Singleton<PulseManager, PulseManager>(_container);

            // Singleton MainModernFrameViewModel
            ContainerExtensions.PerRequest<NavBarViewModel, NavBarViewModel>(_container);

            // Register the HomeViewModel
            ContainerExtensions.Singleton<HomeViewModel, HomeViewModel>(_container);

            // Register the TerminalViewModel
            //ContainerExtensions.Singleton<TerminalViewModel, TerminalViewModel>(_container);

            // Register the ViewDataBaseViewModel
            ContainerExtensions.Singleton<ViewDataBaseViewModel, ViewDataBaseViewModel>(_container);

            // Register the PlaybackViewModel
            ContainerExtensions.Singleton<PlaybackViewModel, PlaybackViewModel>(_container);

            // Register the ValidationTestViewModel
            ContainerExtensions.Singleton<ValidationTestViewModel, ValidationTestViewModel>(_container);

            // Register the CompassCalViewModel
            ContainerExtensions.Singleton<CompassCalViewModel, CompassCalViewModel>(_container);

            // Register the ViewDataBaseGraphicalViewModel
            ContainerExtensions.Singleton<ViewDataBaseGraphicalViewModel, ViewDataBaseGraphicalViewModel>(_container);

            // Register the ViewDataBaseTextViewModel
            ContainerExtensions.Singleton<ViewDataBaseTextViewModel, ViewDataBaseTextViewModel>(_container);

            // Register the ViewDataBaseDvlViewModel
            ContainerExtensions.Singleton<ViewDataBaseDvlViewModel, ViewDataBaseDvlViewModel>(_container);

            // Register the ViewDataBaseTextViewModel
            ContainerExtensions.PerRequest<ViewDataTextEnsembleViewModel, ViewDataTextEnsembleViewModel>(_container);

            // Register the ScreenDataBaseViewModel
            ContainerExtensions.Singleton<ScreenDataBaseViewModel, ScreenDataBaseViewModel>(_container);

            // Register the AveragingBaseViewModel
            ContainerExtensions.Singleton<AveragingBaseViewModel, AveragingBaseViewModel>(_container);

            // Register the ValidationTestBaseViewModel
            ContainerExtensions.Singleton<ValidationTestBaseViewModel, ValidationTestBaseViewModel>(_container);

            // Register the DownloadDataViewModel
            ContainerExtensions.Singleton<DownloadDataViewModel, DownloadDataViewModel>(_container);

            // Register the UpdateFirmwareViewModel
            ContainerExtensions.Singleton<UpdateFirmwareViewModel, UpdateFirmwareViewModel>(_container);

            // Register the CompassUtilityViewModel
            ContainerExtensions.Singleton<CompassUtilityViewModel, CompassUtilityViewModel>(_container);

            // Register the CompassUtilityViewModel
            ContainerExtensions.Singleton<ViewDataBaseBackscatterViewModel, ViewDataBaseBackscatterViewModel>(_container);

            // Register the TerminalAdcpViewModel
            //ContainerExtensions.Singleton<TerminalAdcpViewModel, TerminalAdcpViewModel>(_container);

            // Register the TerminalGpsViewModel
            //ContainerExtensions.Singleton<TerminalNavViewModel, TerminalNavViewModel>(_container);

            // Register the AboutViewModel
            ContainerExtensions.Singleton<AboutViewModel, AboutViewModel>(_container);

            // Register the ExportDataViewModel
            ContainerExtensions.PerRequest<ExportDataViewModel, ExportDataViewModel>(_container);

            // Register the ModeViewModel
            ContainerExtensions.PerRequest<ModeViewModel, ModeViewModel>(_container);

            // Register the ProjectViewModel
            ContainerExtensions.PerRequest<ProjectViewModel, ProjectViewModel>(_container);

            // Register the NewProjectViewModel
            ContainerExtensions.PerRequest<NewProjectViewModel, NewProjectViewModel>(_container);

            // Register the SerialNumberGeneratorViewModel
            ContainerExtensions.PerRequest<SerialNumberGeneratorViewModel, SerialNumberGeneratorViewModel>(_container);

            // Register the CommunicationViewModel
            ContainerExtensions.PerRequest<CommunicationViewModel, CommunicationViewModel>(_container);

            // Register the StorageViewModel
            ContainerExtensions.PerRequest<StorageViewModel, StorageViewModel>(_container);

            // Register the LoadProjectsViewModel
            ContainerExtensions.Singleton<LoadProjectsViewModel, LoadProjectsViewModel>(_container);

            // Register the LoadProjectsViewModel
            ContainerExtensions.PerRequest<AdcpConfigurationViewModel, AdcpConfigurationViewModel>(_container);

            // Register the BottomTrackOnViewModel
            ContainerExtensions.PerRequest<BottomTrackOnViewModel, BottomTrackOnViewModel>(_container);

            // Register the BinsViewModel
            ContainerExtensions.PerRequest<BinsViewModel, BinsViewModel>(_container);

            // Register the PingTimingViewModel
            ContainerExtensions.PerRequest<PingTimingViewModel, PingTimingViewModel>(_container);

            // Register the FrequencyViewModel
            ContainerExtensions.PerRequest<FrequencyViewModel, FrequencyViewModel>(_container);

            // Register the BroadbandModeViewModel
            ContainerExtensions.PerRequest<BroadbandModeViewModel, BroadbandModeViewModel>(_container);

            // Register the VesselMountViewModel
            ContainerExtensions.Singleton<VesselMountViewModel, VesselMountViewModel>(_container);

            // Register the SaveAdcpConfigurationViewModel
            ContainerExtensions.PerRequest<SaveAdcpConfigurationViewModel, SaveAdcpConfigurationViewModel>(_container);

            // Register the SalinityViewModel
            ContainerExtensions.PerRequest<SalinityViewModel, SalinityViewModel>(_container);

            // Register the TimeViewModel
            ContainerExtensions.PerRequest<TimeViewModel, TimeViewModel>(_container);

            // Register the EnsembleIntervalViewModel
            ContainerExtensions.PerRequest<EnsembleIntervalViewModel, EnsembleIntervalViewModel>(_container);

            // Register the SimpleCompassCalViewModel
            ContainerExtensions.PerRequest<SimpleCompassCalViewModel, SimpleCompassCalViewModel>(_container);

            // Register the SimpleCompassCalWizardViewModel
            ContainerExtensions.PerRequest<SimpleCompassCalWizardViewModel, SimpleCompassCalWizardViewModel>(_container);

            // Register the ZeroPressureSensorViewModel
            ContainerExtensions.PerRequest<ZeroPressureSensorViewModel, ZeroPressureSensorViewModel>(_container);

            // Register the DeployAdcpViewModel
            ContainerExtensions.PerRequest<DeployAdcpViewModel, DeployAdcpViewModel>(_container);

            // Register the ScanAdcpViewModel
            ContainerExtensions.PerRequest<ScanAdcpViewModel, ScanAdcpViewModel>(_container);

            // Register the AdcpUtilitiesViewModel
            ContainerExtensions.PerRequest<AdcpUtilitiesViewModel, AdcpUtilitiesViewModel>(_container);

            // Register the BurstModeViewModel
            ContainerExtensions.PerRequest<BurstModeViewModel, BurstModeViewModel>(_container);

            // Register the AdcpPredicitionModelViewModel
            ContainerExtensions.PerRequest<AdcpPredictionModelViewModel, AdcpPredictionModelViewModel>(_container);

            // Register the SelectPlaybackViewModel
            ContainerExtensions.PerRequest<SelectPlaybackViewModel, SelectPlaybackViewModel>(_container);

            // Register the DvlSetupViewModel
            ContainerExtensions.PerRequest<DvlSetupViewModel, DvlSetupViewModel>(_container);

            // Register the RtiCompassCalViewModel
            ContainerExtensions.Singleton<RtiCompassCalViewModel, RtiCompassCalViewModel>(_container);

            // Register the DiagnosticsBaseViewModel
            ContainerExtensions.Singleton<DiagnosticsBaseViewModel, DiagnosticsBaseViewModel>(_container);
        }

        /// <summary>
        /// Select the assemblies.
        /// </summary>
        /// <returns>List of all the assemblies.</returns>
        protected override IEnumerable<System.Reflection.Assembly> SelectAssemblies()
        {
            //var a = base.SelectAssemblies();
            //return a;

            var list = new List<System.Reflection.Assembly>();

            // Add the Vault EXE
            list.Add(System.Reflection.Assembly.GetExecutingAssembly());

            // Add the Pulse_Display DLL
            var refs = System.Reflection.Assembly.GetEntryAssembly().GetReferencedAssemblies();
            foreach (var asmName in System.Reflection.Assembly.GetEntryAssembly().GetReferencedAssemblies())
            {
                var asm = System.Reflection.Assembly.Load(asmName);
                if (asm.GetName().ToString().Contains("Pulse_Display"))
                {
                    list.Add(asm);
                }
            }

            return list;
        }

        /// <summary>
        /// Find the instance of the service requested
        /// in the container or return null.
        /// </summary>
        /// <param name="service">Service to find.</param>
        /// <param name="key">Key for the service if it exist.</param>
        /// <returns>Object from the container based off the service and key given.</returns>
        protected override object GetInstance(Type service, string key)
        {
            var inst = _container.GetInstance(service, key);
            return inst;
        }

        /// <summary>
        /// Get all the instances from the container for the 
        /// given service.  This is usually used to get the views
        /// fro the given a given viewmodel.
        /// </summary>
        /// <param name="service">Service to get from the container.</param>
        /// <returns>List of objects from the container for the given service.</returns>
        protected override System.Collections.Generic.IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        /// <summary>
        /// Buildup the container.
        /// </summary>
        /// <param name="instance"></param>
        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }
    }

}
