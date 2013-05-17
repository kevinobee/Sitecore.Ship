# Sitecore.Ship

Sitecore.Ship is a lightweight means to install Sitecore Update packages via HTTP requests.


## Instructions for Use

This is an early proof of concept and as such packages have not been pushed to NuGet.org

### Building the Package

* Clone this repository to your local file system

* You will need to specify the location of your Sitecore assemblies. This can be down in the `build\build.proj` file. Set the LibsSrcPath to where your Sitecore assemblies are located.

* From a command prompt type `.\build` and press Enter

### Installing the Package

Run the following powershell command in the package manager console of the Visual Studio solution for the target website:

    install-package Sitecore.Ship -Source <path>

Where <path> is the path to the  `artifacts\Packages\Sitecore.Ship.x.x.xxxx.nupkg` that was produced by the build command.

If you wish to remove jthe package run the  following command:

    uninstall-package Sitecore.Ship

Installing the package will do the following:

* Add a new `packageInstallation` section to your `web.config` file. You can set configuration options in this section to enable remote access to the installer and to enable the package streaming functionality. These options are safe by default, that is, no remote access and package streaming disabled.

* Register a single new HTTP handler section in `<system.web>` and `<system.webserver>`

* Add a `ship.config` Sitecore include file to the `App_Config\include` folder.

### Using the Installer

#### GET Request - File Install

http://domain/install/installer.ashx?package=D:\path_to_update_file\your_package.update

#### POST Request - Package Streaming

Issue a POST request to the HTTP Handler and add upload the update package in the form data as per RFC 1867.

http://domain/install/installer.ashx

References:

http://curl.haxx.se/docs/httpscripting.html - see section 4.3 File Upload POST 

The repository comes with a simple console application and a command script to help you stream packages to the installer.

    .\artifacts\Build\Ship -p .\yourpackage.update -u http://domain/install/installer.ashx