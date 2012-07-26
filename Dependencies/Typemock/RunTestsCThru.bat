copy *.* ..\..\..\CThru\CThru.Tests\bin\Release\
copy "..\NUnit 2.4.8\bin\nunit.*.dll"	 ..\..\..\CThru\CThru.Tests\bin\Release\

TmockRunner.exe  -register typemock 5FC6-ATAL-5A0E-9D57-0411 -autoReg "..\NUnit 2.4.8\bin\nunit-console.exe" /noshadow ..\..\..\CThru\CThru.Tests\bin\Release\CThru.Tests.dll