set CUR_DIR=%cd%
set GenProjectPath=%CUR_DIR%/../../../D2StackRandomizer/

cd %GenProjectPath%
MSBuild.exe RandomStackGenerator.sln /t:Clean,Build /p:Configuration=Release
xcopy "%GenProjectPath%\RandomStackGenerator\bin\Release\*" "%CUR_DIR%/StackGen" /R /I /Y /S
xcopy "%GenProjectPath%\RandomStackGenerator\Resources\*" "%CUR_DIR%/StackGen/Resources" /R /I /Y /S

cd %CUR_DIR%