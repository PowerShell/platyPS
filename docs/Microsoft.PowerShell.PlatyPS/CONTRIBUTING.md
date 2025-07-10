# Contributing to platyPS

## Get the code

```
git clone https://github.com/PowerShell/platyPS
```

## Understand code layout

TBD

## Build

To build the whole project, use the `build.ps1` helper script. It depends on the
[dotnet cli](https://docs.microsoft.com/en-us/dotnet/core/tools/) build tool.

On Windows you would also need to
[install full dotnet framework](https://docs.microsoft.com/en-us/dotnet/framework/install/guide-for-developers)
if it's not installed already.

```powershell
.\build.ps1
```

As part of the build, platyPS generates help for itself. The output of the build is placed in
`out\Microsoft.PowerShell.PlatyPS`.

`build.ps1` also imports the module from `out\platyPS` and generates help itself.

> [!NOTE]
> If you changed C# code, `build.ps1` will try to overwrite a DLL in use. You will then need to
> re-open your PowerShell session. If you know a better workflow, please suggest it in the Issues.

## Tests

Each part of the project has a test set:

- The C# part has xUnit tests.
- The PowerShell part has [Pester](https://github.com/pester/Pester) tests. You can run them with
  `Invoke-Pester`.

> [!NOTE]
> Pester tests always force-import the module from the output location of `.\build.ps1`.

## Schema

If you have ideas or concerns about the Markdown schema, feel free to open a GitHub Issue to discuss
it.

## Repo structure

TBD
