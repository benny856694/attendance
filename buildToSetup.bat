::将编译后的文件移动到Setup\InstallFiles文件夹

::删除文件
rmdir /s/q Setup\InstallFiles
::创建文件
md Setup\InstallFiles
md Setup\InstallFiles\detached

::复制可执行文件(相对路径)
xcopy CefSharpLib Setup\InstallFiles /e /d /y /c
xcopy huaanClient\bin\x86\Release Setup\InstallFiles /e /d /y /c /exclude:copy_exclude.txt
xcopy InsuranceBrowserLib\bin\Release Setup\InstallFiles /e /d /y /c /exclude:copy_exclude.txt
xcopy MultiPlayer\bin\Release Setup\InstallFiles /e /d /y /c /exclude:copy_exclude.txt
xcopy WinfromUI\bin\Release Setup\InstallFiles /e /d /y /c /exclude:copy_exclude.txt
xcopy ..\HaSdkDemoCsharp\lib\ Setup\InstallFiles /e /d /y /c /exclude:copy_exclude.txt
xcopy ..\AttendanceWeb\dist Setup\InstallFiles\detached /e /d /y /c /exclude:copy_exclude.txt
xcopy tool Setup\InstallFiles /e /d /y /c