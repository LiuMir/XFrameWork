@echo off

set ProtocPath=%cd%\protoc.exe
set ProtoSourcePath=%cd%\Protos
set CsharpSourceOutputPath=%cd%\Csharp

:: ɾ��֮ǰ���е�cs�ļ�
del /f /q %CsharpSourceOutputPath%\*.*


for /r %%i in (*.proto) do (
	echo %%~nxi ת�����
	%ProtocPath% -I=%ProtoSourcePath% --csharp_out=%CsharpSourceOutputPath% %%i
	)

echo.
echo .proto�ļ�ת.cs�ļ�����
echo.
pause