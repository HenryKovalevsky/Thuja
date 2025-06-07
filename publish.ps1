param(
    [Parameter(Mandatory=$true)]
    [string]$Version,
    
    [Parameter(Mandatory=$true)]
    [string]$ApiKey
)

$outputDir = Join-Path $PWD "publish"
$projects = @(
    "src\Thuja\Thuja.fsproj",
    "src\Thuja.Tutu\Thuja.Tutu.fsproj"
)

# build
foreach ($project in $projects) {
    $projectPath = Join-Path $PWD $project
    dotnet pack $projectPath -c Release -p:PackageVersion=$Version --output $outputDir
}

# publish
$nupkgFiles = Get-ChildItem $outputDir -Filter *.nupkg
foreach ($nupkg in $nupkgFiles) {
    dotnet nuget push $nupkg.FullName --api-key $ApiKey --source https://api.nuget.org/v3/index.json --skip-duplicate
}