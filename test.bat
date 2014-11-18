@ECHO OFF
SET PATH=%PATH%;C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE



msbuild CashRegister.Test\CashRegister.Test.csproj
mstest /testcontainer:CashRegister.Test\bin\debug\cashregister.test.dll