::���������ļ��ƶ���Setup\InstallFiles�ļ���

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
xcopy .\brandings Setup\InstallFiles\branding /e /y