.nuget\NuGet.exe restore CashRegister.sln

if "%1" == "Release" (
	msbuild /m /p:VisualStudioVersion=12.0 /p:Configuration=Release
)else (
	msbuild /m /p:VisualStudioVersion=12.0 /p:Configuration=Debug
)
