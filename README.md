# Sitecore.Ship

[![Join the chat at https://gitter.im/kevinobee/Sitecore.Ship](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/kevinobee/Sitecore.Ship?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

Sitecore.Ship is a lightweight means to install Sitecore Update packages via HTTP requests.


## Instructions for Use

The latest release versions of the Sitecore.Ship packages can be found on the NuGet gallery:

* [Sitecore.Ship](http://www.nuget.org/packages/Sitecore.Ship/) 
* [Sitecore.Ship.AspNet](http://www.nuget.org/packages/Sitecore.Ship.AspNet/) 


### Building the Package

* Clone this repository to your local file system

* You will need to specify the location of your Sitecore assemblies. This can be down in the `build\build.proj` file. Set the LibsSrcPath to where your Sitecore assemblies are located.

* From a command prompt type `.\build` and press Enter

### Installing the Package

Ensure that the website project is set to run with .NET Framework 4.5

Ensure that the web.config file for the target project if already existing is included into your project.

Run the following powershell command in the package manager console of the Visual Studio solution for the target website:

    install-package Sitecore.Ship -Source <path>

Where `<path>` is the path to the  `artifacts\Packages\` folder that was produced by the build command.

Installing the package will do the following:

* Add a new `packageInstallation` section to your `web.config` file. You can set configuration options in this section to enable remote access to the installer and to enable the package streaming functionality. These options are safe by default, that is, no remote access and package streaming disabled. **Note:** the configuration settings are ignored in this branch of Sitecore.Ship.

* Register a single new HTTP handler section in `<system.webserver>`. Support for classic mode in IIS has been removed.

* Add a `ship.config` Sitecore include file to the `App_Config\include` folder.

### Uninstalling the Package

Run the following commands:

    uninstall-package Sitecore.Ship

    uninstall-package Nancy.Hosting.Aspnet

    uninstall-package Nancy


### Using the Installer

#### Install Package - Specify Server File Path

Issue a POST request to `/services/package/install` and pass it a path parameter in the x-www-form-urlencoded form-data specifying the location of the update package.

Example:

    POST /services/package/install HTTP/1.1
    Host: shiptester
    Cache-Control: no-cache
    
    ----WebKitFormBoundaryE19zNvXGzXaLvS5C
    Content-Disposition: form-data; name="path"
    
    d:\package.update
    ----WebKitFormBoundaryE19zNvXGzXaLvS5C


When the package is installed correctly a 201 Created HTTP Status code will be returned.

    Content-Length 108
    Content-Type application/json
    Date Sun, 14 Jul 2013 07:44:50 GMT
    Location: http://shiptest/services/package/latestversion

The body of a successfull request will contain details of the package contents in JSON format. For example:

     {"Entries":[{"ID":"110d559f-dea5-42ea-9c1c-8a5df7e70ef9","Path":"addeditems/master/sitecore/content/home"}]}

The request also takes an optional `DisableIndexing` parameter in the x-www-form-urlencoded form-data which defaults to *false*. When the parameter is set to *true* updating of search indexes during the package installation will be suspended. Disabling the search index updates will increase the speed at which packages are installed into the CMS. You can read more about this approach on Alex Shyba's 
[blog](http://sitecoreblog.alexshyba.com/2010/04/sitecore-installation-wizard-disable.html "Sitecore Installation Wizard � disable search index update during install")
 


#### Install Package - File Upload

Issue a POST request to `/services/package/install/fileupload` and pass it the location of an update package file to upload to the server.

Example:

    POST /services/package/install/fileupload HTTP/1.1
    Host: shiptester
    Accept: application/json, text/javascript, */*
    Cache-Control: no-cache
    
    ----WebKitFormBoundaryE19zNvXGzXaLvS5C
    Content-Disposition: form-data; name="path"; filename="package.update"
    Content-Type: 
    
    
    ----WebKitFormBoundaryE19zNvXGzXaLvS5C


The request also accepts an optional `DisableIndexing` parameter. For details of this and information on the command's response format refer to the `Install Package - Specify Server File Path` above.

*Note* that if you have `recordInstallationHistory` enabled you will need to provide `PackageId` and `Description` form parameters in the request that you make.


#### Package - Latest Version

Issue a GET request to `/services/package/latestversion` to get details of the last package to have been installed by the Ship services.

Note that latest version reporting is disabled by default. Refer to the `recordInstallationHistory` setting in the Configuration Options section.



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


Issue a GET request to `/services/publish/lastcompleted/{source}/{target}/{language}` to review the timestamp of the last publish operation. The `{source}`, `{target}` and `{language}` parameters are optional and will default to `master`, `web` and `en` respectively if not specified in the request.

To publish a list of items

Issue a Post to '/services/publish/listofitems' with JSON  like the following:
	
	{
    
		"Items": [
					 {
						"itemId" : "{DEDCC8EB-F542-449C-9EFA-4F248EDB900B}",
						"PublishChildren": "false"
					},
					 {
						"itemId" : "{CE0A2933-AB9B-4131-ABD2-DE7C2A69FBB9}",
						"PublishChildren": "false"
					}
				 ],
			

		"TargetDatabases": [
							"#TargetDatabase#"
						   ],
		"TargetLanguages": [
							"en"
						   ]
        
   
	}

#### About

Issue a GET request to `/services/about`


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

### Tools

POSTMAN is a powerful HTTP client that runs as a Chrome browser extension and greatly helps with testing test REST web services. Find out more <http://www.getpostman.com/> 

References:

http://curl.haxx.se/docs/httpscripting.html - see section 4.3 File Upload POST 


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