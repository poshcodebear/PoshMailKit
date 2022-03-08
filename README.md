# PSMailKit

Drop-in replacement for Send-MailMessage using MailKit

## Installing NuGet packages

You need two packages to build this (Visual Studio should restore them without isuse): System.Management.Automatios.dll (version
10.0.10586; deprecated but needed to target Windows PowerShell rather than just modern PowerShell), and MailKit.

```powershell
Install-Package System.Management.Automation.dll -Version 10.0.10586
Install-Package MailKit
```

Note: if there is a way to get this to work with Windows PowerShell 5.1 using the modern System.Management.Automation, I will switch
to that; as this is my first binary PowerShell module, however, I'm going to hold off on trying to make a .NET 6 project work in
.NET Framework based versions of Windows PowerShell.
