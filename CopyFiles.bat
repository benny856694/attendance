::���������ļ��ƶ���Setup\InstallFiles�ļ���
@ECHO off
cls
:menu
ECHO.
ECHO ��ѡ��Ʒ��
ECHO 0. ȱʡ(Tommi)
ECHO 1. Mox
ECHO 2. SmartSchoolNetwork
ECHO 3. Deepleeds
set choice=
set /p choice=����������ѡ��Ʒ��.
if not '%choice%'=='' set choice=%choice:~0,1%
if '%choice%'=='0' goto default
if '%choice%'=='1' goto mox
if '%choice%'=='2' goto SmartSchoolNetwork
if '%choice%'=='3' goto Deepleeds
ECHO "%choice%" ��Ч, ����������
ECHO.
goto menu
:default
set brand=default
goto start
:mox
set brand=mox
goto start
:SmartSchoolNetwork
set brand=smartschoolnetwork
goto start
:Deepleeds
set brand=deepleeds
goto start

:start
ECHO ��ѡ���� %brand%
pause ������س�

::ɾ���ļ�
rmdir /s/q Setup\InstallFiles
::�����ļ�
md Setup\InstallFiles
md Setup\InstallFiles\detached
md Setup\InstallFiles\branding

::���ƿ�ִ���ļ�(���·��)
xcopy CefSharpLib Setup\InstallFiles /e /y 
xcopy huaanClient\bin\x86\Release Setup\InstallFiles /e /y /exclude:copy_exclude.txt
xcopy InsuranceBrowserLib\bin\Release Setup\InstallFiles /e /y /exclude:copy_exclude.txt
xcopy MultiPlayer\bin\Release Setup\InstallFiles /e /y /exclude:copy_exclude.txt
xcopy WinfromUI\bin\Release Setup\InstallFiles /e /y /exclude:copy_exclude.txt
xcopy ..\HaSdkDemoCsharp\lib\ Setup\InstallFiles /e /y /exclude:copy_exclude.txt
xcopy ..\AttendanceWeb\dist Setup\InstallFiles\detached /e /y /exclude:copy_exclude.txt
xcopy tool Setup\InstallFiles /e /y
xcopy .\brandings\%brand%\ Setup\InstallFiles\branding /e /y