# PoshMailKit

A PowerShell front end to the MailKit project including a near-drop-in replacement for Send-MailMessage

## The deprecation of `Send-MailMessage` and `System.Net.Mail.SmtpClient`

If you've used `Send-MailMessage` recently, you've likely seen this error message:

![Warning saying Send-MailMessage is obsolete, that it does not guarantee a secure connection to SMTP servers, but there is no replacement](https://thepowershellbear.files.wordpress.com/2022/03/send-mailmessage-warning.png "Not sure why they didn't just fix it...")

If you go to the [link for more information](https://aka.ms/SendMailMessage), it will let you know that `Net.Mail.SmtpClient` (which `Send-MailMessage` is based on) does not support many modern (and more secure) protocols. The recommendation is to use [`MailKit`](https://github.com/jstedfast/MailKit) instead.

Unfortunately, this doesn't help much if you're using PowerShell instead of C#, unless you're wanting to import this type library into your scripts and build emails manually, which isn't a great experience when you're used to `Send-MailMessage` doing that for you.

## `PoshMailKit` and `Send-MKMailMessage`

This project was started to build a better replacement for `Send-MailMessage` based on `MailKit` to address the shortcomings of current replacements available. `Send-MKMailMessage` was built first-and-foremost to replicate all existing functionality of `Send-MailMessage` using `MailKit` as its basis, and to do so by taking the same parameter input as `Send-MailMessage`: if you're using `Send-MailMessage`, no matter what way you're using it, you should be able to just change the call to `Send-MKMailMessage` and have it work the exact same way. At most, you should only have to add a `-Legacy` switch to ensure backwards compatibility with `Send-MailMessage`.

As there are improvements available in `MailKit`, and some of them are at least somewhat incompatible with the way `Send-MailMessage` accepts input, there are some parameters that have been deprecated into a "Legacy mode" with Modern counterparts, and the new cmdlet behaves differently in a few spots by default in "Modern mode". For example, it defaults to attempting SSL/TLS connections, where `Send-MailMessage` would only do so if you explicitly told it to.

## Beyond `Send-MKMailMessage`

At this time, `Send-MKMailMessage` is the only cmdlet that has been built, but the goal is to add additional tools to the project for other mail tasks made possible by MailKit.

Additionally, the first version of `PoshMailKit` is built on .NET Framework 4.8 to support use in Windows PowerShell 5.1, but future versions will likely be based on .NET 6.0+ and likely will not be available to Windows PowerShell at all; that said, if that is the direction this goes in, version 1.x will be maintained for some period of time for PS 5 support; it just won't get any new cmdlets.

## Installing from PowerShell Gallery for use

You can find the current version on the PowerShell Gallery here: https://www.powershellgallery.com/packages/PoshMailKit/

You can also install using the following command:

```powershell
Install-Module -Name PoshMailKit
```

## Building the project

This is built in Visual Studio 2022 using C# 7 (Framework 4.8). It relies on MailKit, System.Management.Automation.dll (version 10.0.10586; deprecated but needed to target Windows PowerShell rather than just modern PowerShell), and System.IO.Abstractions (for unit testing with a mock filesystem), as well as the dependencies of those packages.

The unit test project relies on MailKit, System.IO.Abstractions, System.IO.Abstractions.TestingHelpers, xunit, and Moq.

Everything should install either automatically or through the NuGet package manager when loaded in VS 2022.
