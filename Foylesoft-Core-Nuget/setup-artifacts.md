# FoyleSoft.AzureCore

## Project setup

Ensure you have installed the latest version of the Azure Artifacts credential provider from the "Get the tools" menu.
Add a nuget.config file to your project, in the same folder as your .csproj or .sln file

nuget pack "YOUR-FOLDER-PATH/Foylesoft-Core-Nuget.nuspec"

<code>
    <pre>
        &lt;add key="FoyleSoftNugetArtifact" value="https://pkgs.dev.azure.com/foylesoftnuget/FoylesoftCoreNuget/_packaging/FoyleSoftNugetArtifact/nuget/v3/index.json" /&gt;
    </pre>
</code>

## Restore packages

Run this command in your project directory

<pre>
nuget.exe restore
</pre>

## Publish packages

Publish a package by providing the package path, an API Key (any string will do), and the feed URL

<pre>
nuget.exe push -Source "FoyleSoftNugetArtifact" -ApiKey az &lt;packagePath&gt;
</pre>

## Push Command

nuget push -Source https://pkgs.dev.azure.com/foylesoftnuget/FoylesoftCoreNuget/_packaging/FoyleSoftNugetArtifact/nuget/v3/index.json -ApiKey az "{YOUR-PROJECT-WORKING-DIRECTORY}/FoyleSoftPackages/FoyleSoft.AzureCore.0.0.0.1.nupkg"
