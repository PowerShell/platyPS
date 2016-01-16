## Add-History

### SYNOPSIS
Appends entries to the session history.

### DESCRIPTION
The Add-History cmdlet adds entries to the end of the session history, that is, the list of commands entered during the current session.
You can use the Get-History cmdlet to get the commands and pass them to Add-History, or you can export the commands to a CSV or XML file, then import the commands, and pass the imported file to Add-History. You can use this cmdlet to add specific commands to the history or to create a single history file that includes commands from more than one session.

### PARAMETERS

#### InputObject [PSObject[]]

```powershell
[Parameter(
  Position = 1,
  ValueFromPipeline = $true,
  ParameterSetName = 'Set 1')]
```

Adds the specified HistoryInfo object to the session history. You can use this parameter to submit a HistoryInfo object, such as the ones that are returned by the Get-History, Import-Clixml, or Import-Csv cmdlets, to Add-History.

#### Passthru [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Returns a history object for each history entry. By default, this cmdlet does not generate any output.


### INPUTS
#### Microsoft.PowerShell.Commands.HistoryInfo
You can pipe a HistoryInfo object to Add-History.

### OUTPUTS
#### None or Microsoft.PowerShell.Commands.HistoryInfo
When you use the PassThru parameter, Add-History returns a HistoryInfo object. Otherwise, this cmdlet does not generate any output.

### NOTES
The session history is a list of the commands entered during the session along with the ID. The session history represents the order of execution, the status, and the start and end times of the command. As you enter each command, Windows PowerShell adds it to the history so that you can reuse it.  For more information about the session history, see about_History.
To specify the commands to add to the history, use the InputObject parameter. The Add-History command accepts only HistoryInfo objects, such as those returned for each command by the Get-History cmdlet. You cannot pass it a path and file name or a list of commands.
You can use the InputObject parameter to pass a file of HistoryInfo objects to Add-History. To do so, export the results of a Get-History command to a file by using the Export-Csv or Export-Clixml cmdlet and then import the file by using the Import-Csv or Import-Clixml cmdlets. You can then pass the file of imported HistoryInfo objects to Add-History through a pipeline or in a variable. For more information, see the examples.
The file of HistoryInfo objects that you pass to the Add-History cmdlet must include the type information, column headings, and all of the properties of the HistoryInfo objects. If you intend to pass the objects back to Add-History, do not use the NoTypeInformation parameter of the Export-Csv cmdlet and do not delete the type information, column headings, or any fields in the file.
To edit the session history, export the session to a CSV or XML file, edit the file, import the file, and use Add-History to append it to the current session history.


### EXAMPLES
#### -------------------------- EXAMPLE 1 --------------------------

```powershell
PS C:\>get-history | export-csv c:\testing\history.csv
PS C:\>import-csv history.csv | add-history

```
These commands add the commands typed in one Windows PowerShell session to the history of a different Windows PowerShell session. The first command gets objects representing the commands in the history and exports them to the History.csv file. The second command is typed at the command line of a different session. It uses the Import-Csv cmdlet to import the objects in the History.csv file. The pipeline operator passes the objects to the Add-History cmdlet, which adds the objects representing the commands in the History.csv file to the current session history.






#### -------------------------- EXAMPLE 2 --------------------------

```powershell
PS C:\>import-clixml c:\temp\history.xml | add-history -passthru | foreach-object -process {invoke-history}

```
This command imports commands from the History.xml file, adds them to the current session history, and then executes the commands in the combined history.
The first command uses the Import-Clixml cmdlet to import a command history that was exported to the History.xml file. The pipeline operator (|) passes the commands to the Add-History parameter, which adds the commands to the current session history. The PassThru parameter passes the objects representing the added commands down the pipeline.
The command then uses the ForEach-Object cmdlet to apply the Invoke-History command to each of the commands in the combined history. The Invoke-History command is formatted as a script block (enclosed in braces) as required by the Process parameter of the ForEach-Object cmdlet.






#### -------------------------- EXAMPLE 3 --------------------------

```powershell
PS C:\>get-history -id 5 -count 5 | add-history

```
This command adds the first five commands in the history to the end of the history list. It uses the Get-History cmdlet to get the five commands ending in command 5. The pipeline operator (|) passes them to the Add-History cmdlet, which appends them to the current history. The Add-History command does not include any parameters, but Windows PowerShell associates the objects passed through the pipeline with the InputObject parameter of  Add-History.






#### -------------------------- EXAMPLE 4 --------------------------

```powershell
PS C:\>$a = import-csv c:\testing\history.csv
PS C:\>add-history -inputobject $a -passthru

```
These commands add the commands in the History.csv file to the current session history. The first command uses the Import-Csv cmdlet to import the commands in the History.csv file and store its contents in the variable $a. The second command uses the Add-History cmdlet to add the commands from History.csv to the current session history. It uses the InputObject parameter to specify the $a variable and the PassThru parameter to generate an object to display at the command line. Without the PassThru parameter, the Add-History cmdlet does not generate any output.






#### -------------------------- EXAMPLE 5 --------------------------

```powershell
PS C:\>add-history -inputobject (import-clixml c:\temp\history01.xml)

```
This command adds the commands in the History01.xml file to the current session history. It uses the InputObject parameter to pass the results of the command in parentheses to the Add-History cmdlet. The command in parentheses, which is executed first, imports the History01.xml file into Windows PowerShell. The Add-History cmdlet then adds the commands in the file to the session history.







### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289569)
[Clear-History]()
[Get-History]()
[Invoke-History]()
[about_History]()

## Add-PSSnapin

### SYNOPSIS
Adds one or more Windows PowerShell snap-ins to the current session.

### DESCRIPTION
The Add-PSSnapin cmdlet adds registered Windows PowerShell snap-ins to the current session. After the snap-ins are added, you can use the cmdlets and providers that the snap-ins support in the current session.
To add the snap-in to all future Windows PowerShell sessions, add an Add-PSSnapin command to your Windows PowerShell profile. For more information, see about_Profiles.
Beginning in Windows PowerShell 3.0, the core commands that are included in Windows PowerShell are packaged in modules. The exception is Microsoft.PowerShell.Core, which is a snap-in (PSSnapin). By default, only the Microsoft.PowerShell.Core snap-in is added to the session. Modules are imported automatically on first use and you can use the Import-Module cmdlet to import them.

### PARAMETERS

#### Name [String[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 1')]
```

Specifies the name of the snap-in. (This is the Name, not the AssemblyName or ModuleName.) Wildcards are permitted.
To find the names of the registered snap-ins on your system, type: "get-pssnapin -registered".

#### PassThru [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Returns an object representing each added snap-in. By default, this cmdlet does not generate any output.


### INPUTS
#### None
You cannot pipe objects to Add-PSSnapin.

### OUTPUTS
#### None or System.Management.Automation.PSSnapInInfo
When you use the PassThru parameter, Add-PSSnapin returns a PSSnapInInfo object that represents the snap-in. Otherwise, this cmdlet does not generate any output.

### NOTES
Beginning in Windows PowerShell 3.0, the core commands that are installed with Windows PowerShell are packaged in modules. In Windows PowerShell 2.0, and in host programs that create older-style sessions in later versions of Windows PowerShell, the core commands are packaged in snap-ins ("PSSnapins"). The exception is Microsoft.PowerShell.Core, which is always a snap-in. Also, remote sessions, such as those started by the New-PSSession cmdlet, are older-style sessions that include core snap-ins.
For information about the CreateDefault2 method that creates newer-style sessions with core modules, see "CreateDefault2 Method" in MSDN at [http://msdn.microsoft.com/en-us/library/windows/desktop/system.management.automation.runspaces.initialsessionstate.createdefault2(v=VS.85).aspx]().
For detailed information about snap-ins in Windows PowerShell, see about_Pssnapins. For information about how to create a Windows PowerShell snap-in, see "How to Create a Windows PowerShell Snap-in" in the MSDN (Microsoft Developer Network) library at http://go.microsoft.com/fwlink/?LinkId=144762http://go.microsoft.com/fwlink/?LinkId=144762.
Add-PSSnapin adds the snap-in only to the current session. To add the snap-in to all Windows PowerShell sessions, add it to your Windows PowerShell profile. For more information, see about_Profiles.
You can add any Windows PowerShell snap-in that has been registered by using the Microsoft .NET Framework install utility. For more information, see "How to Register Cmdlets, Providers, and Host Applications" in the MSDN library at http://go.microsoft.com/fwlink/?LinkID=143619http://go.microsoft.com/fwlink/?LinkID=143619.
To get a list of snap-ins that are registered on your computer, type get-pssnapin -registered.
Before adding a snap-in, Add-PSSnapin checks the version of the snap-in to verify that it is compatible with the current version of Windows PowerShell. If the snap-in fails the version check, Windows PowerShell reports an error.

### EXAMPLES
#### -------------------------- EXAMPLE 1 --------------------------

```powershell
PS C:\>add-PSSnapIn Microsoft.Exchange, Microsoft.Windows.AD

```
This command adds the Microsoft Exchange and Active Directory snap-ins to the current session.






#### -------------------------- EXAMPLE 2 --------------------------

```powershell
PS C:\>get-pssnapin -registered | add-pssnapin -passthru

```
This command adds all of the registered Windows PowerShell snap-ins to the session. It uses the Get-PSSnapin cmdlet with the Registered parameter to get objects representing each of the registered snap-ins. The pipeline operator (|) passes the result to Add-PSSnapin, which adds them to the session. The PassThru parameter returns objects that represent each of the added snap-ins.






#### -------------------------- EXAMPLE 3 --------------------------

```powershell
The first command gets snap-ins that have been added to the current session, including the snap-ins that are installed with Windows PowerShell. In this example, ManagementFeatures is not returned. This indicates that it has not been added to the session.
PS C:\>get-pssnapin

The second command gets snap-ins that have been registered on your system (including those that have already been added to the session). It does not include the snap-ins that are installed with Windows PowerShell.In this case, the command does not return any snap-ins. This indicates that the ManagementFeatures snapin has not been registered on the system.
PS C:\>get-pssnapin -registered

The third command creates an alias, "installutil", for the path to the InstallUtil tool in .NET Framework.
PS C:\>set-alias installutil $env:windir\Microsoft.NET\Framework\v2.0.50727\installutil.exe

The fourth command uses the InstallUtil tool to register the snap-in. The command specifies the path to ManagementCmdlets.dll, the file name or "module name" of the snap-in.
PS C:\>installutil C:\Dev\Management\ManagementCmdlets.dll

The fifth command is the same as the second command. This time, you use it to verify that the ManagementCmdlets snap-in is registered.
PS C:\>get-pssnapin -registered

The sixth command uses the Add-PSSnapin cmdlet to add the ManagementFeatures snap-in to the session. It specifies the name of the snap-in, ManagementFeatures, not the file name.
PS C:\>add-pssnapin ManagementFeatures

To verify that the snap-in is added to the session, the seventh command uses the Module parameter of the Get-Command cmdlet. It displays the items that were added to the session by a snap-in or module.
PS C:\>get-command -module ManagementFeatures

You can also use the PSSnapin property of the object that the Get-Command cmdlet returns to find the snap-in or module in which a cmdlet originated. The eighth command uses dot notation to find the value of the PSSnapin property of the Set-Alias cmdlet.
PS C:\>(get-command set-alias).pssnapin

```
This example demonstrates the process of registering a snap-in on your system and then adding it to your session. It uses ManagementFeatures, a fictitious snap-in implemented in a file called ManagementCmdlets.dll.



### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289570)
[Get-PSSnapin]()
[Remove-PSSnapin]()
[about_Profiles]()
[about_PSSnapins]()

## Clear-History

### SYNOPSIS
Deletes entries from the command history.

### DESCRIPTION
The Clear-History cmdlet deletes commands from the command history, that is, the list of commands entered during the current session.
Without parameters, Clear-History deletes all commands from the session history, but you can use the parameters of Clear-History to delete selected commands.

### PARAMETERS

#### CommandLine [String[]]

```powershell
[Parameter(ParameterSetName = 'Set 2')]
```

Deletes commands with the specified text strings. If you enter more than one string, Clear-History deletes commands with any of the strings.

#### Count [Int32]

```powershell
[Parameter(Position = 2)]
```

Clears the specified number of  history entries, beginning with the oldest entry in the history.
If you use the Count and Id parameters in the same command, the cmdlet clears the number of entries specified by the Count parameter, beginning with the entry specified by the Id parameter.  For example, if Count is 10 and Id is 30, Clear-History clears items 21 through 30 inclusive.
If you use the Count and CommandLine parameters in the same command, Clear-History clears the number of entries specified by the Count parameter, beginning with the entry specified by the CommandLine parameter.

#### Id [Int32[]]

```powershell
[Parameter(
  Position = 1,
  ParameterSetName = 'Set 1')]
```

Deletes commands with the specified history IDs.
To find the history ID of a command, use Get-History.

#### Newest [switch]

Deletes the newest entries in the history. By default, Clear-History deletes the oldest entries in the history.

#### Confirm [switch]

Prompts you for confirmation before running the cmdlet.Prompts you for confirmation before running the cmdlet.

#### WhatIf [switch]

Shows what would happen if the cmdlet runs. The cmdlet is not run.Shows what would happen if the cmdlet runs. The cmdlet is not run.


### INPUTS
#### None
You cannot pipe objects to Clear-History.

### OUTPUTS
#### None
This cmdlet does not generate any output.

### NOTES
The session history is a list of the commands entered during the session. You can view the history, add and delete commands, and run commands from the history. For more information, see about_History.
Deleting a command from the history does not change the history IDs of the remaining items in the command history.


### EXAMPLES
#### -------------------------- EXAMPLE 1 --------------------------

```powershell
PS C:\>clear-history

```
Deletes all commands from the session history.






#### -------------------------- EXAMPLE 2 --------------------------

```powershell
PS C:\>clear-history -id 23, 25

```
Deletes the commands with history IDs 23 and 25.






#### -------------------------- EXAMPLE 3 --------------------------

```powershell
PS C:\>clear-history -command *help*, *command

```
Deletes commands that include "help" or end in "command".






#### -------------------------- EXAMPLE 4 --------------------------

```powershell
PS C:\>clear-history -count 10 -newest

```
Deletes the 10 newest commands from the history.






#### -------------------------- EXAMPLE 5 --------------------------

```powershell
PS C:\>clear-history -id 10 -count 3

```
Deletes the three oldest commands, beginning with the entry with ID 10.







### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289571)
[Add-History]()
[Get-History]()
[Invoke-History]()
[about_History]()

## Connect-PSSession

### SYNOPSIS
Reconnects to disconnected sessions

### DESCRIPTION
The Connect-PSSession cmdlet reconnects to user-managed Windows PowerShell sessions ("PSSessions") that were disconnected. It works on sessions that are disconnected intentionally, such as by using the Disconnect-PSSession cmdlet or the InDisconnectedSession parameter of the Invoke-Command cmdlet, and those that were disconnected unintentionally, such as by a temporary network outage.
Connect-PSSession can connect to any disconnected session that was started by the same user, including those that were started by or disconnected from other sessions on other computers.
However, Connect-PSSession cannot connect to broken or closed sessions, or interactive sessions started by using the Enter-PSSession cmdlet. Also you cannot connect sessions to sessions started by other users, unless you can provide the credentials of the user who created the session.
For more information about the Disconnected Sessions feature, see about_Remote_Disconnected_Sessions.
This cmdlet is introduced in Windows PowerShell 3.0.

### PARAMETERS

#### Authentication [AuthenticationMechanism]

```powershell
[Parameter(ParameterSetName = 'Set 2')]
[Parameter(ParameterSetName = 'Set 3')]
[Parameter(ParameterSetName = 'Set 4')]
[Parameter(ParameterSetName = 'Set 5')]
```

Specifies the mechanism that is used to authenticate the user's credentials in the command to reconnect to the disconnected session. Valid values are Default, Basic, Credssp, Digest, Kerberos, Negotiate, and NegotiateWithImplicitCredential.  The default value is Default.
For information about the values of this parameter, see "AuthenticationMechanism enumeration" in MSDN.
CAUTION: Credential Security Support Provider (CredSSP) authentication, in which the user's credentials are passed to a remote computer to be authenticated, is designed for commands that require authentication on more than one resource, such as accessing a remote network share. This mechanism increases the security risk of the remote operation. If the remote computer is compromised, the credentials that are passed to it can be used to control the network session.

#### CertificateThumbprint [String]

```powershell
[Parameter(ParameterSetName = 'Set 2')]
[Parameter(ParameterSetName = 'Set 3')]
[Parameter(ParameterSetName = 'Set 4')]
[Parameter(ParameterSetName = 'Set 5')]
```

Specifies the digital public key certificate (X509) of a user account that has permission to connect to the disconnected session. Enter the certificate thumbprint of the certificate.
Certificates are used in client certificate-based authentication. They can be mapped only to local user accounts; they do not work with domain accounts.
To get a certificate thumbprint, use a Get-Item or Get-ChildItem command in the Windows PowerShell Cert: drive.

#### ComputerName [String[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 3')]
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 4')]
```

Specifies the computers on which the disconnected sessions are stored. Sessions are stored on the computer that is at the "server-side" or receiving end of a connection. The default is the local computer.
Type the NetBIOS name, an IP address, or a fully qualified domain name of one computer. Wildcards are not permitted. To specify the local computer, type the computer name, "localhost", or a dot (.)

#### Credential [PSCredential]

```powershell
[Parameter(ParameterSetName = 'Set 2')]
[Parameter(ParameterSetName = 'Set 3')]
[Parameter(ParameterSetName = 'Set 4')]
[Parameter(ParameterSetName = 'Set 5')]
```

Specifies a user account that has permission to connect to the disconnected session. The default is the current user.
Type a user name, such as "User01" or "Domain01\User01". Or, enter a PSCredential object, such as one generated by the Get-Credential cmdlet. If you type a user name, you will be prompted for a password.

#### Id [Int32[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 6')]
```

Specifies the IDs of the disconnected sessions. The ID parameter works only when the disconnected session was previously connected to the current session.
This parameter is valid, but not effective, when the session is stored on the local computer, but was not connected to the current session.

#### InstanceId [Guid[]]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 2')]
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 4')]
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 7')]
```

Specifies the instance IDs of the disconnected sessions.
The instance ID is a GUID that uniquely identifies a PSSession on a local or remote computer.
The instance ID is stored in the InstanceID property of the PSSession.

#### Name [String[]]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 1')]
[Parameter(ParameterSetName = 'Set 3')]
[Parameter(ParameterSetName = 'Set 5')]
```

Specifies the friendly names of the disconnected sessions.

#### Port [Int32]

```powershell
[Parameter(ParameterSetName = 'Set 3')]
[Parameter(ParameterSetName = 'Set 4')]
```

Specifies the network port on the remote computer that is used to reconnect to the session.  To connect to a remote computer, the remote computer must be listening on the port that the connection uses.  The default ports are 5985 (the WinRM port for HTTP) and 5986 (the WinRM port for HTTPS).
Before using an alternate port, you must configure the WinRM listener on the remote computer to listen at that port. To configure the listener, type the following two commands at the Windows PowerShell prompt:
Remove-Item -Path WSMan:\Localhost\listener\listener* -Recurse
New-Item -Path WSMan:\Localhost\listener -Transport http -Address * -Port <port-number>
Do not use the Port parameter unless you must. The port that is set in the command applies to all computers or sessions on which the command runs. An alternate port setting might prevent the command from running on all computers.

#### Session [PSSession[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 8')]
```

Specifies the disconnected sessions. Enter a variable that contains the PSSessions or a command that creates or gets the PSSessions, such as a Get-PSSession command.

#### SessionOption [PSSessionOption]

```powershell
[Parameter(ParameterSetName = 'Set 2')]
[Parameter(ParameterSetName = 'Set 3')]
[Parameter(ParameterSetName = 'Set 4')]
[Parameter(ParameterSetName = 'Set 5')]
```

Sets advanced options for the session.  Enter a SessionOption object, such as one that you create by using the New-PSSessionOption cmdlet, or a hash table in which the keys are session option names and the values are session option values.
The default values for the options are determined by the value of the $PSSessionOption preference variable, if it is set. Otherwise, the default values are established by options set in the session configuration.
The session option values take precedence over default values for sessions set in the $PSSessionOption preference variable and in the session configuration. However, they do not take precedence over maximum values, quotas or limits set in the session configuration. 
For a description of the session options, including the default values, see New-PSSessionOption. For information about the $PSSessionOption preference variable, see about_Preference_Variables (http://go.microsoft.com/fwlink/?LinkID=113248). For more information about session configurations, see about_Session_Configurations (http://go.microsoft.com/fwlink/?LinkID=145152).

#### ThrottleLimit [Int32]

Specifies the maximum number of concurrent connections that can be established to run this command. If you omit this parameter or enter a value of 0, the default value, 32, is used.
The throttle limit applies only to the current command, not to the session or to the computer.

#### UseSSL [switch]

```powershell
[Parameter(ParameterSetName = 'Set 3')]
[Parameter(ParameterSetName = 'Set 4')]
```

Uses the Secure Sockets Layer (SSL) protocol to connect to the disconnected session. By default, SSL is not used.
WS-Management encrypts all Windows PowerShell content transmitted over the network. UseSSL is an additional protection that sends the data across an HTTPS connection instead of an HTTP connection.
If you use this parameter, but SSL is not available on the port used for the command, the command fails.

#### AllowRedirection [switch]

```powershell
[Parameter(ParameterSetName = 'Set 2')]
[Parameter(ParameterSetName = 'Set 5')]
```

Allows redirection of this connection to an alternate Uniform Resource Identifier (URI).
When you use the ConnectionURI parameter, the remote destination can return an instruction to redirect to a different URI. By default, Windows PowerShell does not redirect connections, but you can use this parameter to allow it to redirect the connection.
You can also limit the number of times the connection is redirected by changing the MaximumConnectionRedirectionCount session option value. Use the  MaximumRedirection parameter of the New-PSSessionOption cmdlet or set the MaximumConnectionRedirectionCount property of the $PSSessionOption preference variable. The default value is 5.

#### ApplicationName [String]

```powershell
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 3')]
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 4')]
```

Connects only to sessions that use the specified application.
Enter the application name segment of the connection URI. For example, in the following connection URI, the application name is WSMan: http://localhost:5985/WSMAN. The application name of a session is stored in the Runspace.ConnectionInfo.AppName property of the session.
The value of this parameter is used to select and filter sessions. It does not change the application that the session uses.

#### ConfigurationName [String]

```powershell
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 2')]
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 3')]
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 4')]
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 5')]
```

Connects only to sessions that use the specified session configuration.
Enter a configuration name or the fully qualified resource URI for a session configuration. If you specify only the configuration name, the following schema URI is prepended:  http://schemas.microsoft.com/powershell. The configuration name of a session is stored in the ConfigurationName property of the session.
The value of this parameter is used to select and filter sessions. It does not change the session configuration that the session uses.
For more information about session configurations, see about_Session_Configurations .

#### ConnectionUri [Uri[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 2')]
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 5')]
```

Specifies the Uniform Resource Identifiers (URIs) of the connection endpoints for the disconnected sessions.
The URI must be fully qualified.  The format of this string is as follows:

<Transport>://<ComputerName>:<Port>/<ApplicationName>

The default value is as follows:

http://localhost:5985/WSMAN
If you do not specify a connection URI, you can use the UseSSL and Port  parameters to specify the connection URI values.
Valid values for the Transport segment of the URI are HTTP and HTTPS. If you specify a connection URI with a Transport segment, but do not specify a port, the session is created with standards ports: 80 for HTTP and 443 for HTTPS. To use the default ports for Windows PowerShell remoting, specify port 5985 for HTTP or 5986 for HTTPS.
If the destination computer redirects the connection to a different URI, Windows PowerShell prevents the redirection unless you use the AllowRedirection parameter in the command.

#### Confirm [switch]

Prompts you for confirmation before running the cmdlet.Prompts you for confirmation before running the cmdlet.

#### WhatIf [switch]

Shows what would happen if the cmdlet runs. The cmdlet is not run.Shows what would happen if the cmdlet runs. The cmdlet is not run.


### INPUTS
#### System.Management.Automation.Runspaces.PSSession
You can pipe a session (PSSession) to the Connect-PSSession cmdlet.

### OUTPUTS
#### System.Management.Automation.Runspaces.PSSession
Connect-PSSession returns an object that represents the session to which it reconnected.

### NOTES
Connect-PSSession reconnects only to sessions that are disconnected, that is, sessions that have a value of Disconnected for  the State property. Only sessions that are connected to (terminate at) computers running Windows PowerShell 3.0 or later can be disconnected and reconnected.
If you use the Connect-PSSession cmdlet on a session that is not disconnected, the command has no effect on the session and it does not generate errors.
Disconnected loopback sessions with interactive tokens (those created with the EnableNetworkAccess parameter) can be reconnected only from the computer on which the session was created. This restriction protects the computer from malicious access.
The value of the State property of a PSSession is relative to the current session. Therefore, a value of Disconnected means that the PSSession is not connected to the current session. However, it does not mean that the PSSession is disconnected from all sessions. It might be connected to a different session. To determine whether you can connect or reconnect to the session, use the Availability property.
An Availability value of None indicates that you can connect to the session. A value of Busy indicates that you cannot connect to the PSSession because it is connected to another session.
For more information about the values of the State property of sessions, see "RunspaceState Enumeration" in MSDN at [http://msdn.microsoft.com/en-us/library/windows/desktop/system.management.automation.runspaces.runspacestate(v=VS.85).aspx]().
For more information about the values of the Availability property of sessions, see RunspaceAvailability Enumeration at [http://msdn.microsoft.com/en-us/library/windows/desktop/system.management.automation.runspaces.runspaceavailability(v=vs.85).aspx]().
You cannot change the idle timeout value of a PSSession when you connect to the PSSession. The SessionOption parameter of Connect-PSSession takes a SessionOption object that has an IdleTimeout value. However, the IdleTimeout value of the SessionOption object and the IdleTimeout value of the $PSSessionOption variable are ignored when connecting to a PSSession.
You can set and change the idle timeout of a PSSession when you create the PSSession (by using the New-PSSession or Invoke-Command cmdlets) and when you disconnect from the PSSession.
The IdleTimeout property of  a PSSession is critical to disconnected sessions, because it determines how long a disconnected session is maintained on the remote computer. Disconnected sessions are considered to be idle from the moment that they are disconnected, even if commands are running in the disconnected session.

### EXAMPLES
#### Example 1

```powershell
PS C:\>Connect-PSSession -ComputerName Server01 -Name ITTask
Id Name            ComputerName    State         ConfigurationName     Availability
-- ----            ------------    -----         -----------------     ------------
 4 ITTask          Server01        Opened        ITTasks                  Available

```
This command reconnects to the ITTask session on the Server01 computer.
The output shows that the command was successful. The State of the session is Opened and the Availability is Available, indicating that you can run commands in the session.


#### Example 2

```powershell
PS C:\>Get-PSSession 

Id Name            ComputerName    State         ConfigurationName     Availability
-- ----            ------------    -----         -----------------     ------------
 1 Backups         Localhost       Opened        Microsoft.PowerShell     Available


PS C:\> Get-PSSession | Disconnect-PSSession

Id Name            ComputerName    State         ConfigurationName     Availability
-- ----            ------------    -----         -----------------     ------------
 1 Backups         Localhost       Disconnected  Microsoft.PowerShell          None


PS C:\> Get-PSSession | Connect-PSSession

Id Name            ComputerName    State         ConfigurationName     Availability
-- ----            ------------    -----         -----------------     ------------
 1 Backups         Localhost       Opened        Microsoft.PowerShell     Available


```
This example shows the effect of disconnecting and then reconnecting to a session.
The first command uses the Get-PSSession cmdlet. Without the ComputerName parameter, the command gets only sessions that were created in the current session.
The output shows that the command gets the Backups session on the local computer. The State of the session is Opened and the Availability is Available.
The second command uses the Get-PSSession cmdlet to get the PSSessions that were created in the current session and the Disconnect-PSSession cmdlet to disconnect the sessions. The output shows that the Backups session was disconnected. The State of the session is Disconnected and the Availability is None.
The third command uses the Get-PSSession cmdlet to get the PSSessions that were created in the current session and the Connect-PSSession cmdlet to reconnect the sessions. The output shows that the Backups session was reconnected. The State of the session is Opened and the Availability is Available.
If you use the Connect-PSSession cmdlet on a session that is not disconnected, the command has no effect on the session and it does not generate any errors.


#### Example 3

```powershell
The administrator begins by creating a sessions on a remote computer and running a script in the session.The first command uses the New-PSSession cmdlet to create the ITTask session on the Server01 remote computer. The command uses the ConfigurationName parameter to specify the ITTasks session configuration. The command saves the sessions in the $s variable.
PS C:\>$s = New-PSSession -ComputerName Server01 -Name ITTask -ConfigurationName ITTasks

 The second command Invoke-Command cmdlet to start a background job in the session in the $s variable. It uses the FilePath parameter to run the script in the background job.
PS C:\>Invoke-Command -Session $s {Start-Job -FilePath \\Server30\Scripts\Backup-SQLDatabase.ps1}
Id     Name            State         HasMoreData     Location             Command
--     ----            -----         -----------     --------             -------
2      Job2            Running       True            Server01             \\Server30\Scripts\Backup...

The third command uses the Disconnect-PSSession cmdlet to disconnect from the session in the $s variable. The command uses the OutputBufferingMode parameter with a value of Drop to prevent the script from being blocked by having to deliver output to the session. It uses the IdleTimeoutSec parameter to extend the session timeout to 15 hours.When the command completes, the administrator locks her computer and goes home for the evening.
PS C:\>Disconnect-PSSession -Session $s -OutputBufferingMode Drop -IdleTimeoutSec 60*60*15
Id Name            ComputerName    State         ConfigurationName     Availability
-- ----            ------------    -----         -----------------     ------------
 1 ITTask          Server01        Disconnected  ITTasks               None

Later that evening, the administrator starts her home computer, logs on to the corporate network, and starts Windows PowerShell. The fourth command uses the  Get-PSSession cmdlet to get the sessions on the Server01 computer. The command finds the ITTask session.The fifth command uses the Connect-PSSession cmdlet to connect to the ITTask session. The command saves the session in the $s variable.
PS C:\>Get-PSSession -ComputerName Server01 -Name ITTask

Id Name            ComputerName    State         ConfigurationName     Availability
-- ----            ------------    -----         -----------------     ------------
 1 ITTask          Server01        Disconnected  ITTasks               None


PS C:\>$s = Connect-PSSession -ComputerName Server01 -Name ITTask


Id Name            ComputerName    State         ConfigurationName     Availability
-- ----            ------------    -----         -----------------     ------------
 1 ITTask          Server01        Opened        ITTasks               Available

The sixth command uses the Invoke-Command cmdlet to run a Get-Job command in the session in the $s variable. The output shows that the job completed successfully.The seventh command uses the Invoke-Command cmdlet to run a Receive-Job command in the session in the $s variable in the session. The command saves the results in the $BackupSpecs variable.The eighth command uses the Invoke-Command cmdlet to runs another script in the session. The command uses the value of the $BackupSpecs variable in the session as input to the script.


PS C:\>Invoke-Command -Session $s {Get-Job}

Id     Name            State         HasMoreData     Location             Command
--     ----            -----         -----------     --------             -------
2      Job2            Completed     True            Server01             \\Server30\Scripts\Backup...

PS C:\>Invoke-Command -Session $s {$BackupSpecs = Receive-Job -JobName Job2}

PS C:\>Invoke-Command -Session $s {\\Server30\Scripts\New-SQLDatabase.ps1 -InitData $BackupSpecs.Initialization}

The ninth command disconnects from the session in the $s variable.The administrator closes Windows PowerShell and closes the computer. She can reconnect to the session on the next day and check the script status from her work computer.
PS C:\>Disconnect-PSSession -Session $s -OutputBufferingMode Drop -IdleTimeoutSec 60*60*15
Id Name            ComputerName    State         ConfigurationName     Availability
-- ----            ------------    -----         -----------------     ------------
 1 ITTask          Server01        Disconnected  ITTasks               None

```
This series of commands shows how the Connect-PSSession cmdlet might be used in an enterprise scenario. In this case, a system administrator starts a long-running job in a session on a remote computer. After starting the job, the administrator disconnects from the session and goes home. Later that evening, the administrator logs on to her home computer and verifies that the job ran to completion.



### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289572)
[Connect-PSSession]()
[Disconnect-PSSession]()
[Enter-PSSession]()
[Exit-PSSession]()
[Get-PSSession]()
[Get-PSSessionConfiguration]()
[New-PSSession]()
[New-PSSessionOption]()
[New-PSTransportOption]()
[Receive-PSSession]()
[Register-PSSessionConfiguration]()
[Remove-PSSession]()
[about_PSSessions]()
[about_Remote]()
[about_Remote_Disconnected_Sessions]()
[about_Session_Configurations]()

## Debug-Job

### SYNOPSIS
Debugs a running background, remote, or Windows PowerShell Workflow job.

### DESCRIPTION
The Debug-Job cmdlet lets you debug scripts that are running within jobs. The cmdlet is designed to debug Windows PowerShell Workflow jobs, background jobs, and jobs running in remote sessions. Debug-Job accepts a running job object, name, ID, or InstanceId as input, and starts a debugging session on the script it is running. The debugger quit command stops the job and running script. Starting in Windows PowerShell 5.0, the exit command detaches the debugger, and allows the job to continue running.

### PARAMETERS

#### Id [Int32]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 2')]
```

Specifies the ID number of a running job. To get the ID number of a job, run the Get-Job cmdlet.

#### InstanceId [Guid]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 3')]
```

Specifies the InstanceId GUID of a running job. To get the InstanceId of a job, run the Get-Job cmdlet, piping the results into a Format-* cmdlet, as shown in this example:  Get-Job | Format-List -Property Id,Name,InstanceId,State.

#### Job [Job]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 1')]
```

Specifies a running job object. The simplest way to use this parameter is to save the results of a Get-Job command that returns the running job that you want to debug in a variable, and then specify the variable as the value of this parameter.

#### Name [String]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 4')]
```

Specifies a job by the job’s friendly name. When you start a job, you can specify a job name by adding the JobName parameter, in cmdlets such as Invoke-Command and Start-Job.

#### Confirm [switch]

Prompts you for confirmation before running the cmdlet.Prompts you for confirmation before running the cmdlet.

#### WhatIf [switch]

Shows what would happen if the cmdlet runs. The cmdlet is not run.Shows what would happen if the cmdlet runs. The cmdlet is not run.


### INPUTS
#### System.Management.Automation.RemotingJob


### OUTPUTS
#### 


### NOTES

### EXAMPLES
#### Example 1: Debug a job by job ID

```powershell
PS C:\>Debug-Job -ID 3
Id     Name            PSJobTypeName   State         HasMoreData     Location             Command
--     ----            -------------   -----         -----------     --------             -------
3      Job3            RemoteJob       Running       True            PowerShellIx         TestWFDemo1.ps1
          Entering debug mode. Use h or ? for help.

          Hit Line breakpoint on 'C:\TestWFDemo1.ps1:8'

          At C:\TestWFDemo1.ps1:8 char:5
          +     Write-Output -InputObject "Now writing output:" 
          +     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
          [DBG:PowerShellIx]: PS C:\>> list

              3: 
              4:  workflow SampleWorkflowTest
              5:  {
              6:      param ($MyOutput) 
              7: 
              8:*     Write-Output -InputObject "Now writing output:" 
              9:      Write-Output -Input $MyOutput
             10: 
             11:      Write-Output -InputObject "Get PowerShell process:" 
             12:      Get-Process -Name powershell
             13: 
             14:      Write-Output -InputObject "Workflow function complete." 
             15:  }
             16: 
             17:  # Call workflow function
             18:  SampleWorkflowTest -MyOutput "Hello"

```
In this example, the debugger breaks into a running job with an ID of 3.



### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/?LinkId=512991)
[Get-Job]()
[Receive-Job]()
[Remove-Job]()
[Resume-Job]()
[Start-Job]()
[Stop-Job]()
[Suspend-Job]()
[Wait-Job]()
[about_Debuggers]()
[about_Jobs]()
[about_Job_Details]()
[about_Remote_Jobs]()
[about_Scheduled_Jobs]()

## Disable-PSRemoting

### SYNOPSIS
Prevents remote users from running commands on the local computer. 

### DESCRIPTION
The Disable-PSRemoting cmdlet prevents users on other computers from running commands on the local computer.
Disable-PSRemoting blocks remote access to all session configurations on the local computer. This prevents remote users from creating temporary or persistent sessions to the local computer. Disable-PSRemoting does not prevent users of the local computer from creating sessions ("PSSessions") on the local computer or remote computers.
To re-enable remote access to all session configurations, use the Enable-PSRemoting cmdlet. To enable remote access to selected session configurations, use the AccessMode parameter of the Set-PSSessionConfiguration cmdlet. You can also use the Enable-PSSessionConfiguration and Disable-PSSessionConfiguration cmdlets to enable and disable session configurations for all users. For more information about session configurations, see about_Session_Configurations (http://go.microsoft.com/fwlink/?LinkID=145152).
In Windows PowerShell 2.0, Disable-PSRemoting prevents all users from creating user-managed sessions ("PSSessions") to the local computer. In Windows PowerShell 3.0, Disable-PSRemoting prevents users on other computers from creating user-managed sessions on the local computer, but allows users of the local computer to create user-managed "loopback" sessions.
To run this cmdlet, start Windows PowerShell with the "Run as administrator" option.
CAUTION: On systems that have both Windows PowerShell 3.0 and the Windows PowerShell 2.0 engine, do not use Windows PowerShell 2.0 to run the Enable-PSRemoting and Disable-PSRemoting cmdlets. The commands might appear to succeed, but the remoting is not configured correctly. Remote commands, and later attempts to enable and disable remoting, are likely to fail.

### PARAMETERS

#### Force [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Suppresses all user prompts. By default, you are prompted to confirm each operation.

#### Confirm [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Prompts you for confirmation before running the cmdlet.Prompts you for confirmation before running the cmdlet.

#### WhatIf [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Shows what would happen if the cmdlet runs. The cmdlet is not run.Shows what would happen if the cmdlet runs. The cmdlet is not run.


### INPUTS
#### None
You cannot pipe input to this cmdlet.

### OUTPUTS
#### None
This cmdlet does not return any object.

### NOTES
Disabling the session configurations does not undo all the changes made by the Enable-PSRemoting or Enable-PSSessionConfiguration cmdlets. You might have to undo the following changes manually.
1. Stop and disable the WinRM service.
2. Delete the listener that accepts requests on any IP address.
3. Disable the firewall exceptions for WS-Management communications.
4. Restore the value of the LocalAccountTokenFilterPolicy to 0, which restricts remote access to members of the Administrators group on the computer.
A session configuration is a group of settings that define the environment for a session. Every session that connects to the computer must use one of the session configurations that are registered on the computer. By denying remote access to all session configurations, you effectively prevent remote users from establishing sessions that connect to the computer.
In Windows PowerShell 2.0, Disable-PSRemoting adds a "Deny_All" entry to the security descriptors of all session configurations. This setting prevents all users from creating user-managed sessions ("PSSessions") to the local computer. In Windows PowerShell 3.0, Disable-PSRemoting adds a "Network_Deny_All" entry to the security descriptors of all session configurations. This setting prevents users on other computers from creating user-managed sessions on the local computer, but allows users of the local computer to create user-managed "loopback" sessions.
In Windows PowerShell 2.0, Disable-PSRemoting is the equivalent of "Disable-PSSessionConfiguration -name *". In Windows PowerShell 3.0 and later releases, Disable-PSRemoting is the equivalent of "Set-PSSessionConfiguration -Name <Configuration name> -AccessMode Local"
In Windows PowerShell 2.0, Disable-PSRemoting is a function. Beginning in Windows PowerShell 3.0, it is a cmdlet.

### EXAMPLES
#### -------------------------- EXAMPLE 1 --------------------------

```powershell
PS C:\>Disable-PSRemoting

```
This command prevents remote access to all session configurations on the computer.


#### -------------------------- EXAMPLE 2 --------------------------

```powershell
PS C:\>Disable-PSRemoting -Force

```
This command prevents remote access all session configurations on the computer without prompting.


#### -------------------------- EXAMPLE 3 --------------------------

```powershell
PS C:\>Disable-PSRemoting -Force


[ADMIN] PS C:\>New-PSSession -ComputerName localhost


Id Name       ComputerName    State    Configuration         Availability
-- ----       ------------    -----    -------------         ------------
1 Session1   Server02...     Opened   Microsoft.PowerShell     Available
# On Server02 remote computer:
PS C:\>New-PSSession -ComputerName Server01

[SERVER01] Connecting to remote server failed with the following error
message : Access is denied. For more information, see the about_Remote_Troubleshooting Help topic.
+ CategoryInfo          : OpenError: (System.Manageme....RemoteRunspace:RemoteRunspace) [], PSRemotingTransportException
+ FullyQualifiedErrorId : PSSessionOpenFailed


```
This example shows the effect of using the Disable-PSRemoting cmdlet. To run this command sequence, start Windows PowerShell with the "Run as administrator" option.
The first command uses the Disable-PSRemoting cmdlet to disable all registered session configurations on the Server01 computer.
The second command uses the New-PSSession cmdlet to create a remote session to the local computer (also known as a "loopback"). The command succeeds.
The third command is run on the Server02 remote computer. The command uses the New-PSSession cmdlet to create a session to the Server01 remote computer. Because remote access is disabled, the command fails.


#### -------------------------- EXAMPLE 4 --------------------------

```powershell
PS C:\>Disable-PSRemoting -force

[ADMIN] PS C:\>Get-PSSessionConfiguration | Format-Table -Property Name, Permission -Auto

Name                          Permission
----                          ----------
microsoft.powershell          NT AUTHORITY\NETWORK AccessDenied, BUILTIN\Administrators AccessAllowed
microsoft.powershell.workflow NT AUTHORITY\NETWORK AccessDenied, BUILTIN\Administrators AccessAllowed
microsoft.powershell32        NT AUTHORITY\NETWORK AccessDenied, BUILTIN\Administrators AccessAllowed
microsoft.ServerManager       NT AUTHORITY\NETWORK AccessDenied, BUILTIN\Administrators AccessAllowed
WithProfile                   NT AUTHORITY\NETWORK AccessDenied, BUILTIN\Administrators AccessAllowed


[ADMIN] PS C:\>Enable-PSRemoting -Force
WinRM already is set up to receive requests on this machine.
WinRM already is set up for remote management on this machine.

[ADMIN] PS C:\>Get-PSSessionConfiguration | Format-Table -Property Name, Permission -Auto

Name                          Permission
----                          ----------
microsoft.powershell          BUILTIN\Administrators AccessAllowed
microsoft.powershell.workflow BUILTIN\Administrators AccessAllowed
microsoft.powershell32        BUILTIN\Administrators AccessAllowed
microsoft.ServerManager       BUILTIN\Administrators AccessAllowed
WithProfile                   BUILTIN\Administrators AccessAllowed

```
This example shows the effect on the session configurations of using the Disable-PSRemoting and Enable-PSRemoting cmdlets.
The first command uses the Disable-PSRemoting cmdlet to disable remote access to all session configurations. The Force parameter suppresses all user prompts.
The second command uses the Get-PSSessionConfiguration cmdlet to display the session configurations on the computer. The command uses a pipeline operator to send the results to a Format-Table command, which displays only the Name and Permission properties of the configurations in a table.
The output shows that only remote users are denied access to the configurations. Members of the Administrators group on the local computer are allowed to use the session configurations. The output also shows that the command affects all session configurations, including the user-created "WithProfile" session configuration.
The third command uses the Enable-PSRemoting cmdlet to re-enable remote access to all session configurations on the computer. The command uses the Force parameter to suppress all user prompts and to restart the WinRM service without prompting.
The fourth command uses the Get-PSSessionConfiguration and Format-Table cmdlets to display the names and permissions of the session configurations. The results show that the "AccessDenied" security descriptors have been removed from all session configurations.


#### -------------------------- EXAMPLE 5 --------------------------

```powershell
PS C:\>Register-PSSessionConfiguration -Name Test -FilePath .\TestEndpoint.pssc -ShowSecurityDescriptorUI

[ADMIN] PS C:\>Get-PSSessionConfiguration | Format-Table -Property Name, Permission -Wrap

Name                          Permission
----                          ----------
microsoft.powershell          BUILTIN\Administrators AccessAllowed
Test                          NT AUTHORITY\INTERACTIVE AccessAllowed, BUILTIN\Administrators AccessAllowed,
DOMAIN01\User01 AccessAllowed

[ADMIN] PS C:\>Disable-PSRemoting -Force


[ADMIN] PS C:\>Get-PSSessionConfiguration | Format-Table -Property Name, Permission -Wrap

Name                          Permission
----                          ----------
microsoft.powershell          NT AUTHORITY\NETWORK AccessDenied, BUILTIN\Administrators AccessAllowed
Test                          NT AUTHORITY\NETWORK AccessDenied, NTAUTHORITY\INTERACTIVE AccessAllowed,
BUILTIN\Administrators AccessAllowed, DOMAIN01\User01 AccessAllowed

# Domain01\User01

PS C:\>New-PSSession -ComputerName Server01 -ConfigurationName Test
[Server01] Connecting to remote server failed with the following error message : Access is denied. For more information, see the about_Rem
ote_Troubleshooting Help topic.
+ CategoryInfo          : OpenError: (System.Manageme....RemoteRunspace:RemoteRunspace) [], PSRemotingTransportException
+ FullyQualifiedErrorId : PSSessionOpenFailed

```
This example demonstrates that the Disable-PSRemoting cmdlet disables remote access to all session configurations, including session configurations with custom security descriptors. 
The first command uses the Register-PSSessionConfiguration cmdlet to create the Test session configuration. The command uses the FilePath parameter to specify a session configuration file that customizes the session and the ShowSecurityDescriptorUI parameter to display a dialog box that sets permissions for the session configuration. In the Permissions dialog box, we create custom full-access permissions for the Domain01\User01 user.

The second command uses the Get-PSSessionConfiguration and Format-Table cmdlets to display the session configurations and their properties. The output shows that the Test session configuration allows interactive access and special permissions for the Domain01\User01 user.
The third command uses the Disable-PSRemoting cmdlet to disable remote access to all session configurations.
The fourth command uses the Get-PSSessionConfiguration and Format-Table cmdlets to display the session configurations and their properties. The output shows that an AccessDenied security descriptor for all network users is added to all session configurations, including the Test session configuration. Although the other security desriptors are not changed, the "network_deny_all" security descriptor takes precedence.
The fifth command shows that the Disable-PSRemoting command prevents even the Domain01\User01 user with special permissions to the Test session configuration from using the Test session configuration to connect to the computer remotely. 


#### -------------------------- EXAMPLE 6 --------------------------

```powershell
PS C:\>Disable-PSRemoting -Force


[ADMIN] PS C:\>Get-PSSessionConfiguration | Format-Table -Property Name, Permission -Auto

Name                          Permission
----                          ----------
microsoft.powershell          NT AUTHORITY\NETWORK AccessDenied, BUILTIN\Administrators AccessAllowed
microsoft.powershell.workflow NT AUTHORITY\NETWORK AccessDenied, BUILTIN\Administrators AccessAllowed
microsoft.powershell32        NT AUTHORITY\NETWORK AccessDenied, BUILTIN\Administrators AccessAllowed
microsoft.ServerManager       NT AUTHORITY\NETWORK AccessDenied, BUILTIN\Administrators AccessAllowed
WithProfile                   NT AUTHORITY\NETWORK AccessDenied, BUILTIN\Administrators AccessAllowed

[ADMIN] PS C:\>Set-PSSessionConfiguration -Name Microsoft.ServerManager -AccessMode Remote -Force

[ADMIN] PS C:\>Get-PSSessionConfiguration | Format-Table -Property Name, Permission -Auto

Name                          Permission
----                          ----------
microsoft.powershell          NT AUTHORITY\NETWORK AccessDenied, BUILTIN\Administrators AccessAllowed
microsoft.powershell.workflow NT AUTHORITY\NETWORK AccessDenied, BUILTIN\Administrators AccessAllowed
microsoft.powershell32        NT AUTHORITY\NETWORK AccessDenied, BUILTIN\Administrators AccessAllowed
microsoft.ServerManager       BUILTIN\Administrators AccessAllowed
WithProfile                   NT AUTHORITY\NETWORK AccessDenied, BUILTIN\Administrators AccessAllowed

```
This example shows how to re-enable remote access only to selected session configurations. 
The first command uses the Disable-PSRemoting cmdlet to disable remote access to all session configurations. 
The second command uses the Get-PSSessionConfiguration and Format-Table cmdlets to display the session configurations and their properties. The output shows that an AccessDenied security descriptor for all network users is added to all session configurations. 
The third command uses the Set-PSSessionConfiguration cmdlet. The command uses the AccessMode parameter with a value of Remote to enable remote access to the Microsoft.ServerManager session configuration. You can also use the AccessMode parameter to enable Local access and to disable session configurations. 
The fourth command uses the Get-PSSessionConfiguration and Format-Table cmdlets to display the session configurations and their properties. The output shows that the AccessDenied security descriptor for all network users is removed, thereby restoring remote access to the Microsoft.ServerManager session configuration. 



### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289573)
[Disable-PSSessionConfiguration]()
[Enable-PSRemoting]()
[Get-PSSessionConfiguration]()
[Register-PSSessionConfiguration]()
[Set-PSSessionConfiguration]()
[Unregister-PSSessionConfiguration]()
[WSMan Provider]()

## Disable-PSSessionConfiguration

### SYNOPSIS
Disables session configurations on the local computer.

### DESCRIPTION
The Disable-PSSessionConfiguration cmdlet disables session configurations on the local computer, thereby preventing all users from using the session configurations to create a user-managed sessions ("PSSessions") on the local computer. This is an advanced cmdlet that is designed to be used by system administrators to manage customized session configurations for their users.
Beginning in Windows PowerShell 3.0, the Disable-PSSessionConfiguration cmdlet sets the Enabled setting of the session configuration (WSMan:\localhost\Plugins\<SessionConfiguration>\Enabled) to "False".
In Windows PowerShell 2.0, the Disable-PSSessionConfiguration cmdlet adds a "Deny_All" entry to the security descriptor of one or more registered session configurations.
Without parameters, Disable-PSSessionConfiguration disables the Microsoft.PowerShell configuration, which is the default configuration that is used for sessions. Unless the user specifies a different configuration, both local and remote users are effectively prevented from creating any sessions that connect to the computer. 
To disable all session configurations on the computer, use Disable-PSRemoting.

### PARAMETERS

#### Force [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Suppresses all user prompts. By default, you are prompted to confirm each operation.

#### Name [String[]]

```powershell
[Parameter(
  Position = 1,
  ParameterSetName = 'Set 1')]
```

Specifies the names of session configurations to disable. Enter one or more configuration names. Wildcards are permitted. You can also pipe a string that contains a configuration name or a session configuration object to Disable-PSSessionConfiguration.
If you omit this parameter, Disable-PSSessionConfiguration disables the Microsoft.PowerShell session configuration.

#### Confirm [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Prompts you for confirmation before running the cmdlet.Prompts you for confirmation before running the cmdlet.

#### WhatIf [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Shows what would happen if the cmdlet runs. The cmdlet is not run.Shows what would happen if the cmdlet runs. The cmdlet is not run.


### INPUTS
#### Microsoft.PowerShell.Commands.PSSessionConfigurationCommands#PSSessionConfiguration, System.String
You can pipe a session configuration object or a string that contains the name of a session configuration to Disable-PSSessionConfiguration.

### OUTPUTS
#### None
This cmdlet does not return any objects.

### NOTES
To run this cmdlet on Windows Vista, Windows Server 2008, and later versions of Windows, you must start Windows PowerShell with the "Run as administrator" option.


### EXAMPLES
#### -------------------------- EXAMPLE 1 --------------------------

```powershell
PS C:\>Disable-PSSessionConfiguration

```
This command disables the Microsoft.PowerShell session configuration.


#### -------------------------- EXAMPLE 2 --------------------------

```powershell
PS C:\>Disable-PSSessionConfiguration -Name *

```
This command disables all registered session configurations on the computer.


#### -------------------------- EXAMPLE 3 --------------------------

```powershell
PS C:\>Disable-PSSessionConfiguration -Name Microsoft* -Force

```
This command disables all session configurations that have names that begin with "Microsoft". The command uses the Force parameter to suppress all user prompts from the command.


#### -------------------------- EXAMPLE 4 --------------------------

```powershell
PS C:\>Get-PSSessionConfiguration -Name MaintenanceShell, AdminShell | Disable-PSSessionConfiguration

```
This command disables the MaintenanceShell and AdminShell session configurations.
The command uses a pipeline operator (|) to send the results of a Get-PSSessionConfiguration command to Disable-PSSessionConfiguration.






#### -------------------------- EXAMPLE 5 --------------------------

```powershell
The first command uses the Get-PSSessionConfiguration and Format-Table cmdlets to display only the Name and Permission properties of the session configuration objects. This table format makes it easier to see the values of the objects. The results show that members of the Administrators group are permitted to use the session configurations.
PS C:\>Get-PSSessionConfiguration | format-table -property Name, Permission -auto

Name                   Permission
----                   ----------
MaintenanceShell       BUILTIN\Administrators AccessAllowed
microsoft.powershell   BUILTIN\Administrators AccessAllowed
microsoft.powershell32 BUILTIN\Administrators AccessAllowed


The second command uses the Disable-PSSessionConfiguration cmdlet to disable the MaintenanceShell session configuration. The command uses the Force parameter to suppress all user prompts.
PS C:\>Disable-PSSessionConfiguration -name MaintenanceShell -force

The third command repeats the first command. The results show that you can still get the object that represents the MaintenanceShell session configuration even though everyone is denied access to it. The "AccessDenied" entry takes precedence over all other entries in the security descriptor.
PS C:\>Get-PSSessionConfiguration | format-table -property Name, Permission -auto

Name                   Permission
----                   ----------
MaintenanceShell       Everyone AccessDenied, BUILTIN\Administrators AccessAllowed
microsoft.powershell   BUILTIN\Administrators AccessAllowed
microsoft.powershell32 BUILTIN\Administrators AccessAllowed


The fourth command uses the Set-PSSessionConfiguration cmdlet to increase the MaximumDataSizePerCommandMB setting on the MaintenanceShell session configuration to 60. The results show that the command was successful even though everyone is denied access to the configuration.
PS C:\>Set-PSSessionConfiguration -name MaintenanceShell -MaximumReceivedDataSizePerCommandMB 60

ParamName            ParamValue
---------            ----------
psmaximumreceived... 60
"Restart WinRM service"
WinRM service need to be restarted to make the changes effective. Do you want to run the command "restart-service winrm"?
[Y] Yes  [N] No  [S] Suspend  [?] Help (default is "Y"): y


The fifth command attempts to use the MaintenanceShell session configuration in a session. It uses the New-PSSession cmdlet to create a new session and the ConfigurationName parameter to specify the MaintenanceShell configuration.The results show that the  New-PSSession command fails because the user is denied access to the configuration. 
PS C:\>new-pssession -computername localhost -configurationName MaintenanceShell
[localhost] Connecting to remote server failed with the following error message : Access is denied. For more information, see the about_Remote_Troubl
eshooting Help topic.
+ CategoryInfo          : OpenError: (System.Manageme....RemoteRunspace:RemoteRunspace) [], PSRemotingTransportException
+ FullyQualifiedErrorId : PSSessionOpenFailed

```
This example shows the effect of disabling a session configuration.



### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289574)
[Disable-PSSessionConfiguration]()
[Enable-PSSessionConfiguration]()
[Get-PSSessionConfiguration]()
[New-PSSessionConfigurationFile]()
[New-PSSessionConfigurationOption]()
[Register-PSSessionConfiguration]()
[Set-PSSessionConfiguration]()
[Test-PSSessionConfigurationFile]()
[Unregister-PSSessionConfiguration]()
[WSMan Provider]()
[about_Session_Configurations]()
[about_Session_Configuration_Files]()

## Disconnect-PSSession

### SYNOPSIS
Disconnects from a session.

### DESCRIPTION
The Disconnect-PSSession cmdlet disconnects a Windows PowerShell session ("PSSession"), such as one started by using the New-PSSession cmdlet, from the current session. As a result, the PSSession is in a disconnected state. You can connect to the disconnected PSSession from the current session or from another session on the local computer or a different computer.
The Disconnect-PSSession cmdlet disconnects only open PSSessions that are connected to the current session. Disconnect-PSSession cannot disconnect broken or closed PSSessions, or interactive PSSessions started by using the Enter-PSSession cmdlet,  and it cannot disconnect PSSessions that are connected to other sessions.
To reconnect to a disconnected PSSession, use the Connect-PSSession or Receive-PSSession cmdlets.
When a PSSession is disconnected, the commands in the PSSession continue to run until they complete, unless the PSSession times out or the commands in the PSSession are blocked by a full output buffer. To change the idle timeout, use the IdleTimeoutSec parameter. To change the output buffering mode, use the OutputBufferingMode parameter You can also use the InDisconnectedSession parameter of the Invoke-Command cmdlet to run a command in a disconnected session.
For more information about the Disconnected Sessions feature, see about_Remote_Disconnected_Sessions.
This cmdlet is introduced in Windows PowerShell 3.0.

### PARAMETERS

#### Id [Int32[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 2')]
```

Disconnects from sessions with the specified session ID. Type one or more IDs (separated by commas), or use the range operator (..) to specify a range of IDs.
To get the ID of a session, use the Get-PSSession cmdlet. The instance ID is stored in the ID property of the session.

#### IdleTimeoutSec [Int32]

Changes the idle timeout value of the disconnected PSSession. Enter a value in seconds. The minimum value is 60 (1 minute).
The idle timeout determines how long the disconnected PSSession is maintained on the remote computer. When the timeout expires, the PSSession is deleted.
Disconnected PSSessions are considered to be idle from the moment that they are disconnected, even if commands are running in the disconnected session. 
The default value for the idle timeout of a session is set by the value of the IdleTimeoutMs property of the session configuration. The default value is 7200000 milliseconds (2 hours).
The value of this parameter takes precedence over the value of the IdleTimeout property of the $PSSessionOption preference variable and the default idle timeout value in the session configuration. However, this value cannot exceed the value of the MaxIdleTimeoutMs property of the session configuration. The default value of MaxIdleTimeoutMs is 12 hours (43200000 milliseconds).

#### InstanceId [Guid[]]

```powershell
[Parameter(
  Mandatory = $true,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 4')]
```

Disconnects from sessions with the specified instance IDs.
The instance ID is a GUID that uniquely identifies a session on a local or remote computer. The instance ID is unique, even across multiple sessions on multiple computers.
To get the instance ID of a session, use the Get-PSSession cmdlet. The instance ID is stored in the InstanceID property of the session.

#### Name [String[]]

```powershell
[Parameter(
  Mandatory = $true,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 3')]
```

Disconnects from sessions with the specified friendly names. Wildcards are permitted.
To get the friendly name of a session, use the Get-PSSession cmdlet. The friendly name is stored in the Name property of the session.

#### Session [PSSession[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 1')]
```

Disconnects from the specified PSSessions. Enter PSSession objects, such as those that the New-PSSession cmdlet returns. You can also pipe a PSSession object to Disconnect-PSSession.
The Get-PSSession cmdlet can get all PSSessions that terminate at a remote computer, including PSSessions that are disconnected and PSSessions that are connected to other sessions on other computers. Disconnect-PSSession disconnects only PSSession that are connected to the current session. If you pipe other PSSessions to Disconnect-PSSession, the Disconnect-PSSession command fails.

#### ThrottleLimit [Int32]

Sets the throttle limit for the Disconnect-PSSession command.
The throttle limit is the maximum number of concurrent connections that can be established to run this command. If you omit this parameter or enter a value of 0, the default value, 32, is used.
The throttle limit applies only to the current command, not to the session or to the computer.

#### OutputBufferingMode [OutputBufferingMode]

Determines how command output is managed in the disconnected session when the output buffer is full. The default value is Block.
If the command in the disconnected session is returning output and the output buffer fills, the value of this parameter effectively determines whether the command continues to run while the session is disconnected. A value of Block suspends the command until the session is reconnected. A value of Drop allows the command to complete, although data might be lost. When using the Drop value, redirect the command output to a file on disk.
Valid values are:
-- Block: When the output buffer is full, execution is suspended until the buffer is clear. 
-- Drop: When the output buffer is full, execution continues. As new output is saved, the oldest output is discarded.
-- None: No output buffering mode is specified. The value of the OutputBufferingMode property of the session configuration is used for the disconnected session.

#### Confirm [switch]

Prompts you for confirmation before running the cmdlet.Prompts you for confirmation before running the cmdlet.

#### WhatIf [switch]

Shows what would happen if the cmdlet runs. The cmdlet is not run.Shows what would happen if the cmdlet runs. The cmdlet is not run.


### INPUTS
#### System.Management.Automation.Runspaces.PSSession
You can pipe a session to Disconnect-PSSession.

### OUTPUTS
#### System.Management.Automation.Runspaces.PSSession
Disconnect-PSSession returns an object that represents the session that it disconnected.

### NOTES
The Disconnect-PSSession cmdlet works only when the local and remote computers are running Windows PowerShell 3.0 or later versions of Windows PowerShell.
If you use the Disconnect-PSSession cmdlet on a disconnected session, the command has no effect on the session and it does not generate errors.
Disconnected loopback sessions with interactive security tokens (those created with the EnableNetworkAccess parameter) can be reconnected only from the computer on which the session was created. This restriction protects the computer from malicious access.
When you disconnect a PSSession, the session state is Disconnected and the availability is None. 
The value of the State property is relative to the current session. Therefore, a value of Disconnected means that the PSSession is not connected to the current session. However, it does not mean that the PSSession is disconnected from all sessions. It might be connected to a different session. To determine whether you can connect or reconnect to the session, use the Availability property.
An Availability value of None indicates that you can connect to the session. A value of Busy indicates that you cannot connect to the PSSession because it is connected to another session.
For more information about the values of the State property of sessions, see "RunspaceState Enumeration" in MSDN at [http://msdn.microsoft.com/en-us/library/windows/desktop/system.management.automation.runspaces.runspacestate(v=VS.85).aspx]().
For more information about the values of the Availability property of sessions, see RunspaceAvailability Enumeration at [http://msdn.microsoft.com/en-us/library/windows/desktop/system.management.automation.runspaces.runspaceavailability(v=vs.85).aspx]().

### EXAMPLES
#### Example 1

```powershell
PS C:\>Disconnect-PSSession -Name UpdateSession
Id Name            ComputerName    State         ConfigurationName     Availability
-- ----            ------------    -----         -----------------     ------------ 
1  UpdateSession   Server01        Disconnected  Microsoft.PowerShell          None

```
This command disconnects the UpdateSession PSSession on the Server01 computer from the current session. The command uses the Name parameter to identify the PSSession.
The output shows that the attempt to disconnect was successful. The session state is Disconnected and the Availability is None, which indicates that the session is not busy and can be reconnected.


#### Example 2

```powershell
PS C:\>Get-PSSession -ComputerName Server12 -Name ITTask | Disconnect-PSSession -OutputBufferingMode Drop -IdleTimeoutSec 86400
Id Name            ComputerName    State         ConfigurationName     Availability
-- ----            ------------    -----         -----------------     ------------ 
1  ITTask          Server12        Disconnected  ITTasks               None

```
This command disconnects the ITTask PSSession on the Server12 computer from the current session. The ITTask session was created in the current session and connects to the Server12 computer. The command uses the Get-PSSession cmdlet to get the session and the Disconnect-PSSession cmdlet to disconnect it. 
The Disconnect-PSSession command uses the OutputBufferingMode parameter to set the output mode to Drop. This setting ensures that the script that is running in the session can continue to run even if the session output buffer is full. Because the script writes its output to a report on a file share, other output can be lost without consequence. 
The command also uses the IdleTimeoutSec parameter to extend the idle timeout of the session to 24 hours. This setting allows time for this administrator or other administrators to reconnect to the session to verify that the script ran and troubleshoot if needed.


#### Example 3

```powershell
The technician begins by creating sessions on several remote computers and running a script in each session.The first command uses the New-PSSession cmdlet to create the ITTask session on three remote computers. The command saves the sessions in the $s variable. The second command uses the FilePath parameter of the Invoke-Command cmdlet to run a script in the sessions in the $s variable.
PS C:\>$s = New-PSSession -ComputerName Srv1, Srv2, Srv30 -Name ITTask

PS C:\>Invoke-Command $s -FilePath \\Server01\Scripts\Get-PatchStatus.ps1

The script running on the Srv1 computer generates unexpected errors. The technician contacts his manager and asks for assistance. The manager directs the technician to disconnect from the session so he can investigate.The second command uses the Get-PSSession cmdlet to get the ITTask session on the Srv1 computer and the Disconnect-PSSession cmdlet to disconnect it. This command does not affect the ITTask sessions on  the other computers.
PS C:\>Get-PSSession -Name ITTask -ComputerName Srv1 | Disconnect-PSSession
Id Name            ComputerName    State         ConfigurationName     Availability
-- ----            ------------    -----         -----------------     ------------ 
1 ITTask           Srv1            Disconnected  Microsoft.PowerShell          None

The third  command uses the Get-PSSession cmdlet to get the ITTask sessions. The output shows that the ITTask sessions on the Srv2 and Srv30 computers were not affected by the command to disconnect.
PS C:\>Get-PSSession -ComputerName Srv1, Srv2, Srv30 -Name ITTask
Id Name            ComputerName    State         ConfigurationName     Availability
-- ----            ------------    -----         -----------------     ------------
 1 ITTask          Srv1            Disconnected  Microsoft.PowerShell          None
 2 ITTask          Srv2            Opened        Microsoft.PowerShell     Available
 3 ITTask          Srv30           Opened        Microsoft.PowerShell     Available

The manager logs on to his home computer, connects to his corporate network, starts Windows PowerShell, and uses the Get-PSSession cmdlet to get the ITTask session on the Srv1 computer. He uses the credentials of the technician to access the session.
PS C:\>Get-PSSession -ComputerName Srv1 -Name ITTask -Credential Domain01\User01
Id Name            ComputerName    State         ConfigurationName     Availability
-- ----            ------------    -----         -----------------     ------------
 1 ITTask          Srv1            Disconnected  Microsoft.PowerShell          None

Next, the manager uses the  Connect-PSSession cmdlet to connect to the ITTask session on the Srv1 computer. The command saves the session in the $s variable.
PS C:\>$s = Connect-PSSession -ComputerName Srv1 -Name ITTask -Credential Domain01\User01

The manager uses the Invoke-Command cmdlet to run some diagnostic commands in the session in the $s variable. He recognizes that the script failed because it did not find a required directory. The manager uses the MkDir function to create the directory, and then he restarts the Get-PatchStatus.ps1 script and disconnects from the session.The manager reports his findings to the technician, suggests that he reconnect to the session to complete the tasks, and asks him to add a command to the Get-PatchStatus.ps1 script that creates the required directory if it does not exist.
PS C:\>Invoke-Command -Session $s {dir $home\Scripts\PatchStatusOutput.ps1}

PS C:\>Invoke-Command -Session $s {mkdir $home\Scripts\PatchStatusOutput}

PS C:\>Invoke-Command -Session $s -FilePath \\Server01\Scripts\Get-PatchStatus.ps1

PS C:\>Disconnect-PSSession -Session $s

```
This series of commands shows how the Disconnect-PSSession cmdlet might be used in an enterprise scenario. In this case, a new technician starts a script in a session on a remote computer and runs into a problem. The technician disconnects from the session so that a more experienced manager can connect to the session and resolve the problem.


#### Example 4

```powershell
The first command uses the New-PSSessionOption cmdlet to create a session option object. It uses the IdleTimeout parameter to set an idle timeout of 48 hours (172800000 milliseconds). The command saves the session option object in the $Timeout variable.
PS C:\>$Timeout = New-PSSessionOption -IdleTimeout 172800000

The second command uses the New-PSSession cmdlet to create the ITTask session on the Server01 computer. The command save the session in the $s variable. The value of the SessionOption parameter is the 48-hour idle timeout in the $Timeout variable.
PS C:\>$s = New-PSSession -Computer Server01 -Name ITTask -SessionOption $Timeout

The third command disconnects the ITTask session in the $s variable. The command fails because the idle timeout value of the session exceeds the MaxIdleTimeoutMs quota in the session configuration. Because the idle timeout is not used until the session is disconnected, this violation can go undetected while the session is in use.
PS C:\>Disconnect-PSSession -Session $s
Disconnect-PSSession : The session ITTask cannot be disconnected because the specified
idle timeout value 172800(seconds) is either greater than the server maximum allowed
43200 (seconds) or less that the minimum allowed60(seconds).  Choose an idle time out
value that is within the allowed range and try again.

The fourth command uses the Invoke-Command cmdlet to run a Get-PSSessionConfiguration command for the Microsoft.PowerShell session configuration on the Server01 computer. The command uses the Format-List cmdlet to display all properties of the session configuration in a list.The output shows that the  MaxIdleTimeoutMS property, which establishes the maximum permitted IdleTimeout value for sessions that use the session configuration, is 43200000 milliseconds (12 hours).
PS C:\>Invoke-Command -ComputerName Server01 {Get-PSSessionConfiguration Microsoft.PowerShell} | Format-List -Property *
Architecture                  : 64
Filename                      : %windir%\system32\pwrshplugin.dll
ResourceUri                   : http://schemas.microsoft.com/powershell/microsoft.powershell
MaxConcurrentCommandsPerShell : 1000
UseSharedProcess              : false
ProcessIdleTimeoutSec         : 0
xmlns                         : http://schemas.microsoft.com/wbem/wsman/1/config/PluginConfiguration
MaxConcurrentUsers            : 5
lang                          : en-US
SupportsOptions               : true
ExactMatch                    : true
RunAsUser                     :
IdleTimeoutms                 : 7200000
PSVersion                     : 3.0
OutputBufferingMode           : Block
AutoRestart                   : false
SecurityDescriptorSddl        : O:NSG:BAD:P(A;;GA;;;BA)S:P(AU;FA;GA;;;WD)(AU;SA;GXGW;;;WD)
MaxMemoryPerShellMB           : 1024
MaxIdleTimeoutms              : 2147483647
Uri                           : http://schemas.microsoft.com/powershell/microsoft.powershell
SDKVersion                    : 2
Name                          : microsoft.powershell
XmlRenderingType              : text
Capability                    : {Shell}
RunAsPassword                 :
MaxProcessesPerShell          : 15
ParentResourceUri             : http://schemas.microsoft.com/powershell/microsoft.powershell
Enabled                       : true
MaxShells                     : 25
MaxShellsPerUser              : 25
Permission                    : BUILTIN\Administrators AccessAllowed
PSComputerName                : localhost
RunspaceId                    : aea84310-6dbf-4c21-90ac-13980039925a
PSShowComputerName            : True


The fifth command gets the session option values of the session in the $s variable. The values of many session options are properties of the ConnectionInfo property of the Runspace property of the session.The output shows that the value of the IdleTimeout property of the session is 172800000 milliseconds (48 hours), which violates the MaxIdleTimeoutMs quota of 12 hours in the session configuration.To resolve this conflict, you can use the ConfigurationName parameter to select a different session configuration or use the IdleTimeout parameter to reduce the idle timeout of the session.
PS C:\>$s.Runspace.ConnectionInfo
ConnectionUri                     : http://Server01/wsman
ComputerName                      : Server01
Scheme                            : http
Port                              : 80
AppName                           : /wsman
Credential                        :
ShellUri                          : http://schemas.microsoft.com/powershell/Microsoft.PowerShell
AuthenticationMechanism           : Default
CertificateThumbprint             :
MaximumConnectionRedirectionCount : 5
MaximumReceivedDataSizePerCommand :
MaximumReceivedObjectSize         : 209715200
UseCompression                    : True
NoMachineProfile                  : False
ProxyAccessType                   : None
ProxyAuthentication               : Negotiate
ProxyCredential                   :
SkipCACheck                       : False
SkipCNCheck                       : False
SkipRevocationCheck               : False
NoEncryption                      : False
UseUTF16                          : False
OutputBufferingMode               : Drop
IncludePortInSPN                  : False
Culture                           : en-US
UICulture                         : en-US
OpenTimeout                       : 180000
CancelTimeout                     : 60000
OperationTimeout                  : 180000
IdleTimeout                       : 172800000

The sixth command disconnects the session. It uses the IdleTimeoutSec parameter to set the idle timeout to the 12-hour maximum.
PS C:\>Disconnect-PSSession $s -IdleTimeoutSec 43200
Id Name            ComputerName    State         ConfigurationName     Availability
-- ----            ------------    -----         -----------------     ------------
 4 ITTask          Server01        Disconnected  Microsoft.PowerShell          None

The seventh command gets the value of the IdleTimeout property of the disconnected session, which is measured in milliseconds. The output confirms that the command was successful.
PS C:\>$s.Runspace.ConnectionInfo.IdleTimeout
43200000

```
This example shows how to correct the value of the IdleTimeout property of a session so that it can be disconnected.
The idle timeout property of a session is critical to disconnected sessions, because it determines how long a disconnected session is maintained before it is deleted. You can set the idle timeout option when you create a session and when you disconnect it. The default values for the idle timeout of a session are set in the $PSSessionOption preference variable on the local computer and in the session configuration on the remote computer. Values set for the session take precedence over values set in the session configuration, but session values cannot exceed quotas set in the session configuration, such as the MaxIdleTimeoutMs value.



### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289575)
[Connect-PSSession]()
[Enter-PSSession]()
[Exit-PSSession]()
[Get-PSSession]()
[Get-PSSessionConfiguration]()
[New-PSSession]()
[New-PSSessionOption]()
[New-PSTransportOption]()
[Receive-PSSession]()
[Register-PSSessionConfiguration]()
[Remove-PSSession]()
[about_PSSessions]()
[about_Remote]()
[about_Remote_Disconnected_Sessions]()

## Enable-PSRemoting

### SYNOPSIS
Configures the computer to receive remote commands.

### DESCRIPTION
The Enable-PSRemoting cmdlet configures the computer to receive Windows PowerShell remote commands that are sent by using the WS-Management technology.
On Windows Server® 2012, Windows PowerShell remoting is enabled by default. You can use Enable-PSRemoting to enable Windows PowerShell remoting on other supported versions of Windows and to re-enable remoting on Windows Server 2012 if it becomes disabled.
You need to run this command only once on each computer that will receive commands. You do not need to run it on computers that only send commands. Because the configuration activates listeners, it is prudent to run it only where it is needed.
Beginning in Windows PowerShell 3.0, the Enable-PSRemoting cmdlet can enable Windows PowerShell remoting on client versions of Windows when the computer is on a public network. For more information, see the description of the SkipNetworkProfileCheck parameter.
The Enable-PSRemoting cmdlet performs the following operations:
-- Runs the [Set-WSManQuickConfig]() cmdlet, which performs the following tasks:
----- Starts the WinRM service.
----- Sets the startup type on the WinRM service to Automatic.
----- Creates a listener to accept requests on any IP address.
----- Enables a firewall exception for WS-Management communications.
----- Registers the Microsoft.PowerShell and Microsoft.PowerShell.Workflow session configurations, if it they are not already registered.
----- Registers the Microsoft.PowerShell32 session configuration on 64-bit computers, if it is not already registered.
----- Enables all session configurations.
----- Changes the security descriptor of all session configurations to allow remote access.
----- Restarts the WinRM service to make the preceding changes effective.
To run this cmdlet, start Windows PowerShell with the "Run as administrator" option.
CAUTION: On systems that have both Windows PowerShell 3.0 and the Windows PowerShell 2.0 engine, do not use Windows PowerShell 2.0 to run the Enable-PSRemoting and Disable-PSRemoting cmdlets. The commands might appear to succeed, but the remoting is not configured correctly. Remote commands, and later attempts to enable and disable remoting, are likely to fail.

### PARAMETERS

#### Force [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Suppresses all user prompts. By default, you are prompted to confirm each operation.

#### SkipNetworkProfileCheck [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Enables remoting on client versions of Windows when the computer is on a public network. This parameter enables a firewall rule for public networks that allows remote access only from computers in the same local subnet.
This parameter has no effect on server versions of Windows, which, by default, have a local subnet firewall rule for public networks. If the local subnet firewall rule is disabled on a server version of Windows, Enable-PSRemoting re-enables it, regardless of the value of this parameter.
To remove the local subnet restriction and enable remote access from all locations on public networks, use the Set-NetFirewallRule cmdlet in the NetSecurity module. For more information, see Notes and Examples.
This parameter is introduced in Windows PowerShell 3.0.

#### Confirm [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Prompts you for confirmation before running the cmdlet.Prompts you for confirmation before running the cmdlet.

#### WhatIf [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Shows what would happen if the cmdlet runs. The cmdlet is not run.Shows what would happen if the cmdlet runs. The cmdlet is not run.


### INPUTS
#### None
You cannot pipe input to this cmdlet.

### OUTPUTS
#### System.String
Enable-PSRemoting returns strings that describe its results.

### NOTES
In Windows PowerShell 3.0, Enable-PSRemoting creates the following firewall exceptions for WS-Management communications.
On server versions of Windows, Enable-PSRemoting creates firewall rules  for private and domain networks that allow remote access, and creates a firewall rule for public networks that allows remote access only from computers in the same local subnet.
 On client versions of Windows, Enable-PSRemoting in Windows PowerShell 3.0 creates firewall rules for private and domain networks that allow unrestricted remote access. To create a firewall rule for public networks that allows remote access from the same local subnet, use the SkipNetworkProfileCheck parameter.
On client or server versions of Windows, to create a firewall rule for public networks that removes the local subnet restriction and allows remote access , use the Set-NetFirewallRule cmdlet in the NetSecurity module to run the following command: Set-NetFirewallRule -Name "WINRM-HTTP-In-TCP-PUBLIC" -RemoteAddress Any
In Windows PowerShell 2.0, Enable-PSRemoting creates the following firewall exceptions for WS-Management communications. 
On server versions of Windows, it creates firewall rules for all networks that allow remote access.
 On client versions of Windows, Enable-PSRemoting in Windows PowerShell 2.0 creates a firewall exception only for domain and private network locations. To minimize security risks, Enable-PSRemoting does not create a firewall rule for public networks on client versions of Windows. When the current network location is public, Enable-PSRemoting returns the following message: "Unable to check the status of the firewall."
Beginning in Windows PowerShell 3.0, Enable-PSRemoting enables all session configurations by setting the value of the Enabled property of all session configurations (WSMan:\<ComputerName>\Plugin\<SessionConfigurationName>\Enabled) to True ($true).
In Windows PowerShell 2.0, Enable-PSRemoting removes the Deny_All setting from the security descriptor of session configurations. In Windows PowerShell 3.0, Enable-PSRemoting removes the Deny_All and Network_Deny_All settings, thereby providing remote access to session configurations that were reserved for local use.

### EXAMPLES
#### -------------------------- EXAMPLE 1 --------------------------

```powershell
PS C:\>Enable-PSRemoting

```
This command configures the computer to receive remote commands.






#### -------------------------- EXAMPLE 2 --------------------------

```powershell
PS C:\>Enable-PSRemoting -Force

```
This command configures the computer to receive remote commands. It uses the Force parameter to suppress the user prompts.






#### Example 3

```powershell
PS C:\>Enable-PSRemoting -SkipNetworkProfileCheck -Force

PS C:\>Set-NetFirewallRule -Name "WINRM-HTTP-In-TCP-PUBLIC" -RemoteAddress Any


```
This example shows how to allow remote access from public networks on client versions of Windows. Before using these commands, analyze the security setting and verify that the computer network will be safe from harm.
The first command enables remoting in Windows PowerShell. By default, this creates network rules that allow remote access from private and domain networks. The command uses the SkipNetworkProfileCheck parameter to allow remote access from public networks in the same local subnet. The command uses the Force parameter to suppress confirmation messages.
The SkipNetworkProfileCheck parameter has no effect on server version of Windows, which allow remote access from public networks in the same local subnet by default.
The second command eliminates the subnet restriction. The command uses the Set-NetFirewallRule cmdlet in the NetSecurity module to add a firewall rule that allows remote access from public networks from any remote location, including locations in different subnets.



### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289576)
[Disable-PSSessionConfiguration]()
[Enable-PSSessionConfiguration]()
[Get-PSSessionConfiguration]()
[Register-PSSessionConfiguration]()
[Set-PSSessionConfiguration]()
[Disable-PSRemoting]()
[WSMan Provider]()
[about_Remote]()
[about_Session_Configurations]()

## Enable-PSSessionConfiguration

### SYNOPSIS
Enables the session configurations on the local computer.

### DESCRIPTION
The Enable-PSSessionConfiguration cmdlet enables registered session configurations that have been disabled, such as by using the Disable-PSSessionConfiguration or Disable-PSRemoting cmdlets, or the AccessMode parameter of Register-PSSessionConfiguration. This is an advanced cmdlet that is designed to be used by system administrators to manage customized session configurations for their users.
Without parameters, Enable-PSSessionConfiguration enables the Microsoft.PowerShell configuration, which is the default configuration that is used for sessions.
Enable-PSSessionConfiguration removes the "Deny_All" setting from the security descriptor of the affected session configurations, turns on the listener that accepts requests on any IP address, and restarts the WinRM service. Beginning in Windows PowerShell 3.0, Enable-PSSessionConfiguration also sets the value of the Enabled property of the session configuration (WSMan:\<computer>\PlugIn\<SessionConfigurationName>\Enabled) to "True". However,  Enable-PSSessionConfiguration does not remove or change the "Network_Deny_All" (AccessMode=Local) security descriptor setting that allows only users of the local computer to use to the session configuration.
The Enable-PSSessionConfiguration cmdlet calls the Set-WSManQuickConfig cmdlet. However, it should not be used to enable remoting on the computer. Instead, use the more comprehensive cmdlet, Enable-PSRemoting. 

### PARAMETERS

#### Force [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Suppresses all user prompts, and restarts the WinRM service without prompting. Restarting the service makes the configuration change effective.
To prevent a restart and suppress the restart prompt, use the NoServiceRestart parameter.

#### Name [String[]]

```powershell
[Parameter(
  Position = 1,
  ParameterSetName = 'Set 1')]
```

Specifies the names of session configurations to enable. Enter one or more configuration names. Wildcards are permitted.
You can also pipe a string that contains a configuration name or a session configuration object to Enable-PSSessionConfiguration.
If you omit this parameter, Enable-PSSessionConfiguration enables the Microsoft.PowerShell session configuration.

#### SecurityDescriptorSddl [String]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Replaces the security descriptor on the session configuration with the specified security descriptor.
If you omit this parameter, Enable-PSSessionConfiguration just deletes the "deny all" item from the security descriptor.

#### SkipNetworkProfileCheck [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Enables the session configuration when the computer is on a public network. This parameter enables a firewall rule for public networks that allows remote access only from computers in the same local subnet. By default, Enable-PSSessionConfiguration fails on a public network.
This parameter is designed for client versions of Windows. Server versions of Windows have a local subnet firewall rule for public networks by default. However, if the local subnet firewall rule is disabled on a server version of Windows, this parameter re-enables it.
To remove the local subnet restriction and enable remote access from all locations on public networks, use the Set-NetFirewallRule cmdlet in the NetSecurity module. For more information, see Enable-PSRemoting.
This parameter is introduced in Windows PowerShell 3.0.

#### Confirm [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Prompts you for confirmation before running the cmdlet.Prompts you for confirmation before running the cmdlet.

#### WhatIf [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Shows what would happen if the cmdlet runs. The cmdlet is not run.Shows what would happen if the cmdlet runs. The cmdlet is not run.


### INPUTS
#### Microsoft.PowerShell.Commands.PSSessionConfigurationCommands#PSSessionConfiguration, System.String
You can pipe a session configuration object or a string that contains the name of a session configuration to Enable-PSSessionConfiguration.

### OUTPUTS
#### None
This cmdlet does not return any objects.

### NOTES
To run this cmdlet on Windows Vista, Windows Server 2008, and later versions of Windows, you must start Windows PowerShell with the "Run as administrator" option.


### EXAMPLES
#### -------------------------- EXAMPLE 1 --------------------------

```powershell
PS C:\>Enable-PSSessionConfiguration

```
This command re-enables the Microsoft.PowerShell default session configuration on the computer.




#### -------------------------- EXAMPLE 2 --------------------------

```powershell
PS C:\>Enable-PSSessionConfiguration -name MaintenanceShell, AdminShell

```
This command re-enables the MaintenanceShell and AdminShell session configurations on the computer.




#### -------------------------- EXAMPLE 3 --------------------------

```powershell
PS C:\>Enable-PSSessionConfiguration -name *
PS C:\>Get-PSSessionConfiguration | Enable-PSSessionConfiguration

```
These commands re-enable all session configurations on the computer. The commands are equivalent, so you can use either one.
Enable-PSSessionConfiguration does not generate an error if you enable a session configuration that is already enabled.




#### -------------------------- EXAMPLE 4 --------------------------

```powershell
PS C:\>Enable-PSSessionConfiguration -name MaintenanceShell -securityDescriptorSDDL "O:NSG:BAD:P(A;;GXGWGR;;;BA)(A;;GAGR;;;S-1-5-21-123456789-188441444-3100496)S:P"

```
This command re-enables the MaintenanceShell session configuration and specifies a new security descriptor for the configuration.





### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289577)
[Disable-PSSessionConfiguration]()
[Enable-PSSessionConfiguration]()
[Get-PSSessionConfiguration]()
[New-PSSessionConfigurationFile]()
[New-PSSessionOption]()
[Register-PSSessionConfiguration]()
[Set-PSSessionConfiguration]()
[Test-PSSessionConfigurationFile]()
[Unregister-PSSessionConfiguration]()
[WSMan Provider]()
[about_Session_Configurations]()
[about_Session_Configuration_Files]()

## Enter-PSHostProcess

### SYNOPSIS
Connects to and enters into an interactive session with a local process.

### DESCRIPTION
Enter-PSHostProcess connects to and enters into an interactive session with a local process.
Instead of creating a new process to host Windows PowerShell and run a remote session, the remote, interactive session is run in an existing process that is already running Windows PowerShell. When you are interacting with a remote session on a specified process, you can enumerate running runspaces, and then select a runspace to debug by running either Debug-Runspace or Enable-RunspaceDebug.
The process that you want to enter must be hosting Windows PowerShell (System.Management.Automation.dll). You must be either a member of the Administrators group on the computer on which the process is found, or you must be the user who is running the script that started the process.
After you have selected a runspace to debug, a remote debug session is opened for the runspace if it is either currently running a command or is stopped in the debugger. You can then debug the runspace script in the same way you would debug other remote session scripts. 
Detach from a debugging session, and then the interactive session with the process, by running exit twice, or stop script execution by running the existing debugger quit command.
If you specify a process by using the Name parameter, and there is only one process found with the specified name, the process is entered. If more than one process with the specified name is found, Windows PowerShell returns an error, and lists all processes found with the specified name.
To support attaching to processes on remote computers, the Enter-PSHostProcess cmdlet is enabled in a specified remote computer, so that you can attach to a local process within a remote Windows PowerShell session.

### PARAMETERS

#### AppDomainName [String]

```powershell
[Parameter(Position = 2)]
```



#### HostProcessInfo [PSHostProcessInfo]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipeline = $true,
  ParameterSetName = 'Set 3')]
```



#### Id [Int32]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 1')]
```

Specifies a process by the process ID. To get a process ID, run the Get-Process cmdlet.

#### Name [String]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 4')]
```

Specifies a process by the process name. To get a process name, run the Get-Process cmdlet. You can also get process names from the Properties dialog box of a process in Task Manager.

#### Process [Process]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipeline = $true,
  ParameterSetName = 'Set 2')]
```

Specifies a process by the process object. The simplest way to use this parameter is to save the results of a Get-Process command that returns process that you want to enter in a variable, and then specify the variable as the value of this parameter.


### INPUTS
#### System.Diagnostics.Process


### OUTPUTS
#### 


### NOTES
Enter-PSHostProcess cannot enter the process of the Windows PowerShell session in which you are running the command. You can, however, enter the process of another Windows PowerShell session, or a Windows PowerShell ISE session that is running at the same time as the session in which you are running Enter-PSHostProcess.
Enter-PSHostProcess can enter only those processes that are hosting Windows PowerShell; that is, they have loaded the Windows PowerShell engine.
To exit a process from within the process, type exit, and then press Enter.

### EXAMPLES
#### Example 1: Start debugging a runspace within the Windows PowerShell ISE process

```powershell
In this example, you run Enter-PSHostProcess from within the Windows PowerShell console to enter the Windows PowerShell ISE process. In the resulting interactive session, you can find a runspace that you want to debug by running Get-Runspace, and then debug the runspace.
PS C:\>Enter-PSHostProcess -Name powershell_ise
[Process:1520]: PS C:\Test\Documents>

Next, get available runspaces within the process you have entered.
PS C:\>[Process:1520]: PS C:\> Get-Runspace
Id    Name          InstanceId                               State           Availability
--    -------       -----------                              ------          -------------
1     Runspace1     2d91211d-9cce-42f0-ab0e-71ac258b32b5     Opened          Available
2     Runspace2     a3855043-cb16-424a-a616-685360c3763b     Opened          RemoteDebug
3     MyLocalRS     2236dbd8-2105-4dec-a15a-a27d0bfaacb5     Opened          LocalDebug
4     MyRunspace    771356e9-8c44-4b70-9de5-dd17cb41e48e     Opened          Busy
5     Runspace8     3e517382-a97a-49ba-9c3c-fd21f6664288     Broken          None

The runspace objects returned by Get-Runspace also have a NoteProperty called ScriptStackTrace of the running command stack, if available.Next, debug runspace ID 4, that is running another user’s long-running script. From the list returned from Get-Runspace, note that the runspace state is Opened, and Availability is Busy, meaning that the runspace is still running the long-running script.
PS C:\>[Process:1520]: PS C:\> (Get-Runspace -Id 4).ScriptStackTrace
Command                    Arguments                           Location
-------                    ---------                           --------
MyModuleWorkflowF1         {}                                  TestNoFile3.psm1: line 6
WFTest1                    {}                                  TestNoFile2.ps1: line 14
TestNoFile2.ps1            {}                                  TestNoFile2.ps1: line 22
<ScriptBlock>              {}                                  <No file>

Start an interactive debugging session with this runspace by running the Debug-Runspace cmdlet.
PS C:\>[Process: 1520]: PS C:\> Debug-Runspace -Id 4
Hit Line breakpoint on 'C:\TestWFVar1.ps1:83'

At C:\TestWFVar1.ps1:83 char:1
+ $scriptVar = "Script Variable"
+ ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

[Process: 1520]: [RSDBG: 4]: PS C:\>>

After you are finished debugging, allow the script to continue running without the debugger attached by running the exit debugger command. Alternatively, you can quit the debugger with the q or Stop commands.
PS C:\>[Process:346]: [RSDBG: 3]: PS C:\>> exit
[Process:1520]: PS C:\>

When you are finished working in the process, exit the process by running the Exit-PSHostProcess cmdlet. This returns you to the PS C:\> prompt.
PS C:\>[Process:1520]: PS C:\> Exit-PSHostProcess
PS C:\>

```




### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/?LinkID=403736)
[Get-Process]()
[Exit-PSHostProcess]()
[Debug-Runspace]()
[Enable-RunspaceDebug]()
[about_Debuggers]()

## Enter-PSSession

### SYNOPSIS
Starts an interactive session with a remote computer.

### DESCRIPTION
The Enter-PSSession cmdlet starts an interactive session with a single remote computer. During the session, the commands that you type run on the remote computer, just as though you were typing directly on the remote computer. You can have only one interactive session at a time.
Typically, you use the ComputerName parameter to specify the name of the remote computer. However, you can also use a session that you create by using the New-PSSession cmdlet for the interactive session. However, you cannot use the Disconnect-PSSession, Connect-PSSession, or Receive-PSSession cmdlets to disconnect from or re-connect to an interactive session.
To end the interactive session and disconnect from the remote computer, use the Exit-PSSession cmdlet, or type "exit".

### PARAMETERS

#### AllowRedirection [switch]

```powershell
[Parameter(ParameterSetName = 'Set 2')]
```

Allows redirection of this connection to an alternate Uniform Resource Identifier (URI). By default, redirection is not allowed.
When you use the ConnectionURI parameter, the remote destination can return an instruction to redirect to a different URI. By default, Windows PowerShell does not redirect connections, but you can use this parameter to allow it to redirect the connection.
You can also limit the number of times the connection is redirected by changing the MaximumConnectionRedirectionCount session option value. Use the  MaximumRedirection parameter of the New-PSSessionOption cmdlet or set the MaximumConnectionRedirectionCount property of the $PSSessionOption preference variable. The default value is 5.

#### ApplicationName [String]

```powershell
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 1')]
```

Specifies the application name segment of the connection URI. Use this parameter to specify the application name when you are not using the ConnectionURI parameter in the command.
The default value is the value of the $PSSessionApplicationName preference variable on the local computer. If this preference variable is not defined, the default value is WSMAN. This value is appropriate for most uses. For more information, see about_Preference_Variables.
The WinRM service uses the application name to select a listener to service the connection request. The value of this parameter should match the value of the URLPrefix property of a listener on the remote computer.

#### Authentication [AuthenticationMechanism]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
[Parameter(ParameterSetName = 'Set 2')]
```

Specifies the mechanism that is used to authenticate the user's credentials.   Valid values are "Default", "Basic", "Credssp", "Digest", "Kerberos", "Negotiate", and "NegotiateWithImplicitCredential".  The default value is "Default".
CredSSP authentication is available only in Windows Vista, Windows Server 2008, and later versions of Windows.
For information about the values of this parameter, see the description of the System.Management.Automation.Runspaces.AuthenticationMechanism enumeration in the MSDN (Microsoft Developer Network) library at http://go.microsoft.com/fwlink/?LinkId=144382.
Caution: Credential Security Support Provider (CredSSP) authentication, in which the user's credentials are passed to a remote computer to be authenticated, is designed for commands that require authentication on more than one resource, such as accessing a remote network share. This mechanism increases the security risk of the remote operation. If the remote computer is compromised, the credentials that are passed to it can be used to control the network session.

#### CertificateThumbprint [String]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
[Parameter(ParameterSetName = 'Set 2')]
```

Specifies the digital public key certificate (X509) of a user account that has permission to perform this action. Enter the certificate thumbprint of the certificate.
Certificates are used in client certificate-based authentication. They can be mapped only to local user accounts; they do not work with domain accounts.
To get a certificate, use the Get-Item or Get-ChildItem command in the Windows PowerShell Cert: drive.

#### ComputerName [String]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 1')]
```

Starts an interactive session with the specified remote computer. Enter only one computer name. The default is the local computer.
Type the NetBIOS name, the IP address, or the fully qualified domain name of the computer. You can also pipe a computer name to Enter-PSSession.
To use an IP address in the value of the ComputerName parameter, the command must include the Credential parameter. Also, the computer must be configured for HTTPS transport or the IP address of the remote computer must be included in the WinRM TrustedHosts list on the local computer. For instructions for adding a computer name to the TrustedHosts list, see "How to Add  a Computer to the Trusted Host List" in about_Remote_Troubleshooting.
Note:  In Windows Vista and later versions of Windows, to include the local computer in the value of the ComputerName parameter, you must start Windows PowerShell with the "Run as administrator" option.

#### ConfigurationName [String]

```powershell
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 1')]
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 2')]
```

Specifies the session configuration that is used for the interactive session.
Enter a configuration name or the fully qualified resource URI for a session configuration. If you specify only the configuration name, the following schema URI is prepended:  http://schemas.microsoft.com/powershell.
The session configuration for a session is located on the remote computer. If the specified session configuration does not exist on the remote computer, the command fails.
The default value is the value of the $PSSessionConfigurationName preference variable on the local computer. If this preference variable is not set, the default is Microsoft.PowerShell. For more information, see about_Preference_Variables.

#### ConnectionUri [Uri]

```powershell
[Parameter(
  Position = 2,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 2')]
```

Specifies a Uniform Resource Identifier (URI) that defines the connection endpoint for the session. The URI must be fully qualified.  The format of this string is as follows:
<Transport>://<ComputerName>:<Port>/<ApplicationName>
The default value is as follows:
http://localhost:5985/WSMAN
If you do not specify a ConnectionURI, you can use the UseSSL, ComputerName, Port, and ApplicationName parameters to specify the ConnectionURI values.
Valid values for the Transport segment of the URI are HTTP and HTTPS. If you specify a connection URI with a Transport segment, but do not specify a port, the session is created with standards ports: 80 for HTTP and 443 for HTTPS. To use the default ports for Windows PowerShell remoting, specify port 5985 for HTTP or 5986 for HTTPS.
If the destination computer redirects the connection to a different URI, Windows PowerShell prevents the redirection unless you use the AllowRedirection parameter in the command.

#### Credential [PSCredential]

```powershell
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 1')]
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 2')]
```

Specifies a user account that has permission to perform this action. The default is the current user.
Type a user name, such as "User01", "Domain01\User01", or "User@Domain.com", or enter a PSCredential object, such as one returned by the Get-Credential cmdlet.
When you type a user name, you will be prompted for a password.

#### EnableNetworkAccess [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
[Parameter(ParameterSetName = 'Set 2')]
```

Adds an interactive security token to loopback sessions. The interactive token lets you run commands in the loopback session that get data from other computers. For example, you can run a command in the session that copies XML files from a remote computer to the local computer.
A "loopback session" is a PSSession that originates and terminates on the same computer. To create a loopback session, omit the ComputerName parameter or set its value to ".", "localhost", or the name of the local computer.
By default, loopback sessions are created with a network token, which might not provide sufficient permission to authenticate to remote computers.
The EnableNetworkAccess parameter is effective only in loopback sessions. If you use the EnableNetworkAccess parameter when creating a session on a remote computer, the command succeeds, but the parameter is ignored.
You can also allow remote access in a loopback session by using the CredSSP value of the Authentication parameter, which delegates the session credentials to other computers.
This parameter is introduced in Windows PowerShell 3.0.

#### Id [Int32]

```powershell
[Parameter(
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 3')]
```

Specifies the ID of an existing session. Enter-PSSession uses the specified session for the interactive session.
To find the ID of a session, use the Get-PSSession cmdlet.

#### InstanceId [Guid]

```powershell
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 4')]
```

Specifies the instance ID of an existing session. Enter-PSSession uses the specified session for the interactive session.
The instance ID is a GUID. To find the instance ID of a session, use the Get-PSSession cmdlet. You can also use the Session, Name, or ID parameters to specify an existing session.  Or, you can use the ComputerName parameter to start a temporary session.

#### Name [String]

```powershell
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 5')]
```

Specifies the friendly name of an existing session. Enter-PSSession uses the specified session for the interactive session.
If the name that you specify matches more than one session, the command fails. You can also use the Session, InstanceID, or ID parameters to specify an existing session. Or, you can use the ComputerName parameter to start a temporary session.
To establish a friendly name for a session, use the Name parameter of the New-PSSession cmdlet.

#### Port [Int32]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Specifies the network port  on the remote computer used for this command. To connect to a remote computer, the remote computer must be listening on the port that the connection uses.  The default ports are 5985 (the WinRM port for HTTP) and 5986 (the WinRM port for HTTPS).
Before using an alternate port, you must configure the WinRM listener on the remote computer to listen at that port. Use the following commands to configure the listener:
1. winrm delete winrm/config/listener?Address=*+Transport=HTTP
2. winrm create winrm/config/listener?Address=*+Transport=HTTP @{Port="<port-number>"}

Do not use the Port parameter unless you must. The port setting in the command applies to all computers or sessions on which the command runs. An alternate port setting might prevent the command from running on all computers.

#### Session [PSSession]

```powershell
[Parameter(
  Position = 1,
  ParameterSetName = 'Set 6')]
```

Specifies a Windows PowerShell session (PSSession) to use for the interactive session. This parameter takes a session object. You can also use the Name, InstanceID, or ID parameters to specify a PSSession.
Enter a variable that contains a session object or a command that creates or gets a session object, such as a New-PSSession or Get-PSSession command. You can also pipe a session object to Enter-PSSession. You can submit only one PSSession with this parameter. If you enter a variable that contains more than one PSSession, the command fails.
When you use Exit-PSSession or the EXIT keyword, the interactive session ends, but the PSSession that you created remains open and available for use.

#### SessionOption [PSSessionOption]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
[Parameter(ParameterSetName = 'Set 2')]
```

Sets advanced options for the session.  Enter a SessionOption object, such as one that you create by using the New-PSSessionOption cmdlet, or a hash table in which the keys are session option names and the values are session option values.
The default values for the options are determined by the value of the $PSSessionOption preference variable, if it is set. Otherwise, the default values are established by options set in the session configuration.
The session option values take precedence over default values for sessions set in the $PSSessionOption preference variable and in the session configuration. However, they do not take precedence over maximum values, quotas or limits set in the session configuration. 
For a description of the session options, including the default values, see New-PSSessionOption. For information about the $PSSessionOption preference variable, see about_Preference_Variables (http://go.microsoft.com/fwlink/?LinkID=113248). For more information about session configurations, see about_Session_Configurations (http://go.microsoft.com/fwlink/?LinkID=145152).

#### UseSSL [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Uses the Secure Sockets Layer (SSL) protocol to establish a connection to the remote computer. By default, SSL is not used.
WS-Management encrypts all Windows PowerShell content transmitted over the network. UseSSL is an additional protection that sends the data across an HTTPS connection instead of an HTTP connection.
If you use this parameter, but SSL is not available on the port used for the command, the command fails.


### INPUTS
#### System.String or System.Management.Automation.Runspaces.PSSession
You can pipe a computer name (a string) or a session object to Enter-PSSession.

### OUTPUTS
#### None
The cmdlet does not return any output.

### NOTES
To connect to a remote computer, you must be a member of the Administrators group on the remote computer.
In Windows Vista and later versions of Windows, to start an interactive session on the local computer, you must start Windows PowerShell with the "Run as administrator" option.
When you use Enter-PSSession, your user profile on the remote computer is used for the interactive session. The commands in the remote user profile, including commands to add Windows PowerShell snap-ins and to change the command prompt, run before the remote prompt is displayed.
Enter-PSSession uses the UI culture setting on the local computer for the interactive session. To find the local UI culture, use the $UICulture automatic variable.
Enter-PSSession requires the Get-Command, Out-Default, and Exit-PSSession cmdlets. If these cmdlets are not included in the session configuration on the remote computer, the Enter-PSSession commands fails.
Unlike Invoke-Command, which parses and interprets the commands before sending them to the remote computer, Enter-PSSession sends the commands directly to the remote computer without interpretation.
If the session that you want to enter is busy processing a command, there might be a delay before Windows PowerShell responds to the Enter-PSSession command. You will be connected as soon as the session is available. To cancel the Enter-PSSession command, press CTRL+C.

### EXAMPLES
#### -------------------------- EXAMPLE 1 --------------------------

```powershell
PS C:\>Enter-PSSession
[localhost]: PS C:\>

```
This command starts an interactive session on the local computer. The command prompt changes to indicate that you are now running commands in a different session.
The commands that you enter run in the new session, and the results are returned to the default session as text.




#### -------------------------- EXAMPLE 2 --------------------------

```powershell
The first command uses the Enter-PSSession cmdlet to start an interactive session with Server01, a remote computer. When the session starts, the command prompt changes to include the computer name.
PS C:\>Enter-PSSession -Computer Server01
[Server01]: PS C:\>

The second command gets the PowerShell process and redirects the output to the Process.txt file. The command is submitted to the remote computer, and the file is saved on the remote computer.
[Server01]: PS C:\>Get-Process Powershell > C:\ps-test\Process.txt

The third command uses the Exit keyword to end the interactive session and close the connection.
[Server01]: PS C:\>exit
PS C:\>

The fourth command confirms that the Process.txt file is on the remote computer. A Get-ChildItem ("dir") command on the local computer cannot find the file.
PS C:\>dir C:\ps-test\process.txt
Get-ChildItem : Cannot find path 'C:\ps-test\process.txt' because it does not exist.
At line:1 char:4
+ dir <<<<  c:\ps-test\process.txt

```
This command shows how to work in an interactive session with a remote computer.


#### -------------------------- EXAMPLE 3 --------------------------

```powershell
PS C:\>$s = New-PSSession -ComputerName Server01
PS C:\>Enter-PSSession -Session $s
[Server01]: PS C:\>

```
These commands use the Session parameter of Enter-PSSession to run the interactive session in an existing Windows PowerShell session (PSSession).




#### -------------------------- EXAMPLE 4 --------------------------

```powershell
PS C:\>Enter-PSSession -ComputerName Server01 -Port 90 -Credential Domain01\User01
[Server01]: PS C:\>

```
This command starts an interactive session with the Server01 computer. It uses the Port parameter to specify the port and the Credential parameter to specify the account of a user with permission to connect to the remote computer.




#### -------------------------- EXAMPLE 5 --------------------------

```powershell
PS C:\>Enter-PSSession -ComputerName Server01
[Server01]: PS C:\>Exit-PSSession
PS C:\>

```
This example shows how to start and stop an interactive session. The first command uses the Enter-PSSession cmdlet to start an interactive session with the Server01 computer.
The second command uses the Exit-PSSession cmdlet to end the session. You can also use the Exit keyword to end the interactive session. Exit-PSSession and Exit have the same effect.





### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289578)
[Exit-PSSession]()
[Get-PSSession]()
[Invoke-Command]()
[New-PSSession]()
[Remove-PSSession]()
[Connect-PSSession]()
[Disconnect-PSSession]()
[Receive-PSSession]()
[about_PSSessions]()
[about_Remote]()

## Exit-PSHostProcess

### SYNOPSIS
Closes an interactive session with a local process.

### DESCRIPTION
Exit-PSHostProcess closes an interactive session with a local process that you have opened by running the Enter-PSHostProcess cmdlet. You run the Exit-PSHostProcess cmdlet from within the process, when you are finished debugging or troubleshooting a script that is running within a process.

### PARAMETERS


### INPUTS
#### None


### OUTPUTS
#### 


### NOTES

### EXAMPLES
#### Example 1: Exit a process

```powershell
PS C:\>[Process:1520]: PS C:\> Exit-PSHostProcess
PS C:\>

```
In this example, you have been working within an active process to debug a script running in a runspace within the process, as described in Enter-PSHostProcess. After you type the exit command to exit the debugger, run the Exit-PSHostProcess cmdlet to close your interactive session with the process. The cmdlet closes your session in the process, and returns you to the PS C:\> prompt.



### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/?LinkID=403737)
[Get-Process]()
[Enter-PSHostProcess]()
[Debug-Runspace]()
[Enable-RunspaceDebug]()
[about_Debuggers]()

## Exit-PSSession

### SYNOPSIS
Ends an interactive session with a remote computer.

### DESCRIPTION
The Exit-PSSession cmdlet ends interactive sessions that you started by using Enter-PSSession.
You can also use the Exit keyword to end an interactive session. The effect is the same as using Exit-PSSession.

### PARAMETERS


### INPUTS
#### None
You cannot pipe objects to Exit-PSSession.

### OUTPUTS
#### None
This cmdlet does not return any output.

### NOTES
This cmdlet takes only the common parameters.


### EXAMPLES
#### -------------------------- EXAMPLE 1 --------------------------

```powershell
PS C:\>Enter-PSSession -computername Server01
Server01\PS> Exit-PSSession
PS C:\>

```
These commands start and then stop an interactive session with the Server01 remote computer.




#### -------------------------- EXAMPLE 2 --------------------------

```powershell
PS C:\>$s = new-pssession -computername Server01
PS C:\>Enter-PSSession -session $s
Server01\PS> Exit-PSSession
PS C:\>$s
Id Name            ComputerName    State    ConfigurationName
-- ----            ------------    -----    -----------------
1  Session1        Server01        Opened   Microsoft.PowerShell

```
These commands start and stop an interactive session with the Server01 computer that uses a Windows PowerShell session (PSSession).
Because the interactive session was started by using a Windows PowerShell session (PSSession), the PSSession is still available when the interactive session ends. If you use the ComputerName parameter, Enter-PSSession creates a temporary session that it closes when the interactive session ends.
The first command uses the New-PSSession cmdlet to create a PSSession on the Server01 computer. The command saves the PSSession in the $s variable.
The second command uses the Enter-PSSession cmdlet to start an interactive session using the PSSession in $s.
The third command uses the Exit-PSSession cmdlet to stop the interactive session.
The final command displays the PSSession in the $s variable. The State property shows the PSSession is still open and available for use.






#### -------------------------- EXAMPLE 3 --------------------------

```powershell
PS C:\>Enter-PSSession -computername Server01
Server01\PS> exit
PS C:\>

```
This command uses the Exit keyword to stop an interactive session started by using the Enter-PSSession cmdlet. The Exit keyword has the same effect as using Exit-PSSession.





### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289579)
[Connect-PSSession]()
[Disconnect-PSSession]()
[Enter-PSSession]()
[Get-PSSession]()
[Invoke-Command]()
[New-PSSession]()
[Receive-PSSession]()
[Remove-PSSession]()
[about_PSSessions]()
[about_Remote]()

## Export-Console

### SYNOPSIS
Exports the names of snap-ins in the current session to a console file.

### DESCRIPTION
The Export-Console cmdlet exports the names of the Windows PowerShell snap-ins in the current session to a Windows PowerShell console file (.psc1). You can use this cmdlet to save the snap-ins for use in future sessions.
To add the snap-ins in the .psc1 console file to a session, start Windows PowerShell (Powershell.exe) at the command line by using Cmd.exe or another Windows PowerShell session, and then use the PSConsoleFile parameter of Powershell.exe to specify the console file.
For more information about Windows PowerShell snap-ins, see about_PSSnapins.

### PARAMETERS

#### Force [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Overwrites the data in a console file without warning, even if the file has the read-only attribute. The read-only attribute is changed and is not reset when the command completes.

#### NoClobber [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Will not overwrite (replace the contents of) an existing console file. By default, if a file exists in the specified path, Export-Console overwrites the file without warning.

#### Path [String]

```powershell
[Parameter(
  Position = 1,
  ParameterSetName = 'Set 1')]
```

Specifies a path and file name for the console file (*.psc1). Enter a path (optional) and name. Wildcards are not permitted.
If you type only a file name, Export-Console creates a file with that name and the ".psc1" file name extension in the current directory.
This parameter is required unless you have opened Windows PowerShell with the PSConsoleFile parameter or exported a console file during the current session. It is also required when you use the NoClobber parameter to prevent the current console file from being overwritten.
If you omit this parameter, Export-Console overwrites (replaces the content of) the console file that was used most recently in this session. The path to the most recently used console file is stored in the value of the $ConsoleFileName automatic variable. For more information, see about_Automatic_Variables.

#### Confirm [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Prompts you for confirmation before running the cmdlet.Prompts you for confirmation before running the cmdlet.

#### WhatIf [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Shows what would happen if the cmdlet runs. The cmdlet is not run.Shows what would happen if the cmdlet runs. The cmdlet is not run.


### INPUTS
#### System.String
You can pipe a path string to Export-Console.

### OUTPUTS
#### System.IO.FileInfo
Export-Console creates a file that contains the exported aliases.

### NOTES
When a console file (.psc1) is used to start the session, the name of the console file is automatically stored in the $ConsoleFileName automatic variable.  The value of $ConsoleFileName is updated when you use the Path parameter of Export-Console to specify a new console file. When no console file is used, $ConsoleFileName has no value ($null).
To use a Windows PowerShell console file in a new session, use the following syntax to start Windows PowerShell:
"powershell.exe -PsConsoleFile <ConsoleFile>.psc1".
You can also save Windows PowerShell snap-ins for future sessions by adding an Add-PSSnapin command to your Windows PowerShell profile. For more information, see about_Profiles.


### EXAMPLES
#### -------------------------- EXAMPLE 1 --------------------------

```powershell
PS C:\>export-console -path $pshome\Consoles\ConsoleS1.psc1

```
This command exports the names of Windows PowerShell snap-ins in the current session to the ConsoleS1.psc1 file in the Consoles subdirectory of the Windows PowerShell installation directory, $pshome.






#### -------------------------- EXAMPLE 2 --------------------------

```powershell
PS C:\>export-console

```
This command exports the names of Windows PowerShell snap-ins from current session to the Windows PowerShell console file that was most recently used in the current session. It overwrites the previous file contents.
If you have not exported a console file during the current session, you are prompted for permission to continue and then prompted for a file name.






#### -------------------------- EXAMPLE 3 --------------------------

```powershell
PS C:\>add-pssnapin NewPSSnapin
PS C:\>export-console -path NewPSSnapinConsole.psc1
PS C:\>powershell.exe -PsConsoleFile NewPsSnapinConsole.psc1

```
These commands add the NewPSSnapin Windows PowerShell snap-in to the current session, export the names of Windows PowerShell snap-ins in the current session to a console file, and then start a Windows PowerShell session with the console file.
The first command uses the Add-PSSnapin cmdlet to add the NewPSSnapin snap-in to the current session. You can only add Windows PowerShell snap-ins that are registered on your system.
The second command exports the Windows PowerShell snap-in names to the NewPSSnapinConsole.psc1 file.
The third command starts Windows PowerShell with the NewPSSnapinConsole.psc1 file. Because the console file includes the Windows PowerShell snap-in name, the cmdlets and providers in the snap-in are available in the current session.






#### -------------------------- EXAMPLE 4 --------------------------

```powershell
PS C:\>export-console -path Console01
PS C:\>notepad console01.psc1
<?xml version="1.0" encoding="utf-8"?>
<PSConsoleFile ConsoleSchemaVersion="1.0">
  <PSVersion>2.0</PSVersion>
  <PSSnapIns>
     <PSSnapIn Name="NewPSSnapin" />
  </PSSnapIns>
</PSConsoleFile>

```
This command exports the names of the Windows PowerShell snap-ins in the current session to the Console01.psc1 file in the current directory.
The second command displays the contents of the Console01.psc1 file in Notepad.






#### -------------------------- EXAMPLE 5 --------------------------

```powershell
PS C:\>powershell.exe -PSConsoleFile Console01.psc1
PS C:\>add-pssnapin MySnapin
PS C:\>export-console NewConsole.psc1
PS C:\>$consolefilename
PS C:\>add-pssnapin SnapIn03
PS C:\>export-console

```
This example shows how to use the $ConsoleFileName automatic variable to determine the console file that will be updated if you use Export-Console without a Path parameter value.
The first command uses the PSConsoleFile parameter of PowerShell.exe to open Windows PowerShell with the Console01.psc1 file.
The second command uses the Add-PSSnapin cmdlet to add the MySnapin Windows PowerShell snap-in to the current session.
The third command uses the Export-Console cmdlet to export the names of all the Windows PowerShell snap-ins in the session to the NewConsole.psc1 file.
The fourth command uses the $ConsoleFileName parameter to display the most recently used console file. The sample output shows that NewConsole.ps1 is the most recently used file.
The fifth command adds SnapIn03 to the current console.
The sixth command uses the ExportConsole cmdlet without a Path parameter. This command exports the names of all the Windows PowerShell snap-ins in the current session to the most recently used file, NewConsole.psc1.







### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289580)
[Add-PSSnapin]()
[Get-PSSnapin]()
[Remove-PSSnapin]()

## Export-ModuleMember

### SYNOPSIS
Specifies the module members that are exported.

### DESCRIPTION
The Export-ModuleMember cmdlet specifies the module members (such as cmdlets, functions, variables, and aliases) that are exported from a script module (.psm1) file, or from a dynamic module created by using the New-Module cmdlet. This cmdlet can be used only in a script module file or a dynamic module.
If a script module does not include an Export-ModuleMember command, the functions in the script module are exported, but the variables and aliases are not. When a script module includes Export-ModuleMember commands, only the members specified in the Export-ModuleMember commands are exported. You can also use Export-ModuleMember to suppress or export members that the script module imports from other modules.
An Export-ModuleMember command is optional, but it is a best practice. Even if the command confirms the default values, it demonstrates the intention of the module author.

### PARAMETERS

#### Alias [String[]]

```powershell
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 1')]
```

Specifies the aliases that are exported from the script module file. Enter the alias names. Wildcards are permitted.

#### Cmdlet [String[]]

```powershell
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 1')]
```

Specifies the cmdlets that are exported from the script module file. Enter the cmdlet names. Wildcards are permitted.
You cannot create cmdlets in a script module file, but you can import cmdlets from a binary module into a script module and re-export them from the script module.

#### Function [String[]]

```powershell
[Parameter(
  Position = 1,
  ParameterSetName = 'Set 1')]
```

Specifies the functions that are exported from the script module file. Enter the function names. Wildcards are permitted. You can also pipe function name strings to Export-ModuleMember.

#### Variable [String[]]

```powershell
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 1')]
```

Specifies the variables that are exported from the script module file. Enter the variable names (without a dollar sign). Wildcards are permitted.


### INPUTS
#### System.String
You can pipe function name strings to Export-ModuleMember.

### OUTPUTS
#### None
This cmdlet does not generate any output.

### NOTES
To exclude a member from the list of exported members, add an Export-ModuleMember command that lists all other members but omits the member that you want to exclude.


### EXAMPLES
#### -------------------------- EXAMPLE 1 --------------------------

```powershell
PS C:\>Export-ModuleMember -function * -alias *

```
This command exports the aliases defined in the script module, along with the functions defined in the script module.
To export the aliases, which are not exported by default, you must also explicitly specify the functions. Otherwise, only the aliases will be exported.






#### -------------------------- EXAMPLE 2 --------------------------

```powershell
PS C:\>Export-ModuleMember -function Get-Test, New-Test, Start-Test -alias gtt, ntt, stt

```
This command exports three aliases and three functions defined in the script module.
You can use this command format to specify the names of module members.






#### -------------------------- EXAMPLE 3 --------------------------

```powershell
PS C:\>Export-ModuleMember

```
This command specifies that no members defined in the script module are exported.
This command prevents the module members from being exported, but it does not hide the members. Users can read and copy module members or use the call operator (&) to invoke module members that are not exported.






#### -------------------------- EXAMPLE 4 --------------------------

```powershell
PS C:\>Export-ModuleMember -variable increment

```
This command exports only the $increment variable from the script module. No other members are exported.
If you want to export a variable, in addition to exporting the functions in a module, the Export-ModuleMember command must include the names of all of the functions and the name of the variable.






#### -------------------------- EXAMPLE 5 --------------------------

```powershell
PS C:\># From TestModule.psm1
function new-test
   { <function code> }
export-modulemember -function new-test
function validate-test
   { <function code> }
function start-test
   { <function code> }
set-alias stt start-test
export-modulemember -function *-test -alias stt

```
These commands show how multiple Export-ModuleMember commands are interpreted in a script module (.psm1) file.
These commands create three functions and one alias, and then they export two of the functions and the alias.
Without the Export-ModuleMember commands, all three of the functions would be exported, but the alias would not be exported. With the Export-ModuleMember commands, only the Get-Test and Start-Test functions and the STT alias are exported.






#### -------------------------- EXAMPLE 6 --------------------------

```powershell
PS C:\>new-module -script {function SayHello {"Hello!"}; set-alias Hi SayHello; Export-ModuleMember -alias Hi -function SayHello}

```
This command shows how to use Export-ModuleMember in a dynamic module that is created by using the New-Module cmdlet.
In this example, Export-ModuleMember is used to export both the "Hi" alias and the "SayHello" function in the dynamic module.






#### -------------------------- EXAMPLE 7 --------------------------

```powershell
PS C:\>function export
{
  param ([parameter(mandatory=$true)] [validateset("function","variable")] $type,
  [parameter(mandatory=$true)] $name,
  [parameter(mandatory=$true)] $value)
  if ($type -eq "function")
   {
     Set-item "function:script:$name" $value
     Export-ModuleMember $name
   }
else
   {
     Set-Variable -scope Script $name $value
     Export-ModuleMember -variable $name
   }
}
export function New-Test
   {
...
   }
function helper
{
...
}
export variable interval 0
$interval = 2

```
This example includes a function named Export that declares a function or creates a variable, and then writes an Export-ModuleMember command for the function or variable. This lets you declare and export a function or variable in a single command.
To use the Export function, include it in your script module. To export a function, type "Export" before the Function keyword.
To export a variable, use the following format to declare the variable and set its value:
export variable <variable-name> <value>
The commands in the example show the correct format. In this example, only the New-Test function and the $Interval variable are exported.







### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289581)
[Get-Module]()
[Import-Module]()
[Remove-Module]()
[about_Modules]()

## ForEach-Object

### SYNOPSIS
Performs an operation against each item in a collection of input objects.

### DESCRIPTION
The ForEach-Object cmdlet performs an operation on each item in a collection of input objects. The input objects can be piped to the cmdlet or specified by using the InputObject parameter.
Beginning in Windows PowerShell 3.0, there are two different ways to construct a ForEach-Object command.
Script block. You can use a script block to specify the operation. Within the script block, use the $_ variable to represent the current object. The script block is the value of the Process parameter. The script block can contain any Windows PowerShell script.
For example, the following command gets the value of the ProcessName property of each process on the computer.
Get-Process | ForEach-Object {$_.ProcessName}
Operation statement. You can also write a operation statement, which is much more like natural language. You can use the operation statement to specify a property value or call a method. Operation statements were introduced in Windows PowerShell 3.0.
For example, the following command also gets the value of the ProcessName property of each process on the computer.
Get-Process | ForEach-Object ProcessName
When using the script block format, in addition to using the script block that describes the operations that are performed on each input object, you can provide two additional script blocks. The Begin script block, which is the value of the Begin parameter, runs before the first input object is processed. The End script block, which is the value of the End parameter, runs after the last input object is processed.

### PARAMETERS

#### Begin [ScriptBlock]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Specifies a script block that runs before processing any input objects.

#### End [ScriptBlock]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Specifies a script block that runs after processing all input objects.

#### InputObject [PSObject]

```powershell
[Parameter(ValueFromPipeline = $true)]
```

Specifies the input objects. ForEach-Object runs the script block or operation statement on each input object. Enter a variable that contains the objects, or type a command or expression that gets the objects. When you use the InputObject parameter with ForEach-Object, instead of piping command results to ForEach-Object, the InputObject value—even if the value is a collection that is the result of a command, such as -InputObject (Get-Process)—is treated as a single object. Because InputObject cannot return individual properties from an array or collection of objects, it is recommended that if you use ForEach-Object to perform operations on a collection of objects for those objects that have specific values in defined properties, you use ForEach-Object in the pipeline, as shown in the examples in this topic.

#### Process [ScriptBlock[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 1')]
```

Specifies the operation that is performed on each input object. Enter a script block that describes the operation.

#### RemainingScripts [ScriptBlock[]]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Takes all script blocks that are not taken by the Process parameter.
This parameter is introduced in Windows PowerShell 3.0.

#### ArgumentList [Object[]]

```powershell
[Parameter(ParameterSetName = 'Set 2')]
```

Specifies the arguments to a method call.
This parameter is introduced in Windows PowerShell 3.0.

#### MemberName [String]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 2')]
```

Specifies the property to get or the method to call. Wildcard characters are permitted, but work only if the resulting string resolves to a unique value. If, for example, you run Get-Process | ForEach -MemberName *Name, and more than one member exists with a name that contains the string Name--such as the ProcessName and Name properties--the command fails.
This parameter is introduced in Windows PowerShell 3.0.

#### Confirm [switch]

Prompts you for confirmation before running the cmdlet.Prompts you for confirmation before running the cmdlet.

#### WhatIf [switch]

Shows what would happen if the cmdlet runs. The cmdlet is not run.Shows what would happen if the cmdlet runs. The cmdlet is not run.


### INPUTS
#### System.Management.Automation.PSObject
You can pipe any object to ForEach-Object.

### OUTPUTS
#### System.Management.Automation.PSObject
The objects that ForEach-Object returns are determined by the input.

### NOTES
The ForEach-Object cmdlet works much like the Foreach statement, except that you cannot pipe input to a Foreach statement. For more information about the Foreach statement, see about_Foreach (http://go.microsoft.com/fwlink/?LinkID=113229).

### EXAMPLES
#### -------------------------- EXAMPLE 1 --------------------------

```powershell
PS C:\>30000, 56798, 12432 | ForEach-Object -Process {$_/1024}
29.296875
55.466796875
12.140625

```
This command takes an array of three integers and divides each one of them by 1024.


#### -------------------------- EXAMPLE 2 --------------------------

```powershell
PS C:\>Get-ChildItem $pshome | ForEach-Object -Process {if (!$_.PSIsContainer) {$_.Name; $_.Length / 1024; "" }}


```
This command gets the files and directories in the Windows PowerShell installation directory ($pshome) and passes them to the ForEach-Object cmdlet. If the object is not a directory (the value of the PSISContainer property is false), the script block gets the name of the file, divides the value of its Length property by 1024, and adds a space ("") to separate it from the next entry.   


#### -------------------------- EXAMPLE 3 --------------------------

```powershell
PS C:\>$Events = Get-EventLog -LogName System -Newest 1000
PS C:\>$events | ForEach-Object -Begin {Get-Date} -Process {Out-File -Filepath Events.txt -Append -InputObject $_.Message} -End {Get-Date}

```
This command gets the 1000 most recent events from the System event log and stores them in the $Events variable. It then pipes the events to the ForEach-Object cmdlet. 
The Begin parameter displays the current date and time. Next, the Process parameter uses the Out-File cmdlet to create a text file named events.txt and stores the message property of each of the events in that file. Last, the End parameter is used to display the date and time after all of the processing has completed.






#### -------------------------- EXAMPLE 4 --------------------------

```powershell
PS C:\>Get-ItemProperty -Path HKCU:\Network\* | ForEach-Object {Set-ItemProperty -Path $_.PSPath -Name RemotePath -Value $_.RemotePath.ToUpper();}

```
This command changes the value of the RemotePath registry entry in all of the subkeys under the HKCU:\Network key to uppercase text. You can use this format to change the form or content of a registry entry value.
Each subkey in the Network key represents a mapped network drive that will reconnect at logon. The RemotePath entry contains the UNC path of the connected drive. For example, if you map the E: drive to \\Server\Share, there will be an E subkey of HKCU:\Network and the value of the RemotePath registry entry in the E subkey will be \\Server\Share.

The command uses the Get-ItemProperty cmdlet to get all of the subkeys of the Network key and the Set-ItemProperty cmdlet to change the value of the RemotePath registry entry in each key. In the Set-ItemProperty command, the path is the value of the PSPath property of the registry key. (This is a property of the Microsoft .NET Framework object that represents the registry key; it is not a registry entry.) The command uses the ToUpper() method of the RemotePath value, which is a string (REG_SZ).
Because Set-ItemProperty is changing the property of each key, the ForEach-Object cmdlet is required to access the property.






#### -------------------------- EXAMPLE 5 --------------------------

```powershell
PS C:\>1, 2, $null, 4 | ForEach-Object {"Hello"}
Hello
Hello
Hello
Hello

```
This example shows the effect of piping the $null automatic variable to the ForEach-Object cmdlet.
Because Windows PowerShell treats null as an explicit placeholder, the ForEach-Object cmdlet generates a value for $null, just as it does for other objects that you pipe to it.
For more information about the $null automatic variable, see about_Automatic_Variables.






#### -------------------------- EXAMPLE 6 --------------------------

```powershell
PS C:\>Get-Module -List | ForEach-Object -MemberName Path
PS C:\>Get-Module -List | Foreach Path

```
These commands gets the value of the Path property of all installed Windows PowerShell modules. They use the MemberName parameter to specify the Path property of modules.
The second command is equivalent to the first. It uses the Foreach alias of the Foreach-Object cmdlet and omits the name of the MemberName parameter, which is optional.
The ForEach-Object cmdlet is very useful for getting property values, because it gets the value without changing the type, unlike the Format cmdlets or the Select-Object cmdlet, which change the property value type.


#### -------------------------- EXAMPLE 7 --------------------------

```powershell
PS C:\>"Microsoft.PowerShell.Core", "Microsoft.PowerShell.Host" | ForEach-Object {$_.Split(".")}
PS C:\>"Microsoft.PowerShell.Core", "Microsoft.PowerShell.Host" | ForEach-Object -MemberName Split -ArgumentList "."
PS C:\>"Microsoft.PowerShell.Core", "Microsoft.PowerShell.Host" | Foreach Split "."
Microsoft
PowerShell
Core
Microsoft
PowerShell
Host

```
These commands split two dot-separated module names into their component names. The commands call the Split method of strings. The three commands use different syntax, but they are equivalent and interchangeable.
The first command uses the traditional syntax, which includes a script block and the current object operator ($_). It uses the dot syntax to specify the method and parentheses to enclose the delimiter argument.
The second command uses the MemberName parameter to specify the Split method and the ArgumentName parameter to identify the dot (".") as the split delimiter.
The third command  uses the Foreach alias of the Foreach-Object cmdlet and omits the names of the MemberName and ArgumentList parameters, which are optional.
The output of these three commands, shown below, is identical.
Split is just one of many useful methods of strings. To see all of the properties and methods of strings, pipe a string to the Get-Member cmdlet.



### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289582)

## Format-Default

### SYNOPSIS
This cmdlet is for internal system use only.

### DESCRIPTION


### PARAMETERS

#### InputObject [PSObject]

```powershell
[Parameter(
  ValueFromPipeline = $true,
  ParameterSetName = 'Set 1')]
```




### INPUTS
#### None


### OUTPUTS
#### 


### NOTES

### EXAMPLES
#### 1:

```powershell
PS C:\>


```



#### 2:

```powershell

PS C:\>


```




### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=309740)

## Get-Command

### SYNOPSIS
Gets all commands.

### DESCRIPTION
The Get-Command cmdlet gets all commands that are installed on the computer, including cmdlets, aliases, functions, workflows, filters, scripts, and applications. Get-Command gets the commands from Windows PowerShell modules and snap-ins and commands that were imported from other sessions. To get only commands that have been imported into the current session, use the ListImported parameter.
Without parameters, a "Get-Command" command gets all of the cmdlets, functions, workflows and aliases installed on the computer. A "Get-Command *" command gets all types of commands, including all of the non-Windows-PowerShell files in the Path environment variable ($env:path), which it lists in the "Application" command type. 
A Get-Command command that uses the exact name of the command (without wildcard characters) automatically imports the module that contains the command so you can use the command immediately. To enable, disable, and configure automatic importing of modules, use the $PSModuleAutoLoadingPreference preference variable. For more information, see about_Preference_Variables (http://go.microsoft.com/fwlink/?LinkID=113248).
Get-Command gets its data directly from the command code, unlike Get-Help, which gets its information from help topics.
In Windows PowerShell 2.0, Get-Command gets only commands in current session. It does not get commands from modules that are installed, but not imported. To limit Get-Command in Windows PowerShell 3.0 and later to commands in the current session, use the ListImported parameter.
Starting in Windows PowerShell 5.0, results of the Get-Command cmdlet display a Version column by default; a new Version property has been added to the CommandInfo class.

### PARAMETERS

#### All [switch]

```powershell
[Parameter(ValueFromPipelineByPropertyName = $true)]
```

Gets all commands, including commands of the same type that have the same name. By default, Get-Command gets only the commands that run when you type the command name.
For more information about the method that Windows PowerShell uses to select the command to run when multiple commands have the same name, see about_Command_Precedence (http://go.microsoft.com/fwlink/?LinkID=113214). For information about module-qualified command names and running commands that do not run by default because of a name conflict, see about_Modules (http://go.microsoft.com/fwlink/?LinkID=144311).
This parameter is introduced in Windows PowerShell 3.0.
In Windows PowerShell 2.0, Get-Command gets all commands by default.

#### ArgumentList [Object[]]

```powershell
[Parameter(Position = 2)]
```


Gets information about a cmdlet or function when it is used with the specified  parameters ("arguments").  The alias for ArgumentList is Args.
To detect dynamic parameters that are available only when certain other parameters are used, set the value of ArgumentList to the parameters that trigger the dynamic parameters.
To detect the dynamic parameters that a provider adds to a cmdlet, set the value of the ArgumentList parameter to a path in the provider drive, such as "WSMan:", "HKLM:" or "Cert:".  When the command is a Windows PowerShell provider cmdlet, enter only one path in each command; the provider cmdlets return only the dynamic parameters for the first path the value of ArgumentList. For information about the provider cmdlets, see about_Providers (http://go.microsoft.com/fwlink/?LinkID=113250).

#### CommandType [CommandTypes]

```powershell
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 2')]
```

Gets only the specified types of commands. Enter one or more command types. Use CommandType or its alias, Type. By default, Get-Command gets all cmdlets, functions, and workflows, and aliases.
Valid values are:
-- Alias: Gets the aliases of all Windows PowerShell commands. For more information, see about_Aliases.
-- All: Gets all command types. This parameter value is the equivalent of "Get-Command *".
-- Application: Gets non-Windows-PowerShell files in paths listed in the Path environment variable ($env:path), including .txt, .exe, and .dll files. For more information about the Path environment variable, see about_Environment_Variables.
-- Cmdlet: Gets all cmdlets.
-- ExternalScript: Gets all .ps1 files in the paths listed in the Path environment variable ($env:path).
-- Filter and Function: Gets all Windows PowerShell advanced and simple functions and filters.
-- Script: Gets all script blocks. To get Windows PowerShell scripts (.ps1 files), use the ExternalScript value.
-- Workflow: Gets all workflows. For more information about workflows, see Introducing Windows PowerShell Workflow.

#### FullyQualifiedModule [ModuleSpecification[]]

```powershell
[Parameter(ValueFromPipelineByPropertyName = $true)]
```

Specifies modules with names that are specified in the form of ModuleSpecification objects (described by the Remarks section of [Module Specification Constructor (Hashtable)]() on MSDN). For example, the FullyQualifiedModule parameter accepts a module name that is specified in the format @{ModuleName = "modulename"; ModuleVersion = "version_number"} or @{ModuleName = "modulename"; ModuleVersion = "version_number"; Guid = "GUID"}. ModuleName and ModuleVersion are required, but Guid is optional.
You cannot specify the FullyQualifiedModule parameter in the same command as a Module parameter; the two parameters are mutually exclusive.

#### Module [String[]]

```powershell
[Parameter(ValueFromPipelineByPropertyName = $true)]
```

Gets the commands that came from the specified modules or snap-ins. Enter the names of modules or snap-ins, or enter snap-in or module objects.
This parameter takes string values, but the value of this parameter can also be a PSModuleInfo or PSSnapinInfo object, such as the objects that the Get-Module, Get-PSSnapin, and Import-PSSession cmdlets return.
You can refer to this parameter by its name, Module, or by its alias, PSSnapin. The parameter name that you choose has no effect on the command output.

#### Name [String[]]

```powershell
[Parameter(
  Position = 1,
  ParameterSetName = 'Set 2')]
```

Gets only commands with the specified name. Enter a name or name pattern. Wildcards are permitted.
To get commands that have the same name, use the All parameter. When two commands have the same name, by default, Get-Command gets the command that runs when you type the command name.

#### Noun [String[]]

```powershell
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 1')]
```

Gets commands (cmdlets, functions, workflows, and aliases) that have names that include the specified noun. Enter one or more nouns or noun patterns. Wildcards are permitted.

#### Syntax [switch]

```powershell
[Parameter(ValueFromPipelineByPropertyName = $true)]
```

Gets only specified data about the command.
* For aliases, gets the standard name.
* For cmdlets, gets the syntax.
* For functions and filters, gets the function definition.
* For scripts and applications (files), gets the path and filename.

#### TotalCount [Int32]

```powershell
[Parameter(ValueFromPipelineByPropertyName = $true)]
```

Gets the specified number of commands. You can use this parameter to limit the output of a command.

#### Verb [String[]]

```powershell
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 1')]
```

Gets commands (cmdlets, functions, workflows, and aliases) that have names that include the specified verb. Enter one or more verbs or verb patterns. Wildcards are permitted.

#### ListImported [switch]

```powershell
[Parameter(ValueFromPipelineByPropertyName = $true)]
```

Gets only commands in the current session.
Beginning in Windows PowerShell 3.0, by default, Get-Command gets all installed commands, including, but not limited to, the commands in the current session. In Windows PowerShell 2.0, it gets only commands in the current session.
This parameter is introduced in Windows PowerShell 3.0.

#### ParameterName [String[]]

Gets commands in the session that have the specified parameters. Enter parameter names and/or parameter aliases. Wildcard are supported.
The ParameterName and ParameterType parameters search only commands in the current session.
This parameter is introduced in Windows PowerShell 3.0.

#### ParameterType [PSTypeName[]]

Gets commands in the session that have parameters of the specified type. Enter the full name or partial name of a parameter type. Wildcards are supported.
The ParameterName and ParameterType parameters search only commands in the current session.
This parameter is introduced in Windows PowerShell 3.0.


### INPUTS
#### System.String
You can pipe command names to Get-Command.

### OUTPUTS
#### System.Management.Automation.CommandInfo System.Management.Automation.AliasInfo System.Management.Automation.ApplicationInfo System.Management.Automation.CmdletInfo System.Management.Automation.FunctionInfo System.Management.Automation.WorkflowInfo
All Get-Command output types are derived from this  class. The type of object that is returned depends on the type of command that Get-Command gets.
Represents aliases.
Represents or applications and files
Represents cmdlets.
Represents functions and filters


### NOTES
When more than one command with the same name is available to the session, Get-Command returns the command that runs when you type the command name. To get commands with the same name (listed in execution order), use the All parameter. For more information, see about_Command_Precedence.
When a module is imported automatically, the effect is the same as using the Import-Module cmdlet. The module can add commands, types and formatting files, and run scripts in the session. To enable, disable, and configuration automatic importing of modules, use the $PSModuleAutoLoadingPreference preference variable. For more information, see about_Preference_Variables (http://go.microsoft.com/fwlink/?LinkID=113248).

### EXAMPLES
#### -------------------------- EXAMPLE 1 --------------------------

```powershell
PS C:\>Get-Command

```
This command gets the Windows PowerShell cmdlets, functions, and aliases that are installed on the computer.


#### -------------------------- EXAMPLE 2 --------------------------

```powershell
PS C:\>Get-Command -ListImported

```
This command uses the ListImported parameter to get only the commands in the current session.


#### -------------------------- EXAMPLE 3 --------------------------

```powershell
PS C:\>Get-Command -Type Cmdlet | Sort-Object -Property Noun | Format-Table -GroupBy Noun

```
This command gets all of the cmdlets, sorts them alphabetically by the noun in the cmdlet name, and then displays them in noun-based groups. This display can help you find the cmdlets for a task.


#### -------------------------- EXAMPLE 4 --------------------------

```powershell
PS C:\>Get-Command -Module Microsoft.PowerShell.Security, PSScheduledJob

```
This command uses the Module parameter to get the commands in the Microsoft.PowerShell.Security and  PSScheduledJob modules.


#### -------------------------- EXAMPLE 5 --------------------------

```powershell
PS C:\>Get-Command Get-AppLockerPolicy

```
This command gets information about the Get-AppLockerPolicy cmdlet. It also imports the AppLocker module, which adds all of the commands in the AppLocker module to the current session.
When a module is imported automatically, the effect is the same as using the Import-Module cmdlet. The module can add commands, types and formatting files, and run scripts in the session. To enable, disable, and configuration automatic importing of modules, use the $PSModuleAutoLoadingPreference preference variable. For more information, see about_Preference_Variables (http://go.microsoft.com/fwlink/?LinkID=113248).


#### -------------------------- EXAMPLE 6 --------------------------

```powershell
PS C:\>Get-Command Get-Childitem -Args Cert: -Syntax

```
This command uses the ArgumentList and Syntax parameters to get the syntax of the Get-ChildItem cmdlet when it is used in the Cert: drive. The Cert: drive is a Windows PowerShell drive that the Certificate Provider adds to the session.
When you compare the syntax displayed in the output with the syntax that is displayed when you omit the Args (ArgumentList) parameter, you'll see that the Certificate provider adds a dynamic parameter, CodeSigningCert, to the Get-ChildItem cmdlet.
For more information about the Certificate provider, see Certificate Provider.


#### -------------------------- EXAMPLE 7 --------------------------

```powershell
PS C:\>function Get-DynamicParameters
{
    param ($Cmdlet, $PSDrive)
    (Get-Command $Cmdlet -ArgumentList $PSDrive).ParameterSets | ForEach-Object {$_.Parameters} | Where-Object { $_.IsDynamic } | Select-Object -Property Name -Unique
}
PS C:\> Get-DynamicParameters -Cmdlet Get-ChildItem -PSDrive Cert:

Name
----
CodeSigningCert

```
The Get-DynamicParameters function in this example gets the dynamic parameters of a cmdlet. This is an alternative to the method used in the previous example. Dynamic parameter can be added to a cmdlet by another cmdlet or a provider.
The command in the example uses the Get-DynamicParameters function to get the dynamic parameters that the Certificate provider adds to the Get-ChildItem cmdlet when it is used in the Cert: drive.


#### -------------------------- EXAMPLE 8 --------------------------

```powershell
PS C:\>Get-Command *

```
This command gets all commands of all types on the local computer, including executable files in the paths of the Path environment variable ($env:path). It returns an ApplicationInfo object (System.Management.Automation.ApplicationInfo) for each file, not a FileInfo object (System.IO.FileInfo).






#### -------------------------- EXAMPLE 9 --------------------------

```powershell
PS C:\>Get-Command -ParameterName *Auth* -ParameterType AuthenticationMechanism

```
This command gets cmdlets that have a parameter whose name includes "Auth" and whose type is AuthenticationMechanism. You can use a command like this one to find cmdlets that let you specify the method that is used to authenticate the user. 
The ParameterType parameter distinguishes parameters that take an AuthenticationMechanism value from those that take an AuthenticationLevel parameter, even when they have similar names.


#### -------------------------- EXAMPLE 10 --------------------------

```powershell
PS C:\>Get-Command dir
CommandType     Name                                               ModuleName
-----------     ----                                               ----------
Alias           dir -> Get-ChildItem

```
This example shows how to use the Get-Command cmdlet with an alias. Although it is typically used on cmdlets and functions, Get-Command also gets scripts, functions, aliases, workflows, and executable files.
The output of the command shows the special view of the Name property value for aliases. The view shows the alias and the full command name.


#### -------------------------- EXAMPLE 11 --------------------------

```powershell
PS C:\>Get-Command Notepad -All | Format-Table CommandType, Name, Definition

CommandType     Name           Definition
-----------     ----           ----------
Application     notepad.exe    C:\WINDOWS\system32\notepad.exe
Application     NOTEPAD.EXE    C:\WINDOWS\NOTEPAD.EXE

```
This example uses the All parameter of the Get-Command cmdlet to show all instances of the "Notepad" command on the local computer. The All parameter is useful when there is more than one command with the same name in the session.
Beginning in Windows PowerShell 3.0, by default, when the session includes multiple commands with the same name, Get-Command gets only the command that runs when you type the command name. With the All parameter, Get-Command gets all commands with the specified name and returns them in execution precedence order. To run a command other than the first one in the list, type the fully qualified path to the command.
For more information about command precedence, see about_Command_Precedence (http://go.microsoft.com/fwlink/?LinkID=113214).


#### -------------------------- EXAMPLE 12 --------------------------

```powershell
PS C:\>(Get-Command Get-Date).ModuleName
Microsoft.PowerShell.Utility

```
This command gets the name of the snap-in or module in which the Get-Date cmdlet originated. The command uses the ModuleName property of all commands.
This command format works on commands in Windows PowerShell modules and snap-ins, even if they are not imported into the session.


#### -------------------------- EXAMPLE 13 --------------------------

```powershell
PS C:\>Get-Command -Type Cmdlet | Where-Object OutputType | Format-List -Property Name, OutputType

```
This command gets the cmdlets and functions that have an output type and the type of objects that they return.
The first part of the command gets all cmdlets. A pipeline operator (|) sends the cmdlets to the Where-Object cmdlet, which selects only the ones in which the OutputType property is populated. Another pipeline operator sends the selected cmdlet objects to the Format-List cmdlet, which displays the name and output type of each cmdlet in a list.
The OutputType property of a CommandInfo object has a non-null value only when the cmdlet code defines the OutputType attribute for the cmdlet.


#### -------------------------- EXAMPLE 14 --------------------------

```powershell
PS C:\>Get-Command -ParameterType (((Get-NetAdapter)[0]).PSTypeNames)
CommandType     Name                                               ModuleName
-----------     ----                                               ----------
Function        Disable-NetAdapter                                 NetAdapter
Function        Enable-NetAdapter                                  NetAdapter
Function        Rename-NetAdapter                                  NetAdapter
Function        Restart-NetAdapter                                 NetAdapter
Function        Set-NetAdapter                                     NetAdapter

```
This command finds cmdlets that take net adapter objects as input. You can use this command format to find the cmdlets that accept the type of objects that any command returns.
The command uses the PSTypeNames intrinsic property of all objects, which gets the types that describe the object. To get the PSTypeNames property of a net adapter, and not the PSTypeNames property of a collection of net adapters, the command uses array notation to get the first net adapter that the cmdlet returns.



### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289583)
[Export-PSSession]()
[Get-Help]()
[Get-Member]()
[Get-PSDrive]()
[Import-PSSession]()
[about_Command_Precedence]()

## Get-Help

### SYNOPSIS
Displays information about Windows PowerShell commands and concepts.

### DESCRIPTION
The Get-Help cmdlet displays information about Windows PowerShell concepts and commands, including cmdlets, functions, CIM commands, workflows, providers, aliases and scripts.
To get help for a Windows PowerShell command, type "Get-Help" followed by the command name, such as: Get-Help Get-Process. To get a list of all help topics on your system, type: Get-Help *. You can display the entire help topic or use the parameters of the Get-Help cmdlet to get selected parts of the topic, such as the syntax, parameters, or examples.
Conceptual help topics in Windows PowerShell begin with "about_", such as "about_Comparison_Operators". To see all "about_" topics, type: Get-Help about_*. To see a particular topic, type: Get-Help about_<topic-name>, such as Get-Help about_Comparison_Operators.
To get help for a Windows PowerShell provider, type "Get-Help" followed by the provider name. For example, to get help for the Certificate provider, type: Get-Help Certificate.
In addition to "Get-Help", you can also type "help" or "man", which displays one screen of text at a time, or "<cmdlet-name> -?", which is identical to Get-Help but works only for commands.
Get-Help gets the help content that it displays from help files on your computer. Without the help files, Get-Help displays only basic information about commands. Some Windows PowerShell modules come with help files. However, beginning in Windows PowerShell 3.0, the modules that come with Windows do not include help files. To download or update the help files for a module in Windows PowerShell 3.0, use the Update-Help cmdlet. 
You can also view the help topics for Windows PowerShell online in the TechNet Library. To get the online version of a help topic, use the Online parameter, such as: Get-Help Get-Process -Online. You can read all of the help topics beginning at: [http://go.microsoft.com/fwlink/?LinkID=107116]().
If you type "Get-Help" followed by the exact name of a help topic, or by a word unique to a help topic, Get-Help displays the topic contents. If you enter a word or word pattern that appears in several help topic titles, Get-Help displays a list of the matching titles. If you enter a word that does not appear in any help topic titles, Get-Help displays a list of topics that include that word in their contents. 
Get-Help can get help topics for all supported languages and locales. Get-Help first looks for help files in the locale set for Windows, then in the parent locale (such as "pt" for "pt-BR"), and then in a fallback locale. Beginning in Windows PowerShell 3.0, if Get-Help does not find help in the fallback locale, it looks for help topics in English ("en-US") before returning an error message or displaying auto-generated help.
For information about the symbols that Get-Help displays in the command syntax diagram, see [about_Command_Syntax](). For information about parameter attributes, such as Required and Position, see [about_Parameters]().
TROUBLESHOOTING NOTE: In Windows PowerShell 3.0 and 4.0, Get-Help cannot find About topics in modules unless the module is imported into the current session. This is a known issue. To get About topics in a module, import the module, either by using the Import-Module cmdlet or by running a cmdlet in the module.

### PARAMETERS

#### Category [String[]]

Displays help only for items in the specified category and their aliases. Valid values are Alias, Cmdlet, Function, Provider, Workflow, and HelpFile. Conceptual topics are in the HelpFile category.

#### Component [String[]]

Displays commands with the specified component value, such as "Exchange." Enter a component name. Wildcards are permitted.
This parameter has no effect on displays of conceptual ("About_") help.

#### Detailed [switch]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 2')]
```

Adds parameter descriptions and examples to the basic help display.
This parameter is effective only when help files are for the command are installed on the computer. It has no effect on displays of conceptual ("About_") help.

#### Examples [switch]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 3')]
```

Displays only the name, synopsis, and examples. To display only the examples, type "(Get-Help <cmdlet-name>).Examples".
This parameter is effective only when help files are for the command are installed on the computer. It has no effect on displays of conceptual ("About_") help. 

#### Full [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Displays the entire help topic for a cmdlet, including parameter descriptions and attributes, examples, input and output object types, and additional notes. 
This parameter is effective only when help files are for the command are installed on the computer. It has no effect on displays of conceptual ("About_") help.

#### Functionality [String[]]

Displays help for items with the specified functionality. Enter the functionality. Wildcards are permitted.
This parameter has no effect on displays of conceptual ("About_") help.

#### Name [String]

```powershell
[Parameter(
  Position = 1,
  ValueFromPipelineByPropertyName = $true)]
```

Gets help about the specified command or concept. Enter the name of a cmdlet, function, provider, script, or workflow, such as "Get-Member", a conceptual topic name, such as "about_Objects", or an alias, such as "ls". Wildcards are permitted in cmdlet and provider names, but you cannot use wildcards to find the names of function help and script help topics.
To get help for a script that is not located in a path that is listed in the Path environment variable, type the path and file name of the script .
If you enter the exact name of a help topic, Get-Help displays the topic contents. If you enter a word or word pattern that appears in several help topic titles, Get-Help displays a list of the matching titles. If you enter a word that does not match any help topic titles, Get-Help displays a list of topics that include that word in their contents. 
The names of conceptual topics, such as "about_Objects", must be entered in English, even in non-English versions of Windows PowerShell.

#### Online [switch]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 4')]
```

Displays the online version of a help topic in the default Internet browser. This parameter is valid only for cmdlet, function, workflow and script help topics. You cannot use the Online parameter in Get-Help commands in a remote session.
For information about supporting this feature in help topics that you write, see about_Comment_Based_Help (http://go.microsoft.com/fwlink/?LinkID=144309), and "Supporting Online Help" (http://go.microsoft.com/fwlink/?LinkID=242132), and "How to Write Cmdlet Help" (http://go.microsoft.com/fwlink/?LinkID=123415) in the MSDN (Microsoft Developer Network) library.

#### Parameter [String]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 5')]
```

Displays only the detailed descriptions of the specified parameters. Wildcards are permitted.
This parameter has no effect on displays of conceptual ("About_") help.

#### Path [String]

Gets help that explains how the cmdlet works in the specified provider path. Enter a Windows PowerShell provider path.
This parameter gets a customized version of a cmdlet help topic that explains how the cmdlet works in the specified Windows PowerShell provider path. This parameter is effective only for help about a provider cmdlet and only when the provider includes a custom version of the provider cmdlet help topic  in its help file. To use this parameter, install the help file for the module that includes the provider.
To see the custom cmdlet help for a provider path, go to the provider path location and enter a Get-Help command or, from any path location, use the Path parameter of Get-Help to specify the provider path. You can also find custom cmdlet help online in the provider help section of the help topics. For example, you can find help for the New-Item cmdlet in the Wsman:\*\ClientCertificate path (http://go.microsoft.com/fwlink/?LinkID=158676).
For more information about Windows PowerShell providers, see about_Providers (http://go.microsoft.com/fwlink/?LinkID=113250).

#### Role [String[]]

Displays help customized for the specified user role. Enter a role. Wildcards are permitted.
Enter the role that the user plays in an organization. Some cmdlets display different text in their help files based on the value of this parameter. This parameter has no effect on help for the core cmdlets.

#### ShowWindow [switch]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 6')]
```

Displays the help topic in a window for easier reading. The window includes a "Find" search feature and a "Settings" box that lets you set options for the display, including options to display only selected sections of a help topic. 
The ShowWindow parameter supports help topics for commands (cmdlets, functions, CIM commands, workflows, scripts) and conceptual "About" topics. It does not support provider help.
This parameter is introduced in Windows PowerShell 3.0.


### INPUTS
#### None
You cannot pipe objects to this cmdlet.

### OUTPUTS
#### ExtendedCmdletHelpInfo System.String MamlCommandHelpInfo
If you run Get-Help on a command that does not have a help file, Get-Help returns an ExtendedCmdletHelpInfo object that represents autogenerated help.
If you get a conceptual help topic, Get-Help returns it as a string.
If you get a command that has a help file, Get-Help returns a MamlCommandHelpInfo object.

### NOTES
Without parameters, "Get-Help" displays information about the Windows PowerShell help system.
Windows PowerShell 3.0 does not come with help files. To download and install the help files that Get-Help reads, use the Update-Help cmdlet. You can use the Update-Help cmdlet to download and install help files for the core commands that come with Windows PowerShell  and for any modules that you install. You can also use it to update the help files so that the help on your computer is never outdated.
You can also read the help topics about the commands that come with Windows PowerShell online beginning at http://go.microsoft.com/fwlink/?LinkID=107116.
Get-Help displays help in the locale set for Windows or in the fallback language for that locale. If you do not have help files for the primary or fallback locale, Get-Help behaves as though there are no help files on the computer. To get help for a different locale, use Region and Language in Control Panel to change the settings for Windows.
The full view of help (-Full) includes a table of information about the parameters. The table includes the following fields:
-- Required:  Indicates whether the parameter is required (true) or optional (false).
-- Position:  Indicates whether the parameter is named or positional (numbered). Positional parameters must appear in a specified place in the command.
-- "Named" indicates that the parameter name is required, but that the parameter can appear anywhere in the command.
-- <Number> indicates that the parameter name is optional, but when the name is omitted, the parameter must be in the place specified by the number. For example, "2" indicates that when the parameter name is omitted, the parameter must be the second (2) or only unnamed parameter in the command. When the parameter name is used, the parameter can appear anywhere in the command.
-- Default value: The parameter value that Windows PowerShell uses if you do not include the parameter in the command.
-- Accepts pipeline input: Indicates whether you can (true) or cannot (false) send objects to the parameter through a pipeline. "By Property Name" means that the pipelined object must have a property with the same name as the parameter name.
-- Accepts wildcard characters: Indicates whether the value of a parameter can include wildcard characters, such as * and ?.

### EXAMPLES
#### -------------------------- EXAMPLE 1 --------------------------

```powershell
PS C:\>Get-Help

```
This command displays help about the Windows PowerShell help system.


#### -------------------------- EXAMPLE 2 --------------------------

```powershell
PS C:\>Get-Help *

```
This command displays a list of the available help topics.


#### -------------------------- EXAMPLE 3 --------------------------

```powershell
PS C:\>Get-Help Get-Alias
PS C:\>Help Get-Alias
PS C:\>Get-Alias -?

```
These commands display basic information about the Get-Alias cmdlet. The "Get-Help" and "-?" commands display the information on a single page. The "Help" command displays the information one page at a time.


#### -------------------------- EXAMPLE 4 --------------------------

```powershell
PS C:\>Get-Help about_*

```
This command displays a list of the conceptual topics included in Windows PowerShell help. All of these topics begin with the characters "about_". To display a particular help file, type "get-help <topic-name>, for example, "Get-Help about_Signing".
This command displays the conceptual topics only when the help files for those topics are installed on the computer. For information about downloading and installing help files in Windows PowerShell 3.0, see Update-Help.


#### -------------------------- EXAMPLE 5 --------------------------

```powershell
The first command uses the Get-Help cmdlet to get help for the Get-Command cmdlet. Without help files, Get-Help display the cmdlet name, syntax and alias of Get-Command, and prompts you to use the Update-Help cmdlet to get the newest help files.
PS C:\>Get-Help Get-Command
NAME
    Get-Command


SYNTAX
   
 Get-Command [[-Name] <string[]>] [-CommandType {Alias | Function | Filter | Cmdlet | ExternalScript | Application |

    Script | All}] [[-ArgumentList] <Object[]>] [-Module <string[]>] [-Syntax] [-TotalCount <int>] [<CommonParameters>]


    Get-Command [-Noun <string[]>] [-Verb <string[]>] [[-ArgumentList] <Object[]>] [-Module <string[]>] [-Syntax] 

    [-TotalCount <int>] [<CommonParameters>]



ALIASES
    gcm 


REMARKS
    Get-Help cannot find the help files for this cmdlet on this computer.
    It is displaying only partial help. To download and install help files
    for this cmdlet, use Update-Help. 

The second command runs the Update-Help cmdlet without parameters. This command downloads help files from the Internet for all of the modules in the current session and installs them on the local computer.This command works only when the local computer is connected to the Internet. If your computer is not connected to the Internet, you might be able to install help files from a network share. For more information, see Save-Help.
PS C:\>Update-Help

Now that the help files are downloaded, we can repeat the first command in the sequence. This command gets help for the Get-Command cmdlet. The cmdlet now gets more extensive help for Get-Command and you can use the Detailed, Full, Example,  and Parameter parameters of Get-Help to customize the displays.You can use the Get-Help cmdlet as soon as the Update-Help command completes. You do not need to restart Windows PowerShell. 
PS C:\>Get-Help Get-Command


```
This example shows how to download and install new or updated help files for a module. It uses features that were introduced in Windows PowerShell 3.0. 
The example compares the help that Get-Help displays for commands when you do not have help files installed on your computer and when you do have help files. You can use the same command sequence to update the help files on your computer so that your local help content is never obsolete.
To download and install the help files for the commands that come with Windows PowerShell, and for any modules in the $pshome\Modules directory, open Windows PowerShell with the "Run as administrator" option. If you are not a member of the Administrators group on the computer, you cannot download help for these modules. However, you can use the Online parameter to open the online version of help for a command, and you can read the help for Windows PowerShell in the TechNet Library beginning at [http://go.microsoft.com/fwlink/?LinkID=107116]().


#### -------------------------- EXAMPLE 6 --------------------------

```powershell
PS C:\>Get-Help ls -Detailed

```
This command displays detailed help for the Get-ChildItem cmdlet by specifying one of its aliases, "ls." The Detailed parameter of Get-Help gets the detailed view of the help topic, which includes parameter descriptions and examples. To see the complete help topic for a cmdlet, use the Full parameter.
The Full and Detailed parameters are effective only when help files for the command are installed on the computer.


#### -------------------------- EXAMPLE 7 --------------------------

```powershell
PS C:\>Get-Help Format-Table -Full

```
This command uses the Full parameter of Get-Help to display the full view help for the Format-Table cmdlet. The full view of help includes parameter descriptions, examples, and a table of technical details about the parameters.
The Full parameter is effective only when help files for the command are installed on the computer.


#### -------------------------- EXAMPLE 8 --------------------------

```powershell
PS C:\>Get-Help Start-Service -Examples

```
This command displays examples of using the Start-Service cmdlet. It uses the Examples parameter of Get-Help to display only the Examples section of the cmdlet help topics.
The Examples parameter is effective only when help files for the command are installed on the computer.


#### -------------------------- EXAMPLE 9 --------------------------

```powershell
PS C:\>Get-Help Format-List -Parameter GroupBy

```
This command uses the Parameter parameter of Get-Help to display a  detailed description of the GroupBy parameter of the Format-List cmdlet. For detailed descriptions of all parameters of the Format-List cmdlet, type "Get-Help Format-List -Parameter *".


#### -------------------------- EXAMPLE 10 --------------------------

```powershell
PS C:\>Get-Help Add-Member -Full | Out-String -Stream | Select-String -Pattern Clixml

```
This example shows how to search for a word in particular cmdlet help topic. This command searches for the word "Clixml" in the full version of the help topic for the Add-Member cmdlet.
Because the Get-Help cmdlet generates a MamlCommandHelpInfo object, not a string, you need to use a cmdlet that transforms the help topic content into a string, such as Out-String or Out-File.


#### -------------------------- EXAMPLE 11 --------------------------

```powershell
PS C:\>Get-Help Get-Member -Online

```
This command displays the online version of the help topic for the Get-Member cmdlet.


#### -------------------------- EXAMPLE 14 --------------------------

```powershell
PS C:\>Get-Help remoting

```
This command displays a list of topics that include the word "remoting."
When you enter a word that does not appear in any topic title, Get-Help displays a list of topics that include that word.


#### -------------------------- EXAMPLE 15 --------------------------

```powershell
The first command uses the Path parameter of Get-Help to specify the provider path. This command can be entered at any path location.
PS C:\>Get-Help Get-Item -Path SQLSERVER:\DataCollection

NAME

    Get-Item


SYNOPSIS

    Gets a collection of Server objects for the local computer and any computers

    to which you have made a SQL Server PowerShell connection.

    ...

The second command uses the Set-Location cmdlet (alias = "cd") to navigate to the provider path. From that location, even without the Path parameter, the Get-Help command gets the provider-specific help for the Get-Item cmdlet.
PS C:\>cd SQLSERVER:\DataCollection
SQLSERVER:\DataCollection> Get-Help Get-Item

NAME

    Get-Item


SYNOPSIS

    Gets a collection of Server objects for the local computer and any computers

    to which you have made a SQL Server PowerShell connection.

    ...


The third command shows that a Get-Help command in a file system path, and without the Path parameter, gets the standard help for the Get-Item cmdlet.
PS C:\>Get-Item

NAME

    Get-Item


SYNOPSIS

    Gets the item at the specified location.
    ...

```
This example shows how to get help that explains how to use the Get-Item cmdlet in the DataCollection node of the Windows PowerShell SQL Server provider. The example shows two ways of getting the provider-specific help for Get-Item.
You can also get provider-specific help for cmdlets online in the section that describes the provider. For example, for provider-specific online help for the New-Item cmdlet in each WSMan provider path, see [http://go.microsoft.com/fwlink/?LinkID=158676]().


#### -------------------------- EXAMPLE 16 --------------------------

```powershell
PS C:\>Get-Help C:\PS-Test\MyScript.ps1

```
This command gets help for the MyScript.ps1 script. For information about writing help for your functions and scripts, see [about_Comment_Based_Help]().



### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289584)
[Updatable Help Status Table (http://go.microsoft.com/fwlink/?LinkID=270007)]()
[about_Command_Syntax]()
[Get-Command]()
[about_Comment_Based_Help]()
[about_Parameters]()

## Get-History

### SYNOPSIS
Gets a list of the commands entered during the current session.

### DESCRIPTION
The Get-History cmdlet gets the session history, that is, the list of commands entered during the current session. 
Windows PowerShell automatically maintains a history of each session. The number of entries in the session history is determined by the value of the $MaximumHistoryCount preference variable. Beginning in Windows PowerShell 3.0, the default value is 4096. 
You can save the session history in XML or CSV format. By default, history files are saved in the home directory, but you can save the file in any location.
For more information about the history features in Windows PowerShell, see about_History (http://go.microsoft.com/fwlink/?LinkID=113233).

### PARAMETERS

#### Count [Int32]

```powershell
[Parameter(
  Position = 2,
  ParameterSetName = 'Set 1')]
```

Displays the specified number of the most recent history entries. By, default, Get-History gets all entries in the session history. If you use both the Count and Id parameters in a command, the display ends with the command that is specified by the Id parameter.
In Windows PowerShell 2.0, by default, Get-History gets the 32 most recent entries.

#### Id [Int64[]]

```powershell
[Parameter(
  Position = 1,
  ValueFromPipeline = $true,
  ParameterSetName = 'Set 1')]
```

Specifies the ID number of an entry in the session history. Get-History gets only the specified entry. If you use both the Id and Count parameters in a command, Get-History gets the most recent entries ending with the entry specified by the Id parameter.


### INPUTS
#### Int64
You can pipe a history ID to Get-History.

### OUTPUTS
#### Microsoft.PowerShell.Commands.HistoryInfo
Get-History returns a history object for each history item that it gets.

### NOTES
The session history is a list of the commands entered during the session. The session history represents the order of execution, the status, and the start and end times of the command. As you enter each command, Windows PowerShell adds it to the history so that you can reuse it. For more information about the command history, see about_History (http://go.microsoft.com/fwlink/?LinkID=113233).
Beginning in Windows PowerShell 3.0, the default value of the $MaximumHistoryCount preference variable is 4096. In Windows PowerShell 2.0, the default value is 64. For more information about the $MaximumHistoryCount variable, see about_Preference_Variables (http://go.microsoft.com/fwlink/?LinkID=113248).

### EXAMPLES
#### -------------------------- EXAMPLE 1 --------------------------

```powershell
PS C:\>Get-History

```
This command gets the entries in the session history. The default display shows each command and its ID, which indicates the order of execution.






#### -------------------------- EXAMPLE 2 --------------------------

```powershell
PS C:\>Get-History | Where-Object {$_.CommandLine -like "*Service*"}

```
This command gets entries in the command history that include "service". The first command gets all entries in the session history. The pipeline operator (|) passes the results to the Where-Object cmdlet, which selects only the commands that include "service".






#### -------------------------- EXAMPLE 3 --------------------------

```powershell
PS C:\>Get-History -ID 7 -Count 5 | Export-Csv History.csv

```
This command gets the five most recent history entries ending with entry 7. The pipeline operator (|) passes the result to the Export-Csv cmdlet, which formats the history as comma-separated text and saves it in the History.csv file. The file includes the data that is displayed when you format the history as a list, including the status and start and end times of the command.






#### -------------------------- EXAMPLE 4 --------------------------

```powershell
PS C:\>Get-History -Count 1

```
This command gets the last (most recently entered) command in the command history. It uses the Count parameter to display just one command. By default, Get-History gets the most recent commands. This command can be abbreviated to "h -c 1" and is equivalent to pressing the up-arrow key.






#### -------------------------- EXAMPLE 5 --------------------------

```powershell
PS C:\>Get-History | Format-List -Property *

```
This command displays all of the properties of entries in the session history. The pipeline operator (|) passes the results of a Get-History command to the Format-List cmdlet, which displays all of the properties of each history entry, including the ID, status, and start and end times of the command.







### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289585)
[Add-History]()
[Clear-History]()
[Invoke-History]()
[about_History]()

## Get-Job

### SYNOPSIS
Gets Windows PowerShell background jobs that are running in the current session.

### DESCRIPTION
The Get-Job cmdlet gets objects that represent the background jobs that were started in the current session. You can use Get-Job to get jobs that were started by using the Start-Job cmdlet, or by using the AsJob parameter of any cmdlet.
Without parameters, a "Get-Job" command gets all jobs in the current session. You can use the parameters of Get-Job to get particular jobs.
The job object that Get-Job returns contains useful information about the job, but it does not contain the job results. To get the results, use the Receive-Job cmdlet.
A Windows PowerShell background job is a command that runs "in the background" without interacting with the current session. Typically, you use a background job to run a complex command that takes a long time to complete. For more information about background jobs in Windows PowerShell, see about_Jobs.
Beginning in Windows PowerShell 3.0, the Get-Job cmdlet also gets custom job types, such as workflow jobs and instances of scheduled jobs. To find the job type of a job, use the PSJobTypeName property of the job. 
To enable Get-Job to get a custom job type, import the module that supports the custom job type into the session before running a Get-Job command, either by using the Import-Module cmdlet or by using or getting a cmdlet in the module. For information about a particular custom job type, see the documentation of the custom job type feature.

### PARAMETERS

#### Command [String[]]

```powershell
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 2')]
```

Gets the jobs that include the specified command. The default is all jobs. Enter a command (as a string). You can use wildcards to specify a command pattern.

#### Filter [Hashtable]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 3')]
```

Gets jobs that satisfy all of the conditions established in the associated hash table. Enter a hash table where the keys are job properties and the values are job property values.
This parameter works only on custom job types, such as workflow jobs and scheduled jobs. It does not work on standard background jobs, such as those created by using the Start-Job cmdlet. For information about support for this parameter, see the help topic for the job type.
This parameter is introduced in Windows PowerShell 3.0.

#### Id [Int32[]]

```powershell
[Parameter(
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 1')]
```

Gets only jobs with the specified IDs.
The ID is an integer that uniquely identifies the job within the current session. It is easier to remember and to type than the instance ID, but it is unique only within the current session. You can type one or more IDs (separated by commas). To find the ID of a job, type "Get-Job" without parameters.

#### IncludeChildJob [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
[Parameter(ParameterSetName = 'Set 2')]
[Parameter(ParameterSetName = 'Set 4')]
[Parameter(ParameterSetName = 'Set 5')]
[Parameter(ParameterSetName = 'Set 6')]
```

Returns child jobs, in addition to parent jobs.
This parameter is particularly useful for investigating workflow jobs, for which Get-Job returns a container parent job, and job failures, because the reason for the failure is saved in a property of the child job.
This parameter is introduced in Windows PowerShell 3.0.

#### InstanceId [Guid[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 6')]
```

Gets jobs with the specified instance IDs. The default is all jobs.
An instance ID is a GUID that uniquely identifies the job on the computer. To find the instance ID of a job, use Get-Job.

#### Name [String[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 5')]
```

Gets the job with the specified friendly names. Enter a job name, or use wildcard characters to enter a job name pattern. By default, Get-Job gets all jobs in the current session.

#### State [JobState]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 4')]
```

Gets only jobs in the specified state. Valid values are NotStarted, Running, Completed, Failed, Stopped, Blocked, Suspended, Disconnected, Suspending, Stopping. By default, Get-Job gets all the jobs in the current session.
For more information about job states, see "JobState Enumeration" in MSDN at [http://msdn.microsoft.com/en-us/library/windows/desktop/system.management.automation.jobstate(v=vs.85).aspx]()

#### After [DateTime]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
[Parameter(ParameterSetName = 'Set 2')]
[Parameter(ParameterSetName = 'Set 4')]
[Parameter(ParameterSetName = 'Set 5')]
[Parameter(ParameterSetName = 'Set 6')]
```

Gets completed jobs that ended after the specified date and time. Enter a DateTime object, such as one returned by the Get-Date cmdlet or a string that can be converted to a DateTime object, such as "Dec 1, 2012 2:00 AM" or "11/06".
This parameter works only on custom job types, such as workflow jobs and scheduled jobs, that have an EndTime property. It does not work on standard background jobs, such as those created by using the Start-Job cmdlet. For information about support for this parameter, see the help topic for the job type.
This parameter is introduced in Windows PowerShell 3.0.

#### Before [DateTime]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
[Parameter(ParameterSetName = 'Set 2')]
[Parameter(ParameterSetName = 'Set 4')]
[Parameter(ParameterSetName = 'Set 5')]
[Parameter(ParameterSetName = 'Set 6')]
```

Gets completed jobs that ended before the specified date and time. Enter a DateTime object, such as one returned by the Get-Date cmdlet or a string that can be converted to a DateTime object, such as "Dec 1, 2012 2:00 AM" or "11/06".
This parameter works only on custom job types, such as workflow jobs and scheduled jobs, that have an EndTime property. It does not work on standard background jobs, such as those created by using the Start-Job cmdlet. For information about support for this parameter, see the help topic for the job type.
This parameter is introduced in Windows PowerShell 3.0.

#### ChildJobState [JobState]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
[Parameter(ParameterSetName = 'Set 2')]
[Parameter(ParameterSetName = 'Set 4')]
[Parameter(ParameterSetName = 'Set 5')]
[Parameter(ParameterSetName = 'Set 6')]
```

Gets only the child jobs that have the specified state. Valid values are: NotStarted, Running, Completed, Failed, Stopped, Blocked, Suspended, Disconnected, Suspending, and Stopping.
By default, Get-Job does not get child jobs. With the IncludeChildJob parameter, Get-Job gets all child jobs. If you use the ChildJobState parameter, the IncludeChildJob parameter has no effect.
This parameter is introduced in Windows PowerShell 3.0.

#### HasMoreData [Boolean]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
[Parameter(ParameterSetName = 'Set 2')]
[Parameter(ParameterSetName = 'Set 4')]
[Parameter(ParameterSetName = 'Set 5')]
[Parameter(ParameterSetName = 'Set 6')]
```

Gets only jobs with the specified HasMoreData property value. The HasMoreData property indicates whether all job results have been received in the current session. To get jobs that have more results, type -HaveMoreData:true. To get jobs that do not have more results, type -HaveMoreData:$false.
To get the results of a job, use the Receive-Job cmdlet.
When you use the Receive-Job cmdlet, it deletes from its in-memory (session-specific) storage the results that it returned. When it has returned all results of the job in the current session, it sets the value of the HasMoreData property of the job to False ($false) to indicate that it has no more results for the job in the current session. Use the Keep parameter of Receive-Job to prevent Receive-Job from deleting results and changing the value of the HasMoreData property. For more information, type "Get-Help Receive-Job".
The HasMoreData property is specific to the current session. If results for a custom job type are saved outside of the session, such as the scheduled job type, which saves job results on disk, you can use the Receive-Job cmdlet in a different session to get the job results again, even if the value of HasMoreData is false. For more information, see the help topics for the custom job type.
This parameter is introduced in Windows PowerShell 3.0.

#### Newest [Int32]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
[Parameter(ParameterSetName = 'Set 2')]
[Parameter(ParameterSetName = 'Set 4')]
[Parameter(ParameterSetName = 'Set 5')]
[Parameter(ParameterSetName = 'Set 6')]
```

Gets the jobs that ended most recently. Enter the number of jobs to return.
The Newest parameter does not sort or return the newest jobs in end-time order. To sort the output, use the Sort-Object cmdlet.
This parameter is introduced in Windows PowerShell 3.0.


### INPUTS
#### None
You cannot pipe input to this cmdlet.

### OUTPUTS
#### System.Management.Automation.RemotingJob
Get-Job returns objects that represent the jobs in the session.

### NOTES
The PSJobTypeName property of jobs indicates the job type of the job. The property value is determined by the job type author. The following list shows common job types.
--  BackgroundJob: Local job started by using the Start-Job cmdlet.
-- RemoteJob: Job started in a PSSession by using the AsJob parameter of the Invoke-Command cmdlet.
-- PSWorkflowJob: Job started by using the AsJob common parameter of workflows.

### EXAMPLES
#### -------------------------- EXAMPLE 1 --------------------------

```powershell
PS C:\>Get-Job

```
This command gets all background jobs started in the current session. It does not include jobs created in other sessions, even if the jobs run on the local computer.


#### -------------------------- EXAMPLE 2 --------------------------

```powershell
The first command uses the Get-Job cmdlet to get a job. It uses the Name parameter to identify the job. The command stores the job object that Get-Job returns in the $j variable. In this example, there is only one job with the specified name.
PS C:\>$j = Get-Job -Name Job1

The second command gets the InstanceId property of the object in the $j variable and stores it in the $ID variable.
PS C:\>$ID = $j.InstanceID

The third command displays the value of the $ID variable.
PS C:\>$ID

Guid
----
03c3232e-1d23-453b-a6f4-ed73c9e29d55

The fourth command uses Stop-Job cmdlet to stop the job. It uses the InstanceId parameter to identify the job and $ID variable to represent the instance ID of the job.
PS C:\>Stop-Job -InstanceId $ID

```
These commands show how to get the instance ID of a job and then use it to stop a job. Unlike the name of a job, which is not unique, the instance ID is unique.


#### -------------------------- EXAMPLE 3 --------------------------

```powershell
PS C:\>Get-Job -Command "*get-process*"

```
This command gets the jobs on the system that include a Get-Process command. The command uses the Command parameter of Get-Job to limit the jobs retrieved. The command uses wildcard characters (*) to get jobs that include a Get-Process command anywhere within the command string.


#### -------------------------- EXAMPLE 4 --------------------------

```powershell
PS C:\>"*get-process*" | Get-Job

```
Like the command in the  previous example, this command gets the jobs on the system that include a Get-Process command. The command uses a pipeline operator (|) to send a string (in quotation marks) to the Get-Job cmdlet. It is the equivalent of the previous command.


#### -------------------------- EXAMPLE 5 --------------------------

```powershell
PS C:\>Get-Job -State NotStarted

```
This command gets only those jobs that have been created but have not yet been started. This includes jobs that are scheduled to run in the future and those not yet scheduled.


#### -------------------------- EXAMPLE 6 --------------------------

```powershell
PS C:\>Get-Job -name Job*

```
This command gets all jobs that have job names beginning with "job". Because "job<number>" is the default name for a job, this command gets all jobs that do not have an explicitly assigned name.


#### -------------------------- EXAMPLE 7 --------------------------

```powershell
The first command uses the Start-Jobcmdlet to start a background job that runs a Get-Process command on the local computer. The command uses the Name parameter of Start-Job to assign a friendly name to the job.
PS C:\>Start-Job -ScriptBlock {Get-Process} -Name MyJob

The second command uses Get-Job to get the job. It uses the Name parameter of Get-Job to identify the job. The command saves the resulting job object in the $j variable.
PS C:\>
$j = Get-Job -Name MyJob

The third command displays the value of the job object in the $j variable. The value of the State property shows that the job is complete. The value of the HasMoreData property shows that there are results available from the job that have not yet been retrieved.
PS C:\>$j
Id     Name            PSJobTypeName   State         HasMoreData     Location             Command
--     ----            -------------   -----         -----------     --------             -------
6      MyJob           BackgroundJob   Completed     True            localhost            Get-Process

The fourth command uses the Receive-Job cmdlet to get the results of the job. It uses the job object in the $j variable to represent the job. You can also use a pipeline operator to send a job object to Receive-Job.
PS C:\>Receive-Job -Job $j
Handles  NPM(K)    PM(K)      WS(K) VM(M)   CPU(s)     Id ProcessName
-------  ------    -----      ----- -----   ------     -- -----------
    124       4    13572      12080    59            1140 audiodg
    783      16    11428      13636   100             548 CcmExec
     96       4     4252       3764    59            3856 ccmsetup
...

```
This example shows how to use Get-Job to get a job object, and then it shows how to use the job object to represent the job in a command.


#### -------------------------- EXAMPLE 8 --------------------------

```powershell
The first command uses the Start-Job cmdlet to start a job on the local computer.
PS C:\>Start-Job -ScriptBlock {Get-EventLog System}

The second command uses the AsJob parameter of the Invoke-Command cmdlet to start a job on the S1 computer. Even though the commands in the job run on the remote computer, the job object is created on the local computer, so you use local commands to manage the job.
PS C:\>Invoke-Command -ComputerName S1 -ScriptBlock {Get-EventLog System} -AsJob

The third command uses the Invoke-Command cmdlet to run a Start-Job command on the S2 computer. With this method, the job object is created on the remote computer, so you use remote commands to manage the job.
PS C:\>Invoke-Command -ComputerName S2 -ScriptBlock {Start-Job -ScriptBlock {Get-EventLog System}}

The fourth command uses Get-Job to get the jobs stored on the local computer. The PSJobTypeName property of jobs, introduced in Windows PowerShell 3.0, shows that the local job started by using the Start-Job cmdlet is a background job and the job started in a remote session by using the Invoke-Command cmdlet is a remote job..
PS C:\>Get-Job
Id     Name       PSJobTypeName   State         HasMoreData     Location        Command
--     ----       -------------   -----         -----------     --------        -------
1      Job1       BackgroundJob   Running       True            localhost       Get-EventLog System
2      Job2       RemoteJob       Running       True            S1              Get-EventLog System

The fifth command uses Invoke-Command to run a Get-Job command on the S2 computer.The sample output shows the results of the Get-Job command. On the S2 computer, the job appears to be a local job. The computer name is "localhost" and the job type is a background job.For more information about running background jobs on remote computers, see about_Remote_Jobs.
PS C:\>Invoke-Command -ComputerName S2 -ScriptBlock {Start-Job -ScriptBlock {Get-EventLog System}}
Id    Name     PSJobTypeName  State      HasMoreData   Location   Command
--    ----     -------------  -----      -----------   -------    -------
4     Job4     BackgroundJob  Running    True          localhost  Get-Eventlog System

```
This example demonstrates that the Get-Job cmdlet can get all of the jobs that were started in the current session, even if they were started by using different methods.


#### -------------------------- EXAMPLE 9 --------------------------

```powershell
The first command uses the Start-Job cmdlet to start a job on the local computer. The job object that Start-Job returns shows that the job failed. The value of the State property is "Failed".
PS C:\>Start-Job -ScriptBlock {Get-Process}
Id     Name       PSJobTypeName   State       HasMoreData     Location             Command
--     ----       -------------   -----       -----------     --------             -------
1      Job1       BackgroundJob   Failed      False           localhost            Get-Process

The second command uses the Get-Job cmdlet to get the job. The command uses the dot method to get the value of the JobStateInfo property of the object. It uses a pipeline operator to send the object in the JobStateInfo property to the Format-List cmdlet, which formats all of the properties of the object (*) in a list.The result of the Format-List command shows that the value of the Reason property of the job is blank.
PS C:\>(Get-Job).JobStateInfo | Format-List -Property *
State  : Failed
Reason :

The third command investigates further. It uses a Get-Job command to get the job and then uses a pipeline operator to send the entire job object to the Format-List cmdlet, which displays all of the properties of the job in a list.The display of all properties in the job object shows that the job contains a child job named "Job2".
PS C:\>Get-Job | Format-List -Property *
HasMoreData   : False

StatusMessage :

Location      : localhost

Command       : get-process

JobStateInfo  : Failed

Finished      : System.Threading.ManualReset

EventInstanceId    : fb792295-1318-4f5d-8ac8-8a89c5261507

Id            : 1

Name          : Job1

ChildJobs     : {Job2}

Output        : {}

Error         : {}

Progress      : {}

Verbose       : {}

Debug         : {}

Warning       : {}

StateChanged  :

The fourth command uses Get-Job to get the job object that represents the Job2 child job. This is the job in which the command actually ran. It uses the dot method to get the Reason property of the JobStateInfo property.The result shows that the job failed because of an "Access Denied" error. In this case, the user forgot to use the "Run as administrator" option when opening Windows PowerShell.Because background jobs use the remoting features of Windows PowerShell, the computer must be configured for remoting to run a job, even when the job runs on the local computer.For information about requirements for remoting in Windows PowerShell, see about_Remote_Requirements. For troubleshooting tips, see about_Remote_Troubleshooting.
PS C:\>(Get-Job -Name job2).JobStateInfo.Reason
Connecting to remote server using WSManCreateShellEx api failed. The async callback gave the following error message : Access is denied.

```
This command shows how to use the job object that Get-Job returns to investigate why a job failed. It also shows how to get the child jobs of each job.


#### -------------------------- EXAMPLE 10 --------------------------

```powershell
The first command uses the Workflow keyword to create the "WFProcess" workflow.
PS C:\>Workflow WFProcess {Get-Process}

The second command uses the AsJob parameter of the WFProcess workflow to run the workflow as a background job. It uses the JobName parameter of the workflow to specify a name for the job, and the PSPrivateMetadata parameter of the workflow to specify a custom ID.
PS C:\>WFProcess -AsJob -JobName WFProcessJob -PSPrivateMetadata @{MyCustomId = 92107} 

The third command uses the Filter parameter of Get-Job to get the job by custom ID that was specified in the PSPrivateMetadata parameter.
PS C:\>Get-Job -Filter @{MyCustomId = 92107}
Id     Name            State         HasMoreData     Location             Command

--     ----            -----         -----------     --------             -------

1      WFProcessJob    Completed     True            localhost            WFProcess

```
This example shows how to use the Filter parameter to get a workflow job. The Filter parameter, introduced in Windows PowerShell 3.0 is valid only on custom job types, such as workflow jobs and scheduled jobs.


#### -------------------------- EXAMPLE 11 --------------------------

```powershell
The first command gets the jobs in the current session. The output includes a background job, a remote job and several instances of a scheduled job. The remote job, Job4, appears to have failed.
PS C:\>Get-Job
Id     Name            PSJobTypeName   State         HasMoreData     Location             Command
--     ----            -------------   -----         -----------     --------             -------
2      Job2            BackgroundJob   Completed     True            localhost            .\Get-Archive.ps1
4      Job4            RemoteJob       Failed        True            Server01, Server02   .\Get-Archive.ps1
7      UpdateHelpJob   PSScheduledJob  Completed     True            localhost            Update-Help
8      UpdateHelpJob   PSScheduledJob  Completed     True            localhost            Update-Help
9      UpdateHelpJob   PSScheduledJob  Completed     True            localhost            Update-Help
10     UpdateHelpJob   PSScheduledJob  Completed     True            localhost            Update-Help

The second command uses the IncludeChildJob parameter of Get-Job. The output adds the child jobs of all jobs that have child jobs.In this case, the revised output shows that only the Job5 child job of Job4 failed.
PS C:\>Get-Job -IncludeChildJob
Id     Name            PSJobTypeName   State         HasMoreData     Location             Command
--     ----            -------------   -----         -----------     --------             -------
2      Job2            BackgroundJob   Completed     True            localhost           .\Get-Archive.ps1
3      Job3                            Completed     True            localhost           .\Get-Archive.ps1
4      Job4            RemoteJob       Failed        True            Server01, Server02  .\Get-Archive.ps1
5      Job5                            Failed        False           Server01            .\Get-Archive.ps1
6      Job6                            Completed     True            Server02            .\Get-Archive.ps1
7      UpdateHelpJob   PSScheduledJob  Completed     True            localhost            Update-Help
8      UpdateHelpJob   PSScheduledJob  Completed     True            localhost            Update-Help
9      UpdateHelpJob   PSScheduledJob  Completed     True            localhost            Update-Help
10     UpdateHelpJob   PSScheduledJob  Completed     True            localhost            Update-Help

The third command uses the ChildJobState parameter with a value of Failed.The output includes all parent jobs and only the child jobs that failed.
PS C:\>Get-Job -Name Job4 -ChildJobState Failed
Id     Name            PSJobTypeName   State         HasMoreData     Location             Command
--     ----            -------------   -----         -----------     --------             -------
2      Job2            BackgroundJob   Completed     True            localhost           .\Get-Archive.ps1
4      Job4            RemoteJob       Failed        True            Server01, Server02  .\Get-Archive.ps1
5      Job5                            Failed        False           Server01            .\Get-Archive.ps1
7      UpdateHelpJob   PSScheduledJob  Completed     True            localhost            Update-Help
8      UpdateHelpJob   PSScheduledJob  Completed     True            localhost            Update-Help
9      UpdateHelpJob   PSScheduledJob  Completed     True            localhost            Update-Help
10     UpdateHelpJob   PSScheduledJob  Completed     True            localhost            Update-Help

The fifth command uses the JobStateInfo property of jobs and its Reason property to find out why Job5 failed.
PS C:\>(Get-Job -Name Job5).JobStateInfo.Reason
Connecting to remote server Server01 failed with the following error message : 
Access is denied.
For more information, see the about_Remote_Troubleshooting Help topic.

```
This example shows the effect of using the IncludeChildJob and ChildJobState parameters of the Get-Job cmdlet.



### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289586)
[Invoke-Command]()
[Get-Job]()
[Receive-Job]()
[Remove-Job]()
[Resume-Job]()
[Start-Job]()
[Stop-Job]()
[Suspend-Job]()
[Wait-Job]()
[about_Jobs]()
[about_Job_Details]()
[about_Remote_Jobs]()
[about_Scheduled_Jobs]()

## Get-Module

### SYNOPSIS
Gets the modules that have been imported or that can be imported into the current session. 

### DESCRIPTION
The Get-Module cmdlet gets the Windows PowerShell modules that have been imported, or that can be imported, into a Windows PowerShell session. The module object that Get-Module returns contains valuable information about the module. You can also pipe the module objects to other cmdlets, such as the Import-Module and Remove-Module cmdlets.
Without parameters, Get-Module gets modules that have been imported into the current session. To get all installed modules, use the ListAvailable parameter.
Get-Module gets modules, but it does not import them. Beginning in Windows PowerShell 3.0, modules are automatically imported when you use a command in the module, but a Get-Module command does not trigger an automatic import. You can also import the modules into your session by using the Import-Module cmdlet. 
Beginning in Windows PowerShell 3.0, you can get (and then, import) modules from remote sessions into the local session. This strategy uses the Implicit Remoting feature of Windows PowerShell and is equivalent to using the Import-PSSession cmdlet. When you use commands in modules imported from another session, the commands run implicitly in the remote session, allowing you to manage the remote computer from the local session.
Also, beginning in Windows PowerShell 3.0, you can use Get-Module and Import-Module to get and import Common Information Model (CIM) modules, in which the cmdlets are defined in Cmdlet Definition XML (CDXML) files. This feature allows you to use cmdlets that are implemented in non-managed code assemblies, such as those written in C++.
With these new features, the Get-Module and Import-Module cmdlets become primary tools for managing heterogeneous enterprises that include Windows computers and computers that are running other operating systems. 
To manage remote Windows computers that have Windows PowerShell and Windows PowerShell remoting enabled, create a PSSession on the remote computer and then use the PSSession parameter of Get-Module to get the Windows PowerShell modules in the PSSession. When you import the modules, and then use the imported commands in the current session, the commands run implicitly in the PSSession on the remote computer. You can use this strategy to manage the remote computer.
You can use a similar strategy to manage computers that do not have Windows PowerShell remoting enabled, including computers that are not running a Windows operating system, and Windows computers that have Windows PowerShell, but do not have Windows PowerShell remoting enabled.
Begin by creating a "CIM session" on the remote computer; a connection to Windows Management Instrumentation (WMI) on the remote computer. Then use the CIMSession parameter of Get-Module to get CIM modules from the CIM session. When you import a CIM module (by using the Import-Module cmdlet) and then run the imported commands, the commands run implicitly on the remote computer. You can use this WMI and CIM strategy to manage the remote computer.

### PARAMETERS

#### All [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
[Parameter(ParameterSetName = 'Set 2')]
```

Gets all modules in each module folder, including nested modules, manifest (.psd1) files, script module (.psm1) files, and binary module (.dll) files. Without the All parameter, Get-Module gets only the default module in each module folder.

#### CimNamespace [String]

```powershell
[Parameter(ParameterSetName = 'Set 3')]
```

Specifies the namespace of an alternate CIM provider that exposes CIM modules. The default value is the namespace of the Module Discovery WMI provider.
Use this parameter to get CIM modules from computers and devices that are not running a Windows operating system.
This parameter is introduced in Windows PowerShell 3.0.

#### CimResourceUri [Uri]

```powershell
[Parameter(ParameterSetName = 'Set 3')]
```

Specifies an alternate location for CIM modules. The default value is the resource URI of the Module Discovery WMI provider on the remote computer.
Use this parameter to get CIM modules from computers and devices that are not running a Windows operating system.
This parameter is introduced in Windows PowerShell 3.0.

#### ListAvailable [switch]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 2')]
[Parameter(ParameterSetName = 'Set 3')]
[Parameter(ParameterSetName = 'Set 4')]
```

Gets all installed modules. Get-Module gets modules in paths listed in the PSModulePath environment variable. Without this parameter, Get-Module gets only the modules that are both listed in the PSModulePath environment variable, and that are loaded in the current session. ListAvailable does not return information about modules that are not found in the PSModulePath environment variable, even if those modules are loaded in the current session.

#### FullyQualifiedName [String[]]

```powershell
[Parameter(
  Position = 1,
  ValueFromPipeline = $true,
  ParameterSetName = 'Set 1')]
[Parameter(
  Position = 1,
  ValueFromPipeline = $true,
  ParameterSetName = 'Set 2')]
[Parameter(
  Position = 1,
  ValueFromPipeline = $true,
  ParameterSetName = 'Set 4')]
```

Gets modules with names that are specified in the form of ModuleSpecification objects (described by the Remarks section of [ModuleSpecification Constructor (Hashtable)]() on MSDN). For example, the FullyQualifiedName parameter accepts a module name that is specified in the format @{ModuleName = "modulename"; ModuleVersion = "version_number"} or @{ModuleName = "modulename"; ModuleVersion = "version_number"; Guid = "GUID"}. ModuleName and ModuleVersion are required, but Guid is optional.
You cannot specify the FullyQualifiedName parameter in the same command as a Name parameter; the two parameters are mutually exclusive.

#### Name [String[]]

```powershell
[Parameter(
  Position = 1,
  ValueFromPipeline = $true)]
```

Gets only modules with the specified names or name patterns. Wildcards are permitted. You can also pipe the names to Get-Module. You cannot specify the FullyQualifiedName parameter in the same command as a Name parameter; the two parameters are mutually exclusive. The Name parameter cannot accept a module GUID as a value; to return modules by specifying a GUID, use the FullyQualifiedName parameter instead of the Name parameter.

#### CimSession [CimSession]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 3')]
```

Specifies a CIM session on the remote computer. Enter a variable that contains the CIM session or a command that gets the CIM session, such as a [Get-CIMSession]() command.
Get-Module uses the CIM session connection to get modules from the remote computer. When you import the module (by using the Import-Module cmdlet) and use the commands from the imported module in the current session, the commands actually run on the remote computer.
You can use this parameter to get modules from computers and devices that are not running a Windows operating system, and Windows computers that have Windows PowerShell, but do not have Windows PowerShell remoting enabled.
The CimSession parameter gets all modules in the CIMSession. However, you can import only CIM-based and Cmdlet Definition XML (CDXML)-based modules.

#### PSSession [PSSession]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 4')]
```

Gets the modules in the specified user-managed Windows PowerShell session (PSSession). Enter a variable that contains the session, a command that gets the session, such as a Get-PSSession command, or a command that creates the session, such as a New-PSSession command.
When the session is connected to a remote computer, the ListAvailable parameter is required.
A Get-Module command with the PSSession parameter is equivalent to using the Invoke-Command cmdlet to run a Get-Module -ListAvailable command in a PSSession.
This parameter is introduced in Windows PowerShell 3.0.

#### Refresh [switch]

```powershell
[Parameter(ParameterSetName = 'Set 2')]
[Parameter(ParameterSetName = 'Set 3')]
[Parameter(ParameterSetName = 'Set 4')]
```

Refreshes the cache of installed commands. The command cache is created when the session starts. It enables the Get-Command cmdlet to get commands from modules that are not imported into the session.
This parameter is designed for development and testing scenarios in which the contents of modules have changed since the session started.
When the Refresh parameter is used in a command, the ListAvailable parameter is required.
This parameter is introduced in Windows PowerShell 3.0.


### INPUTS
#### System.String
You can pipe module names to Get-Module.

### OUTPUTS
#### System.Management.Automation.PSModuleInfo
Get-Module returns objects that represent modules. When you use the ListAvailable parameter, Get-Module returns a ModuleInfoGrouping object, which is a type of PSModuleInfo object that has the same properties and methods.

### NOTES
Beginning in Windows PowerShell 3.0, the core commands that are included in Windows PowerShell are packaged in modules. The exception is Microsoft.PowerShell.Core, which is a snap-in (PSSnapin). By default, only the Microsoft.PowerShell.Core snap-in is added to the session by default. Modules are imported automatically on first use and you can use the Import-Module cmdlet to import them.
Beginning in Windows PowerShell 3.0, the core commands that are installed with Windows PowerShell are packaged in modules. In Windows PowerShell 2.0, and in host programs that create older-style sessions in later versions of Windows PowerShell, the core commands are packaged in snap-ins ("PSSnapins"). The exception is Microsoft.PowerShell.Core, which is always a snap-in. Also, remote sessions, such as those started by the New-PSSession cmdlet, are older-style sessions that include core snap-ins.
For information about the CreateDefault2 method that creates newer-style sessions with core modules, see "CreateDefault2 Method" in MSDN at [http://msdn.microsoft.com/en-us/library/windows/desktop/system.management.automation.runspaces.initialsessionstate.createdefault2(v=VS.85).aspx]().
Get-Module only gets modules in locations that are stored in the value of the PSModulePath environment variable ($env:PSModulePath). You can use the Path parameter of the Import-Module cmdlet to import modules in other locations, but you cannot use the Get-Module cmdlet to get them.
Also, beginning in Windows PowerShell 3.0, new properties have been added to the object that Get-Module returns that make it easier to learn about modules even before they are imported. All properties are populated before importing, including the ExportedCommands, ExportedCmdlets and ExportedFunctions properties that list the commands that the module exports.
The ListAvailable parameter gets only well-formed modules, that is, folders that contain at least one file whose base name (the name without the file name extension) is the same as the name of the module folder. Folders that contain files with different names are considered to be containers, but not modules.
To get modules that are implemented as .dll files, but are not enclosed in a module folder, use both the ListAvailable and All parameters.
To use the CIM session feature, the remote computer must have WS-Management remoting and Windows Management Instrumentation (WMI), which is the Microsoft implementation of the Common Information Model (CIM) standard. The computer must also have the Module Discovery WMI provider or an alternate WMI provider that has the same basic features.
You can use the CIM session feature on computers that are not running a Windows operating system and on Windows computers that have Windows PowerShell, but do not have Windows PowerShell remoting enabled.
You can also use the CIM parameters to get CIM modules from computers that have Windows PowerShell remoting enabled, including the local computer. When you create a CIM session on the local computer, Windows PowerShell uses DCOM, instead of WMI, to create the session.

### EXAMPLES
#### -------------------------- EXAMPLE 1 --------------------------

```powershell
PS C:\>Get-Module

```
This command gets modules that have been imported into the current session.






#### -------------------------- EXAMPLE 2 --------------------------

```powershell
PS C:\>Get-Module -ListAvailable

```
This command gets the modules that are installed on the computer and can be imported into the current session.
Get-Module looks for available modules in the path specified by the $env:PSModulePath environment variable. For more information about PSModulePath, see about_Modules and about_Environment_Variables.






#### -------------------------- EXAMPLE 3 --------------------------

```powershell
PS C:\>Get-Module -ListAvailable -All

```
This command gets all of the exported files for all available modules.






#### -------------------------- EXAMPLE 4 --------------------------

```powershell
PS C:\>Get-Module -FullyQualifiedName @{ModuleName="Microsoft.PowerShell.Management";ModuleVersion="3.1.0.0"} | Format-Table -Property Name,Version
Name                                                                                 Version                                                                             
----                                                                                 -------                                                                             
Microsoft.PowerShell.Management                                                      3.1.0.0  

```
This command gets the Microsoft.PowerShell.Management module by specifying the module’s fully qualified name, with the FullyQualifiedName parameter. The command then pipes the results into Format-Table, to format the results as a table with Name and Version as the column headings.






#### -------------------------- EXAMPLE 5 --------------------------

```powershell
PS C:\>Get-Module | Get-Member -MemberType Property | Format-Table Name

Name

----

AccessMode

Author

ClrVersion

CompanyName

Copyright

Definition

Description

DotNetFrameworkVersion

ExportedAliases

ExportedCmdlets

ExportedCommands

ExportedFormatFiles

ExportedFunctions

ExportedTypeFiles

ExportedVariables

ExportedWorkflows

FileList

Guid

HelpInfoUri

LogPipelineExecutionDetails

ModuleBase

ModuleList

ModuleType

Name

NestedModules

OnRemove

Path

PowerShellHostName

PowerShellHostVersion

PowerShellVersion

PrivateData

ProcessorArchitecture

RequiredAssemblies

RequiredModules

RootModule

Scripts

SessionState

Version

```
This command gets the properties of the PSModuleInfo object that Get-Module returns. There is one object for each module file. 
You can use the properties to format and filter the module objects. For more information about the properties, see "PSModule Properties" in the MSDN (Microsoft Developer Network) library at [http://go.microsoft.com/fwlink/?LinkId=143624]().
The output includes the new properties, such as Author and CompanyName, that were introduced in Windows PowerShell 3.0


#### -------------------------- EXAMPLE 6 --------------------------

```powershell
PS C:\>Get-Module -ListAvailable -All | Format-Table -Property Name, Moduletype, Path -Groupby Name

   Name: AppLocker


Name      ModuleType Path

----      ---------- ----

AppLocker   Manifest C:\Windows\system32\WindowsPowerShell\v1.0\Modules\AppLocker\AppLocker.psd1



   Name: Appx


Name ModuleType Path

---- ---------- ----

Appx   Manifest C:\Windows\system32\WindowsPowerShell\v1.0\Modules\Appx\en-US\Appx.psd1

Appx   Manifest C:\Windows\system32\WindowsPowerShell\v1.0\Modules\Appx\Appx.psd1

Appx     Script C:\Windows\system32\WindowsPowerShell\v1.0\Modules\Appx\Appx.psm1



   Name: BestPractices


Name          ModuleType Path

----          ---------- ----

BestPractices   Manifest C:\Windows\system32\WindowsPowerShell\v1.0\Modules\BestPractices\BestPractices.psd1



   Name: BitsTransfer


Name         ModuleType Path

----         ---------- ----

BitsTransfer   Manifest C:\Windows\system32\WindowsPowerShell\v1.0\Modules\BitsTransfer\BitsTransfer.psd1


```
This command gets all module files (imported and available) and groups them by module name. This lets you see the module files that each script is exporting.


#### -------------------------- EXAMPLE 7 --------------------------

```powershell
The first command gets the PSModuleInfo object that represents BitsTransfer module. It saves the object in the $m variable.
PS C:\>$m = Get-Module -list -Name BitsTransfer

The second command uses the Get-Content cmdlet to get the content of the manifest file in the specified path. It uses dot notation to get the path to the manifest file, which is stored in the Path property of the object.The output shows the contents of the module manifest.
PS C:\>Get-Content $m.Path

@{
GUID="{8FA5064B-8479-4c5c-86EA-0D311FE48875}"
Author="Microsoft Corporation"
CompanyName="Microsoft Corporation"
Copyright="© Microsoft Corporation. All rights reserved."
ModuleVersion="1.0.0.0"
Description="Windows Powershell File Transfer Module"
PowerShellVersion="2.0"
CLRVersion="2.0"
NestedModules="Microsoft.BackgroundIntelligentTransfer.Management"
FormatsToProcess="FileTransfer.Format.ps1xml"
RequiredAssemblies=Join-Path $psScriptRoot "Microsoft.BackgroundIntelligentTransfer.Management.Interop.dll"
}

```
These commands display the contents of the module manifest for the Windows PowerShell BitsTransfer module. 
Modules are not required to have manifest files and, when they do have a manifest file, the manifest file is required only to include a version number. However, manifest files often provide useful information about a module, its requirements, and its contents.


#### -------------------------- EXAMPLE 8 --------------------------

```powershell
PS C:\>dir (Get-Module -ListAvailable FileTransfer).ModuleBase
Directory: C:\Windows\system32\WindowsPowerShell\v1.0\Modules\FileTransfer
Mode                LastWriteTime     Length Name
----                -------------     ------ ----
d----        12/16/2008  12:36 PM            en-US
-a---        11/19/2008  11:30 PM      16184 FileTransfer.Format.ps1xml
-a---        11/20/2008  11:30 PM       1044 FileTransfer.psd1
-a---        12/16/2008  12:20 AM     108544 Microsoft.BackgroundIntelligentTransfer.Management.Interop.dll

```
This command lists the files in the module's directory. This is another way to determine what is in a module before you import it. Some modules might have help files or ReadMe files that describe the module.


#### -------------------------- EXAMPLE 9 --------------------------

```powershell
PS C:\>$s = New-PSSession -ComputerName Server01

PS C:\>Get-Module -PSSession $s -ListAvailable

```
These commands get the modules that are installed on the Server01 computer.
The first command uses the New-PSSession cmdlet to create a PSSession on the Server01 computer. The command saves the PSSession in the $s variable.
The second command uses the PSSession and ListAvailable parameters of Get-Module to get the modules in the PSSession in the $s variable.
If you pipe modules from other sessions to the Import-Module cmdlet, Import-Module imports the module into the current session by using the implicit remoting feature. This is equivalent to using the  Import-PSSession cmdlet. You can use the cmdlets from the module in the current session, but commands that use these cmdlets actually run the remote session. For more information, see Import-Module and Import-PSSession.


#### -------------------------- EXAMPLE 10 --------------------------

```powershell
The first command uses the New-CimSession cmdlet to create a session on the RSDGF03 remote computer. The session connects to WMI on the remote computer. The command saves the CIM session in the $cs variable.
PS C:\>$cs = New-CimSession -ComputerName RSDGF03

The second command uses in the CIM session in the $cs variable to run a Get-Module command on the RSDGF03 computer. The command uses the Name parameter to specify the Storage module.The command uses a pipeline operator (|) to send the Storage module to the Import-Module cmdlet, which imports it into the local session.
PS C:\>Get-Module -CimSession $cs -Name Storage | Import-Module

The third command runs the Get-Command cmdlet on the Get-Disk command in the Storage module.When you import a CIM module into the local session, Windows PowerShell converts the CDXML files that represent in the CIM module into Windows PowerShell scripts, which appear as functions in the local session.
PS C:\>Get-Command Get-Disk
CommandType     Name                  ModuleName

-----------     ----                  ----------

Function        Get-Disk              Storage

The fourth command runs the Get-Disk command. Although the command is typed in the local session, it runs implicitly on the remote computer from which it was imported.The command gets objects from the remote computer and returns them to the local session.
PS C:\>Get-Disk
Number Friendly Name              OperationalStatus          Total Size Partition Style

------ -------------              -----------------          ---------- ---------------

0      Virtual HD ATA Device      Online                          40 GB MBR

```
The commands in this example enable you to manage the storage systems of a remote computer that is not running a Windows operating system. In this example, because the administrator of the computer has installed the Module Discovery WMI provider, the CIM commands can use the default values, which are designed for the provider.



### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289587)
[Get-CimSession]()
[New-CimSession]()
[about_Modules]()
[Get-PSSession]()
[Import-Module]()
[Import-PSSession]()
[New-PSSession]()
[Remove-Module]()

## Get-PSHostProcessInfo

### SYNOPSIS


### DESCRIPTION


### PARAMETERS

#### Id [Int32[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 2')]
```



#### Name [String[]]

```powershell
[Parameter(
  Position = 1,
  ParameterSetName = 'Set 1')]
```



#### Process [Process[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipeline = $true,
  ParameterSetName = 'Set 3')]
```




### INPUTS
#### None


### OUTPUTS
#### 


### NOTES

### EXAMPLES
#### 1:

```powershell
PS C:\>


```



#### 2:

```powershell

PS C:\>


```




### RELATED LINKS
[Online Version:]()

## Get-PSSession

### SYNOPSIS
Gets the Windows PowerShell sessions on local and remote computers.

### DESCRIPTION
The Get-PSSession cmdlet gets the user-managed Windows PowerShell sessions ("PSSessions") on local and remote computers.
Beginning in Windows PowerShell 3.0, sessions are stored on the computers at the remote end of each connection. You can use the ComputerName or ConnectionUri parameters of Get-PSSession to get the sessions that connect to the local computer or remote computers, even if they were not created in the current session.
Without parameters, Get-PSSession gets all sessions that were created in the current session.
Use the filtering parameters, including Name, ID, InstanceID,  State, ApplicationName, and ConfigurationName to select from among the sessions that Get-PSSession returns.
Use the remaining parameters to configure the temporary connection in which the Get-PSSession command runs when you use the ComputerName or ConnectionUri parameters.
NOTE: In Windows PowerShell 2.0, without parameters, Get-PSSession gets all sessions that were created in the current session. The ComputerName  parameter gets sessions that were created in the current session and connect to the specified computer.
For more information about Windows PowerShell sessions, see about_PSSessions (http://go.microsoft.com/fwlink/?LinkID=135181).

### PARAMETERS

#### Authentication [AuthenticationMechanism]

```powershell
[Parameter(ParameterSetName = 'Set 2')]
[Parameter(ParameterSetName = 'Set 3')]
[Parameter(ParameterSetName = 'Set 4')]
[Parameter(ParameterSetName = 'Set 5')]
```

Specifies the mechanism that is used to authenticate credentials for the session in which the Get-PSSession command runs.
This parameter configures the temporary connection that is created to run a Get-PSSession command with the ComputerName or ConnectionUri parameter.
Valid values are Default, Basic, Credssp, Digest, Kerberos, Negotiate, and NegotiateWithImplicitCredential.  The default value is Default.
For information about the values of this parameter, see the description of the System.Management.Automation.Runspaces.AuthenticationMechanism enumeration in MSDN.
CAUTION: Credential Security Support Provider (CredSSP) authentication, in which the user's credentials are passed to a remote computer to be authenticated, is designed for commands that require authentication on more than one resource, such as accessing a remote network share. This mechanism increases the security risk of the remote operation. If the remote computer is compromised, the credentials that are passed to it can be used to control the network session.
This parameter is introduced in Windows PowerShell 3.0.

#### CertificateThumbprint [String]

```powershell
[Parameter(ParameterSetName = 'Set 2')]
[Parameter(ParameterSetName = 'Set 3')]
[Parameter(ParameterSetName = 'Set 4')]
[Parameter(ParameterSetName = 'Set 5')]
```

Specifies the digital public key certificate (X509) of a user account that has permission to create the session in which the Get-PSSession command runs. Enter the certificate thumbprint of the certificate.
This parameter configures the temporary connection that is created to run a Get-PSSession command with the ComputerName or ConnectionUri parameter.
Certificates are used in client certificate-based authentication. They can be mapped only to local user accounts; they do not work with domain accounts.
To get a certificate thumbprint, use a Get-Item or Get-ChildItem command in the Windows PowerShell Cert: drive.
This parameter is introduced in Windows PowerShell 3.0.

#### ComputerName [String[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 3')]
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 4')]
```

Gets the sessions that connect to the specified computers. Wildcards are not permitted. There is no default value.
Beginning in Windows PowerShell 3.0, PSSessions are stored on the computers at the remote end of each connection. To get the sessions on the specified computers, Windows PowerShell creates a temporary connection to each computer and runs a Get-PSSession command.
Type the NetBIOS name, an IP address, or a fully-qualified domain name of one or more computers. To specify the local computer, type the computer name, "localhost", or a dot (.).
Note: This parameter gets sessions only from computers running Windows PowerShell 3.0 or later versions of Windows PowerShell. Earlier versions do not store sessions.

#### Credential [PSCredential]

```powershell
[Parameter(ParameterSetName = 'Set 2')]
[Parameter(ParameterSetName = 'Set 3')]
[Parameter(ParameterSetName = 'Set 4')]
[Parameter(ParameterSetName = 'Set 5')]
```

Runs the command with the permissions of the specified user. Specify a user account that has permission to connect to the remote computer and run a Get-PSSession command. The default is the current user. Type a user name, such as "User01", "Domain01\User01", or "User@Domain.com", or enter a PSCredential object, such as one returned by the Get-Credential cmdlet. When you type a user name, you will be prompted for a password.
This parameter configures to the temporary connection that is created to run a Get-PSSession command with the ComputerName or ConnectionUri parameter.
This parameter is introduced in Windows PowerShell 3.0.

#### Id [Int32[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 6')]
```

Gets only the sessions with the specified IDs. Type one or more IDs (separated by commas), or use the range operator (..) to specify a range of IDs. You cannot use the ID parameter with the ComputerName parameter.
An ID is an integer that uniquely identifies the user-managed sessions (PSSessions) in the current session. It is easier to remember and type than the InstanceId, but it is unique only within the current session.  The ID of a session is stored in the ID property of the session.


#### InstanceId [Guid[]]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 4')]
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 5')]
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 7')]
```

Gets only the sessions with the specified instance IDs.
The instance ID is a GUID that uniquely identifies a session on a local or remote computer. The InstanceID is unique, even when you have multiple sessions running in Windows PowerShell.
The instance ID of a session is stored in the InstanceID property of the session.

#### Name [String[]]

```powershell
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 1')]
[Parameter(ParameterSetName = 'Set 2')]
[Parameter(ParameterSetName = 'Set 3')]
```

Gets only the sessions with the specified friendly names. Wildcards are permitted.
The friendly name of a session is stored in the Name property of the session.

#### Port [Int32]

```powershell
[Parameter(ParameterSetName = 'Set 3')]
[Parameter(ParameterSetName = 'Set 4')]
```

Specifies the specified network port that is used for the temporary connection in which the Get-PSSession command runs. To connect to a remote computer, the remote computer must be listening on the port that the connection uses.  The default ports are 5985 (the WinRM port for HTTP) and 5986 (the WinRM port for HTTPS).
Before using an alternate port, you must configure the WinRM listener on the remote computer to listen at that port. To configure the listener, type the following two commands at the Windows PowerShell prompt:
Remove-Item -Path WSMan:\Localhost\listener\listener* -Recurse
New-Item -Path WSMan:\Localhost\listener -Transport http -Address * -Port <port-number>
This parameter configures to the temporary connection that is created to run a Get-PSSession command with the ComputerName or ConnectionUri parameter.
Do not use the Port parameter unless you must. The Port set in the command applies to all computers or sessions on which the command runs. An alternate port setting might prevent the command from running on all computers. Uses the specified port to run the Get-PSSession command.
This parameter is introduced in Windows PowerShell 3.0.

#### State [SessionFilterState]

```powershell
[Parameter(ParameterSetName = 'Set 2')]
[Parameter(ParameterSetName = 'Set 3')]
[Parameter(ParameterSetName = 'Set 4')]
[Parameter(ParameterSetName = 'Set 5')]
```

Gets only sessions  in the specified state. Valid values are: All, Opened, Disconnected, Closed, and Broken. The default value is All.
The session state value is relative to the current sessions. Sessions that were not created in the current sessions and are not connected to the current session have a state of Disconnected even when they are connected to a different session.
The state of a session is stored in the State property of the session.
This parameter is introduced in Windows PowerShell 3.0.

#### ThrottleLimit [Int32]

```powershell
[Parameter(ParameterSetName = 'Set 2')]
[Parameter(ParameterSetName = 'Set 3')]
[Parameter(ParameterSetName = 'Set 4')]
[Parameter(ParameterSetName = 'Set 5')]
```

Specifies the maximum number of concurrent connections that can be established to run the Get-PSSession command. If you omit this parameter or enter a value of 0 (zero), the default value, 32, is used. The throttle limit applies only to the current command, not to the session or to the computer.
This parameter is introduced in Windows PowerShell 3.0.

#### UseSSL [switch]

```powershell
[Parameter(ParameterSetName = 'Set 3')]
[Parameter(ParameterSetName = 'Set 4')]
```

Uses the Secure Sockets Layer (SSL) protocol to establish the connection in which the Get-PSSession command runs. By default, SSL is not used. If you use this parameter, but SSL is not available on the port used for the command, the command fails.
This parameter configures the temporary connection that is created to run a Get-PSSession command with the ComputerName parameter.
This parameter is introduced in Windows PowerShell 3.0.

#### AllowRedirection [switch]

```powershell
[Parameter(ParameterSetName = 'Set 2')]
[Parameter(ParameterSetName = 'Set 5')]
```

Allows redirection of this connection to an alternate Uniform Resource Identifier (URI). By default, Windows PowerShell does not redirect connections.
This  parameter configures the temporary connection that is created to run a Get-PSSession command with the ConnectionUri parameter.
This parameter is introduced in Windows PowerShell 3.0.

#### ApplicationName [String]

```powershell
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 3')]
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 4')]
```

Connects only to sessions that use the specified application.
Enter the application name segment of the connection URI. For example, in the following connection URI, the application name is WSMan: http://localhost:5985/WSMAN. The application name of a session is stored in the Runspace.ConnectionInfo.AppName property of the session.
The value of this parameter is used to select and filter sessions. It does not change the application that the session uses.

#### ConfigurationName [String]

```powershell
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 2')]
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 3')]
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 4')]
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 5')]
```

Gets only to sessions that use the specified session configuration.
Enter a configuration name or the fully qualified resource URI for a session configuration. If you specify only the configuration name, the following schema URI is prepended:  http://schemas.microsoft.com/powershell. The configuration name of a session is stored in the ConfigurationName property of the session.
The value of this parameter is used to select and filter sessions. It does not change the session configuration that the session uses.
For more information about session configurations, see about_Session_Configurations .

#### ConnectionUri [Uri[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 2')]
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 5')]
```

Specifies a Uniform Resource Identifier (URI) that defines the connection endpoint for the temporary session in which the Get-PSSession command runs. The URI must be fully qualified.
This  parameter configures the temporary connection that is created to run a Get-PSSession command with the ConnectionUri parameter.
The format of this string is:
<Transport>://<ComputerName>:<Port>/<ApplicationName>The default value is "http://localhost:5985/WSMAN".
If you do not specify a ConnectionUri, you can use the UseSSL, ComputerName, Port, and ApplicationName parameters to specify the ConnectionURI values. Valid values for the Transport segment of the URI are HTTP and HTTPS. If you specify a connection URI with a Transport segment, but do not specify a port, the session is created with standards ports: 80 for HTTP and 443 for HTTPS. To use the default ports for Windows PowerShell remoting, specify port 5985 for HTTP or 5986 for HTTPS.
If the destination computer redirects the connection to a different URI, Windows PowerShell prevents the redirection unless you use the AllowRedirection parameter in the command.
This parameter is introduced in Windows PowerShell 3.0.
Note: This parameter gets sessions only from computers running Windows PowerShell 3.0 or later versions of Windows PowerShell. Earlier versions do not store sessions.

#### SessionOption [PSSessionOption]

```powershell
[Parameter(ParameterSetName = 'Set 2')]
[Parameter(ParameterSetName = 'Set 3')]
[Parameter(ParameterSetName = 'Set 4')]
[Parameter(ParameterSetName = 'Set 5')]
```

Sets advanced options for the session.  Enter a SessionOption object, such as one that you create by using the New-PSSessionOption cmdlet, or a hash table in which the keys are session option names and the values are session option values.
The default values for the options are determined by the value of the $PSSessionOption preference variable, if it is set. Otherwise, the default values are established by options set in the session configuration.
The session option values take precedence over default values for sessions set in the $PSSessionOption preference variable and in the session configuration. However, they do not take precedence over maximum values, quotas or limits set in the session configuration. 
For a description of the session options, including the default values, see New-PSSessionOption. For information about the $PSSessionOption preference variable, see about_Preference_Variables (http://go.microsoft.com/fwlink/?LinkID=113248). For more information about session configurations, see about_Session_Configurations (http://go.microsoft.com/fwlink/?LinkID=145152).


### INPUTS
#### None
You cannot pipe input to this cmdlet.

### OUTPUTS
#### System.Management.Automation.Runspaces.PSSession

### NOTES
Get-PSSession gets user-managed sessions "PSSessions," such as those that are created by using the New-PSSession, Enter-PSSession, and Invoke-Command cmdlets. It does not get the system-managed session that is created when you start Windows PowerShell.
Beginning in Windows PowerShell 3.0, PSSessions are stored on the computer that is at the "server-side" or receiving end of a connection. To get the sessions that are stored on the local computer or a remote computer, Windows PowerShell establishes a temporary session to the specified computer and runs query commands in the  session.
To get sessions that connect to a remote computer, use the ComputerName or ConnectionUri parameters to specify the remote computer. To filter the sessions that Get-PSSession gets, use the Name, ID, InstanceID, and State parameters. Use the remaining parameters to configure the temporary session that Get-PSSession uses.
When you use the ComputerName or ConnectionUri parameters, Get-PSSession gets only sessions from computers running Windows PowerShell 3.0 and later versions of Windows PowerShell.
The value of the State property of a PSSession is relative to the current session. Therefore, a value of Disconnected means that the PSSession is not connected to the current session. However, it does not mean that the PSSession is disconnected from all sessions. It might be connected to a different session. To determine whether you can connect or reconnect to the PSSession from the current session, use the Availability property.
An Availability value of None indicates that you can connect to the session. A value of Busy indicates that you cannot connect to the PSSession because it is connected to another session.
For more information about the values of the State property of sessions, see "RunspaceState Enumeration" in MSDN at [http://msdn.microsoft.com/en-us/library/windows/desktop/system.management.automation.runspaces.runspacestate(v=VS.85).aspx]().
For more information about the values of the Availability property of sessions, see RunspaceAvailability Enumeration at [http://msdn.microsoft.com/en-us/library/windows/desktop/system.management.automation.runspaces.runspaceavailability(v=vs.85).aspx]().

### EXAMPLES
#### -------------------------- EXAMPLE 1 --------------------------

```powershell
PS C:\>Get-PSSession

```
This command gets all of the PSSessions that were created in the current session. It does not get PSSessions that were created in other sessions or on other computers, even if they connect to this computer.


#### -------------------------- EXAMPLE 2 --------------------------

```powershell
PS C:\>Get-PSSession -ComputerName localhost

```
This command gets the PSSessions that are connected to the local computer. To indicate the local computer, type the computer name, "localhost" or a dot (.)
The command returns all of the sessions on the local computer, even if they were created in different sessions or on different computers.


#### -------------------------- EXAMPLE 3 --------------------------

```powershell
PS C:\>Get-PSSession -ComputerName Server02
 Id Name            ComputerName    State         ConfigurationName     Availability
 -- ----            ------------    -----         -----------------     ------------
  2 Session3        Server02       Disconnected  ITTasks                       Busy
  1 ScheduledJobs   Server02       Opened        Microsoft.PowerShell     Available
  3 Test            Server02       Disconnected  Microsoft.PowerShell          Busy

```
This command gets the PSSessions that are connected to the Server02 computer.
The command returns all of the sessions on Server02, even if they were created in different sessions or on different computers.
The output shows that two of the sessions have a Disconnected state and a Busy availability. They were created in different sessions and are currently in use. The "ScheduledJobs" session, which is Opened and Available, was created in the current session.


#### -------------------------- EXAMPLE 4 --------------------------

```powershell
PS C:\>New-PSSession -ComputerName Server01, Server02, Server03
PS C:\>$s1, $s2, $s3 = Get-PSSession

```
This example shows how to save the results of a Get-PSSession command in multiple variables.
The first command uses the New-PSSession cmdlet to create PSSessions on three remote computers.
The second command uses a Get-PSSession cmdlet to get the three PSSessions. It then saves each of the PSSessions in a separate variable.
When Windows PowerShell assigns an array of objects to an array of variables, it assigns the first object to the first variable, the second object to the second variable, and so on. If there are more objects than variables, it assigns all remaining objects to the last variable in the array. If there are more variables than objects, the extra variables are not used.


#### -------------------------- EXAMPLE 5 --------------------------

```powershell
PS C:\>Get-PSSession | Format-Table -Property ComputerName, InstanceID
PS C:\>$s = Get-PSSession -InstanceID a786be29-a6bb-40da-80fb-782c67f7db0f
PS C:\>Remove-PSSession -Session $s

```
This example shows how to get a PSSession by using its instance ID, and then to delete the PSSession.
The first command gets all of the PSSessions that were created in the current session. It sends the PSSessions to the Format-Table cmdlet, which displays the ComputerName and InstanceID properties of each PSSession.
The second command uses the Get-PSSession cmdlet to get a particular PSSession and to save it in the $s variable. The command uses the InstanceID parameter to identify the PSSession.
The third command uses the Remove-PSSession cmdlet to delete the PSSession in the $s variable.


#### -------------------------- EXAMPLE 6 --------------------------

```powershell
The first command gets sessions on the Server02 and Server12 remote computers that have names that begin with "BackupJob" and use the ITTasks session configuration.The command uses the Name parameter to specify the name pattern and the ConfigurationName parameter to specify the session configuration. The value of the SessionOption parameter is a hash table that sets the value of the OperationTimeout to 240000 milliseconds (4 minutes). This setting gives the command more time to complete.The ConfigurationName and SessionOption parameters are used to configure the temporary sessions in which the Get-PSSession cmdlet runs on each computer.The output shows that the command returns the BackupJob04 session. The session is disconnected and the Availability is None, which indicates that it is not in use.
PS C:\>Get-PSSession -ComputerName Server02, Server12 -Name BackupJob* -ConfigurationName ITTasks -SessionOption @{OperationTimeout=240000}
 Id Name            ComputerName    State         ConfigurationName     Availability
 -- ----            ------------    -----         -----------------     ------------
  3 BackupJob04     Server02        Disconnected        ITTasks                  None

The second command uses the Get-PSSession cmdlet to get to the BackupJob04 session and the Connect-PSSession cmdlet to connect to the session. The command saves the session in the $s variable.
PS C:\>$s = Get-PSSession -ComputerName Server02 -Name BackupJob04 -ConfigurationName ITTasks | Connect-PSSession

The third command gets the session in the $s variable. The output shows that the Connect-PSSession command was successful. The session is in the Opened state and is available for use.
PS C:\>$s
Id Name            ComputerName    State         ConfigurationName     Availability
-- ----            ------------    -----         -----------------     ------------
 5 BackupJob04     Server02        Opened        ITTasks                  Available

```
The commands in this example find a session that has a particular name format and uses a particular session configuration and then connect to the session. You can use a command like this one to find a session in which a colleague started a task and connect to finish the task.


#### -------------------------- EXAMPLE 7 --------------------------

```powershell
PS C:\>Get-PSSession -ID 2

```
This command gets the PSSession with ID 2. Because the value of the ID property is unique only in the current session, the ID parameter is valid only for local commands.



### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289588)
[Connect-PSSession]()
[Disconnect-PSSession]()
[Receive-PSSession]()
[Enter-PSSession]()
[Exit-PSSession]()
[Invoke-Command]()
[New-PSSession]()
[Remove-PSSession]()
[about_PSSessions]()
[about_Remote]()

## Get-PSSessionConfiguration

### SYNOPSIS
Gets the registered session configurations on the computer. 

### DESCRIPTION
The Get-PSSessionConfiguration cmdlet gets the session configurations that have been registered on the local computer. This is an advanced cmdlet that is designed to be used by system administrators to manage customized session configurations for their users.
Beginning in Windows PowerShell 3.0, you can define the properties of a session configuration by using a session configuration (.pssc) file. This feature lets you create customized and restricted sessions without writing a computer program. For more information about session configuration files, see about_Session_Configuration_Files (http://go.microsoft.com/fwlink/?LinkID=236023).
Also, beginning in Windows PowerShell 3.0, new note properties have been added to the session configuration object that Get-PSSessionConfiguration returns. These properties make it easier for users and session configuration authors to examine and compare session configurations.
To create and register a session configuration, use the Register-PSSessionConfiguration cmdlet. For more information about session configurations, see about_Session_Configurations (http://go.microsoft.com/fwlink/?LinkID=145152).

### PARAMETERS

#### Name [String[]]

```powershell
[Parameter(
  Position = 1,
  ParameterSetName = 'Set 1')]
```

Gets only the session configurations with the specified name or name pattern. Enter one or more session configuration names. Wildcards are permitted.

#### Force [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Suppresses the prompt to restart the WinRM service, if the service is not already running.


### INPUTS
#### None
You cannot pipe input to this cmdlet.

### OUTPUTS
####  Microsoft.PowerShell.Commands.PSSessionConfigurationCommands#PSSessionConfiguration


### NOTES
To run this cmdlet, start Windows PowerShell with the "Run as administrator" option.
To view the session configurations on the computer, you must be a member of the Administrators group on the computer.
To run a Get-PSSessionConfiguration command on a remote computer, Credential Security Service Provider (CredSSP) authentication must be enabled in the client settings on the local computer (by using the Enable-WSManCredSSP cmdlet) and in the service settings on the remote computer, and you must use the CredSSP value of the Authentication parameter when establishing the remote session. Otherwise, access is denied.
The note properties of the object that Get-PSSessionConfiguration returns appear on the object only when they have a value. Only session configurations that were created by using a session configuration file have all of the defined properties.
The properties of a session configuration object vary with the options set for the session configuration and the values of those options. Also, session configurations that use a session configuration file have additional properties.
You can use commands in the WSMan: drive to change the properties of session configurations. However, you cannot use the WSMan: drive in Windows PowerShell 2.0 to change session configuration properties that are introduced in Windows PowerShell 3.0, such as OutputBufferingMode. Windows PowerShell 2.0 commands do not generate an error, but they are ineffective. To change  properties introduced in Windows PowerShell 3.0, use the WSMan: drive in Windows PowerShell 3.0.

### EXAMPLES
#### -------------------------- EXAMPLE 1 --------------------------

```powershell
PS C:\>Get-PSSessionConfiguration

```
This command gets the session configurations on the local computer.


#### -------------------------- EXAMPLE 2 --------------------------

```powershell
PS C:\>Get-PSSessionConfiguration -Name Microsoft*

Name                      PSVersion  StartupScript        Permission
----                      ---------  -------------        ----------
microsoft.powershell      2.0                             BUILTIN\Administrators AccessAll...
microsoft.powershell32    2.0                             BUILTIN\Administrators AccessAll...

```
This command gets the two default session configurations that come with Windows PowerShell. The command uses the Name parameter of Get-PSSessionConfiguration to get only the session configurations with names that begin with "Microsoft".


#### -------------------------- EXAMPLE 3 --------------------------

```powershell
PS C:\>Get-PSSessionConfiguration -Name Full  | Format-List -Property *
 
                      
Copyright                     : (c) 2011 User01. All rights reserved.
AliasDefinitions              : {System.Collections.Hashtable}
SessionType                   : Default
CompanyName                   : Unknown
GUID                          : 1e9cb265-dae0-4bd3-89a9-8338a47698a1
Author                        : User01
ExecutionPolicy               : Restricted
SchemaVersion                 : 1.0.0.0
LanguageMode                  : FullLanguage
Architecture                  : 64
Filename                      : %windir%\system32\pwrshplugin.dll
ResourceUri                   : http://schemas.microsoft.com/powershell/Full
MaxConcurrentCommandsPerShell : 1500
UseSharedProcess              : false
ProcessIdleTimeoutSec         : 0
xmlns                         : http://schemas.microsoft.com/wbem/wsman/1/config/PluginConfiguration
MaxConcurrentUsers            : 10
lang                          : en-US
SupportsOptions               : true
ExactMatch                    : true
configfilepath                : C:\WINDOWS\System32\WindowsPowerShell\v1.0\SessionConfig\Full_1e9cb265-dae0-4bd3-89a9-8338a47698a1.pssc
RunAsUser                     :
IdleTimeoutms                 : 7200000
PSVersion                     : 3.0
OutputBufferingMode           : Block
AutoRestart                   : false
MaxShells                     : 300
MaxMemoryPerShellMB           : 1024
MaxIdleTimeoutms              : 43200000
SDKVersion                    : 1
Name                          : Full
XmlRenderingType              : text
Capability                    : {Shell}
RunAsPassword                 :
MaxProcessesPerShell          : 25
Enabled                       : True
MaxShellsPerUser              : 30
Permission                    :

```
This example shows the properties and property values of a session configuration that was created by using a session configuration file. 
The command uses the Get-PSSessionConfiguration command to get the Full session configuration. A pipeline operator sends the Full session configuration to the Format-List cmdlet. The Property parameter with a value of * (all) directs Format-List to display all of the properties and property values of the object in a list.
The output of this command has very useful information, including the author of the session configuration, the session type, language mode, and execution policy of sessions that are created with this session configuration, session quotas, and the full path to the session configuration file.
This view of a session configuration is used for sessions that include a session configuration file. For more information about session configuration files, see about_Session_Configuration_Files (http://go.microsoft.com/fwlink/?LinkID=236023).


#### -------------------------- EXAMPLE 4 --------------------------

```powershell
PS C:\>(Get-PSSessionConfiguration Microsoft.PowerShell.Workflow).PSObject.Properties | Select-Object Name,Value | Sort-Object Name

Name                                                                                                              Value

----                                                                                                              -----

ActivityProcessIdleTimeoutSec                                                                                        60

AllowedActivity                                                                                   {PSDefaultActivities}

Architecture                                                                                                         64

AssemblyName                                                ...licKeyToken=31bf3856ad364e35, processorArchitecture=MSIL

AutoRestart                                                                                                       false

Capability                                                                                                      {Shell}

Enabled                                                                                                            true

EnableValidation                                                                                                   true

ExactMatch                                                                                                        False

Filename                                                                              %windir%\system32\pwrshplugin.dll

IdleTimeoutms                                                                                                   7200000

lang                                                                                                              en-US

MaxActivityProcesses                                                                                                  5

MaxConcurrentCommandsPerShell                                                                                      1000

MaxConcurrentUsers                                                                                                    5

MaxConnectedSessions                                                                                                100

MaxDisconnectedSessions                                                                                            1000

MaxIdleTimeoutms                                                                                             2147483647

MaxMemoryPerShellMB                                                                                                1024

MaxPersistenceStoreSizeGB                                                                                            10

MaxProcessesPerShell                                                                                                 15

MaxRunningWorkflows                                                                                                  30

MaxSessionsPerRemoteNode                                                                                              5

MaxSessionsPerWorkflow                                                                                                5

MaxShells                                                                                                            25

MaxShellsPerUser                                                                                                     25

ModulesToImport                                             %windir%\system32\windowspowershell\v1.0\Modules\PSWorkflow

Name                                                                                      microsoft.powershell.workflow

OutOfProcessActivity                                                                                     {InlineScript}

OutputBufferingMode                                                                                               Block

ParentResourceUri                                           ...s.microsoft.com/powershell/microsoft.powershell.workflow

Permission                                                  ...ssAllowed, BUILTIN\Remote Management Users AccessAllowed

PersistencePath                                             ...s\juneb\AppData\Local\Microsoft\Windows\PowerShell\WF\PS

PersistWithEncryption                                                                                             False

ProcessIdleTimeoutSec                                                                                             28800

PSSessionConfigurationTypeName                              ...osoft.PowerShell.Workflow.PSWorkflowSessionConfiguration

PSVersion                                                                                                           3.0

RemoteNodeSessionIdleTimeoutSec                                                                                      60

ResourceUri                                                 ...s.microsoft.com/powershell/microsoft.powershell.workflow

RunAsPassword

RunAsUser

SDKVersion                                                                                                            2

SecurityDescriptorSddl                                      ...;GA;;;BA)(A;;GA;;;RM)S:P(AU;FA;GA;;;WD)(AU;SA;GXGW;;;WD)

SessionConfigurationData                                    ...    </SessionConfigurationData>

SessionThrottleLimit                                                                                                100

SupportsOptions                                                                                                    true

Uri                                                         ...s.microsoft.com/powershell/microsoft.powershell.workflow

UseSharedProcess                                                                                                   true

WorkflowShutdownTimeoutMSec                                                                                         500

xmlns                                                       ...as.microsoft.com/wbem/wsman/1/config/PluginConfiguration

XmlRenderingType                                                                                                   text

```
This command gets the properties of the Microsoft.PowerShell.Worfklow session configuration and sorts them into alphabetical order for easy reading. You can use this command format in a function to get this display for any session configuration.
This example was contributed by Shay Levy, a Windows PowerShell MVP from Sderot, Israel.


#### -------------------------- EXAMPLE 5 --------------------------

```powershell
PS C:\>dir wsman:\localhost\plugin
Type            Keys                                Name
----            ----                                ----
Container       {Name=Event Forwarding Plugin}      Event Forwarding Plugin
Container       {Name=Full}                         Full
Container       {Name=microsoft.powershell}         microsoft.powershell
Container       {Name=microsoft.powershell.workf... microsoft.powershell.workflow
Container       {Name=microsoft.powershell32}       microsoft.powershell32
Container       {Name=microsoft.ServerManager}      microsoft.ServerManager
Container       {Name=WMI Provider}                 WMI Provider

```
This command uses the Get-ChildItem cmdlet (alias = dir) in the WSMan: provider drive to look at the content of the Plugin node. This is another way to look at the session configurations on the computer.
The PlugIn node contains ContainerElement objects (Microsoft.WSMan.Management.WSManConfigContainerElement) that represent the registered Windows PowerShell session configurations, along with other plug-ins for WS-Management.


#### -------------------------- EXAMPLE 6 --------------------------

```powershell
The first command uses the Connect-WSMan cmdlet to connect to the WinRM service on the Server01 remote computer.
PS C:\>Connect-WSMan -ComputerName Server01

The second command uses the Get-ChildItem cmdlet ("dir") in the WSMan: drive to get the items in the Server01\Plugin path.The output shows the items in the Plugin directory on the Server01 computer. The items include the session configurations, which are a type of WSMan plug-in, along with other types of plug-ins on the computer.
PS C:\>dir WSMan:\Server01\Plugin
   WSManConfig: Microsoft.WSMan.Management\WSMan::localhost\Plugin

Type            Keys                                Name
----            ----                                ----
Container       {Name=Empty}                        Empty
Container       {Name=Event Forwarding Plugin}      Event Forwarding Plugin
Container       {Name=Full}                         Full
Container       {Name=microsoft.powershell}         microsoft.powershell
Container       {Name=microsoft.powershell.workf... microsoft.powershell.workflow
Container       {Name=microsoft.powershell32}       microsoft.powershell32
Container       {Name=microsoft.ServerManager}      microsoft.ServerManager
Container       {Name=NoLanguage}                   NoLanguage
Container       {Name=RestrictedLang}               RestrictedLang
Container       {Name=RRS}                          RRS
Container       {Name=SEL Plugin}                   SEL Plugin
Container       {Name=WithProfile}                  WithProfile
Container       {Name=WMI Provider}                 WMI Provider

The third command returns the names of the plugins that are session configurations. The command searches for a value of Shell in the Capability property, which is in the Plugin\Resources\<ResourceNumber> path in the WSMan: drive.
PS C:\>dir WSMan:\Server01\Plugin\*\Resources\Resource*\Capability | where {$_.Value -eq "Shell"} | foreach {($_.PSPath.split("\"))[3] }
Empty
Full
microsoft.powershell
microsoft.powershell.workflow
microsoft.powershell32
microsoft.ServerManager
NoLanguage
RestrictedLang
RRS
WithProfile

```
This example shows how to use the WSMan provider to view the session configurations on a remote computer. This method does not provide as much information as a Get-PSSessionConfiguration command, but the user does not need to be a member of the Administrators group to run this command.


#### -------------------------- EXAMPLE 7 --------------------------

```powershell
The first command uses the Enable-WSManCredSSP cmdlet to enable CredSSP delegation from the Server01 local computer to the Server02 remote computer. This configures the CredSSP client setting on the local computer.
PS C:\>Enable-WSManCredSSP -Delegate Server02

The second command uses the Connect-WSMan cmdlet to connect to the Server02 computer. This action adds a node for the Server02 computer to the WSMan: drive on the local computer, allowing you to view and change the WS-Management settings on the Server02 computer.
PS C:\>Connect-WSMan Server02

The third command uses the Set-Item cmdlet to change the value of the CredSSP item in the Service node of the Server02 computer to True. This configures the service settings on the remote computer.
PS C:\>Set-Item WSMan:\Server02*\Service\Auth\CredSSP -Value $true

The fourth command uses the Invoke-Command cmdlet to run a Get-PSSessionConfiguration command on the Server02 computer. The command uses the Credential parameter, and it uses the Authentication parameter with a value of CredSSP.The output shows the session configurations on the Server02 remote computer.
PS C:\>Invoke-Command -ScriptBlock {Get-PSSessionConfiguration} -ComputerName Server02 -Authentication CredSSP -Credential Domain01\Admin01

Name                      PSVersion  StartupScript        Permission                          PSComputerName
----                      ---------  -------------        ----------                          --------------
microsoft.powershell      2.0                             BUILTIN\Administrators AccessAll... server02.corp.fabrikam.com
microsoft.powershell32    2.0                             BUILTIN\Administrators AccessAll... server02.corp.fabrikam.com
MyX86Shell                2.0        c:\test\x86Shell.ps1 BUILTIN\Administrators AccessAll... server02.corp.fabrikam.com

```
This example shows how to run a Get-PSSessionConfiguration command on a remote computer. The command requires that CredSSP delegation be enabled in the client settings on the local computer and in the service settings on the remote computer. 
To run the commands in this example, you must be a member of the Administrators group on the local computer and the remote computer and you must start Windows PowerShell with the "Run as administrator" option.


#### -------------------------- EXAMPLE 8 --------------------------

```powershell
PS C:\>(Get-PSSessionConfiguration -Name CustomShell).resourceURI
http://schemas.microsoft.com/powershell/microsoft.CustomShell

```
This command uses the Get-PSSessionConfiguration cmdlet to get the resource URI of a session configuration.
This command is useful when setting the value of the $PSSessionConfigurationName preference variable, which takes a resource URI.
The $PSSessionConfiguationName variable specifies the default configuration that is used when you create a session. This variable is set on the local computer, but it specifies a configuration on the remote computer. For more information about the $PSSessionConfiguration variable, see about_Preference_Variables (http://go.microsoft.com/fwlink/?LinkID=113248).



### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289589)
[Disable-PSSessionConfiguration]()
[Enable-PSSessionConfiguration]()
[Get-PSSessionConfiguration]()
[New-PSSessionConfigurationFile]()
[New-PSSessionConfigurationOption]()
[Register-PSSessionConfiguration]()
[Set-PSSessionConfiguration]()
[Test-PSSessionConfigurationFile]()
[Unregister-PSSessionConfiguration]()
[WSMan Provider]()
[about_Session_Configurations]()
[about_Session_Configuration_Files]()

## Get-PSSnapin

### SYNOPSIS
Gets the Windows PowerShell snap-ins on the computer.

### DESCRIPTION
The Get-PSSnapin cmdlet gets the Windows PowerShell snap-ins that have been added to the current session or that have been registered on the system. The snap-ins are listed in the order in which they are detected.
Get-PSSnapin gets only registered snap-ins. To register a Windows PowerShell snap-in, use the InstallUtil tool included with the Microsoft .NET Framework 2.0. For more information, see "How to Register Cmdlets, Providers, and Host Applications" in the MSDN (Microsoft Developer Network) library at http://go.microsoft.com/fwlink/?LinkId=143619.
Beginning in Windows PowerShell 3.0, the core commands that are included in Windows PowerShell are packaged in modules. The exception is Microsoft.PowerShell.Core, which is a snap-in (PSSnapin). Only the Microsoft.PowerShell.Core snap-in is added to the session by default. Modules are imported automatically on first use and you can use the Import-Module cmdlet to import them.

### PARAMETERS

#### Name [String[]]

```powershell
[Parameter(
  Position = 1,
  ParameterSetName = 'Set 1')]
```

Gets only the specified Windows PowerShell snap-ins. Enter the names of one or more Windows PowerShell snap-ins. Wildcards are permitted.
The parameter name ("Name") is optional.

#### Registered [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Gets the Windows PowerShell snap-ins that have been registered on the system (even if they have not yet been added to the session).
The snap-ins that are installed with Windows PowerShell do not appear in this list.
Without this parameter, Get-PSSnapin gets the Windows PowerShell snap-ins that have been added to the session.


### INPUTS
#### None
You cannot pipe input to Get-PSSnapin.

### OUTPUTS
#### System.Management.Automation.PSSnapInInfo
Get-PSSnapin returns an object for each snap-in that it gets.

### NOTES
Beginning in Windows PowerShell 3.0, the core commands that are installed with Windows PowerShell are packaged in modules. In Windows PowerShell 2.0, and in host programs that create older-style sessions in later versions of Windows PowerShell, the core commands are packaged in snap-ins ("PSSnapins"). The exception is Microsoft.PowerShell.Core, which is always a snap-in. Also, remote sessions, such as those started by the New-PSSession cmdlet, are older-style sessions that include core snap-ins.
For information about the CreateDefault2 method that creates newer-style sessions with core modules, see "CreateDefault2 Method" in MSDN at [http://msdn.microsoft.com/en-us/library/windows/desktop/system.management.automation.runspaces.initialsessionstate.createdefault2(v=VS.85).aspx]().

### EXAMPLES
#### -------------------------- EXAMPLE 1 --------------------------

```powershell
PS C:\>get-PSSnapIn

```
This command gets the Windows PowerShell snap-ins that are currently loaded in the session. This includes the snap-ins that are installed with Windows PowerShell and those that have been added to the session.






#### -------------------------- EXAMPLE 2 --------------------------

```powershell
PS C:\>get-PSSnapIn -registered

```
This command gets the Windows PowerShell snap-ins that have been registered on the computer, including those that have already been added to the session. The output does not include snap-ins that are installed with Windows PowerShell or Windows PowerShell snap-in dynamic-link libraries (DLLs) that have not yet been registered on the system.






#### -------------------------- EXAMPLE 3 --------------------------

```powershell
PS C:\>get-PSSnapIn smp*

```
This command gets the Windows PowerShell snap-ins in the current session that have names that begin with "smp".







### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289590)
[Add-PSSnapin]()
[Remove-PSSnapin]()

## Import-Module

### SYNOPSIS
Adds modules to the current session.

### DESCRIPTION
The Import-Module cmdlet adds one or more modules to the current session. The modules that you import must be installed on the local computer or a remote computer.
Beginning in Windows PowerShell 3.0, installed modules are automatically imported to the session when you use any commands or providers in the module. However, you can still use the Import-Module command to import a module and you can enable and disable automatic module importing by using the $PSModuleAutoloadingPreference preference variable. For more information about modules, see about_Modules (http://go.microsoft.com/fwlink/?LinkID=144311). For more information about the $PSModuleAutoloadingPreference variable, see about_Preference_Variables (http://go.microsoft.com/fwlink/?LinkID=113248).
A module is a package that contains members (such as cmdlets, providers, scripts, functions, variables, and other tools and files) that can be used in Windows PowerShell. After a module is imported, you can use the module members in your session.
To import a module, use the Name, Assembly,  ModuleInfo, MinimumVersion and RequiredVersion parameters to identify the module to import. By default, Import-Module imports all members that the module exports, but you can use the Alias, Function, Cmdlet, and Variable parameters to restrict the members that are imported. You can also use the NoClobber parameter to prevent Import-Module from importing members that have the same names as members in the current session.
Import-Module imports a module only into the current session. To import the module into all sessions, add an Import-Module command to your Windows PowerShell profile. For more information about profiles, see about_Profiles (http://go.microsoft.com/fwlink/?LinkID=113729).
Also, beginning in Windows PowerShell 3.0, you can use Import-Module to import Common Information Model (CIM) modules, in which the cmdlets are defined in Cmdlet Definition XML (CDXML) files. This feature allows you to use cmdlets that are implemented in non-managed code assemblies, such as those written in C++.
With these new features, Import-Module cmdlet becomes a primary tool for managing heterogeneous enterprises that include Windows computers and computers that are running other operating systems. 
To manage remote Windows computers that have Windows PowerShell and Windows PowerShell remoting enabled, create a PSSession on the remote computer and then use the PSSession parameter of Get-Module to get the Windows PowerShell modules in the PSSession. When you import the modules, and then use the imported commands in the current session, the commands run implicitly in the PSSession on the remote computer. You can use this strategy to manage the remote computer.
You can use a similar strategy to manage computers that do not have Windows PowerShell remoting enabled, including computers that are not running a Windows operating system, and Windows computers that have Windows PowerShell, but do not have Windows PowerShell remoting enabled.
Begin by creating a "CIM session" on the remote computer; a connection to Windows Management Instrumentation (WMI) on the remote computer. Then use the CIMSession parameter of Import-Module to import CIM modules from the remote computer. When you import a CIM module and then run the imported commands, the commands run implicitly on the remote computer. You can use this WMI and CIM strategy to manage the remote computer.

### PARAMETERS

#### Alias [String[]]

Imports only the specified aliases from the module into the current session. Enter a comma-separated list of aliases. Wildcard characters are permitted.
Some modules automatically export selected aliases into your session when you import the module. This parameter lets you select from among the exported aliases.

#### ArgumentList [Object[]]

Specifies arguments (parameter values) that are passed to a script module during the Import-Module command.  This parameter is valid only when you are importing a script module.
You can also refer to ArgumentList by its alias, "args". For more information, see about_Aliases.

#### AsCustomObject [switch]

Returns a custom object with members that represent the imported module members. This parameter is valid only for script modules.
When you use the AsCustomObject parameter, Import-Module imports the module members into the session and then returns a PSCustomObject object instead of a PSModuleInfo object. You can save the custom object in a variable and use dot notation to invoke the members.

#### Assembly [Assembly[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipeline = $true,
  ParameterSetName = 'Set 2')]
```

Imports the cmdlets and providers implemented in the specified assembly objects. Enter a variable that contains assembly objects or a command that creates assembly objects. You can also pipe an assembly object to Import-Module.
When you use this parameter, only the cmdlets and providers implemented by the specified assemblies are imported. If the module contains other files, they are not imported, and you might be missing important members of the module. Use this parameter for debugging and testing the module, or when you are instructed to use it by the module author.

#### CimNamespace [String]

```powershell
[Parameter(ParameterSetName = 'Set 3')]
```

Specifies the namespace of an alternate CIM provider that exposes CIM modules. The default value is the namespace of the Module Discovery WMI provider.
Use this parameter to import CIM modules from computers and devices that are not running a Windows operating system.
This parameter is introduced in Windows PowerShell 3.0.

#### CimResourceUri [Uri]

```powershell
[Parameter(ParameterSetName = 'Set 3')]
```

Specifies an alternate location for CIM modules. The default value is the  resource URI of the Module Discovery WMI provider on the remote computer.
Use this parameter to import CIM modules from computers and devices that are not running a Windows operating system.
This parameter is introduced in Windows PowerShell 3.0.

#### Cmdlet [String[]]

Imports only the specified cmdlets from the module into the current session. Enter a list of cmdlets. Wildcard characters are permitted.
Some modules automatically export selected cmdlets into your session when you import the module. This parameter lets you select from among the exported cmdlets.

#### DisableNameChecking [switch]

Suppresses the message that warns you when you import a cmdlet or function whose name includes an unapproved verb or a prohibited character.
By default,  when a module that you import exports cmdlets or functions that have unapproved verbs in their names, the Windows PowerShell displays the following warning message:
"WARNING: Some imported command names include unapproved verbs which might make them less discoverable.  Use the Verbose parameter for more detail or type Get-Verb to see the list of approved verbs."
This message is only a warning. The complete module is still imported, including the non-conforming commands. Although the message is displayed to module users, the naming problem should be fixed by the module author.

#### Force [switch]

Re-imports a module and its members, even if the module or its members have an access mode of read-only.

#### FullyQualifiedName [String[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipeline = $true,
  ParameterSetName = 'Set 1')]
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipeline = $true,
  ParameterSetName = 'Set 3')]
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipeline = $true,
  ParameterSetName = 'Set 4')]
```

Imports modules with names that are specified in the form of ModuleSpecification objects (described by the Remarks section of [Module Specification Constructor (Hashtable)]() on MSDN). For example, the FullyQualifiedName parameter accepts a module name that is specified in the format @{ModuleName = "modulename"; ModuleVersion = "version_number"} or @{ModuleName = "modulename"; ModuleVersion = "version_number"; Guid = "GUID"}. ModuleName and ModuleVersion are required, but Guid is optional.
You cannot specify the FullyQualifiedName parameter in the same command as a Name parameter; the two parameters are mutually exclusive.

#### Function [String[]]

Imports only the specified functions from the module into the current session. Enter a list of functions. Wildcard characters are permitted.
Some modules automatically export selected functions into your session when you import the module. This parameter lets you select from among the exported functions.

#### Global [switch]

Imports modules into the global session state so they are available to all commands in the session. By default, the commands in a module, including commands from nested modules, are imported into the caller's session state. To restrict the commands that a module exports, use an Export-ModuleMember command in the script module.
The Global parameter is equivalent to the Scope parameter with a value of Global.


#### ModuleInfo [PSModuleInfo[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipeline = $true,
  ParameterSetName = 'Set 5')]
```

Specifies module objects to import. Enter a variable that contains the module objects, or a command that gets the module objects, such as a "Get-Module -ListAvailable" command. You can also pipe module objects to Import-Module.

#### Name [String[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipeline = $true,
  ParameterSetName = 'Set 1')]
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipeline = $true,
  ParameterSetName = 'Set 3')]
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipeline = $true,
  ParameterSetName = 'Set 4')]
```

Specifies the names of the modules to import. Enter the name of the module or the name of a file in the module, such as a .psd1, .psm1, .dll, or ps1 file. File paths are optional. Wildcards are not permitted. You can also pipe module names and file names to Import-Module.
If you omit a path, Import-Module looks for the module in the paths saved in the PSModulePath environment variable ($env:PSModulePath).
Specify only the module name whenever possible. When you specify a file name, only the members that are implemented in that file are imported. If the module contains other files, they are not imported, and you might be missing important members of the module.

#### NoClobber [switch]

Does not import commands that have the same names as existing commands in the current session. By default, Import-Module imports all exported module commands.
Commands with the same names can hide or replace commands in the session. To avoid command name conflicts in a session, use the Prefix or NoClobber parameters. For more information about name conflicts and command precedence, see "Modules and Name Conflicts" in about_Modules and about_Command_Precedence.

This parameter was added in Windows PowerShell 3.0.

#### PassThru [switch]

Returns objects that represent the modules that were imported. By default, this cmdlet does not generate any output.

#### Prefix [String]

Adds the specified prefix to the nouns in the names of imported module members.
Use this parameter to avoid name conflicts that might occur when different members in the session have the same name. This parameter does not change the module, and it does not affect files that the module imports for its own use (known as "nested modules"). It affects only the names of members in the current session.
For example, if you specify the prefix "UTC" and then import a Get-Date cmdlet, the cmdlet is known in the session as Get-UTCDate, and it is not confused with the original Get-Date cmdlet.
The value of this parameter takes precedence over the DefaultCommandPrefix property of the module, which specifies the default prefix.

#### Variable [String[]]

Imports only the specified variables from the module into the current session. Enter a list of variables. Wildcard characters are permitted.
Some modules automatically export selected variables into your session when you import the module. This parameter lets you select from among the exported variables.

#### CimSession [CimSession]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 3')]
```

Specifies a CIM session on the remote computer. Enter a variable that contains the CIM session or a command that gets the CIM session, such as a [Get-CIMSession]() command.
Import-Module uses the CIM session connection to import modules from the remote computer into the current session. When you use the commands from the imported module in the current session, the commands actually run on the remote computer.
You can use this parameter to import modules from computers and devices that are not running a Windows operating system, and Windows computers that have Windows PowerShell, but do not have Windows PowerShell remoting enabled.
This parameter is introduced in Windows PowerShell 3.0.

#### MinimumVersion [Version]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
[Parameter(ParameterSetName = 'Set 3')]
[Parameter(ParameterSetName = 'Set 4')]
```

Imports only a version of the module that is greater than or equal to the specified value. If no version qualifies, Import-Module generates an error.
By default, Import-Module imports the module without checking the version number.
Use the MinimumVersion parameter name or its alias, Version.
To specify an exact version, use the RequiredVersion parameter. You can also use the Module and Version parameters of the #Requires keyword to require a specific version of a module in a script.
This parameter is introduced in Windows PowerShell 3.0.

#### PSSession [PSSession]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 4')]
```

Imports modules from the specified Windows PowerShell user-managed session ("PSSession") into the current session. Enter a variable that contains a PSSession or a command that gets a PSSession, such as a Get-PSSession command.
When you import a module from a different session into the current session, you can use the cmdlets from the module in the current session, just as you would use cmdlets from a local module. Commands that use the remote cmdlets actually run in the remote session, but the remoting details are managed in the background by Windows PowerShell. 
This parameter uses the Implicit Remoting feature of Windows PowerShell. It is equivalent to using the Import-PSSession cmdlet to import particular modules from a session.
Import-Module cannot import Windows PowerShell Core modules from another session. The Windows PowerShell Core modules have names that begin with Microsoft.PowerShell.
This parameter is introduced in Windows PowerShell 3.0.

#### RequiredVersion [Version]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
[Parameter(ParameterSetName = 'Set 3')]
[Parameter(ParameterSetName = 'Set 4')]
```

Imports only the specified version of the module. If the version is not installed, Import-Module generates an error.
By default, Import-Module imports the module without checking the version number.
To specify a minimum version, use the MinimumVersion parameter. You can also use the Module and Version parameters of the #Requires keyword to require a specific version of a module in a script.
This parameter is introduced in Windows PowerShell 3.0.
Scripts that use the RequiredVersion parameter to import modules that are included with existing releases of the Windows operating system do not automatically run in future releases of the Windows operating system. This is because Windows PowerShell module version numbers in future releases of the Windows operating system are higher than module version numbers in existing releases of the Windows operating system.

#### Scope [String]

Imports the module only into the specified scope.
 Valid values are:
-- Global: Available to all commands in the session. Equivalent to the Global parameter.
                         
-- Local: Available only in the current scope.
By default, the module is imported into the current scope, which could be a script or module.
This parameter is introduced in Windows PowerShell 3.0.


### INPUTS
#### System.String, System.Management.Automation.PSModuleInfo, System.Reflection.Assembly
You can pipe a module name, module object, or assembly object to Import-Module.

### OUTPUTS
#### None, System.Management.Automation.PSModuleInfo, or System.Management.Automation.PSCustomObject
By default, Import-Module does not generate any output. If you use the PassThru parameter, it generates a System.Management.Automation.PSModuleInfo object that represents the module. If you use the AsCustomObject parameter, it generates a PSCustomObject object.

### NOTES
Before you can import a module, the module must be installed on the local computer, that is, the module directory must be copied to a directory that is accessible to your local computer. For more information, see about_Modules (http://go.microsoft.com/fwlink/?LinkID=144311).
You can also use the PSSession and CIMSession parameters to import modules that are installed on remote computers. However, commands that use the cmdlets in these modules actually run in the remote session on the remote computer.
If you import members with the same name and the same type into your session, Windows PowerShell uses the member imported last by default. Variables and aliases are replaced, and the originals are not accessible. Functions, cmdlets and providers are merely "shadowed" by the new members, and they can be accessed by qualifying the command name with the name of its snap-in, module, or function path.
To update the formatting data for commands that have been imported from a module, use the Update-FormatData cmdlet. Update-FormatData also updates the formatting data for commands in the session that were imported from modules. If the formatting file for a module changes, you can run an Update-FormatData command to update the formatting data for imported commands. You do not need to import the module again.
Beginning in Windows PowerShell 3.0, the core commands that are installed with Windows PowerShell are packaged in modules. In Windows PowerShell 2.0, and in host programs that create older-style sessions in later versions of Windows PowerShell, the core commands are packaged in snap-ins ("PSSnapins"). The exception is Microsoft.PowerShell.Core, which is always a snap-in. Also, remote sessions, such as those started by the New-PSSession cmdlet, are older-style sessions that include core snap-ins.
For information about the CreateDefault2 method that creates newer-style sessions with core modules, see "CreateDefault2 Method" in MSDN at [http://msdn.microsoft.com/en-us/library/windows/desktop/system.management.automation.runspaces.initialsessionstate.createdefault2(v=VS.85).aspx]().
Import-Module cannot import Windows PowerShell Core modules from another session. The Windows PowerShell Core modules have names that begin with Microsoft.PowerShell.
In Windows PowerShell 2.0, some of the property values of the module object, such as the ExportedCmdlets and NestedModules property values, were not populated until the module was imported and were not available on the module object that the PassThru parameter returns. In Windows PowerShell 3.0, all module property values are populated.
If you attempt to import a module that contains mixed-mode assemblies that are not compatible with Windows PowerShell 3.0, Import-Module returns an error message like the following one.
Import-Module : Mixed mode assembly is built against version 'v2.0.50727' of the runtime and cannot be loaded in the 4.0 runtime without additional configuration information.
This error occurs when a module that is designed for Windows PowerShell 2.0 contains at least one mixed-module assembly, that is, an assembly that includes both managed and non-managed code, such as C++ and C#.
To import a module that contains mixed-mode assemblies, start Windows PowerShell 2.0 by using the following command, and then try the Import-Module command again.
PowerShell.exe -Version 2.0
To use the CIM session feature, the remote computer must have WS-Management remoting and Windows Management Instrumentation (WMI), which is the Microsoft implementation of the Common Information Model (CIM) standard. The computer must also have the Module Discovery WMI provider or an alternate CIM provider that has the same basic features.
You can use the CIM session feature on computers that are not running a Windows operating system and on Windows computers that have Windows PowerShell, but do not have Windows PowerShell remoting enabled.
You can also use the CIM parameters to get CIM modules from computers that have Windows PowerShell remoting enabled, including the local computer. When you create a CIM session on the local computer, Windows PowerShell uses DCOM, instead of WMI, to create the session.

### EXAMPLES
#### -------------------------- EXAMPLE 1 --------------------------

```powershell
PS C:\>Import-Module -Name BitsTransfer

```
This command imports the members of the BitsTransfer module into the current session.
The Name parameter name (-Name) is optional and can be omitted.
By default, Import-Module does not generate any output when it imports a module. To request output, use the PassThru or AsCustomObject parameter, or the Verbose common parameter.


#### -------------------------- EXAMPLE 2 --------------------------

```powershell
PS C:\>Get-Module -ListAvailable | Import-Module

```
This command imports all available modules in the path specified by the PSModulePath environment variable ($env:PSModulePath) into the current session.






#### -------------------------- EXAMPLE 3 --------------------------

```powershell
PS C:\>$m = Get-Module -ListAvailable BitsTransfer, ServerManager
PS C:\>Import-Module -ModuleInfo $m

```
These commands import the members of the BitsTransfer and ServerManager modules into the current session.
The first command uses the Get-Module-Module cmdlet to get the BitsTransfer and ServerManager modules. It saves the objects in the $m variable. The ListAvailable parameter is required when you are getting modules that are not yet imported into the session.
The second command uses the ModuleInfo parameter of Import-Module to import the modules into the current session.
These commands are equivalent to using a pipeline operator (|) to send the output of a Get-Module command to Import-Module.


#### -------------------------- EXAMPLE 4 --------------------------

```powershell
PS C:\>Import-Module -Name c:\ps-test\modules\test -Verbose
VERBOSE: Loading module from path 'C:\ps-test\modules\Test\Test.psm1'.
VERBOSE: Exporting function 'my-parm'.
VERBOSE: Exporting function 'Get-Parameter'.
VERBOSE: Exporting function 'Get-Specification'.
VERBOSE: Exporting function 'Get-SpecDetails'.

```
This command uses an explicit path to identify the module to import.
It also uses the Verbose common parameter to get a list of the items imported from the module. Without the Verbose, PassThru, or AsCustomObject parameter, Import-Module does not generate any output when it imports a module.


#### -------------------------- EXAMPLE 5 --------------------------

```powershell
PS C:\>Import-Module BitsTransfer -cmdlet Add-BitsTransferFile, Get-BitsTransfer
PS C:\>Get-Module BitsTransfer

Name              : BitsTransfer
Path              : C:\Windows\system32\WindowsPowerShell\v1.0\Modules\BitsTransfer\BitsTransfer.psd1
Description       :
Guid              : 8fa5064b-8479-4c5c-86ea-0d311fe48875
Version           : 1.0.0.0
ModuleBase        : C:\Windows\system32\WindowsPowerShell\v1.0\Modules\BitsTransfer
ModuleType        : Manifest
PrivateData       :
AccessMode        : ReadWrite
ExportedAliases   : {}
ExportedCmdlets   : {[Add-BitsTransfer, Add-BitsTransfer], [Complete-BitsTransfer, Complete-BitsTransfer],
 [Get-BitsTransfer, Get-BitsTransfer], [Remove-BitsTransfer, Remove-BitsTransfer]...}
ExportedFunctions : {}
ExportedVariables : {}
NestedModules     : {Microsoft.BackgroundIntelligentTransfer.Management}

PS C:\>Get-Command -Module BitsTransfer

CommandType     Name                                               ModuleName
-----------     ----                                               ----------
Cmdlet          Add-BitsFile                                       bitstransfer
Cmdlet          Complete-BitsTransfer                              bitstransfer
Cmdlet          Get-BitsTransfer                                   bitstransfer
Cmdlet          Remove-BitsTransfer                                bitstransfer
Cmdlet          Resume-BitsTransfer                                bitstransfer
Cmdlet          Set-BitsTransfer                                   bitstransfer
Cmdlet          Start-BitsTransfer                                 bitstransfer
Cmdlet          Suspend-BitsTransfer                               bitstransfer

```
This example shows how to restrict the module members that are imported into the session and the effect of this command on the session.
The first command imports only the Add-BitsTransfer and Get-BitsTransfer cmdlets from the BitsTransfer module. The command uses the Cmdlet parameter to restrict the cmdlets that the module imports. You can also use the Alias, Variable, and Function parameters to restrict other members that a module imports.
The second command uses the Get-Module cmdlet to get the object that represents the BitsTransfer module. The ExportedCmdlets property lists all of the cmdlets that the module exports, even when they were not all imported.
The third command uses the Module parameter of the Get-Command cmdlet to get the commands that were imported from the BitsTransfer module. The results confirm that only the Add-BitsTransfer and Get-BitsTransfer cmdlets were imported.


#### -------------------------- EXAMPLE 6 --------------------------

```powershell
PS C:\>Import-Module BitsTransfer -Prefix PS -PassThru

ModuleType Name                                ExportedCommands
---------- ----                                ----------------
Manifest   bitstransfer                        {Add-BitsFile, Complete-...

PS C:\>Get-Command -Module BitsTransfer

CommandType     Name                                               ModuleName
-----------     ----                                               ----------
Cmdlet          Add-BitsFile                                       bitstransfer
Cmdlet          Add-PSBitsFile                                     bitstransfer
Cmdlet          Complete-BitsTransfer                              bitstransfer
Cmdlet          Complete-PSBitsTransfer                            bitstransfer
Cmdlet          Get-BitsTransfer                                   bitstransfer
Cmdlet          Get-PSBitsTransfer                                 bitstransfer
Cmdlet          Remove-BitsTransfer                                bitstransfer
Cmdlet          Remove-PSBitsTransfer                              bitstransfer
Cmdlet          Resume-BitsTransfer                                bitstransfer
Cmdlet          Resume-PSBitsTransfer                              bitstransfer
Cmdlet          Set-BitsTransfer                                   bitstransfer
Cmdlet          Set-PSBitsTransfer                                 bitstransfer
Cmdlet          Start-BitsTransfer                                 bitstransfer
Cmdlet          Start-PSBitsTransfer                               bitstransfer
Cmdlet          Suspend-BitsTransfer                               bitstransfer
Cmdlet          Suspend-PSBitsTransfer                             bitstransfer

```
These commands import the BitsTransfer module into the current session, add a prefix to the member names, and then display the prefixed member names.
The first command uses the Import-Module cmdlet to import the BitsTransfer module. It uses the Prefix parameter to add the PS prefix to all members that are imported from the module and the PassThru parameter to return a module object that represents the imported module.
The second command uses the Get-Command cmdlet to get the members that have been imported from the module. It uses the Module parameter to specify the module. The output shows that the module members were correctly prefixed.
The prefix that you use applies only to the members in the current session. It does not change the module.


#### -------------------------- EXAMPLE 7 --------------------------

```powershell
PS C:\>Get-Module -List | Format-Table -Property Name, ModuleType -AutoSize

Name          ModuleType
----          ----------
Show-Calendar     Script
BitsTransfer    Manifest
PSDiagnostics   Manifest
TestCmdlets       Script

PS C:\>$a = Import-Module -Name Show-Calendar -AsCustomObject -Passthru

PS C:\>$a | Get-Member

    TypeName: System.Management.Automation.PSCustomObject
Name          MemberType   Definition
----          ----------   ----------
Equals        Method       bool Equals(System.Object obj)
GetHashCode   Method       int GetHashCode()
GetType       Method       type GetType()
ToString      Method       string ToString()
Show-Calendar ScriptMethod System.Object Show-Calendar();

PS C:\>$a."Show-Calendar"()

```
These commands demonstrate how to get and use the custom object that Import-Module returns.
Custom objects include synthetic members that represent each of the imported module members. For example, the cmdlets and functions in a module are converted to script methods of the custom object.
Custom objects are very useful in scripting. They are also useful when several imported objects have the same names. Using the script method of an object is equivalent to specifying the fully qualified name of an imported member, including its module name.
The AsCustomObject parameter can be used only when importing a script module, so the first task is to determine which of the available modules is a script module.
The first command uses the Get-Module cmdlet to get the available modules. The command uses a pipeline operator (|) to pass the module objects to the Format-Tablee cmdlet, which lists the Name and ModuleType of each module in a table.
The second command uses the Import-Module cmdlet to import the PSDiagnostics script module. The command uses the AsCustomObject parameter to request a custom object and the PassThru parameter to return the  object. The command saves the resulting custom object in the $a variable.
The third command uses a pipeline operator to send the $a variable to the Get-Member cmdlet, which gets the properties and methods of the PSCustomObject in $a. The output shows a Show-Calendar script method.
The last command uses the Show-Calendar script method. The method name must be enclosed in quotation marks, because it includes a hyphen.






#### -------------------------- EXAMPLE 8 --------------------------

```powershell
PS C:\>Import-Module BitsTransfer
PS C:\>Import-Module BitsTransfer -force -Prefix PS

```
This example shows how to use the Force parameter of Import-Module when you are re-importing a module into the same session.
The first command imports the BitsTransfer module. The second command imports the module again, this time using the Prefix parameter.
The second command also includes the Force parameter, which removes the module and then imports it again. Without this parameter, the session would include two copies of each BitsTransfer cmdlet, one with the standard name and one with the prefixed name.


#### -------------------------- EXAMPLE 9 --------------------------

```powershell
PS C:\>Get-Date
Thursday, March 15, 2012 6:47:04 PM

PS C:\>Import-Module TestModule

PS C:\>Get-Date
12075

PS C:\>Get-Command Get-Date -All | Format-Table -Property CommandType, Name, ModuleName -AutoSize

CommandType     Name         ModuleName
-----------     ----         ----------
Function        Get-Date     TestModule
Cmdlet          Get-Date     Microsoft.PowerShell.Utility

PS C:\>Microsoft.PowerShell.Utility\Get-Date
Saturday, September 12, 2009 6:33:23 PM

```
This example shows how to run commands that have been hidden by imported commands.
The first command run the Get-Date cmdlet. It returns a DateTime object with the current date.
The second command imports the TestModule module. This module includes a function named Get-Date that returns the year and day of the year.
The third command runs the Get-Date command again. Because functions take precedence over cmdlets, the Get-Date function from the TestModule module runs, instead of the Get-Date cmdlet.
The fourth command uses the All parameter of the Get-Command to get all of the Get-Date commands in the session. The results show that there are two Get-Date commands in the session, a function from the TestModule module and a cmdlet from the Microsoft.PowerShell.Utility module.
The fifth command runs the hidden cmdlet by qualifying the command name with the module name.
For more information about command precedence in Windows PowerShell, see about_Command_Precedence (http://go.microsoft.com/fwlink/?LinkID=113214).


#### -------------------------- EXAMPLE 10 --------------------------

```powershell
PS C:\>Import-Module -Name PSWorkflow -MinimumVersion 3.0.0.0

```
This command imports the PSWorkflow module. It uses the MinimumVersion (alias=Version) parameter of Import-Module to import only version 3.0.0.0 or greater of the module.
You can also use the RequiredVersion parameter to import a particular version of a module, or use the Module and Version parameters of the #Requires keyword to require a particular version of a module in a script.


#### -------------------------- EXAMPLE 10 --------------------------

```powershell
The first command uses the New-PSSession cmdlet to create a remote session (PSSession) to the Server01 computer. The command saves the PSSession in the $s variable
PS C:\>$s = New-PSSession -ComputerName Server01

The second command uses the PSSession parameter of the Get-Module cmdlet to get the NetSecurity module in the session in the $s variable.This command is equivalent to using the Invoke-Command cmdlet to run a Get-Module command in the session in $s (Invoke-Command $s {Get-Module -ListAvailable -Name NetSecurity).The output shows that the NetSecurity module is installed on the computer and is available to the session in the $s variable.
PS C:\>Get-Module -PSSession $s -ListAvailable -Name NetSecurity
ModuleType Name                                ExportedCommands
---------- ----                                ----------------
Manifest   NetSecurity                         {New-NetIPsecAuthProposal, New-NetIPsecMainModeCryptoProposal, New-Ne...

The third command uses the PSSession parameter of the Import-Module cmdlet to import the NetSecurity module from the session in the $s variable into the current session.
PS C:\>Import-Module -PSSession $s -Name NetSecurity

The fourth command uses the Get-Command cmdlet to get commands that begin with "Get" and include "Firewall" from the Net-Security module.The output gets the commands and confirms that the module and its cmdlets were imported into the current session.
PS C:\>Get-Command -Module NetSecurity -Name Get-*Firewall*
CommandType     Name                                               ModuleName
-----------     ----                                               ----------
Function        Get-NetFirewallAddressFilter                       NetSecurity
Function        Get-NetFirewallApplicationFilter                   NetSecurity
Function        Get-NetFirewallInterfaceFilter                     NetSecurity
Function        Get-NetFirewallInterfaceTypeFilter                 NetSecurity
Function        Get-NetFirewallPortFilter                          NetSecurity
Function        Get-NetFirewallProfile                             NetSecurity
Function        Get-NetFirewallRule                                NetSecurity
Function        Get-NetFirewallSecurityFilter                      NetSecurity
Function        Get-NetFirewallServiceFilter                       NetSecurity
Function        Get-NetFirewallSetting                             NetSecurity

The fifth command uses the Get-NetFirewallRule cmdlet to get Windows Remote Management firewall rules on the Server01 computer. This command is equivalent to using the Invoke-Command cmdlet to run a Get-NetFirewallRule command on the session in the $s variable (Invoke-Command -Session $s {Get-NetFirewallRule -DisplayName "Windows Remote Management*"}).
PS C:\>Get-NetFirewallRule -DisplayName "Windows Remote Management*" | Format-Table -Property DisplayName, Name -AutoSize
DisplayName                                              Name
-----------                                              ----
Windows Remote Management (HTTP-In)                      WINRM-HTTP-In-TCP
Windows Remote Management (HTTP-In)                      WINRM-HTTP-In-TCP-PUBLIC
Windows Remote Management - Compatibility Mode (HTTP-In) WINRM-HTTP-Compat-In-TCP

```
This example shows how to use the Import-Module cmdlet to import a module from a remote computer. This command uses the Implicit Remoting feature of Windows PowerShell.
When you import modules from another session, you can use the cmdlets in the current session. However, commands that use the cmdlets actually run in the remote session.


#### -------------------------- EXAMPLE 11 --------------------------

```powershell
The first command uses the New-CimSession cmdlet to create a session on the RSDGF03 remote computer. The session connects to WMI on the remote computer. The command saves the CIM session in the $cs variable.
PS C:\>$cs = New-CimSession -ComputerName RSDGF03

The second command uses the CIM session in the $cs variable to run an Import-Module command on the RSDGF03 computer. The command uses the Name parameter to specify the Storage CIM module.
PS C:\>Import-Module -CimSession $cs -Name Storage

The third command runs the a Get-Command command on the Get-Disk command in the Storage module.When you import a CIM module into the local session, Windows PowerShell converts the CDXML files for each command into Windows PowerShell scripts, which appear as functions in the local session.
PS C:\>Get-Command Get-Disk
CommandType     Name                  ModuleName
-----------     ----                  ----------
Function        Get-Disk              Storage

The fourth command runs the Get-Disk command. Although the command is typed in the local session, it runs implicitly on the remote computer from which it was imported.The command gets objects from the remote computer and returns them to the local session.
PS C:\>Get-Disk
Number Friendly Name              OperationalStatus          Total Size Partition Style
------ -------------              -----------------          ---------- ---------------
0      Virtual HD ATA Device      Online                          40 GB MBR

```
The commands in this example enable you to manage the storage systems of a remote computer that is not running a Windows operating system. In this example, because the administrator of the computer has installed the Module Discovery WMI provider, the CIM commands can use the default values, which are designed for the provider.



### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289591)
[Export-ModuleMember]()
[Get-Module]()
[New-Module]()
[Remove-Module]()
[Get-Verb]()
[about_Modules]()

## Invoke-Command

### SYNOPSIS
Runs commands on local and remote computers.

### DESCRIPTION
The Invoke-Command cmdlet runs commands on a local or remote computer and returns all output from the commands, including errors. With a single Invoke-Command command, you can run commands on multiple computers.
To run a single command on a remote computer, use the ComputerName parameter. To run a series of related commands that share data, use the New-PSSession cmdlet to create a PSSession (a persistent connection) on the remote computer, and then use the Session parameter of Invoke-Command to run the command in the PSSession. To run a command in a disconnected session, use the InDisconnectedSession parameter. To run a command in a background job, use the AsJob parameter.
You can also use Invoke-Command on a local computer to evaluate or run a string in a script block as a command. Windows PowerShell converts the script block to a command and runs the command immediately in the current scope, instead of just echoing the string at the command line.
To start an interactive session with a remote computer, use the Enter-PSSession cmdlet. To establish a persistent connection to a remote computer, use the New-PSSession cmdlet.
Before using Invoke-Command to run commands on a remote computer, read about_Remote (http://go.microsoft.com/fwlink/?LinkID=135182).

### PARAMETERS

#### AllowRedirection [switch]

```powershell
[Parameter(ParameterSetName = 'Set 2')]
[Parameter(ParameterSetName = 'Set 3')]
```

Allows redirection of this connection to an alternate Uniform Resource Identifier (URI).
When you use the ConnectionURI parameter, the remote destination can return an instruction to redirect to a different URI. By default, Windows PowerShell does not redirect connections, but you can use this parameter to allow it to redirect the connection.
You can also limit the number of times the connection is redirected by changing the MaximumConnectionRedirectionCount session option value. Use the  MaximumRedirection parameter of the New-PSSessionOption cmdlet or set the MaximumConnectionRedirectionCount property of the $PSSessionOption preference variable. The default value is 5.

#### ApplicationName [String]

```powershell
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 4')]
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 5')]
```

Specifies the application name segment of the connection URI. Use this parameter to specify the application name when you are not using the ConnectionURI parameter in the command.
The default value is the value of the $PSSessionApplicationName preference variable on the local computer. If this preference variable is not defined, the default value is WSMAN. This value is appropriate for most uses. For more information, see about_Preference_Variables (http://go.microsoft.com/fwlink/?LinkID=113248).
The WinRM service uses the application name to select a listener to service the connection request. The value of this parameter should match the value of the URLPrefix property of a listener on the remote computer.

#### ArgumentList [Object[]]

Supplies the values of local variables in the command. The variables in the command are replaced by these values before the command is run on the remote computer. Enter the values in a comma-separated list. Values are associated with variables in the order that they are listed. The alias for ArgumentList is "Args".
The values in ArgumentList can be actual values, such as "1024", or they can be references to local variables, such as "$max".
To use local variables in a command, use the following command format:
{param($<name1>[, $<name2>]...) <command-with-local-variables>} -ArgumentList <value> -or- <local-variable>
The "param" keyword lists the local variables that are used in the command. The ArgumentList parameter supplies the values of the variables, in the order that they are listed.

#### AsJob [switch]

```powershell
[Parameter(ParameterSetName = 'Set 2')]
[Parameter(ParameterSetName = 'Set 3')]
[Parameter(ParameterSetName = 'Set 4')]
[Parameter(ParameterSetName = 'Set 5')]
[Parameter(ParameterSetName = 'Set 6')]
[Parameter(ParameterSetName = 'Set 7')]
```

Runs the command as a background job on a remote computer. Use this parameter to run commands that take an extensive time to complete.
When you use AsJob, the command returns an object that represents the job, and then displays the command prompt. You can continue to work in the session while the job completes.  To manage the job, use the Job cmdlets. To get the job results, use the Receive-Job cmdlet.
The AsJob parameter is similar to using the Invoke-Command cmdlet to run a Start-Job command remotely. However, with AsJob, the job is created on the local computer, even though the job runs on a remote computer, and the results of the remote job are automatically returned to the local computer.
For more information about Windows PowerShell background jobs, see about_Jobs (http://go.microsoft.com/fwlink/?LinkID=113251) and about_Remote_Jobs (http://go.microsoft.com/fwlink/?LinkID=135184).

#### Authentication [AuthenticationMechanism]

```powershell
[Parameter(ParameterSetName = 'Set 2')]
[Parameter(ParameterSetName = 'Set 3')]
[Parameter(ParameterSetName = 'Set 4')]
[Parameter(ParameterSetName = 'Set 5')]
```

Specifies the mechanism that is used to authenticate the user's credentials.   Valid values are Default, Basic, Credssp, Digest, Kerberos, Negotiate, and NegotiateWithImplicitCredential.  The default value is Default.
CredSSP authentication is available only in Windows Vista, Windows Server 2008, and later versions of Windows.
For information about the values of this parameter, see the description of the System.Management.Automation.Runspaces.AuthenticationMechanism enumeration in MSDN.
CAUTION: Credential Security Support Provider (CredSSP) authentication, in which the user's credentials are passed to a remote computer to be authenticated, is designed for commands that require authentication on more than one resource, such as accessing a remote network share. This mechanism increases the security risk of the remote operation. If the remote computer is compromised, the credentials that are passed to it can be used to control the network session.

#### CertificateThumbprint [String]

```powershell
[Parameter(ParameterSetName = 'Set 2')]
[Parameter(ParameterSetName = 'Set 5')]
```

Specifies the digital public key certificate (X509) of a user account that has permission to perform this action. Enter the certificate thumbprint of the certificate.
Certificates are used in client certificate-based authentication. They can only be mapped to local user accounts; they do not work with domain accounts.
To get a certificate, use the Get-Item or Get-ChildItem commands in the Windows PowerShell Cert: drive.

#### ComputerName [String[]]

```powershell
[Parameter(
  Position = 1,
  ParameterSetName = 'Set 4')]
[Parameter(
  Position = 1,
  ParameterSetName = 'Set 5')]
```

Specifies the computers on which the command runs. The default is the local computer.
When you use the ComputerName parameter, Windows PowerShell creates a temporary connection that is used only to run the specified command and is then closed. If you need a persistent connection, use the Session parameter.
Type the NETBIOS name, IP address, or fully-qualified domain name of one or more computers in a comma-separated list. To specify the local computer, type the computer name, "localhost", or a dot (.).
To use an IP address in the value of the ComputerName parameter, the command must include the Credential parameter. Also, the computer must be configured for HTTPS transport or the IP address of the remote computer must be included in the WinRM TrustedHosts list on the local computer. For instructions for adding a computer name to the TrustedHosts list, see "How to Add  a Computer to the Trusted Host List" in about_Remote_Troubleshooting.
Note:  On Windows Vista, and later versions of Windows, to include the local computer in the value of the ComputerName parameter, you must open Windows PowerShell with the "Run as administrator" option.

#### ConfigurationName [String]

```powershell
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 2')]
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 3')]
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 4')]
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 5')]
```

Specifies the session configuration that is used for the new PSSession.
Enter a configuration name or the fully qualified resource URI for a session configuration. If you specify only the configuration name, the following schema URI is prepended:  http://schemas.microsoft.com/PowerShell.
The session configuration for a session is located on the remote computer. If the specified session configuration does not exist on the remote computer, the command fails.
The default value is the value of the $PSSessionConfigurationName preference variable on the local computer. If this preference variable is not set, the default is Microsoft.PowerShell. For more information, see about_preference_variables.

#### ConnectionUri [Uri[]]

```powershell
[Parameter(
  Position = 1,
  ParameterSetName = 'Set 2')]
[Parameter(
  Position = 1,
  ParameterSetName = 'Set 3')]
```

Specifies a Uniform Resource Identifier (URI) that defines the connection endpoint of the session. The URI must be fully qualified.
The format of this string is as follows:

<Transport>://<ComputerName>:<Port>/<ApplicationName>

The default value is as follows:

http://localhost:5985/WSMAN
If you do not specify a connection URI, you can use the UseSSL and Port  parameters to specify the connection URI values.
Valid values for the Transport segment of the URI are HTTP and HTTPS. If you specify a connection URI with a Transport segment, but do not specify a port, the session is created with standards ports: 80 for HTTP and 443 for HTTPS. To use the default ports for Windows PowerShell remoting, specify port 5985 for HTTP or 5986 for HTTPS.
If the destination computer redirects the connection to a different URI, Windows PowerShell prevents the redirection unless you use the AllowRedirection parameter in the command.

#### Credential [PSCredential]

```powershell
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 2')]
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 3')]
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 4')]
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 5')]
```

Specifies a user account that has permission to perform this action. The default is the current user.
Type a user name, such as "User01" or "Domain01\User01", or enter a variable that contains a PSCredential object, such as one generated by the Get-Credential cmdlet. When you type a user name, you will be prompted for a password.

#### EnableNetworkAccess [switch]

```powershell
[Parameter(ParameterSetName = 'Set 2')]
[Parameter(ParameterSetName = 'Set 3')]
[Parameter(ParameterSetName = 'Set 4')]
[Parameter(ParameterSetName = 'Set 5')]
```

Adds an interactive security token to loopback sessions. The interactive token lets you run commands in the loopback session that get data from other computers. For example, you can run a command in the session that copies XML files from a remote computer to the local computer.
A "loopback session" is a PSSession that originates and terminates on the same computer. To create a loopback session, omit the ComputerName parameter or set its value to ".", "localhost", or the name of the local computer.
By default, loopback sessions are created with a network token, which might not provide sufficient permission to authenticate to remote computers.
The EnableNetworkAccess parameter is effective only in loopback sessions. If you use the EnableNetworkAccess parameter when creating a session on a remote computer, the command succeeds, but the parameter is ignored.
You can also allow remote access in a loopback session by using the CredSSP value of the Authentication parameter, which delegates the session credentials to other computers.
To protect the computer from malicious access, disconnected loopback sessions that have interactive tokens (those created with the EnableNetworkAccess parameter) can be reconnected only from the computer on which the session was created. Disconnected sessions that use CredSSP authentication can be reconnected from other computers. For more information, see Disconnect-PSSession.
This parameter is introduced in Windows PowerShell 3.0.

#### FilePath [String]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 2,
  ParameterSetName = 'Set 3')]
[Parameter(
  Mandatory = $true,
  Position = 2,
  ParameterSetName = 'Set 4')]
[Parameter(
  Mandatory = $true,
  Position = 2,
  ParameterSetName = 'Set 6')]
```

Runs the specified local script on one or more remote computers. Enter the path and file name of the script, or pipe a script path to Invoke-Command. The script must reside on the local computer or in a directory that the local computer can access. Use the ArgumentList parameter to specify the values of parameters in the script.
When you use this parameter, Windows PowerShell converts the contents of the specified script file to a script block, transmits the script block to the remote computer, and runs it on the remote computer.

#### HideComputerName [switch]

```powershell
[Parameter(ParameterSetName = 'Set 2')]
[Parameter(ParameterSetName = 'Set 3')]
[Parameter(ParameterSetName = 'Set 4')]
[Parameter(ParameterSetName = 'Set 5')]
[Parameter(ParameterSetName = 'Set 6')]
[Parameter(ParameterSetName = 'Set 7')]
```

Omits the computer name of each object from the output display. By default, the name of the computer that generated the object appears in the display.
This parameter affects only the output display. It does not change the object.

#### InDisconnectedSession [switch]

```powershell
[Parameter(ParameterSetName = 'Set 2')]
[Parameter(ParameterSetName = 'Set 3')]
[Parameter(ParameterSetName = 'Set 4')]
[Parameter(ParameterSetName = 'Set 5')]
```

Runs a command or script in a disconnected session.
When you use the InDisconnectedSession parameter, Invoke-Command creates a persistent session on each remote computer, starts the command specified by the ScriptBlock or FilePath parameter, and then disconnects from the session. The commands continue to run in the disconnected sessions.The InDisconnectedSession parameter enables you to run commands without maintaining a connection to the remote sessions. Also, because the session is disconnected before any results are returned, the InDisconnectedSession parameter assures that all command results are returned to the reconnected session, instead of being split between sessions.
You cannot use the InDisconnectedSession parameter with the Session parameter or the AsJob parameter.
Commands that use the InDisconnectedSession parameter return a PSSession object that represents the disconnected session. They do not return the command output. To connect to the disconnected session, use the Connect-PSSession or Receive-PSSession cmdlets. To get the results of commands that ran in the session, use the Receive-PSSession cmdlet.To run commands that generate output in a disconnected session, set the value of the OutputBufferingMode session option to Drop. If you intend to connect to the disconnected session, set the idle timeout in the session so that it provides sufficient time for you to connect before deleting the session.
You can set the output buffering mode and idle timeout in the SessionOption parameter or in the $PSSessionOption preference variable. For more information about session options, see New-PSSessionOption and about_Preference_Variables.
For more information about the Disconnected Sessions feature, see about_Remote_Disconnected_Sessions.
This parameter is introduced in Windows PowerShell 3.0.

#### InputObject [PSObject]

```powershell
[Parameter(ValueFromPipeline = $true)]
```

Specifies input to the command. Enter a variable that contains the objects or type a command or expression that gets the objects.
When using InputObject, use the $input automatic variable in the value of the ScriptBlock parameter to represent the input objects.

#### JobName [String]

```powershell
[Parameter(ParameterSetName = 'Set 2')]
[Parameter(ParameterSetName = 'Set 3')]
[Parameter(ParameterSetName = 'Set 4')]
[Parameter(ParameterSetName = 'Set 5')]
[Parameter(ParameterSetName = 'Set 6')]
[Parameter(ParameterSetName = 'Set 7')]
```

Specifies a friendly name for the background job. By default, jobs are named "Job<n>", where <n> is an ordinal number.
If you use the JobName parameter in a command, the command is run as a job, and Invoke-Command returns a job object, even if you do not include the AsJob parameter in the command.
For more information about Windows PowerShell background jobs, see about_Jobs (http://go.microsoft.com/fwlink/?LinkID=113251).

#### Port [Int32]

```powershell
[Parameter(ParameterSetName = 'Set 4')]
[Parameter(ParameterSetName = 'Set 5')]
```

Specifies the network port  on the remote computer used for this command. To connect to a remote computer, the remote computer must be listening on the port that the connection uses.  The default ports are 5985 (the WinRM port for HTTP) and 5986 (the WinRM port for HTTPS).
Before using an alternate port, configure the WinRM listener on the remote computer to listen at that port. To configure the listener, type the following two commands at the Windows PowerShell prompt:
Remove-Item -Path WSMan:\Localhost\listener\listener* -Recurse
New-Item -Path WSMan:\Localhost\listener -Transport http -Address * -Port <port-number>
Do not use the Port parameter unless you must. The port that is set in the command applies to all computers or sessions on which the command runs. An alternate port setting might prevent the command from running on all computers.

#### ScriptBlock [ScriptBlock]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 1')]
[Parameter(
  Mandatory = $true,
  Position = 2,
  ParameterSetName = 'Set 2')]
[Parameter(
  Mandatory = $true,
  Position = 2,
  ParameterSetName = 'Set 5')]
[Parameter(
  Mandatory = $true,
  Position = 2,
  ParameterSetName = 'Set 7')]
```

Specifies the commands to run. Enclose the commands in braces ( { } ) to create a script block. This parameter is required.
By default, any variables in the command are evaluated on the remote computer. To include local variables in the command, use the ArgumentList parameter.

#### Session [PSSession[]]

```powershell
[Parameter(
  Position = 1,
  ParameterSetName = 'Set 6')]
[Parameter(
  Position = 1,
  ParameterSetName = 'Set 7')]
```

Runs the command in the specified Windows PowerShell sessions (PSSessions). Enter a variable that contains the PSSessions or a command that creates or gets the PSSessions, such as a New-PSSession or Get-PSSession command.
Runs the command in the specified Windows PowerShell sessions (PSSessions). Enter a variable that contains the PSSessions or a command that creates or gets the PSSessions, such as a New-PSSession or Get-PSSession command.
When you create a PSSession, Windows PowerShell establishes a persistent connection to the remote computer. Use a PSSession to run a series of related commands that share data. To run a single command or a series of unrelated commands, use the ComputerName parameter.
To create a PSSession, use the New-PSSession cmdlet. For more information, see about_PSSessions.

#### SessionOption [PSSessionOption]

```powershell
[Parameter(ParameterSetName = 'Set 2')]
[Parameter(ParameterSetName = 'Set 3')]
[Parameter(ParameterSetName = 'Set 4')]
[Parameter(ParameterSetName = 'Set 5')]
```

Sets advanced options for the session.  Enter a SessionOption object, such as one that you create by using the New-PSSessionOption cmdlet, or a hash table in which the keys are session option names and the values are session option values.
The default values for the options are determined by the value of the $PSSessionOption preference variable, if it is set. Otherwise, the default values are established by options set in the session configuration.
The session option values take precedence over default values for sessions set in the $PSSessionOption preference variable and in the session configuration. However, they do not take precedence over maximum values, quotas or limits set in the session configuration. 
For a description of the session options, including the default values, see New-PSSessionOption. For information about the $PSSessionOption preference variable, see about_Preference_Variables (http://go.microsoft.com/fwlink/?LinkID=113248). For more information about session configurations, see about_Session_Configurations (http://go.microsoft.com/fwlink/?LinkID=145152).

#### ThrottleLimit [Int32]

```powershell
[Parameter(ParameterSetName = 'Set 2')]
[Parameter(ParameterSetName = 'Set 3')]
[Parameter(ParameterSetName = 'Set 4')]
[Parameter(ParameterSetName = 'Set 5')]
[Parameter(ParameterSetName = 'Set 6')]
[Parameter(ParameterSetName = 'Set 7')]
```

Specifies the maximum number of concurrent connections that can be established to run this command. If you omit this parameter or enter a value of 0, the default value, 32, is used.
The throttle limit applies only to the current command, not to the session or to the computer.

#### UseSSL [switch]

```powershell
[Parameter(ParameterSetName = 'Set 4')]
[Parameter(ParameterSetName = 'Set 5')]
```

Uses the Secure Sockets Layer (SSL) protocol to establish a connection to the remote computer. By default, SSL is not used.
WS-Management encrypts all Windows PowerShell content transmitted over the network. UseSSL is an additional protection that sends the data across an HTTPS, instead of HTTP.
If you use this parameter, but SSL is not available on the port used for the command, the command fails.

#### NoNewScope [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Runs the specified command in the current scope. By default, Invoke-Command runs commands in their own scope.
This parameter is valid only in commands that are run in the current session, that is, commands that omit both the ComputerName and Session parameters.
This parameter is introduced in Windows PowerShell 3.0.

#### SessionName [String[]]

```powershell
[Parameter(ParameterSetName = 'Set 4')]
[Parameter(ParameterSetName = 'Set 5')]
```

Specifies a friendly name for a disconnected session. You can use the name to refer to the session in subsequent commands, such as a Get-PSSession command. This parameter is valid only with the InDisconnectedSession parameter.
This parameter is introduced in Windows PowerShell 3.0.


### INPUTS
#### System.Management.Automation.ScriptBlock
You can pipe a command in a script block to Invoke-Command. Use the $input automatic variable to represent the input objects in the command.

### OUTPUTS
#### System.Management.Automation.PSRemotingJob, System.Management.Automation.Runspaces.PSSession, or the output of the invoked command
When you use the AsJob parameter, Invoke-Command returns a job object. When you use the InDisconnectedSession parameter, Invoke-Command returns a PSSession object. Otherwise, it returns the output of the invoked command (the value of the ScriptBlock parameter).

### NOTES
On Windows Vista, and later versions of Windows, to use the ComputerName parameter of Invoke-Command to run a command on the local computer, you must open Windows PowerShell with the "Run as administrator" option.
When you run commands on multiple computers, Windows PowerShell connects to the computers in the order in which they appear in the list. However, the command output is displayed in the order that it is received from the remote computers, which might be different.
Errors that result from the command that Invoke-Command runs are included in the command results. Errors that would be terminating errors in a local command are treated as non-terminating errors in a remote command. This strategy ensures that terminating errors on one computer do not terminate the command on all computers on which it is run. This practice is used even when a remote command is run on a single computer.
If the remote computer is not in a domain that the local computer trusts, the computer might not be able to authenticate the user's credentials. To add the remote computer to the list of "trusted hosts" in WS-Management, use the following command in the WSMAN provider, where <Remote-Computer-Name> is the name of the remote computer:
Set-Item -Path WSMan:\Localhost\Client\TrustedHosts -Value <Remote-Computer-Name>
In Windows PowerShell 2.0, you cannot use the Select-Object cmdlet to select the PSComputerName property of the object that Invoke-Command returns. Instead, to display the value of the PSComputerName property, use the dot method to get the PSComputerName property value ($result.PSComputerName), use a Format cmdlet, such as the Format-Table cmdlet, to display the value of the PSComputerName property, or use a Select-Object command where the value of the property parameter is a calculated property that has a label other than "PSComputerName".
This limitation does not apply to Windows PowerShell 3.0 or later versions of Windows PowerShell.
When you disconnect a PSSession, such as by using the InDisconnectedSession parameter, the session state is Disconnected and the availability is None. 
The value of the State property is relative to the current session. Therefore, a value of Disconnected means that the PSSession is not connected to the current session. However, it does not mean that the PSSession is disconnected from all sessions. It might be connected to a different session. To determine whether you can connect or reconnect to the session, use the Availability property.
An Availability value of None indicates that you can connect to the session. A value of Busy indicates that you cannot connect to the PSSession because it is connected to another session.
For more information about the values of the State property of sessions, see "RunspaceState Enumeration" in MSDN at [http://msdn.microsoft.com/en-us/library/windows/desktop/system.management.automation.runspaces.runspacestate(v=VS.85).aspx]().
For more information about the values of the Availability property of sessions, see RunspaceAvailability Enumeration at [http://msdn.microsoft.com/en-us/library/windows/desktop/system.management.automation.runspaces.runspaceavailability(v=vs.85).aspx]().

### EXAMPLES
#### -------------------------- EXAMPLE 1 --------------------------

```powershell
PS C:\>Invoke-Command -FilePath c:\scripts\test.ps1 -ComputerName Server01

```
This command runs the Test.ps1 script on the Server01 computer.
The command uses the FilePath parameter to specify a script that is located on the local computer. The script runs on the remote computer and the results are returned to the local computer.


#### -------------------------- EXAMPLE 2 --------------------------

```powershell
PS C:\>Invoke-Command -ComputerName server01 -Credential domain01\user01 -ScriptBlock {Get-Culture}

```
This command runs a Get-Culture command on the Server01 remote computer.
It uses the ComputerName parameter to specify the computer name and the Credential parameter to run the command in the security context of "Domain01\User01," a user with permission to run commands. It uses the ScriptBlock parameter to specify the command to be run on the remote computer.
In response, Windows PowerShell displays a dialog box that requests the password and an authentication method for the User01 account. It then runs the command on the Server01 computer and returns the result.


#### -------------------------- EXAMPLE 3 --------------------------

```powershell
PS C:\>$s = New-PSSession -ComputerName Server02 -Credential Domain01\User01
PS C:\>Invoke-Command -Session $s -ScriptBlock {Get-Culture}

```
This example runs the same "Get-Culture" command in a session (a persistent connection) on the Server02 remote computer. Typically, you create a session only when you are running a series of commands on the remote computer.
The first command uses the New-PSSession cmdlet to create a session on the Server02 remote computer. Then, it saves the session in the $s variable.
The second command uses the Invoke-Command cmdlet to run the Get-Culture command on Server02. It uses the Session parameter to specify the session  saved in the $s variable.
In response, Windows PowerShell runs the command in the session on the Server02 computer.


#### -------------------------- EXAMPLE 4 --------------------------

```powershell
PS C:\>Invoke-Command -ComputerName Server02 -ScriptBlock {$p = Get-Process PowerShell}
PS C:\>Invoke-Command -ComputerName Server02 -ScriptBlock {$p.VirtualMemorySize}
PS C:\>
PS C:\>$s = New-PSSession -ComputerName Server02
PS C:\>Invoke-Command -Session $s -ScriptBlock {$p = Get-Process PowerShell}
PS C:\>Invoke-Command -Session $s -ScriptBlock {$p.VirtualMemorySize}
17930240

```
This example compares the effects of using ComputerName and Session parameters of Invoke-Command. It shows how to use a session to run a series of commands that share the same data.
The first two commands use the ComputerName parameter of Invoke-Command to run commands on the Server02 remote computer. The first command uses the Get-Process cmdlet to get the PowerShell process on the remote computer and to save it in the $p variable. The second command gets the value of the VirtualMemorySize property of the PowerShell process.
The first command succeeds. But the second command fails, because when you use the ComputerName parameter, Windows PowerShell creates a connection just to run the command. Then, it closes the connection when the command is complete. The $p variable was created in one connection, but it does not exist in the connection created for the second command.
The problem is solved by creating a session (a persistent connection) on the remote computer and by running both of the related commands in the same session.
The third command uses the New-PSSession cmdlet to create a session on the Server02 computer. Then it saves the session in the $s variable. The fourth and fifth commands repeat the series of commands used in the first set, but in this case, the Invoke-Command command uses the Session parameter to run both of the commands in the same session.
In this case, because both commands run in the same session, the commands succeed, and the $p value remains active in the $s session for later use.


#### -------------------------- EXAMPLE 5 --------------------------

```powershell
PS C:\>$command = { Get-EventLog -log "Windows PowerShell" | where {$_.Message -like "*certificate*"} }
PS C:\>Invoke-Command -ComputerName S1, S2 -ScriptBlock $command

```
This example shows how to enter a command that is saved in a local variable.
When the entire command is saved in a local variable, you can specify the variable as the value of the ScriptBlock parameter. You do not have to use the "param" keyword or the ArgumentList variable to submit the value of the local variable.
The first command saves a Get-EventLog command in the $command variable. The command is formatted as a script block.
The second command uses the Invoke-Command cmdlet to run the command in $command on the S1 and S2 remote computers.






#### -------------------------- EXAMPLE 6 --------------------------

```powershell
PS C:\>Invoke-Command -ComputerName Server01, Server02, TST-0143, localhost -ConfigurationName MySession.PowerShell -ScriptBlock {Get-EventLog "Windows PowerShell"}

```
This example demonstrates how to use the Invoke-Command cmdlet to run a single command on multiple computers.
The command uses the ComputerName parameter to specify the computers. The computer names are presented in a comma-separated list. The list of computers includes the "localhost" value, which represents the local computer.
The command uses the ConfigurationName parameter to specify an alternate session configuration for Windows PowerShell and the ScriptBlock parameter to specify the command.
In this example, the command in the script block gets the events in the Windows PowerShell event log on each remote computer.






#### -------------------------- EXAMPLE 7 --------------------------

```powershell
PS C:\>$version = Invoke-Command -ComputerName (Get-Content Machines.txt) -ScriptBlock {(Get-Host).Version}

```
This command gets the version of the Windows PowerShell host program running on 200 remote computers.
Because only one command is run, it is not necessary to create persistent connections (sessions) to each of the computers. Instead, the command uses the ComputerName parameter to indicate the computers.
The command uses the Invoke-Command cmdlet to run a Get-Host command. It uses dot notation to get the Version property of the Windows PowerShell host.
To specify the computers, it uses the Get-Content cmdlet to get the contents of the Machine.txt file, a file of computer names.
These commands run synchronously (one at a time). When the commands complete, the output of the commands from all of the computers is saved in the $version variable. The output includes the name of the computer from which the data originated.






#### -------------------------- EXAMPLE 8 --------------------------

```powershell
PS C:\>$s = New-PSSession -ComputerName Server01, Server02
PS C:\>Invoke-Command -Session $s -ScriptBlock {Get-EventLog system} -AsJob

Id   Name    State      HasMoreData   Location           Command
---  ----    -----      -----         -----------        ---------------
1    Job1    Running    True          Server01,Server02  Get-EventLog system

PS C:\>$j = Get-Job

PS C:\>$j | Format-List -Property *

HasMoreData   : True
StatusMessage :
Location      : Server01,Server02
Command       : Get-EventLog system
JobStateInfo  : Running
Finished      : System.Threading.ManualResetEvent
InstanceId    : e124bb59-8cb2-498b-a0d2-2e07d4e030ca
Id            : 1
Name          : Job1
ChildJobs     : {Job2, Job3}
Output        : {}
Error         : {}
Progress      : {}
Verbose       : {}
Debug         : {}
Warning       : {}
StateChanged  :

PS C:\>$results = $j | Receive-Job

```
These commands run a background job on two remote computers. Because the Invoke-Command command uses the AsJob parameter, the commands run on the remote computers, but the job actually resides on the local computer and the results are transmitted to the local computer.
The first command uses the New-PSSession cmdlet to create sessions on the Server01 and Server02 remote computers.
The second command uses the Invoke-Command cmdlet to run a background job in each of the sessions. The command uses the AsJob parameter to run the command as a background job. This command returns a job object that contains two child job objects, one for each of the jobs run on the two remote computers.
The third command uses a Get-Job command to save the job object in the $j variable.
The fourth command uses a pipeline operator (|) to send the value of the $j variable to the Format-List cmdlet, which displays all properties of the job object in a list.
The fifth command gets the results of the jobs. It pipes the job object in $j to the Receive-Job cmdlet and stores the results in the $results variable.






#### -------------------------- EXAMPLE 9 --------------------------

```powershell
PS C:\>$MWFO_Log = "Microsoft-Windows-Forwarding/Operational"
PS C:\>Invoke-Command -ComputerName Server01 -ScriptBlock {Get-EventLog -LogName $Using:MWFO_Log -Newest 10}

```
This example shows how to include the values of local variables in a command run on a remote computer. The command uses the Using scope modifier to identify a local variable in a remote command. By default, all variables are assumed to be defined in the remote session. The Using scope modifier was introduced in Windows PowerShell 3.0. For more information about the Using scope modifier, see about_Remote_Variables http://go.microsoft.com/fwlink/?LinkID=252653.
The first command saves the name of the Microsoft-Windows-Forwarding/Operational event log in the $MWFO_Log variable.
The second command uses the Invoke-Command cmdlet to run a Get-EventLog command on the Server01 remote computer that gets the 10 newest events from the Microsoft-Windows-Forwarding/Operational event log on Server01. The value of the LogName parameter is the $MWFO_Log variable, which is prefixed by the Using scope modifier to indicate that it was created in the local session, not in the remote session.


#### -------------------------- EXAMPLE 10 --------------------------

```powershell
PS C:\>Invoke-Command -ComputerName S1, S2 -ScriptBlock {Get-Process PowerShell}

PSComputerName    Handles  NPM(K)    PM(K)      WS(K) VM(M)   CPU(s)     Id   ProcessName
--------------    -------  ------    -----      ----- -----   ------     --   -----------
S1                575      15        45100      40988   200     4.68     1392 PowerShell
S2                777      14        35100      30988   150     3.68     67   PowerShell

PS C:\>Invoke-Command -ComputerName S1, S2 -ScriptBlock {Get-Process PowerShell} -HideComputerName

Handles  NPM(K)    PM(K)      WS(K) VM(M)   CPU(s)     Id   ProcessName
-------  ------    -----      ----- -----   ------     --   -----------
575      15        45100      40988   200     4.68     1392 PowerShell
777      14        35100      30988   150     3.68     67   PowerShell

```
This example shows the effect of using the HideComputerName parameter of Invoke-Command.
The first two commands use the Invoke-Command cmdlet to run a Get-Process command for the PowerShell process. The output of the first command includes the PsComputerName property, which contains the name of the computer on which the command ran. The output of the second command, which uses the HideComputerName parameter, does not include the PsComputerName column.
Using the HideComputerName parameter does not change the object; it just changes the display. You can still use the Format cmdlets to display the PsComputerName property of any of the affected objects.






#### -------------------------- EXAMPLE 11 --------------------------

```powershell
PS C:\>Invoke-Command -ComputerName (Get-Content Servers.txt) -FilePath C:\Scripts\Sample.ps1 -ArgumentList Process, Service

```
This example uses the Invoke-Command cmdlet to run the Sample.ps1 script on all of the computers listed in the Servers.txt file. The command uses the FilePath parameter to specify the script file. This command allows you to run the script on the remote computers, even if the script file is not accessible to the remote computers.
When you submit the command, the content of the Sample.ps1 file is copied into a script block and the script block is run on each of the remote computers. This procedure is equivalent to using the ScriptBlock parameter to submit the contents of the script.






#### -------------------------- EXAMPLE 12 --------------------------

```powershell
PS C:\>$LiveCred = Get-Credential

PS C:\>Invoke-Command -ConfigurationName Microsoft.Exchange -ConnectionUri https://ps.exchangelabs.com/PowerShell -Credential $LiveCred -Authentication Basic -ScriptBlock {Set-Mailbox Dan -DisplayName "Dan Park"}

```
This example shows how to run a command on a remote computer that is identified by a URI (Internet address). This particular example runs a Set-Mailbox command on a remote Exchange server. The backtick (`) in the command is the Windows PowerShell continuation character.
The first command uses the Get-Credential cmdlet to store Windows Live ID credentials in the $LiveCred variable. A credentials dialog box prompts the user to enter Windows Live ID credentials.
The second command uses the Invoke-Command cmdlet to run a Set-Mailbox command. The command uses the ConfigurationName parameter to specify that the command should run in a session that uses the Microsoft.Exchange session configuration. The ConnectionURI parameter specifies the URL of the Exchange server endpoint.
The Credential parameter specifies the Windows Live credentials stored in the $LiveCred variable. The AuthenticationMechanism parameter specifies the use of basic authentication. The ScriptBlock parameter specifies a script block that contains the command.


#### -------------------------- EXAMPLE 13 --------------------------

```powershell
PS C:\>$max = New-PSSessionOption -MaximumRedirection 1

PS C:\>Invoke-Command -ConnectionUri https://ps.exchangelabs.com/PowerShell -ScriptBlock {Get-Mailbox dan} -AllowRedirection -SessionOption $max

```
This command shows how to use the AllowRedirection and SessionOption parameters to manage URI redirection in a remote command.
The first command uses the New-PSSessionOption cmdlet to create a PSSessionOpption object that it saves in the $Max variable. The command uses the MaximumRedirection parameter to set the MaximumConnectionRedirectionCount property of the PSSessionOption object to 1.
The second command uses the Invoke-Command cmdlet to run a Get-Mailbox command on a remote server running Microsoft Exchange Server. The command uses the AllowRedirection parameter to provide explicit permission to redirect the connection to an alternate endpoint. It also uses the SessionOption parameter to specify the session object in the $max variable.
As a result, if the remote computer specified by the ConnectionURI parameter returns a redirection message, Windows PowerShell will redirect the connection, but if the new destination returns another redirection message, the redirection count value of 1 is exceeded, and Invoke-Command returns a non-terminating error.


#### -------------------------- EXAMPLE 14 --------------------------

```powershell
PS C:\>$so = New-PSSessionOption -SkipCACheck
PS C:\>Invoke-Command -Session $s -ScriptBlock { Get-Hotfix } -SessionOption $so -Credential server01\user01

```
This example shows how to create and use a SessionOption parameter.
The first command uses the New-PSSessionOption cmdlet to create a session option. It saves the resulting SessionOption object in the $so parameter.
The second command uses the Invoke-Command cmdlet to run a Get-HotFix command remotely. The value of the SessionOption parameter is the SessionOption object in the $so variable.


#### -------------------------- EXAMPLE 15 --------------------------

```powershell
PS C:\>Enable-WSManCredSSP -Delegate Server02
PS C:\>Connect-WSMan Server02
PS C:\>Set-Item WSMan:\Server02*\Service\Auth\CredSSP -Value $true
PS C:\>$s = New-PSSession Server02
PS C:\>Invoke-Command -Session $s -ScriptBlock {Get-Item \\Net03\Scripts\LogFiles.ps1} -Authentication CredSSP -Credential Domain01\Admin01

```
This example shows how to access a network share from within a remote session.
The command requires that CredSSP delegation be enabled in the client settings on the local computer and in the service settings on the remote computer. To run the commands in this example, you must be a member of the Administrators group on the local computer and the remote computer.
The first command uses the Enable-WSManCredSSP cmdlet to enable CredSSP delegation from the Server01 local computer to the Server02 remote computer. This configures the CredSSP client setting on the local computer.
The second command uses the Connect-WSMan cmdlet to connect to the Server02 computer. This action adds a node for the Server02 computer to the WSMan: drive on the local computer, allowing you to view and change the WS-Management settings on the Server02 computer.
The third command uses the Set-Item cmdlet to change the value of the CredSSP item in the Service node of the Server02 computer to True. This action enables CredSSP in the service settings on the remote computer.
The fourth command uses the New-PSSession cmdlet to create a PSSession on the Server02 computer. It saves the PSSession in the $s variable.
The fifth command uses the Invoke-Command cmdlet to run a Get-Item command in the session in $s that gets a script from the Net03\Scripts network share. The command uses the Credential parameter and it uses the Authentication parameter with a value of CredSSP.






#### -------------------------- EXAMPLE 16 --------------------------

```powershell
PS C:\>Invoke-Command -ComputerName (Get-Content Servers.txt) -InDisconnectedSession -FilePath \\Scripts\Public\ConfigInventory.ps1 -SessionOption @{OutputBufferingMode="Drop";IdleTimeout=43200000} 

```
This command runs a script on more than a hundred computers. To minimize the impact on the local computer, it connects to each computer, starts the script, and then disconnects from each computer. The script continues to run in the disconnected sessions.
The command uses the Invoke-Command cmdlet to run the script. The value of the ComputerName parameter is a Get-Content command that gets the names of the remote computers from a text file. The InDisconnectedSession parameter disconnects the sessions as soon as it starts the command. The value of the FilePath parameter is the script that Invoke-Command runs on each computer that is named in the Servers.txt file.
The value of the SessionOption parameter is a hash table that sets the value of the OutputBufferingMode option to Drop and the value of the IdleTimeout option to 43200000 milliseconds (12 hours).
To get the results of commands and scripts that run in disconnected sessions, use the Receive-PSSession cmdlet.



### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289592)
[Enter-PSSession]()
[Exit-PSSession]()
[Get-PSSession]()
[Invoke-Item]()
[New-PSSession]()
[Remove-PSSession]()
[WSMan Provider]()
[about_PSSessions]()
[about_Remote]()
[about_Remote_Disconnected_Sessions]()
[about_Remote_Variables]()
[about_Scopes]()

## Invoke-History

### SYNOPSIS
Runs commands from the session history.

### DESCRIPTION
The Invoke-History cmdlet runs commands from the session history. You can pass objects representing the commands from Get-History to Invoke-History, or you can identify commands in the current history by using their ID number. To find the identification number of a command, use the Get-History cmdlet.

### PARAMETERS

#### Id [String]

```powershell
[Parameter(
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 1')]
```

Identifies a command in the history. You can type the ID number of the command or the first few characters of the command.
If you type characters, Invoke-History matches the most recent commands first. If you omit this parameter, Invoke-History runs the last (most recent) command. The parameter name ("id") is optional. To find the ID number of a command, use the Get-History cmdlet.

#### Confirm [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Prompts you for confirmation before running the cmdlet.Prompts you for confirmation before running the cmdlet.

#### WhatIf [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Shows what would happen if the cmdlet runs. The cmdlet is not run.Shows what would happen if the cmdlet runs. The cmdlet is not run.


### INPUTS
#### System.String
You can pipe a history ID to Invoke-History.

### OUTPUTS
#### None
Invoke-History does not generate any output, but output might be generated by the commands that Invoke-History runs.

### NOTES
The session history is a list of the commands entered during the session along with the ID. The session history represents the order of execution, the status, and the start and end times of the command. As you enter each command, Windows PowerShell adds it to the history so that you can reuse it.  For more information about the session history, see about_History (http://go.microsoft.com/fwlink/?LinkID=113233).
You can also refer to Invoke-History by its built-in aliases, "r" and "ihy". For more information, see about_Aliases (http://go.microsoft.com/fwlink/?LinkID=113207).


### EXAMPLES
#### -------------------------- EXAMPLE 1 --------------------------

```powershell
PS C:\>Invoke-History

```
This command runs the last (most recent) command in the session history. You can abbreviate this command as "r" (think "repeat" or "rerun"), the alias for Invoke-History.






#### -------------------------- EXAMPLE 2 --------------------------

```powershell
PS C:\>Invoke-History -Id 132

```
This command runs the command in the session history with ID 132. Because the name of the Id parameter is optional, you can abbreviate this command as "Invoke-History 132", "ihy 132", or "r 132".






#### -------------------------- EXAMPLE 3 --------------------------

```powershell
PS C:\>Invoke-History get-pr

```
This command runs the most recent Get-Process command in the session history. When you type characters for the Id parameter, Invoke-History runs the first command that it finds that matches the pattern, beginning with the most recent commands. This command uses the Id parameter, but it omits the optional parameter name.






#### -------------------------- EXAMPLE 4 --------------------------

```powershell
PS C:\>16..24 | ForEach {Invoke-History -Id $_ }

```
This command runs commands 16 through 24. Because you can list only one ID value, the command uses the ForEach-Object cmdlet to run the Invoke-History command once for each ID value.






#### -------------------------- EXAMPLE 5 --------------------------

```powershell
PS C:\>Get-History -Id 255 -Count 7 | Invoke-History

```
This command runs the 7 commands in the history that end with command 255 (typically 249 through 255). It uses the Get-History cmdlet to retrieve the commands. The pipeline operator (|) passes the commands to Invoke-History, which executes them.







### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289593)
[Add-History]()
[Clear-History]()
[Get-History]()
[about_History]()

## New-Module

### SYNOPSIS
Creates a new dynamic module that exists only in memory.

### DESCRIPTION
The New-Module cmdlet creates a dynamic module from a script block. The members of the dynamic module, such as functions and variables, are immediately available in the session and remain available until you close the session.
Like static modules, by default, the cmdlets and functions in a dynamic module are exported and the variables and aliases are not. However, you can use the Export-ModuleMember cmdlet and the parameters of New-Module to override the defaults.
You can also use the AsCustomObject parameter of the New-Module cmdlet to return the dynamic module as a custom object. The members of the modules, such as functions, are implemented as script methods of the custom object instead of being imported into the session.
Dynamic modules  exist only in memory, not on disk. Like all modules, the members of dynamic modules run in a private module scope that is a child of the global scope. Get-Module cannot get a dynamic module, but Get-Command can get the exported members.
To make a dynamic module available to Get-Module, pipe a New-Module command to Import-Module, or pipe the module object that New-Module returns to Import-Module. This action adds the dynamic module to the Get-Module list, but it does not save the module to disk or make it persistent.

### PARAMETERS

#### ArgumentList [Object[]]

Specifies arguments (parameter values) that are passed to the script block.

#### AsCustomObject [switch]

Returns a custom object that represents the dynamic module. The module members are implemented as script methods of the custom object, but they are not imported into the session. You can save the custom object in a variable and use dot notation to invoke the members.
If the module has multiple members with the same name, such as a function and a variable that are both named "A," only one member with each name is accessible from the custom object.

#### Cmdlet [String[]]

Exports only the specified cmdlets from the module into the current session. Enter a comma-separated list of cmdlets. Wildcard characters are permitted. By default, all cmdlets in the module are exported.
You cannot define cmdlets in a script block, but a dynamic module can include cmdlets if it imports the cmdlets from a binary module.

#### Function [String[]]

Exports only the specified functions from the module into the current session. Enter a comma-separated list of functions. Wildcard characters are permitted. By default, all functions defined in a module are exported.

#### Name [String]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipeline = $true,
  ParameterSetName = 'Set 2')]
```

Specifies a name for the new module. You can also pipe a module name to New-Module.
The default value is an autogenerated name that begins with "__DynamicModule_" and is followed by a GUID that specifies the path to the dynamic module.

#### ReturnResult [switch]

Runs the script block and returns the script block results instead of returning a module object.

#### ScriptBlock [ScriptBlock]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 1')]
[Parameter(
  Mandatory = $true,
  Position = 2,
  ParameterSetName = 'Set 2')]
```

Specifies the contents of the dynamic module. Enclose the contents in braces ( { } ) to create a script block. This parameter is required.


### INPUTS
#### System.String
You can pipe a module name string to New-Module.

### OUTPUTS
#### System.Management.Automation.PSModuleInfo, System.Management.Automation.PSCustomObject, or None
By default, New-Module generates a PSModuleInfo object. If you use the AsCustomObject parameter, it generates a PSCustomObject object. If you use the ReturnResult parameter, it returns the result of evaluating the script block in the dynamic module.

### NOTES
You can also refer to New-Module by its alias, "nmo". For more information, see about_Aliases.


### EXAMPLES
#### -------------------------- EXAMPLE 1 --------------------------

```powershell
PS C:\>new-module -scriptblock {function Hello {"Hello!"}}

Name              : __DynamicModule_2ceb1d0a-990f-45e4-9fe4-89f0f6ead0e5
Path              : 2ceb1d0a-990f-45e4-9fe4-89f0f6ead0e5
Description       :
Guid              : 00000000-0000-0000-0000-000000000000
Version           : 0.0
ModuleBase        :
ModuleType        : Script
PrivateData       :
AccessMode        : ReadWrite
ExportedAliases   : {}
ExportedCmdlets   : {}
ExportedFunctions : {[Hello, Hello]}
ExportedVariables : {}
NestedModules     : {}

```
This command creates a new dynamic module with a function called "Hello". The command returns a module object that represents the new dynamic module.






#### -------------------------- EXAMPLE 2 --------------------------

```powershell
PS C:\>new-module -scriptblock {function Hello {"Hello!"}}

Name              : __DynamicModule_2ceb1d0a-990f-45e4-9fe4-89f0f6ead0e5
Path              : 2ceb1d0a-990f-45e4-9fe4-89f0f6ead0e5
Description       :
Guid              : 00000000-0000-0000-0000-000000000000
Version           : 0.0
ModuleBase        :
ModuleType        : Script
PrivateData       :
AccessMode        : ReadWrite
ExportedAliases   : {}
ExportedCmdlets   : {}
ExportedFunctions : {[Hello, Hello]}
ExportedVariables : {}
NestedModules     : {}
PS C:\>get-module
PS C:\>
PS C:\>get-command Hello

CommandType     Name   Definition
-----------     ----   ----------
Function        Hello  "Hello!"

```
This example demonstrates that dynamic modules are not returned by the Get-Module cmdlet, but the members that they export are returned by the Get-Command cmdlet.






#### -------------------------- EXAMPLE 3 --------------------------

```powershell
PS C:\>New-Module -scriptblock {$SayHelloHelp="Type 'SayHello', a space, and a name."; function SayHello ($name) { "Hello, $name" }; Export-ModuleMember -function SayHello -Variable SayHelloHelp}

PS C:\>$SayHelloHelp

Type 'SayHello', a space, and a name.

PS C:\>SayHello Jeffrey
Hello, Jeffrey

```
This command uses the Export-ModuleMember cmdlet to export a variable into the current session. Without the Export-ModuleMember command, only the function is exported.
The output shows that both the variable and the function were exported into the session.






#### -------------------------- EXAMPLE 4 --------------------------

```powershell
PS C:\>new-module -scriptblock {function Hello {"Hello!"}} -name GreetingModule | import-module
PS C:\>get-module

Name              : GreetingModule
Path              : d54dfdac-4531-4db2-9dec-0b4b9c57a1e5
Description       :
Guid              : 00000000-0000-0000-0000-000000000000
Version           : 0.0
ModuleBase        :
ModuleType        : Script
PrivateData       :
AccessMode        : ReadWrite
ExportedAliases   : {}
ExportedCmdlets   : {}
ExportedFunctions : {[Hello, Hello]}
ExportedVariables : {}
NestedModules     : {}

PS C:\>get-command hello

CommandType     Name                                                               Definition
-----------     ----                                                               ----------
Function        Hello                                                              "Hello!"

```
This command demonstrates that you can make a dynamic module available to the Get-Module cmdlet by piping the dynamic module to the Import-Module cmdlet.
The first command uses a pipeline operator (|) to send the module object that New-Module generates to the Import-Module cmdlet. The command uses the Name parameter of New-Module to assign a friendly name to the module. Because Import-Module does not return any objects by default, there is no output from this command.
The second command uses the Get-Module cmdlet to get the modules in the session. The result shows that Get-Module can get the new dynamic module.
The third command uses the Get-Command cmdlet to get the Hello function that the dynamic module exports.






#### -------------------------- EXAMPLE 5 --------------------------

```powershell
PS C:\>$m = new-module -scriptblock {function Hello ($name) {"Hello, $name"}; function Goodbye ($name) {"Goodbye, $name"}} -AsCustomObject
PS C:\>$m
PS C:\>$m | get-member
TypeName: System.Management.Automation.PSCustomObject

Name        MemberType   Definition
----        ----------   ----------
Equals      Method       bool Equals(System.Object obj)
GetHashCode Method       int GetHashCode()
GetType     Method       type GetType()
ToString    Method       string ToString()
Goodbye     ScriptMethod System.Object Goodbye();
Hello       ScriptMethod System.Object Hello();

PS C:\ps-test> $m.goodbye("Jane")
Goodbye, Jane

PS C:\ps-test> $m.hello("Manoj")
Hello, Manoj

```

This example shows how to use the AsCustomObject parameter of New-Module to generate a custom object with script methods that represent the exported functions.
The first command uses the New-Module cmdlet to generate a dynamic module with two functions, Hello and Goodbye. The command uses the AsCustomObject parameter to generate a custom object instead of the PSModuleInfo object that New-Module generates by default. The command saves the custom object in the $m variable.
The second command attempts to display the value of the $m variable. No content appears.
The third command uses a pipeline operator (|) to send the custom object to the Get-Member cmdlet, which displays the properties and methods of the custom object. The output shows that the object has script methods that represent the Hello and Goodbye functions.
The fourth and fifth commands use the script method format to call the Hello and Goodbye functions.






#### -------------------------- EXAMPLE 6 --------------------------

```powershell
PS C:\>new-module -scriptblock {function SayHello {"Hello, World!"}; SayHello} -returnResult
Hello, World!

```
This command uses the ReturnResult parameter to request the results of running the script block instead of requesting a module object.
The script block in the new module defines the SayHello function and then calls the function.







### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289594)
[Export-ModuleMember]()
[Get-Module]()
[Import-Module]()
[Remove-Module]()
[about_Modules]()

## New-ModuleManifest

### SYNOPSIS
Creates a new module manifest.

### DESCRIPTION
The New-ModuleManifest cmdlet creates a new module manifest (.psd1) file, populates its values, and saves the manifest file in the specified path.
Module authors can use this cmdlet to create a manifest for their module. A module manifest is a .psd1 file that contains a hash table. The keys and values in the hash table describe the contents and attributes of the module, define the prerequisites, and determine how the components are processed.  Manifests are not required for a module.
New-ModuleManifest creates a manifest that includes all of the commonly used manifest keys, so you can use the default output as a manifest template. To add or change values, or to add module keys that this cmdlet does not add, open the resulting file in a text editor.
Each parameter of this cmdlet (except for Path and PassThru) creates a module manifest key and its value. In a module manifest, only the ModuleVersion key is required. Unless specified in the parameter description, if you omit a parameter from the command, New-ModuleManifest creates a comment string for the associated value that has no effect.
In Windows PowerShell 2.0, New-ModuleManifest prompts you for the values of commonly used parameters that are not specified in the command, in addition to required parameter values. Beginning in Windows PowerShell 3.0, it prompts only when required parameter values are not specified.

### PARAMETERS

#### AliasesToExport [String[]]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Specifies the aliases that the module exports. Wildcards are permitted.
You can use this parameter to restrict the aliases that are exported by the module. It can remove aliases from the list of exported aliases, but it cannot add aliases to the list.
If you omit this parameter, New-ModuleManifest creates an AliasesToExport key with a value of * (all), meaning that all aliases that are exported by the module are exported by the manifest.

#### Author [String]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Specifies the module author.
If you omit this parameter, New-ModuleManifest creates an Author key with the name of the current user.

#### ClrVersion [Version]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Specifies the minimum version of the Common Language Runtime (CLR) of the Microsoft .NET Framework that the module requires.

#### CmdletsToExport [String[]]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Specifies the cmdlets that the module exports. Wildcards are permitted.
You can use this parameter to restrict the cmdlets that are exported by the module. It can remove cmdlets from the list of exported cmdlets, but it cannot add cmdlets to the list.
If you omit this parameter, New-ModuleManifest creates a CmdletsToExport key with a value of * (all), meaning that all cmdlets that are exported by the module are exported by the manifest.

#### CompanyName [String]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Identifies the company or vendor who created the module.
If you omit this parameter, New-ModuleManifest creates a CompanyName key with a value of "Unknown".

#### Copyright [String]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Specifies a copyright statement for the module.
If you omit this parameter, New-ModuleManifest creates a Copyright key with a value of  "(c) <year> <username>. All rights reserved." where <year> is the current year and <username> is the value of the Author key (if one is specified) or the name of the current user.

#### Description [String]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Describes the contents of the module.

#### DotNetFrameworkVersion [Version]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Specifies the minimum version of the Microsoft .NET Framework that the module requires.

#### FileList [String[]]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Specifies all items that are included in the module.
This key is designed to act as a module inventory. The files listed in the key are not automatically exported with the module.

#### FormatsToProcess [String[]]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Specifies the formatting files (.ps1xml) that run when the module is imported.
When you import a module, Windows PowerShell runs the Update-FormatData cmdlet with the specified files. Because formatting files are not scoped, they affect all session states in the session.

#### FunctionsToExport [String[]]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Specifies the functions that the module exports. Wildcards are permitted.
You can use this parameter to restrict the functions that are exported by the module. It can remove functions from the list of exported aliases, but it cannot add functions to the list.
If you omit this parameter, New-ModuleManifest creates an FunctionsToExport key with a value of * (all), meaning that all functions that are exported by the module are exported by the manifest.

#### Guid [Guid]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Specifies a unique identifier for the module. The GUID can be used to distinguish among modules with the same name.
If you omit this parameter, New-ModuleManifest creates a GUID key in the manifest and generates a GUID for the value.
To create a new GUID in Windows PowerShell, type "[guid]::NewGuid()".

#### HelpInfoUri [String]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Specifies the Internet address of the HelpInfo XML file for the module. Enter an Uniform Resource Identifier (URI) that begins with "http" or "https".
The   HelpInfo XML file supports the Updatable Help feature that was introduced in Windows PowerShell 3.0. It contains information about the location of downloadable help files for the module and the version numbers of the newest help files for each supported locale. For information about Updatable Help, see about_Updatable_Help (http://go.microsoft.com/fwlink/?LinkID=235801). For information about the HelpInfo XML file, see "Supporting Updatable Help" in MSDN.
This parameter is introduced in Windows PowerShell 3.0.

#### ModuleList [Object[]]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Lists all modules that are included in this module.
Enter each module name as a string or as a hash table with ModuleName and ModuleVersion keys. The hash table can also have an optional GUID key. You can combine strings and hash tables in the parameter value. For more information, see the examples.
This key is designed to act as a module inventory. The modules that are listed in the value of this key are not automatically processed.

#### ModuleVersion [Version]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Specifies the version of the module.
This parameter is not required by the cmdlet, but a ModuleVersion key is required in the manifest. If you omit this parameter, New-ModuleManifest creates a ModuleVersion key with a value of "1.0".

#### NestedModules [Object[]]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Specifies script modules (.psm1) and binary modules (.dll) that are imported into the module's session state. The files in the NestedModules key run in the order in which they are listed in the value.
Enter each module name as a string or as a hash table with ModuleName and ModuleVersion keys. The hash table can also have an optional GUID key. You can combine strings and hash tables in the parameter value. For more information, see the examples.
Typically, nested modules contain commands that the root module needs for its internal processing. By default, the commands in nested modules are exported from the module's session state into the caller's session state, but the root module can restrict the commands that it exports (for example, by using an Export-ModuleMembercommand).
Nested modules in the module session state are available to the root module, but they are not returned by a Get-Module command in the caller's session state.
Scripts (.ps1) that are listed in the NestedModules key are run in the module's session state, not in the caller's session state. To run a script in the caller's session state, list the script file name in the value of the ScriptsToProcess key in the manifest.

#### PassThru [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Writes the resulting module manifest to the console, in addition to creating a .psd1 file. By default, this cmdlet does not generate any output.

#### Path [String]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 1')]
```

Specifies the path and file name of the new module manifest. Enter a path and file name with a .psd1 file name extension, such as "$pshome\Modules\MyModule\MyModule.psd1". This parameter is required.
If you specify the path to an existing file, New-ModuleManifest replaces the file without warning unless the file has the read-only attribute.
The manifest should be located in the module's directory, and the manifest file name should be the same as the module directory name, but with a .psd1 file name extension.
Note: You cannot use variables, such as $pshome or $home, in response to a prompt for a Path parameter value.  To use a variable, include the Path parameter in the command.

#### PowerShellHostName [String]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Specifies the name of the Windows PowerShell host program that the module requires. Enter the name of the host program, such as "Windows PowerShell ISE Host" or "ConsoleHost". Wildcards are not permitted.
To find the name of a host program, in the program, type "$host.name".

#### PowerShellHostVersion [Version]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Specifies the minimum version of the Windows PowerShell host program that works with the module. Enter a version number, such as 1.1.

#### PowerShellVersion [Version]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Specifies the minimum version of Windows PowerShell that will work with this module. For example, you can enter 3.0, 4.0, or 5.0 as the value of this parameter.

#### PrivateData [Object]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Specifies data that is passed to the module when it is imported.

#### ProcessorArchitecture [ProcessorArchitecture]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Specifies the processor architecture that the module requires. Valid values are x86, AMD64, IA64, and None (unknown or unspecified).

#### RequiredAssemblies [String[]]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Specifies the assembly (.dll) files that the module requires. Enter the assembly file names. Windows PowerShell loads the specified assemblies before updating types or formats, importing nested modules, or importing the module file that is specified in the value of the RootModule key.
Use this parameter to list all the assemblies that the module requires, including assemblies that must be loaded to update any formatting or type files that are listed in the FormatsToProcess or TypesToProcess keys, even if those assemblies are also listed as binary modules in the NestedModules key.

#### RequiredModules [Object[]]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Specifies modules that must be in the global session state.  If the required modules are not in the global session state, Windows PowerShell imports them. If the required modules are not available, the Import-Module command fails.
Enter each module name as a string or as a hash table with ModuleName and ModuleVersion keys. The hash table can also have an optional GUID key. You can combine strings and hash tables in the parameter value. For more information, see the examples.
In Windows PowerShell 2.0, Import-Module does not import required modules automatically. It just verifies that the required modules are in the global session state.

#### ScriptsToProcess [String[]]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Specifies script (.ps1) files that run in the caller's session state when the module is imported. You can use these scripts to prepare an environment, just as you might use a login script.
To specify scripts that run in the module's session state, use the NestedModules key.

#### TypesToProcess [String[]]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Specifies the type files (.ps1xml) that run when the module is imported.
When you import the module, Windows PowerShell runs the Update-TypeData cmdlet with the specified files. Because type files are not scoped, they affect all session states in the session.

#### VariablesToExport [String[]]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Specifies the variables that the module exports. Wildcards are permitted.
You can use this parameter to restrict the variables that are exported by the module. It can remove variables from the list of exported variables, but it cannot add variables to the list.
If you omit this parameter, New-ModuleManifest creates a VariablesToExport key with a value of * (all), meaning that all variables that are exported by the module are exported by the manifest.

#### DefaultCommandPrefix [String]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Specifies a prefix that is prepended to the nouns of all commands in the module when they are imported into a session. Enter a prefix string. Prefixes prevent command name conflicts in a user's session.
Module users can override this prefix by specifying the Prefix parameter of the Import-Module cmdlet.
This parameter is introduced in Windows PowerShell 3.0.

#### RootModule [String]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Specifies the primary or "root" file of the module. Enter the file name of a script (.ps1), a script module (.psm1), a module manifest(.psd1), an assembly (.dll), a cmdlet definition XML file (.cdxml), or a workflow (.xaml). When the module is imported, the members that are exported from the root module file are imported into the caller's session state. 
If a module has a manifest file and no root file has been designated in the RootModule key, the manifest becomes the primary file for the module, and the module becomes a "manifest module" (ModuleType = Manifest).
To export members from .psm1 or .dll files in a module that has a manifest, the names of those files must be specified in the values of the RootModule or NestedModules keys in the manifest. Otherwise, their members are not exported.
Note: In Windows PowerShell 2.0, this key was called "ModuleToProcess." You can use the "RootModule" parameter name or its "ModuleToProcess" alias.

#### Confirm [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Prompts you for confirmation before running the cmdlet.Prompts you for confirmation before running the cmdlet.

#### WhatIf [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Shows what would happen if the cmdlet runs. The cmdlet is not run.Shows what would happen if the cmdlet runs. The cmdlet is not run.


### INPUTS
#### None
You cannot pipe input to this cmdlet.

### OUTPUTS
#### None or System.String
By default, New-ModuleManifest does not generate any output. However, if you use the PassThru parameter, it generates a System.String object representing the module manifest..

### NOTES
Module manifests are usually optional. However, a module manifest is required to export an assembly that is installed in the global assembly cache.
To add or change files in the $pshome\Modules directory (%Windir%\System32\WindowsPowerShell\v1.0\Modules), start Windows PowerShell with the "Run as administrator" option.
In Windows PowerShell 2.0, many parameters of New-ModuleManifest are mandatory, even though they are not required in a module manifest. In Windows PowerShell 3.0, only the Path parameter is  mandatory.
A "session" is an instance of the Windows PowerShell execution environment. A session can have one or more session states. By default, a session has only a global session state, but each imported module has its own session state. Session states allow the commands in a module to run without affecting the global session state.
The "caller's session state" is the session state into which a module is imported. Typically, it refers to the global session state, but when a module imports nested modules, the "caller" is the module and the "caller's session state" is the module's session state.

### EXAMPLES
#### -------------------------- EXAMPLE 1 --------------------------

```powershell
PS C:\>New-ModuleManifest -Path C:\Users\User01\Documents\WindowsPowerShell\Modules\Test-Module\Test-Module.psd1 -PassThru

## Module manifest for module 'TestModule'
## Generated by: User01
## Generated on: 1/24/2012
#@{
# Script module or binary module file associated with this manifest
# RootModule = ''
# Version number of this module.ModuleVersion = '1.0'
# ID used to uniquely identify this moduleGUID = 'd0a9150d-b6a4-4b17-a325-e3a24fed0aa9'
# Author of this moduleAuthor = 'User01'
# Company or vendor of this moduleCompanyName = 'Unknown'
# Copyright statement for this moduleCopyright = '(c) 2012 User01. All rights reserved.'
# Description of the functionality provided by this module
# Description = ''
# Minimum version of the Windows PowerShell engine required by this module
# PowerShellVersion = ''
# Name of the Windows PowerShell host required by this module
# PowerShellHostName = ''
# Minimum version of the Windows PowerShell host required by this module
# PowerShellHostVersion = ''
# Minimum version of the .NET Framework required by this module
# DotNetFrameworkVersion = ''
# Minimum version of the common language runtime (CLR) required by this module
# CLRVersion = ''
# Processor architecture (None, X86, Amd64) required by this module
# ProcessorArchitecture = ''
# Modules that must be imported into the global environment prior to importing this module
# RequiredModules = @()
# Assemblies that must be loaded prior to importing this module
# RequiredAssemblies = @()
# Script files (.ps1) that are run in the caller's environment prior to importing this module
# ScriptsToProcess = @()
# Type files (.ps1xml) to be loaded when importing this module
# TypesToProcess = @()
# Format files (.ps1xml) to be loaded when importing this module
# FormatsToProcess = @()
# Modules to import as nested modules of the module specified in RootModule/ModuleToProcess
# NestedModules = @()
# Functions to export from this moduleFunctionsToExport = '*'
# Cmdlets to export from this moduleCmdletsToExport = '*'
# Variables to export from this moduleVariablesToExport = '*'
# Aliases to export from this moduleAliasesToExport = '*'
# List of all modules packaged with this module# ModuleList = @()
# List of all files packaged with this module
# FileList = @()
# Private data to pass to the module specified in RootModule/ModuleToProcess
# PrivateData = ''
# HelpInfo URI of this module
# HelpInfoURI = ''
# Default prefix for commands exported from this module. Override the default prefix using Import-Module -Prefix.
# DefaultCommandPrefix = ''}


```
This command creates a new module manifest in the file that is specified by the Path parameter. The PassThru parameter sends the output to the pipeline as well as to the file.
The output shows the default values of all keys in the manifest.


#### -------------------------- EXAMPLE 2 --------------------------

```powershell
PS C:\>New-ModuleManifest -PowerShellVersion 1.0 -AliasesToExport JKBC, DRC, TAC -Path C:\ps-test\ManifestTest.psd1

```
This command creates a new module manifest. It uses the PowerShellVersion and AliasesToExport parameters to add values to the corresponding manifest keys.


#### -------------------------- EXAMPLE 3 --------------------------

```powershell
PS C:\>New-ModuleManifest -RequiredModules BitsTransfer,@{ModuleName="PSScheduledJob";ModuleVersion="1.0.0.0";GUID="50cdb55f-5ab7-489f-9e94-4ec21ff51e59"}

```
This example shows how to use the string and hash table formats of the  ModuleList, RequiredModules, and NestedModules parameter. You can combine strings and hash tables in the same parameter value.
This command commands creates a module manifest for a module that requires the BitsTransfer  and PSScheduledJob modules.
The command uses a string format to specify the name of the BitsTransfer module and the hash table format to specify the name, a GUID, and a version of the PSScheduledJob module.


#### -------------------------- EXAMPLE 4 --------------------------

```powershell
PS C:\>New-ModuleManifest -HelpInfoUri "http://http://go.microsoft.com/fwlink/?LinkID=603"

```
This example shows creates a module manifest for a module that supports the Updatable Help feature. This feature allows users to use the Update-Help and Save-Help cmdlets, which download help files for the module from the Internet and install them in the module.
The command uses the HelpInfoUri parameter to create a HelpInfoUri key in the module manifest. The value of the parameter and the key must begin with "http" or "https". This value tells the Updatable Help system where to find the HelpInfo XML updatable help information file for the module.
For information about Updatable Help, see about_Updatable_Help (http://go.microsoft.com/fwlink/?LinkID=235801). For information about the HelpInfo XML file, see "Supporting Updatable Help" in MSDN.


#### Example 5

```powershell
PS C:\>Get-Module PSScheduledJob -List | Format-List -Property *

LogPipelineExecutionDetails : 
FalseName                        : 
PSScheduledJobPath                        : C:\WINDOWS\system32\WindowsPowerShell\v1.0\Modules\PSScheduledJob\PSScheduledJob.psd1
Definition                  :
Description                 :
Guid                        : 50cdb55f-5ab7-489f-9e94-4ec21ff51e59
HelpInfoUri                 : http://go.microsoft.com/fwlink/?LinkID=223911
ModuleBase                  : C:\WINDOWS\system32\WindowsPowerShell\v1.0\Modules\PSScheduledJob
PrivateData                 :
Version                     : 1.0.0.0
ModuleType                  : 
BinaryAuthor                      : Microsoft Corporation
AccessMode                  : ReadWrite
ClrVersion                  : 4.0
CompanyName                 : Microsoft Corporation
Copyright                   : c Microsoft Corporation. All rights reserved.
DotNetFrameworkVersion      :
ExportedFunctions           : {}
ExportedCmdlets             : {[New-JobTrigger, New-JobTrigger], [Add-JobTrigger, Add-JobTrigger], [Remove-JobTrigger, Remove-JobTrigger], [Get-JobTrigger, Get-JobTrigger]...}
ExportedCommands            : {[New-JobTrigger, New-JobTrigger], [Add-JobTrigger, Add-JobTrigger], [Remove-JobTrigger, Remove-JobTrigger], [Get-JobTrigger, Get-JobTrigger]...}
FileList                    : {}
ModuleList                  : {}
NestedModules               : {}
PowerShellHostName          :
PowerShellHostVersion       :
PowerShellVersion           : 4.0
ProcessorArchitecture       : None
Scripts                     : {}
RequiredAssemblies          : {}
RequiredModules             : {}
RootModule                  : Microsoft.PowerShell.ScheduledJob.dll
ExportedVariables           : {}
ExportedAliases             : {}
ExportedWorkflows           : {}
SessionState                :
OnRemove                    :
ExportedFormatFiles         : {C:\WINDOWS\system32\WindowsPowerShell\v1.0\Modules\PSScheduledJob\PSScheduledJob.Format.ps1xml}
ExportedTypeFiles           : {C:\WINDOWS\system32\WindowsPowerShell\v1.0\Modules\PSScheduledJob\PSScheduledJob.types.ps1xml}

PS C:\>Get-Module -List | Format-Table -Property Name, PowerShellVersion

Name                                                        PowerShellVersion
----                                                        -----------------
ADDeploymentWF                                              4.0
AppLocker                                                   4.0
Appx                                                        4.0
BestPractices                                               4.0
BitsTransfer                                                4.0
BranchCache                                                 4.0
CimCmdlets                                                  4.0
DirectAccessClientComponents                                4.0
Dism                                                        4.0
DnsClient                                                   4.0
International                                               4.0
iSCSI                                                       4.0
IscsiTarget                                                 4.0
Kds                                                         4.0
Microsoft.PowerShell.Diagnostics                            4.0
Microsoft.PowerShell.Host                                   4.0
Microsoft.PowerShell.Management                             4.0…

```
This example shows how to get the module manifest values of a module -- essentially a "Get-ModuleManifest" command. Because the values in the module manifest are reflected in the values of properties of the module object, you can get the module manifest values by displaying the module object properties.
The first command uses the Get-Module cmdlet to get the PSScheduledJob module. The command uses the List parameter, because the module is installed, but not imported into the session. The command sends the module to the Format-List cmdlet, which displays all properties and values of the module object in a list.
The second command uses the Format-Table cmdlet to display the PowerShellVersion property of all installed modules in a table. The PowerShellVersion property is defined in the module manifest.



### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289595)
[Export-ModuleMember]()
[Get-Module]()
[Import-Module]()
[New-Module]()
[Remove-Module]()
[Test-ModuleManifest]()
[about_Modules]()

## New-PSSession

### SYNOPSIS
Creates a persistent connection to a local or remote computer.

### DESCRIPTION
The New-PSSession cmdlet creates a Windows PowerShell session (PSSession) on a local or remote computer.  When you create a PSSession, Windows PowerShell establishes a persistent connection to the remote computer.
Use a PSSession to run multiple commands that share data, such as a function or the value of a variable. To run commands in a PSSession, use the Invoke-Command cmdlet. To use the PSSession to interact directly with a remote computer, use the Enter-PSSession cmdlet. For more information, see about_PSSessions (http://go.microsoft.com/fwlink/?LinkID=135181).
You can run commands on a remote computer without creating a PSSession by using the ComputerName parameters of Enter-PSSession or Invoke-Command. When you use the ComputerName parameter, Windows PowerShell creates a temporary connection that is used for the command and is then closed.

### PARAMETERS

#### AllowRedirection [switch]

```powershell
[Parameter(ParameterSetName = 'Set 2')]
```

Allows redirection of this connection to an alternate Uniform Resource Identifier (URI).
When you use the ConnectionURI parameter, the remote destination can return an instruction to redirect to a different URI. By default, Windows PowerShell does not redirect connections, but you can use this parameter to allow it to redirect the connection.
You can also limit the number of times the connection is redirected by changing the MaximumConnectionRedirectionCount session option value. Use the  MaximumRedirection parameter of the New-PSSessionOption cmdlet or set the MaximumConnectionRedirectionCount property of the $PSSessionOption preference variable. The default value is 5.

#### ApplicationName [String]

```powershell
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 1')]
```

Specifies the application name segment of the connection URI. Use this parameter to specify the application name when you are not using the ConnectionURI parameter in the command.
The default value is the value of the $PSSessionApplicationName preference variable on the local computer. If this preference variable is not defined, the default value is "WSMAN". This value is appropriate for most uses. For more information, see about_Preference_Variables (http://go.microsoft.com/fwlink/?LinkID=113248).
The WinRM service uses the application name to select a listener to service the connection request. The value of this parameter should match the value of the URLPrefix property of a listener on the remote computer.

#### Authentication [AuthenticationMechanism]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
[Parameter(ParameterSetName = 'Set 2')]
```

Specifies the mechanism that is used to authenticate the user's credentials.   Valid values are "Default", "Basic", "Credssp", "Digest", "Kerberos", "Negotiate", and "NegotiateWithImplicitCredential".  The default value is "Default".
For more information about the values of this parameter, see the description of the System.Management.Automation.Runspaces.AuthenticationMechanism enumeration in the MSDN (Microsoft Developer Network) library at http://go.microsoft.com/fwlink/?LinkID=144382.
Caution: Credential Security Support Provider (CredSSP) authentication, in which the user's credentials are passed to a remote computer to be authenticated, is designed for commands that require authentication on more than one resource, such as accessing a remote network share. This mechanism increases the security risk of the remote operation. If the remote computer is compromised, the credentials that are passed to it can be used to control the network session.

#### CertificateThumbprint [String]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
[Parameter(ParameterSetName = 'Set 2')]
```

Specifies the digital public key certificate (X509) of a user account that has permission to perform this action. Enter the certificate thumbprint of the certificate.
Certificates are used in client certificate-based authentication. They can be mapped only to local user accounts; they do not work with domain accounts.
To get a certificate, use the Get-Item or Get-ChildItem command in the Windows PowerShell Cert: drive.

#### ComputerName [String[]]

```powershell
[Parameter(
  Position = 1,
  ParameterSetName = 'Set 1')]
```

Creates a persistent connection (PSSession) to the specified computer. If you enter multiple computer names, New-PSSession creates multiple PSSessions, one for each computer. The default is the local computer.
Type the NetBIOS name, an IP address, or a fully qualified domain name of one or more remote computers. To specify the local computer, type the computer name, "localhost", or a dot (.). When the computer is in a different domain than the user, the fully qualified domain name is required. You can also pipe a computer name (in quotes) to New-PSSession.
To use an IP address in the value of the ComputerName parameter, the command must include the Credential parameter. Also, the computer must be configured for HTTPS transport or the IP address of the remote computer must be included in the WinRM TrustedHosts list on the local computer. For instructions for adding a computer name to the TrustedHosts list, see "How to Add  a Computer to the Trusted Host List" in about_Remote_Troubleshooting (http://go.microsoft.com/fwlink/?LinkID=135188).
Note:  To include the local computer in the value of the ComputerName parameter, start Windows PowerShell with the "Run as administrator" option.

#### ConfigurationName [String]

```powershell
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 1')]
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 2')]
```

Specifies the session configuration that is used for the new PSSession.
Enter a configuration name or the fully qualified resource Uniform Resource Identifier (URI) for a session configuration. If you specify only the configuration name, the following schema URI is prepended:  http://schemas.microsoft.com/PowerShell.
The session configuration for a session is located on the remote computer. If the specified session configuration does not exist on the remote computer, the command fails.
The default value is the value of the $PSSessionConfigurationName preference variable on the local computer. If this preference variable is not set, the default is Microsoft.PowerShell. For more information, see about_Preference_Variables (http://go.microsoft.com/fwlink/?LinkID=113248).

#### ConnectionUri [Uri[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 2')]
```

Specifies a Uniform Resource Identifier (URI) that defines the connection endpoint for the session. The URI must be fully qualified.  The format of this string is as follows:
<Transport>://<ComputerName>:<Port>/<ApplicationName>
The default value is as follows:
http://localhost:5985/WSMAN
If you do not specify a ConnectionURI, you can use the UseSSL, ComputerName, Port, and ApplicationName parameters to specify the ConnectionURI values.
Valid values for the Transport segment of the URI are HTTP and HTTPS. If you specify a connection URI with a Transport segment, but do not specify a port, the session is created with standards ports: 80 for HTTP and 443 for HTTPS. To use the default ports for Windows PowerShell remoting, specify port 5985 for HTTP or 5986 for HTTPS.
If the destination computer redirects the connection to a different URI, Windows PowerShell prevents the redirection unless you use the AllowRedirection parameter in the command.

#### Credential [PSCredential]

```powershell
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 1')]
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 2')]
```

Specifies a user account that has permission to perform this action. The default is the current user.
Type a user name, such as "User01", "Domain01\User01", or "User@Domain.com", or enter a PSCredential object, such as one returned by the Get-Credentiall cmdlet.
When you type a user name, you will be prompted for a password.

#### EnableNetworkAccess [switch]

Adds an interactive security token to loopback sessions. The interactive token lets you run commands in the loopback session that get data from other computers. For example, you can run a command in the session that copies XML files from a remote computer to the local computer.
A "loopback session" is a PSSession that originates and terminates on the same computer. To create a loopback session, omit the ComputerName parameter or set its value to ".", "localhost", or the name of the local computer.
By default, loopback sessions are created with a network token, which might not provide sufficient permission to authenticate to remote computers.
The EnableNetworkAccess parameter is effective only in loopback sessions. If you use the EnableNetworkAccess parameter when creating a session on a remote computer, the command succeeds, but the parameter is ignored.
You can also allow remote access in a loopback session by using the CredSSP value of the Authentication parameter, which delegates the session credentials to other computers.
To protect the computer from malicious access, disconnected loopback sessions that have interactive tokens (those created with the EnableNetworkAccess parameter) can be reconnected only from the computer on which the session was created. Disconnected sessions that use CredSSP authentication can be reconnected from other computers. For more information, see Disconnect-PSSession.
This parameter is introduced in Windows PowerShell 3.0.

#### Name [String[]]

Specifies a friendly name for the PSSession.
You can use the name to refer to the PSSession when using other cmdlets, such as Get-PSSession and Enter-PSSession. The name is not required to be unique to the computer or the current session.

#### Port [Int32]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Specifies the network port on the remote computer that is used for this connection.  To connect to a remote computer, the remote computer must be listening on the port that the connection uses.  The default ports are 5985 (the WinRM port for HTTP) and 5986 (the WinRM port for HTTPS).
Before using an alternate port, you must configure the WinRM listener on the remote computer to listen at that port. Use the following commands to configure the listener:
1. winrm delete winrm/config/listener?Address=*+Transport=HTTP
2. winrm create winrm/config/listener?Address=*+Transport=HTTP @{Port="<port-number>"}
Do not use the Port parameter unless you must. The port setting in the command applies to all computers or sessions on which the command runs. An alternate port setting might prevent the command from running on all computers.

#### Session [PSSession[]]

```powershell
[Parameter(
  Position = 1,
  ParameterSetName = 'Set 3')]
```

Uses the specified PSSession as a model for the new PSSession. This parameter creates new PSSessions with the same properties as the specified PSSessions.
Enter a variable that contains the PSSessions or a command that creates or gets the PSSessions, such as a New-PSSession or Get-PSSession command.
The resulting PSSessions have the same computer name, application name, connection URI, port, configuration name, throttle limit, and Secure Sockets Layer (SSL) value as the originals, but they have a different display name, ID, and instance ID (GUID).

#### SessionOption [PSSessionOption]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
[Parameter(ParameterSetName = 'Set 2')]
```

Sets advanced options for the session.  Enter a SessionOption object, such as one that you create by using the New-PSSessionOption cmdlet, or a hash table in which the keys are session option names and the values are session option values.
The default values for the options are determined by the value of the $PSSessionOption preference variable, if it is set. Otherwise, the default values are established by options set in the session configuration.
The session option values take precedence over default values for sessions set in the $PSSessionOption preference variable and in the session configuration. However, they do not take precedence over maximum values, quotas or limits set in the session configuration. 
For a description of the session options, including the default values, see New-PSSessionOption. For information about the $PSSessionOption preference variable, see about_Preference_Variables (http://go.microsoft.com/fwlink/?LinkID=113248). For more information about session configurations, see about_Session_Configurations (http://go.microsoft.com/fwlink/?LinkID=145152).

#### ThrottleLimit [Int32]

Specifies the maximum number of concurrent connections that can be established to run this command. If you omit this parameter or enter a value of 0  (zero), the default value, 32, is used.
The throttle limit applies only to the current command, not to the session or to the computer.

#### UseSSL [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Uses the Secure Sockets Layer (SSL) protocol to establish a connection to the remote computer. By default, SSL is not used.
WS-Management encrypts all Windows PowerShell content transmitted over the network. UseSSL is an additional protection that sends the data across an HTTPS connection instead of an HTTP connection.
If you use this parameter, but SSL is not available on the port used for the command, the command fails.


### INPUTS
#### System.String, System.URI, System.Management.Automation.Runspaces.PSSession
You can pipe a computer name (string), ConnectionURI (URI), or session (PSSession) object to New-PSSession.

### OUTPUTS
#### System.Management.Automation.Runspaces.PSSession


### NOTES
This cmdlet uses the Windows PowerShell remoting infrastructure. To use this cmdlet, the local computer and any remote computers must be configured for Windows PowerShell remoting. For more information, see about_Remote_Requirements (http://go.microsoft.com/fwlink/?LinkID=135187).
To create a PSSession on the local computer, start Windows PowerShell with the "Run as administrator" option.
When you are finished with the PSSession, use the Remove-PSSession cmdlet to delete the PSSession and release its resources.


### EXAMPLES
#### -------------------------- EXAMPLE 1 --------------------------

```powershell
PS C:\>$s = New-PSSession

```
This command creates a new PSSession on the local computer and saves the PSSession in the $s variable.
You can now use this PSSession to run commands on the local computer.






#### -------------------------- EXAMPLE 2 --------------------------

```powershell
PS C:\>$Server01 = New-PSSession -ComputerName Server01

```
This command creates a new PSSession on the Server01 computer and saves it in the $Server01 variable.
When creating multiple PSSessions, assign them to variables with useful names. This will help you manage the PSSessions in subsequent commands.






#### -------------------------- EXAMPLE 3 --------------------------

```powershell
PS C:\>$s1, $s2, $s3 = New-PSSession -ComputerName Server1,Server2,Server3

```
This command creates three new PSSessions, one on each of the computers specified by the ComputerName parameter.
The command uses the assignment operator (=) to assign the new PSSessions to an array of variables: $s1, $s2, $s3. It assigns the Server01 PSSession to $s1, the Server02 PSSession to $s2, and the Server03 PSSession to $s3.
When you assign multiple objects to an array of variables, Windows PowerShell assigns each object to a variable in the array respectively. If there are more objects than variables, all remaining objects are assigned to the last variable. If there are more variables than objects, the remaining variables are empty (null).






#### -------------------------- EXAMPLE 4 --------------------------

```powershell
PS C:\>New-PSSession -ComputerName Server01 -Port 8081 -UseSSL -ConfigurationName E12

```
This command creates a new PSSession on the Server01 computer that connects to server port 8081 and uses the SSL protocol. The new PSSession uses an alternate session configuration called "E12".
Before setting the port, you must configure the WinRM listener on the remote computer to listen on port 8081. For more information, see the description of the Port parameter.






#### -------------------------- EXAMPLE 5 --------------------------

```powershell
PS C:\>New-PSSession -Session $s -Credential Domain01\User01

```
This command creates a new PSSession with the same properties as an existing PSSession. You can use this command format when the resources of an existing PSSession are exhausted and a new PSSession is needed to offload some of the demand.
The command uses the Session parameter of New-PSSession to specify the PSSession saved in the $s variable. It uses the credentials of the Domain1\Admin01 user to complete the command.






#### -------------------------- EXAMPLE 6 --------------------------

```powershell
PS C:\>$global:s = New-PSSession -ComputerName Server1.Domain44.Corpnet.Fabrikam.com -Credential Domain01\Admin01

```
This example shows how to create a PSSession with a global scope on a computer in a different domain.
By default, PSSessions created at the command line are created with local scope and PSSessions created in a script have script scope.
To create a PSSession with global scope, create a new PSSession and then store the PSSession in a variable that is cast to a global scope. In this case, the $s variable is cast to a global scope.
The command uses the ComputerName parameter to specify the remote computer. Because the computer is in a different domain than the user account, the full name of the computer is specified along with the credentials of the user.






#### -------------------------- EXAMPLE 7 --------------------------

```powershell
PS C:\>$rs = Get-Content C:\Test\Servers.txt | New-PSSession -ThrottleLimit 50

```
This command creates a PSSession on each of the 200 computers listed in the Servers.txt file and it stores the resulting PSSession in the $rs variable. The PSSessions have a throttle limit of 50.
You can use this command format when the names of computers are stored in a database, spreadsheet, text file, or other text-convertible format.






#### -------------------------- EXAMPLE 8 --------------------------

```powershell
PS C:\>$s = New-PSSession -URI http://Server01:91/NewSession -Credential Domain01\User01

```
This command creates a PSSession on the Server01 computer and stores it in the $s variable. It uses the URI parameter to specify the transport protocol, the remote computer, the port, and an alternate session configuration. It also uses the Credential parameter to specify a user account with permission to create a session on the remote computer.






#### -------------------------- EXAMPLE 9 --------------------------

```powershell
PS C:\>$s = New-PSSession -ComputerName (Get-Content Servers.txt) -Credential Domain01\Admin01 -ThrottleLimit 16
PS C:\>Invoke-Command -Session $s -ScriptBlock {Get-Process PowerShell} -AsJob

```
These commands create a set of PSSessions and then run a background job in each of the PSSessions.
The first command creates a new PSSession on each of the computers listed in the Servers.txt file. It uses the New-PSSession cmdlet to create the PSSession. The value of the ComputerName parameter is a command that uses the Get-Content cmdlet to get the list of computer names the Servers.txt file.
The command uses the Credential parameter to create the PSSessions with the permission of a domain administrator, and it uses the ThrottleLimit parameter to limit the command to 16 concurrent connections. The command saves the PSSessions in the $s variable.
The second command uses the AsJob parameter of the Invoke-Command cmdlet to start a background job that runs a "Get-Process PowerShell" command in each of the PSSessions in $s.
For more information about background jobs, see about_Jobs (http://go.microsoft.com/fwlink/?LinkID=113251) and about_Remote_Jobs (http://go.microsoft.com/fwlink/?LinkID=135184).






#### -------------------------- EXAMPLE 10 --------------------------

```powershell
PS C:\>New-PSSession -ConnectionURI https://management.exchangelabs.com/Management

```
This command creates a new PSSession that connects to a computer that is specified by a URI instead of a computer name.






#### -------------------------- EXAMPLE 11 --------------------------

```powershell
PS C:\>$so = New-PSSessionOption -SkipCACheck
PS C:\>New-PSSession -ConnectionUri https://management.exchangelabs.com/Management -SessionOption $so -Credential Server01\Admin01

```
This example shows how to create a session option object and use the  SessionOption parameter.
The first command uses the New-PSSessionOption cmdlet to create a session option. It saves the resulting SessionOption object in the $so parameter.
The second command uses the option in a new session. The command uses the New-PSSession cmdlet to create a new session. The value of the SessionOption parameter is the SessionOption object in the $so variable.







### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289596)
[Connect-PSSession]()
[Disconnect-PSSession]()
[Enter-PSSession]()
[Exit-PSSession]()
[Get-PSSession]()
[Invoke-Command]()
[Receive-PSSession]()
[Remove-PSSession]()
[about_PSSessions]()
[about_Remote]()

## New-PSSessionConfigurationFile

### SYNOPSIS
Creates a file that defines a session configuration.

### DESCRIPTION
The New-PSSessionConfigurationFile cmdlet creates a file of settings that define a session configuration and the environment of sessions that are created by using the session configuration. To use the file in a session configuration, use the Path parameters of the Register-PSSessionConfiguration or Set-PSSessionConfiguration cmdlets.
The session configuration file that New-PSSessionConfigurationFile creates is a human-readable text file that contains a hash table of the session configuration properties and values. The file has a .pssc file name extension. 
All parameters of New-PSSessionConfigurationFile are optional, except for the Path parameter. If you omit a parameter, the corresponding key in the session configuration file is commented-out, except where noted in the parameter description.
A "session configuration" also known as an "endpoint" is a collection of settings on the local computer that define the environment for Windows PowerShell sessions (PSSessions) that connect to (terminate at) the computer. All PSSessions use a session configuration. To specify a particular session configuration, use the ConfigurationName parameter of cmdlets that create a session, such as the New-PSSession cmdlet. 
A session configuration file makes it easy to define a session configuration without complex scripts or code assemblies. The settings in the file are used in addition to the optional startup script and any assemblies in the session configuration.
For more information about session configurations and session configuration files, see about_Session_Configurations (http://go.microsoft.com/fwlink/?LinkID=145152) and about_Session_Configuration_Files (http://go.microsoft.com/fwlink/?LinkID=236023).
This cmdlet is introduced in Windows PowerShell 3.0.

### PARAMETERS

#### AliasDefinitions [Hashtable[]]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Adds the specified aliases to sessions that use the session configuration. Enter a hash table with the following keys:
-- Name: Name of the alias. This key is required.
-- Value: The command that the alias represents. This key is required.
-- Description: A text string that describes the alias. This key is optional.
-- Options: Alias options. This key is optional. The default value is None. Valid values are None, ReadOnly, Constant, Private, or AllScope.
For example: @{Name="hlp";Value="Get-Help";Description="Gets help";Options="ReadOnly"}

#### AssembliesToLoad [String[]]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Specifies the assemblies to load into the sessions that use the session configuration.

#### Author [String]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Identifies the author of the session configuration or the configuration file. The default is the current user. The value of this parameter is visible in the session configuration file, but it is not a property of the session configuration object.

#### CompanyName [String]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Identifies the company that created the session configuration or the configuration file. The default value is "Unknown". The value of this parameter is visible in the session configuration file, but it is not a property of the session configuration object.

#### Copyright [String]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Adds a copyright to the session configuration file. The value of this parameter is visible in the session configuration file, but it is not a property of the session configuration object.
If you omit this parameter, New-PSSessionConfigurationFile generates a copyright statement by using the value of the Author parameter.

#### Description [String]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Describes the session configuration or the session configuration file. The value of this parameter is visible in the session configuration file, but it is not a property of the session configuration object.

#### EnvironmentVariables [Object]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Adds environment variables to the session. Enter a hash table in which the keys are the environment variable names and the values are the environment variable values.
For example: EnvironmentVariables=@{TestShare="\\Server01\TestShare"}

#### ExecutionPolicy [ExecutionPolicy]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Specifies the execution policy of sessions that use the session configuration. If you omit this parameter, the value of the ExecutionPolicy key in the session configuration file is "Restricted". For information about execution policies in Windows PowerShell, see about_Execution_Policies (http://go.microsoft.com/fwlink/?LinkID=135170).

#### FormatsToProcess [String[]]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Specifies the formatting files (.ps1xml) that run in sessions that use the session configuration. The value of this parameter must be a full or absolute path to the formatting files.

#### FunctionDefinitions [Hashtable[]]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Adds the specified functions to sessions that use the session configuration. Enter a hash table with the following keys:
-- Name: Name of the function. This key is required.
-- ScriptBlock: Function body. Enter a script block. This key is required.
-- Options: Function options. This key is optional. The default value is None. Valid values are None, ReadOnly, Constant, Private, or AllScope.
For example: @{Name="Get-PowerShellProcess";ScriptBlock={Get-Process PowerShell};Options="AllScope"}

#### Guid [Guid]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Specifies a unique identifier for the session configuration file. If you omit this parameter, New-PSSessionConfigurationFile generates a GUID for the file.
To create a new GUID in Windows PowerShell, type "[guid]::NewGuid()".

#### LanguageMode [PSLanguageMode]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Determines which elements of the Windows PowerShell language are permitted in sessions that use this session configuration. You can use this parameter to restrict the commands that particular users can run on the computer.
Valid values are:
-- FullLanguage: All language elements are permitted.
-- ConstrainedLanguage: Commands that contain scripts to be evaluated are not allowed. The ConstrainedLanguage mode restricts user access to Microsoft .NET Framework types, objects, or methods.
-- NoLanguage: Users may run cmdlets and functions, but are not permitted to use any language elements, such as script blocks, variables, or operators.

-- RestrictedLanguage: Users may run cmdlets and functions, but are not permitted to use script blocks or variables except for the following permitted variables: $PSCulture, $PSUICulture, $True, $False, and  $Null. Users may use only the basic comparison operators (-eq, -gt, -lt). Assignment statements, property references, and method calls are not permitted.
The default value of the LanguageMode parameter depends on the value of the SessionType parameter.
 
                        
-- Empty                            :  NoLanguage
-- RestrictedRemoteServer  :  NoLanguage
-- Default                          :  FullLanguage

#### ModulesToImport [Object[]]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Specifies the modules and snap-ins that are automatically imported into sessions that use the session configuration. 
By default, only the Microsoft.PowerShell.Core snap-in is imported into remote sessions, but unless the cmdlets are excluded, users can use the Import-Module and Add-PSSnapin cmdlets to add modules and snap-ins to the session.
Each module or snap-in in the value of this parameter can be represented by a string or as a hash table. A module string consists only of the name of the module or snap-in. A module hash table can include ModuleName, ModuleVersion, and GUID keys. Only the ModuleName key is required.
For example, the following value consists of a string and a hash table. Any combination of strings and hash tables, in any order, is valid.
"TroubleshootingPack", @{ModuleName="PSDiagnostics"; ModuleVersion="1.0.0.0";GUID="c61d6278-02a3-4618-ae37-a524d40a7f44"},
The value of the ModulesToImport parameter of the Register-PSSessionConfiguration cmdlet takes precedence over the value of the ModulesToImport key in the session configuration file.

#### Path [String]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 1')]
```

Specifies the path and file name of the session configuration file. The file must have a .pssc file name extension.

#### PowerShellVersion [Version]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Specifies the version of the Windows PowerShell engine in sessions that use the session configuration. Valid values are 2.0 and 3.0. If you omit this parameter, the PowerShellVersion key is commented-out and newest version of Windows PowerShell runs in the session.
The value of the PSVersion parameter of the Register-PSSessionConfiguration cmdlet takes precedence over the value of the PowerShellVersion key in the session configuration file.

#### SchemaVersion [Version]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Specifies the version of the session configuration file schema. The default value is "1.0.0.0". 

#### ScriptsToProcess [String[]]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Adds the specified scripts to sessions that use the session configuration. Enter the path and file names of the scripts. The value of this parameter must be a full or absolute path to script file names.

#### SessionType [SessionType]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Specifies the type of session that is created by using the session configuration. The default value is Default. Valid values are:
-- Empty: No modules or snap-ins are added to session by default. Use the parameters of this cmdlet to add modules, functions, scripts, and other features to the session. This option is designed for you to create custom sessions by adding selected command. If you do not add commands to an empty session, the session is limited to expressions and might not be usable.
-- Default: Adds the Microsoft.PowerShell.Core snap-in to the session. This snap-in includes the Import-Module and Add-PSSnapin cmdlets that users can use to import other modules and snap-ins unless you explicitly prohibit the use of the cmdlets.
-- RestrictedRemoteServer: Includes only the following proxy functions:  Exit-PSSession,Get-Command, Get-FormatData, Get-Help, Measure-Object, Out-Default, and Select-Object. Use the parameters of this cmdlet to add modules, functions, scripts, and other features to the session.

#### TypesToProcess [String[]]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Adds the specified type files (.ps1xml) to sessions that use the session configuration. Enter the type file names. The value of this parameter must be a full or absolute path to type file names.

#### VariableDefinitions [Object]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Adds the specified variables to sessions that use the session configuration. Enter a hash table with the following keys:
-- Name: Name of the variable. This key is required.
-- Value: Variable value. This key is required.
 
-- Options: Variable options. This key is optional. The default value is None. Valid values are None, ReadOnly, Constant, Private, or AllScope.
For example: @{Name="WarningPreference";Value="SilentlyContinue";Options="AllScope"}

#### VisibleAliases [String[]]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Limits the aliases in the session to those specified in the value of this parameter, plus any aliases that you define in the AliasDefinition parameter. Wildcards are supported. By default, all aliases that are defined by the Windows PowerShell engine and all aliases that modules export are visible in the session.
For example: VisibleAliases="gcm", "gp"
When any Visible parameter is included in the session configuration file, Windows PowerShell removes the Import-Module cmdlet and its "ipmo" alias from the session.

#### VisibleCmdlets [String[]]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Limits the cmdlets in the session to those specified in the value of this parameter. Wildcards are supported.
By default, all cmdlets that modules in the session export are visible in the session. Use the SessionType and ModulesToImport parameters to determine which modules and snap-ins are imported into the session.
When any Visible parameter is included in the session configuration file, Windows PowerShell removes the Import-Module cmdlet and its "ipmo" alias from the session.

#### VisibleFunctions [String[]]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Limits the functions in the session to those specified in the value of this parameter, plus any functions that you define in the FunctionDefinition parameter. Wildcards are supported.
By default, all functions that modules in the session export are visible in the session. Use the SessionType and ModulesToImport parameters to determine which modules and snap-ins are imported into the session.
When any Visible parameter is included in the session configuration file, Windows PowerShell removes the Import-Module cmdlet and its "ipmo" alias from the session.

#### VisibleProviders [String[]]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Limits the Windows PowerShell providers in the session to those specified in the value of this parameter. Wildcards are supported.
By default, all providers that modules in the session export are visible in the session. Use the SessionType and ModulesToImport parameters to determine which modules and snap-ins are imported into the session.
When any Visible parameter is included in the session configuration file, Windows PowerShell removes the Import-Module cmdlet and its "ipmo" alias from the session.


### INPUTS
#### None
This cmdlet does not take input from the pipeline.

### OUTPUTS
#### None
This cmdlet does not generate any output.

### NOTES
The Visible parameters, such as VisibleCmdlets and VisibleProviders, do not import items into the session. Instead, they select from among the items imported into the session. For example, if the value of the VisibleProviders parameter is the Certificate provider, but the ModulesToImport parameter doesn't specify the Microsoft.PowerShell.Security module that contains the Certificate provider, the Certificate provider is not visible in the session.
New-PSSessionConfigurationFile creates a session configuration file with a .pssc file name extension in the path that you specify in the Path parameter. When you use the session configuration file to create a session configuration, the Register-PSSessionConfiguration cmdlet copies the configuration file and saves an active copy of the file in the SessionConfig subdirectory of the $pshome directory.
The ConfigFilePath property of the session configuration contains the fully qualified path to the active session configuration file. You can edit the active configuration file in the $pshome directory at any time, either by using Windows PowerShell ISE or any text editor. The changes that you make affect all new sessions that use the session configuration, but not existing sessions.
Before using an edited session configuration file, use the Test-PSSessionConfigurationFile cmdlet to verify that the configuration file entries are valid.

### EXAMPLES
#### Example 1: Designing a specialized session

```powershell
PS C:\>New-PSSessionConfigurationFile -ModulesToImport DMSCmdlets, *Microsoft* -ScriptsToProcess \\Server01\Scripts\Get-DMSServers.ps1

```
The following command creates a session configuration file for IT technical sessions on a cloud-based document management server.
You can use the resulting file to create a customized session configuration on the server. The ACLs on the session configuration determine who can use the session configuration to create a session on the server. 
Customized sessions that include the cmdlets, functions and scripts that technical users need make it easier for those users to write scripts that automate common tasks.


#### Example 2: Restricting Language in a Session

```powershell
The first pair of commands uses the New-PSSessionConfigurationFile cmdlet to create two session configuration files. The first command creates a no-language file. The second command creates a restricted-language file. Other than the value of the LanguageMode parameter, the session configuration files are equivalent.
PS C:\>New-PSSessionConfigurationFile -Path .\NoLanguage.pssc -LanguageMode NoLanguage
 
                        
PS C:\>New-PSSessionConfigurationFile -Path .\RestrictedLanguage.pssc -LanguageMode RestrictedLanguage

The second pair of commands uses the configuration files to create session configurations on the local computer.
PS C:\>Register-PSSessionConfiguration -Path .\NoLanguage.pssc -Name NoLanguage -Force
 
                        
PS C:\>Register-PSSessionConfiguration -Path .\RestrictedLanguage.pssc -Name RestrictedLanguage -Force

The third pair of commands creates two sessions, each of which uses one of the session configurations that were created in the previous command pair.
PS C:\>$NoLanguage = New-PSSession -ComputerName Srv01 -ConfigurationName NoLanguage
 
                        
PS C:\>$RestrictedLanguage = New-PSSession -ComputerName Srv01 -ConfigurationName RestrictedLanguage

The seventh command uses the Invoke-Command cmdlet to run an If statement in the no-Language session. The command fails, because the language elements in the command are not permitted in a no-language session.
PS C:\>Invoke-Command -Session $NoLanguage {if ((Get-Date) -lt "1January2014") {"Before"} else {"After"} }
The syntax is not supported by this runspace. This might be because it is in no-language mode.
    + CategoryInfo          : ParserError: (if ((Get-Date) ...") {"Before"}  :String) [], ParseException
    + FullyQualifiedErrorId : ScriptsNotAllowed
    + PSComputerName        : localhost


The eighth command uses the Invoke-Command cmdlet to run the same If statement in the restricted-language session. Because these language elements are permitted in the restricted-language session, the command succeeds.
PS C:\>Invoke-Command -Session $RestrictedLanguage {if ((Get-Date) -lt "1January2014") {"Before"} else {"After"} }
Before

```
The commands in this example compare a no-language session to a restricted-language session. The example shows the effect of using the LanguageMode parameter of New-PSSessionConfigurationFile to limit the types of commands and statements that users can run in a session that uses a custom session configuration. 
To run the commands in this example, start Windows PowerShell with the "Run as administrator" option. This option is required to run the Register-PSSessionConfiguration cmdlet.


#### Example 3: Changing a Session Configuration File

```powershell
The first command uses the New-PSSessionConfigurationFile cmdlet to create a session configuration file that imports the required modules.
PS C:\>New-PSSessionConfigurationFile -Path .\New-ITTasks.pssc -ModulesToImport Microsoft*, ITTasks, PSScheduledJob

The second command uses the Set-PSSessionConfiguration cmdlet to replace the current .pssc file with the new one. Changes to the session configuration affects all sessions created after the change completes.
PS C:\>Set-PSSessionConfiguration -Name  ITTasks -Path .\New-ITTasks.pssc

```
This example shows how to change the session configuration file that is used in a session configuration. In this scenario, the administrator wants to add the PSScheduledJob module to sessions created with the ITTasks session configuration. Previously, these sessions had only the core modules and an internal "ITTasks" module.


#### Example 4: Editing a Session Configuration File

```powershell
The first command uses the Get-PSSessionConfiguration command to get the path to the configuration file for the ITConfig session configuration. The path is stored in the ConfigFilePath property of the session configuration.
PS C:\>(Get-PSSessionConfiguration -Name ITConfig).ConfigFilePath
C:\WINDOWS\System32\WindowsPowerShell\v1.0\SessionConfig\ITConfig_1e9cb265-dae0-4bd3-89a9-8338a47698a1.pssc

To edit the session configuration copy of the configuration file, you might need to edit the file permissions.In this case, the current user, who is a member of the Administrators group on the system, was explicitly granted full control of the file by using the following method: Right-click the file icon, and then click Properties. On the Security tab, click Edit, and then click Add. Add the user, and then, in the Full control column, click Allow.Now the user can edit the file. A new "slst" alias for the Select-String cmdlet is added to the file.
PS C:\>AliasDefinitions = @(@{Name='slst';Value='Select-String'})

The second command uses the Test-PSSessionConfigurationFile cmdlet to test the edited file. The command uses the Verbose parameter, which displays the file errors that the cmdlet detects, if any.In this case, the cmdlet returns True ($true), which indicates that it did not detect any errors in the file.
PS C:\>Test-PSSessionConfigurationFile -Path (Get-PSSessionConfiguration -Name ITConfig).ConfigFilePath
True

```
This example shows how to change a session configuration by editing the active session configuration copy of the configuration file.


#### Example 5: Sample Configuration File

```powershell
PS C:\>New-PSSessionConfigurationFile
-Path .\SampleFile.pssc
-Schema "1.0.0.0"
-Author "User01"
-Copyright "(c) Fabrikam Corporation. All rights reserved."
-CompanyName "Fabrikam Corporation"
-Description "This is a sample file."
-ExecutionPolicy AllSigned
-PowerShellVersion "3.0"
-LanguageMode FullLanguage
-SessionType Default
-EnvironmentVariables @{TESTSHARE="\\Test2\Test"}
-ModulesToImport @{ModuleName="PSScheduledJob"; ModuleVersion="1.0.0.0"; GUID="50cdb55f-5ab7-489f-9e94-4ec21ff51e59"}, PSDiagnostics
-AssembliesToLoad "System.Web.Services","FSharp.Compiler.CodeDom.dll"
-TypesToProcess "Types1.ps1xml","Types2.ps1xml"
-FormatsToProcess "CustomFormats.ps1xml"
-ScriptsToProcess "Get-Inputs.ps1"
-AliasDefinitions @{Name="hlp";Value="Get-Help";Description="Gets help.";Options="AllScope"},@{Name="Update";Value="Update-Help";Description="Updates help";Options="ReadOnly"}
-FunctionDefinitions @{Name="Get-Function";ScriptBlock={Get-Command -CommandType Function};Options="ReadOnly"}
-VariableDefinitions @{Name="WarningPreference";Value="SilentlyContinue"}
-VisibleAliases "c*","g*","i*","s*" `-VisibleCmdlets "Get*"
-VisibleFunctions "Get*"
-VisibleProviders "FileSystem","Function","Variable"
 
                      
@{
# Version number of the schema used for this configuration file
SchemaVersion = '1.0.0.0'

# ID used to uniquely identify this session configuration
GUID = 'f7039ffa-7e54-4382-b358-a393c75c30d3'

# Specifies the execution policy for this session configuration
ExecutionPolicy = 'AllSigned'

# Specifies the language mode for this session configuration
LanguageMode = 'FullLanguage'

# Initial state of this session configuration
SessionType = 'Default'

# Environment variables defined in this session configuration
EnvironmentVariables = @{
    TESTSHARE='\\Test2\Test'
}

# Author of this session configuration
Author = 'User01'

# Company associated with this session configuration
CompanyName = 'Fabrikam Corporation'

# Copyright statement for this session configuration
Copyright = '(c) Fabrikam Corporation. All rights reserved.'

# Description of the functionality provided by this session configuration
Description = 'This is a sample file.'

# Version of the Windows PowerShell engine used by this session configuration
PowerShellVersion = '3.0'

# Modules that will be imported
ModulesToImport = @{
    ModuleVersion='1.0.0.0'
    ModuleName='PSScheduledJob'
    GUID='50cdb55f-5ab7-489f-9e94-4ec21ff51e59'
}, 'PSDiagnostics'

# Assemblies that will be loaded in this session configuration
AssembliesToLoad = 'System.Web.Services', 'FSharp.Compiler.CodeDom.dll'

# Aliases visible in this session configuration
VisibleAliases = 'c*', 'g*', 'i*', 's*'

# Cmdlets visible in this session configuration
VisibleCmdlets = 'Get*'

# Functions visible in this session configuration
VisibleFunctions = 'Get*'

# Providers visible in this session configuration
VisibleProviders = 'FileSystem', 'Function', 'Variable'

# Aliases defined in this session configuration
AliasDefinitions = @(
@{
    Description='Gets help.'
    Name='hlp'
    Options='AllScope'
    Value='Get-Help'
}, 
@{ 
    Description='Updates help'
    Name='Update'
    Options='ReadOnly'
    Value='Update-Help'
}
)

# Functions defined in this session configuration
FunctionDefinitions = @(
@{
    Name='Get-Function'
    Options='ReadOnly'
    ScriptBlock={Get-Command -CommandType Function}
}
)

# Variables defined in this session configuration
VariableDefinitions = @(
@{
    Value='SilentlyContinue'
    Name='WarningPreference'

# Type files (.ps1xml) that will be loaded in this session configuration
TypesToProcess = 'C:\WINDOWS\System32\WindowsPowerShell\v1.0\SessionConfig\Types1.ps1xml', 'C:\WINDOWS\System32\WindowsPowerShell\v1.0\SessionConfig\Types2.ps1xml'

# Format files (.ps1xml) that will be loaded in this session configuration
FormatsToProcess = 'C:\WINDOWS\System32\WindowsPowerShell\v1.0\SessionConfig\CustomFormats.ps1xml'

# Specifies the scripts to execute after the session is configured
ScriptsToProcess = 'C:\WINDOWS\System32\WindowsPowerShell\v1.0\SessionConfig\Get-Inputs.ps1'
}


```
This example displays a New-PSSessionConfigurationFile command that uses all of the cmdlet parameters. It is included to show the correct input format for each parameter.
The resulting SampleFile.pssc is displayed in the output.



### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289597)
[Disable-PSSessionConfiguration]()
[Enable-PSSessionConfiguration]()
[Get-PSSessionConfiguration]()
[New-PSSessionConfigurationFile]()
[New-PSSessionConfigurationOption]()
[Register-PSSessionConfiguration]()
[Set-PSSessionConfiguration]()
[Test-PSSessionConfigurationFile]()
[Unregister-PSSessionConfiguration]()
[WSMan Provider]()
[about_Session_Configurations]()
[about_Session_Configuration_Files]()

## New-PSSessionOption

### SYNOPSIS
Creates an object that contains advanced options for a PSSession. 

### DESCRIPTION
The New-PSSessionOption cmdlet creates an object that contains advanced options for a user-managed session ("PSSession"). You can use the object as the value of the SessionOption parameter of cmdlets that create a PSSession, such as New-PSSession, Enter-PSSession, and Invoke-Command.
Without parameters, New-PSSessionOption generates an object that contains the default values for all of the options. Because all of the properties can be edited, you can use the resulting object as a template, and create standard option objects for your enterprise.
You can also save a session option object in the $PSSessionOption preference variable. The values of this variable establish new default values for the session options. They effective when no session options are set for the session and they take precedence over options set in the session configuration, but you can override them by specifying session options or a session option object in a cmdlet that creates a session. For more information about the $PSSessionOption preference variable, see about_Preference_Variables (http://go.microsoft.com/fwlink/?LinkID=113248).
When you use a session option object in a cmdlet that creates a session, the session option values take precedence over default values for sessions set in the $PSSessionOption preference variable and in the session configuration. However, they do not take precedence over maximum values, quotas or limits set in the session configuration. For more information about session configurations, see about_Session_Configurations (http://go.microsoft.com/fwlink/?LinkID=145152).

### PARAMETERS

#### ApplicationArguments [PSPrimitiveDictionary]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Specifies a primitive dictionary that is sent to the remote session. Commands and scripts in the remote session, including startup scripts in the session configuration, can find this dictionary in the ApplicationArguments property of the $PSSenderInfo automatic variable. You can use this parameter to send data to the remote session.
A primitive dictionary is like a hash table, but it contains keys that are case-insensitive strings and values that can be serialized and deserialized during Windows PowerShell remoting handshakes. If you enter a hash table for the value of this parameter, Windows PowerShell converts it to a primitive dictionary.
For more information, see about_Hash_Tables (http://go.microsoft.com/fwlink/?LinkID=135175), about_Session_Configurations (http://go.microsoft.com/fwlink/?LinkID=145152), and about_Automatic_Variables (http://go.microsoft.com/fwlink/?LinkID=113212).

#### CancelTimeout [Int32]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Determines how long Windows PowerShell waits for a cancel operation (CTRL + C)  to complete before terminating it. Enter a value in milliseconds.
The default value is 60000 (one minute). A value of 0 (zero) means no timeout; the command continues indefinitely.

#### Culture [CultureInfo]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Specifies the culture to use for the session. Enter a culture name in <languagecode2>-<country/regioncode2> format, such as "ja-jP",  a variable that contains a CultureInfo object, or a command that gets a CultureInfo object, such as "get-culture".
The default value is $null, and the culture that is set in the operating system  is used in the session.

#### IdleTimeout [Int32]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Determines how long the session stays open if the remote computer does not receive any communication from the local computer, including the heartbeat signal. When the interval expires, the session closes.
The idle timeout value is of significant importance if you intend to disconnect and reconnect to a session. You can reconnect only if the session has not timed out.
Enter a value in milliseconds. The minimum value is 60000 (1 minute). The maximum is the value of the MaxIdleTimeoutms property of the session configuration. The default value, -1, does not set an idle timeout.
The session uses the idle timeout that is set in the session options, if any. If none is set (-1), the session uses the value of the IdleTimeoutMs property of the session configuration or the WSMan shell timeout value (WSMan:\<ComputerName>\Shell\IdleTimeout), whichever is shortest.
If the idle timeout set in the session options exceeds the value of the MaxIdleTimeoutMs property of the session configuration, the command to create a session fails.
The IdleTimeoutMs value of the default Microsoft.PowerShell session configuration is 7200000 milliseconds (2 hours). Its MaxIdleTimeoutMs value is 2147483647 milliseconds (>24 days). The default value of the WSMan shell idle timeout (WSMan:\<ComputerName>\Shell\IdleTimeout) is 7200000 milliseconds (2 hours).
The idle timeout value of a session can also be changed when disconnecting from a session or reconnecting to a session. For more information, see Disconnect-PSSession and Connect-PSSession.
In Windows PowerShell 2.0, the default value of the IdleTimeout parameter is 240000 (4 minutes).

#### MaximumReceivedDataSizePerCommand [Int32]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Specifies the maximum number of bytes that the local computer can receive from the remote computer in a single command. Enter a value in bytes. By default, there is no data size limit.
This option is designed to protect the resources on the client computer.

#### MaximumReceivedObjectSize [Int32]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Specifies the maximum size of an object that the local computer can receive from the remote computer. This option is designed to protect the resources on the client computer. Enter a value in bytes.
In Windows PowerShell 2.0, if you omit this parameter, there is no object size limit. Beginning in Windows PowerShell 3.0, if you omit this parameter, the default value is 200 MB.


#### MaximumRedirection [Int32]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Determines how many times Windows PowerShell redirects a connection to an alternate Uniform Resource Identifier (URI) before the connection fails. The default value is 5. A value of 0 (zero) prevents all redirection.
This option is used in the session only when the AllowRedirection parameter is used in the command that creates the session.

#### NoCompression [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Turns off packet compression in the session. Compression uses more processor cycles, but it makes transmission faster.

#### NoEncryption [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Turns off data encryption.

#### NoMachineProfile [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Prevents loading the user's Windows user profile. As a result, the session might be created faster, but user-specific registry settings, items such as environment variables, and certificates are not available in the session.

#### OpenTimeout [Int32]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Determines how long the client computer waits for the session connection to be established. When the interval expires, the command to establish the connection fails. Enter a value in milliseconds.
The default value is 180000 (3 minutes). A value of 0 (zero) means no time-out; the command continues indefinitely.

#### OperationTimeout [Int32]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Determines the maximum time that any operation in the session can run. When the interval expires, the operation fails. Enter a value in milliseconds.
The default value is 180000 (3 minutes). A value of 0 (zero) means no time-out; the operation continues indefinitely.

#### OutputBufferingMode [OutputBufferingMode]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Determines how command output is managed in disconnected sessions when the output buffer becomes full.
If the output buffering mode is not set in the session or in the session configuration, the default value is Block. Users can also change the output buffering mode when disconnecting the session.
If you omit this parameter, the value of the OutputBufferingMode of the session option object is None. A value of Block or Drop overrides the output buffering mode transport option set in the session configuration.
Valid values are: 
-- Block: When the output buffer is full, execution is suspended until the buffer is clear. 
-- Drop: When the output buffer is full, execution continues. As new output is saved, the oldest output is discarded.
-- None: No output buffering mode is specified.
For more information about the output buffering mode transport option, see New-PSTransportOption.
This parameter is introduced in Windows PowerShell 3.0.

#### ProxyAccessType [ProxyAccessType]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Determines which mechanism is used to resolve the host name. Valid values are IEConfig, WinHttpConfig, AutoDetect, NoProxyServer and None. The default value is None.
For information about the values of this parameter, see the description of the System.Management.Automation.Remoting.ProxyAccessType enumeration in the MSDN (Microsoft Developer Network) Library at http://go.microsoft.com/fwlink/?LinkId=144756.

#### ProxyAuthentication [AuthenticationMechanism]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Specifies the authentication method that is used for proxy resolution. Valid values are Basic, Digest, and Negotiate. The default value is Negotiate.
For information about the values of this parameter, see the description of the System.Management.Automation.Runspaces.AuthenticationMechanism enumeration in the MSDN library at http://go.microsoft.com/fwlink/?LinkID=144382.

#### ProxyCredential [PSCredential]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Specifies the credentials to use for proxy authentication. Enter a variable that contains a PSCredential object or a command that gets a PSCredential object, such as a Get-Credential command. If this option is not set, no credentials are specified.

#### SkipCACheck [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Specifies that when connecting over HTTPS, the client does not validate that the server certificate is signed by a trusted certification authority (CA).
Use this option only when the remote computer is trusted by using another mechanism, such as when the remote computer is part of a network that is physically secure and isolated or when the remote computer is listed as a trusted host in a WinRM configuration.

#### SkipCNCheck [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Specifies that the certificate common name (CN) of the server does not need to match the hostname of the server. This option is used only in remote operations that use the HTTPS protocol.
Use this option only for trusted computers.

#### SkipRevocationCheck [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Does not validate the revocation status of the server certificate.

#### UICulture [CultureInfo]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Specifies the UI culture to use for the session.
Enter a culture name in <languagecode2>-<country/regioncode2> format, such as "ja-jP",  a variable that contains a CultureInfo object, or a command that gets a CultureInfo object, such as a Get-Culture command.
The default value is $null, and the UI culture that is set in the operating system when the session is created is used in the session.

#### UseUTF16 [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Encode the request in UTF16 format rather than UTF8 format.

#### IncludePortInSPN [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Includes the port number in the Service Principal Name (SPN) used for Kerberos authentication, for example, “HTTP/<ComputerName>:5985”. This option allows a client that uses a non-default SPN to authenticate against a remote computer that uses Kerberos authentication.
The option is designed for enterprises where multiple services that support Kerberos authentication are running under different user accounts. For example, an IIS application that allows Kerberos authentication can require the default SPN to be registered to a user account that is different from the computer account. In such cases, Windows PowerShell remoting cannot use Kerberos to authenticate because it requires an SPN that is registered to the computer account. To resolve this problem, administrators can create different SPNs (such as by using Setspn.exe) that are registered to different user accounts and can distinguish between them by including the port number in the SPN.
For more information about SetSPN.exe, see "SetSPN Overview" at http://go.microsoft.com/fwlink/?LinkID=189413.
This parameter is introduced in Windows PowerShell 3.0.


### INPUTS
#### None
You cannot pipe input to this cmdlet.

### OUTPUTS
#### System.Management.Automation.Remoting.PSSessionOption


### NOTES
If the SessionOption parameter is not used in a command to create a PSSession, the session options are determined by the property values of the $PSSessionOption preference variable, if it is set. For more information about the $PSSessionOption variable, see about_Preference_Variables (http://go.microsoft.com/fwlink/?LinkID=113248).
The properties of a session configuration object vary with the options set for the session configuration and the values of those options. Also, session configurations that use a session configuration file have additional properties.

### EXAMPLES
#### -------------------------- EXAMPLE 1 --------------------------

```powershell
PS C:\>New-PSSessionOption

MaximumConnectionRedirectionCount : 5
NoCompression                     : False
NoMachineProfile                  : False
ProxyAccessType                   : IEConfig
ProxyAuthentication               : Negotiate
ProxyCredential                   :
SkipCACheck                       : False
SkipCNCheck                       : False
SkipRevocationCheck               : False
OperationTimeout                  : 00:03:00
NoEncryption                      : False
UseUTF16                          : False
Culture                           :
UICulture                         :
MaximumReceivedDataSizePerCommand :
MaximumReceivedObjectSize         :
ApplicationArguments              :
OpenTimeout                       : 00:03:00
CancelTimeout                     : 00:01:00
IdleTimeout                       : 00:04:00

```
This command creates a session option object with all of the default values.






#### -------------------------- EXAMPLE 2 --------------------------

```powershell
PS C:\>$pso = New-PSSessionOption -Culture "fr-fr" -MaximumReceivedObjectSize 10MB
PS C:\>New-PSSession -ComputerName Server01 -SessionOption $pso

```
This example shows how to use a session option object to configure a session.
The first command creates a new session option object and saves it in the value of the $pso variable.
The second command uses the New-PSSession cmdlet to create a session on the Server01 remote computer. The command uses the session option object in the value of the $pso variable as the value of the SessionOption parameter of the command.






#### -------------------------- EXAMPLE 3 --------------------------

```powershell
PS C:\>Enter-PSSession -ComputerName Server01 -SessionOption (New-PSSessionOption -NoEncryption -NoCompression)

```
This command uses the Enter-PSSession cmdlet to start an interactive session with the Server01 computer. The value of the SessionOption parameter is a New-PSSessionOption command with the NoEncryption and NoCompression switch parameters.
The New-PSSessionOption command is enclosed in parentheses to make sure that it runs before the Enter-PSSession command.






#### -------------------------- EXAMPLE 4 --------------------------

```powershell
PS C:\>$a = New-PSSessionOption

MaximumConnectionRedirectionCount : 5
NoCompression                     : False
NoMachineProfile                  : False
ProxyAccessType                   : IEConfig
ProxyAuthentication               : Negotiate
ProxyCredential                   :
SkipCACheck                       : False
SkipCNCheck                       : False
SkipRevocationCheck               : False
OperationTimeout                  : 00:03:00
NoEncryption                      : False
UseUTF16                          : False
Culture                           :
UICulture                         :
MaximumReceivedDataSizePerCommand :
MaximumReceivedObjectSize         :
ApplicationArguments              :
OpenTimeout                       : 00:03:00
CancelTimeout                     : 00:01:00
IdleTimeout                       : 00:04:00

PS C:\>$a.UICulture = (Get-UICulture)
PS C:\>$a.OpenTimeout = (New-Timespan -Minutes 4)
PS C:\>$a.MaximumConnectionRedirectionCount = 1
PS C:\>$a

MaximumConnectionRedirectionCount : 1
NoCompression                     : False
NoMachineProfile                  : False
ProxyAccessType                   : IEConfig
ProxyAuthentication               : Negotiate
ProxyCredential                   :
SkipCACheck                       : False
SkipCNCheck                       : False
SkipRevocationCheck               : False
OperationTimeout                  : 00:03:00
NoEncryption                      : False
UseUTF16                          : False
Culture                           :
UICulture                         : en-US
MaximumReceivedDataSizePerCommand :
MaximumReceivedObjectSize         :
ApplicationArguments              :
OpenTimeout                       : 00:04:00
CancelTimeout                     : 00:01:00
IdleTimeout                       : 00:04:00

```
This example demonstrates that you can edit the session option object. All properties have read/write values.
Use this method to create a standard session object for your enterprise, and then create customized versions of it for particular uses.






#### -------------------------- EXAMPLE 5 --------------------------

```powershell
PS C:\>$PSSessionOption = New-PSSessionOption -OpenTimeOut 120000

```
This command creates a $PSSessionOption preference variable.
When the $PSSessionOption preference variable exists in the session, it establishes default values for options in the sessions that are created by using the New-PSSession, Enter-PSSession, and Invoke-Command cmdlets.
To make the $PSSessionOption variable available in all sessions, add it to your Windows PowerShell session and to your Windows PowerShell profile.
For more information about the $PSSessionOption preference variable, see about_Preference_Variables (http://go.microsoft.com/fwlink/?LinkID=113248). For more information about profiles, see about_Profiles (http://go.microsoft.com/fwlink/?LinkID=113729).






#### -------------------------- EXAMPLE 6 --------------------------

```powershell
PS C:\>$skipCN = New-PSSessionOption -SkipCNCheck
PS C:\>New-PSSession -ComputerName 171.09.21.207 -UseSSL -Credential Domain01\User01 -SessionOption $SkipCN

```
This example shows how to use a SessionOption object to fulfill the requirements for a remote session configuration.
The first command uses the New-PSSessionOption cmdlet to create a session option object with the SkipCNCheck property. The command saves the resulting session object in the $skipCN variable.
The second command uses the New-PSSession cmdlet to create a new session on a remote computer. The $skipCN check variable is used in the value of the SessionOption parameter.
Because the computer is identified by its IP address, the value of the ComputerName parameter does not match any of the common names in the certificate used for Secure Sockets Layer (SSL). As a result, the SkipCNCheck option is required.






#### -------------------------- EXAMPLE 7 --------------------------

```powershell
PS C:\>$team = @{Team="IT"; Use="Testing"}
PS C:\>$TeamOption = New-PSSessionOption -ApplicationArguments $team
PS C:\>$s = New-PSSession -ComputerName Server01 -SessionOption $TeamOption
PS C:\>Invoke-Command -Session $s {$PSSenderInfo.SpplicationArguments}

Name                 Value
----                 -----
Team                 IT
Use                  Testing
PSVersionTable       {CLRVersion, BuildVersion, PSVersion, WSManStackVersion...}

PS C:\>Invoke-Command -Session $s {if ($PSSenderInfo.ApplicationArguments.Use -ne "Testing") {.\logFiles.ps1} else {"Just testing."}}
Just testing.

```
This example shows how to use the ApplicationArguments parameter of the New-PSSessionOption cmdlet to make additional data available to the remote session.
The first command creates a hash table with two keys, Team and Use. The command saves the hash table in the $team variable. (For more information about hash tables, see about_Hash_Tables (http://go.microsoft.com/fwlink/?LinkID=135175).)
The second command uses the ApplicationArguments parameter of the New-PSSessionOption cmdlet to create a session option object that contains the data in the $team variable. The command saves the session option object in the $teamOption variable.
When New-PSSessionOption creates the session option object, it automatically converts the hash table in the value of the ApplicationArguments parameter to a primitive dictionary so the data can be reliably transmitted to the remote session.
The third command uses the New-PSSession cmdlet to start a session on the Server01 computer. It uses the SessionOption parameter to include the options in the $teamOption variable.
The fourth command demonstrates that the data in the $team variable is available to commands in the remote session. The data appears in the ApplicationArguments property of the $PSSenderInfo automatic variable.
The fifth command shows how the data might be used. The command uses the Invoke-Command cmdlet to run a script only when the value of the Use property is not "Testing". When the value of Use is "Testing", the command returns "Just testing."







### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289598)
[Enter-PSSession]()
[Invoke-Command]()
[New-PSSession]()

## New-PSTransportOption

### SYNOPSIS
Creates an object that contains advanced options for a session configuration.

### DESCRIPTION
The New-PSTransportOption cmdlet creates an object that contains transport options for session configurations. You can use the object as the value of the TransportOption parameter of cmdlets that create or change a session configuration, such as the Register-PSSessionConfiguration and Set-PSSessionConfiguration cmdlets.
You can also change the transport option settings by editing the values of the session configuration properties in the WSMan: drive. For more information, see WSMan Provider.
The session configuration options represent the session values set on the "server-side," or receiving end of a remote connection. The "client-side," or sending end of the connection, can set session option values when the session is created, or when the client disconnects from or reconnects to the session. Unless stated otherwise, when the setting values conflict, the client-side values take precedence. However, the client-side values cannot violate maximum values and quotas set in the session configuration.
Without parameters, New-PSTransportOption generates a transport option object with null values for all of the options. If you omit a parameter, the object has a null value for the property that the parameter represents. A null value has no effect on the session configuration.
For more information about session options, see New-PSSessionOption. For more information about session configurations, see about_Session_Configurations (http://go.microsoft.com/fwlink/?LinkID=145152). 
This cmdlet is introduced in Windows PowerShell 3.0.

### PARAMETERS

#### IdleTimeoutSec [Int32]

```powershell
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 1')]
```

Determines how long each session stays open if the remote computer does not receive any communication from the local computer, including the heartbeat signal. When the interval expires, the session closes.
The idle timeout value is of significant importance when the user intends to disconnect and reconnect to a session. The user can reconnect only if the session has not timed out.
The IdleTimeoutSec parameter corresponds to the IdleTimeoutMs property of a session configuration.
Enter a value in seconds. The default value is 7200 (2 hours). The minimum value is 60 (1 minute). The maximum is the value of the IdleTimeout property of Shell objects in the WSMan configuration (WSMan:\<ComputerName>\Shell\IdleTimeout). The default value is 7200000 milliseconds (2 hours).
If an idle timeout value is set in the session options and in the session configuration, value set in the session options takes precedence, but it cannot exceed the value of the MaxIdleTimeoutMs property of the session configuration. To set the value of the MaxIdleTimeoutMs property, use the MaxIdleTimeoutSec parameter.

#### MaxConcurrentCommandsPerSession [Int32]

```powershell
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 1')]
```

Limits the number of commands that can run concurrently in each session to the specified value. The default value is 1000.
The MaxConcurrentCommandsPerSession parameter corresponds to the MaxConcurrentCommandsPerShell property of a session configuration.

#### MaxConcurrentUsers [Int32]

```powershell
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 1')]
```

Limits the number of users who can run commands concurrently in each session to the specified value. The default value is 5.

#### MaxIdleTimeoutSec [Int32]

```powershell
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 1')]
```

Limits the idle timeout set for each session to the specified value. The default value is [Int]::MaxValue (~25 days).
The idle timeout value is of significant importance when the user intends to disconnect and reconnect to a session. The user can reconnect only if the session has not timed out.
The MaxIdleTimeoutSec parameter corresponds to the MaxIdleTimeoutMs property of a session configuration.

#### MaxMemoryPerSessionMB [Int32]

```powershell
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 1')]
```

Limits the memory used by each session to the specified value. Enter a value in megabytes. The default value is 1024 megabytes (1 GB).
The MaxMemoryPerSessionMB parameter corresponds to the MaxMemoryPerShellMB property of a session configuration.

#### MaxProcessesPerSession [Int32]

```powershell
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 1')]
```

Limits the number of processes running in each session to the specified value. The default value is 15.
The MaxProcessesPerSession parameter corresponds to the MaxProcessesPerShell property of a session configuration.

#### MaxSessions [Int32]

```powershell
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 1')]
```

Limits the number of sessions that use the session configuration. The default value is 25.
The MaxSessions parameter corresponds to the MaxShells property of a session configuration.

#### MaxSessionsPerUser [Int32]

```powershell
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 1')]
```

Limits the number of sessions that use the session configuration and run with the credentials of a given user to the specified value. The default value is 25.
When setting this value, consider that many users might be using the credentials of a "run as" user.
The MaxSessionsPerUser parameter corresponds to the MaxShellsPerUser property of a session configuration.

#### OutputBufferingMode [OutputBufferingMode]

```powershell
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 1')]
```

Determines how command output is managed in disconnected sessions when the output buffer becomes full.
The default value of the OutputBufferingMode property of sessions is Block.
Valid values are: 
-- Block: When the output buffer is full, execution is suspended until the buffer is clear. 
-- Drop: When the output buffer is full, execution continues. As new output is saved, the oldest output is discarded.
-- None: No output buffering mode is specified.

#### ProcessIdleTimeoutSec [Int32]

```powershell
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 1')]
```

Limits the timeout for each host process to the specified value. The default value, 0, means that there is no timeout value for the process.
Other session configurations have per-process timeout values. For example, the Microsoft.PowerShell.Workflow session configuration has a per-process timeout value of 28800 seconds (8 hours).


### INPUTS
#### None
This cmdlet does not take input from the pipeline.

### OUTPUTS
#### Microsoft.PowerShell.Commands.WSManConfigurationOption


### NOTES
The properties of a session configuration object vary with the options set for the session configuration and the values of those options. Also, session configurations that use a session configuration file have additional properties.

### EXAMPLES
#### Example 1

```powershell
PS C:\>New-PSTransportOption
ProcessIdleTimeoutSec           :
MaxIdleTimeoutSec               :
MaxSessions                     :
MaxConcurrentCommandsPerSession :
MaxSessionsPerUser              :
MaxMemoryPerSessionMB           :
MaxProcessesPerSession          :
MaxConcurrentUsers              :
IdleTimeoutSec                  :
OutputBufferingMode             :

```
This command runs the New-PSTransportOption with no parameters. The output shows that the cmdlet generates a transport option object with null values for all properties.


#### Example 2

```powershell
The first command uses the New-PSTransportOption cmdlet to create a transport options object, which it saves in the $t variable. The command uses the MaxSessions parameter to increase the maximum number of sessions to 40. 
PS C:\>$t = New-PSTransportOption -MaxSessions 40

The second command uses the Register-PSSessionConfiguration cmdlet create the ITTasks session configuration. The command uses the TransportOption parameter to specify the transport options object in the $t variable.
PS C:\>Register-PSSessionConfiguration -Name ITTaska -TransportOption $t

The third command uses the Get-PSSessionConfiguration cmdlet to get the ITTasks session configurations and the Format-List cmdlet to display all of the properties of the session configuration object in a list. The output shows that the value of the MaxShells property of the session configuration is 40.
PS C:\>Get-PSSessionConfiguration -Name ITTasks | Format-List -Property *
Architecture                  : 64
Filename                      : %windir%\system32\pwrshplugin.dll
ResourceUri                   : http://schemas.microsoft.com/powershell/ITTasks
MaxConcurrentCommandsPerShell : 1000
UseSharedProcess              : false
ProcessIdleTimeoutSec         : 0
xmlns                         : http://schemas.microsoft.com/wbem/wsman/1/config/PluginConfiguration
MaxConcurrentUsers            : 5
lang                          : en-US
SupportsOptions               : true
ExactMatch                    : true
RunAsUser                     :
IdleTimeoutms                 : 7200000
PSVersion                     : 3.0
OutputBufferingMode           : Block
AutoRestart                   : false
MaxShells                     : 40
MaxMemoryPerShellMB           : 1024
MaxIdleTimeoutms              : 43200000
SDKVersion                    : 2
Name                          : ITTasks
XmlRenderingType              : text
Capability                    : {Shell}
RunAsPassword                 :
MaxProcessesPerShell          : 15
Enabled                       : True
MaxShellsPerUser              : 25
Permission                    :

```
This example shows how to use a transport options object to set session configuration options


#### Example 3

```powershell
The first command uses the New-PSTransportOption cmdlet to create a transport option object. The command uses the IdleTimeoutSec parameter to set the IdleTimeoutSec property value of the object to one hour (3600 seconds). The command saves the transport objects object in the $t variable.
PS C:\>$t = New-PSTransportOption -IdleTimeoutSec 3600

The second command uses the Set-PSSessionConfiguration cmdlet to change the transport options of the ITTasks session configuration. The command uses the TransportOption parameter to specify the transport options object in the $t variable.
PS C:\>Set-PSSessionConfiguration -Name ITTasks -TransportOption $t

The third command uses the New-PSSession cmdlet to create the MyITTasks session on the local computer. The command uses the ConfigurationName property to specify the ITTasks session configuration. The command saves the session in the $s variable.Notice that the command does not use the SessionOption parameter of New-PSSession to set a custom idle timeout for the session. If it did, the idle timeout value set in the session option would take precedence over the idle timeout set in the session configuration.
PS C:\>$s = New-PSSession -Name MyITTasks -ConfigurationName ITTasks

The fourth command uses the Format-List cmdlet to display all properties of the session in the $s variable in a list. The output shows that the session has an idle timeout of one hour (360,000 milliseconds).
PS C:\>$s | Format-List -Property *
State                  : Opened
IdleTimeout            : 3600000
OutputBufferingMode    : Block
ComputerName           : localhost
ConfigurationName      : ITTasks
InstanceId             : 4110c3f5-68ea-40fa-9bbf-04a433dbb02d
Id                     : 1
Name                   : MyITTasks
Availability           : Available
ApplicationPrivateData : {PSVersionTable}
Runspace               : System.Management.Automation.RemoteRunspace

```
This command shows the effect of setting a transport option in a session configuration on the sessions that use the session configuration.



### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289599)
[about_Session_Configurations]()
[New-PSSession]()
[New-PSSessionOption]()
[Register-PSSessionConfiguration]()
[Set-PSSessionConfiguration]()

## Out-Default

### SYNOPSIS
Sends the output to the default formatter and to the default output cmdlet. 

### DESCRIPTION
The Out-Default cmdlet sends output to the default formatter and the default output cmdlet. This cmdlet has no effect on the formatting or output of Windows PowerShell commands. It is a placeholder that lets you write your own Out-Default function or cmdlet.

### PARAMETERS

#### InputObject [PSObject]

```powershell
[Parameter(
  ValueFromPipeline = $true,
  ParameterSetName = 'Set 1')]
```

Accepts input to the cmdlet.


### INPUTS
#### None


### OUTPUTS
#### 


### NOTES

### EXAMPLES
#### 1:

```powershell
```



### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289600)
[Format-Custom]()
[Format-List]()
[Format-Table]()
[Format-Wide]()
[about_Format.ps1.xml]()

## Out-Host

### SYNOPSIS
Sends output to the command line.

### DESCRIPTION
The Out-Host cmdlet sends output to the Windows PowerShell host for display. The host displays the output at the command line. Because Out-Host is the default, you do not need to specify it unless you want to use its parameters to change the display.

### PARAMETERS

#### InputObject [PSObject]

```powershell
[Parameter(
  ValueFromPipeline = $true,
  ParameterSetName = 'Set 1')]
```

Specifies the objects that are written to the console. Enter a variable that contains the objects, or type a command or expression that gets the objects.

#### Paging [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Displays one page of output at a time, and waits for user input before displaying the remaining pages, much like the traditional "more" command. By default, all of the output is displayed on a single page. The page size is determined by the characteristics of the host.


### INPUTS
#### System.Management.Automation.PSObject
You can pipe any object to Out-Host.

### OUTPUTS
#### None
Out-Host does not generate any output. However, the host might display the objects that Out-Host sends to it.

### NOTES
The cmdlets that contain the Out verb (the Out cmdlets) do not format objects; they just render them and send them to the specified display destination. If you send an unformatted object to an Out cmdlet, the cmdlet sends it to a formatting cmdlet before rendering it.
The Out cmdlets do not have parameters for names or file paths. To send data to an Out cmdlet, use a pipeline operator (|) to send the output of a Windows PowerShell command to the cmdlet. You can also store data in a variable and use the InputObject parameter to pass the data to the cmdlet. For help, see the examples.
Out-Host sends data, but it does not emit any output objects. If you pipe the output of Out-Host to the Get-Member cmdlet, Get-Member reports that no objects have been specified.


### EXAMPLES
#### -------------------------- EXAMPLE 1 --------------------------

```powershell
PS C:\>Get-Process | Out-Host -Paging

```
This command displays the processes on the system one page at a time. It uses the Get-Process cmdlet to get the processes on the system. The pipeline operator (|) sends the results to Out-Host, which displays them at the console. The Paging parameter displays one page of data at a time.
The same command format is used for the Help function that is built into Windows PowerShell. That function gets data from the Get-Help cmdlet and then uses the Paging parameter of Out-Host to display the data one page at a time by using this command format: Get-Help $Args[0] | Out-Host -Paging.


#### -------------------------- EXAMPLE 2 --------------------------

```powershell
PS C:\>$a = Get-History
PS C:\>Out-Host -InputObject $a

```
These commands display the session history at the command line. The first command uses the Get-History cmdlet to get the session history, and then it stores the history in the $a variable. The second command uses Out-Host to display the content of the $a variable, and it uses the InputObject parameter to specify the variable to Out-Host.



### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289601)
[Clear-Host]()
[Out-Default]()
[Out-File]()
[Out-Null]()
[Out-Printer]()
[Out-String]()
[Write-Host]()

## Out-LineOutput

### SYNOPSIS
Out-LineOutput

### DESCRIPTION
This cmdlet is for internal use only.

### PARAMETERS

#### InputObject [PSObject]

```powershell
[Parameter(
  ValueFromPipeline = $true,
  ParameterSetName = 'Set 1')]
```



#### LineOutput [Object]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 1')]
```




### INPUTS
#### None


### OUTPUTS
#### 


### NOTES

### EXAMPLES
#### 1:

```powershell
PS C:\>


```



#### 2:

```powershell

PS C:\>


```




### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=313917)

## Out-Null

### SYNOPSIS
Deletes output instead of sending it down the pipeline.

### DESCRIPTION
The Out-Null cmdlet sends output to NULL, in effect, deleting it.

### PARAMETERS

#### InputObject [PSObject]

```powershell
[Parameter(
  ValueFromPipeline = $true,
  ParameterSetName = 'Set 1')]
```

Specifies the object that was sent to null (deleted). Enter a variable that contains the objects, or type a command or expression that gets the objects.


### INPUTS
#### System.Management.Automation.PSObject
You can pipe any object to Out-Null.

### OUTPUTS
#### None
Out-Null does not generate any output.

### NOTES
The cmdlets that contain the Out verb (the Out cmdlets) do not have parameters for names or file paths. To send data to an Out cmdlet, use a pipeline operator (|) to send the output of a Windows PowerShell command to the cmdlet. You can also store data in a variable and use the InputObject parameter to pass the data to the cmdlet. For more information, see the examples.

Out-Null does not return any output objects. If you pipe the output of Out-Null to the Get-Member cmdlet, Get-Member reports that no objects have been specified.

### EXAMPLES
#### -------------------------- EXAMPLE 1 --------------------------

```powershell
PS C:\>Get-ChildItem | Out-Null

```
This command gets the items in the local directory, but then it deletes them instead of passing them through the pipeline or displaying them at the command line. This is useful for deleting output that you do not need.



### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289602)
[Out-Default]()
[Out-File]()
[Out-Host]()
[Out-Printer]()
[Out-String]()

## Receive-Job

### SYNOPSIS
Gets the results of the Windows PowerShell background jobs in the current session. 

### DESCRIPTION
The Receive-Job cmdlet gets the results of Windows PowerShell background jobs, such as those started by using the Start-Job cmdlet or the AsJob parameter of any cmdlet. You can get the results of all jobs or identify jobs by their name, ID, instance ID, computer name, location, or session, or by submitting a job object.
When you start a Windows PowerShell background job, the job starts, but the results do not appear immediately. Instead, the command returns an object that represents the background job. The job object contains useful information about the job, but it does not contain the results. This method allows you to continue working in the session while the job runs.  For more information about background jobs in Windows PowerShell, see about_Jobs.
The Receive-Job cmdlet gets the results that have been generated by the time that the Receive-Job command is submitted. If the results are not yet complete, you can run additional Receive-Job commands to get the remaining results.
By default, job results are deleted from the system when you receive them, but you can use the Keep parameter to save the results so that you can receive them again. To delete the job results, run the Receive-Job command again (without the Keep parameter), close the session, or use the Remove-Job cmdlet to delete the job from the session.
Beginning in Windows PowerShell 3.0, Receive-Job also gets the results of custom job types, such as workflow jobs and instances of scheduled jobs. To enable Receive-Job to get the results a custom job type, import the module that supports the custom job type into the session before running a Receive-Job command, either by using the Import-Module cmdlet or by using or getting a cmdlet in the module. For information about a particular custom job type, see the documentation of the custom job type feature.

### PARAMETERS

#### ComputerName [String[]]

```powershell
[Parameter(
  Position = 2,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 2')]
```

Gets the results of jobs that were run on the specified computers. Enter the computer names. Wildcards are supported. The default is all jobs in the current session.
This parameter selects from among the job results that are stored on the local computer. It does not get data from remote computers. To get job results that are stored on remote computers, use the Invoke-Command cmdlet to run a Receive-Job command remotely.

#### Id [Int32[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 3')]
```

Gets the results of jobs with the specified IDs. The default is all jobs in the current session.
The ID is an integer that uniquely identifies the job within the current session. It is easier to remember and type than the instance ID, but it is unique only within the current session. You can type one or more IDs (separated by commas). To find the ID of a job, type "Get-Job" without parameters.

#### InstanceId [Guid[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 4')]
```

Gets the results of jobs with the specified instance IDs. The default is all jobs in the current session.
An instance ID is a GUID that uniquely identifies the job on the computer. To find the instance ID of a job, use the Get-Job cmdlet.

#### Job [Job[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 1')]
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 2')]
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 5')]
```

Specifies the job for which results are being retrieved. This parameter is required in a Receive-Job command. Enter a variable that contains the job or a command that gets the job. You can also pipe a job object to Receive-Job.

#### Keep [switch]

Saves the job results in the system, even after you have received them. By default, the job results are deleted when they are retrieved.
To delete the results, use Receive-Job to receive them again without the Keep parameter, close the session, or use the Remove-Job cmdlet to delete the job from the session.

#### Location [String[]]

```powershell
[Parameter(
  Position = 2,
  ParameterSetName = 'Set 1')]
```

Gets only the results of jobs with the specified location. The default is all jobs in the current session.

#### Name [String[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 6')]
```

Gets the results of jobs with the specified friendly name. Wildcards are supported. The default is all jobs in the current session.

#### NoRecurse [switch]

Gets results only from the specified job. By default, Receive-Job also gets the results of all child jobs of the specified job.

#### Session [PSSession[]]

```powershell
[Parameter(
  Position = 2,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 5')]
```

Gets the results of jobs that were run in the specified Windows Powershell session (PSSession). Enter a variable that contains the PSSession or a command that gets the PSSession, such as a Get-PSSession command. The default is all jobs in the current session.

#### Wait [switch]

Suppresses the command prompt until all job results are received. By default, Receive-Job immediately returns the available results.
By default, the Wait parameter waits until the job is in one of the following states:  Completed, Failed, Stopped, Suspended, or Disconnected. To direct the Wait parameter to continue waiting if the job state is Suspended or Disconnected, use the Force parameter with the Wait parameter.
This parameter is introduced in Windows PowerShell 3.0.

#### AutoRemoveJob [switch]

Deletes the job after returning the job results. If the job has more results, the job is still deleted, but Receive-Job displays a message.
This parameter works only on custom job types. It is designed for instances of job types that save the job or the type outside of the session, such as instances of scheduled jobs.
This parameter is introduced in Windows PowerShell 3.0.

#### WriteEvents [switch]

Reports changes in the job state while waiting for the job to complete.
This parameter is valid only when the Wait parameter is used in the command and the Keep parameter is omitted.
This parameter is introduced in Windows PowerShell 3.0.

#### WriteJobInResults [switch]

Returns the job object followed by the results.
This parameter is valid only when the Wait parameter is used in the command and the Keep parameter is omitted.
This parameter is introduced in Windows PowerShell 3.0.

#### Force [switch]

Continues waiting if jobs are in the Suspended or Disconnected state. By default, the Wait parameter of Receive-Job returns (terminates the wait) when jobs are in one of the following states: Completed, Failed, Stopped, Suspended, or Disconnected.
The Force parameter is valid only when the Wait parameter is also used in the command.
This parameter is introduced in Windows PowerShell 3.0.


### INPUTS
#### System.Management.Automation.Job
You can pipe job objects to Receive-Job.

### OUTPUTS
#### PSObject
Receive-Job returns the results of the commands in the job.

### NOTES

### EXAMPLES
#### -------------------------- EXAMPLE 1 --------------------------

```powershell
PS C:\>$job = Start-Job -ScriptBlock {Get-Process}
PS C:\>Receive-Job -Job $job

```
These commands use the Job parameter of Receive-Job to get the results of a particular job. 
The first command uses the Start-Job cmdlet to start a job that runs a Get-Process command. The command uses the assignment operator (=) to save the resulting job object in the $job variable.
The second command uses the Receive-Job cmdlet to get the results of the job. It uses the Job parameter to specify the job.


#### -------------------------- EXAMPLE 2 --------------------------

```powershell
PS C:\>$job = Start-Job -ScriptBlock {Get-Process}
PS C:\>$job | Receive-Job

```
This example is the same as Example 2, except that the command uses a pipeline operator (|) to send the job object to Receive-Job. As a result, the command does not need a Job parameter to specify the job.


#### -------------------------- EXAMPLE 3 --------------------------

```powershell
The first command uses the Invoke-Command cmdlet to start a background job that runs a Get-Service command on three remote computers. The command uses the AsJob parameter to run the command as a background job. The command saves the resulting job object in the $j variable.When you use the AsJob parameter of Invoke-Command to start a job, the job object is created on the local computer, even though the job runs on the remote computers. As a result, you use local commands to manage the job.Also, when you use AsJob, Windows PowerShell returns one job object that contains a child job for each job that was started. In this case, the job object contains three child jobs, one for each job on each remote computer.
PS C:\>


PS C:\>$j = Invoke-Command -ComputerName Server01, Server02, Server03 -ScriptBlock {Get-Service} -AsJob

The second command uses the dot method to display the value of the ChildJobs property of the job object in $j. The display shows that the command created three child jobs, one for the job on each remote computer.
PS C:\>$j.ChildJobs

Id   Name     State      HasMoreData   Location       Command
--   ----     -----      -----------   --------       -------
2    Job2     Completed  True          Server01       Get-Service
3    Job3     Completed  True          Server02       Get-Service
4    Job4     Completed  True          Server03       Get-Service

The third command uses the Receive-Job cmdlet to get the results of the Job3 child job that ran on the Server02 computer. It uses the Name parameter to specify the name of the child job and the Keep parameter to save the job results even after they are received.
PS C:\>Receive-Job -Name Job3 -Keep

Status  Name        DisplayName                        PSComputerName
------  ----------- -----------                        --------------
Running AeLookupSvc Application Experience             Server02
Stopped ALG         Application Layer Gateway Service  Server02
Running Appinfo     Application Information            Server02
Running AppMgmt     Application Management             Server02

```
These commands get the results of one of several background jobs run on remote computers.


#### -------------------------- EXAMPLE 4 --------------------------

```powershell
The first command uses the New-PSSession cmdlet to create three user-managed sessions ("PSSessions), one on each of the servers specified in the command. It saves the sessions in the $s variable.
PS C:\>$s = new-pssession -computername Server01, Server02, Server03

The second command uses the Invoke-Command cmdlet to run a Start-Job command in each of the PSSessions in the $s variable. The job runs a Get-Eventlog command that gets the events in the System log. The command saves the results in the $j variable.Because the command used Invoke-Command to run the Start-Job command, the command actually started three independent jobs on each of the three computers. As a result, the command returned three job objects representing three jobs run locally on three different computers.
PS C:\>$j = invoke-command -session $s -scriptblock {start-job -scriptblock {get-eventlog -logname system}}

The third command displays the three job objects in $j.
PS C:\>$j

Id   Name     State      HasMoreData   Location   Command
--   ----     -----      -----------   --------   -------
1    Job1     Completed  True          Localhost  get-eventlog system
2    Job2     Completed  True          Localhost  get-eventlog system
3    Job3     Completed  True          Localhost  get-eventlog system


The fourth command uses Invoke-Command to run a Receive-Job command in each of the sessions in $s and save the results in the $Results variable.Because $j is a local variable, the script block uses the Using scope modifier to identify the $j variable. For more information about the Using scope modifier, see about_Remote_Variables (http://go.microsoft.com/fwlink/?LinkID=252653).
PS C:\>$results = Invoke-Command -Session $s -ScriptBlock {Receive-Job -Job $Using:j}

```
This example shows how to get the results of background jobs run on three remote computers.



### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289603)
[Get-Job]()
[Invoke-Command]()
[Remove-Job]()
[Resume-Job]()
[Start-Job]()
[Stop-Job]()
[Suspend-Job]()
[Wait-Job]()
[about_Jobs]()
[about_Job_Details]()
[about_Remote_Jobs]()
[about_Remote_Variables]()
[about_Scopes]()

## Receive-PSSession

### SYNOPSIS
Gets results of commands in disconnected sessions

### DESCRIPTION
The Receive-PSSession cmdlet gets the results of commands running in Windows PowerShell sessions ("PSSession") that were disconnected. If the session is currently connected, Receive-PSSession gets the results of commands that were running when the session was disconnected. If the session is still disconnected, Receive-PSSession connects to the session, resumes any commands that were suspended, and gets the results of commands running in the session.
You can use a Receive-PSSession in addition to or in place of a Connect-PSSession command. Receive-PSSession can connect to any disconnected or reconnected session, including those that were started in other sessions or on other computers. 
Receive-PSSession works on PSSessions that were disconnected intentionally, such as by using the Disconnect-PSSession cmdlet or the InDisconnectedSession parameter of the Invoke-Command cmdlet, or unintentionally, such as by a network interruption. 
If you use the Receive-PSSession cmdlet to connect to a session in which no commands are running or suspended, Receive-PSSession connects to the session, but returns no output or errors.
For more information about the Disconnected Sessions feature, see about_Remote_Disconnected_Sessions.
This cmdlet is introduced in Windows PowerShell 3.0.

### PARAMETERS

#### Authentication [AuthenticationMechanism]

```powershell
[Parameter(ParameterSetName = 'Set 2')]
[Parameter(ParameterSetName = 'Set 3')]
[Parameter(ParameterSetName = 'Set 4')]
[Parameter(ParameterSetName = 'Set 5')]
```

Specifies the mechanism that is used to authenticate the user's credentials in the command to reconnect to the disconnected session.   Valid values are Default, Basic, Credssp, Digest, Kerberos, Negotiate, and NegotiateWithImplicitCredential.  The default value is Default.
For information about the values of this parameter, see "AuthenticationMechanism enumeration" in MSDN.
CAUTION: Credential Security Support Provider (CredSSP) authentication, in which the user's credentials are passed to a remote computer to be authenticated, is designed for commands that require authentication on more than one resource, such as accessing a remote network share. This mechanism increases the security risk of the remote operation. If the remote computer is compromised, the credentials that are passed to it can be used to control the network session.

#### CertificateThumbprint [String]

```powershell
[Parameter(ParameterSetName = 'Set 2')]
[Parameter(ParameterSetName = 'Set 3')]
[Parameter(ParameterSetName = 'Set 4')]
[Parameter(ParameterSetName = 'Set 5')]
```

Specifies the digital public key certificate (X509) of a user account that has permission to connect to the disconnected session. Enter the certificate thumbprint of the certificate.
Certificates are used in client certificate-based authentication. They can be mapped only to local user accounts; they do not work with domain accounts.
To get a certificate thumbprint, use a Get-Item or Get-ChildItem command in the Windows PowerShell Cert: drive.

#### ComputerName [String]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 2')]
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 3')]
```

Specifies the computer on which the disconnected session is stored. Sessions are stored on the computer that is at the "server-side" or receiving end of a connection. The default is the local computer.
Type the NetBIOS name, an IP address, or a fully qualified domain name of one computer. Wildcards are not permitted. To specify the local computer, type the computer name, "localhost", or a dot (.)

#### Credential [PSCredential]

```powershell
[Parameter(ParameterSetName = 'Set 2')]
[Parameter(ParameterSetName = 'Set 3')]
[Parameter(ParameterSetName = 'Set 4')]
[Parameter(ParameterSetName = 'Set 5')]
```

Specifies a user account that has permission to connect to the disconnected session. The default is the current user.
Type a user name, such as "User01" or "Domain01\User01". Or, enter a PSCredential object, such as one generated by the Get-Credential cmdlet. If you type a user name, you will be prompted for a password.

#### Id [Int32]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 6')]
```

Specifies the ID of the disconnected session. The ID parameter works only when the disconnected session was previously connected to the current session.
This parameter is valid, but not effective, when the session is stored on the local computer, but was not connected to the current session.

#### InstanceId [Guid]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 3')]
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 5')]
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 7')]
```

Specifies the instance ID of the disconnected session.
The instance ID is a GUID that uniquely identifies a PSSession on a local or remote computer.
The instance ID is stored in the InstanceID property of the PSSession.

#### JobName [String]

Specifies a friendly name for the job that Receive-PSSession returns.
Receive-PSSession returns a job when the value of the OutTarget parameter is Job or the job that is running in the disconnected session was started in the current session.
If the job that is running in the disconnected session was started in the current session, Windows PowerShell reuses the original job object in the session and ignores the value of the JobName parameter.
If the job that is running in the disconnected session was started in a different session, Windows PowerShell creates a new job object. It uses a default name, but you can use this parameter to change the name.
If the default value or explicit value of  the OutTarget parameter is not Job, the command succeeds, but the JobName parameter has no effect.

#### Name [String]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 2')]
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 4')]
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 8')]
```

Specifies the friendly name of the disconnected session.

#### OutTarget [OutTarget]

Determines how the session results are returned.
Valid values are:
-- Job: Returns the results asynchronously in a job object. You can use the JobName parameter to specify a name or new name for the job.
-- Host: Returns the results to the command line (synchronously). If the command is being resumed or the results consist of a large number of objects, the response might be delayed.
The default value of the OutTarget parameter is Host. However, if the command that is being received in disconnected session was started in the current session, the default value of the OutTarget parameter is the form in which the command was started. If the command was started as a job, it is returned as a job by default. Otherwise, it is returned to the host program by default.
Typically, the host program displays returned objects at the command line without delay, but this behavior can vary.

#### Port [Int32]

```powershell
[Parameter(ParameterSetName = 'Set 2')]
[Parameter(ParameterSetName = 'Set 3')]
```

Specifies the network port on the remote computer that is used to reconnect to the session.  To connect to a remote computer, the remote computer must be listening on the port that the connection uses.  The default ports are 5985 (the WinRM port for HTTP) and 5986 (the WinRM port for HTTPS).
Before using an alternate port, you must configure the WinRM listener on the remote computer to listen at that port. To configure the listener, type the following two commands at the Windows PowerShell prompt:
Remove-Item -Path WSMan:\Localhost\listener\listener* -Recurse
New-Item -Path WSMan:\Localhost\listener -Transport http -Address * -Port <port-number>
Do not use the Port parameter unless you must. The port that is set in the command applies to all computers or sessions on which the command runs. An alternate port setting might prevent the command from running on all computers.

#### Session [PSSession]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 1')]
```

Specifies the disconnected session. Enter a variable that contains the PSSession or a command that creates or gets the PSSession, such as a Get-PSSession command.

#### UseSSL [switch]

```powershell
[Parameter(ParameterSetName = 'Set 2')]
[Parameter(ParameterSetName = 'Set 3')]
```

Uses the Secure Sockets Layer (SSL) protocol to connect to the disconnected session. By default, SSL is not used.
WS-Management encrypts all Windows PowerShell content transmitted over the network. UseSSL is an additional protection that sends the data across an HTTPS connection instead of an HTTP connection.
If you use this parameter, but SSL is not available on the port used for the command, the command fails.

#### AllowRedirection [switch]

```powershell
[Parameter(ParameterSetName = 'Set 4')]
[Parameter(ParameterSetName = 'Set 5')]
```

Allows redirection of this connection to an alternate Uniform Resource Identifier (URI).
When you use the ConnectionURI parameter, the remote destination can return an instruction to redirect to a different URI. By default, Windows PowerShell does not redirect connections, but you can use this parameter to allow it to redirect the connection.
You can also limit the number of times the connection is redirected by changing the MaximumConnectionRedirectionCount session option value. Use the  MaximumRedirection parameter of the New-PSSessionOption cmdlet or set the MaximumConnectionRedirectionCount property of the $PSSessionOption preference variable. The default value is 5.

#### ApplicationName [String]

```powershell
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 2')]
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 3')]
```

Connects only to sessions that use the specified application.
Enter the application name segment of the connection URI. For example, in the following connection URI, the application name is WSMan: http://localhost:5985/WSMAN. The application name of a session is stored in the Runspace.ConnectionInfo.AppName property of the session.
The value of this parameter is used to select and filter sessions. It does not change the application that the session uses.

#### ConfigurationName [String]

```powershell
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 2')]
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 3')]
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 4')]
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 5')]
```

Connects only to sessions that use the specified session configuration.
Enter a configuration name or the fully qualified resource URI for a session configuration. If you specify only the configuration name, the following schema URI is prepended:  http://schemas.microsoft.com/powershell. The configuration name of a session is stored in the ConfigurationName property of the session.
The value of this parameter is used to select and filter sessions. It does not change the session configuration that the session uses.
For more information about session configurations, see about_Session_Configurations .

#### ConnectionUri [Uri]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 4')]
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 5')]
```

Specifies a Uniform Resource Identifier (URI) that defines the connection endpoint that is used to reconnect to the disconnected session. 
The URI must be fully qualified.  The format of this string is as follows:

<Transport>://<ComputerName>:<Port>/<ApplicationName>

The default value is as follows:

http://localhost:5985/WSMAN
If you do not specify a connection URI, you can use the UseSSL, ComputerName, Port, and ApplicationName parameters to specify the connection URI values.
Valid values for the Transport segment of the URI are HTTP and HTTPS. If you specify a connection URI with a Transport segment, but do not specify a port, the session is created with standards ports: 80 for HTTP and 443 for HTTPS. To use the default ports for Windows PowerShell remoting, specify port 5985 for HTTP or 5986 for HTTPS.
If the destination computer redirects the connection to a different URI, Windows PowerShell prevents the redirection unless you use the AllowRedirection parameter in the command.

#### SessionOption [PSSessionOption]

```powershell
[Parameter(ParameterSetName = 'Set 2')]
[Parameter(ParameterSetName = 'Set 3')]
[Parameter(ParameterSetName = 'Set 4')]
[Parameter(ParameterSetName = 'Set 5')]
```

Sets advanced options for the session.  Enter a SessionOption object, such as one that you create by using the New-PSSessionOption cmdlet, or a hash table in which the keys are session option names and the values are session option values.
The default values for the options are determined by the value of the $PSSessionOption preference variable, if it is set. Otherwise, the default values are established by options set in the session configuration.
The session option values take precedence over default values for sessions set in the $PSSessionOption preference variable and in the session configuration. However, they do not take precedence over maximum values, quotas or limits set in the session configuration. 
For a description of the session options, including the default values, see New-PSSessionOption. For information about the $PSSessionOption preference variable, see about_Preference_Variables (http://go.microsoft.com/fwlink/?LinkID=113248). For more information about session configurations, see about_Session_Configurations (http://go.microsoft.com/fwlink/?LinkID=145152).

#### Confirm [switch]

Prompts you for confirmation before running the cmdlet.Prompts you for confirmation before running the cmdlet.

#### WhatIf [switch]

Shows what would happen if the cmdlet runs. The cmdlet is not run.Shows what would happen if the cmdlet runs. The cmdlet is not run.


### INPUTS
#### System.Management.Automation.Runspaces.PSSession System.Int32 System.Guid System.String
You can pipe session objects, such as those returned by the Get-PSSession cmdlet to Receive-PSSession.
You can pipe session IDs to Receive-PSSession.
You can pipe the instance IDs of sessions to Receive-PSSession.
You can pipe session names to Receive-PSSession.

### OUTPUTS
#### System.Management.Automation.Job or PSObject
Receive-PSSession gets the results of commands that ran in the disconnected session, if any. If the value or default value of the OutTarget parameter is Job, Receive-PSSession returns a job object. Otherwise, it returns objects that represent that command results.

### NOTES
Receive-PSSession gets results only from sessions that were disconnected. Only sessions that are connected to (terminate at) computers running Windows PowerShell 3.0 or later can be disconnected and reconnected.
If the commands that were running in the disconnected session did not generate results or if the results were already returned to another session, Receive-PSSession does not generate any output.
The output buffering mode of a session determines how commands in the session manage output when the session is disconnected. When the value of the OutputBufferingMode option of the session is Drop and the output buffer is full, the command begins to delete output. Receive-PSSession cannot recover this output. For more information about the output buffering mode option, see the help topics for the New-PSSessionOption and New-PSTransportOption cmdlets.
You cannot change the idle timeout value of a PSSession when you connect to the PSSession or receive results. The SessionOption parameter of Receive-PSSession takes a SessionOption object that has an IdleTimeout value. However, the IdleTimeout value of the SessionOption object and the IdleTimeout value of the $PSSessionOption variable are ignored when connecting to a PSSession or receiving results.
You can set and change the idle timeout of a PSSession when you create the PSSession (by using the New-PSSession or Invoke-Command cmdlets) and when you disconnect from the PSSession.
The IdleTimeout property of  a PSSession is critical to disconnected sessions, because it determines how long a disconnected session is maintained on the remote computer. Disconnected sessions are considered to be idle from the moment that they are disconnected, even if commands are running in the disconnected session.
If you start a start a job in a remote session by using the AsJob parameter of the Invoke-Command cmdlet, the job object is created in the current session, even though the job runs in the remote session. If you disconnect the remote session, the job object in the current session is now disconnected from the job. The job object still contains any results that were returned to it, but it does not receive new results from the job in the disconnected session.
If a different client connects to the session that contains the running job, the results that were delivered to the original job object in the original session are not available in the newly connected session. Only results that were not delivered to the original job object are available in the reconnected session.
Similarly, if you start a script in a session and then disconnect from the session, any results that the script delivers to the session before disconnecting are not available to another client that connects to the session.
To prevent data loss in sessions that you intend to disconnect, use the InDisconnectedSession parameter of the  Invoke-Command cmdlet. Because this parameter prevents results from being returned to the current session, all results are available when the session is reconnected.
You can also prevent data loss by using the Invoke-Command cmdlet to run a Start-Job command in the remote session. In this case, the job object is created in the remote session. You cannot use the Receive-PSSession cmdlet to get the job results. Instead, use the Connect-PSSession cmdlet to connect to the session and then use the Invoke-Command cmdlet to run a Receive-Job command in the session.
When a session that contains a running job is disconnected and then reconnected, the original job object is reused only if the job is disconnected and reconnected to the same session, and the command to reconnect does not specify a new job name. If the session is reconnected to a different client session or a new job name is specified, Windows PowerShell creates a new job object for the new session.
When you disconnect a PSSession, the session state is Disconnected and the availability is None. 
The value of the State property is relative to the current session. Therefore, a value of Disconnected means that the PSSession is not connected to the current session. However, it does not mean that the PSSession is disconnected from all sessions. It might be connected to a different session. To determine whether you can connect or reconnect to the session, use the Availability property.
An Availability value of None indicates that you can connect to the session. A value of Busy indicates that you cannot connect to the PSSession because it is connected to another session.
For more information about the values of the State property of sessions, see "RunspaceState Enumeration" in MSDN at [http://msdn.microsoft.com/en-us/library/windows/desktop/system.management.automation.runspaces.runspacestate(v=VS.85).aspx]().
For more information about the values of the Availability property of sessions, see RunspaceAvailability Enumeration at [http://msdn.microsoft.com/en-us/library/windows/desktop/system.management.automation.runspaces.runspaceavailability(v=vs.85).aspx]().

### EXAMPLES
#### Example 1

```powershell
PS C:\>Receive-PSSession -ComputerName Server01 -Name ITTask

```
This command uses the Receive-PSSession cmdlet to connect to the ITTask session on the Server01 computer and get the results of commands that were running in the session.
Because the command does not use the OutTarget parameter, the results appear at the command line.


#### Example 2

```powershell
PS C:\>Get-PSSession -ComputerName  Server01, Server02 | Receive-PSSession

```
This command gets the results of all commands running in all disconnected sessions on the Server01 and Server02 computers.
If any session was not disconnected or is not running commands, Receive-PSSession does not connect to the session and does not return any output or errors.


#### Example 3

```powershell
PS C:\>Receive-PSSession -ComputerName Server01 -Name ITTask -OutTarget Job -JobName ITTaskJob01 -Credential Domain01\Admin01
Id     Name            State         HasMoreData     Location
--     ----            -----         -----------     --------
16     ITTaskJob01     Running       True            Server01

```
This command uses the Receive-PSSession cmdlet to get the results of a script that was running in the ITTask session on the Server01 computer.
The command uses the ComputerName and Name parameters to identify the disconnected session. It uses the OutTarget parameter with a value of Job to direct Receive-PSSession to return the results as a job and the JobName parameter to specify a name for the job in the reconnected session.
The command uses the Credential parameter to run the Receive-PSSession command with the permissions of a domain administrator.
The output shows that Receive-PSSession returned the results as a job in the current session. To get the job results, use a Receive-Job command


#### Example 4

```powershell
The first command uses the New-PSSession cmdlet to create a session on the Server01 computer. The command saves the session in the $s variable.The second command gets the session in the $s variable. Notice that the State is Opened and the Availability is Available. These values indicate that you are connected to the session and can run commands in the session.
PS C:\>$s = New-PSSession -ComputerName Server01 -Name AD -ConfigurationName ADEndpoint
PS C:\>$s 

Id Name    ComputerName    State         ConfigurationName     Availability
 -- ----    ------------    -----         -----------------     ------------
  8 AD      Server01        Opened        ADEndpoint            Available

The third command uses the Invoke-Command cmdlet to run a script in the session in the $s variable.The script begins to run and return data, but a network outage occurs that interrupts the session. The user has to exit the session and restart the local computer.
PS C:\>Invoke-Command -Session $s -FilePath \\Server12\Scripts\SharedScripts\New-ADResolve.ps1
 Running "New-ADResolve.ps1" ….exit

# Network outage
# Restart local computer
# Network access is not re-established within 4 minutes

When the computer restarts, the user starts Windows PowerShell and runs a Get-PSSession command to get sessions on the Server01 computer. The output shows that the AD session still exists on the Server01 computer. The State indicates that it is disconnected and the Availability value, None, indicates that it is not connected to any client sessions.
PS C:\>Get-PSSession -ComputerName Server01

 Id Name    ComputerName    State         ConfigurationName     Availability
 -- ----    ------------    -----         -----------------     ------------
  1 Backup  Server01        Disconnected  Microsoft.PowerShell          None
  8 AD      Server01        Disconnected  ADEndpoint                   None


The fifth command uses the Receive-PSSession cmdlet to reconnect to the AD session and get the results of the script that ran in the session. The command uses the OutTarget parameter to request the results in a job named "ADJob".The command returns a job object. The output indicates that the script is still running.
PS C:\>Receive-PSSession -ComputerName Server01 -Name AD -OutTarget Job -JobName AD
Job Id     Name      State         HasMoreData     Location
--     ----      -----         -----------     --------
16     ADJob     Running       True            Server01

The sixth command uses the Get-PSSession cmdlet to check the job state. The output confirms that, in addition to resuming script execution and getting the script results, the Receive-PSSession cmdlet reconnected to the AD session, which is now open and available for commands.
PS C:\>Get-PSSession -ComputerName Server01
Id Name    ComputerName    State         ConfigurationName     Availability
-- ----    ------------    -----         -----------------     ------------ 
 1 Backup  Server01        Disconnected  Microsoft.PowerShell          Busy
 8 AD      Server01        Opened        ADEndpoint                Available


```
This example uses the Receive-PSSession cmdlet to get the results of a job after a network outage disrupts a session connection. Windows PowerShell automatically attempts to reconnect the session once each second for the next four minutes and abandons the effort only if all attempts in the four-minute interval fail.


#### Example 5

```powershell
The first command uses the Invoke-Command cmdlet to run a script on the three remote computers. Because the scripts gathers and summarize data from multiple databases, it often takes the script an extended time to complete. The command uses the InDisconnectedSession parameter, which starts the scripts and then immediately disconnects the sessions.The command uses the SessionOption parameter to extend the IdleTimeout value of the disconnected session. Disconnected sessions are considered to be idle from the moment they are disconnected, so it's important to set the idle timeout for a long enough period that the commands can complete and you can reconnect to the session, if necessary. You can set the IdleTimeout only when you create the PSSession and change it only when you disconnect from it. You cannot change the IdleTimeout value when connecting to a PSSession or receiving its results.After running the command, the user exits Windows PowerShell and closes the computer .
PS C:\>Invoke-Command -InDisconnectedSession -ComputerName Server01, Server02, Server30 -FilePath \\Server12\Scripts\SharedScripts\Get-BugStatus.ps1 -Name BugStatus -SessionOption @{IdleTimeout = 86400000} -ConfigurationName ITTasks# Exit

# Start Windows PowerShell on a different computer.

On the next day, the user resumes Windows and starts Windows PowerShell. The second command uses the Get-PSSession cmdlet to get the sessions in which the scripts were running. The command identifies the sessions by the computer name, session name, and the name of the session configuration and saves the sessions in the $s variable.The third command displays the value of the $s variable. The output shows that the sessions are disconnected, but not busy, as expected.
PS C:\>$s = Get-PSSession -ComputerName Server01, Server02, Server30 -Name BugStatus
 PS C:\>$s
Id Name    ComputerName    State         ConfigurationName     Availability
 -- ----    ------------    -----         -----------------     ------------
  1 ITTask  Server01        Disconnected  ITTasks                       None
  8 ITTask  Server02        Disconnected  ITTasks                       None
  2 ITTask  Server30        Disconnected  ITTasks                       None


The fourth command uses the Receive-PSSession cmdlet to connect to the sessions in the $s variable and get their results. The command saves the results in the $Results variable..Another display of the $s variable shows that the sessions are connected and available for commands.
PS C:\>$Results = Receive-PSSession -Session $s
PS C:\>$s
 Id Name    ComputerName    State         ConfigurationName     Availability
-- ----    ------------    -----         -----------------     ------------ 
 1 ITTask  Server01        Opened        ITTasks                  Available
 8 ITTask  Server02        Opened        ITTasks                  Available
 2 ITTask  Server30        Opened        ITTasks                  Available


The fifth command displays the script results in the $Results variable. If any of the results are unexpected, the user can run commands in the sessions to investigate.
PS C:\>$Results
Bug Report - Domain 01
----------------------
ComputerName          BugCount          LastUpdated
--------------        ---------         ------------
Server01              121               Friday, December 30, 2011 5:03:34 PM…

```
This example uses the Receive-PSSession cmdlet to reconnect to sessions that were intentionally disconnected and get the results of jobs that were running in the sessions.


#### Example 5

```powershell
The first command uses the New-PSSession cmdlet to create the Test session on the Server01 computer. The command saves the session in the $s variable.
PS C:\>$s = New-PSSession -ComputerName Server01 -Name Test

The second command uses the Invoke-Command cmdlet to run a command in the session in the $s variable. The command uses the AsJob parameter to run the command as a job and to create the job object in the current session. The command returns a job object, which is saved in the $j variable.The third command displays the job object in the $j variable.
PS C:\>$j = Invoke-Command -Session $s { 1..1500 | Foreach-Object {"Return $_"; sleep 30}} -AsJob

PS C:\>$j
Id     Name           State         HasMoreData     Location
--     ----           -----         -----------     --------
16     Job1           Running       True            Server01

The fourth command disconnects the session in the $s variable.
PS C:\>$s | Disconnect-PSSession
Id Name   ComputerName    State         ConfigurationName     Availability
-- ----   ------------    -----         -----------------     ------------ 
1  Test   Server01        Disconnected  Microsoft.PowerShell  None

The fifth command shows the effect of disconnecting on the job object in the $j variable. The job state is now Disconnected.
PS C:\>$j
Id     Name           State         HasMoreData     Location
--     ----           -----         -----------     --------
16     Job1           Disconnected  True            Server01

The sixth command runs a Receive-Job command on the job in the $j variable. The output shows that the job began to return output before the session (and the job) were disconnected.
PS C:\>Receive-Job $j -Keep
Return 1Return 2

The seventh command is run in the same client session. The command uses the Connect-PSSession cmdlet to reconnect to the Test session on the Server01 computer and saves the session in the $s2 variable.
PS C:\>$s2 = Connect-PSSession -ComputerName Server01 -Name Test

The eighth command uses the Receive-PSSession cmdlet to get the results of the job that was running in the session. Because the command is run in the same session, Receive-PSSession returns the results as a job by default and reuses the same job object. The command saves the job in the $j2 variable.The ninth command uses the Receive-Job cmdlet to get the results of the job in the $j variable.
PS C:\>$j2 = Receive-PSSession -ComputerName Server01 -Name Test

PS C:\>Receive-Job $j
Return 3
Return 4…

```
This example shows what happens to a job that is running in a disconnected session.



### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289604)
[Connect-PSSession]()
[Enter-PSSession]()
[Exit-PSSession]()
[Get-PSSession]()
[Invoke-Command]()
[New-PSSession]()
[New-PSSessionOption]()
[Receive-PSSession]()
[Remove-PSSession]()
[about_PSSessions]()
[about_Remote]()
[about_Remote_Disconnected_Sessions]()
[about_Session_Configurations]()

## Register-PSSessionConfiguration

### SYNOPSIS
Creates and registers a new session configuration.

### DESCRIPTION
The Register-PSSessionConfiguration cmdlet creates and registers a  new session configuration on the local computer. This is an advanced cmdlet that is designed to be used by system administrators to create custom sessions for remote users.
Every Windows PowerShell session (PSSession) uses a session configuration, also known as an "endpoint." When users create a session that connects to the computer, they can select a session configuration or use the default session configuration that is registered when you enable Windows PowerShell remoting. Users can also set the $PSSessionConfigurationName preference variable, which specifies a default configuration for remote sessions created in the current session.
The session configuration defines the environment for the remote session. The configuration can determine which commands and language elements are available in the session, and it can include settings that protect the computer, such as those that limit the amount of data that the session can receive remotely in a single object or command.  The security descriptor (ACL) of the session configuration determines which users have permission to use the session configuration.
You can define the elements of configuration by using an assembly that implements a new configuration class and by using a script that runs in the session. Beginning in Windows PowerShell 3.0, you can also use a session configuration file to define the session configuration.
For information about session configurations, see about_Session_Configurations (http://go.microsoft.com/fwlink/?LinkID=145152). For information about session configuration files, see about_Session_Configuration_Files (http://go.microsoft.com/fwlink/?LinkID=236023).

### PARAMETERS

#### AccessMode [PSSessionConfigurationAccessMode]

Enables and disables the session configuration and determines whether it can be used for remote or local sessions on the computer. Remote is the default.
Valid values are:

--  Disabled: Disables the session configuration. It cannot be used for remote or local access to the computer.

--  Local: Allows users of the local computer to use the session configuration to create a local "loopback" session on the same computer, but denies access to remote users.

 
                        
--  Remote: Allows local and remote users to use the session configuration to create sessions and run commands on this computer.
The value of this parameter can be overridden at a later time by the actions of other cmdlets. For example, the Enable-PSRemoting cmdlet allows remote access to all session configurations, the Enable-PSSessionConfiguration cmdlet enables session configurations, and the Disable-PSRemoting cmdlet prevents remote access to all session configurations.
This parameter is introduced in Windows PowerShell 3.0.

#### ApplicationBase [String]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
[Parameter(ParameterSetName = 'Set 2')]
```

Specifies the path to the assembly file (*.dll) that is specified in the value of the AssemblyName parameter. Use this parameter when the value of the AssemblyName parameter does not include a path. The default is the current directory.

#### AssemblyName [String]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 2,
  ParameterSetName = 'Set 2')]
```

Specifies the name of an assembly file (*.dll) in which the configuration type is defined. You can specify the path to the .dll in this parameter or in the value of the ApplicationBase parameter.
This parameter is required when the ConfigurationTypeName parameter is specified.

#### ConfigurationTypeName [String]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 3,
  ParameterSetName = 'Set 2')]
```

Specifies the fully qualified name of the Microsoft .NET Framework type that is used for this configuration. The type that you specify must implement the System.Management.Automation.Remoting.PSSessionConfiguration class.
To specify the assembly file (.dll) that implements the configuration type, use the AssemblyName and ApplicationBase parameters.
Creating a type allows you to control more aspects of the session configuration, such as exposing or hiding certain parameters of cmdlets, or setting data size and object size limits that users cannot override.
If you omit this parameter, the DefaultRemotePowerShellConfiguration class is used for the session configuration.

#### Force [switch]

Suppresses all users prompts and restarts the WinRM service without prompting. Restarting the service makes the configuration change effective.
To prevent a restart and suppress the restart prompt, use the NoServiceRestart parameter.

#### MaximumReceivedDataSizePerCommandMB [Double]

Limits the amount of data that can be sent to this computer in any single remote command. Enter the data size in megabytes (MB). The default is 50 MB.
If a data size limit is defined in the configuration type that is specified in the ConfigurationTypeName parameter, the limit in the configuration type is used and the value of this parameter is ignored.

#### MaximumReceivedObjectSizeMB [Double]

Limits the amount of data that can be sent to this computer in any single object. Enter the data size in megabytes (MB). The default is 10 MB.
If an object size limit is defined in the configuration type that is specified in the ConfigurationTypeName parameter, the limit in the configuration type is used and the value of this parameter is ignored.

#### Name [String]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true)]
```

Specifies a name for the session configuration. This parameter is required.

#### NoServiceRestart [switch]

Does not restart the WinRM service, and suppresses the prompt to restart the service.
By default, when you enter a Register-PSSessionConfiguration command, you are prompted to restart the WinRM service to make the new session configuration effective.  Until the WinRM service is restarted, the new session configuration is not effective.
To restart the WinRM service without prompting, use the Force parameter. To restart the WinRM service manually, use the Restart-Service cmdlet.

#### Path [String]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 3')]
```

Specifies the path and file name of a session configuration file (.pssc), such as one created by the New-PSSessionConfigurationFile cmdlet. If you omit the path, the default is the current directory.
This parameter is introduced in Windows PowerShell 3.0.

#### ProcessorArchitecture [String]

Determines whether a 32-bit or 64-bit version of the Windows PowerShell process is started in sessions that use this session configuration. Valid values are x86 (32-bit) and AMD64 (64-bit). The default value is determined by the processor architecture of the computer that hosts the session configuration.
You can use this parameter to create a 32-bit session on a 64-bit computer. Attempts to create a 64-bit process on a 32-bit computer fail.

#### PSVersion [Version]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
[Parameter(ParameterSetName = 'Set 2')]
```

Specifies the version of Windows PowerShell in sessions that use this session configuration.
 The value of this parameter takes precedence over the value of the PowerShellVersion key in the session configuration file.
This parameter is introduced in Windows PowerShell 3.0.

#### RunAsCredential [PSCredential]

Runs commands in the session with the permissions of the specified user. By default, commands run with the permissions of the current user.
This parameter is introduced in Windows PowerShell 3.0.

#### SecurityDescriptorSddl [String]

Specifies a Security Descriptor Definition Language (SDDL) string for the configuration.
This string determines the permissions that are required to use the new session configuration. To use a session configuration in a session, users must have at least "Execute(Invoke)" permission for the configuration.
If the security descriptor is complex, consider using the ShowSecurityDescriptorUI parameter instead of this parameter. You cannot use both parameters in the same command.
If you omit this parameter, the root SDDL for the WinRM service is used for this configuration. To view or change the root SDDL, use the WSMan provider. For example "get-item wsman:\localhost\service\rootSDDL". For more information about the WSMan provider, type "get-help wsman".

#### SessionType [PSSessionType]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Specifies the type of session that is created by using the session configuration. The default value is Default. Valid values are:
-- Empty: No modules or snap-ins are added to session by default. Use the parameters of this cmdlet to add modules, functions, scripts, and other features to the session.
-- Default: Adds the Microsoft.PowerShell.Core snap-in to the session. This module includes the Import-Module and Add-PSSnapin cmdlets that users can use to import other modules and snap-ins unless you explicitly prohibit the use of the cmdlets.
-- RestrictedRemoteServer: Includes only the following cmdlets:  Exit-PSSession,Get-Command, Get-FormatData, Get-Help, Measure-Object, Out-Default, and Select-Object.  Use a script or assembly, or the keys in the session configuration file, to add modules, functions, scripts, and other features to the session.
The value of this parameter takes precedence over the value of the  SessionType key in the session configuration file.
This parameter is introduced in Windows PowerShell 3.0.

#### SessionTypeOption [PSSessionTypeOption]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
[Parameter(ParameterSetName = 'Set 2')]
```

Sets type-specific options for the session configuration.  Enter a session type options object, such as the PSWorkflowExecutionOption object that the New-PSWorkflowExecutionOption cmdlet returns.
The options of sessions that use the session configuration are determined by the values of session options and the session configuration options. Unless specified, options set in the session, such as by using the New-PSSessionOption cmdlet, take precedence over options set in the session configuration. However, session option values cannot exceed maximum values set in the session configuration.
This parameter is introduced in Windows PowerShell 3.0.

#### ShowSecurityDescriptorUI [switch]

Displays a property sheet that helps you to create the SDDL for the session configuration. The property sheet appears after you enter the Register-PSSessionConfiguration command and then restart the WinRM service.
When setting the permissions for the configuration, remember that users must have at least "Execute(Invoke)" permission to use the session configuration in a session.
You cannot use the SecurityDescriptorSDDL parameter and this parameter in the same command.

#### StartupScript [String]

Specifies the fully qualified path to a Windows PowerShell script. The specified script runs in the new session that uses the session configuration.
You can use the script to further configure the session. If the script generates an error (even a non-terminating error), the session is not created and the user's New-PSSession command fails.

#### ThreadApartmentState [ApartmentState]

Determines the apartment state of the threads in the session. Valid values are STA, MTA, and Unknown. Unknown is the default.

#### ThreadOptions [PSThreadOptions]

Defines how threads are created and used when a command is executed in the session. Valid values are Default, ReuseThread, UseCurrentThread, and UseNewThread. UseCurrentThread is the default.
For more information, see "PSThreadOptions Enumeration" in MSDN.

#### TransportOption [PSTransportOption]


This parameter is introduced in Windows PowerShell 3.0.

#### UseSharedProcess [switch]

Use only one process to host all sessions that are started by the same user  and use the same session configuration. By default, each session is hosted in its own process.
This parameter is introduced in Windows PowerShell 3.0.

#### ModulesToImport [String[]]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
[Parameter(ParameterSetName = 'Set 2')]
```

Specifies the modules and snap-ins that are automatically imported into sessions that use the session configuration.
By default, only the Microsoft.PowerShell.Core snap-in is imported into sessions, but unless the cmdlets are excluded, users can use the Import-Module and Add-PSSnapin cmdlets to add modules and snap-ins to the session.
The modules specified in this parameter value are imported in additions to modules that are specified by the SessionType parameter and those listed in the ModulesToImport key in the session configuration file (New-PSSessionConfigurationFile). However, settings in the session configuration file can hide the commands exported by modules or prevent users from using them.
This parameter is introduced in Windows PowerShell 3.0.

#### Confirm [switch]

Prompts you for confirmation before running the cmdlet.Prompts you for confirmation before running the cmdlet.

#### WhatIf [switch]

Shows what would happen if the cmdlet runs. The cmdlet is not run.Shows what would happen if the cmdlet runs. The cmdlet is not run.


### INPUTS
#### None
You cannot pipe input to this cmdlet.

### OUTPUTS
#### Microsoft.WSMan.Management.WSManConfigContainerElement


### NOTES
To run this cmdlet on Windows Vista, Windows Server 2008, and later versions of Windows, start Windows PowerShell with the "Run as administrator" option.
This cmdlet generates XML that represents a Web Services for Management (WS-Management) plug-in configuration and sends the XML to WS-Management, which registers the plug-in on the local computer ("new-item wsman:\localhost\plugin").
The properties of a session configuration object vary with the options set for the session configuration and the values of those options. Also, session configurations that use a session configuration file have additional properties.

### EXAMPLES
#### -------------------------- EXAMPLE 1 --------------------------

```powershell
PS C:\>Register-PSSessionConfiguration -Name NewShell -ApplicationBase c:\MyShells\ -AssemblyName MyShell.dll -ConfigurationTypeName MyClass

```
This command registers the NewShell session configuration. It uses the AssemblyName and ApplicationBase parameters to specify the location of the MyShell.dll file, which specifies the cmdlets and providers in the session configuration. It also uses the ConfigurationTypeName parameter to specify a new class that further configures the session.
To use this configuration, type "new-pssession -configurationname newshell".


#### -------------------------- EXAMPLE 2 --------------------------

```powershell
PS C:\>Register-PSSessionConfiguration -name MaintenanceShell -StartupScript C:\ps-test\Maintenance.ps1

```
This command registers the MaintenanceShell configuration on the local computer. The command uses the StartupScript parameter to specify the Maintenance.ps1 script.
When a user uses a New-PSSession command and selects the MaintenanceShell configuration, the Maintenance.ps1 script runs in the new session. The script can configure the session, including importing modules, adding Windows PowerShell snap-ins, and setting the execution policy for the session. If the script generates any errors, including non-terminating errors, the New-PSSession command fails.


#### -------------------------- EXAMPLE 3 --------------------------

```powershell
PS C:\>$sddl = "O:NSG:BAD:P(A;;GA;;;BA)S:P(AU;FA;GA;;;WD)(AU;FA;SA;GWGX;;WD)"
PS C:\>Register-PSSessionConfiguration -name AdminShell -SecurityDescriptorSDDL $sddl -MaximumReceivedObjectSizeMB 20 -StartupScript C:\scripts\AdminShell.ps1

```
This example registers the AdminShell session configuration.
The first command saves a custom SDDL in the $sddl variable.
The second command registers the new shell. The command uses the SecurityDescritorSDDL parameter to specify the SDDL in the value of the $sddl variable and the MaximumReceivedObjectSizeMB parameter to increase the object size limit. It also uses the StartupScript parameter to specify a script that configures the session.
As an alternative to using the SecurityDescriptorSDDL parameter, you can use the ShowSecurityDescriptorUI parameter, which displays a property sheet that you can use to set permissions for the session configuration. When you click "OK" in the property sheet, the tool generates an SDDL for the session configuration.


#### -------------------------- EXAMPLE 4 --------------------------

```powershell
The first command uses the Register-PSSessionConfiguration cmdlet to register the MaintenanceShell configuration. It saves the object that the cmdlet returns in the $s variable.
PS C:\>$s = Register-PSSessionConfiguration -name MaintenanceShell -StartupScript C:\ps-test\Maintenance.ps1

The second command displays the contents of the $s variable.
PS C:\>$s

WSManConfig: Microsoft.WSMan.Management\WSMan::localhost\Plugin
Name                      Type                 Keys
----                      ----                 ----
MaintenanceShell          Container            {Name=MaintenanceShell}


The third command uses the GetType method and its FullName property to display the type name of the object that Register-PSSessionConfiguration returns.
PS C:\>$s.GetType().FullName

TypeName: Microsoft.WSMan.Management.WSManConfigContainerElement


The fourth command uses the Format-List cmdlet to display all the properties of the object that Register-PSSessionConfiguration returns in a list. The PSPath property shows that the object is stored in a directory of the WSMan: drive.
PS C:\>$s | Format-List -Property *

PSPath            : Microsoft.WSMan.Management\WSMan::localhost\Plugin\MaintenanceShell
PSParentPath      : Microsoft.WSMan.Management\WSMan::localhost\Plugin
PSChildName       : MaintenanceShell
PSDrive           : WSMan
PSProvider        : Microsoft.WSMan.Management\WSMan
PSIsContainer     : True
Keys              : {Name=MaintenanceShell}
Name              : MaintenanceShell
TypeNameOfElement : Container


The fifth command uses the Get-ChildItem cmdlet to display the items in the WSMan:\LocalHost\PlugIn path. These include the new MaintenanceShell configuration and the two default configurations that come with Windows PowerShell.
PS C:\>dir WSMan:\LocalHost\Plugin

Name                      Type                 Keys
----                      ----                 ----
MaintenanceShell          Container            {Name=MaintenanceShell}
microsoft.powershell      Container            {Name=microsoft.powershell}
microsoft.powershell32    Container            {Name=microsoft.powershell32}

```
This example shows that a Register-PSSessionConfiguration command returns a WSManConfigContainerElement. It also shows how to find the container elements in the WSMan: drive.


#### -------------------------- EXAMPLE 5 --------------------------

```powershell
PS C:\>Register-PSSessionConfiguration -Name WithProfile -StartupScript Add-Profile.ps1

# Add-Profile.ps1
. c:\users\admin01\documents\windowspowershell\profile.ps1

```
This command creates and registers the WithProfile session configuration on the local computer. The command uses the StartupScript parameter to direct Windows PowerShell to run the specified script in any session that uses the session configuration.
The content of the specified script, Add-Profile.ps1, is also displayed. The script contains a single command that uses dot sourcing to run the user's CurrentUserAllHosts profile in the current scope of the session.
For more information about profiles, see about_Profiles. For more information about dot sourcing, see about_Scopes.


#### -------------------------- EXAMPLE 6 --------------------------

```powershell
The first pair of commands use the New-PSSessionConfigurationFile cmdlet to create two session configuration files. The first command creates a no-Language file. The second command creates a restricted-language file. Other than the value of the LanguageMode parameter, the session configuration files are equivalent.
PS C:\>New-PSSessionConfigurationFile -Path .\NoLanguage.pssc -LanguageMode NoLanguage
PS C:\>New-PSSessionConfigurationFile -Path .\RestrictedLanguage.pssc -LanguageMode Restricted

The second pair of commands use the configuration files to create session configurations on the local computer.
PS C:\>Register-PSSessionConfiguration -Path .\NoLanguage.pssc -Name NoLanguage -Force
PS C:\>Register-PSSessionConfiguration -Path .\RestrictedLanguage.pssc -Name RestrictedLanguage -Force

The third pair of command creates two sessions, each of which uses one of the session configurations that was created in the previous command pair.
PS C:\>$NoLanguage = New-PSSession -ComputerName Srv01 -ConfigurationName NoLanguage
PS C:\>$RestrictedLanguage = New-PSSession -ComputerName Srv01 -ConfigurationName RestrictedLanguage

The seventh command uses the Invoke-Command cmdlet to run an If statement in the no-Language session. The command fails, because the language elements in the command are not permitted in a no-language session.
PS C:\>Invoke-Command -Session $NoLanguage {if ((Get-Date) -lt "1January2014") {"Before"} else {"After"} }
The syntax is not supported by this runspace. This might be because it is in no-language mode.
    + CategoryInfo          : ParserError: (if ((Get-Date) ...") {"Before"}  :String) [], ParseException
    + FullyQualifiedErrorId : ScriptsNotAllowed
    + PSComputerName        : localhost


The eighth command uses the Invoke-Command cmdlet to run the same If statement in the restricted-language session. Because these language elements are permitted in the restricted-language session, the command succeeds.
PS C:\>Invoke-Command -Session $RestrictedLanguage {if ((Get-Date) -lt "1January2014") {"Before"} else {"After"} }
Before

```
The commands in this example compare a no-language session to a restricted-language session. The example shows the effect of using the LanguageMode parameter of New-PSSessionConfigurationFile to limit the types of commands and statements that users can run in a session that uses a custom session configuration. 
To run the commands in this example, start Windows PowerShell with the "Run as administrator" option. This option is required to run the Register-PSSessionConfiguration cmdlet .



### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289605)
[Disable-PSSessionConfiguration]()
[Enable-PSSessionConfiguration]()
[Get-PSSessionConfiguration]()
[New-PSSessionConfigurationFile]()
[New-PSSessionConfigurationOption]()
[Register-PSSessionConfiguration]()
[Set-PSSessionConfiguration]()
[Test-PSSessionConfigurationFile]()
[Unregister-PSSessionConfiguration]()
[WSMan Provider]()
[about_Session_Configurations]()
[about_Session_Configuration_Files]()

## Remove-Job

### SYNOPSIS
Deletes a Windows PowerShell background job.

### DESCRIPTION
The Remove-Job cmdlet deletes Windows PowerShell background jobs that were started by using the Start-Job or the AsJob parameter of any cmdlet.
You can use this cmdlet to delete all jobs or delete selected jobs based on their name, ID, instance ID, command, or state, or by passing a job object to Remove-Job. Without parameters or parameter values, Remove-Job has no effect.
Beginning in Windows PowerShell 3.0, you can use the Remove-Job cmdlet to delete custom job types, such as scheduled jobs and workflow jobs. If you use Remove-Job to delete a scheduled job, it deletes the scheduled job and deletes all instances of the scheduled job on disk, including the results of all triggered job instances.
Before deleting a running job, use the Stop-Job cmdlet to stop the job. If you try to delete a running job, the command fails. You can use the Force parameter of Remove-Job to delete a running job.
If you do not delete a background job, the job remains in the global job cache until you close the session in which the job was created.

### PARAMETERS

#### Command [String[]]

```powershell
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 2')]
```

Deletes jobs that include the specified words in the command.

#### Filter [Hashtable]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 3')]
```

Deletes jobs that satisfy all of the conditions established in the associated hash table. Enter a hash table where the keys are job properties and the values are job property values.
This parameter works only on custom job types, such as workflow jobs and scheduled jobs. It does not work on standard background jobs, such as those created by using the Start-Job cmdlet. For information about support for this parameter, see the help topic for the job type.
This parameter is introduced in Windows PowerShell 3.0.

#### Force [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
[Parameter(ParameterSetName = 'Set 3')]
[Parameter(ParameterSetName = 'Set 4')]
[Parameter(ParameterSetName = 'Set 5')]
[Parameter(ParameterSetName = 'Set 6')]
```

Deletes the job even if the status is "Running". Without the Force parameter, Remove-Job does not delete running jobs.

#### Id [Int32[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 1')]
```

Deletes background jobs with the specified IDs.
The ID is an integer that uniquely identifies the job within the current session. It is easier to remember and type than the instance ID, but it is unique only within the current session. You can type one or more IDs (separated by commas). To find the ID of a job, type "Get-Job" without parameters.

#### InstanceId [Guid[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 4')]
```

Deletes jobs with the specified instance IDs.
An instance ID is a GUID that uniquely identifies the job on the computer. To find the instance ID of a job, use Get-Job or display the job object.

#### Job [Job[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 5')]
```

Specifies the jobs to be deleted. Enter a variable that contains the jobs or a command that gets the jobs. You can also use a pipeline operator to submit jobs to the Remove-Job cmdlet.

#### Name [String[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 6')]
```

Deletes only the jobs with the specified friendly names. Wildcards are permitted.
Because the friendly name is not guaranteed to be unique, even within the session, use the WhatIf and Confirm parameters when deleting jobs by name.

#### State [JobState]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 7')]
```

Deletes only jobs with the specified status. Valid values are Valid values are NotStarted, Running, Completed, Failed, Stopped, Blocked, Disconnected, Suspending, Stopping, and Suspended. To delete jobs with a state of Running, use the Force parameter.

#### Confirm [switch]

Prompts you for confirmation before running the cmdlet.Prompts you for confirmation before running the cmdlet.

#### WhatIf [switch]

Shows what would happen if the cmdlet runs. The cmdlet is not run.Shows what would happen if the cmdlet runs. The cmdlet is not run.


### INPUTS
#### System.Management.Automation.Job
You can pipe a job object to Remove-Job.

### OUTPUTS
#### None
This cmdlet does not generate any output.

### NOTES

### EXAMPLES
#### -------------------------- EXAMPLE 1 --------------------------

```powershell
PS C:\>$batch = Get-Job -Name BatchJob
PS C:\>$batch | Remove-Job

```
These commands delete a background job named BatchJob from the current session. The first command uses the Get-Job cmdlet to get an object representing the job, and then it saves the job in the $batch variable. The second command uses a pipeline operator (|) to send the job to the Remove-Job cmdlet.
This command is equivalent to using the Job parameter of Remove-Job, for example, "remove-job -job $batch".


#### -------------------------- EXAMPLE 2 --------------------------

```powershell
PS C:\>Get-job | Remove-Job

```
This command deletes all of the jobs in the current session.


#### -------------------------- EXAMPLE 3 --------------------------

```powershell
PS C:\>Remove-Job -State NotStarted

```
This command deletes all jobs from the current session that have not yet been started.


#### -------------------------- EXAMPLE 4 --------------------------

```powershell
PS C:\>Remove-Job -Name *batch -Force

```
This command deletes all jobs with friendly names that end with "batch" from the current session, including jobs that are running.
It uses the Name parameter of Remove-Job to specify a job name pattern, and it uses the Force parameter to ensure that all jobs are removed, even those that might be in progress.


#### -------------------------- EXAMPLE 5 --------------------------

```powershell
PS C:\>$j = Invoke-Command -ComputerName Server01 -ScriptBlock {Get-Process} -AsJob
PS C:\>$j | Remove-Job

```
This example shows how to use the Remove-Job cmdlet to remove a job that was started on a remote computer by using the AsJob parameter of the Invoke-Command cmdlet.
The first command uses the Invoke-Command cmdlet to run a job on the Server01 computer. It uses the AsJob parameter to run the command as a background job, and it saves the resulting job object in the $j variable.
Because the command used the AsJob parameter, the job object is created on the local computer, even though the job runs on a remote computer. As a result, you use local commands to manage the job.
The second command uses the Remove-Job cmdlet to remove the job. It uses a pipeline operator (|) to send the job in $j to Remove-Job. Note that this is a local command. A remote command is not required to remove a job that was started by using the AsJob parameter.


#### -------------------------- EXAMPLE 6 --------------------------

```powershell
The first command uses the New-PSSession cmdlet to create a PSSession (a persistent connection) to the Server01 computer. A persistent connection is required when running a Start-Job command remotely. The command saves the PSSession in the $s variable.
PS C:\>$s = New-PSSession -ComputerName Server01

The second command uses the Invoke-Command cmdlet to run a Start-Job command in the PSSession in $s. The job runs a Get-Process command. It uses the Name parameter of Start-Job to specify a friendly name for the job.
PS C:\>Invoke-Command -Session $s -ScriptBlock {Start-Job -ScriptBlock {Get-Process} -Name MyJob}

The third command uses the Invoke-Command cmdlet to run a Remove-Job command in the PSSession in $s. The command uses the Name parameter of Remove-Job to identify the job to be deleted.
PS C:\>Invoke-Command -Session $s -ScriptBlock {Remove-Job -Name MyJob}

```
This example shows how to remove a job that was started by using Invoke-Command to run a Start-Job command. In this case, the job object is created on the remote computer and you use remote commands to manage the job.


#### -------------------------- EXAMPLE 7 --------------------------

```powershell
The first command uses the Start-Job cmdlet to start a background job. The command saves the resulting job object in the $j variable.
PS C:\>$j = Start-Job -ScriptBlock {Get-Process Powershell}

The second command uses a pipeline operator (|) to send the job object in $j to the Format-List cmdlet. The Format-List command uses the Property parameter with a value of * (all) to display all of the properties of the job object in a list.The job object display shows the values of the ID and InstanceID properties, along with the other properties of the object.
PS C:\>$j | Format-List -Property *

HasMoreData   : False
StatusMessage :
Location      : localhost
Command       : get-process powershell
JobStateInfo  : Failed
Finished      : System.Threading.ManualResetEvent
InstanceId    : dce2ee73-f8c9-483e-bdd7-a549d8687eed
Id            : 1
Name          : Job1
ChildJobs     : {Job2}
Output        : {}
Error         : {}
Progress      : {}
Verbose       : {}
Debug         : {}
Warning       : {}
StateChanged  :

The third command uses a Remove-Job command to remove the job from the current session. To generate the command, you can copy and paste the InstanceID value from the object display.To copy a value in the Windows PowerShell console, use the mouse to select the value, and then press Enter to copy it. To paste a value, right-click.
PS C:\>Remove-Job -InstanceID dce2ee73-f8c9-483e-bdd7-a549d8687eed

```
This example shows how to remove a job based on its instance ID.



### RELATED LINKS
[Online Version:](http://technet.microsoft.com/library/hh849742.aspx)
[Get-Job]()
[Invoke-Command]()
[Receive-Job]()
[Resume-Job]()
[Start-Job]()
[Stop-Job]()
[Suspend-Job]()
[Wait-Job]()
[about_Job_Details]()
[about_Remote_Jobs]()
[about_Jobs]()

## Remove-Module

### SYNOPSIS
Removes modules from the current session.

### DESCRIPTION
The Remove-Module cmdlet removes the members of a module, such as cmdlets and functions, from the current session.
If the module includes an assembly (.dll), all members that are implemented by the assembly are removed, but the assembly is not unloaded.
This cmdlet does not uninstall the module or delete it from the computer. It affects only the current Windows PowerShell session.

### PARAMETERS

#### Force [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
[Parameter(ParameterSetName = 'Set 2')]
```

Removes read-only modules. By default, Remove-Module removes only read-write modules.
The ReadOnly and ReadWrite values are stored in AccessMode property of a module.

#### FullyQualifiedName [String[]]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
[Parameter(ParameterSetName = 'Set 3')]
```

Removes modules with names that are specified in the form of ModuleSpecification objects (described by the Remarks section of [Module Specification Constructor (Hashtable)]() on MSDN). For example, the FullyQualifiedName parameter accepts a module name that is specified in the format @{ModuleName = "modulename"; ModuleVersion = "version_number"} or @{ModuleName = "modulename"; ModuleVersion = "version_number"; Guid = "GUID"}. ModuleName and ModuleVersion are required, but Guid is optional.
You cannot specify the FullyQualifiedName parameter in the same command as a Name parameter; the two parameters are mutually exclusive.

#### ModuleInfo [PSModuleInfo[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipeline = $true,
  ParameterSetName = 'Set 1')]
```

Specifies the module objects to remove. Enter a variable that contains a module object (PSModuleInfo) or a command that gets a module object, such as a Get-Module command. You can also pipe module objects to Remove-Module.

#### Name [String[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipeline = $true,
  ParameterSetName = 'Set 2')]
```

Specifies the names of modules to remove. Wildcard characters are permitted. You can also pipe name strings to Remove-Module.

#### Confirm [switch]

Prompts you for confirmation before running the cmdlet.Prompts you for confirmation before running the cmdlet.

#### WhatIf [switch]

Shows what would happen if the cmdlet runs. The cmdlet is not run.Shows what would happen if the cmdlet runs. The cmdlet is not run.


### INPUTS
#### System.String, System.Management.Automation.PSModuleInfo
You can pipe module names (strings) and module objects to Remove-Module.

### OUTPUTS
#### None
Remove-Module does not generate any output.

### NOTES

### EXAMPLES
#### -------------------------- EXAMPLE 1 --------------------------

```powershell
PS C:\>Remove-Module -Name BitsTransfer

```
This command removes the BitsTransfer module from the current session.






#### -------------------------- EXAMPLE 2 --------------------------

```powershell
PS C:\>Get-Module | Remove-Module

```
This command removes all modules from the current session.






#### -------------------------- EXAMPLE 3 --------------------------

```powershell
PS C:\>"FileTransfer", "PSDiagnostics" | Remove-Module -Verbose
VERBOSE: Performing operation "Remove-Module" on Target "filetransfer (Path: 'C:\Windows\system32\WindowsPowerShell\v1.0\Modules\filetransfer\filetransfer.psd1')". 
VERBOSE: Performing operation "Remove-Module" on Target "Microsoft.BackgroundIntelligentTransfer.Management (Path: 'C:\Windows\assembly\GAC_MSIL\Microsoft.BackgroundIntelligentTransfer.Management\1.0.0.0__31bf3856ad364e35\Microsoft.BackgroundIntelligentTransfe
r.Management.dll')".
VERBOSE: Performing operation "Remove-Module" on Target "psdiagnostics (Path: 'C:\Windows\system32\WindowsPowerShell\v1.0\Modules\psdiagnostics\psdiagnostics.psd1')". 
VERBOSE: Removing imported function 'Start-Trace'. 
VERBOSE: Removing imported function 'Stop-Trace'. 
VERBOSE: Removing imported function 'Enable-WSManTrace'. 
VERBOSE: Removing imported function 'Disable-WSManTrace'. 
VERBOSE: Removing imported function 'Enable-PSWSManCombinedTrace'. 
VERBOSE: Removing imported function 'Disable-PSWSManCombinedTrace'. 
VERBOSE: Removing imported function 'Set-LogProperties'. 
VERBOSE: Removing imported function 'Get-LogProperties'. 
VERBOSE: Removing imported function 'Enable-PSTrace'. 
VERBOSE: Removing imported function 'Disable-PSTrace'. 
VERBOSE: Performing operation "Remove-Module" on Target "PSDiagnostics (Path: 'C:\Windows\system32\WindowsPowerShell\v1.0\Modules\psdiagnostics\PSDiagnostics.psm1')".

```
This command removes the BitsTransfer and PSDiagnostics modules from the current session.
The command uses a pipeline operator (|) to send the module names to Remove-Module. It uses the Verbose common parameter to get detailed information about the members that are removed.
The Verbose messages show the items that are removed. The messages differ because the BitsTransfer module includes an assembly that implements its cmdlets and a nested module with its own assembly. The PSDiagnostics module includes a module script file (.psm1) that exports functions.






#### -------------------------- EXAMPLE 4 --------------------------

```powershell
PS C:\>$a = Get-Module BitsTransfer
PS C:\>Remove-Module -ModuleInfo $a

```
This command uses the ModuleInfo parameter to remove the BitsTransfer module.







### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289607)
[Get-Module]()
[Import-Module]()
[about_Modules]()
[about_modules]()

## Remove-PSSession

### SYNOPSIS
Closes one or more Windows PowerShell sessions (PSSessions).

### DESCRIPTION
The Remove-PSSession cmdlet closes Windows PowerShell sessions (PSSessions) in the current session. It stops any commands that are running in the PSSessions, ends the PSSession, and releases the resources that the PSSession was using. If the PSSession is connected to a remote computer, Remove-PSSession also closes the connection between the local and remote computers.
To remove a PSSession, enter the Name, ComputerName, ID, or InstanceID of the session.
If you have saved the PSSession in a variable, the session object remains in the variable, but the state of the PSSession is "Closed."

### PARAMETERS

#### ComputerName [String[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 2')]
```

Closes the PSSessions that are connected to the specified computers. Wildcards are permitted.
Type the NetBIOS name, an IP address, or a fully qualified domain name of one or more remote computers. To specify the local computer, type the computer name, "localhost", or a dot (.).

#### Id [Int32[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 1')]
```

Closes the PSSessions with the specified IDs. Type one or more IDs (separated by commas) or use the range operator (..) to specify a range of IDs
An ID is an integer that uniquely identifies the PSSession in the current session. It is easier to remember and type than the InstanceId, but it is unique only within the current session.  To find the ID of a PSSession, use Get-PSSession without parameters.

#### InstanceId [Guid[]]

```powershell
[Parameter(
  Mandatory = $true,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 3')]
```

Closes the PSSessions with the specified instance IDs.
The instance ID is a GUID that uniquely identifies a PSSession in the current session. The InstanceID is unique, even when you have multiple sessions running on a single computer.
The InstanceID is stored in the InstanceID property of the object that represents a PSSession. To find the InstanceID of the PSSessions in the current session, type "get-pssession | format-table Name, ComputerName, InstanceId".

#### Name [String[]]

```powershell
[Parameter(
  Mandatory = $true,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 4')]
```

Closes the PSSessions with the specified friendly names. Wildcards are permitted.
Because the friendly name of a PSSession might not be unique, when using the Name parameter, consider also using the WhatIf  or Confirm parameter in the Remove-PSSession command.

#### Session [PSSession[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 5')]
```

Specifies the session objects of the PSSessions to close.  Enter a variable that contains the PSSessions or a command that creates or gets the PSSessions, such as a New-PSSession or Get-PSSession command. You can also pipe one or more session objects to Remove-PSSession.

#### Confirm [switch]

Prompts you for confirmation before running the cmdlet.Prompts you for confirmation before running the cmdlet.

#### WhatIf [switch]

Shows what would happen if the cmdlet runs. The cmdlet is not run.Shows what would happen if the cmdlet runs. The cmdlet is not run.


### INPUTS
#### System.Management.Automation.Runspaces.PSSession 
You can pipe a session object to Remove-PSSession.

### OUTPUTS
#### None
Remove-PSSession does not return any objects.

### NOTES
The ID parameter is mandatory. You cannot type "remove-pssession" without parameters. To delete all the PSSessions in the current session, type "get-pssession | remove-pssession".
A PSSession uses a persistent connection to a remote computer. Create a PSSession to run a series of commands that share data. For more information, see about_PSSessions.
PSSessions are specific to the current session. When you end a session, the PSSessions that you created in that session are forcibly closed.


### EXAMPLES
#### -------------------------- EXAMPLE 1 --------------------------

```powershell
PS C:\>remove-pssession -id 1, 2

```
This command removes the PSSessions that have IDs 1 and 2.






#### -------------------------- EXAMPLE 2 --------------------------

```powershell
PS C:\>get-pssession | remove-pssession

- or -

PS C:\>remove-pssession -session (get-pssession)

- or -

PS C:\>$s = get-pssession
PS C:\>remove-pssession -session $s

```
These commands remove all of the PSSessions in the current session. Although the three command formats look different, they have the same effect.






#### -------------------------- EXAMPLE 3 --------------------------

```powershell
PS C:\>$r = get-pssession -computername Serv*
$r | remove-pssession

```
These commands close the PSSessions that are connected to computers that have names that begin with "Serv".






#### -------------------------- EXAMPLE 4 --------------------------

```powershell
PS C:\>get-pssession | where {$_.port -eq 90} | remove-pssession

```
This command closes the PSSessions that are connected to port 90. You can use this command format to identify PSSessions by properties other than ComputerName, Name, InstanceID, and ID.






#### -------------------------- EXAMPLE 5 --------------------------

```powershell
PS C:\>get-pssession | ft computername, instanceID  -auto

ComputerName InstanceId
------------ ----------------
Server01     875d231b-2788-4f36-9f67-2e50d63bb82a
localhost    c065ffa0-02c4-406e-84a3-dacb0d677868
Server02     4699cdbc-61d5-4e0d-b916-84f82ebede1f
Server03     4e5a3245-4c63-43e4-88d0-a7798bfc2414
TX-TEST-01   fc4e9dfa-f246-452d-9fa3-1adbdd64ae85

PS C:\>remove-pssession -InstanceID fc4e9dfa-f246-452d-9fa3-1adbdd64ae85

```
These commands show how to close a PSSession based on its instance ID (RemoteRunspaceID).
The first command uses the Get-PSsession cmdlet to get the PSSessions in the current session. It uses a pipeline operator (|) to send the PSSessions to the Format-Table cmdlet (alias: ft), which formats their ComputerName and InstanceID properties in a table. The AutoSize parameter ("auto") compresses the columns for display.
From the resulting display, the administrator can identify the PSSession to be closed, and copy and paste the InstanceID of that PSSession into the second command.
The second command uses the Remove-PSSession cmdlet to remove the PSSession with the specified instance ID.






#### -------------------------- EXAMPLE 6 --------------------------

```powershell
PS C:\>function EndPSS { get-pssession | remove-pssession }

```
This function deletes all of the PSSessions in the current session. After you add this function to your Windows Powershell profile, to delete all sessions, just type "endpss".







### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289608)
[Connect-PSSession]()
[Disconnect-PSSession]()
[Enter-PSSession]()
[Exit-PSSession]()
[Get-PSSession]()
[Invoke-Command]()
[New-PSSession]()
[Receive-PSSession]()
[about_PSSessions]()
[about_Remote]()

## Remove-PSSnapin

### SYNOPSIS
Removes Windows PowerShell snap-ins from the current session.

### DESCRIPTION
The Remove-PSSnapin cmdlet removes a Windows PowerShell snap-in from the current session. You can use it to remove snap-ins that you have added to Windows PowerShell, but you cannot use it to remove the snap-ins that are installed with Windows PowerShell.
After a snap-in is removed from the current session, it is still loaded, but the cmdlets and providers in the snap-in are no longer available in the session.

### PARAMETERS

#### Name [String[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 1')]
```

Specifies the names of Windows PowerShell snap-ins to remove from the current session. The parameter name ("Name") is optional, and wildcard characters (*) are permitted in the value.

#### PassThru [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Returns an object representing the snap-in. By default, this cmdlet does not generate any output.

#### Confirm [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Prompts you for confirmation before running the cmdlet.Prompts you for confirmation before running the cmdlet.

#### WhatIf [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Shows what would happen if the cmdlet runs. The cmdlet is not run.Shows what would happen if the cmdlet runs. The cmdlet is not run.


### INPUTS
#### System.Management.Automation.PSSnapInInfo
You can pipe a snap-in object to Remove-PSSnapin.

### OUTPUTS
#### None or System.Management.Automation.PSSnapInInfo
By default, Remove-PsSnapin does not generate any output. However, if you use the PassThru parameter, it generates a System.Management.Automation.PSSnapInInfo object representing the snap-in.

### NOTES
You can also refer to Remove-PSSnapin by its built-in alias, "rsnp". For more information, see about_Aliases.
Remove-PSSnapin does not check the version of Windows PowerShell before removing a snap-in from the session. If a snap-in cannot be removed, a warning appears and the command fails.
Remove-PSSnapin affects only the current session. If you have added an Add-PSSnapin command to your Windows PowerShell profile, you should delete the command to remove the snap-in from future sessions. For instructions, see about_Profiles.


### EXAMPLES
#### -------------------------- EXAMPLE 1 --------------------------

```powershell
PS C:\>remove-pssnapin -name Microsoft.Exchange

```
This command removes the Microsoft.Exchange snap-in from the current session. When the command is complete, the cmdlets and providers that the snap-in supported are not available in the session.






#### -------------------------- EXAMPLE 2 --------------------------

```powershell
PS C:\>get-PSSnapIn smp* | remove-PSSnapIn

```
This command removes the Windows PowerShell snap-ins that have names beginning with "smp" from the current session.
The command uses the Get-PSSnapin cmdlet to get objects representing the snap-ins. The pipeline operator (|) sends the results to the Remove-PSSnapin cmdlet, which removes them from the session. The providers and cmdlets that this snap-in supports are no longer available in the session.
When you pipe objects to Remove-PSSnapin, the names of the objects are associated with the Name parameter, which accepts objects from the pipeline that have a Name property.






#### -------------------------- EXAMPLE 3 --------------------------

```powershell
PS C:\>remove-pssnapin -name *event*

```
This command removes all Windows PowerShell snap-ins that have names that include "event". This command specifies the "Name" parameter name, but the parameter name can be omitted because it is optional.







### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289609)
[Add-PSSnapin]()
[Get-PSSnapin]()
[about_Profiles]()

## Resume-Job

### SYNOPSIS
Restarts a suspended job

### DESCRIPTION
The Resume-Job cmdlet resumes a workflow job that was suspended, such as by using the Suspend-Job cmdlet or the about_Suspend-Workflow activity. When a workflow job is resumed, the job engine reconstructs the state, metadata, and output from saved resources, such as checkpoints, so the job is restarted without any loss of state or data. The job state is changed from Suspended to Running.
Use the parameters of Resume-Job to select jobs by name, ID, instance ID or pipe a job object, such as one returned by the Get-Job cmdlet, to Resume-Job. You can also use a property filter to select a job to be resumed. 
By default, Resume-Job returns immediately, even though all jobs might not yet be resumed. To suppress the command prompt until all specified jobs are resumed, use the Wait parameter.
The Resume-Job cmdlet works only on custom job types, such as workflow jobs. It does not work on standard background jobs, such as those that are started by using the Start-Job cmdlet. If you submit a job of an unsupported type, Resume-Job generates a terminating error and stops running. 
To identify a workflow job, look for a value of PSWorkflowJob in the PSJobTypeName property of the job. To determine whether a particular custom job type supports the Resume-Job cmdlet, see the help topics for the custom job type.
NOTE: Before using a Job cmdlet on a custom job type, import the module that supports the custom job type, either by using the Import-Module cmdlet or getting or using a cmdlet in the module.
This cmdlet is introduced in Windows PowerShell 3.0.


### PARAMETERS

#### Filter [Hashtable]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 2')]
```

Resumes only jobs that satisfy all of the conditions established in the associated hash table. Enter a hash table where the keys are job properties and the values are job property values.

#### Id [Int32[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 1')]
```

Resumes the jobs with the specified IDs.
The ID is an integer that uniquely identifies the job within the current session. It is easier to remember and to type than the instance ID, but it is unique only within the current session. You can type one or more IDs (separated by commas). To find the ID of a job, use the Get-Job cmdlet.

#### InstanceId [Guid[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 3')]
```

Resumes jobs with the specified instance IDs. The default is all jobs.
An instance ID is a GUID that uniquely identifies the job on the computer. To find the instance ID of a job, use the Get-Job cmdlet.

#### Job [Job[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 4')]
```

Specifies the jobs to be resumed. Enter a variable that contains the jobs or a command that gets the jobs. You can also pipe jobs to the Resume-Job cmdlet.

#### Name [String[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 5')]
```

Resumes jobs with the specified friendly names. Enter one or more job names. Wildcards are supported.

#### State [JobState]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 6')]
```

Resumes only those jobs in the specified state. Valid values are NotStarted, Running, Completed, Failed, Stopped, Blocked, Suspended, Disconnected, Suspending, and Stopping, but Resume-Job resumes only jobs in the Suspended state.
For more information about job states, see "JobState Enumeration" in MSDN at [http://msdn.microsoft.com/en-us/library/windows/desktop/system.management.automation.jobstate(v=vs.85).aspx]()

#### Wait [switch]

```powershell
[Parameter(ParameterSetName = 'Set 7')]
```

Suspends the command prompt until all specified jobs are resumed. By default, Resume-Job returns immediately.

#### Confirm [switch]

Prompts you for confirmation before running the cmdlet.Prompts you for confirmation before running the cmdlet.

#### WhatIf [switch]

Shows what would happen if the cmdlet runs. The cmdlet is not run.Shows what would happen if the cmdlet runs. The cmdlet is not run.


### INPUTS
#### System.Management.Automation.Job
You can pipe all types of jobs to Resume-Job. However, if Resume-Job gets a job of an unsupported type, it throws a terminating error.

### OUTPUTS
#### None or System.Management.Automation.Job
If you use the PassThru parameter, Resume-Job returns the jobs that it tried to resume. Otherwise, this cmdlet does not generate any output.

### NOTES
Resume-Job can only resume jobs that are suspended. If you submit a job in a different state, Resume-Job runs the resume operation on the job, but generates a warning to notify you that the job could not be resumed. To suppress the warning, use the WarningAction common parameter with a value of SilentlyContinue.
If a job is not of a type that supports resuming, such as a workflow job (PSWorkflowJob), Resume-Job throws a terminating error.
The mechanism and location for saving a suspended job might vary depending on the job type. For example, suspended workflow jobs are saved in a flat file store by default, but can also be saved in a SQL database.
When you resume a job, the job state changes from Suspended to Running. To find the jobs that are running, including those that were resumed by this cmdlet, use the State parameter of the Get-Job cmdlet to get jobs in the Running state.
Some job types have options or properties that prevent Windows PowerShell from suspending the job. If attempts to suspend the job fail, verify that the job options and properties allow suspending.

### EXAMPLES
#### Example 1: Resume a job by ID

```powershell
The first command uses the Get-Job cmdlet to get the job. The output shows that the job is a suspended workflow job.
PS C:\>Get-Job EventJob
Id     Name            PSJobTypeName   State         HasMoreData     Location   Command
--     ----            -------------   -----         -----------     --------   -------
4      EventJob        PSWorkflowJob   Suspended     True            Server01   \\Script\Share\Event.ps1

The second command uses the Id parameter of the Resume-Job cmdlet to resume the job with an Id value of 4.
PS C:\>Resume-Job -Id 4

```
The commands in this example verify that the job is a suspended workflow job and then resume the job.


#### Example 2: Resume a job by name

```powershell
PS C:\>Resume-Job -Name WorkflowJob, InventoryWorkflow, WFTest*


```
This command uses the Name parameter to resume several workflow jobs on the local computer.


#### Example 3: Use custom property values

```powershell
PS C:\>Resume-Job -Filter @{CustomID="T091291"} -State Suspended

```
This command uses the value of a custom property to identify the workflow job to resume. It uses the Filter parameter to identify the workflow job by its CustomID property. It also uses the State parameter to verify that the workflow job is suspended, before it tries to resume it.


#### Example 4: Resume all suspended jobs on a remote computer

```powershell
PS C:\>Invoke-Command -ComputerName Srv01 -ScriptBlock {Get-Job -State Suspended | Resume-Job}

```
This command resumes all suspended jobs on the Srv01 remote computer.
The command uses the Invoke-Command cmdlet to run a command on the Srv01 computer. The remote command uses the State parameter of the Get-Job cmdlet to get all suspended jobs on the computer. A pipeline operator (|) sends the suspended jobs to the Resume-Job cmdlet, which resumes them.


#### Example 4: Wait for jobs to resume

```powershell
PS C:\>Resume-Job -Name WorkflowJob, InventoryWorkflow, WFTest* -Wait

```
This command uses the Wait parameter to direct Resume-Job to return only after all specified jobs are resumed. The Wait parameter is especially useful in scripts that assume that jobs are resumed before the script continues.


#### Example 5: Resume a Workflow that Suspends Itself

```powershell
This code sample shows the Suspend-Workflow activity in a workflow.
#SampleWorkflow
Workflow Test-Suspend
{
    $a = Get-Date
    Suspend-Workflow
    (Get-Date)- $a
}

The following command runs the Test-Suspend workflow on the Server01 computer.When you run the workflow, the workflow runs the Get-Date activity and saves the result in the $a variable. Then it runs the Suspend-Workflow activity. In response, it takes a checkpoint, suspends the workflow, and returns a workflow job object. Suspend-Workflow returns a workflow job object even if the workflow is not explicitly run as a job.
PS C:\>Test-Suspend -PSComputerName Server01
Id     Name            PSJobTypeName   State         HasMoreData     Location             Command

--     ----            -------------   -----         -----------     --------             -------

8      Job8            PSWorkflowJob   Suspended     True            Server01             Test-Suspend

The following command resumes the Test-Suspend workflow in Job8. It uses the Wait parameter to hold the command prompt until the job is resumed.
PS C:\>Resume-Job -Name Job8 -Wait
Id     Name            PSJobTypeName   State         HasMoreData     Location             Command

--     ----            -------------   -----         -----------     --------             -------

8      Job8            PSWorkflowJob   Running       True            Server01             Test-Suspend

This command uses the Receive-Job cmdlet to get the results of the Test-Suspend workflow. The final command in the workflow returns a TimeSpan object that represents the elapsed time between the current date and time and the date and time that was saved in the $a variable before the workflow was suspended.
PS C:\>Receive-Job -Name Job8
        Days              : 0

        Hours             : 0

        Minutes           : 0

        Seconds           : 19

        Milliseconds      : 823

        Ticks             : 198230041

        TotalDays         : 0.000229432917824074

        TotalHours        : 0.00550639002777778

        TotalMinutes      : 0.330383401666667

        TotalSeconds      : 19.8230041

        TotalMilliseconds : 19823.0041

        PSComputerName    : Server01


```
The Resume-Job cmdlet lets you resume a workflow job that was suspend by using the Suspend-Workflow activity. This activity suspends a workflow from within a workflow. It is valid only in workflows.
For information about the Suspend-Workflow, see about_Suspend-Workflow.



### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289610)
[Get-Job]()
[Receive-Job]()
[Remove-Job]()
[Resume-Job]()
[Start-Job]()
[Stop-Job]()
[Suspend-Job]()
[Wait-Job]()

## Save-Help

### SYNOPSIS
Downloads and saves the newest help files to a file system directory.

### DESCRIPTION
The Save-Help cmdlet downloads the newest help files for Windows PowerShell modules and saves them to a directory that you specify. This feature lets you update the help files on computers that do not have access to the Internet, and makes it easier to update the help files on multiple computers. 
In Windows PowerShell 3.0, Save-Help worked only for modules that are installed on the local computer. Although it was possible to import a module from a remote computer, or obtain a reference to a PSModuleInfo object from a remote computer by using Windows PowerShell remoting, the HelpInfoUri property was not preserved, and Save-Help would not work for remote module Help. 
In Windows PowerShell 4.0, the HelpInfoUri property is preserved over Windows PowerShell remoting, which allows Save-Help to work for modules that are installed on remote computers. It is also possible to save a PSModuleInfo object to disk or removable media by running Export-CliXml on a computer that does not have Internet access, import the object on a computer that does have Internet access, and then run Save-Help on the PSModuleInfo object. The saved help can be transported to the remote computer by using removable storage media (such as a USB drive), and then the help can be installed on the remote computer by running Update-Help. This process can be used to install help on computers that do not have any kind of network access.
To install saved help files, run the Update-Help cmdlet. Add its SourcePath parameter to specify the directory in which you saved the Help files.
Without parameters, a Save-Help command downloads the newest help for all modules in the session and for modules that are installed on the computer in a location listed in the PSModulePath environment variable. Modules that do not support Updatable Help are skipped without warning.
The Save-Help cmdlet checks the version of any help files in the destination directory and, if newer help files are available, it downloads the newest help files from the Internet and saves them in the directory. The Save-Help cmdlet works just like the Update-Help cmdlet, except that it saves the downloaded cabinet (.cab) files in a directory, instead of extracting the help files from the cabinet files and installing them on the computer.
The saved help for each module consists of one help information (HelpInfo XML) file and one cabinet (.cab) file for the help files each UI culture. You do not need to extract the help files from the cabinet file. The Update-Help cmdlet extracts the help files, validates the XML for safety, and then installs the help files and the help information file in a language-specific subdirectory of the module directory.
To save the help files for modules in the Windows PowerShell installation directory ($pshome\Modules), start Windows PowerShell with the "Run as administrator" option. You must be a member of the Administrators group on the computer to download the help files for these modules.
This cmdlet is introduced in Windows PowerShell 3.0.

### PARAMETERS

#### Credential [PSCredential]

Runs the command with credentials of a user who has permission to access the file system location specified by the DestinationPath parameter. This parameter is valid only when the DestinationPath or LiteralPath parameter is used in the command.
This parameter enables you to run Save-Help commands with the DestinationPath parameter on remote computers. By providing explicit credentials, you can run the command on a remote computer and access a file share on a third computer without encountering an "access denied" error or using CredSSP authentication to delegate credentials.

#### DestinationPath [String[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 1')]
```

Specifies the path to the directory in which the help files are saved. Enter the path to a directory. Do not specify a file name or file name extension.

#### Force [switch]

Overrides the once-per-day limitation, circumvents version checking, and downloads files that exceed the 1 GB limit
Without this parameter, only one Save-Help command for each module is permitted in each 24-hour period, downloads are limited to 1 GB of uncompressed content per module, and help files for a module are installed only when they are newer than the files on the computer.
The once-per-day limit protects the servers that host the help files, and makes it practical for you to add a Save-Help command to your Windows PowerShell profile.
To save help for a module in multiple UI cultures without the Force parameter, include all UI cultures in the same command, such as: Save-Help -Module PSScheduledJobs -UICulture en-US, fr-FR, pt-BR

#### LiteralPath [String[]]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 2')]
```

Specifies a path to the destination directory. Unlike the value of the DestinationPath parameter, the value of the LiteralPath parameter is used exactly as it is typed. No characters are interpreted as wildcards. If the path includes escape characters, enclose it in single quotation marks. Single quotation marks tell Windows PowerShell not to interpret any characters as escape sequences.

#### FullyQualifiedModule [ModuleSpecification[]]

```powershell
[Parameter(
  Position = 2,
  ValueFromPipelineByPropertyName = $true)]
```

Specifies modules with names that are specified in the form of ModuleSpecification objects (described by the Remarks section of [Module Specification Constructor (Hashtable)]() on MSDN). For example, the FullyQualifiedModule parameter accepts a module name that is specified in the format @{ModuleName = "modulename"; ModuleVersion = "version_number"} or @{ModuleName = "modulename"; ModuleVersion = "version_number"; Guid = "GUID"}. ModuleName and ModuleVersion are required, but Guid is optional.
You cannot specify the FullyQualifiedModule parameter in the same command as a Module parameter; the two parameters are mutually exclusive.

#### Module [String[]]

```powershell
[Parameter(
  Position = 2,
  ValueFromPipelineByPropertyName = $true)]
```

Downloads help for the specified modules. Enter one or more module names or name patters in a comma-separated list or in a file with one module name on each line. Wildcard characters are permitted. You can also pipe module objects from the Get-Module cmdlet to Save-Help. 
By default, Save-Help downloads help for all modules that support Updatable Help and are installed on the local computer in a location listed in the PSModulePath environment variable.
To save help for modules that are not installed on the computer, run a Get-Module command on a remote computer. Then pipe the resulting module objects to the Save-Help cmdlet or submit the module objects as the value of the Module or InputObject parameters.
If the module that you specify is installed on the computer, you can enter the module name or a module object. If the module is not installed on the computer, you must enter a module object, such as one returned by the Get-Module cmdlet.
The Module parameter of the Save-Help cmdlet does not accept the full path to a module file or module manifest file. To save help for a module that is not in a PSModulePath location, import the module into the current session before running the Save-Help command.
A value of "*" (all) attempts to update help for all modules that are installed on the computer, including modules that do not support Updatable Help. This value might generate errors as the command encounters modules that do not support Updatable Help.

#### UICulture [CultureInfo[]]

```powershell
[Parameter(Position = 3)]
```

Gets updated help files for the specified UI culture. Enter one or more language codes, such as "es-ES", a variable that contains culture objects, or a command that gets culture objects, such as a Get-Culture or Get-UICulture command.
Wildcards are not permitted and you cannot submit a partial language code, such as "de".
By default, Save-Help gets help files in the UI culture set for Windows or its fallback culture. If you use the UICulture parameter, Save-Help looks for help only for the specified UI culture, not in any fallback culture.

#### UseDefaultCredentials [switch]

Runs the command, including the web download, with the credentials of the current user. By default, the command runs without explicit credentials.
This parameter is effective only when the web download uses NTLM, negotiate, or Kerberos-based authentication.


### INPUTS
#### System.Management.Automation.PSModuleInfo
You can pipe a module object from the Get-Module cmdlet to the Module parameter of Save-Help.

### OUTPUTS
#### None
Save-Help does not generate any output.

### NOTES
To save help for modules in the $pshome\Modules directory, start Windows PowerShell with the "Run as administrator" option. Only  members of the Administrators group on the computer can download help for the for modules in the $pshome\Modules directory.
The saved help for each module consists of  one help information (HelpInfo XML) file and one cabinet (.cab) file for the help files each UI culture. You do not need to extract the help files from the cabinet file. The Update-Help cmdlet extracts the help files, validates the XML, and then installs the help files and the help information file in a language-specific subdirectory of the module directory.
The Save-Help cmdlet can save help for modules that are not installed on the computer. However, because help files are installed in the module directory, the Update-Help cmdlet can install updated help file only for modules that are installed on the computer.
If Save-Help cannot find updated help files for a module, or cannot find updated help files in the specified language, it continues silently without displaying an error message. To see which files were saved by the command, use the Verbose parameter.
Modules are the smallest unit of updatable help. You cannot save help for a particular cmdlet; only for all cmdlets in module. To find the module that contains a particular cmdlet, use the ModuleName property of the Get-Command cmdlet, for example, (Get-Command <cmdlet-name>).ModuleName
Save-Help supports all modules and the Windows PowerShell Core snap-ins. It does not support any other snap-ins.
The Update-Help and Save-Help cmdlets use the following ports to download help files: Port 80 for HTTP and port 443 for HTTPS.
The Update-Help and Save-Help cmdlets are not supported on Windows Preinstallation Environment (Windows PE).

### EXAMPLES
#### Example 1: Save the help for the DhcpServer module

```powershell
PS C:\># Option 1: Run Invoke-Command to get the PSModuleInfo object for the remote DHCP Server module, save the PSModuleInfo object in the variable $m, and then run Save-Help.

$m = Invoke-Command -ComputerName RemoteServer -ScriptBlock { Get-Module -Name DhcpServer -ListAvailable }
Save-Help -Module $m -DestinationPath C:\SavedHelp


# Option 2: Open a PSSession--targeted at the remote computer that is running the DhcpServer module--to get the PSModuleInfo object for the remote module, and then run Save-Help.

$s = New-PSSession -ComputerName RemoteServer
$m = Get-Module -PSSession $s -Name DhcpServer -ListAvailable
Save-Help -Module $m -DestinationPath C:\SavedHelp


# Option 3: Open a CIM session--targeted at the remote computer that is running the DhcpServer module--to get the PSModuleInfo object for the remote module, and then run Save-Help.

$c = New-CimSession -ComputerName RemoteServer
$m = Get-Module -CimSession $c -Name DhcpServer -ListAvailable
Save-Help -Module $m -DestinationPath C:\SavedHelp

```
This example shows three different ways to use Save-Help to save the help for the DhcpServer module from an Internet-connected client computer, without installing the DhcpServer module or the DHCP Server role on the local computer.


#### Example 2: Install help for the DhcpServer module

```powershell
PS C:\># First, run Export-CliXml to export the PSModuleInfo object to a shared folder or to removable media.

$m = Get-Module -Name DhcpServer -ListAvailable
Export-CliXml -Path E:\UsbFlashDrive\DhcpModule.xml -InputObject $m

# Next, transport the removable media to a computer that has Internet access, and then import the PSModuleInfo object with Import-CliXml. Run Save-Help to save the Help for the imported DhcpServer module PSModuleInfo object.

$deserialized_m = Import-CliXml E:\UsbFlashDrive\DhcpModule.xml
Save-Help -Module $deserialized_m -DestinationPath E:\UsbFlashDrive\SavedHelp

# Finally, transport the removable media back to the computer that does not have network access, and then install the help by running Update-Help.
Update-Help -Module DhcpServer -SourcePath E:\UsbFlashDrive\SavedHelp

```
This example shows how to install help that you saved in Example 1 for the DhcpServer module on a computer that does not have Internet access.


#### Example 3: Save help for all modules

```powershell
PS C:\>Save-Help -DestinationPath \\Server01\FileShare01

```
This command downloads the newest help files for all modules in the UI culture set for Windows on the local computer. It saves the help files in the \\Server01\Fileshare01 directory.


#### Example 4: Save help for a module on the computer

```powershell
PS C:\>Save-Help -Module ServerManager -DestinationPath \\Server01\FileShare01 -Credential Domain01/Admin01

```
This command downloads the newest help files for the ServerManager module and saves them in the \\Server01\Fileshare01 directory.
When a module is installed on the computer, you can type the module name as the value of the Module parameter, even if the module is not imported into the current session.
The command uses the Credential parameter to supply the credentials of a user who has permission to write to the file share.


#### Example 5: Save help for a module on a different computer

```powershell
PS C:\>Invoke-Command -ComputerName Server02 {Get-Module -Name CustomSQL -ListAvailable} | Save-Help -DestinationPath \\Server01\FileShare01 -Credential Domain01\Admin01

```
These commands download the newest help files for the CustomSQL module and save them in the \\Server01\Fileshare01 directory. 
Because the CustomSQL module is not installed on the computer, the sequence includes an Invoke-Command command that gets the module object for the CustomSQL module from the Server02 computer and then pipes the module object to the Save-Help cmdlet. 
When a module is not installed on the computer, Save-Help needs the module object, which includes information about the location of the newest help files.


#### Example 6: Save help for a module in multiple languages

```powershell
PS C:\>Save-Help -Module Microsoft.PowerShell* -UICulture de-DE, en-US, fr-FR, ja-JP -DestinationPath D:\Help

```
This command saves help for the Windows PowerShell Core modules in four different UI cultures. The language packs for these locales do not need to be installed on the computer.
Save-Help can download help files for modules in different UI cultures only when the module owner makes the translated files available on the Internet.


#### Example 7: Save help more than once each day

```powershell
PS C:\>Save-Help -Force -DestinationPath \\Server3\AdminShare\Help

```
This command saves help for all modules that are installed on the computer. The command uses It uses the Force parameter to override the rule that prevents the Save-Help cmdlet from downloading help more than once in each 24-hour period.
The Force parameter also overrides the 1 GB restriction and circumvents version checking, so you can download files even if the version is not greater than the version in the destination directory.
The command uses the Save-Help cmdlet to download and save the help files to the specified directory. The Force parameter is required when you need to run a Save-Help command more than once each day. 



### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289611)
[Updatable Help Status Table (http://go.microsoft.com/fwlink/?LinkID=270007)]()
[Get-Culture]()
[Get-Help]()
[Get-Module]()
[Get-UICulture]()
[Update-Help]()

## Set-PSDebug

### SYNOPSIS
Turns script debugging features on and off, sets the trace level, and toggles strict mode.

### DESCRIPTION
The Set-PSDebug cmdlet turns script debugging features on and off, sets the trace level, and toggles strict mode.
When the Trace parameter is set to 1, each line of script is traced as it is executed. When the parameter is set to 2, variable assignments, function calls, and script calls are also traced. If the Step parameter is specified, you are prompted before each line of the script is executed.

### PARAMETERS

#### Off [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Turns off all script debugging features.
Note: A "set-strictmode -off" command disables the verification set by a "set-psdebug -strict" command. For more information, see Set-StrictMode.

#### Step [switch]

```powershell
[Parameter(ParameterSetName = 'Set 2')]
```

Turns on script stepping. Before each line is run, the user is prompted to stop, continue, or enter a new interpreter level to inspect the state of the script.
Note: Specifying the Step parameter automatically sets a Trace level of 1.

#### Strict [switch]

```powershell
[Parameter(ParameterSetName = 'Set 2')]
```

Specifies that the interpreter should throw an exception if a variable is referenced before a value is assigned to the variable.
Note: A "set-strictmode -off" command disables the verification set by a "set-psdebug -strict" command. For more information, see Set-StrictMode.

#### Trace [Int32]

```powershell
[Parameter(ParameterSetName = 'Set 2')]
```

Specifies the trace level:
0 - Turn script tracing off
1 - Trace script lines as they are executed
2 - Trace script lines, variable assignments, function calls, and scripts.


### INPUTS
#### None
You cannot pipe input to this cmdlet.

### OUTPUTS
#### None
This cmdlet does not return any output.

### NOTES

### EXAMPLES
#### -------------------------- EXAMPLE 1 --------------------------

```powershell
PS C:\>set-psdebug -trace 2; foreach ($i in 1..3) {$i}

DEBUG:    1+ Set-PsDebug -trace 2; foreach ($i in 1..3) {$i}
DEBUG:    1+ Set-PsDebug -trace 2; foreach ($i in 1..3) {$i}
1
DEBUG:    1+ Set-PsDebug -trace 2; foreach ($i in 1..3) {$i}
2
DEBUG:    1+ Set-PsDebug -trace 2; foreach ($i in 1..3) {$i}
3

```
This command sets the trace level to 2, and then runs a script that displays the numbers 1, 2, and 3.






#### -------------------------- EXAMPLE 2 --------------------------

```powershell
PS C:\>set-psdebug -step; foreach ($i in 1..3) {$i}

DEBUG:    1+ Set-PsDebug -step; foreach ($i in 1..3) {$i}
Continue with this operation?
1+ Set-PsDebug -step; foreach ($i in 1..3) {$i}
[Y] Yes  [A] Yes to All  [N] No  [L] No to All  [S] Suspend  [?] Help
(default is "Y"):a
DEBUG:    1+ Set-PsDebug -step; foreach ($i in 1..3) {$i}
1
2
3

```
This command turns on stepping and then
runs a script that displays the numbers 1, 2, and 3.






#### -------------------------- EXAMPLE 3 --------------------------

```powershell
PS C:\>set-psdebug -off; foreach ($i in 1..3) {$i}
1
2
3

```
This command turns off all debugging features, and then runs a script that displays the numbers 1, 2, and 3.






#### -------------------------- EXAMPLE 4 --------------------------

```powershell
PS C:\>set-psdebug -strict; $NewVar
The variable $NewVar cannot be retrieved because it has not been set yet.
At line:1 char:28
+ Set-PsDebug -strict;$NewVar <<<<

```
This command puts the interpreter in strict mode, and attempts to access a variable that has not yet been set.







### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289612)
[Debug-Process]()
[Set-PSBreakpoint]()
[Set-StrictMode]()
[Write-Debug]()
[about_Debuggers]()

## Set-PSSessionConfiguration

### SYNOPSIS
Changes the properties of a registered session configuration.

### DESCRIPTION
The Set-PSSessionConfiguration cmdlet changes the properties of the session configurations on the local computer.
Use the Name parameter to identify the session configuration that you want to change. Use the other parameters to specify new values for the properties of the session configuration. To delete a property value from the configuration (and use the default value), enter an empty string ("") or a value of $null for the corresponding parameter.
Beginning in Windows PowerShell 3.0, you can use a session configuration file to define a session configuration. This feature provides a simple and discoverable method for setting and changing the properties of sessions that use the session configuration. To specify a session configuration file, use the Path parameter of Set-PSSessionConfiguration. For information about session configuration files, see about_Session_Configuration_Files (http://go.microsoft.com/fwlink/?LinkID=236023). For information about creating and editing a session configuration file, see New-PSSessionConfigurationFile.
Session configurations define the environment of remote sessions (PSSessions) that connect to the local computer. Every PSSession uses a session configuration. The session configuration determines the features of the PSSession, such as the modules that are available in the session, the cmdlets that are permitted to run, the language mode, quotas, and timeouts. The security descriptor (SDDL) of the session configuration determines who can use the session configuration to connect to the local computer. For more information about session configurations, see about_Session_Configurations.
To see the properties of a session configuration, use the Get-PSSessionConfiguration cmdlet or the WSMan Provider. For more information about the WSMan Provider, type "Get-Help WSMan".

### PARAMETERS

#### ApplicationBase [String]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
[Parameter(ParameterSetName = 'Set 2')]
```

Specifies the path to the assembly file (*.dll) that is specified in the value of the AssemblyName parameter.

#### AssemblyName [String]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 2,
  ParameterSetName = 'Set 2')]
```

Creates a session configuration based on a class that is defined in an assembly.
Enter the path (optional) and file name of an assembly (a .dll file) that defines a session configuration. If you enter only the name, you can enter the path in the value of the ApplicationBase parameter.

#### ConfigurationTypeName [String]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 3,
  ParameterSetName = 'Set 2')]
```

Specifies the type of the session configuration that is defined in the assembly in the AssemblyName parameter. The type that you specify must implement the System.Management.Automation.Remoting.PSSessionConfiguration class.
This parameter is required when you specify an assembly name.

#### Force [switch]

Suppresses all user prompts, and restarts the WinRM service without prompting. Restarting the service makes the configuration change effective.
To prevent a restart and suppress the restart prompt, use the NoServiceRestart parameter.

#### MaximumReceivedDataSizePerCommandMB [Double]

Changes the limit on the amount of data that can be sent to this computer in any single remote command. Enter the data size in megabytes (MB). The default is 50 MB.
If a data size limit is defined in the configuration type that is specified in the ConfigurationTypeName parameter, the limit in the configuration type is used and the value of this parameter is ignored.

#### MaximumReceivedObjectSizeMB [Double]

Changes the limits on the amount of data that can be sent to this computer in any single object. Enter the data size in megabytes (MB). The default is 10 MB.
If an object size limit is defined in the configuration type that is specified in the ConfigurationTypeName parameter, the limit in the configuration type is used and the value of this parameter is ignored.

#### Name [String]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true)]
```

Specifies the name of the session configuration that you want to change.
You cannot use this parameter to change the name of the session configuration.

#### NoServiceRestart [switch]

Does not restart the WinRM service, and suppresses the prompt to restart the service.
By default, when you enter a Set-PSSessionConfiguration command, you are prompted to restart the WinRM service to make the new session configuration effective.  Until the WinRM service is restarted, the new session configuration is not effective.
To restart the WinRM service without prompting, use the Force parameter. To restart the WinRM service manually, use the Restart-Service cmdlet.

#### PSVersion [Version]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
[Parameter(ParameterSetName = 'Set 2')]
```

Specifies the version of Windows PowerShell in sessions that use this session configuration.
 The value of this parameter takes precedence over the value of the PowerShellVersion key in the session configuration file.
This parameter is introduced in Windows PowerShell 3.0.

#### RunAsCredential [PSCredential]

Runs commands in the session with the permissions of the specified user. By default, commands run with the permissions of the current user.
This parameter is introduced in Windows PowerShell 3.0.

#### SecurityDescriptorSddl [String]

Specifies a different Security Descriptor Definition Language (SDDL) string for the configuration.
This string determines the permissions that are required to use the new session configuration. To use a session configuration in a session, users must have at least "Execute(Invoke)" permission for the configuration.
To use the default security descriptor for the configuration, enter an empty string ("") or a value of $null. The default is the root SDDL in the WSMan: drive.
If the security descriptor is complex, consider using the ShowSecurityDescriptorUI parameter instead of this one. You cannot use both parameters in the same command.

#### ShowSecurityDescriptorUI [switch]

Displays a property sheet that helps you to create a new SDDL for the session configuration. The property sheet appears after you enter the Set-PSSessionConfiguration command and then restart the WinRM service.
When setting the permissions to the configuration, remember that users must have at least "Execute(Invoke)" permission to use the session configuration in a session.
You cannot use the SecurityDescriptorSDDL parameter and this parameter in the same command.

#### StartupScript [String]

Adds or changes the startup script for the configuration. Enter the fully qualified path to a Windows PowerShell script. The specified script runs in the new session that uses the session configuration.
To delete a startup script from a session configuration, enter an empty string ("") or a value of $null.
You can use a startup script to further configure the user's session. If the script generates an error (even a non-terminating error), the session is not created and the user's New-PSSession command fails.

#### ThreadApartmentState [ApartmentState]

Changes the apartment state setting for the threads in the session. Valid values are STA, MTA and Unknown. Unknown is the default.

#### ThreadOptions [PSThreadOptions]

Changes the thread options setting in the configuration. This setting defines how threads are created and used when a command is executed in the session. Valid values are Default, ReuseThread, UseCurrentThread, and UseNewThread. UseCurrentThread is the default.
For more information, see "PSThreadOptions Enumeration" in MSDN.

#### UseSharedProcess [switch]

Use only one process to host all sessions that are started by the same user  and use the same session configuration. By default, each session is hosted in its own process.
This parameter is introduced in Windows PowerShell 3.0.

#### AccessMode [PSSessionConfigurationAccessMode]

Enables and disables the session configuration and determines whether it can be used for remote or local sessions on the computer. Remote is the default.
Valid values are:

--  Disabled: Disables the session configuration. It cannot be used for remote or local access to the computer. This value sets the Enabled property of the session configuration (WSMan:\<ComputerName>\PlugIn\<SessionConfigurationName>\Enabled) to False.

--  Local: Adds a Network_Deny_All entry to security descriptor of the session configuration. Users of the local computer can use the session configuration to create a local "loopback" session on the same computer, but remote users are denied access.

 
                        
--  Remote: Removes Deny_All and Network_Deny_All entries from the security descriptors of the session configuration. Users of local and remote computers can use the session configuration to create sessions and run commands on this computer.
The value of this parameter can be overridden by the actions of other cmdlets. For example, the Enable-PSRemoting cmdlet enables all session configurations on the computer and permits remote access to them, and the Disable-PSRemoting cmdlet permits only local access to all session configurations on the computer.
This parameter is introduced in Windows PowerShell 3.0.

#### SessionTypeOption [PSSessionTypeOption]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
[Parameter(ParameterSetName = 'Set 2')]
```

Sets type-specific options for the session configuration.  Enter a session type options object, such as the PSWorkflowExecutionOption object that the New-PSWorkflowExecutionOption cmdlet returns.
The options of sessions that use the session configuration are determined by the values of session options and the session configuration options. Unless specified, options set in the session, such as by using the New-PSSessionOption cmdlet, take precedence over options set in the session configuration. However, session option values cannot exceed maximum values set in the session configuration.
This parameter is introduced in Windows PowerShell 3.0.

#### TransportOption [PSTransportOption]

Sets transport options for the session configuration.  Enter a transport options object, such as the WSManConfigurationOption object that the New-PSTransportOption cmdlet returns.
The options of sessions that use the session configuration are determined by the values of session options and the session configuration options. Unless specified, options set in the session, such as by using the New-PSSessionOption cmdlet, take precedence over options set in the session configuration. However, session option values cannot exceed maximum values set in the session configuration.
This parameter is introduced in Windows PowerShell 3.0.

#### Path [String]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 3')]
```

Adds or replaces a session configuration file (.pssc), such as one created by the New-PSSessionConfigurationFile cmdlet. If you omit the path, the default is the current directory.
For information about editing a session configuration file, see the help topic for the New-PSSessionConfigurationFile cmdlet.
This parameter is introduced in Windows PowerShell 3.0.

#### ModulesToImport [String[]]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
[Parameter(ParameterSetName = 'Set 2')]
```

Specifies the modules and snap-ins that are automatically imported into sessions that use the session configuration. Enter the module and snap-in names.
By default, only the Microsoft.PowerShell.Core snap-in is imported into sessions, but unless the cmdlets are excluded, users can use the Import-Module and Add-PSSnapin cmdlets to add modules and snap-ins to the session.
The modules specified in this parameter value are imported in additions to modules specified in the session configuration file (New-PSSessionConfigurationFile). However, settings in the session configuration file can hide the commands exported by modules or prevent users from using them.
The modules specified in this parameter value replace the list of modules specified by using the ModulesToImport parameter of the Register-PSSessionConfiguration cmdlet.
This parameter is introduced in Windows PowerShell 3.0.

#### Confirm [switch]

Prompts you for confirmation before running the cmdlet.Prompts you for confirmation before running the cmdlet.

#### WhatIf [switch]

Shows what would happen if the cmdlet runs. The cmdlet is not run.Shows what would happen if the cmdlet runs. The cmdlet is not run.


### INPUTS
#### None
You cannot pipe input to this cmdlet.

### OUTPUTS
#### Microsoft.WSMan.Management.WSManConfigLeafElement


### NOTES
To run this cmdlet, start Windows PowerShell with the "Run as administrator" option.
The Set-PSSessionConfiguration cmdlet does not change the configuration name and the WSMan provider does not support the Rename-Item cmdlet. To change the name of a session configuration, use the Unregister-PSSessionConfiguration cmdlet to delete the configuration and then use the Register-PSSessionConfiguration cmdlet to create and register a new session configuration.
You can use the Set-PSSessionConfiguration cmdlet to change the default Microsoft.PowerShell and Microsoft.PowerShell32 session configurations. They are not protected. To revert to the original version of a default session configuration, use the Unregister-PSSessionConfiguration cmdlet to delete the default session configuration and then use the Enable-PSRemoting cmdlet to restore it.
The properties of a session configuration object vary with the options set for the session configuration and the values of those options. Also, session configurations that use a session configuration file have additional properties.
You can use commands in the WSMan: drive to change the properties of session configurations. However, you cannot use the WSMan: drive in Windows PowerShell 2.0 to change session configuration properties that are introduced in Windows PowerShell 3.0, such as OutputBufferingMode. Windows PowerShell 2.0 commands do not generate an error, but they are ineffective. To change  properties introduced in Windows PowerShell 3.0, use the WSMan: drive in Windows PowerShell 3.0.

### EXAMPLES
#### -------------------------- EXAMPLE 1 --------------------------

```powershell
PS C:\>Set-PSSessionConfiguration -Name MaintenanceShell -ThreadApartmentState STA

```
This command changes the thread apartment state in the MaintenanceShell configuration to STA. The change is effective when you restart the WinRM service.






#### -------------------------- EXAMPLE 2 --------------------------

```powershell
The first command uses the Register-PSSessionConfiguration cmdlet to create the AdminShell configuration.
PS C:\>Register-PSSessionConfiguration -name AdminShell -AssemblyName C:\Shells\AdminShell.dll -ConfigurationType AdminClass

The second command uses the Set-PSSessionConfiguration cmdlet to add the AdminConfig.ps1 script to the configuration. The change is effective when you restart WinRM.
PS C:\>Set-PSSessionConfiguration -Name AdminShell -StartupScript AdminConfig.ps1

The third command removes the AdminConfig.ps1 script from the configuration. It uses the Set-PSSessionConfiguration cmdlet with a value of $null for the StartupScript parameter.
PS C:\>Set-PSSessionConfiguration -Name AdminShell -StartupScript $null

```
This example shows how to create and then change a session configuration.


#### -------------------------- EXAMPLE 3 --------------------------

```powershell
PS C:\>Set-PSSessionConfiguration -name IncObj -MaximumReceivedObjectSizeMB 20

WSManConfig: Microsoft.WSMan.Management\WSMan::localhost\Plugin\IncObj\InitializationParameters
ParamName                       ParamValue
---------                       ----------
psmaximumreceivedobjectsizemb   20
"Restart WinRM service"
WinRM service need to be restarted to make the changes effective. Do you want to run the command "restart-service winrm"?
[Y] Yes  [N] No  [S] Suspend  [?] Help (default is "Y"): y

```
This example shows sample output from the Set-PSSessionConfiguration cmdlet.
The Set-PSSessionConfiguration command in this example increases the value of the MaximumReceivedObjectSizeMB property to 20.
The Set-PSSessionConfiguration command returns a Microsoft.WSMan.Management.WSManConfigLeafElement object that shows the parameter name and new value.
It also prompts you to restart the WinRM service. The Set-PSSessionConfiguration change is not effective until the WinRM service is restarted.






#### -------------------------- EXAMPLE 4 --------------------------

```powershell
The first command uses the Set-PSSessionConfiguration cmdlet to change the startup script in the MaintenanceShell session configuration to Maintenance.ps1. The output of this command shows the change and prompts you to restart the WinRM service. The response is "y" (yes).
PS C:\>Set-PSSessionConfiguration -Name MaintenanceShell -StartupScript C:\ps-test\Maintenance.ps1
WSManConfig: Microsoft.WSMan.Management\WSMan::localhost\Plugin\MaintenanceShell\InitializationParameters

ParamName            ParamValue
---------            ----------
startupscript        c:\ps-test\Mainte...

"Restart WinRM service"
WinRM service need to be restarted to make the changes effective. Do you want to run
the command "restart-service winrm"?
[Y] Yes  [N] No  [S] Suspend  [?] Help (default is "Y"): y


The second command uses the Get-PSSessionConfiguration cmdlet to get the MaintenanceShell session configuration. The command uses a pipeline operator (|) to send the results of the command to the Format-List cmdlet, which displays all of the properties of the session configuration object in a list.
PS C:\>Get-PSSessionConfiguration MaintenanceShell | Format-List -Property *
xmlns            : http://schemas.microsoft.com/wbem/wsman/1/config/PluginConfiguration
Name             : MaintenanceShell
Filename         : %windir%\system32\pwrshplugin.dll
SDKVersion       : 1
XmlRenderingType : text
lang             : en-US
PSVersion        : 2.0
startupscript    : c:\ps-test\Maintenance.ps1
ResourceUri      : http://schemas.microsoft.com/powershell/MaintenanceShell
SupportsOptions  : true
ExactMatch       : true
Capability       : {Shell}
Permission       :

The third command uses the WSMan provider to view the initialization parameters for the MaintenanceShell configuration. The command uses the Get-ChildItem cmdlet (alias = dir) to get the child items in the InitializationParameters node for the MaintenanceShell plug-in.For more information about the WSMan provider, type "get-help wsman".
PS C:\>dir WSMan:\localhost\Plugin\MaintenanceShell\InitializationParameters
ParamName     ParamValue
---------     ----------
PSVersion     2.0
startupscript c:\ps-test\Maintenance.ps1


```
This example shows different ways of viewing the results of a Set-PSSessionConfiguration command.



### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289613)
[Disable-PSSessionConfiguration]()
[Enable-PSSessionConfiguration]()
[Get-PSSessionConfiguration]()
[New-PSSessionConfigurationFile]()
[New-PSSessionOption]()
[New-PSTransportOption]()
[New-PSWorkflowExecutionOption]()
[Register-PSSessionConfiguration]()
[Set-PSSessionConfiguration]()
[Test-PSSessionConfigurationFile]()
[Unregister-PSSessionConfiguration]()
[WSMan Provider]()
[about_Session_Configurations]()
[about_Session_Configuration_Files]()

## Set-StrictMode

### SYNOPSIS
Establishes and enforces coding rules in expressions, scripts, and script blocks.  

### DESCRIPTION
The Set-StrictMode cmdlet configures strict mode for the current scope (and all child scopes) and turns it on and off. When strict mode is on, Windows PowerShell generates a terminating error when the content of an expression, script, or script block violates basic best-practice coding rules.
Use the Version parameter to determine which coding rules are enforced.
Unlike the Set-PSDebug cmdlet, Set-StrictMode affects only the current scope and its child scopes, so you can use it in a script or function without affecting the global scope.
When Set-StrictMode is off, uninitialized variables (Version 1) are assumed to have a value of 0 (zero) or $null, depending on type. References to non-existent properties return $null, and the results of function syntax that is not valid vary with the error. Unnamed variables are not permitted.

### PARAMETERS

#### Off [switch]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 2')]
```

Turns strict mode off. This parameter also turns off "Set-PSDebug -Strict".

#### Version [Version]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 1')]
```

Specifies the conditions that cause an error in strict mode. This parameter is required.
The valid values are "1.0", "2.0", and "Latest". The following list shows the effect of each value.
1.0
-- Prohibits references to uninitialized variables, except for uninitialized variables in strings.
2.0
-- Prohibits references to uninitialized variables (including uninitialized variables in strings).
-- Prohibits references to non-existent properties of an object.
-- Prohibits function calls that use the syntax for calling methods.
-- Prohibits a variable without a name (${}).
Latest:
--Selects the latest (most strict) version available.  Use this value to assure that scripts use the strictest available version, even when new versions are added to Windows PowerShell.


### INPUTS
#### None
You cannot pipe input to this cmdlet.

### OUTPUTS
#### None
This cmdlet does not return any output.

### NOTES
Set-StrictMode is similar to the Strict parameter of Set-PSDebug. "Set-Strictmode -version 1" is equivalent to "Set-PSDebug -strict", except that Set-PSDebug is effective in all scopes. Set-StrictMode is effective only in the scope in which it is set and in its child scopes. For more information about scopes in Windows PowerShell, see about_Scopes.


### EXAMPLES
#### -------------------------- EXAMPLE 1 --------------------------

```powershell
PS C:\>set-strictmode -version 1.0
PS C:\>$a -gt 5
False
The variable $a cannot be retrieved because it has not been set yet.
At line:1 char:3
+ $a <<<<  -gt 5
+ CategoryInfo          : InvalidOperation: (a:Token) [], RuntimeException
+ FullyQualifiedErrorId : VariableIsUndefined

```
This command turns strict mode on and sets it to version 1.0. As a result, attempts to reference variables that are not initialized will fail.
The sample output shows the effect of version 1.0 strict mode.






#### -------------------------- EXAMPLE 2 --------------------------

```powershell
PS C:\># set-strictmode -version 2.0
# Strict mode is off by default.

PS C:\>function add ($a, $b) {$a + $b}
PS C:\>add 3 4
7
PS C:\>add(3,4)
3
4
PS C:\>set-strictmode -version 2.0
PS C:\>add(3,4)

The function or command was called like a method. Parameters should be separated by spaces, as described in 'Get-Help about_Parameter.'
At line:1 char:4
+ add <<<< (3,4)
+ CategoryInfo          : InvalidOperation: (:) [], RuntimeException
+ FullyQualifiedErrorId : StrictModeFunctionCallWithParens

PS C:\>set-strictmode -off
PS C:\>$string = "This is a string."
PS C:\>$string.Month
PS C:\>
PS C:\>set-strictmode -version 2.0
PS C:\>$string = "This is a string."
PS C:\>$string.Month

Property 'month' cannot be found on this object; make sure it exists.
At line:1 char:9
+ $string. <<<< month
+ CategoryInfo          : InvalidOperation: (.:OperatorToken) [], RuntimeException
+ FullyQualifiedErrorId : PropertyNotFoundStrict

```
This command turns strict mode on and sets it to version 2.0. As a result, Windows PowerShell throws an error if you use method syntax (parentheses and commas) for a function call or reference uninitialized variables or non-existent properties.
The sample output shows the effect of version 2.0 strict mode.
Without version 2.0 strict mode, the "(3,4)" value is interpreted as a single array object to which nothing is added. With version 2.0 strict mode, it is correctly interpreted as faulty syntax for submitting two values.
Without version 2.0, the reference to the non-existent Month property of a string returns only null. With version 2.0, it is interpreted correctly as a reference error.







### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289614)
[Set-PSDebug]()
[about_Scopes]()
[about_Debuggers]()

## Start-Job

### SYNOPSIS
Starts a Windows PowerShell background job.

### DESCRIPTION
The Start-Job cmdlet starts a Windows PowerShell background job on the local computer.
A Windows PowerShell background job runs a command "in the background" without interacting with the current session. When you start a background job, a job object is returned immediately, even if the job takes an extended time to complete. You can continue to work in the session without interruption while the job runs.
The job object contains useful information about the job, but it does not contain the job results. When the job completes, use the Receive-Job cmdlet to get the results of the job. For more information about background jobs, see about_Jobs.
To run a background job on a remote computer, use the AsJob parameter that is available on many cmdlets, or use the Invoke-Command cmdlet to run a Start-Job command on the remote computer.  For more information, see about_Remote_Jobs.
Beginning in Windows PowerShell 3.0, Start-Job can start instances of custom job types, such as scheduled jobs. For information about using Start-Job to start jobs with custom types, see the help topics for the job type feature.

### PARAMETERS

#### ArgumentList [Object[]]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
[Parameter(ParameterSetName = 'Set 2')]
[Parameter(ParameterSetName = 'Set 3')]
```

Specifies the arguments (parameter values) for the script that is specified by the FilePath parameter.
Because all of the values that follow the ArgumentList parameter name are interpreted as being values of ArgumentList, the ArgumentList parameter should be the last parameter in the command.

#### Authentication [AuthenticationMechanism]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
[Parameter(ParameterSetName = 'Set 2')]
[Parameter(ParameterSetName = 'Set 3')]
```

Specifies the mechanism that is used to authenticate the user's credentials.   Valid values are Default, Basic, Credssp, Digest, Kerberos, Negotiate, and NegotiateWithImplicitCredential.  The default value is Default.
CredSSP authentication is available only in Windows Vista, Windows Server 2008, and later versions of Windows.
For information about the values of this parameter, see the description of the System.Management.Automation.Runspaces.AuthenticationMechanism enumeration in MSDN.
CAUTION: Credential Security Support Provider (CredSSP) authentication, in which the user's credentials are passed to a remote computer to be authenticated, is designed for commands that require authentication on more than one resource, such as accessing a remote network share. This mechanism increases the security risk of the remote operation. If the remote computer is compromised, the credentials that are passed to it can be used to control the network session.

#### Credential [PSCredential]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
[Parameter(ParameterSetName = 'Set 2')]
[Parameter(ParameterSetName = 'Set 3')]
```

Specifies a user account that has permission to perform this action. The default is the current user.
Type a user name, such as "User01" or "Domain01\User01", or enter a PSCredential object, such as one from the Get-Credential cmdlet.

#### FilePath [String]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 2')]
```

Runs the specified local script as a background job. Enter the path and file name of the script or pipe a script path to Start-Job. The script must reside on the local computer or in a directory that the local computer can access.
When you use this parameter, Windows PowerShell converts the contents of the specified script file to a script block and runs the script block as a background job.

#### InitializationScript [ScriptBlock]

```powershell
[Parameter(
  Position = 2,
  ParameterSetName = 'Set 1')]
[Parameter(
  Position = 2,
  ParameterSetName = 'Set 2')]
[Parameter(
  Position = 2,
  ParameterSetName = 'Set 3')]
```

Specifies commands that run before the job starts. Enclose the commands in braces ( { } ) to create a script block.
Use this parameter to prepare the session in which the job runs. For example, you can use it to add functions, snap-ins, and modules to the session.

#### InputObject [PSObject]

```powershell
[Parameter(
  ValueFromPipeline = $true,
  ParameterSetName = 'Set 1')]
[Parameter(
  ValueFromPipeline = $true,
  ParameterSetName = 'Set 2')]
[Parameter(
  ValueFromPipeline = $true,
  ParameterSetName = 'Set 3')]
```

Specifies input to the command. Enter a variable that contains the objects, or type a command or expression that generates the objects.
In the value of the ScriptBlock parameter, use the $input automatic variable to represent the input objects.

#### LiteralPath [String]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 3')]
```

Runs the specified local script as a background job. Enter the path to a script on the local computer.
Unlike the FilePath parameter, the value of LiteralPath is used exactly as it is typed. No characters are interpreted as wildcards. If the path includes escape characters, enclose it in single quotation marks. Single quotation marks tell Windows PowerShell not to interpret any characters as escape sequences.

#### Name [String]

```powershell
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 1')]
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 2')]
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 3')]
```

Specifies a friendly name for the new job. You can use the name to identify the job to other job cmdlets, such as Stop-Job.
The default friendly name is Job#, where "#" is an ordinal number that is incremented for each job.

#### PSVersion [Version]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
[Parameter(ParameterSetName = 'Set 2')]
[Parameter(ParameterSetName = 'Set 3')]
```

Runs the job with the specified version of Windows PowerShell. Valid values are 2.0 and 3.0.
This parameter is introduced in Windows PowerShell 3.0.

#### RunAs32 [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
[Parameter(ParameterSetName = 'Set 2')]
[Parameter(ParameterSetName = 'Set 3')]
```

Runs the job in a 32-bit process.  Use this parameter to force the job to run in a 32-bit process on a 64-bit operating system.
NOTE: On 64-bit versions of Windows 7 and Windows Server 2008 R2, when the Start-Job command includes the RunAs32 parameter, you cannot use the Credential parameter to specify the credentials of another user.

#### ScriptBlock [ScriptBlock]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 1')]
```

Specifies the commands to run in the background job. Enclose the commands in braces ( { } ) to create a script block. This parameter is required.

#### DefinitionName [String]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 4')]
```

Starts the job with the specified job definition name. Use this parameter to start custom job types that have a definition name, such as scheduled jobs.
When you use Start-Job to start an instance of a scheduled job, the job starts immediately, regardless of job triggers or job options. The resulting job instance is a scheduled job, but it is not saved to disk like triggered scheduled jobs. Also, you cannot use the ArgumentList parameter of Start-Job to provide values for parameters of scripts that run in a scheduled job. For more information, see about_Scheduled_Jobs.

This parameter is introduced in Windows PowerShell 3.0.

#### DefinitionPath [String]

```powershell
[Parameter(
  Position = 2,
  ParameterSetName = 'Set 4')]
```

Starts the job at the specified path location. Enter the definition path. The concatenation of the values of the DefinitionPath and DefinitionName parameters should be the fully-qualified path to the job definition. Use this parameter to start custom job types that have a definition path, such as scheduled jobs.
For scheduled jobs, the value of the DefinitionPath parameter is "$home\AppData\Local\Windows\PowerShell\ScheduledJob".
This parameter is introduced in Windows PowerShell 3.0.

#### Type [String]

```powershell
[Parameter(
  Position = 3,
  ParameterSetName = 'Set 4')]
```

Starts only jobs of the specified custom type. Enter a custom job type name, such as PSScheduledJob for scheduled jobs or PSWorkflowJob for workflows jobs. This parameter is not valid for standard background jobs.
This parameter is introduced in Windows PowerShell 3.0.


### INPUTS
#### System.String
You can pipe an object with the Name property to the Name parameter. For example, you can pipe a FileInfo object from Get-ChildItem to Start-Job.

### OUTPUTS
#### System.Management.Automation.PSRemotingJob
Start-Job returns an object that represents the job that it started.

### NOTES
To run in the background, Start-Job runs in its own session within the current session. When you use the Invoke-Command cmdlet to run a Start-Job command in a session on a remote computer, Start-Job runs in a session within the remote session.


### EXAMPLES
#### -------------------------- EXAMPLE 1 --------------------------

```powershell
PS C:\>start-job -scriptblock {get-process}

Id    Name  State    HasMoreData  Location   Command
---   ----  -----    -----------  --------   -------
1     Job1  Running  True         localhost  get-process

```
This command starts a background job that runs a Get-Process command. The command returns a job object with information about the job. The command prompt returns immediately so that you can work in the session while the job runs in the background.






#### -------------------------- EXAMPLE 2 --------------------------

```powershell
PS C:\>$jobWRM = invoke-command -computerName (get-content servers.txt) -scriptblock {get-service winrm} -jobname WinRM -throttlelimit 16 -AsJob

```
This command uses the Invoke-Command cmdlet and its AsJob parameter to start a background job that runs a "get-service winrm" command on numerous computers. Because the command is running on a server with substantial network traffic, the command uses the ThrottleLimit parameter of Invoke-Command to limit the number of concurrent commands to 16.
The command uses the ComputerName parameter to specify the computers on which the job runs. The value of the ComputerName parameter is a Get-Content command that gets the text in the Servers.txt file, a file of computer names in a domain.
The command uses the ScriptBlock parameter to specify the command and the JobName parameter to specify a friendly name for the job.






#### -------------------------- EXAMPLE 3 --------------------------

```powershell
PS C:\>$j = start-job -scriptblock {get-eventlog -log system} -credential domain01\user01
PS C:\>$j | format-list -property *

HasMoreData   : True
StatusMessage :
Location      : localhost
Command       : get-eventlog -log system
JobStateInfo  : Running
Finished      : System.Threading.ManualResetEvent
InstanceId    : 2d9d775f-63e0-4d48-b4bc-c05d0e177f34
Id            : 1
Name          : Job1
ChildJobs     : {Job2}
Output        : {}
Error         : {}
Progress      : {}
Verbose       : {}
Debug         : {}
Warning       : {}
StateChanged  :

PS C:\>$j.JobStateInfo.state
Completed
PS C:\>$results = receive-job -job $j
PS C:\>$results

Index Time          Type        Source                EventID Message
----- ----          ----        ------                ------- -------
84366 Feb 18 19:20  Information Service Control M...     7036 The description...
84365 Feb 18 19:16  Information Service Control M...     7036 The description...
84364 Feb 18 19:10  Information Service Control M...     7036 The description...
...

```
These commands manage a background job that gets all of the events from the System log in Event Viewer. The job runs on the local computer.
The first command uses the Start-Job cmdlet to start the job. It uses the Credential parameter to specify the user account of a user who has permission to run the job on the computer. Then it saves the job object that Start-Job returns in the $j variable.
At this point, you can resume your other work while the job completes.
The second command uses a pipeline operator (|) to pass the job object in $j to the Format-List cmdlet. The Format-List command uses the Property parameter with a value of all (*) to display all of the properties of the job object in a list.
The third command displays the value of the JobStateInfo property. This contains the status of the job.
The fourth command uses the Receive-Job cmdlet to get the results of the job. It stores the results in the $results variable.
The final command displays the contents of the $results variable.






#### -------------------------- EXAMPLE 4 --------------------------

```powershell
PS C:\>start-job -filepath c:\scripts\sample.ps1

```
This command runs the Sample.ps1 script as a background job.






#### -------------------------- EXAMPLE 5 --------------------------

```powershell
PS C:\>start-job -name WinRm -scriptblock {get-process winrm}

```
This command runs a background job that gets the WinRM process on the local computer. The command uses the ScriptBlock parameter to specify the command that runs in the background job. It uses the Name parameter to specify a friendly name for the new job.






#### -------------------------- EXAMPLE 6 --------------------------

```powershell
PS C:\>start-job -name GetMappingFiles -initializationScript {import-module MapFunctions} -scriptblock {Get-Map -name * | set-content D:\Maps.tif} -runAs32

```
This command starts a job that collects a large amount of data and saves it in a .tif file. The command uses the InitializationScript parameter to run a script block that imports a required module. It also uses the RunAs32 parameter to run the job in a 32-bit process even if the computer has a 64-bit operating system.







### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289615)
[Get-Job]()
[Invoke-Command]()
[Receive-Job]()
[Remove-Job]()
[Resume-Job]()
[Start-Job]()
[Stop-Job]()
[Suspend-Job]()
[Wait-Job]()
[about_Job_Details]()
[about_Remote_Jobs]()
[about_Jobs]()

## Stop-Job

### SYNOPSIS
Stops a Windows PowerShell background job.

### DESCRIPTION
The Stop-Job cmdlet stops Windows PowerShell background jobs that are in progress. You can use this cmdlet to stop all jobs or stop selected jobs based on their name, ID, instance ID, or state, or by passing a job object to Stop-Job.
You can use Stop-Job to stop background jobs, such as those that were started by using the Start-Job cmdlet or the AsJob parameter of any cmdlet. When you stop a background job, Windows PowerShell completes all tasks that are pending in that job queue and then ends the job. No new tasks are added to the queue after this command is submitted.
This cmdlet does not delete background jobs. To delete a job, use the Remove-Job cmdlet.
Beginning in Windows PowerShell 3.0, Stop-Job also stops custom job types, such as workflow jobs and instances of scheduled jobs. To enable Stop-Job to stop a job with custom job type, import the module that supports the custom job type into the session before running a Stop-Job command, either by using the Import-Module cmdlet or by using or getting a cmdlet in the module. For information about a particular custom job type, see the documentation of the custom job type feature.

### PARAMETERS

#### Filter [Hashtable]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 2')]
```

Stops jobs that satisfy all of the conditions established in the associated hash table. Enter a hash table where the keys are job properties and the values are job property values.
This parameter works only on custom job types, such as workflow jobs and scheduled jobs. It does not work on standard background jobs, such as those created by using the Start-Job cmdlet. For information about support for this parameter, see the help topic for the job type.
This parameter is introduced in Windows PowerShell 3.0.

#### Id [Int32[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 1')]
```

Stops jobs with the specified IDs. The default is all jobs in the current session.
The ID is an integer that uniquely identifies the job within the current session. It is easier to remember and type than the InstanceId, but it is unique only within the current session. You can type one or more IDs (separated by commas). To find the ID of a job, type "Get-Job" without parameters.

#### InstanceId [Guid[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 3')]
```

Stops only jobs with the specified instance IDs. The default is all jobs.
An instance ID is a GUID that uniquely identifies the job on the computer. To find the instance ID of a job, use Get-Job.

#### Job [Job[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 4')]
```

Specifies the jobs to be stopped. Enter a variable that contains the jobs or a command that gets the jobs. You can also use a pipeline operator to submit jobs to the Stop-Job cmdlet. By default, Stop-Job deletes all jobs that were started in the current session.

#### Name [String[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 5')]
```

Stops only the jobs with the specified friendly names. Enter the job names in a comma-separated list or use wildcard characters (*) to enter a job name pattern. By default, Stop-Job stops all jobs created in the current session.
Because the friendly name is not guaranteed to be unique, use the WhatIf and Confirm parameters when stopping jobs by name.

#### PassThru [switch]

Returns an object representing the new background job. By default, this cmdlet does not generate any output.

#### State [JobState]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 6')]
```

Stops only jobs in the specified state. Valid values are NotStarted, Running, Completed, Failed, Stopped, Blocked, Suspended, Disconnected, Suspending, Stopping.
For more information about job states, see "JobState Enumeration" in MSDN at [http://msdn.microsoft.com/en-us/library/windows/desktop/system.management.automation.jobstate(v=vs.85).aspx]()

#### Confirm [switch]

Prompts you for confirmation before running the cmdlet.Prompts you for confirmation before running the cmdlet.

#### WhatIf [switch]

Shows what would happen if the cmdlet runs. The cmdlet is not run.Shows what would happen if the cmdlet runs. The cmdlet is not run.


### INPUTS
#### System.Management.Automation.RemotingJob
You can pipe a job object to Stop-Job.

### OUTPUTS
#### None or System.Management.Automation.PSRemotingJob
When you use the PassThru parameter, Stop-Job returns a job object. Otherwise, this cmdlet does not generate any output.

### NOTES

### EXAMPLES
#### -------------------------- EXAMPLE 1 --------------------------

```powershell
PS C:\>$s = New-PSSession -ComputerName Server01 -Credential Domain01\Admin02
PS C:\>$j = Invoke-Command -Session $s -ScriptBlock {Start-Job -ScriptBlock {Get-EventLog System}}
PS C:\>Invoke-Command -Session $s -ScriptBlock { Stop-job -Job $Using:j }

```
This example shows how to use the Stop-Job cmdlet to stop a job that is running on a remote computer.
Because the job was started by using the Invoke-Command cmdlet to run a Start-Job command remotely, the job object is stored on the remote computer, and you must use another Invoke-Command command to run a Stop-Job command remotely. For more information about remote background jobs, see about_Remote_Jobs.
The first command creates a Windows PowerShell session (PSSession) on the Server01 computer and saves the session object in the $s variable. The command uses the credentials of a domain administrator.
The second command uses the Invoke-Command cmdlet to run a Start-Job command in the session. The command in the job gets all of the events in the System event log. The resulting job object is stored in the $j variable.
The third command stops the job. It uses the Invoke-Command cmdlet to run a Stop-Job command in the PSSession on Server01. Because the job objects are stored in $j, which is a variable on the local computer, the command uses the Using scope modifier to identify $j as a local variable. For more information about the Using scope modifier, see about_Remote_Variables (http://go.microsoft.com/fwlink/?LinkID=252653).
When the command completes, the job is stopped and the PSSession in $s is available for use.


#### -------------------------- EXAMPLE 2 --------------------------

```powershell
PS C:\>Stop-Job -Name Job1

```
This command stops the Job1 background job.


#### -------------------------- EXAMPLE 3 --------------------------

```powershell
PS C:\>Stop-Job -ID 1, 3, 4

```
This command stops three jobs. It identifies them by their IDs.


#### -------------------------- EXAMPLE 4 --------------------------

```powershell
PS C:\>Get-Job | Stop-Job

```
This command stops all of the background jobs in the current session.


#### -------------------------- EXAMPLE 5 --------------------------

```powershell
PS C:\>Stop-Job -State Blocked

```
This command stops all the jobs that are blocked.


#### -------------------------- EXAMPLE 6 --------------------------

```powershell
PS C:\>Get-Job | Format-Table ID, Name, Command, @{Label="State";Expression={$_.JobStateInfo.State}},
InstanceID -Auto

Id Name Command                 State  InstanceId
-- ---- -------                 -----  ----------
1 Job1 start-service schedule Running 05abb67a-2932-4bd5-b331-c0254b8d9146
3 Job3 start-service schedule Running c03cbd45-19f3-4558-ba94-ebe41b68ad03
5 Job5 get-service s*         Blocked e3bbfed1-9c53-401a-a2c3-a8db34336adf

PS C:\>Stop-Job -InstanceId e3bbfed1-9c53-401a-a2c3-a8db34336adf

```
These commands show how to stop a job based on its instance ID.
The first command uses the Get-Job cmdlet to get the jobs in the current session. The command uses a pipeline operator (|) to send the jobs to a Format-Table command, which displays a table of the specified properties of each job. The table includes the Instance ID of each job. It uses a calculated property to display the job state.
The second command uses a Stop-Job command with the InstanceID parameter to stop a selected job.


#### -------------------------- EXAMPLE 7 --------------------------

```powershell
PS C:\>$j = Invoke-Command -ComputerName Server01 -ScriptBlock {Get-EventLog System} -AsJob
PS C:\>$j | Stop-Job -PassThru

Id    Name    State      HasMoreData     Location         Command
--    ----    ----      -----------     --------          -------
5     Job5    Stopped    True            user01-tablet   get-eventlog system

```
This example shows how to use the Stop-Job cmdlet to stop a job that is running on a remote computer.
Because the job was started by using the AsJob parameter of the Invoke-Command cmdlet, the job object is located on the local computer, even though the job runs on the remote computer. As such, you can use a local Stop-Job command to stop the job.
The first command uses the Invoke-Command cmdlet to start a background job on the Server01 computer. The command uses the AsJob parameter to run the remote command as a background job.
This command returns a job object, which is the same job object that the Start-Job cmdlet returns. The command saves the job object in the $j variable.
The second command uses a pipeline operator to send the job in the $j variable to Stop-Job. The command uses the PassThru parameter to direct Stop-Job to return a job object. The job object display confirms that the State of the job is "Stopped".
For more information about remote background jobs, see about_Remote_Jobs.



### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289616)
[Get-Job]()
[Invoke-Command]()
[Receive-Job]()
[Remove-Job]()
[Resume-Job]()
[Start-Job]()
[Stop-Job]()
[Suspend-Job]()
[Wait-Job]()
[about_Job_Details]()
[about_Remote_Jobs]()
[about_Remote_Variables]()
[about_Jobs]()
[about_Scopes]()

## Suspend-Job

### SYNOPSIS
Temporarily stops workflow jobs.

### DESCRIPTION
The Suspend-Job cmdlet suspends (temporarily interrupts or pauses) workflow jobs. This cmdlet allows users who are running workflows to suspend the workflow. It complements the [Suspend-Workflow]() activity, which is a command in the workflow that suspends the workflow.
The Suspend-Job cmdlet works only on workflow jobs. It does not work on standard background jobs, such as those that are started by using the Start-Job cmdlet. 
To identify a workflow job, look for a value of PSWorkflowJob in the PSJobTypeName property of the job. To determine whether a particular custom job type supports the Suspend-Job cmdlet, see the help topics for the custom job type.
When you suspend a workflow job, the workflow job runs to the next checkpoint, suspends, and immediately returns a workflow job object. To wait for the suspension to complete before getting the job, use the Wait parameter of Suspend-Job or the Wait-Job cmdlet. When the workflow job is suspended, the value of the State property of the job is Suspended.
Suspending correctly relies on checkpoints. The current job state, metadata, and output are saved in the checkpoint so the workflow job can be resumed without any loss of state or data. If the workflow job does not have checkpoints, it cannot be suspended properly. To add checkpoints to a workflow that you are running, use the PSPersist workflow common parameter. You can use the Force parameter to suspend any workflow job immediately and to suspend a workflow job that does not have checkpoints, but the action might cause loss of state and data.
NOTE: Before using a Job cmdlet on a custom job type, such as a workflow job (PSWorkflowJob) import the module that supports the custom job type, either by using the Import-Module cmdlet or using or using a cmdlet in the module.
This cmdlet is introduced in Windows PowerShell 3.0.

### PARAMETERS

#### Filter [Hashtable]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 2')]
```

Suspends only workflow jobs that satisfy all of the conditions established in the associated hash table. Enter a hash table where the keys are workflow job properties and the values are workflow job property values.

#### Force [switch]

Suspends the workflow job immediately. This action might cause a loss of state and data.
By default, Suspend-Job lets the workflow job run until the next checkpoint and then suspends it. You can also use this parameter to suspend workflow jobs that do not have checkpoints.

#### Id [Int32[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 1')]
```

Suspends the workflow jobs with the specified IDs.
The ID is an integer that uniquely identifies the job within the current session. It is easier to remember and to type than the instance ID, but it is unique only within the current session. You can type one or more IDs (separated by commas). To find the ID of a job, use the Get-Job cmdlet.

#### InstanceId [Guid[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 4')]
```

Suspends workflow jobs with the specified instance IDs. The default is all jobs.
An instance ID is a GUID that uniquely identifies the job on the computer. To find the instance ID of a job, use the Get-Job cmdlet.

#### Job [Job[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 5')]
```

Specifies the workflow jobs to be suspended. Enter a variable that contains the workflow jobs or a command that gets the workflow jobs. You can also pipe workflow jobs to the Suspend-Job cmdlet.

#### Name [String[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 6')]
```

Suspends workflow jobs with the specified friendly names. Enter one or more workflow job names. Wildcards are supported.

#### State [JobState]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 3')]
```

Suspends only those workflow jobs in the specified state. Valid values are NotStarted, Running, Completed, Failed, Stopped, Blocked, Suspended, Disconnected, Suspending, Stopping but Suspend-Job suspends only workflow jobs in the Running state.
For more information about job states, see "JobState Enumeration" in MSDN at [http://msdn.microsoft.com/en-us/library/windows/desktop/system.management.automation.jobstate(v=vs.85).aspx]()

#### Wait [switch]

Returns only after the workflow job is in the suspended state. By default, Suspend-Job suspends immediately, even if the workflow job is not yet in the suspended state.
The Wait parameter is equivalent to piping a Suspend-Job command to the Wait-Job cmdlet.

#### Confirm [switch]

Prompts you for confirmation before running the cmdlet.Prompts you for confirmation before running the cmdlet.

#### WhatIf [switch]

Shows what would happen if the cmdlet runs. The cmdlet is not run.Shows what would happen if the cmdlet runs. The cmdlet is not run.


### INPUTS
#### System.Management.Automation.Job
You can pipe all types of jobs to Suspend-Job. However, if Suspend-Job gets a job of an unsupported type, it throws a terminating error.

### OUTPUTS
#### System.Management.Automation.Job
Suspend-Job returns the jobs that it suspended.

### NOTES
The mechanism and location for saving a suspended job might vary depending on the job type. For example, suspended workflow jobs are saved in a flat file store by default, but can also be saved in a database.
If you submit a workflow job that is not in the Running state, Suspend-Job displays a warning message. To suppress the warning, use the WarningAction common parameter with a value of SilentlyContinue.
If a job is not of a type that supports suspending, Suspend-Job throw a terminating error.
To find the workflow jobs that are suspended, including those that were suspended by this cmdlet, use the State parameter of the Get-Job cmdlet to get workflow jobs in the Suspended state.
Some job types have options or properties that prevent Windows PowerShell from suspending the job. If attempts to suspend the job fail, verify that the job options and properties allow suspending.

### EXAMPLES
#### Example 1: Suspend a workflow job by name

```powershell
The first command creates the Get-SystemLog workflow. The workflow uses the CheckPoint-Workflow activity to define a checkpoint in the workflow.
#Sample Workflow                  
Workflow Get-SystemLog
{
    $Events = Get-WinEvent -LogName System
    CheckPoint-Workflow
    InlineScript {\\Server01\Scripts\Analyze-SystemEvents.ps1 -Events $Events}
}

The second command uses the AsJob parameter that is common to all workflows to run the Get-SystemLog workflow as a background job. The command uses the JobName workflow common parameter to specify a friendly name for the workflow job.
PS C:\>Get-SystemLog -AsJob -JobName Get-SystemLogJob

The third command uses the Get-Job cmdlet to get the Get-SystemLogJob workflow job. The output shows that the value of the PSJobTypeName property is PSWorkflowJob.
PS C:\>Get-Job -Name Get-SystemLogJob
Id     Name              PSJobTypeName   State       HasMoreData     Location   Command
--     ----              -------------   -----       -----------     --------   -------
4      Get-SystemLogJob  PSWorkflowJob   Running     True            localhost   Get-SystemLog

The fourth command uses the Suspend-Job cmdlet to suspend the Get-SystemLogJob job. The job runs to the checkpoint and then suspends.
PS C:\>Suspend-Job -Name Get-SystemLogJob
Id     Name              PSJobTypeName   State       HasMoreData     Location   Command
--     ----              -------------   -----       -----------     --------   -------
4      Get-SystemLogJob  PSWorkflowJob   Suspended   True            localhost   Get-SystemLog

```
This example shows how to suspend a workflow job.


#### Example 2: Suspend and resume a workflow job

```powershell
The first command suspends the LogWorkflowJob job.The command returns immediately. The output shows that the workflow job is still running, even though it is in the process of being suspended..
PS C:\>Suspend-Job -Name LogWorkflowJob
Id     Name          PSJobTypeName      State         HasMoreData     Location             Command
--     ----          -------------      -----         -----------     --------             -------
67     LogflowJob    PSWorkflowJob      Running       True            localhost            LogWorkflow

The second command uses the Get-Job cmdlet to get the LogWorkflowJob job. The output shows that the workflow job suspended successfully.
PS C:\>Get-Job -Name LogWorkflowJob
Id     Name          PSJobTypeName      State         HasMoreData     Location             Command
--     ----          -------------      -----         -----------     --------             -------
67     LogflowJob    PSWorkflowJob      Suspended     True            localhost            LogWorkflow

The third command uses the Get-Job cmdlet to get the LogWorkflowJob job and the Resume-Job cmdlet to resume it. The output shows that the workflow job resumed successfully and is now running.
PS C:\>Get-Job -Name LogWorkflowJob | Resume-Job
Id     Name          PSJobTypeName      State         HasMoreData     Location             Command
--     ----          -------------      -----         -----------     --------             -------
67     LogflowJob    PSWorkflowJob      Running       True            localhost            LogWorkflow

```
This example shows how to suspend and resume a workflow job.


#### Example 3: Suspend a workflow job on a remote computer

```powershell
PS C:\>Invoke-Command -ComputerName Srv01 -Scriptblock {Suspend-Job -Filter @{CustomID="031589"}

```
This command uses the Invoke-Command cmdlet to suspend a workflow job on the Srv01 remote computer. The value of the Filters parameter is a hash table that specifies a CustomID value. This CustomID is job metadata (PSPrivateMetadata).


#### Example 4: Wait for the workflow job to suspend

```powershell
PS C:\>Suspend-Job VersionCheck -Wait
Id     Name          PSJobTypeName      State         HasMoreData     Location             Command
--     ----          -------------      -----         -----------     --------             -------
 5     VersionCheck  PSWorkflowJob      Suspended     True            localhost            LogWorkflow

```
This command suspends the VersionCheck workflow job. The command uses the Wait parameter to wait until the workflow job is suspended. When the workflow job runs to the next checkpoint and is suspended, the command completes and returns the job object.


#### Example 5: Force a workflow job to suspend

```powershell
PS C:\>Suspend-Job Maintenance -Force

```
This command suspends the Maintenance workflow job forcibly. The Maintenance job does not have checkpoints, so it cannot be suspended correctly and might not resume properly.



### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289617)
[Get-Job]()
[Receive-Job]()
[Remove-Job]()
[Resume-Job]()
[Start-Job]()
[Stop-Job]()
[Suspend-Job]()
[Wait-Job]()

## Test-ModuleManifest

### SYNOPSIS
Verifies that a module manifest file accurately describes the contents of a module.

### DESCRIPTION
The Test-ModuleManifest cmdlet verifies that the files that are listed in the module manifest (.psd1) file actually exist in the specified paths.
This cmdlet is designed to help module authors test their manifest files. Module users can also use this cmdlet in scripts and commands to detect errors before running scripts that depend on the module.
The Test-ModuleManifest cmdlet returns an object that represents the module (the same type of object that Get-Module returns). If any files are not in the locations specified in the manifest, the cmdlet also generates an error for each missing file.

### PARAMETERS

#### Path [String]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 1')]
```

Specifies the path to the module manifest file.  Enter a path (optional) and the name of the module manifest file with the .psd1 file name extension. The default location is the current directory. Wildcards are supported, but must resolve to a single module manifest file. This parameter is required. The parameter name ("Path") is optional. You can also pipe a path to Test-ModuleManifest.


### INPUTS
#### System.String
You can pipe the path to a module manifest to Test-ModuleManifest.

### OUTPUTS
#### System.Management.Automation.PSModuleInfo
Test-ModuleManifest returns a PSModuleInfo object that represents the module. It returns this object even if the manifest has errors.

### NOTES

### EXAMPLES
#### -------------------------- EXAMPLE 1 --------------------------

```powershell
PS C:\>test-ModuleManifest -path $pshome\Modules\TestModule.psd1

```
This command tests the TestModule.psd1 module manifest.






#### -------------------------- EXAMPLE 2 --------------------------

```powershell
PS C:\>"$pshome\Modules\TestModule.psd1" | test-modulemanifest

Test-ModuleManifest : The specified type data file 'C:\Windows\System32\Wi
ndowsPowerShell\v1.0\Modules\TestModule\TestTypes.ps1xml' could not be processed because the file was not found. Please correct the path and try again.
At line:1 char:34
+ "$pshome\Modules\TestModule.psd1" | test-modulemanifest <<<<
+ CategoryInfo          : ResourceUnavailable: (C:\Windows\System32\WindowsPowerShell\v1.0\Modules\TestModule\TestTypes.ps1xml:String) [Test-ModuleManifest], FileNotFoundException
+ FullyQualifiedErrorId : Modules_TypeDataFileNotFound,Microsoft.PowerShell.Commands.TestModuleManifestCommandName

Name              : TestModule
Path              : C:\Windows\system32\WindowsPowerShell\v1.0\Modules\TestModule\TestModule.psd1
Description       :
Guid              : 6f0f1387-cd25-4902-b7b4-22cff6aefa7b
Version           : 1.0
ModuleBase        : C:\Windows\system32\WindowsPowerShell\v1.0\Modules\TestModule
ModuleType        : Manifest
PrivateData       :
AccessMode        : ReadWrite
ExportedAliases   : {}
ExportedCmdlets   : {}
ExportedFunctions : {}
ExportedVariables : {}
NestedModules     : {}

```
This command uses a pipeline operator (|) to send a path string to Test-ModuleManifest.
The command output shows that the test failed, because the TestTypes.ps1xml file, which was listed in the manifest, was not found.






#### -------------------------- EXAMPLE 3 --------------------------

```powershell
PS C:\>function Test-ManifestBool ($path)
{$a = dir $path | test-modulemanifest -erroraction SilentlyContinue; $?}

```
This function is like Test-ModuleManifest, but it returns a Boolean value;  it returns "True" if the manifest passed the test and "False" otherwise.
The function uses the Get-ChildItem cmdlet (alias = dir) to get the module manifest specified by the $path variable. It uses a pipeline operator (|) to pass the file object to the Test-ModuleManifest cmdlet.
The Test-ModuleManifest command uses the ErrorAction common parameter with a value of SilentlyContinue to suppress the display of any errors that the command generates. It also saves the PSModuleInfo object that Test-ModuleManifest returns in the $a variable, so the object is not displayed.
Then, in a separate command (the semi-colon [;] is the command separator), it displays the value of the $? automatic variable, which returns "True" if the previous command generated no error and "False" otherwise.
You can use this function in conditional statements, such as those that might precede an Import-Module command or a command that uses the module.







### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289618)
[Export-ModuleMember]()
[Get-Module]()
[Import-Module]()
[New-Module]()
[New-ModuleManifest]()
[Remove-Module]()
[about_Modules]()

## Test-PSSessionConfigurationFile

### SYNOPSIS
Verifies the keys and values in a session configuration file.

### DESCRIPTION
The Test-PSSessionConfigurationFile cmdlet verifies that a session configuration file contains valid keys and the values are of the correct type. For enumerated values, the cmdlet verifies that the specified values are valid.
By default, Test-PSSessionConfigurationFile returns "True" ($true) if the file passes all tests and "False" ($false) if it does not. To find any errors, use the Verbose common parameter.
Test-PSSessionConfigurationFile verifies the session configuration files, such as those created by the New-PSSessionConfigurationFile cmdlet. For information about session configurations, see about_Session_Configurations (http://go.microsoft.com/fwlink/?LinkID=145152). For information about session configuration files, see about_Session_Configuration_Files (http://go.microsoft.com/fwlink/?LinkID=236023).
This cmdlet is introduced in Windows PowerShell 3.0.

### PARAMETERS

#### Path [String]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 1')]
```

Specifies the path and file name of a session configuration file (.pssc). If you omit the path, the default is the current directory. Wildcards are supported, but they must resolve to a single file. You can also pipe a session configuration file path to Test-PSSessionConfigurationFile.


### INPUTS
#### System.String
You can pipe a session configuration file path to Test-PSSessionConfigurationFile.

### OUTPUTS
#### System.Boolean


### NOTES

### EXAMPLES
#### Example 1: Test a session configuration file

```powershell
PS C:\>Test-PSSessionConfigurationFile -Path FullLanguage.pssc
True

```
This command uses the Test-PSSessionConfigurationFile cmdlet to test a new session configuration file before using it in a session configuration.


#### Example 2: Test the session configuration file of a session configuration

```powershell
PS C:\>Test-PSSessionConfigurationFile -Path (Get-PSSessionConfiguration -Name Restricted).ConfigFilePath


```
This command uses the Test-PSSessionConfigurationFile cmdlet to test the session configuration file that is being used to in the Restricted session configuration. The value of the Path parameter is a Get-PSSessionConfiguration command that gets the Restricted session configuration. The path to the session configuration file is stored in the value of the ConfigFilePath property of the session configuration.


#### Example 3: Test all session configuration files

```powershell
PS C:\>                     
function Test-AllConfigFiles
{
    Get-PSSessionConfiguration | ForEach-Object { if ($_.ConfigFilePath)
    {$_.ConfigFilePath; Test-PSSessionConfigurationFile -Verbose `
     -Path $_.ConfigFilePath }}
}
 
                      
C:\WINDOWS\System32\WindowsPowerShell\v1.0\SessionConfig\Empty_6fd77bf6-e084-4372-bd8a-af3e207354d3.psscTrueC:\WINDOWS\System32\WindowsPowerShell\v1.0\SessionConfig\Full_1e9cb265-dae0-4bd3-89a9-8338a47698a1.psscVERBOSE: The member 'AliasDefinitions' must contain the required key 'Description'. Add the require key to the fileC:\WINDOWS\System32\WindowsPowerShell\v1.0\SessionConfig\Full_1e9cb265-dae0-4bd3-89a9-8338a47698a1.pssc.FalseC:\WINDOWS\System32\WindowsPowerShell\v1.0\SessionConfig\NoLanguage_0c115179-ff2a-4f66-a5eb-e56e5692ba22.psscTrueC:\WINDOWS\System32\WindowsPowerShell\v1.0\SessionConfig\RestrictedLang_b6bd9474-0a6c-4e06-8722-c2c95bb10d3e.psscTrueC:\WINDOWS\System32\WindowsPowerShell\v1.0\SessionConfig\RRS_3fb29420-2c87-46e5-a402-e21436331efc.psscTrue

```
This function tests all session configuration files that are used in all session configurations on the local computer.
The function uses the Get-PSSessionConfiguration cmdlet to get all session configurations on the local computer. The command pipes the session configuration to the ForEach-Object cmdlet, which runs a command on each of the session configurations.
The ConfigFilePath property of a session configuration contains the path to the session configuration file that is used in the session configuration, if any.
If the value of the ConfigFilePath property is populated (is true), the command gets (prints) the ConfigFilePath property value. Then it uses the Test-PSSessionConfigurationFile cmdlet to test the file in the ConfigFilePath value. The Verbose parameter returns the file error when the file fails the test.



### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289619)
[Disable-PSSessionConfiguration]()
[Enable-PSSessionConfiguration]()
[Get-PSSessionConfiguration]()
[New-PSSessionConfigurationFile]()
[New-PSSessionConfigurationOption]()
[Register-PSSessionConfiguration]()
[Set-PSSessionConfiguration]()
[Test-PSSessionConfigurationFile]()
[Unregister-PSSessionConfiguration]()
[WSMan Provider]()
[about_Session_Configurations]()
[about_Session_Configuration_Files]()

## Unregister-PSSessionConfiguration

### SYNOPSIS
Deletes registered session configurations from the computer.

### DESCRIPTION
The Unregister-PSSessionConfiguration cmdlet deletes registered session configurations from the computer. This is an advanced cmdlet that is designed to be used by system administrators to manage customized session configurations for their users.
To make the change effective, Unregister-PSSessionConfiguration restarts the WinRM service. To prevent the restart, use the NoServiceRestart parameter.
If you accidentally delete the default Microsoft.PowerShell or Microsoft.PowerShell32 session configurations, use the Enable-PSRemoting function to restore them. For more information, see about_Session_Configurations.

### PARAMETERS

#### Force [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Suppresses all user prompts, and restarts the WinRM service without prompting. Restarting the service makes the configuration change effective.
To prevent a restart and suppress the restart prompt, use the NoServiceRestart parameter.

#### Name [String]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 1')]
```

Specifies the names of the session configurations to delete. Enter one session configuration name or a configuration name pattern. Wildcards are permitted. This parameter is required.
You can also pipe a session configurations to Unregister-PSSessionConfiguration.

#### NoServiceRestart [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Does not restart the WinRM service, and suppresses the prompt to restart the service.
By default, when you enter an Unregister-PSSessionConfiguration command, you are prompted to restart the WinRM service to make the change effective.  Until the WinRM service is restarted, users can still use the unregistered session configuration, even though Get-PSSessionConfiguration does not find it.
To restart the WinRM service without prompting, use the Force parameter. To restart the WinRM service manually, use the Restart-Service cmdlet.

#### Confirm [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Prompts you for confirmation before running the cmdlet.Prompts you for confirmation before running the cmdlet.

#### WhatIf [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Shows what would happen if the cmdlet runs. The cmdlet is not run.Shows what would happen if the cmdlet runs. The cmdlet is not run.


### INPUTS
####  Microsoft.PowerShell.Commands.PSSessionConfigurationCommands#PSSessionConfiguration
You can pipe a session configuration object from Get-PSSessionConfiguration to Unregister-PSSessionConfiguration.

### OUTPUTS
#### None
This cmdlet does not return any objects.

### NOTES
To run this cmdlet on Windows Vista, Windows Server 2008, and later versions of Windows, you must start Windows PowerShell with the "Run as administrator" option.


### EXAMPLES
#### -------------------------- EXAMPLE 1 --------------------------

```powershell
PS C:\>unregister-pssessionconfiguration -name MaintenanceShell

```
This command deletes the MaintenanceShell session configuration from the computer.






#### -------------------------- EXAMPLE 2 --------------------------

```powershell
PS C:\>unregister-pssessionconfiguration -name MaintenanceShell -force

```
This command deletes the MaintenanceShell session configuration from the computer. The command uses the Force parameter to suppress all user messages and to restart the WinRM service without prompting.






#### -------------------------- EXAMPLE 3 --------------------------

```powershell
PS C:\>unregister-pssessionconfiguration -name *
PS C:\>get-pssessionconfiguration -name * | unregister-pssessionconfiguration

```
These commands delete all of the session configurations on the computer. The commands have the same effect and can be used interchangeably.






#### -------------------------- EXAMPLE 4 --------------------------

```powershell
PS C:\>unregister-pssessionconfiguration -name maintenanceShell -noServiceRestart
PS C:\>get-pssessionconfiguration -name maintenanceShell

Get-PSSessionConfiguration -name maintenanceShell : No Session Configuration matches criteria "maintenanceShell".
+ CategoryInfo          : NotSpecified: (:) [Write-Error], WriteErrorException
+ FullyQualifiedErrorId : Microsoft.PowerShell.Commands.WriteErrorException

PS C:\>new-pssession -configurationName MaintenanceShell

Id Name      ComputerName    State    Configuration         Availability
-- ----      ------------    -----    -------------         ------------
1 Session1  localhost       Opened   MaintenanceShell      Available

PS C:\>restart-service winrm
PS C:\>new-pssession -configurationName MaintenanceShell

[localhost] Connecting to remote server failed with the following error message : The WS-Management service cannot process the request. The resource URI (http://schemas.microsoft.com/powershell/MaintenanceShell) was not found in the WS-Management catalog. The catalog contains the metadata that describes resources, or logical endpoints. For more information, see the about_Remote_Troubleshooting Help topic.
+ CategoryInfo          : OpenError: (System.Manageme....RemoteRunspace:RemoteRunspace) [], PSRemotingTransportException
+ FullyQualifiedErrorId : PSSessionOpenFailed

```
This example shows the effect of using the NoServiceRestart parameter of Unregister-PSSessionConfiguration. This parameter is designed to prevent a service restart, which would disrupt any sessions on the computer.
The first command uses the Unregister-PSSessionConfiguration cmdlet to deletes the MaintenanceShell session configuration. However, because the command uses the NoServiceRestart parameter, the WinRM service is not restarted and the change is not yet completely effective.
The second command uses the Get-PSSessionConfiguration cmdlet to get the MaintenanceShell session. Because the session has been removed from the WS-Management resource table, Get-PSSession cannot return it.
The third command uses the New-PSSession cmdlet to create a session on the local computer that uses the MaintenanceShell configuration. The command succeeds.
The fourth command uses the Restart-Service cmdlet to restart the WinRM service.
The fifth command again uses the New-PSSession cmdlet to create a session that uses the MaintenanceShell configuration. This time, the session fails because the MaintenanceShell configuration has been deleted.







### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289620)
[Disable-PSSessionConfiguration]()
[Enable-PSSessionConfiguration]()
[Get-PSSessionConfiguration]()
[New-PSSessionConfigurationFile]()
[New-PSSessionConfigurationOption]()
[Register-PSSessionConfiguration]()
[Set-PSSessionConfiguration]()
[Test-PSSessionConfigurationFile]()
[Unregister-PSSessionConfiguration]()
[WSMan Provider]()
[about_Session_Configurations]()
[about_Session_Configuration_Files]()

## Update-Help

### SYNOPSIS
Downloads and installs the newest help files on your computer.

### DESCRIPTION
The Update-Help cmdlet downloads the newest help files for Windows PowerShell modules and installs them on your computer. You can use the Get-Help cmdlet to view the new help files immediately; you do not need to restart Windows PowerShell to make the change effective. This feature enables you to install help files for modules that do not include them and to update help files on your computer so that they never become obsolete.
The Update-Help cmdlet checks the version of the help files on your computer. If you do not have help files for a module or do not have the newest help files for a module, Update-Help downloads the newest help files from the Internet or a file share and installs them on your computer in the correct module folder.
Without parameters, Update-Help updates the help for modules in the session and for all installed  modules (in a PSModulePath location) that support Updatable Help, even if the module is not in the current session. You can also use the Module parameter to update help for a particular module and use the UICulture parameter to download help files in multiple languages and locales.
You can use Update-Help even on computers that are not connected to the Internet. Use the Save-Help cmdlet to download help files from the Internet and save them in a file system location, such as a shared folder or file system directory. Then use the SourcePath parameter of Update-Help to get the updated help files from a file system location and install them on the computer.
You can even automate the running of Update-Help by adding an Update-Help command to your Windows PowerShell profile. By default, Update-Help runs only once per day on each computer. To override the once-per-day limit, use the Force parameter.
To download or update the help files for modules in the Windows PowerShell installation directory ($pshome\Modules), including the Windows PowerShell Core modules, start Windows PowerShell with the "Run as administrator" option. You must be a member of the Administrators group on the computer to update the help files for these modules.
You can also update help files by using the "Update Windows PowerShell Help" menu item in the Help menu in Windows PowerShell Integrated Scripting Environment (ISE). The "Update Windows PowerShell Help" item runs an Update-Help command without parameters. To update help for modules in the $PSHome directory, start Windows PowerShell ISE with the "Run as administrator" option.
This cmdlet is introduced in Windows PowerShell 3.0.

### PARAMETERS

#### Credential [PSCredential]

Runs the command with credentials of a user who has permission to access the file system location specified by the SourcePath parameter. This parameter is valid only when the SourcePath or LiteralPath parameter is used in the command.
This parameter enables you to run Update-Help commands with the SourcePath parameter on remote computers. By providing explicit credentials, you can run the command on a remote computer and access a file share on a third computer without encountering an "access denied" error or using CredSSP authentication to delegate credentials.

#### Force [switch]

Overrides the once-per-day limitation, version checking, and the 1 GB per module limit.
Without this parameter, Update-Help runs only once in each 24-hour period, downloads are limited to 1 GB of uncompressed content per module and help files are installed only when they are newer than the files on the computer.
The once-per-day limit protects the servers that host the help files and makes it practical for you to add an Update-Help command to your Windows PowerShell profile without incurring the resource cost of repeated connections or downloads.
To update help for a module in multiple UI cultures without the Force parameter, include all UI cultures in the same command, such as: Update-Help -Module PSScheduledJobs -UICulture en-US, fr-FR, pt-BR

#### FullyQualifiedModule [ModuleSpecification[]]

```powershell
[Parameter(
  Position = 1,
  ValueFromPipelineByPropertyName = $true)]
```

Specifies modules with names that are specified in the form of ModuleSpecification objects (described by the Remarks section of [Module Specification Constructor (Hashtable)]() on MSDN). For example, the FullyQualifiedModule parameter accepts a module name that is specified in the format @{ModuleName = "modulename"; ModuleVersion = "version_number"} or @{ModuleName = "modulename"; ModuleVersion = "version_number"; Guid = "GUID"}. ModuleName and ModuleVersion are required, but Guid is optional.
You cannot specify the FullyQualifiedModule parameter in the same command as a Module parameter; the two parameters are mutually exclusive.

#### LiteralPath [String[]]

```powershell
[Parameter(
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 2')]
```

Gets updated help files from the specified directory instead of downloading them from the Internet. Use this parameter or the SourcePath parameter if you have used the Save-Help cmdlet to download help files to a directory. 
You can also pipe a directory object, such as one from the Get-Item or Get-ChildItem cmdlets, to Update-Help.
Unlike the value of the SourcePath parameter, the value of the LiteralPath parameter is used exactly as it is typed. No characters are interpreted as wildcards. If the path includes escape characters, enclose it in single quotation marks. Single quotation marks tell Windows PowerShell not to interpret any characters as escape sequences.

#### Module [String[]]

```powershell
[Parameter(
  Position = 1,
  ValueFromPipelineByPropertyName = $true)]
```

Updates help for the specified modules. Enter one or more module names or name patters in a comma-separated list, or specify a file that lists one module name on each line. Wildcard characters are permitted. You can also pipe modules from the Get-Module cmdlet, to the Update-Help cmdlet.
The modules that you specify must be installed on the computer, but they do not need to be imported into the current session. You can specify any module in the session or any module that is installed in a location listed in the PSModulePath environment variable.
A value of "*" (all) attempts to update help for all modules that are installed on the computer, including modules that do not support Updatable Help. This value might generate errors as the command encounters modules that do not support Updatable Help. Instead, run an Update-Help command without parameters.
The Module parameter of the Update-Help cmdlet does not accept the full path to a module file or module manifest file. To update help for a module that is not in a PSModulePath location, import the module into the current session before running the Update-Help command.

#### Recurse [switch]

Searches recursively for help files in the specified directory. This parameter is valid only when the SourcePath parameter is used in the command.

#### SourcePath [String[]]

```powershell
[Parameter(
  Position = 2,
  ParameterSetName = 'Set 1')]
```

Gets updated help files from the specified file system directory, instead of downloading them from the Internet. Enter the path to a directory. Do not specify a file name or file name extension. You can also pipe a directory object, such as one from the Get-Item or Get-ChildItem cmdlets, to Update-Help.
By default, Update-Help downloads updated help files from the Internet. Use this parameter when you have used the Save-Help cmdlet to download  updated help files to a directory.
Administrators can use the "Set the default source path for Update-Help" Group Policy setting under Computer Configuration to specify a default value for the SourcePath parameter. This Group Policy setting prevents users from using Update-Help to download help files from the Internet. For more information, see about_Group_Policy_Settings (http://go.microsoft.com/fwlink/?LinkId=251696).

#### UICulture [CultureInfo[]]

```powershell
[Parameter(Position = 3)]
```

Gets updated help files for the specified UI culture. Enter one or more language codes, such as "es-ES", a variable that contains culture objects, or a command that gets culture objects, such as a Get-Culture or Get-UICulture command. Wildcards are not permitted and you cannot submit a partial language code, such as "de".
By default, Update-Help gets help files in the UI culture set for Windows or its fallback culture. If you use the UICulture parameter, Update-Help looks for help only for the specified UI culture, not in any fallback culture.
Commands that use the UICulture parameter succeed only when the module provides help files for the specified UI culture. If the command fails because the specified UI culture is not supported, the error message includes a list of UI cultures that the module supports.

#### UseDefaultCredentials [switch]

Runs the command, including the Internet download, with the credentials of the current user. By default, the command runs without explicit credentials.
This parameter is effective only when the web download uses NTLM, negotiate, or Kerberos-based authentication.


### INPUTS
#### System.IO.DirectoryInfo System.Management.Automation.PSModuleInfo
You can pipe a directory path to Update-Help.
You can pipe a module object from the Get-Module cmdlet to  Update-Help.

### OUTPUTS
#### None
Update-Help does not generate any output.

### NOTES
To update help for the Windows PowerShell Core modules (which contain the commands that are installed with Windows PowerShell) or any module in the $pshome\Modules directory, start Windows PowerShell with the "Run as administrator" option.
Only  members of the Administrators group on the computer can update help for the for the Windows PowerShell Core modules (the commands that are installed with Windows PowerShell) and for modules in the $pshome\Modules directory. If you do not have permission to update help files, you might be able to read the help topics online.  To open the online version of any cmdlet help topic, type "Get-Help <cmdlet-name> -Online ".
Modules are the smallest unit of updatable help. You cannot update help for a particular cmdlet; only for all cmdlets in module. To find the module that contains a particular cmdlet, use the ModuleName property of the Get-Command cmdlet, for example, (Get-Command <cmdlet-name>).ModuleName
Because help files are installed in the module directory, the Update-Help cmdlet can install updated help file only for modules that are installed on the computer. However, the Save-Help cmdlet can save help for modules that are not installed on the computer.
If Update-Help cannot find updated help files for a module, or cannot find updated help in the specified language, it continues silently without displaying an error message. To see status and progress details, use the Verbose parameter.
The Update-Help cmdlet was introduced in Windows PowerShell 3.0. It does not work in earlier versions of Windows PowerShell. On computers that have both the Windows PowerShell 2.0 engine and Windows PowerShell 3.0, use the Update-Help cmdlet in a Windows PowerShell 3.0 session to download and update help files. The help files are accessible to both Windows PowerShell 2.0 and Windows PowerShell 3.0.
The Update-Help and Save-Help cmdlets use the following ports to download help files: Port 80 for HTTP and port 443 for HTTPS.
Update-Help supports all modules and the Windows PowerShell Core snap-ins. It does not support any other snap-ins.
To update help for a module in a location that is not listed in the PSModulePath environment variable, import the module into the current session and then run an Update-Help command. Run the Update-Help command without parameters or use the Module parameter to specify the module name. The Module parameter of the Update-Help and Save-Help cmdlets does not accept the full path to a module file or module manifest file.
Any module can support Updatable Help. For instructions for supporting Updatable Help in the modules that you author, see "Supporting Updatable Help" in the MSDN Library at http://go.microsoft.com/fwlink/?LinkID=242129.
The Update-Help and Save-Help cmdlets are not supported on Windows Preinstallation Environment (Windows PE).

### EXAMPLES
#### Example 1: Update help for all modules

```powershell
PS C:\>Update-Help

```
This command updates help for all installed modules that support Updatable Help in the language specified by the UI culture that is set for Windows.
To run this command, start Windows PowerShell with the "Run as administrator" option (Start-Process PowerShell -Verb RunAs).


#### Example 2: Update help for specified modules

```powershell
PS C:\>Update-Help -Module ServerManager, Microsoft.PowerShell*

```
This command updates help only for the ServerManager module and for modules that have names that begin with "Microsoft.PowerShell".
Because these modules are in the $pshome\Modules directory, to run this command, start Windows PowerShell with the "Run as administrator" option.


#### Example 3: Update help in different  languages

```powershell
PS C:\>Update-Help -UICulture ja-JP, en-US
Update-Help : Failed to update Help for the module(s) 'ServerManager' with UI culture(s) {ja-JP} : 
The specified culture is not supported: ja-JP. Specify a culture from the following list: {en-US}.

```
This command updates the Japanese and English help files for all modules.
If a module currently does not provide help files for the specified UI culture, the error message lists the UI cultures that the module supports. In this example, the error message indicates that the ServerManager module currently provides help files only in en-US. 


#### Example 4: Update help automatically

```powershell
PS C:\>Register-ScheduledJob -Name UpdateHelpJob -Credential Domain01\User01 -ScriptBlock {Update-Help} -Trigger (New-JobTrigger -Daily -At "3 AM")
Id         Name            JobTriggers     Command                                  Enabled
--         ----            -----------     -------                                  -------
1          UpdateHelpJob   1               Update-Help                              True

```
This command creates a scheduled job that updates help for all modules on the computer every day at 3:00 in the morning.
The command uses the Register-ScheduledJob cmdlet to create a scheduled job that runs an Update-Help command. The command uses the Credential parameter to run the Update-Help cmdlet with the credentials of a member of the Administrators group on the computer. The value of the Trigger parameter is a New-JobTrigger command that creates a job trigger that starts the job every day at 3:00 AM.
To run the Register-ScheduledJob command, start Windows PowerShell with the "Run as administrator" option. When you run the command, Windows PowerShell prompts you for the password of the user specified in the value of the Credential parameter. The credentials are stored with the scheduled job; you are not prompted when the job runs.
You can use the Get-ScheduledJob cmdlet to view the scheduled job, use the Set-ScheduledJob cmdlet to change it, and use the Unregister-ScheduledJob cmdlet to delete it. You can also view and manage the scheduled job in Task Scheduler in the following path: Task Scheduler Library\Microsoft\Windows\PowerShell\ScheduledJobs.


#### Example 5: Update help on multiple computers from a file share

```powershell
The first command uses the Save-Help cmdlet to download the newest help files for all modules that support Updatable Help. The command saves the downloaded help files in the \\Server01\Share\PSHelp file share.The command uses the Credential parameter of the Save-Help cmdlet to specify the credentials of a user who has permission to access the remote file share. By default, the command does not run with explicit credentials and attempts to access the file share might fail.
PS C:\>Save-Help -DestinationPath \\Server01\Share\PSHelp -Credential Domain01\Admin01

The second command uses the Invoke-Command cmdlet to run Update-Help commands on many computers remotely.The Invoke-Command command gets the list of computers from the Servers.txt file. The Update-Help command installs the help files from the file share on all of the remote computers. The remote computer must be able to access the file share at the specified path.The Update-Help command uses the SourcePath parameter to get the updated help files from the file share, instead of the Internet, and the Credential parameter to run the command with explicit credentials. By default, the command runs with network token privileges and attempts to access the file share from each remote computer (a "second hop") might fail.
PS C:\>Invoke-Command -ComputerName (Get-Content Servers.txt) -ScriptBlock {Update-Help -SourcePath \\Server01\Share\Help -Credential Domain01\Admin01}

```
These commands download updated help files for system modules from the Internet and save them in file share. Then the commands install the updated help files from the file share on multiple computers. You can use a strategy like the one shown here to update the help files on numerous computers, even those that are behind firewalls or are not connected to the Internet.
All of the commands in this example were run in a Windows PowerShell session that was started with the "Run as administrator" option.


#### Example 6: Get a List of Updated Help Files

```powershell
PS C:\>Update-Help -Module BestPractices, ServerManager -Verbose

```
This command updates help for two modules. It uses the Verbose common parameter of the Update-Help cmdlet to get a list of the help files that the command updated.
Without the Verbose parameter, Update-Help does not display the results of the command. The Verbose parameter is especially useful when you need to verify that you have updated help files for a particular module or a particular locale.


#### Example 7: Find modules that support Updatable Help

```powershell
PS C:\>Get-Module -ListAvailable | Where HelpInfoUri

```
This command gets modules that support Updatable Help.
The command uses the HelpInfoUri property of modules to identify modules that support Updatable Help. The value of the HelpInfoUri property contains the address of the Internet location where the module stores its Updatable Help information file.
This command uses the simplified syntax of the Where-Object cmdlet. This syntax is introduced in Windows PowerShell 3.0. 


#### Example 8: Inventory updated help files

```powershell
PS C:\>
#Get-UpdateHelpVersion.ps1
Param
      (
         [parameter(Mandatory=$False)]
         [String[]]
         $Module
      )      
$HelpInfoNamespace = @{helpInfo="http://schemas.microsoft.com/powershell/help/2010/05"}

if ($Module) { $Modules = Get-Module $Module -ListAvailable | where {$_.HelpInfoUri} }
else { $Modules = Get-Module -ListAvailable | where {$_.HelpInfoUri} }

foreach ($mModule in $Modules)
{
    $mDir = $mModule.ModuleBase

    if (Test-Path $mdir\*helpinfo.xml)
    {
        $mName=$mModule.Name
        $mNodes = dir $mdir\*helpinfo.xml -ErrorAction SilentlyContinue | Select-Xml -Namespace $HelpInfoNamespace -XPath "//helpInfo:UICulture"
        foreach ($mNode in $mNodes)
        {
            $mCulture=$mNode.Node.UICultureName
            $mVer=$mNode.Node.UICultureVersion
            
            [PSCustomObject]@{"ModuleName"=$mName; "Culture"=$mCulture; "Version"=$mVer}
        }
    }
}

ModuleName                              Culture                                 Version

----------                              -------                                 -------

ActiveDirectory                         en-US                                   3.0.0.0

ADCSAdministration                      en-US                                   3.0.0.0

ADCSDeployment                          en-US                                   3.0.0.0

ADDSDeployment                          en-US                                   3.0.0.0

ADFS                                    en-US                                   3.0.0.0

…


```
The Get-UpdateHelpVersion.ps1 script creates an inventory of the Updatable Help files for each module and their version numbers. Copy the script and paste it in a text file.
The script identifies modules that support Updatable Help by using the HelpInfoUri property of modules. For modules that support Updatable Help, the script looks for and parses the help information file (HelpInfo XML) to find the latest version number.
The script uses the PSCustomObject class and a hash table to create a custom output object.



### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289621)
[Updatable Help Status Table (http://go.microsoft.com/fwlink/?LinkID=270007)]()
[Get-ChildItem]()
[Get-Culture]()
[Get-Help]()
[Get-Item]()
[Get-Module]()
[Get-UICulture]()
[Start-Job]()
[Save-Help]()

## Wait-Job

### SYNOPSIS
Suppresses the command prompt until one or all of the Windows PowerShell background jobs running in the session are complete.

### DESCRIPTION
The Wait-Job cmdlet waits for Windows PowerShell background jobs to complete before it displays the command prompt. You can wait until any background job is complete, or until all background jobs are complete, and you can set a maximum wait time for the job.
When the commands in the job are complete, Wait-Job displays the command prompt and returns a job object so that you can pipe it to another command.
You can use Wait-Job cmdlet to wait for background jobs, such as those that were started by using the Start-Job cmdlet or the AsJob parameter of the Invoke-Command cmdlet. For more information about Windows PowerShell background jobs, see about_Jobs.
Beginning in Windows PowerShell 3.0, the Wait-Job cmdlet also waits for custom job types, such as workflow jobs and instances of scheduled jobs. To enable Wait-Job to wait for jobs of a particular type, import the module that supports the custom job type into the session before running a Get-Job command, either by using the Import-Module cmdlet or by using or getting a cmdlet in the module. For information about a particular custom job type, see the documentation of the custom job type feature.

### PARAMETERS

#### Any [switch]

Displays the command prompt (and returns the job object) when any job completes. By default, Wait-Job waits until all of the specified jobs are complete before displaying the prompt.

#### Filter [Hashtable]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 2')]
```

Waits for jobs that satisfy all of the conditions established in the associated hash table. Enter a hash table where the keys are job properties and the values are job property values.
This parameter works only on custom job types, such as workflow jobs and scheduled jobs. It does not work on standard background jobs, such as those created by using the Start-Job cmdlet. For information about support for this parameter, see the help topic for the job type.
This parameter is introduced in Windows PowerShell 3.0.

#### Id [Int32[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 1')]
```

Waits for jobs with the specified IDs.
The ID is an integer that uniquely identifies the job within the current session. It is easier to remember and type than the InstanceId, but it is unique only within the current session. You can type one or more IDs (separated by commas). To find the ID of a job, type "Get-Job" without parameters.

#### InstanceId [Guid[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 3')]
```

Waits for jobs with the specified instance IDs. The default is all jobs.
An instance ID is a GUID that uniquely identifies the job on the computer. To find the instance ID of a job, use Get-Job.

#### Job [Job[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 4')]
```

Waits for the specified jobs. Enter a variable that contains the job objects or a command that gets the job objects. You can also use a pipeline operator to send job objects to the Wait-Job cmdlet. By default, Wait-Job waits for all jobs created in the current session.

#### Name [String[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 5')]
```

Waits for jobs with the specified friendly name.

#### State [JobState]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 6')]
```

Waits only for jobs in the specified state. Valid values are NotStarted, Running, Completed, Failed, Stopped, Blocked, Suspended, Disconnected, Suspending, Stopping.
For more information about job states, see "JobState Enumeration" in MSDN at [http://msdn.microsoft.com/en-us/library/windows/desktop/system.management.automation.jobstate(v=vs.85).aspx]()

#### Timeout [Int32]

Determines the maximum wait time for each background job, in seconds. The default, -1, waits until the job completes, no matter how long it runs. The timing starts when you submit the Wait-Job command, not the Start-Job command.
If this time is exceeded, the wait ends and the command prompt returns, even if the job is still running. No error message is displayed.

#### Force [switch]

Continues waiting if jobs are in the Suspended or Disconnected state. By default, Wait-Job returns (terminates the wait) when jobs are in one of the following states: Completed, Failed, Stopped, Suspended, or Disconnected.
This parameter is introduced in Windows PowerShell 3.0.


### INPUTS
#### System.Management.Automation.RemotingJob
You can pipe a job object to Wait-Job.

### OUTPUTS
#### System.Management.Automation.PSRemotingJob
Wait-Job returns job objects that represent the completed jobs. If the wait ends because the value of the Timeout parameter is exceeded, Wait-Job does not return any objects.

### NOTES
By default, Wait-Job returns (terminates the wait) when jobs are in one of the following states: Completed, Failed, Stopped, Suspended, or Disconnected. To direct Wait-Job to continue waiting for Suspended and Disconnected jobs, use the Force parameter.

### EXAMPLES
#### -------------------------- EXAMPLE 1 --------------------------

```powershell
PS C:\>Get-Job | Wait-Job

```
This command waits for all of the background jobs running in the session to complete.






#### -------------------------- EXAMPLE 2 --------------------------

```powershell
PS C:\>$s = New-PSSession Server01, Server02, Server03
PS C:\>Invoke-Command -Session $s -ScriptBlock {Start-Job -Name Date1 -ScriptBlock {Get-Date}}
PS C:\>$done = Invoke-Command -Session $s -Command {Wait-Job -Name Date1}
PS C:\>$done.Count
3

```
This example shows how to use the Wait-Job cmdlet with jobs started on remote computers by using the Start-Job cmdlet. Both the Start-Job and Wait-Job commands are submitted to the remote computer by using the Invoke-Command cmdlet.
This example uses Wait-Job to determine whether a Get-Date command running as a background job on three different computers is complete.
The first command creates a Windows PowerShell session (PSSession) on each of the three remote computers and stores them in the $s variable.
The second command uses the Invoke-Command cmdlet to run a Start-Job command in each of the three sessions in $s. All of the jobs are named Date1.
The third command uses the Invoke-Command cmdlet to run a Wait-Job command. This command waits for the Date1 jobs on each computer to complete. It stores the resulting collection (array) of job objects in the $done variable.
The fourth command uses the Count property of the array of job objects in the $done variable to determine how many of the jobs are complete.






#### -------------------------- EXAMPLE 3 --------------------------

```powershell
PS C:\>$s = New-PSSession (Get-Content Machines.txt)
PS C:\>$c = 'Get-EventLog -LogName System | where {$_.EntryType -eq "error" --and $_.Source -eq "LSASRV"} | Out-File Errors.txt'
PS C:\>Invoke-Command -Session $s -ScriptBlock {Start-Job -ScriptBlock {$Using:c}
PS C:\>Invoke-Command -Session $s -ScriptBlock {Wait-Job -Any}

```
This example uses the Any parameter of Wait-Job to determine when the first of many background jobs running in the current session are complete. It also shows how to use the Wait-Job cmdlet to wait for remote jobs to complete.
The first command creates a PSSession on each of the computers listed in the Machines.txt file and stores the PSSessions in the $s variable. The command uses the Get-Content cmdlet to get the contents of the file. The Get-Content command is enclosed in parentheses to ensure that it runs before the New-PSSession command.
The second command stores a Get-EventLog command string (in quotation marks) in the $c variable.
The third command uses the Invoke-Command cmdlet to run a Start-Job command in each of the sessions in $s. The Start-Job command starts a background job that runs the Get-EventLog command in the $c variable.
The command uses the Using scope modifier to indicate that the $c variable was defined on the local computer. The Using scope modifier is introduced in Windows PowerShell 3.0. For more information about the Using scope modifier, see about_Remote_Variables (http://go.microsoft.com/fwlink/?LinkID=252653).
The fourth command uses the Invoke-Command cmdlet to run a Wait-Job command in the sessions. It uses the Any parameter to wait until the first job on the remote computers is complete.






#### -------------------------- EXAMPLE 4 --------------------------

```powershell
PS C:\>$s = New-PSSession Server01, Server02, Server03
PS C:\>$jobs = Invoke-Command -Session $s -ScriptBlock {Start-Job -ScriptBlock {Get-Date}}
PS C:\>$done = Invoke-Command -Session $s -ScriptBlock {Wait-Job -Timeout 30}

```
This example shows how to use the Timeout parameter of Wait-Job to set a maximum wait time for the jobs running on remote computers.
The first command creates a PSSession on each of three remote computers (Server01, Server02, and Server03), and it saves the PSSessions in the $s variable.
The second command uses the Invoke-Command cmdlet to run a Start-Job command in each of the PSSessions in $s. It saves the resulting job objects in the $jobs variable.
The third command uses the Invoke-Command cmdlet to run a Wait-Job command in each of the PSSessions in $s. The Wait-Job command determines whether all of the commands have completed within 30 seconds. It uses the Timeout parameter with a value of 30 (seconds) to establish the maximum wait time and saves the results of the command in the $done variable.
In this case, after 30 seconds, only the command on the Server02 computer has completed. Wait-Job ends the wait, displays the command prompt, and returns the object that represents the job that was completed.
The $done variable contains a job object that represents the job that ran on Server02.






#### -------------------------- EXAMPLE 5 --------------------------

```powershell
PS C:\>Wait-Job -id 1,2,5 -Any

```
This command identifies three jobs by their IDs and waits until any of them are complete. The command prompt returns when the first job completes.






#### -------------------------- EXAMPLE 6 --------------------------

```powershell
PS C:\>Wait-Job -Name DailyLog -Timeout 120

```
This command waits 120 seconds (two minutes) for the DailyLog job to complete. If the job does not complete in the next two minutes, the command prompt returns anyway, and the job continues to run in the background.






#### -------------------------- EXAMPLE 7 --------------------------

```powershell
PS C:\>Wait-Job -Name Job3

```
This Wait-Job command uses the job name to identify the job to wait for.






#### -------------------------- EXAMPLE 8 --------------------------

```powershell
PS C:\>$j = Start-Job -ScriptBlock {Get-ChildItem *.ps1| where {$_lastwritetime -gt ((Get-Date) - (New-TimeSpan -Days 7))}}
PS C:\>$j | Wait-Job

```
This example shows how to use the Wait-Job cmdlet with jobs started on the local computer by using the Start-Job cmdlet.
These commands start a job that gets the Windows PowerShell script files that were added or updated in the last week.
The first command uses the Start-Job cmdlet to start a background job on the local computer. The job runs a Get-ChildItem command that gets all of the files with a ".ps1" file name extension that were added or updated in the last week.
The third command uses the Wait-Job cmdlet to wait until the job is complete. When the job completes, the command displays the job object, which contains information about the job.






#### -------------------------- EXAMPLE 9 --------------------------

```powershell
PS C:\>$s = New-PSSession Server01, Server02, Server03
PS C:\>$j = Invoke-Command -Session $s -ScriptBlock {Get-Process} -AsJob
PS C:\>$j | Wait-Job

```
This example shows how to use the Wait-Job cmdlet with jobs started on remote computers by using the AsJob parameter of the Invoke-Command cmdlet. When using AsJob, the job is created on the local computer and the results are automatically returned to the local computer, even though the job runs on the remote computers.
This example uses Wait-Job to determine whether a Get-Process command running in the sessions on three remote computers is complete.
The first command creates PSSessions on three computers and stores them in the $s variable.
The second command uses the Invoke-Command cmdlet to run a Get-Process command in each of the three PSSessions in $s. The command uses the AsJob parameter to run the command asynchronously as a background job. The command returns a job object, just like the jobs started by using Start-Job, and the job object is stored in the $j variable.
The third command uses a pipeline operator (|) to send the job object in $j to the Wait-Job cmdlet. Notice that an Invoke-Command command is not required in this case, because the job resides on the local computer.






#### -------------------------- EXAMPLE 10 --------------------------

```powershell
PS C:\>Get-Job

Id   Name     State      HasMoreData     Location             Command
--   ----     -----      -----------     --------             -------
1    Job1     Completed  True            localhost,Server01.. get-service
4    Job4     Completed  True            localhost            dir | where

PS C:\>Wait-Job -id 1

```
This command waits for the job with an ID value of 1.







### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289622)
[Get-Job]()
[Invoke-Command]()
[Receive-Job]()
[Remove-Job]()
[Resume-Job]()
[Start-Job]()
[Stop-Job]()
[Suspend-Job]()
[about_Jobs]()
[about_Job_Details]()
[about_Remote_Jobs]()
[about_Remote_Variables]()

## Where-Object

### SYNOPSIS
Selects objects from a collection based on their property values.

### DESCRIPTION
The Where-Object cmdlet selects objects that have particular property values from the collection of objects that are passed to it. For example you can use the Where-Object cmdlet to select files that were created after a certain date, events with a particular ID, or computers with a particular version of Windows.
Beginning in Windows PowerShell 3.0, there are two different ways to construct a Where-Object command.
Script block. You can use a script block to specify the property name, a comparison operator, and a property value. Where-Object returns all objects for which the script block statement is true.
For example, the following command gets processes in the Normal priority class, that is, processes where the value of the PriorityClass property equals "Normal".
Get-Process | Where-Object {$_.PriorityClass -eq "Normal"}
All Windows PowerShell comparison operators are valid in the script block format. For more information about comparison operators, see about_Comparison_Operators (http://go.microsoft.com/fwlink/?LinkID=113217).
Comparison statement. You can also write a comparison statement, which is much more like natural language. Comparison statements were introduced in Windows PowerShell 3.0.
For example, the following commands also get processes that have a priority class of "Normal". These commands are equivalent and can be used interchangeably.
Get-Process | Where-Object -Property PriorityClass -eq -Value "Normal"
Get-Process | Where-Object PriorityClass -eq "Normal"
Beginning in Windows PowerShell 3.0, Where-Object adds comparison operators as parameters in a Where-Object command. Unless specified, all operators are case-insensitive. Prior to Windows PowerShell 3.0, the comparison operators in the Windows PowerShell language could be used only in script blocks.

### PARAMETERS

#### Contains [switch]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 2')]
```

Specifies the Contains operator, which gets objects when any item in the property value of the object is an exact match for the specified value.
For example: Get-Process | where ProcessName -contains "Svchost"
If the property value contains a single object, Windows PowerShell converts it to a collection of one object.
This parameter is introduced in Windows PowerShell 3.0.

#### EQ [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Specifies the equals operator, which gets objects when the property value is the same as the specified value.
This parameter is introduced in Windows PowerShell 3.0.

#### FilterScript [ScriptBlock]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 3')]
```

Specifies the script block that is used to filter the objects. Enclose the script block in braces ( {} ).
The parameter name (-FilterScript) is optional.

#### GE [switch]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 4')]
```

Specifies the Greater-than-or-equal operator, which gets objects when the property value is greater than or equal to the specified value.
This parameter is introduced in Windows PowerShell 3.0.

#### GT [switch]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 5')]
```

Specifies the Greater-than operator, which gets objects when the property value is greater than the specified value.
This parameter is introduced in Windows PowerShell 3.0.

#### In [switch]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 6')]
```

Specifies the In operator, which gets objects when the property value matches any of the specified values.
For example: Get-Process | where -Property ProcessName -in -Value "Svchost", "TaskHost", "WsmProvHost"
If the value of the Value parameter is a single object, Windows PowerShell converts it to a collection of one object. 
If the property value of an object is an array, Windows PowerShell uses reference equality to determine a match. Where-Object returns the object only if the value of the Property parameter and any value of the Value parameter are the same instance of an object.
This parameter is introduced in Windows PowerShell 3.0.

#### InputObject [PSObject]

```powershell
[Parameter(ValueFromPipeline = $true)]
```

Specifies the objects to be filtered. You can also pipe the objects to Where-Object. When you use the InputObject parameter with Where-Object, instead of piping command results to Where-Object, the InputObject value—even if the value is a collection that is the result of a command, such as -InputObject (Get-Process)—is treated as a single object. Because InputObject cannot return individual properties from an array or collection of objects, it is recommended that if you use Where-Object to filter a collection of objects for those objects that have specific values in defined properties, you use Where-Object in the pipeline, as shown in the examples in this topic.

#### Is [switch]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 21')]
```

Specifies the Is operator, which gets objects when the property value is an instance of the specified .NET Framework type. Enclose the type name in square brackets.
For example, Get-Process | where StartTime -Is [DateTime]
This parameter is introduced in Windows PowerShell 3.0.

#### IsNot [switch]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 22')]
```

Specifies the Is-Not operator, which gets objects when the property value is not an instance of the specified .NET Framework type.
For example, Get-Process | where StartTime -IsNot [System.String]
This parameter is introduced in Windows PowerShell 3.0.

#### LE [switch]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 23')]
```

Specifies the Less-than-or-equals operator.
This parameter is introduced in Windows PowerShell 3.0.

#### LT [switch]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 25')]
```

Specifies the Less-than operator, which gets objects when the property value is less than the specified value.
This parameter is introduced in Windows PowerShell 3.0.

#### Like [switch]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 24')]
```

Specifies the Like operator, which gets objects when the property value matches a value that includes wildcard characters. 
For example: Get-Process | where ProcessName -like "*host"
This parameter is introduced in Windows PowerShell 3.0.

#### Match [switch]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 26')]
```

Specifies the Match operator, which gets objects when the property value matches the specified regular expression. When the input is scalar, the matched value is saved in $Matches automatic variable.
For example: Get-Process | where ProcessName -match "shell"
This parameter is introduced in Windows PowerShell 3.0.

#### NE [switch]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 27')]
```

Specifies the Not-equals operator, which gets objects when the property value is different than the specified value.
This parameter is introduced in Windows PowerShell 3.0.

#### NotContains [switch]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 28')]
```

Specifies the  Not-Contains operator, which gets objects when none of the items in the  property value is an exact match for the specified value.
For example: Get-Process | where ProcessName -NotContains "Svchost"
"NotContains" refers to a collection of values and is true when the collection does not contain any items that are an exact match for the specified value.  If the input is a single object, Windows PowerShell converts it to a collection of one object.
This parameter is introduced in Windows PowerShell 3.0.

#### NotIn [switch]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 29')]
```

Specifies the  Not-In operator, which gets objects when the property value is not an exact match for any of the specified values.
For example: Get-Process | where -Value "svchost" -NotIn -Property ProcessName
If the value of the Value parameter is a single object, Windows PowerShell converts it to a collection of one object. 
If the property value of an object is an array, Windows PowerShell uses reference equality to determine a match. Where-Object returns the object only if the value of the Property parameter and any value of the Value parameter are not the same instance of an object.
This parameter is introduced in Windows PowerShell 3.0.

#### NotLike [switch]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 30')]
```

Specifies the Not-Like operator, which gets objects when the property value does not match a value that includes wildcard characters. 
For example: Get-Process | where ProcessName -NotLike "*host"
This parameter is introduced in Windows PowerShell 3.0.

#### NotMatch [switch]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 31')]
```

Specifies the not-match operator, which gets objects when the property value does not match the specified regular expression. When the input is scalar, the matched value is saved in $Matches automatic variable.
For example: Get-Process | where ProcessName -NotMatch "PowerShell"
This parameter is introduced in Windows PowerShell 3.0.

#### Property [String]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 1')]
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 2')]
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 4')]
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 5')]
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 6')]
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 7')]
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 8')]
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 9')]
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 10')]
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 11')]
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 12')]
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 13')]
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 14')]
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 15')]
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 16')]
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 17')]
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 18')]
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 19')]
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 20')]
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 21')]
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 22')]
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 23')]
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 24')]
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 25')]
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 26')]
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 27')]
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 28')]
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 29')]
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 30')]
[Parameter(
  Mandatory = $true,
  Position = 1,
  ParameterSetName = 'Set 31')]
```

Specifies the name of an object property.
The parameter name (-Property) is optional.
This parameter is introduced in Windows PowerShell 3.0.

#### Value [Object]

```powershell
[Parameter(
  Position = 2,
  ParameterSetName = 'Set 1')]
[Parameter(
  Position = 2,
  ParameterSetName = 'Set 2')]
[Parameter(
  Position = 2,
  ParameterSetName = 'Set 4')]
[Parameter(
  Position = 2,
  ParameterSetName = 'Set 5')]
[Parameter(
  Position = 2,
  ParameterSetName = 'Set 6')]
[Parameter(
  Position = 2,
  ParameterSetName = 'Set 7')]
[Parameter(
  Position = 2,
  ParameterSetName = 'Set 8')]
[Parameter(
  Position = 2,
  ParameterSetName = 'Set 9')]
[Parameter(
  Position = 2,
  ParameterSetName = 'Set 10')]
[Parameter(
  Position = 2,
  ParameterSetName = 'Set 11')]
[Parameter(
  Position = 2,
  ParameterSetName = 'Set 12')]
[Parameter(
  Position = 2,
  ParameterSetName = 'Set 13')]
[Parameter(
  Position = 2,
  ParameterSetName = 'Set 14')]
[Parameter(
  Position = 2,
  ParameterSetName = 'Set 15')]
[Parameter(
  Position = 2,
  ParameterSetName = 'Set 16')]
[Parameter(
  Position = 2,
  ParameterSetName = 'Set 17')]
[Parameter(
  Position = 2,
  ParameterSetName = 'Set 18')]
[Parameter(
  Position = 2,
  ParameterSetName = 'Set 19')]
[Parameter(
  Position = 2,
  ParameterSetName = 'Set 20')]
[Parameter(
  Position = 2,
  ParameterSetName = 'Set 21')]
[Parameter(
  Position = 2,
  ParameterSetName = 'Set 22')]
[Parameter(
  Position = 2,
  ParameterSetName = 'Set 23')]
[Parameter(
  Position = 2,
  ParameterSetName = 'Set 24')]
[Parameter(
  Position = 2,
  ParameterSetName = 'Set 25')]
[Parameter(
  Position = 2,
  ParameterSetName = 'Set 26')]
[Parameter(
  Position = 2,
  ParameterSetName = 'Set 27')]
[Parameter(
  Position = 2,
  ParameterSetName = 'Set 28')]
[Parameter(
  Position = 2,
  ParameterSetName = 'Set 29')]
[Parameter(
  Position = 2,
  ParameterSetName = 'Set 30')]
[Parameter(
  Position = 2,
  ParameterSetName = 'Set 31')]
```

Specifies a property value.
The parameter name (-Value) is optional.
This parameter is introduced in Windows PowerShell 3.0.

#### CContains [switch]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 7')]
```

Specifies the case-sensitive Contains operator, which gets objects from a collection when the property value of the object is an exact match for the specified value
For example: Get-Process | where ProcessName -contains "svchost"
"Contains" refers to a collection of values and is true when the collection contains an item that is an exact match for the specified value.  If the input is a single object, Windows PowerShell converts it to a collection of one object.
This parameter is introduced in Windows PowerShell 3.0.

#### CEQ [switch]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 8')]
```

Specifies the case-sensitive Equals operator, which gets objects when the property value is the same as the specified value.
This parameter is introduced in Windows PowerShell 3.0.

#### CGE [switch]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 9')]
```

Specifies the case-sensitive Greater-than-or-equal value, which gets objects when the property value is greater than or equal to the specified value.
This parameter is introduced in Windows PowerShell 3.0.

#### CGT [switch]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 10')]
```

Specifies the case-sensitive Greater-than property, which gets objects when the property value is greater than the specified value.
This parameter is introduced in Windows PowerShell 3.0.

#### CIn [switch]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 11')]
```

Specifies the case-sensitive In operator, which gets objects when the property value includes the specified value.
For example: Get-Process | where -Value "svchost" -CIn ProcessName
The In operator is much like the Contains operator, except that the property and value positions are reversed. For example, the following statements are both true.
"abc", "def" -CContains "abc"
"abc" -CIn "abc", "def"
This parameter is introduced in Windows PowerShell 3.0.

#### CLE [switch]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 12')]
```

Specifies the case-sensitive Less-than-or-equal operator, which gets objects when the property value is less-than or equal to the specified value.
This parameter is introduced in Windows PowerShell 3.0.

#### CLT [switch]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 14')]
```

Specifies the case-sensitive Less-than operator, which gets objects when the property value is less-than the specified value.
This parameter is introduced in Windows PowerShell 3.0.

#### CLike [switch]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 13')]
```

Specifies the case-sensitive Like operator, which gets objects when the property value matches a value that includes wildcard characters. 
For example: Get-Process | where ProcessName -CLike "*host"
This parameter is introduced in Windows PowerShell 3.0.

#### CMatch [switch]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 15')]
```

Specifies the case-sensitive Match operator, which gets objects when the property value matches the specified regular expression. When the input is scalar, the matched value is saved in $Matches automatic variable.
For example: Get-Process | where ProcessName -CMatch "Shell"
This parameter is introduced in Windows PowerShell 3.0.

#### CNE [switch]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 16')]
```

Specifies the case-sensitive Not-Equals operator, which gets objects when the property value is different than the specified value.
This parameter is introduced in Windows PowerShell 3.0.

#### CNotContains [switch]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 17')]
```

Specifies the case-sensitive Not-Contains operator, which gets objects when the property value of the object is not an exact match for the specified value.
For example: Get-Process | where ProcessName -CNotContains "svchost"
"NotContains" and "CNotContains refer to a collection of values and are true when the collection does not contains any items that are an exact match for the specified value. If the input is a single object, Windows PowerShell converts it to a collection of one object.
This parameter is introduced in Windows PowerShell 3.0.

#### CNotIn [switch]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 18')]
```

Specifies the case-sensitive Not-In operator, which gets objects when the property value is not an exact match for the specified value.
For example: Get-Process | where -Value "svchost" -CNotIn -Property ProcessName
The Not-In  and CNot-In operators are much like the Not-Contains and CNot-Contains operators, except that the property and value positions are reversed. For example, the following statements are true.
"abc", "def" -CNotContains "Abc"
"abc" -CNotIn "Abc", "def"

#### CNotLike [switch]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 19')]
```

Specifies the case-sensitive Not-Like operator, which gets objects when the property value does not match a value that includes wildcard characters. 
For example: Get-Process | where ProcessName -CNotLike "*host"
This parameter is introduced in Windows PowerShell 3.0.

#### CNotMatch [switch]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'Set 20')]
```

Specifies the case-sensitive Not-match operator, which gets objects when the property value does not match the specified regular expression. When the input is scalar, the matched value is saved in $Matches automatic variable.
For example: Get-Process | where ProcessName -CNotMatch "Shell"
This parameter is introduced in Windows PowerShell 3.0.


### INPUTS
#### System.Management.Automation.PSObject
You can pipe the objects to be filtered to Where-Object.

### OUTPUTS
#### Object
Where-Object returns selected items from the input object set.

### NOTES
Beginning in Windows PowerShell 4.0, Where() operator behavior has changed. Collection.Where('property -match name') no longer accepts string expressions in the format "Property -CompareOperator Value". However, the Where() operator accepts string expressions in the format of a scriptblock; this is still supported. The following examples show the behavior that has changed.
The following two examples show Where() object behavior that is no longer supported.
(Get-Process).Where('ProcessName -match PowerShell')
(Get-Process).Where('ProcessName -match PowerShell', 'Last', 1)
The following three examples show Where() object behavior that is supported in Windows PowerShell 4.0 and forward.
(Get-Process).Where({$_.ProcessName -match "PowerShell"})
(Get-Process).Where{$_.ProcessName -match "PowerShell"}
(Get-Process).Where({$_.ProcessName -match "PowerShell"}, ‘Last’, 1)

### EXAMPLES
#### -------------------------- EXAMPLE 1 --------------------------

```powershell
PS C:\>Get-Service | Where-Object {$_.Status -eq "Stopped"}
PS C:\>Get-Service | where Status -eq "Stopped"

```
This command gets a list of all services that are currently stopped. The "$_" symbol represents each object that is passed to the Where-Object cmdlet.
The first command uses the script block format. The second command uses the comparison statement format. The commands are equivalent and can be used interchangeably.





#### -------------------------- EXAMPLE 2 --------------------------

```powershell
PS C:\>Get-Process | Where-Object {$_.WorkingSet -gt 25000*1024}
PS C:\>Get-Process | Where-Object WorkingSet -gt (25000*1024)

```
This command lists processes that have a working set greater than 25,000 kilobytes (KB). Because the value of the WorkingSet property is stored in bytes, the value of 25,000 is multiplied by 1,024.
The first command uses the script block format. The second command uses the comparison statement format. The commands are equivalent and can be used interchangeably.


#### -------------------------- EXAMPLE 4 --------------------------

```powershell
PS C:\>Get-Process | Where-Object {$_.ProcessName -Match "^p.*"}
PS C:\>Get-Process | Where-Object ProcessName -Match "^p.*"

```
This command gets the processes that have a ProcessName property value that begins with the letter "p". The match operator lets you use regular expression matches.
The first command uses the script block format. The second command uses the comparison statement format. The commands are equivalent and can be used interchangeably.


#### -------------------------- EXAMPLE 5 --------------------------

```powershell
PS C:\>Get-Process | Where-Object -Property Handles -ge -Value 1000
PS C:\>Get-Process | where Handles -ge 1000

```
This example shows how to use the new comparison statement  format of the Where-Object cmdlet.
The first command uses the comparison statement format. In this command, no aliases are used and all parameters include the parameter name.
The second command is the more natural use of the comparison command format. The "where" alias is substituted for the "Where-Object" cmdlet name and all optional parameter names are omitted.


#### -------------------------- EXAMPLE 6 --------------------------

```powershell
The first pair of commands gets commands that have any value for the OutputType property of the command. They omit commands that do not have an OutputType property and those that have an OutputType property, but no property value.
PS C:\>Get-Command | where OutputType
PS C:\>Get-Command | where {$_.OutputType}

The second pair of commands gets objects that are containers. It gets objects that have the PSIsContainer property with a value of True ($true) and excludes all others.The "equals $True" (-eq $true) part of the command is assumed by the language. You do not need to specify it explicitly.
PS C:\>Get-ChildItem | where PSIsContainer
PS C:\>Get-ChildItem | where {$_.PSIsContainer}

The third pair of commands uses the Not operator (!) to get objects that are not containers. It gets objects that do have the PSIsContainer property and those that have a value of False ($false) for the PSIsContainer property.You cannot use the Not operator (!) in the comparison statement format of the command.
PS C:\>Get-ChildItem | where {!$_.PSIsContainer}
PS C:\>Get-ChildItem | where  PSIsContainer -eq $false

```
This example shows how to write commands that return items that are true or false or have any value for a specified property. The example shows both the script block and comparison statement formats for the command.


#### -------------------------- EXAMPLE 7 --------------------------

```powershell
PS C:\>Get-Module -ListAvailable | where {($_.Name -notlike "Microsoft*" -and $_.Name -notlike "PS*") -and $_.HelpInfoUri}

```
This example shows how to create a Where-Object command with multiple conditions.
This command gets non-core modules that support the Updatable Help feature. The command uses the ListAvailable parameter of the Get-Module cmdlet to get all modules on the computer. A pipeline operator sends the modules to the Where-Object cmdlet, which gets modules whose names do not begin with "Microsoft" or "PS" and have  a value for the HelpInfoURI property, which tells Windows PowerShell where to find updated help files for the module. The comparison statements are connected by the -And logical operator.
The example uses the script block command format. Logical operators, such as -And and -Or, are valid only in script blocks. You cannot use them in the comparison statement format of a Where-Object command.
For more information about Windows PowerShell logical operators, see about_Logical_Operators (http://go.microsoft.com/fwlink/?LinkID=113238). For more information about the Updatable Help feature, see about_Updatable_Help (http://go.microsoft.com/fwlink/?LinkID=235801).



### RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289623)
[Compare-Object]()
[ForEach-Object]()
[Group-Object]()
[Measure-Object]()
[New-Object]()
[Select-Object]()
[Sort-Object]()
[Tee-Object]()
[Where-Object]()

## Clear-Host

### SYNOPSIS
Clears the display in the host program.

### DESCRIPTION
The Clear-Host function removes all text from the current display, including commands and output that might have accumulated. When complete, it displays the command prompt. You can use the function name or its alias, CLS.  Clear-Host affects only the current display. It does not delete saved results or remove any items from the session. Session-specific items, such as variables and functions, are not affected by this function.  Because the behavior of the Clear-Host function is determined by the host program, Clear-Host might work differently in different host programs.

### PARAMETERS


### INPUTS
#### None
           You cannot pipe input to Clear-Host.        

### OUTPUTS
#### None
           Clear-Host does not generate any output         

### NOTES
                           Clear-Host is a simple function, not an advanced function. As such, you cannot use common parameters, such as -Debug, in a Clear-Host command.                         


### EXAMPLES
#### -------------------------- EXAMPLE 1 --------------------------

```powershell
C:\PS> cls  # Before  PS C:\>Get-Process  Handles  NPM(K)    PM(K)      WS(K) VM(M)   CPU(s)     Id ProcessName -------  ------    -----      ----- -----   ------     -- -----------     843      33    14428      22556    99    17.41   1688 CcmExec      44       6     2196       4964    52     0.23    692 conhost     646      12     2332       4896    49     1.12    388 csrss     189      11     2860       7084   114     0.66   2896 csrss      78      11     1876       4008    42     0.22   4000 csrss      76       7     1848       5064    54     0.08   1028 dwm     610      41    23952      44048   208     4.40   2080 explorer       0       0        0         24     0               0 Idle     182      32     7692      15980    91     0.23   3056 LogonUI     186      25     7832      16068    91     0.27   3996 LogonUI    1272      32    11512      20432    58    25.07    548 lsass     267      10     3536       6736    34     0.80    556 lsm     137      17     3520       7472    61     0.05   1220 msdtc     447      31    70316      84476   201 1,429.67    836 MsMpEng     265      18     7136      15628   134     2.20   3544 msseces     248      16     6476       4076    76     0.22   1592 NisSrv     368      25    61312      65508   614     1.78    848 powershell     101       8     2304       6624    70     0.64   3648 rdpclip     258      15     6804      12156    50     2.65    536 services ...  PS C:\> cls #After  PS C:>                         
```

This command uses the CLS alias of Clear-Host to clear the current display.





### RELATED LINKS
[Online version:](http://technet.microsoft.com/library/hh852689(v=wps.630).aspx)
[Get-Host]()
[Out-Host]()
[Read-Host. Write-Host]()

## Get-Verb

### SYNOPSIS
Gets approved Windows PowerShell verbs.

### DESCRIPTION
The Get-Verb function gets verbs that are approved for use in Windows PowerShell commands.    Windows PowerShell recommends that cmdlet and function names have the Verb-Noun format and include an approved verb. This practice makes command names more consistent and predictable, and easier to use, especially for users who do not speak English as a first language.    Commands that use unapproved verbs run in Windows PowerShell. However, when you import a module that includes a command with an unapproved verb in its name, the Import-Module command displays a warning message.  NOTE:   The verb list that Get-Verb returns might not be complete. For an updated list of approved Windows PowerShell verbs with descriptions, see "Cmdlet Verbs" in MSDN at http://go.microsoft.com/fwlink/?LinkID=160773.

### PARAMETERS

#### Verb [string[]]

```powershell
[Parameter(
  Position = 1,
  ValueFromPipeline = $true,
  ParameterSetName = 'Set 1')]
```

Gets only the specified verbs. Enter the name of a verb or a name pattern. Wildcards are permitted.


### INPUTS
#### None

### OUTPUTS
#### Selected.Microsoft.PowerShell.Commands.MemberDefinition

### NOTES
                           Get-Verb returns a modified version of a Microsoft.PowerShell.Commands.MemberDefinition object. The object does not have the standard properties of a MemberDefinition object. Instead it has Verb and Group properties. The Verb property contains a string with the verb name. The Group property contains a string with the verb group.  Windows PowerShell verbs are assigned to a group based on their most common use. The groups are designed to make the verbs easy to find and compare, not to restrict their use. You can use any approved verb for any type of command.  Each Windows PowerShell verb is assigned to one of the following groups.  -- Common: Define generic actions that can apply to almost any cmdlet, such as Add. -- Communications:  Define actions that apply to communications, such as Connect. -- Data:  Define actions that apply to data handling, such as Backup. -- Diagnostic: Define actions that apply to diagnostics, such as Debug. -- Lifecycle: Define actions that apply to the lifecycle of a cmdlet, such as Complete. -- Security: Define actions that apply to security, such as Revoke. -- Other: Define other types of actions.  Some of the cmdlets that are installed with Windows PowerShell, such as Tee-Object and Where-Object, use unapproved verbs. These cmdlets are considered to be historic exceptions and their verbs are classified as "reserved."                          


### EXAMPLES
#### -------------------------- EXAMPLE 1 --------------------------

```powershell
C:\PS> get-verb                        
```

This command gets all approved verbs.



#### -------------------------- EXAMPLE 2 --------------------------

```powershell
C:\PS> get-verb un*  Verb                 Group ----                 ----- Undo                 Common Unlock               Common Unpublish            Data Uninstall            Lifecycle Unregister           Lifecycle Unblock              Security Unprotect            Security                        
```

This command gets all approved verbs that begin with "un".




#### -------------------------- EXAMPLE 3 --------------------------

```powershell
C:\PS> get-verb | where-object {$_.Group -eq "Security"}  Verb                 Group ----                 ----- Block                Security Grant                Security Protect              Security Revoke               Security Unblock              Security Unprotect            Security                        
```
This command gets all approved verbs in the Security group.


#### -------------------------- EXAMPLE 4 --------------------------

```powershell
C:\PS> get-command -module MyModule | where { (get-verb $_.Verb) -eq $null }                        
```

This command finds all commands in a module that have unapproved verbs.



#### -------------------------- EXAMPLE 5 --------------------------

```powershell
C:\PS> $approvedVerbs = get-verb | foreach {$_.verb}  C:\PS> $myVerbs = get-command -module MyModule | foreach {$_.verb}  # Does MyModule export functions with unapproved verbs? C:\PS> ($myVerbs | foreach {$approvedVerbs -contains $_}) -contains $false True  # Which unapproved verbs are used in MyModule? C:\PS>  ($myverbs | where {$approvedVerbs -notcontains $_}) ForEach Sort Tee Where                         
```

These commands detect unapproved verbs in a module and tell which unapproved verbs were detected in the module.





### RELATED LINKS
[Online version:](http://technet.microsoft.com/library/hh852690(v=wps.630).aspx)
[Import-Module]()


