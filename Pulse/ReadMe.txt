Pulse README

Build
------------------------
 - Release Build
	Builds all projects, installer and documentation.

 - Debug Build
	Builds all Except installer and documentation.


Dependencies
------------------------
log4net
http://apache.cyberuse.com//logging/log4net/binaries/log4net-1.2.11-bin-newkey.zip

System.Data.SQLite
http://system.data.sqlite.org/sqlite-netFx40-setup-bundle-x86-2010-1.0.77.0.exe

 Version Numbers
------------------------
   Set version number in:
	 Pulse\Properties\AssemblyInfo.cs
	 Pulse\Pulse_AppCast.xml
	 Pulse\Installer\Installer.nsi 
	  - ${If} $0 != 'X.XX' (Line 79)
	  - WriteRegStr HKCU "Software\Rowe Technology Inc - Pulse" "Version" (Line 112)
	  - "DisplayVersion"
	  - "VersionMajor"
	  - "VersionMinor"
	  
Checkout from SVN
------------------------
Create the branch in SVN for Pulse.
Go into Repo-Browser and right click on the branch and select Properties.
Change the externals for RTI to the latest branch for RTI.