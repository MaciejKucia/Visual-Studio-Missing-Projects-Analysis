@ECHO OFF
SET FRAMEWORK=v4.0.30319
SET MSBUILD14PATH=C:\Program Files (x86)\MSBuild\14.0\bin\

SET COMMAND=C:\Windows\Microsoft.NET\Framework64\%FRAMEWORK%\csc.exe /out:"output\validator.exe" /optimize /nologo ^
  /r:"%MSBUILD14PATH%Microsoft.Build.dll" /r:"%MSBUILD14PATH%Microsoft.Build.Framework.dll" /r:"%MSBUILD14PATH%Microsoft.Build.Utilities.Core.dll"^
  *.cs 

rm "output\validator.exe"
echo %COMMAND%
call %%COMMAND%%
  
copy "%MSBUILD14PATH%Microsoft.Build.dll" .\output\Microsoft.Build.dll /D /Y
copy "%MSBUILD14PATH%Microsoft.Build.dll" .\output\Microsoft.Build.Framework.dll /D /Y
copy "%MSBUILD14PATH%Microsoft.Build.dll" .\output\Microsoft.Build.Utilities.Core.dll /D /Y
  
pause