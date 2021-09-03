; 脚本由 Inno Setup 脚本向导 生成！
; 有关创建 Inno Setup 脚本文件的详细资料请查阅帮助文档！

#define MyAppName "FaceRASystem"
#define MyAppVersion "2.9.0"
#define MyAppPublisher "FaceRASystem"
#define MyAppURL "http://www.huaanvision.com/"
#define MyAppExeName "FaceRASystem.exe"

[Setup]
; 注: AppId的值为单独标识该应用程序。
; 不要为其他安装程序使用相同的AppId值。
; (生成新的GUID，点击 工具|在IDE中生成GUID。)
AppId={{1A3868D7-49C1-4F4D-A0B2-22535C75DF5B}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
DefaultGroupName={#MyAppName}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
VersionInfoVersion={#MyAppVersion}
VersionInfoTextVersion={#MyAppVersion}

;默认安装路径
DefaultDirName={autopf}\{#MyAppName}
DisableProgramGroupPage=yes
DisableDirPage=no
DirExistsWarning=no
OutputDir=Output
OutputBaseFilename={#MyAppName}{#MyAppVersion}
SetupIconFile=InstallFiles\branding\logo.ico
Compression=lzma
SolidCompression=yes


[Languages]
Name: "chinesesimp"; MessagesFile: "compiler:Languages\ChineseSimplified.isl"
Name: "english"; MessagesFile: "compiler:Languages\English.isl"
Name: "japanese"; MessagesFile: "compiler:Languages\Japanese.isl"
Name: "french"; MessagesFile: "compiler:Languages\French.isl"


[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Files]
Source: "InstallFiles\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "InstallFiles\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "InstallFiles\branding\*"; DestDir: "{app}\branding";
; 注意: 不要在任何共享系统文件上使用“Flags: ignoreversion”

;动态连接库  与进程相关(关闭进程等)
Source: "Dependencies\IsTask.dll"; Flags: dontcopy noencryption
Source: "Dependencies\IsTask.dll"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\branding\logo.ico"
Name: "{commonstartmenu}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\branding\logo.ico"
;Name: "{group}\{cm:ProgramOnTheWeb,{#MyAppName}}"; Filename: "{#MyAppURL}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\branding\logo.ico"

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[code]

//1  一些API                                                                     
//1.1  查找安装产品guid是否存在 返回5表示存在
#IFDEF UNICODE
  #DEFINE AW "W"
#ELSE
  #DEFINE AW "A"
#ENDIF
function MsiQueryProductState(ProductCode: string): integer;
  external 'MsiQueryProductState{#AW}@msi.dll stdcall';
function MsiConfigureProduct(ProductCode: string;
  iInstallLevel: integer; eInstallState: integer): integer;
  external 'MsiConfigureProduct{#AW}@msi.dll stdcall';

//1.2  安装前判断是否有进程正在运行
function RunTask(FileName: string; bFullpath: Boolean): Boolean;
  external 'RunTask@files:ISTask.dll stdcall delayload';
function KillTask(ExeFileName: string): Integer;
  external 'KillTask@files:ISTask.dll stdcall delayload';

//1.3  卸载前判断是否有进程正在运行
function RunTaskU(FileName: string; bFullpath: Boolean): Boolean;
  external 'RunTask@{app}/ISTask.dll stdcall delayload uninstallonly';
function KillTaskU(ExeFileName: string): Integer;
  external 'KillTask@{app}/ISTask.dll stdcall delayload uninstallonly';

//1.4  退出
procedure ExitProcess(exitCode:integer);
  external 'ExitProcess@kernel32.dll stdcall';
 //4 删除快捷方式
procedure DeleteShortcut();
begin
   DelTree(ExpandConstant('{group}'), True, True, True);

   DelTree(ExpandConstant('{userstartup}\FaceRASystem'), True, True, True);
   DelTree(ExpandConstant('{commonstartup}\FaceRASystem'), True, True, True);

   DeleteFile(ExpandConstant('{userstartmenu}\FaceRASystem.lnk'));
   DeleteFile(ExpandConstant('{commonstartmenu}\FaceRASystem.lnk'));

   DeleteFile(ExpandConstant('{userdesktop}\FaceRASystem.lnk'));
   DeleteFile(ExpandConstant('{commondesktop}\FaceRASystem.lnk'));
end;

//5.1 关闭进程
procedure CloseProcess();
begin   
    KillTask('huaanClient.exe');   
    KillTask('FaceRASystem.exe');
    KillTask('CefSharp.BrowserSubprocess.exe');
end;
//6  下一步按钮点击事件
function NextButtonClick(CurPage: Integer): Boolean;
begin
    Result := true;
    //路径选择界面
    if CurPage = wpSelectDir then
    begin
        if Pos('desktop',LowerCase(WizardForm.DirEdit.Text))>0 then
        begin
            MsgBox('请勿要安装到桌面, 建议更改安装路径',mbError,MB_OK);
            Result := false
            exit;
        end;
        if Pos(ExpandConstant('{#MyAppName}'),WizardForm.DirEdit.Text)=0 then
        begin
            WizardForm.DirEdit.Text:= WizardForm.DirEdit.Text+'\'+ExpandConstant('{#MyAppName}');
        end;
    end;

    if CurPage = wpReady then 
    begin
       //todo
    end;
  end;
//7  安装时
procedure CurStepChanged(CurStep: TSetupStep);
var 
    Isstr :string;
    ResultCode :Integer;
begin
    //安装前调用
    if CurStep=ssInstall then       
    begin
        
        //InstallVC();
        
        CloseProcess();
        DeleteShortcut();

        if FileExists(ExpandConstant('{app}\libcef.dll')) then
        begin
               DelTree(ExpandConstant('{app}'), True, True, True);
        end
    end;
end;