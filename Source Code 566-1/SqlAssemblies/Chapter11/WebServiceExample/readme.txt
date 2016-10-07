Building and Configuring the CityImageViewer
============================================

Before viewing the city images downloaded from the TerraService-USA web service,
you will need to build and configure the CityImageViewer Windows Forms application.


To build the application, run the following command, ensuring that the folder where
the .NET 2.0 version of csc.exe resides is included in your PATH environment variable:

csc /t:winexe /out:CityImageViewer.exe CityImageViewer.cs Program.cs CityImageViewer.Designer.cs

This will create a Windows application called CityImageViewer.exe.


The CityImageViewer can be run without further configuration only if it's running on
the same machine as SQL Server, and is to connect using Windows mode authentication.
If you wish to run it remotely, or want to use SQL Server authentication, you will need
to modify the connection string in the CityImageViewer.exe.config file.

The connection string is included in the 'value' attribute in the line:

<add key="connString" value="Data Source=(local);Initial Catalog=AdventureWorks;Trusted_connection=true" />

To use a different connection string, simply change the value of this attribute.
