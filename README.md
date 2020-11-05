# Pulse
.NET WPF Application to work with a Rowe Technologies Inc. ADCP and DVL.  Plan, deploy, and view live data.

RTI and Pulse_Display are sub-projects imported in from other repositories.  This repository contains all the Views and ViewModels specific to Pulse.  Pulse_Display contains all the generic View and ViewModels used in mulitple applications.  RTI contains all the backend logic and code to interface with our ADCP.  This includes decoding the data, serial and ethernet communication and screening data.

All the Views are created in the AppBootstrapper.cs file.

![Pulse Graphical](http://rowetechinc.com/img/swfw/pulse_graphical.png)

![Pulse Planning](http://rowetechinc.com/img/swfw/pulse_planning.png)

![Pulse Tabular](http://rowetechinc.com/img/swfw/pulse_tabular.png)

![Pulse Terminal](http://rowetechinc.com/img/swfw/pulse_terminal.png)

## Pulse/ViewModel and Pulse/View
These folders contain all the View (UI elements) and ViewModels (Logic).
This is where the views are created and interact.


## Models
Contains all the data used in the ViewModels.


## Installer
Contains the install scripts to create the installers using Nullsoft NSIS.


## Styles
Modified styles from the original WPF components.