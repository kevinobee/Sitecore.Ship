# Sitecore.Ship

[![Join the chat at https://gitter.im/kevinobee/Sitecore.Ship](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/kevinobee/Sitecore.Ship?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

Sitecore.Ship is a lightweight means to install Sitecore Update packages via HTTP requests.


## API Documentation

[The Sitecore.Ship documentation can be found on Apiary.io](http://docs.sitecoreship.apiary.io/)


## Instructions for Use

The latest release versions of the Sitecore.Ship packages can be found in the [NuGet package gallery](https://www.nuget.org/packages/)

* [Sitecore.Ship](http://www.nuget.org/packages/Sitecore.Ship/) 
* [Sitecore.Ship.AspNet](http://www.nuget.org/packages/Sitecore.Ship.AspNet/) 
 
Early releases of the Sitecore.Ship packages can be found on a public feed from [MyGet](https://www.myget.org/)

* [NuGet V3 feed - Visual Studio 2015+](https://www.myget.org/F/sitecore-ship-prerelease/api/v3/index.json)
* [NuGet V2 feed - Visual Studio 2012+](https://www.myget.org/F/sitecore-ship-prerelease/api/v2)

### Installing Sitecore.Ship via NuGet

Ensure that the website project is set to run with .NET Framework 4.5

Ensure that the web.config file for the target project if already existing is included into your project.

Run the following powershell command in the package manager console of the Visual Studio solution for the target website:

    PM:> install-package Sitecore.Ship

or 

    PM:> install-package Sitecore.Ship.AspNet


Installing the package will do the following:

* Add a new `packageInstallation` section to your `web.config` file. You can set configuration options in this section to enable remote access to the installer and to enable the package streaming functionality. These options are safe by default, that is, no remote access and package streaming disabled. **Note:** the configuration settings are ignored in this branch of Sitecore.Ship.

* Register a single new HTTP handler section in `<system.webserver>`. Support for classic mode in IIS has been removed.

* Add a `ship.config` Sitecore include file to the `App_Config\include` folder.


### Configuration Options

Shown below is a fully specified configuration section for Sitecore.Ship:

    <packageInstallation enabled="true" allowRemote="true" allowPackageStreaming="true" recordInstallationHistory="true">
      <Whitelist>
        <add name="local loopback" IP="127.0.01" />
        <add name="Allowed machine 1" IP="10.20.3.4" />
        <add name="Allowed machine 2" IP="10.40.4.5" />
      </Whitelist>
    </packageInstallation>


Default configuration:

* enabled = false
* allowRemote = false
* allowPackageStreaming = false
* recordInstallationHistory = false
* IP address whitelisting is disabled if no elements are specified below the `<Whitelist>` element or if the element is omited.

When `recordInstallationHistory` has been set to true packages should follow the naming conventions set out below:

Packages should follow the following naming conventions. Id should be an int.

    {ID}-DescriptiveName.{Extension}

where:

* **ID** should be an integer
* **Extension** should be either *update* or *zip*

For example:

    01-AboutPage.update

    02-HomePage.zip

### Uninstalling Sitecore.Ship

Run the following commands:

    PM:> uninstall-package Sitecore.Ship

	
## Working with the Sitecore.Ship Source Code

* Clone this repository to your local file system

* You will need to specify the location of your Sitecore assemblies. This can be down in the `build\build.proj` file. Set the LibsSrcPath to where your Sitecore assemblies are located.

* From a command prompt type `.\build` and press Enter

A successful command line build will generate a NuGet package in the `artifacts\Packages\` folder.


## Tools

* [POSTMAN](http://www.getpostman.com/) is a powerful HTTP client that runs as a Chrome browser extension and greatly helps with testing test REST web services. 

References:

* [cUrl Documentation](http://curl.haxx.se/docs/httpscripting.html) - see section 4.3 File Upload POST 


## Contributing to the Project

If you are interested in contributing to the growth and development of Sitecore.Ship in even a small way, please read the notes below.

The project can be built and tested from the command line by entering:

    .\build

Please ensure that there are no compilation or test failures and no code analysis warnings are being reported.

### Running the Smoke Tests

The `build.proj` file contains a set of smoke tests to verify that the Sitecore.Ship features all run successfully when the package has been installed in a Sitecore website.

In order to run these smoke tests you will need to:

* Have a local install of Sitecore.

* Set the *TestWebsitePath* and *TestWebsiteUrl* in the **build\environment.props** to reference the local Sitecore website.

* Ensure that the test website has the Ship package installed by running the following in the Package Manager Console:

    install-package Sitecore.Ship -Source `<path to folder containing your sitecore.ship nupkg file>`

You can then run the smoke tests by entering:

    .\build RunSmokeTests

A series of curl commands fire off HTTP requests to the Sitecore.Ship service routes and the results are printed out to the console. Each of these commands should execute successfully before you send a pull request back to the main project.

Your participation in the project is very much welcomed.
