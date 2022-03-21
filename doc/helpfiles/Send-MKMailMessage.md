---
external help file: PoshMailKit.dll-help.xml
Module Name: PoshMailKit
online version: https://github.com/poshcodebear/PoshMailKit/blob/main/doc/helpfiles/Send-MKMailMessage.md
schema: 2.0.0
---

# Send-MKMailMessage

## SYNOPSIS

Sends an email message to an SMTP server.

## SYNTAX

### Modern (Default)

```powershell
Send-MKMailMessage [-Attachments <String[]>] [-Bcc <String[]>] [[-Body] <String>] [-Cc <String[]>]
 [-Credential <PSCredential>] -From <String> [-InlineAttachments <Hashtable>] [-Port <Int32>]
 [-ReplyTo <String[]>] [[-SmtpServer] <String>] [[-Subject] <String>] [-To] <String[]>
 [-BodyFormat <TextFormat>] [-CharsetEncoding <Encoding>] [-ContentTransferEncoding <ContentEncoding>]
 [-DeliveryStatusNotification <DeliveryStatusNotification>] [-MessagePriority <MessagePriority>]
 [-SecureSocketOptions <SecureSocketOptions>] [<CommonParameters>]
```

### Legacy

```powershell
Send-MKMailMessage [-Attachments <String[]>] [-Bcc <String[]>] [[-Body] <String>] [-Cc <String[]>]
 [-Credential <PSCredential>] -From <String> [-InlineAttachments <Hashtable>] [-Port <Int32>]
 [-ReplyTo <String[]>] [[-SmtpServer] <String>] [[-Subject] <String>] [-To] <String[]> [-Legacy] [-BodyAsHtml]
 [-DeliveryNotificationOption <DeliveryNotificationOptions>] [-Encoding <Encoding>] [-Priority <MailPriority>]
 [-UseSsl] [<CommonParameters>]
```

## DESCRIPTION

The Send-MKMailMessage cmdlet sends an email message from within PowerShell.

It supports the mechanisms needed to make and send most kinds of email and can communicate with mail servers using most modern supported security protols.

Send-MKMailMessage is designed to be a drop-in replacement of Send-MailMessage, which uses the deprecated System.Net.Mail.SmtpClient class and does not guarantee secure connections to SMTP servers.
It uses the cross-platform mail client library [MailKit](https://github.com/jstedfast/MailKit) to replace the functionality from System.Net.Mail.SmtpClient.

## EXAMPLES

### Example 1: Send an email from one person to another person

```powershell
Send-MKMailMessage -From 'Emily <emily@poshcodebear.com>' -To 'Erica <erica@poshcodebear.com>' -Subject 'Test mail'
```

The Send-MKMailMessage cmdlet uses the From parameter to specify the message's sender.
The To parameter specifies the message's recipient.
The Subject parameter uses the text string Test mail as the message because the optional Body parameter is not included.
Since the SmtpServer parameter isn't specified, it will attempt to use the value of $PSEmailServer as the server to send to.

### Example 2: Send an attachment

```powershell
Send-MKMailMessage -From 'Emily <emily@poshcodebear.com>' -To 'Erica <erica@poshcodebear.com>', 'Jessica <jessica@poshcodebear.com>' -Subject 'Sending the Attachment' -Body "Forgot to send the attachment. Sending now." -Attachments .\honey-report.csv -MessagePriority Urgent -DeliveryStatusNotification Success -SmtpServer 'smtp.poshcodebear.com'
```

The Send-MKMailMessage cmdlet uses the From parameter to specify the message's sender.
The To parameter specifies the message's recipients.
The Subject parameter describes the content of the message.
The Body parameter is the content of the message.

The Attachments parameter specifies the file in the current directory that is attached to the email message.
The MessagePriority parameter sets the message to Urgent priority.
The -DeliveryStatusNotification parameter is set to Success, so the sender will receive email notifications to confirm the success of the message delivery.
The SmtpServer parameter sets the SMTP server to smtp.poshcodebear.com.

### Example 3: Send email using credentials and TLS

```powershell
Send-MKMailMessage -From 'Emily <emily@poshcodebear.com>' -To 'Hibernation Prep Team <hibernators@poshcodebear.com>' -Cc 'Erica <erica@fabrikam.com>' -Bcc 'Hibernation Manager <hiberman@poshcodebear.com>' -Subject "Don't forget, hibernation is just three weeks away!" -Credential poshcodebear\emily -SecureSocketOptions StartTls
```

The Send-MKMailMessage cmdlet uses the From parameter to specify the message's sender.
The To parameter specifies the message's recipients.
The Cc parameter sends a copy of the message to the specified recipient.
The Bcc parameter sends a blind copy of the message.
A blind copy is an email address that is hidden from the other recipients.

The Credential parameter specifies a domain user's credentials are used to send the message.
The SecureSocketOptions parameter specifies that STARTTLS will be used to create a secure connection.

## PARAMETERS

### -Attachments

Specifies files to be attached to the email message.

```yaml
Type: String[]
Parameter Sets: (All)
Aliases: PsPath

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: False
```

### -Bcc

Specifies the email addresses that receive a copy of the email but are not listed as recipients of the message.

Accepts email addresses in the following formats:

* Email by itself: 'mailaddress@domain.tld'
* Email with display (friendly) name: 'Display name \<mailaddress@domain.tld\>'

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Body

Specifies the content of the email message.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: 2
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -BodyAsHtml

Specifies that the Body contents are in an HTML format so the recipients' mail clients will render it correctly.

```yaml
Type: SwitchParameter
Parameter Sets: Legacy
Aliases: BAH

Required: False
Position: Named
Default value: False
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -BodyFormat

Specifies the format of the email message's Body contents so the recipients' mail clients will render it correctly.

The acceptable values for this parameter are:

The acceptable values for this parameter are:

* Plain
* Text
* Flowed
* Html
* Enriched
* CompressedRichText
* RichText

```yaml
Type: TextFormat
Parameter Sets: Modern
Aliases:

Required: False
Position: Named
Default value: Plain
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Cc

Specifies the email addresses that receive a copy of the email, listed as Carbon Copy (CC) recipients of the message.

Accepts email addresses in the following formats:

* Email by itself: 'mailaddress@domain.tld'
* Email with display (friendly) name: 'Display name \<mailaddress@domain.tld\>'

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -CharsetEncoding

Specifies the type of encoding for the message body as a \[System.Text.Encoding\] type.

Note: unlike with the Legacy Encoding parameter, this does not change the content transfer encoding

```yaml
Type: Encoding
Parameter Sets: Modern
Aliases:

Required: False
Position: Named
Default value: UTF8
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -ContentTransferEncoding

Specifies the content transfer encoding type to use for the mail message.

The acceptable values for this parameter are:

* Default
* SevenBit
* EightBit
* Binary
* Base64
* QuotedPrintable
* UUEncode

Note: the Legacy mode transfer encoding with the default Encoding is QuotedPrintable

```yaml
Type: ContentEncoding
Parameter Sets: Modern
Aliases:
Accepted values: Base64, Binary, Default, EightBit, QuotedPrintable, SevenBit, UUEncode

Required: False
Position: Named
Default value: Base64
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Credential

Specifies the credentials to use for authenticating to the SMTP server.
If left blank, no authentication will be attempted.

Type a user name, such as User01 or Domain01\User01, and you will be prompted for your password.
Or, enter a \[PSCredential\] object, such as one from the Get-Credential cmdlet.

Credentials are stored in a \[PSCredential\] object and the password is stored as a \[SecureString\].

Additional info:

* [PSCredential](https://docs.microsoft.com/en-us/dotnet/api/system.management.automation.pscredential)
* [SecureString](https://docs.microsoft.com/en-us/dotnet/api/system.security.securestring)

```yaml
Type: PSCredential
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -DeliveryNotificationOption

Specifies the delivery notification options for the email message.

The delivery notifications are sent to the address in the From parameter.

The acceptable values for this parameter are:

* None: No notification
* OnSuccess: Notify if the delivery is successful
* OnFailure: Notify if the delivery is unsuccessful
* Delay: Notify if the delivery is delayed
* Never: Never notify

```yaml
Type: DeliveryNotificationOptions
Parameter Sets: Legacy
Aliases: DNO
Accepted values: None, OnSuccess, OnFailure, Delay, Never

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -DeliveryStatusNotification

Specifies the delivery notification options for the email message.

The delivery notifications are sent to the address in the From parameter.

The acceptable values for this parameter are:

* Never: Never notify
* Success: Notify if the delivery is successful
* Failure: Notify if the delivery is unsuccessful
* Delay: Notify if the delivery is delayed

```yaml
Type: DeliveryStatusNotification
Parameter Sets: Modern
Aliases:
Accepted values: Never, Success, Failure, Delay

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Encoding

Specifies the type of encoding for the message body.

The acceptable values for this parameter are:

* ASCII: Uses ASCII (7-bit) character set
* BigEndianUnicode: Uses UTF-16 with the big-endian byte order
* BigEndianUTF32: Uses UTF-32 with the big-endian byte order
* Unicode: Uses UTF-16 with the little-endian byte order
* UTF7: Uses UTF-7
* UTF8: Uses UTF-8
* UTF8BOM: Uses UTF-8, explicitly with BOM
* UTF8NoBOM: Uses UTF-8, explicitly without BOM
* UTF32: Uses UTF-32 with the little-endian byte order

```yaml
Type: Encoding
Parameter Sets: Legacy
Aliases: BE
Accepted values: ASCII, BigEndianUnicode, Default, OEM, Unicode, UTF7, UTF8, UTF32

Required: False
Position: Named
Default value: ASCII
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -From

Specifies the sender's email address.

Accepts email addresses in the following formats:

* Email by itself: 'mailaddress@domain.tld'
* Email with display (friendly) name: 'Display name \<mailaddress@domain.tld\>'

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -InlineAttachments

Specifies files to be attached inline to the email message along with a label for each, as a hashtable with the labels as the keys and the file paths as the values.

This is useful for setting up inline images for an HTML body, for example.
Using it this way might look like this:

```powershell
<#...#> -InlineAttachments @{logo = '.\logo-image.png'} -Body "[...]\<img src='cid:logo' />[...]" -BodyFormat Html <#...#>
```

```yaml
Type: Hashtable
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Legacy

Force Send-MKMailMessage to function in Legacy mode.

There are a number of differences in how the cmdlet behaves by default vs the legacy Send-MailMessage.
If any of the Legacy set parameters are used, Send-MKMailMessage will switch to Legacy mode and emulate the behaviors of Send-MailMessage; however, if no Legacy paramaters are used, then it will operate in Modern mode.
Use this switch to force the cmdlet into Legacy mode without needing to use a Legacy parameter.

```yaml
Type: SwitchParameter
Parameter Sets: Legacy
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -MessagePriority

Specifies the priority of the email message.

The acceptable values for this parameter are:

* NonUrgent: Low priority
* Normal: Normal priority (no priority flag set)
* Urgent: High priority

```yaml
Type: MessagePriority
Parameter Sets: Modern
Aliases:
Accepted values: Normal, Urgent, NonUrgent

Required: False
Position: Named
Default value: Normal
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Port

Specifies which port to connect to on the SMTP server.

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: 25
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Priority

Specifies the priority of the email message.

The acceptable values for this parameter are:

* Normal
* High
* Low

```yaml
Type: MailPriority
Parameter Sets: Legacy
Aliases:
Accepted values: Normal, High, Low

Required: False
Position: Named
Default value: Normal
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -ReplyTo

Specifies additional email addresses (other than the From address) to use to reply to this message.

Accepts email addresses in the following formats:

* Email by itself: 'mailaddress@domain.tld'
* Email with display (friendly) name: 'Display name \<mailaddress@domain.tld\>'

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -SecureSocketOptions

Specifies how to approach secure connections.

The delivery notifications are sent to the address in the From parameter.

The acceptable values for this parameter are:

* None: No SSL or TLS encryption should be used
* Auto: Let MailKit decide which SSL or TLS option to use; if the server does not support SSL or TLS, then the connection will continue without any encryption; if it does but the server certificate is invalid or untrusted, the connection will fail
* SslOnConnect: The connection should use SSL or TLS immediately
* StartTls: Elevates the connection to use TLS encryption immediately after reading the greeting and capabilities of the server; if the server does not support the STARTTLS extension, then the connection will fail
* StartTlsWhenAvailable: Elevates the connection to use TLS encryption immediately after reading the greeting and capabilities of the server, but only if the server supports the STARTTLS extension

```yaml
Type: SecureSocketOptions
Parameter Sets: Modern
Aliases:
Accepted values: None, Auto, SslOnConnect, StartTls, StartTlsWhenAvailable

Required: False
Position: Named
Default value: Auto
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -SmtpServer

Specifies the name of the SMTP server to send the email to.

The default value is the value of the $PSEmailServer preference variable.
If the variable is not set and this parameter is not used, the command fails.

```yaml
Type: String
Parameter Sets: (All)
Aliases: ComputerName

Required: False
Position: 3
Default value: $PSEmailServer
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Subject

This parameter specifies the subject of the email message.

```yaml
Type: String
Parameter Sets: (All)
Aliases: sub

Required: False
Position: 1
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -To

Specifies the email addresses of the primary recipients of the email message.

Accepts email addresses in the following formats:

* Email by itself: 'mailaddress@domain.tld'
* Email with display (friendly) name: 'Display name \<mailaddress@domain.tld\>'

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -UseSsl

The Secure Sockets Layer (SSL) protocol is used to establish a secure connection to the remote computer to send mail.
By default, SSL is not used in Legacy mode.

```yaml
Type: SwitchParameter
Parameter Sets: Legacy
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String

You can pipe the path and file names of attachments to Send-MKMailMessage.

## OUTPUTS

### None

This cmdlet does not generate any output.

## NOTES

## RELATED LINKS

[about_PoshMailKit]()

[Get-Credential](https://docs.microsoft.com/en-us/powershell/module/microsoft.powershell.security/get-credential)

[PoshMailKit PowerShell Gallery page:](https://www.powershellgallery.com/packages/PoshMailKit)

[PoshMailKit project:](https://github.com/poshcodebear/PoshMailKit)

[MailKit project:](https://github.com/jstedfast/MailKit)

[MimeKit project:](https://github.com/jstedfast/MimeKit)
