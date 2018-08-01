;BTIC Pulse NSIS Installer
;Install Pulse
;Written by Rico Castelo

; HISTORY
; -----------------------------------------------------------------
; Date            Initials    Version    Comments
; -----------------------------------------------------------------
; 02/17/2017      RC          4.5.1       Initial creation.
;

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
; Include Modern UI
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

  !include "MUI2.nsh"

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
; General
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
	;Name and file
	Name "BTIC - Pulse"
	OutFile "BTIC.Pulse.Installer.v.4.12.0.Update.exe"

	;Default installation folder
	InstallDir "$PROGRAMFILES\BTIC\Pulse"
  
	;Get installation folder from registry if available
	InstallDirRegKey HKCU "Software\BTIC - Pulse" ""

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

	StrCpy $VERSION_NUM "4.12.0" 
	StrCpy $VERSION_MAJOR "4"
	StrCpy $VERSION_MINOR "12"

	SetOutPath $INSTDIR

	; Add Files
	DetailPrint "Installing Pulse."
	CreateDirectory "$INSTDIR\x64"
	CreateDirectory "$INSTDIR\x86"
	File "..\..\bin\Release\Pulse.exe"
	File "..\..\bin\Release\Pulse_Display.dll"
	File "..\..\bin\Release\RTI.dll"
	File "/oname=x64\SQLite.Interop.dll" "..\..\..\packages\System.Data.SQLite.Core.1.0.99.0\build\net451\x64\SQLite.Interop.dll"
	File "/oname=x86\SQLite.Interop.dll" "..\..\..\packages\System.Data.SQLite.Core.1.0.99.0\build\net451\x86\SQLite.Interop.dll"
	File "..\..\bin\Release\Licenses.txt"
	File "Copyright.txt"
	File "EndUserRights.txt"
	File "..\..\..\User Guide\BTIC - Pulse User Guide.pdf"
	
	; Create shortcut in start menu
	CreateDirectory "$SMPROGRAMS\Pulse"
	CreateShortCut "$SMPROGRAMS\Pulse\Pulse.lnk" "$INSTDIR\Pulse.exe"
	CreateShortCut "$SMPROGRAMS\Pulse\Uninstall.lnk" "$INSTDIR\uninstall.exe"
	CreateShortCut "$SMPROGRAMS\Pulse\Pulse User Guide.lnk" "$INSTDIR\RTI - Pulse User Guide.pdf"
	
	; Store installation folder
	WriteRegStr HKCU "Software\BTIC - Pulse" "" $INSTDIR
	WriteRegStr HKCU "Software\BTIC - Pulse" "Version" "$VERSION_MAJOR.$VERSION_MINOR"

	; Create uninstaller
	WriteUninstaller "$INSTDIR\Uninstall.exe"
	
	; Add to Add/Remove
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Pulse" \
				 "DisplayName" "BTIC - Pulse"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Pulse" \
				 "UninstallString" "$\"$INSTDIR\uninstall.exe$\""

	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Pulse" \
				 "Publisher" "BTIC"

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
		MessageBox MB_OK "BTIC Pulse requires Microsoft .NET Framework 4.5.1."
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
	Delete "$INSTDIR\BTIC - Pulse User Guide.pdf"

	Delete "$INSTDIR\Uninstall.exe"

	; Remove the install directory
	RMDir "$INSTDIR\x64"
	RMDir "$INSTDIR\x86"
	RMDir "$INSTDIR"
	RMDir "$PROGRAMFILES\BTIC\Pulse"
	RMDir "$PROGRAMFILES\BTIC"

	; Remove the program data
	Delete "C:\ProgramData\BTIC\Pulse\PulseDb.db" 
	Delete "C:\ProgramData\BTIC\Pulse\PulseErrorLog.log" 
	RMDir "C:\ProgramData\BTIC\Pulse"
	RMDir "C:\ProgramData\BTIC"

	; Remove registry key for SQLite
	DeleteRegKey HKCU "Software\System.Data.SQLite"

	; Remove registery key
	DeleteRegKey /ifempty HKCU "Software\BTIC - Pulse"
	
	; Remove Uninstall 
	DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Pulse"

	; Remove menu short cuts
	Delete "$SMPROGRAMS\Pulse\Pulse.lnk"
	Delete "$SMPROGRAMS\Pulse\Uninstall.lnk"
	Delete "$SMPROGRAMS\Pulse\Pulse User Guide.lnk"
	RMDir "$SMPROGRAMS\Pulse"

SectionEnd