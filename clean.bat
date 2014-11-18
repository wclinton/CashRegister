msbuild /t:clean /p:VisualStudioVersion=12.0
del /f /q /s packages\*.*
rd packages /s /q

rd CashRegister\bin /s /q

rd TestResults /s /q