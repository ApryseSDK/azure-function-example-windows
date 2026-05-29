# Azure Function Example for Windows
This project provides a sample Azure Function that demonstrates how to convert Microsoft Word documents into PDF format using the Apryse PDFNet SDK. The function is invoked via an HTTP trigger and secured with function-level authentication. Designed for a Windows-based environment, this solution serves as a reference implementation for secure, serverless document processing workflows.

# Included Projects
The sample solution includes three separate projects, one for the Azure function app, one console application for testing the function, and a share library for common classes between the function app and console test. 
* azure-function-ap-windows
  * The Azure function to be published to your Azure account. The sample expects function level authentication with the code applied to the function URL. (This can be adjusted to your required authentication.)
* azure-function-console-test
  * The console test will upload the file you are converting to an Azure blob storage for the fuction to convert, execute the Azure function call, and then download the output file from a specified Azure blob storage.
  * The function URL, blob storage connection string, and both input and output containers must be specified.
* azure-function-share-library
  * This is a shared library that includes commen settings used in both the function and test. This includes helper functions to upload, download, and delete files from Azure storage as well as commen settings.

# Publishing

# Extension Modules

# Azure Service Plans
