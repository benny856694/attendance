::���������ļ��ƶ���Setup\InstallFiles�ļ���

::ɾ���ļ�
rmdir /s/q Setup\InstallFiles
::�����ļ�
md Setup\InstallFiles
md Setup\InstallFiles\detached

::���ƿ�ִ���ļ�(���·��)
xcopy CefSharpLib Setup\InstallFiles /e /d /y /c
xcopy huaanClient\bin\x86\Release Setup\InstallFiles /e /d /y /c /exclude:copy_exclude.txt
xcopy InsuranceBrowserLib\bin\Release Setup\InstallFiles /e /d /y /c /exclude:copy_exclude.txt
xcopy MultiPlayer\bin\Release Setup\InstallFiles /e /d /y /c /exclude:copy_exclude.txt
xcopy WinfromUI\bin\Release Setup\InstallFiles /e /d /y /c /exclude:copy_exclude.txt
xcopy ..\HaSdkDemoCsharp\lib\ Setup\InstallFiles /e /d /y /c /exclude:copy_exclude.txt
xcopy ..\AttendanceWeb\dist Setup\InstallFiles\detached /e /d /y /c /exclude:copy_exclude.txt
xcopy tool Setup\InstallFiles /e /d /y /c