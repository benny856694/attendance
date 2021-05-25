call ../build.bat
devenv huaanClient.sln /clean "release|x86"
devenv huaanClient.sln /rebuild "release|x86"
devenv ../HaSdkDemoCsharp/SDKClientSharp.sln /clean "release|x86"
devenv ../HaSdkDemoCsharp/SDKClientSharp.sln /rebuild "release|x86"
call buildToSetup
//cd Setup
//"C:\Program Files (x86)\Inno Setup 6\iscc.exe" package.iss
