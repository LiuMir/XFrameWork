@echo off

set ProtocPath=%cd%\protoc.exe
set ProtoSourcePath=%cd%\Protos
set CsharpSourceOutputPath=%cd%\Csharp

:: 删除之前所有的cs文件
del /f /q %CsharpSourceOutputPath%\*.*


for /r %%i in (*.proto) do (
	echo %%~nxi 转换完成
	%ProtocPath% -I=%ProtoSourcePath% --csharp_out=%CsharpSourceOutputPath% %%i
	)

echo.
echo .proto文件转.cs文件结束
echo.
pause