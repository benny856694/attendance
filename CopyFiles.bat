::将编译后的文件移动到Setup\InstallFiles文件夹
@ECHO off
cls
:menu
ECHO.
ECHO 请选择品牌
ECHO 0. 缺省(Tommi)
ECHO 1. Mox
ECHO 2. SmartSchoolNetwork
ECHO 3. Deepleeds
set choice=
set /p choice=请输入数字选择品牌.
if not '%choice%'=='' set choice=%choice:~0,1%
if '%choice%'=='0' goto default
if '%choice%'=='1' goto mox
if '%choice%'=='2' goto SmartSchoolNetwork
if '%choice%'=='3' goto Deepleeds
ECHO "%choice%" 无效, 请重新输入
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
ECHO 你选择了 %brand%
pause

::删除文件
rmdir /s/q Setup\InstallFiles
::创建文件
md Setup\InstallFiles
md Setup\InstallFiles\detached
md Setup\InstallFiles\branding

::复制可执行文件(相对路径)
xcopy CefSharpLib Setup\InstallFiles /e /y 
xcopy huaanClient\bin\x86\Release Setup\InstallFiles /e /y /exclude:copy_exclude.txt
xcopy InsuranceBrowserLib\bin\Release Setup\InstallFiles /e /y /exclude:copy_exclude.txt
xcopy MultiPlayer\bin\Release Setup\InstallFiles /e /y /exclude:copy_exclude.txt
xcopy WinfromUI\bin\Release Setup\InstallFiles /e /y /exclude:copy_exclude.txt
xcopy ..\HaSdkDemoCsharp\lib\ Setup\InstallFiles /e /y /exclude:copy_exclude.txt
xcopy ..\AttendanceWeb\dist Setup\InstallFiles\detached /e /y /exclude:copy_exclude.txt
xcopy tool Setup\InstallFiles /e /y
xcopy .\brandings\%brand%\ Setup\InstallFiles\branding /e /y