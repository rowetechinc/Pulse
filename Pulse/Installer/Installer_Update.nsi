;Pulse NSIS Installer
;Install Pulse
;Written by Rico Castelo

; HISTORY
; -----------------------------------------------------------------
; Date            Initials    Version    Comments
; -----------------------------------------------------------------
; 02/09/2012      RC          2.01       Changed installed to use System.Data.SQLite v1.0.79.0.
;                                         Updated uninstall section.
;                                         Added .Net 4.0 Full and used the non-web installer.
; 02/10/2012      RC          2.02       Changed version numbers.
; 02/13/2012      RC          2.03       Changed version number.
; 02/24/2012      RC          2.04       Changed version number.
; 03/02/2012      RC          2.04       Changed the license file.
; 03/05/2012      RC          2.05       Changed version number.
; 03/06/2012      RC          2.06       Changed version number.
; 03/20/2012      RC          2.07       Changed version number.
; 04/02/2012      RC          2.08       Changed version number.
; 04/12/2012      RC          2.09       Changed version number.
; 04/23/2012      RC          2.10       Changed version number.
; 04/30/2012      RC          2.10       Removed the DotSpatial dll.
; 04/30/2012      RC          2.11       Changed version number.
; 06/14/2012      RC          2.11       Added Backlight dll.
; 06/18/2012      RC          2.12       Changed version number.
; 07/24/2012      RC          2.13       Changed version number.
; 08/21/2012      RC          2.13       Added Microsoft.Expression.Drawing.dll
; 08/28/2012      RC          2.14       Changed version number.
; 08/29/2012      RC          2.15       Changed version number.
; 09/10/2012      RC          2.15       Added Newtonsoft.Json.dll.
; 09/11/2012      RC          2.15       Added the AutoUpdater.NET.dll.
; 05/07/2013      RC          3.0.0      Removed unneed DLLs for new version.
; 05/23/2013      RC          3.0.0      Added WPF Localization Extension dll and MahApps.Metro dll.  Updated Extended WPF toolkit.  Removed ModernUI.
; 06/14/2013      RC          3.0.1      Updated Extended WPF Toolkit to 2.0.0.
; 07/08/2013      RC          3.0.2      Updated ReactiveUI to 4.5.0.
; 08/26/2013      RC          3.0.8      Updated AutoUpdater.NET to 1.2.
; 08/29/2013      RC          3.0.9      Changed version number.
; 09/06/2013      RC          3.0.9      Changed WPF Localize Extension to 2.1.4.
; 09/11/2013      RC          3.0.9      Added Licenses.txt and EndUserRights.txt.
; 10/03/2013      RC          3.2.0      Changed version number.
; 11/26/2013      RC          3.2.0      Updated MahApps.Metro to 0.10.1.1.
; 12/17/2013      RC          3.2.1      Changed version number.
; 12/30/2013      RC          3.2.2      Changed version number.
; 01/02/2014      RC          3.2.3      Changed version number.
; 01/14/2014      RC          3.2.3      Added WriteableBitmapEx.Wpf.dll.
; 02/21/2014      RC          3.2.4      Changed version number.
; 05/28/2014      RC          3.3.0      Changed version number.
; 06/10/2014      RC          3.3.1      Changed version number.
; 06/24/2014      RC          3.4.0      Changed version number.
; 08/15/2014      RC          4.0.0      Changed version and updated everthing to .NET 4.5.
; 08/19/2014      RC          4.0.1      Changed version number.    
; 08/22/2014      RC          4.0.2      Changed version number.
; 08/25/2014      RC          4.0.3      Changed version number.
; 08/25/2014      RC          4.0.3      Created an Update and Full.  Full will include the .NET installer.
; 09/17/2014      RC          4.1.0      Changed version number.
; 03/04/2015      RC          4.1.1      Update version number and add Pulse_Display.dll.
; 03/16/2014      RC          4.1.2      Changed version number.
; 03/25/2015      RC          4.1.3      Changed version number.
; 04/27/2015      RC          4.1.4      Changed version number.
; 06/19/2015      RC          4.1.5      Changed version number.
; 08/25/2015      RC          4.2        Changed version number.
; 10/06/2015	  RC          4.3        Changed version number.
; 11/11/2015      RC          4.3.1      Fixed DLL for SQLite.
;

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
; Include Modern UI
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

  !include "MUI2.nsh"

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
; General
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
	;Name and file
	Name "Rowe Technology Inc. - Pulse"
	OutFile "Pulse.Installer.v.4.7.0.Update.exe"

	;Default installation folder
	InstallDir "$PROGRAMFILES\Rowe Technology Inc\Pulse"
  
	;Get installation folder from registry if available
	InstallDirRegKey HKCU "Software\Rowe Technology Inc - Pulse" ""

	;Request application privileges for Windows Vista
	RequestExecutionLevel admin

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
; Variables
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

Var /GLOBAL VERSION_NUM
Var /GLOBAL VERSION_MAJOR
Var /GLOBAL VERSION_MINOR


;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
; Interface Settings
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
	!define MUI_ABORTWARNING
	!define MUI_ICON "${NSISDIR}\Contrib\Graphics\Icons\modern-install.ico"

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
; Pages
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
	!insertmacro MUI_PAGE_LICENSE "License.txt"
	!insertmacro MUI_PAGE_COMPONENTS
	!insertmacro MUI_PAGE_DIRECTORY
	!insertmacro MUI_PAGE_INSTFILES
  
	!insertmacro MUI_UNPAGE_CONFIRM
	!insertmacro MUI_UNPAGE_INSTFILES
  
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
; Languages
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
	!insertmacro MUI_LANGUAGE "English"

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
; Installer Sections
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
; Install Main Application and all DLL
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
Section "Core" SecCore

	StrCpy $VERSION_NUM "4.7.0" 
	StrCpy $VERSION_MAJOR "4"
	StrCpy $VERSION_MINOR "7"

	SetOutPath $INSTDIR

	; Remove old program data if it exist
	; The database was modified so the database needs to be removed.
	ReadRegDWORD $0 HKCU "Software\Rowe Technology Inc - Pulse" Version
	${If} $0 < '3.0.9'
		Delete "C:\ProgramData\RTI\Pulse\PulseDb.db" 
		Delete "C:\ProgramData\RTI\Pulse\PulseErrorLog.log" 
		RMDir "C:\ProgramData\RTI\Pulse"
		RMDir "C:\ProgramData\RTI"
	${EndIf}

	; Remove all the old folders if an older version exist
	; 4.0 changes alot of DLL settings
	${If} $0 < '4.0.0.0'
		DetailPrint "Removing previous installation."
		ExecWait '$INSTDIR\Uninstall.exe /S _?=$INSTDIR'
		Delete "$INSTDIR\*.*"
		RMDir $INSTDIR
	${EndIf}

	; Add Files
	DetailPrint "Installing Pulse."
	CreateDirectory "$INSTDIR\x64"
	CreateDirectory "$INSTDIR\x86"
	File "..\bin\Release\Pulse.exe"
	File "..\bin\Release\Pulse_Display.dll"
	File "..\bin\Release\RTI.dll"
	File "/oname=x64\SQLite.Interop.dll" "..\..\packages\System.Data.SQLite.Core.1.0.99.0\build\net451\x64\SQLite.Interop.dll"
	File "/oname=x86\SQLite.Interop.dll" "..\..\packages\System.Data.SQLite.Core.1.0.99.0\build\net451\x86\SQLite.Interop.dll"
	File "..\bin\Release\Licenses.txt"
	File "..\bin\Release\Copyright.txt"
	File "..\bin\Release\EndUserRights.txt"
	File "..\..\User Guide\RTI - Pulse User Guide.pdf"
	
	; Create shortcut in start menu
	CreateDirectory "$SMPROGRAMS\Pulse"
	CreateShortCut "$SMPROGRAMS\Pulse\Pulse.lnk" "$INSTDIR\Pulse.exe"
	CreateShortCut "$SMPROGRAMS\Pulse\Uninstall.lnk" "$INSTDIR\uninstall.exe"
	CreateShortCut "$SMPROGRAMS\Pulse\Pulse User Guide.lnk" "$INSTDIR\RTI - Pulse User Guide.pdf"
	
	; Store installation folder
	WriteRegStr HKCU "Software\Rowe Technology Inc - Pulse" "" $INSTDIR
	WriteRegStr HKCU "Software\Rowe Technology Inc - Pulse" "Version" "$VERSION_MAJOR.$VERSION_MINOR"

	; Create uninstaller
	WriteUninstaller "$INSTDIR\Uninstall.exe"
	
	; Add to Add/Remove
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Pulse" \
				 "DisplayName" "Rowe Technology Inc - Pulse"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Pulse" \
				 "UninstallString" "$\"$INSTDIR\uninstall.exe$\""

	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Pulse" \
				 "Publisher" "Rowe Technology Inc."

	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Pulse" \
				 "DisplayVersion" $VERSION_NUM

	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Pulse" \
				 "VersionMajor" $VERSION_MAJOR

	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Pulse" \
				 "VersionMinor" $VERSION_MINOR

SectionEnd

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
; .Net 4.5.1 Full
; .Net 4.5.1 Installer instructions: http://msdn.microsoft.com/library/ee942965%28v=VS.100%29.aspx
; .Net 4.5.1 Installer commandline options: http://msdn.microsoft.com/library/ee942965%28v=VS.100%29.aspx#command_line_options
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
Section ".Net 4.5.1 Full" SecDotNet45Full

	SetOutPath "$INSTDIR"
 
    ; Magic numbers from http://msdn.microsoft.com/en-us/library/ee942965.aspx
    ClearErrors
    ReadRegDWORD $0 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full" "Release"

    IfErrors NotDetected

    ${If} $0 >= 378389
        DetailPrint "Check for Microsoft .NET Framework 4.5.1 ($0)"		
    ${Else}
    NotDetected:
        DetailPrint "Required Microsoft .NET Framework 4.5.1"
		MessageBox MB_OK "RTI Pulse requires Microsoft .NET Framework 4.5.1.$\nDownload the full version of Pulse which includes all the required files.$\nhttp://www.rowetechinc.com/pulse/Pulse.Installer.v.$VERSION_NUM.Full.exe"
		ExecShell open "http://www.rowetechinc.com/pulse/Pulse.Installer.v.$VERSION_NUM.Full.exe"
    ${EndIf}

SectionEnd

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
; Descriptions
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
; Language strings
LangString DESC_SecCore ${LANG_ENGLISH} "Core files."
LangString DESC_SecDotNet ${LANG_ENGLISH} "Check .Net 4.5.1 Full."

; Assign language strings to sections
!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
	!insertmacro MUI_DESCRIPTION_TEXT ${SecCore} $(DESC_SecCore)
	!insertmacro MUI_DESCRIPTION_TEXT ${SecDotNet45Full} $(DESC_SecDotNet)
!insertmacro MUI_FUNCTION_DESCRIPTION_END

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
; Uninstaller Section
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
Section "Uninstall"

	Delete "$INSTDIR\Pulse.exe"
	Delete "$INSTDIR\Pulse_Display.dll"
	Delete "$INSTDIR\RTI.dll"
	Delete "$INSTDIR\x64\SQLite.Interop.dll"
	Delete "$INSTDIR\x86\SQLite.Interop.dll"

	; Old installations
	Delete "$INSTDIR\log4net.dll"
	Delete "$INSTDIR\OxyPlot.dll"
	Delete "$INSTDIR\OxyPlot.Wpf.dll"
	Delete "$INSTDIR\System.Data.SQLite.dll"
	;Delete "$INSTDIR\System.Data.SQLite.Linq.dll"
	Delete "$INSTDIR\Xceed.Wpf.Toolkit.dll"
	Delete "$INSTDIR\HelixToolkit.Wpf.dll"
	Delete "$INSTDIR\Microsoft.Expression.Drawing.dll"
	Delete "$INSTDIR\Newtonsoft.Json.dll"
	Delete "$INSTDIR\AutoUpdater.NET.dll"
	Delete "$INSTDIR\ReactiveUI.Blend.dll"
	Delete "$INSTDIR\ReactiveUI.dll"
	Delete "$INSTDIR\ReactiveUI.Routing.dll"
	Delete "$INSTDIR\ReactiveUI.Xaml.dll"
	Delete "$INSTDIR\System.Reactive.Core.dll"
	Delete "$INSTDIR\System.Reactive.Interfaces.dll"
	Delete "$INSTDIR\System.Reactive.Linq.dll"
	Delete "$INSTDIR\System.Reactive.PlatformServices.dll"
	Delete "$INSTDIR\System.Reactive.Windows.Threading.dll"
	Delete "$INSTDIR\System.Windows.Interactivity.dll"
	Delete "$INSTDIR\Caliburn.Micro.dll"
	Delete "$INSTDIR\WPFLocalizeExtension.dll"
	Delete "$INSTDIR\XAMLMarkupExtensions.dll"
	Delete "$INSTDIR\MahApps.Metro.dll"
	Delete "$INSTDIR\WriteableBitmapEx.Wpf.dll"
    Delete "$INSTDIR\Licenses.txt"
	Delete "$INSTDIR\Copyright.txt"
	Delete "$INSTDIR\EndUserRights.txt"
	Delete "$INSTDIR\RTI - Pulse User Guide.pdf"

	Delete "$INSTDIR\Uninstall.exe"

	; Remove the install directory
	RMDir "$INSTDIR\x64"
	RMDir "$INSTDIR\x86"
	RMDir "$INSTDIR"
	RMDir "$PROGRAMFILES\Rowe Technology Inc\Pulse"
	RMDir "$PROGRAMFILES\Rowe Technology Inc"

	; Remove the program data
	Delete "C:\ProgramData\RTI\Pulse\PulseDb.db" 
	Delete "C:\ProgramData\RTI\Pulse\PulseErrorLog.log" 
	RMDir "C:\ProgramData\RTI\Pulse"
	RMDir "C:\ProgramData\RTI"

	; Remove registry key for SQLite
	DeleteRegKey HKCU "Software\System.Data.SQLite"

	; Remove registery key
	DeleteRegKey /ifempty HKCU "Software\Rowe Technology Inc - Pulse"
	
	; Remove Uninstall 
	DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Pulse"

	; Remove menu short cuts
	Delete "$SMPROGRAMS\Pulse\Pulse.lnk"
	Delete "$SMPROGRAMS\Pulse\Uninstall.lnk"
	Delete "$SMPROGRAMS\Pulse\Pulse User Guide.lnk"
	RMDir "$SMPROGRAMS\Pulse"

SectionEnd