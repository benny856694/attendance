; �ű��� Inno Setup �ű��� ���ɣ�
; �йش��� Inno Setup �ű��ļ�����ϸ��������İ����ĵ���

#define MyAppName "FaceRASystem"
#define MyAppVersion "2.7.1.0"
#define MyAppPublisher "FaceRASystem"
#define MyAppURL "http://www.huaanvision.com/"
#define MyAppExeName "FaceRASystem.exe"

[Setup]
; ע: AppId��ֵΪ������ʶ��Ӧ�ó���
; ��ҪΪ������װ����ʹ����ͬ��AppIdֵ��
; (�����µ�GUID����� ����|��IDE������GUID��)
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

;Ĭ�ϰ�װ·��
DefaultDirName=D:\Program Files\{#MyAppName}
DisableProgramGroupPage=yes
DirExistsWarning=no
OutputDir=Output
OutputBaseFilename={#MyAppName}{#MyAppVersion}
SetupIconFile=branding\logo.ico
WizardSmallImageFile=branding\logo.bmp
Compression=lzma
SolidCompression=yes

[Languages]
Name: "chinesesimp"; MessagesFile: "compiler:Languages\ChineseSimplified.isl"
Name: "english"; MessagesFile: "compiler:Languages\English.isl"
Name: "japanese"; MessagesFile: "compiler:Languages\Japanese.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Files]
Source: "InstallFiles\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "InstallFiles\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "branding\*"; DestDir: "{app}\branding";
; ע��: ��Ҫ���κι���ϵͳ�ļ���ʹ�á�Flags: ignoreversion��

;��̬���ӿ�  ��������(�رս��̵�)
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

//1  һЩAPI                                                                     
//1.1  ���Ұ�װ��Ʒguid�Ƿ���� ����5��ʾ����
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

//1.2  ��װǰ�ж��Ƿ��н�����������
function RunTask(FileName: string; bFullpath: Boolean): Boolean;
  external 'RunTask@files:ISTask.dll stdcall delayload';
function KillTask(ExeFileName: string): Integer;
  external 'KillTask@files:ISTask.dll stdcall delayload';

//1.3  ж��ǰ�ж��Ƿ��н�����������
function RunTaskU(FileName: string; bFullpath: Boolean): Boolean;
  external 'RunTask@{app}/ISTask.dll stdcall delayload uninstallonly';
function KillTaskU(ExeFileName: string): Integer;
  external 'KillTask@{app}/ISTask.dll stdcall delayload uninstallonly';

//1.4  �˳�
procedure ExitProcess(exitCode:integer);
  external 'ExitProcess@kernel32.dll stdcall';
 //4 ɾ����ݷ�ʽ
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

//5.1 �رս���
procedure CloseProcess();
begin   
    KillTask('huaanClient.exe');   
    KillTask('FaceRASystem.exe');
    KillTask('CefSharp.BrowserSubprocess.exe');
end;
//6  ��һ����ť����¼�
function NextButtonClick(CurPage: Integer): Boolean;
begin
    Result := true;
    //·��ѡ�����
    if CurPage = wpSelectDir then
    begin
        if Pos('desktop',LowerCase(WizardForm.DirEdit.Text))>0 then
        begin
            MsgBox('����Ҫ��װ������, ������İ�װ·��',mbError,MB_OK);
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
//7  ��װʱ
procedure CurStepChanged(CurStep: TSetupStep);
var 
    Isstr :string;
    ResultCode :Integer;
begin
    //��װǰ����
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