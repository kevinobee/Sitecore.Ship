# Sitecore.Ship

Sitecore.Ship is a lightweight means to install Sitecore Update packages via HTTP requests.


## Instructions for Use

This is an early proof of concept and as such packages have not been pushed to NuGet.org

### Building the Package

* Clone this repository to your local file system

* You will need to specify the location of your Sitecore assemblies. This can be down in the `build\build.proj` file. Set the LibsSrcPath to where your Sitecore assemblies are located.

* From a command prompt type `.\build` and press Enter

### Installing the Package

Ensure that the website project is set to run with .NET Framework 4.0

Run the following powershell command in the package manager console of the Visual Studio solution for the target website:

    install-package Sitecore.Ship -Source <path>

Where `<path>` is the path to the  `artifacts\Packages\` folder that was produced by the build command.

Installing the package will do the following:

* Add a new `packageInstallation` section to your `web.config` file. You can set configuration options in this section to enable remote access to the installer and to enable the package streaming functionality. These options are safe by default, that is, no remote access and package streaming disabled. **Note:** the configuration settings are ignored in this branch of Sitecore.Ship.

* Register a single new HTTP handler section in `<system.web>` and `<system.webserver>`

* Add a `ship.config` Sitecore include file to the `App_Config\include` folder.

### Uninstalling the Package

Run the following commands:

    uninstall-package Sitecore.Ship

    uninstall-package Nancy.Hosting.Aspnet

    uninstall-package Nancy


### Using the Installer

#### Update Package - File Install

Issue a POST request to `/services/package/install` and pass it a path parameter in x-www-form-urlencoded form-data specifying the location of the update package.

Example:

    POST /services/package/install HTTP/1.1
    Host: shiptester
    Cache-Control: no-cache
    
    ----WebKitFormBoundaryE19zNvXGzXaLvS5C
    Content-Disposition: form-data; name="path"
    
    d:\foo.update
    ----WebKitFormBoundaryE19zNvXGzXaLvS5C


#### Update Package - Package Streaming

Not implemented yet.


#### Publishing

Issue a POST request to `/services/publish/{mode}` where {mode} is 

* full
* smart
* incremental

The publishing source, targets and languages can be specified as form parameters. These will default to `master`, `web` and `en` respecitely if not present in the form-data.

Example:

    POST /services/publish/full HTTP/1.1
    Host: shiptester
    Cache-Control: no-cache
    
    ----WebKitFormBoundaryE19zNvXGzXaLvS5C
    Content-Disposition: form-data; name="source"
    
    master
    ----WebKitFormBoundaryE19zNvXGzXaLvS5C
    Content-Disposition: form-data; name="targets"
    
    web, target2
    ----WebKitFormBoundaryE19zNvXGzXaLvS5C
    Content-Disposition: form-data; name="languages"
    
    en, da, de-DE
    ----WebKitFormBoundaryE19zNvXGzXaLvS5C


#### About

Issue a GET request to `/services/about`


### Tools

POSTMAN is a powerful HTTP client that runs as a Chrome browser extension and greatly helps with testing test REST web services. Find out more <http://www.getpostman.com/> 

References:

http://curl.haxx.se/docs/httpscripting.html - see section 4.3 File Upload POST 

