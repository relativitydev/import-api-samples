FormatTaskName "------- Executing Task: {0} -------"

properties {
    $ArtifactsDir = Join-Path $Root "Logs"
    $NumberOfProcessors = (Get-ChildItem env:"NUMBER_OF_PROCESSORS").Value
    $Injections = "EnableInjections"
    $MasterSolution = Join-Path $root "Source/Relativity.Import.Client.Samples.NUnit.sln"

    # Properties below this line are defined in build.ps1
    $BuildType = $Null
    $Release = $Null
    $AssemblyVersion = $Null
    $Verbosity = $Null
    $ServerType = $Null
    $Rebuild = $Null
    $SkipBuild = $Null
    $TestTimeoutInMS = $Null
}

task default -Depends Build -Precondition { -not $SkipBuild }

task Build -Description "Builds the source code" -Depends BuildInitialize, UpdateAssemblyInfo, CompileMasterSolution, CopyPDBs {

}

task Initialize -Description "Prepare variables for the build." {

    $script:Injections = if ($BuildType.ToUpper() -ne "DEV") {"DisableInjections"} else {$Injections}
    Write-Verbose "Injections set to: $script:Injections"

    $script:BuildConfig = if ($Release) {"Release"} else {"Debug"}
    Write-Verbose "BuildConfig set to: $script:BuildConfig"
}

task BuildInitialize -Description "Prepare to build the sample and restore NuGet packages." -Depends Initialize {
    
    Write-Verbose "Restoring NuGet packages for $MasterSolution"
    exec { & $NuGetEXE restore $MasterSolution }
}

task UpdateAssemblyInfo -Precondition { $AssemblyVersion -ne "1.0.0.0" } -Description "Update the AssemblyInfo files in \Version\" {
    $VersionPath = Join-Path $Root "Version"
    $ScriptPath = Join-Path $VersionPath "Update-AssemblyInfo.ps1"
    exec { & $ScriptPath -assembly_version $AssemblyVersion -path_to_version_folder $VersionPath }
}

task CompileMasterSolution {
    $LogFilePath = Join-Path $ArtifactsDir "buildsummary.log"
    $ErrorFilePath = Join-Path $ArtifactsDir "builderrors.log"
    $MSBuildTarget = if ($Rebuild) {"rebuild"} else {"build"}

    Initialize-Folder $ArtifactsDir -Safe
    exec { msbuild $MasterSolution `
        "/t:$MSBuildTarget" `
        "/verbosity:$Verbosity" `
        "/property:Configuration=$script:BuildConfig" `
        "/property:Injections=$script:Injections" `
        "/property:PublishWebProjects=True" `
        "/clp:Summary"`
        "/nodeReuse:False" `
        "/nologo" `
        "/maxcpucount" `
        "/flp1:LogFile=`"$LogFilePath`";Verbosity=$Verbosity" `
        "/flp2:errorsonly;LogFile=`"$ErrorFilePath`""
    }
}

task CopyPDBs -Description "Copy all PDBs in the repository into the PDBs folder" {
    $PdbFolder =  "$Root\PDBs\"

    Initialize-Folder -Path $PdbFolder
    Get-ChildItem -Recurse -ErrorAction SilentlyContinue -Filter '*.pdb' -Path $Root |
        Sort-Object -Unique -Property Name |
        Select-Object -ExpandProperty FullName |
        Copy-Item -Destination $PdbFolder -Force
}

task BuildMasterPackage -Description "Builds the MasterPackage folder, which contains additional code & installers used for deployments and CI builds." {

    New-Item -ItemType File -Path (Join-Path $script:PackageDir "BuildType_$BuildType") -Force | Write-Verbose
    $MasterPackageDirectory = Join-Path $script:PackageDir "MasterPackage"
    Import-Module (Join-Path $Root "kCura\DevelopmentScripts\RelativityPackaging.psm1") -Force
    $MasterPackageParameters = @{
        Source = $Root
        Destination = $MasterPackageDirectory
        BuildType = $BuildType
        Version = $AssemblyVersion
        Verbose = $VerbosePreference
    }

    New-MasterPackage @MasterPackageParameters
}

task UnitTest -Description "Run NUnit" -Depends Initialize, NUnit {

}

task NUnit -Description "Run NUnit on Master solution" {
    $NUnit3 = Join-Path $BuildToolsDir "NUnit.ConsoleRunner.3.9.0\tools\nunit3-console.exe"
    $LogPath = Join-Path $ArtifactsDir "Test_Output.xml"

    Initialize-Folder $ArtifactsDir -Safe

    exec { & $NUnit3 $MasterSolution `
        "--noheader" `
        "--labels=On" `
        "--domain=Single" `
        "--process=Multiple" `
        "--agents=$NumberOfProcessors" `
        "--skipnontestassemblies" `
        "--timeout=$TestTimeoutInMS"
    }
}

task Clean -Description "Delete build artifacts" -Depends Initialize {
    Initialize-Folder $ArtifactsDir
    
    Write-Verbose "Running Clean target on $MasterSolution"
    exec { msbuild $MasterSolution `
        "/t:Clean" `
        "/verbosity:$Verbosity" `
        "/property:Configuration=$script:BuildConfig" `
        "/nologo" `
    }
}

task Help -Alias ? -Description "Display task information" {
    WriteDocumentation
}

Function Initialize-Folder {
    param(
        [Parameter(Mandatory=$true, Position=0)]
        [String] $Path,
        [Parameter()]
        [switch] $Safe
    )

    if ((Test-Path $Path) -and $Safe)
    {
        Return
    }

    if (Test-Path $Path)
    {
        Remove-Item -Recurse -Force $Path -ErrorAction Stop
    }

    New-Item -Type Directory $Path -Force -ErrorAction Stop -Verbose:$VerbosePreference
}