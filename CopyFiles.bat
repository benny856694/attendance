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
ECHO 4. �ƿ�ͨ
ECHO 5. Kotak
ECHO 6. ����
set choice=
set /p choice=����������ѡ��Ʒ��.
if not '%choice%'=='' set choice=%choice:~0,1%
if '%choice%'=='0' goto default
if '%choice%'=='1' goto mox
if '%choice%'=='2' goto SmartSchoolNetwork
if '%choice%'=='3' goto Deepleeds
if '%choice%'=='4' goto YunKaTong
if '%choice%'=='5' goto kotak
if '%choice%'=='6' goto qigong
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
:YunKaTong
set brand=�ƿ�ͨ
goto start
:kotak
set brand=kotak
goto start
:qigong
set brand=����
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

ECHO copy  1
xcopy huaanClient\bin\x86\Release Setup\InstallFiles /e /y /exclude:exclude.txt
ECHO copy 2
xcopy InsuranceBrowserLib\bin\Release Setup\InstallFiles /e /y /exclude:exclude.txt
ECHO copy 3
xcopy MultiPlayer\bin\Release Setup\InstallFiles /e /y /exclude:exclude.txt
ECHO copy 4
xcopy WinFormUI\bin\Release Setup\InstallFiles /e /y /exclude:exclude.txt
ECHO copy 5
xcopy ..\HaSdkDemoCsharp\lib\ Setup\InstallFiles /e /y /exclude:exclude.txt
ECHO copy 6
xcopy ..\AttendanceWeb\dist Setup\InstallFiles\detached /e /y /exclude:exclude.txt
ECHO copy 7
xcopy tool Setup\InstallFiles /e /y
ECHO copy 8
xcopy .\brandings\%brand%\ Setup\InstallFiles\branding /e /y