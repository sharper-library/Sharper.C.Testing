$TestsRegex = '\.Tests$'

function AllProjects() {
    Get-ChildItem */project.json
}

function PackageProjects() {
    AllProjects | Where {$_.Directory.Name -notmatch $TestsRegex}
}

function TestProjects() {
    AllProjects | Where {$_.Directory.Name -match $TestsRegex}
}

function CleanCmd() {
    AllProjects | %{$_.Directory} | %{
        if (Test-Path $_/bin) {Remove-Item -Recurse $_/bin}
        if (Test-Path $_/obj) {Remove-Item -Recurse $_/obj}
    }
    if (Test-Path artifacts) {Remove-Item -Recurse artifacts}
}

function InstallCmd() {
    dotnet restore
}

function BuildCmd() {
    Write-Host "Building projects:"
    PackageProjects | %{Write-Host "   $_"}
    if ($env:BUILD_BUILDNUMBER) {
      $env:DOTNET_BUILD_VERSION = $env:BUILD_BUILDNUMBER
    }
    else {
      $env:DOTNET_BUILD_VERSION = 'z'
    }
    PackageProjects | %{
      Write-Host "Building $_"
      dotnet pack $_.Directory -c Release
    }
}

function TestCmd() {
    $codes = (TestProjects) | %{dotnet test $_ | Write-Host; $LASTEXITCODE}
    $code = ($codes | Measure-Object -Sum).Sum
    exit $code
}

function RegisterCmd() {
    PackageProjects | %{
        Get-ChildItem -Recurse *.nupkg | %{
            nuget add $_ -Source "$env:USERPROFILE/.nuget/packages"
        }
    }
}

function RunCommand($name) {
    switch ($name) {
        clean {CleanCmd}
        install {InstallCmd}
        build {BuildCmd}
        test {TestCmd}
        register {RegisterCmd}
        all {CleanCmd; InstallCmd; BuildCmd; RegisterCmd}
    }
}

$args | %{RunCommand $_}
