# Import API Samples

This repository demonstrates how the Import API can be used to import the following:

* Native documents
* Images
* Objects
* Productions

All of the samples are based on [NUnit](https://nunit.org/), they follow the [AAA](https://docs.microsoft.com/en-us/visualstudio/test/unit-test-basics?view=vs-2017#write-your-tests) test structure pattern, and app.config settings are used to define test parameters including Relativity Urls and login credentials. Once these settings have been made, the tests are 100% responsible for all required setup and tear down procedures. Finally, the Relativity.Import.Client.Samples.NUnit C# test project relies on [Relativity NuGet packages](https://www.nuget.org/packages?q=Relativity) to ensure Import API and all dependencies are deployed to the proper target directory.

This page contains the following information:

* [Prerequisites](#prerequisites)
* [Setup](#setup)
* [Import API class](#importapi-class)
  * [Authentication](#authentication)
  * [Jobs](#jobs)
  * [Data Source](#data-source)
  * [Events](#events)
  * [Execute](#execute)
* [Sample Jobs](#sample-jobs)
  * [Import documents with native files](#import-documents-with-native-files)
  * [Import data into RDOs](#import-data-into-rdos)
  * [Import images](#import-images)
  * [Import productions](#import-productions)

## Prerequisites

* Visual Studio 2017
* NUnit Test Adapter
* A bluestem (9.7) or above test environment
* Visual C++ 2010 x86 Runtime
* Visual C++ 2015 x64 Runtime

**Note:** Relativity strongly recommends [using a developer Dev VM](https://platform.relativity.com/9.6/Content/Relativity_Platform/Testing_custom_applications.htm?Highlight=vm#_Developer_test_VMs) for a given release.

## Setup
The steps below should only be required when the repository is being cloned for the first time.

## Step 1 - Install NUnit 3 test adapter
If Visual Studio hasn't already been setup with NUnit 3 Test Adapter, go to `Tools->Extensions and Updates` and install the extension.

![NUnit Test Adapter](Documentation/NUnitTestAdapter.png "NUnit Test Adapter")

## Step 2 - Identify the branch
With each new Relativity release, a new branch is created to not only test and verify the Import API package but to ensure all package references are updated properly. As of this writing, the following three releases are available for consideration:

![Sample Branches](Documentation/Branches.png "Sample Branches")

## Step 3 - Clone the repository
Use the command-line or Visual Studio to clone the repository and target a branch from the previous step.

```bash
git clone -b release-9.7-bluestem https://github.com/relativitydev/import-api-samples.git
```

## Step 4 - Open the solution
Launch Visual Studio 2017 and open the Relativity.Import.Client.Samples.NUnit.sln solution file.

## Step 5 - Update app settings
Double-click the app.config file and update the following underneath the `appSettings` element:

| Setting                  | Description                                                               | Example                                             |
|--------------------------|---------------------------------------------------------------------------|-----------------------------------------------------|
| RelativityUrl            | The Relativity instance URL.                                              | https://hostname.mycompany.corp                     |
| RelativityRestUrl        | The Relativity Rest API URL.                                              | https://hostname.mycompany.corp/relativity.rest/api |
| RelativityServicesUrl    | The Relativity Services API URL.                                          | https://hostname.mycompany.corp/relativity.services |
| RelativityWebApiUrl      | The Relativity Web API URL.                                               | https://hostname.mycompany.corp/relativitywebapi    |
| RelativityUserName       | The Relativity login user name.                                           | email@company.com                                   |
| RelativityPassword       | The Relativity login password.                                            | SomePassword!                                       |
| WorkspaceTemplate        | The workspace template used to create a test workspace for each test run. | Relativity Starter Template                         |
| SqlDropWorkspaceDatabase | Specify whether to drop the SQL database when the test completes.         | True|False                                          |
| SqlInstanceName          | The SQL instance where the workspace databases are stored.                | hostname.mycompany.corp\EDDSINSTANCE001             |
| SqlAdminUserName         | The SQL system administrator user name.                                   | sa                                                  |
| SqlAdminPassword         | The SQL system administrator password.                                    | SomePassword!                                       |

**Note:** The SQL parameters are optional and generally reserved to cleanup Dev VM's as soon as the tests are completed.

## Step 6 - Test settings
In order to run the NUnit tests from within Visual Studio, the test settings must target X64. Go to `Test->Test Settings->Default Processor Architecture` and verify this is set to X64.

![Test Settings](Documentation/TestSettings.png "Test Settings")

## Step 7 - Execute tests
At this point, the setup is complete and should now be able to run all of the tests. If the test explorer isn't visible, go to `Test->Windows->Test Explorer` and click the `Run All` hyper-link at the top. Regardless of which method is used to execute tests, the following actions occur:

* A new test workspace is created (normally 15-30 seconds) to ensure tests are isolated
* The tests are executed
* The test workspace is deleted
* The Test Explorer displays the results

If everything is working properly, the Test Explorer should look something like this:

![Test Explorer](Documentation/TestExplorer.png "Test Explorer")

## ImportAPI class
The `ImportAPI` class is a top-level class that includes functionality for importing documents, images, production sets, and Relativity Dynamic Objects (RDOs). It includes methods for performing these import jobs, as well as other methods for retrieving workspace, field, and other objects.

### WebAPI
The Import API design is based on [Web API](https://www.asp.net/web-api) and the proper URL must be supplied when constructing the object.

* https://hostname.mycompany.corp (Relativity instance)
* https://hostname.mycompany.corp/relativitywebapi (Relativity Web API)

### Authentication
When constructing the `ImportAPI` object, the API caller must first decide which authentication model is used in order to determine how the object is constructed.

#### Relativity username and password authentication
The user must be a member of the System Administrators group in Relativity. These permissions are similar to those required to import a load file through the Relativity Desktop Client.

```csharp
ImportAPI importApi = new ImportAPI(relativityUserName, relativityPassword, relativityWebAPIUrl);
```

#### Bearer token authentication
Uses the current claims principal token to authenticate and should only be used by Relativity Service Account hosted processes.

```csharp
ImportAPI importApi = ImportAPI.CreateByRsaBearerToken(relativityWebAPIUrl);
```

**Note:** This is the preferred method for using Import API within an agent or custom page.

#### Windows authentication
The user is validated against the Relativity Web API instance located at WebServiceURL.

```csharp
ImportAPI importApi = new ImportAPI(relativityWebAPIUrl);
```

### Jobs
Given the `ImportAPI` object, the API caller calls a method to create the appropriate job object.

```csharp
const int SomeObjectArtifactTypeId = 1111111;
const int ProductionSetArtifactId = 2222222;
kCura.Relativity.DataReaderClient.ImportBulkArtifactJob job = importApi.NewNativeDocumentImportJob();
kCura.Relativity.DataReaderClient.ImportBulkArtifactJob job = importApi.NewObjectImportJob(SomeObjectArtifactTypeId);
kCura.Relativity.DataReaderClient.ImageImportBulkArtifactJob job = importApi.NewImageImportJob();
kCura.Relativity.DataReaderClient.ImageImportBulkArtifactJob job = importApi.NewProductionImportJob(ProductionSetArtifactId);
```

#### ImportBulkArtifactJob Settings
When configuring either native document or object import jobs, the `Settings` property exposes a number of options to control import behavior.

<details><summary>View settings</summary>

| Setting                                     | Description                                                                                                                  |
|---------------------------------------------|------------------------------------------------------------------------------------------------------------------------------|
| ArtifactTypeId                              | The target object artifact type identifier.                                                                                  |
| Billable                                    | Indicates whether imported files are billable.                                                                               |
| BulkLoadFileFieldDelimiter                  | The field delimiter used when writing out the bulk load file.                                                                |
| CaseArtifactId                              | The target workspace artifact identifier.                                                                                    |
| CopyFilesToDocumentRepository               | Indicates whether to copy files to the document repository.                                                                  |
|                                             | * If True, the files are copied.                                                                                             |
|                                             | * If False, files will be linked instead.                                                                                    |
| DisableControlNumberCompatibilityMode       | Indicates whether to enable or disable the use of 'Control Number' to override the SelectedIdentifierField.                  |
|                                             | * If True, tries to use "Control Number" for the SelectedIdentifierField and ignores SelectedIdentifierField.                |
|                                             | * If False, and SelectedIdentifierField is not set, uses the default identifier field.                                       |
| DisableExtractedTextEncodingCheck           | Indicates whether to enable or disable encoding checks for each file.                                                        |
|                                             | * If True, encoding checks are disabled.                                                                                     |
|                                             | * If False, encoding checks are disabled.                                                                                    |
| DisableExtractedTextFileLocationValidation  | Indicates whether to enable or disable validation of the extracted text file location.                                       |
|                                             | * If True, validation is disabled. If an extracted text file doesn't exist, the job fails.                                   |
|                                             | * If False, validation is enabled.                                                                                           |
| DisableNativeLocationValidation             | Indicates whether to enable or disable validation of the native file path.                                                   |
|                                             | * If True, validation is disabled.                                                                                           |
|                                             | * If False, validation is enabled.                                                                                           |
| DisableNativeValidation                     | Indicates whether to enable or disable validation of the native file type for the current job.                               |
|                                             | * If True, validation is disabled.                                                                                           |
|                                             | * If False, validation is enabled.                                                                                           |
| DisableUserSecurityCheck                    | Indicates whether to enable or disable user permission checks per document or object                                         |
|                                             | * If True, user permission checks are disabled.                                                                              |
|                                             | * If False, validation checks are enabled.                                                                                   |
| ExtractedTextEncoding                       | The extracted text file encoding.                                                                                            |
| ExtractedTextFieldContainsFilePath          | Indicates whether the extracted text field contains a path to the extracted text file or contains the actual extracted text. |
|                                             | * If True, the extracted text field contains a path to the extracted text file.                                              |
|                                             | * If False, the extracted text field contains the actual extracted text.                                                     |
| FileSizeColumn                              | The column that contains the FileSize on the `SourceData` property.                                                          |
| FileSizeMapped                              | Indicates whether to enable or disable skipping file size checks.                                                            |
|                                             | * If True, the file size is mapped and `OIFileIdColumnName` and `FileSizeColumn` must be mapped.                             |
|                                             | * If False, the file size isn't mapped.                                                                                      |
| FolderPathSourceFieldName                   | The metadata field used to build the folder structure Rest API URL.                                                          |
|                                             | **Note**  All folders are built under the Import Destination folder, indicated by the DestinationFolderArtifactID.  If a     |
|                                             |           folder matching the entered string already exists, the documents will be added to it; otherwise, the folder(s)     |
|                                             |           (including nested folders) will be created and the documents will be imported into the new folder(s).              |
| IdentityFieldId                             | The key field that's set only on Overwrite mode.                                                                             |
| LoadImportedFullTextFromServer              | Indicates whether the Extracted Text field data will be loaded directly from its file path vs. part of a bulk load file.     |
|                                             | **Note**  If True, extracted text files must exactly match the encoding of the extracted text field.                         |
|                                             |           If extracted text is unicode enabled, the files need to be UTF-16 encoded,  otherwise they need to be ANSI.        |
|                                             |           This setting will only be used when `ExtractedTextFieldContainsFilePath` is also set to True.                      |
| MaximumErrorCount                           | The maximum number of errors displayed. This property is optional.                                                           |
| MoveDocumentsInAppendOverlayMode            | Indicates whether existing documents should be moved to a new folder for Append/Overlay mode.                                |
|                                             | * If True, the documents are moved to a new folder for Append/Overlay mode.                                                  |
|                                             | * If False, the documents are not moved                                                                                      |
| NativeFileCopyMode                          | The native file copy behavior.                                                                                               |
|                                             | * DoNotImportNativeFiles: documents are imported without their native files.                                                 |
|                                             | * CopyFiles: native files are copied into the workspace.                                                                     |
|                                             | * SetFileLinks: Link to the native files but don't copy them                                                                 |
| NativeFilePathSourceFieldName               | The name of the field that contains the full path and filename for the native files.                                         |
| OIFileIdColumnName                          | The name of the field that contains the `OutsideInFileId` on the data source object.                                         |
|                                             | **Note**  If `OIFileIdMapped` or `OIFileIdMapped` is True, set this property to the value that indicates the field that      |
|                                             |           contains the `OutsideInFileId` value.                                                                              |
| OIFileIdMapped                              | Indicates whether to enable or disable file identification.                                                                  |
|                                             | **Note**  If True, both `OIFileIdColumnName` and `OIFileIdColumnName` must be set.                                           |
| OIFileTypeColumnName                        | The name of the field that contains the Outside In file type on the data source.                                             |
|                                             | **Note**  If `OIFileIdMapped` is True, this field must be set.                                                               |
| OverwriteMode                               | The import overwrite behavior.                                                                                               |
|                                             | * Append: Import all files even if that causes duplication. This is faster than Append/Overlay mode.                         |
|                                             | * Overlay: Update all files if new versions are made available by the import.                                                |
|                                             | * AppendOverlay: Import all files. Those that are duplicates will be updated to the new version of the file.                 |
| SelectedIdentifierFieldName                 | The name of the field used as an identifier.                                                                                 |
|                                             | **Note**  If this identifier cannot be resolved, the control number will be used in its place.                               |
| StartRecordNumber                           | The record number from which to start the importL.                                                                           |

</details>

#### ImageImportBulkArtifactJob Settings
When configuring either image or production import jobs, the `Settings` property exposes a number of options to control import behavior.

<details><summary>View settings</summary>

| Setting                                     | Description                                                                                                                  |
|---------------------------------------------|------------------------------------------------------------------------------------------------------------------------------|
| ArtifactTypeId                              | The target object artifact type identifier.                                                                                  |
| AutoNumberImages                            | Indicates whether a page number is automatically appended to a page-level identifier.                                        |
|                                             | * If True, a new incremental number (such as 01, 02) is added to the page-level identifier to create a unique page number.   |
|                                             | * If False, the page number isn't appended.                                                                                  |
| BatesNumberField                            | The name of the field that defines the unique identifier.                                                                    |
|                                             | **Note**  This unique identifier may be called `Bates Number` or `Control Number` in a database.                             |
| Billable                                    | Indicates whether imported files are billable.                                                                               |
| CaseArtifactId                              | The target workspace artifact identifier.                                                                                    |
| CopyFilesToDocumentRepository               | Indicates whether to copy files to the document repository.                                                                  |
|                                             | * If True, the files are copied.                                                                                             |
|                                             | * If False, files will be linked instead.                                                                                    |
| DisableExtractedTextEncodingCheck           | Indicates whether to enable or disable encoding checks for each file.                                                        |
|                                             | * If True, encoding checks are disabled.                                                                                     |
|                                             | * If False, encoding checks are enabled.                                                                                     |
| DisableImageLocationValidation              | Indicates whether to enable or disable image location validation.                                                            |
|                                             | * If True, validation checks are disabled.                                                                                   |
|                                             | * If False, validation checks are enabled.                                                                                   |
| DisableImageTypeValidation                  | Indicates whether to enable or disable image type validation.                                                                |
|                                             | * If True, validation checks are disabled.                                                                                   |
|                                             | * If False, validation checks are enabled.                                                                                   |
| DisableUserSecurityCheck                    | Indicates whether to enable or disable user permission checks per document or object                                         |
|                                             | * If True, user permission checks are disabled.                                                                              |
|                                             | * If False, validation checks are enabled.                                                                                   |
| DocumentIdentifierField                     | The name of the field that corresponds to the `DocumentIdentifier` field                                                     |
| ExtractedTextEncoding                       | The extracted text file encoding.                                                                                            |
| ExtractedTextFieldContainsFilePath          | Indicates whether the extracted text field contains a path to the extracted text file or contains the actual extracted text. |
|                                             | * If True, the extracted text field contains a path to the extracted text file.                                              |
|                                             | * If False, the extracted text field contains the actual extracted text.                                                     |
| FileLocationField                           | The name of the field that corresponds with the `FileLocation` field.                                                        |
| FolderPathSourceFieldName                   | The metadata field used to build the folder structure Rest API URL.                                                          |
|                                             | **Note**  All folders are built under the Import Destination folder, indicated by the DestinationFolderArtifactID.  If a     |
|                                             |           folder matching the entered string already exists, the documents will be added to it; otherwise, the folder(s)     |
|                                             |           (including nested folders) will be created and the documents will be imported into the new folder(s).              |
| IdentityFieldId                             | The key field that's set only on Overwrite mode.                                                                             |
| ImageFilePathSourceFieldName                | The name of the field that contains the full path to the image file.                                                         |
| LoadImportedFullTextFromServer              | Indicates whether the Extracted Text field data will be loaded directly from its file path vs. part of a bulk load file.     |
|                                             | **Note**  If True, extracted text files must exactly match the encoding of the extracted text field.                         |
|                                             |           If extracted text is unicode enabled, the files need to be UTF-16 encoded,  otherwise they need to be ANSI.        |
|                                             |           This setting will only be used when `ExtractedTextFieldContainsFilePath` is also set to True.                      |
| MaximumErrorCount                           | The maximum number of errors displayed. This property is optional.                                                           |
| MoveDocumentsInAppendOverlayMode            | Indicates whether existing documents should be moved to a new folder for Append/Overlay mode.                                |
|                                             | * If True, the documents are moved to a new folder for Append/Overlay mode.                                                  |
|                                             | * If False, the documents are not moved                                                                                      |
| NativeFileCopyMode                          | The native file copy behavior.                                                                                               |
|                                             | * DoNotImportNativeFiles: documents are imported without their native files.                                                 |
|                                             | * CopyFiles: native files are copied into the workspace.                                                                     |
|                                             | * SetFileLinks: Link to the native files but don't copy them                                                                 |
| OverlayBehavior                             | The method for overlay imports with multiple choice and multi-object fields                                                  |
|                                             | * UseRelativityDefaults: each field will be imported based on its overlay behavior settings in Relativity.                   |
|                                             | * MergeAll: new imported values will be added to all imported fields.                                                        |
|                                             | * ReplaceAll: all the imported fields previous values will all be overwritten with the imported values.                      |
| OverwriteMode                               | The import overwrite behavior.                                                                                               |
|                                             | * Append: Import all files even if that causes duplication. This is faster than Append/Overlay mode.                         |
|                                             | * Overlay: Update all files if new versions are made available by the import.                                                |
|                                             | * AppendOverlay: Import all files. Those that are duplicates will be updated to the new version of the file.                 |
| SelectedIdentifierFieldName                 | The name of the field used as an identifier.                                                                                 |
|                                             | **Note**  If this identifier cannot be resolved, the control number will be used in its place.                               |
| StartRecordNumber                           | The record number from which to start the importL.                                                                           |

</details>

### Data Source
No matter which job is created, the API caller uses standard [ADO.NET](https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/ado-net-overview) constructs like `System.Data.IDataReader` and `System.Data.DataTable` to define data sources.

```csharp
// Once the data source is defined, the DataColumn names are mapped via the job Settings object.
System.Data.DataTable dataSource = new System.Data.DataTable();
dataSource.Columns.AddRange(new[]
{
    new DataColumn("control number", typeof(string)),
	new DataColumn("file path", typeof(string))
});

dataSource.Rows.Add("REL-4444444", @"C:\temp\sample.pdf");
job.Settings.SelectedIdentifierFieldName = "control number";
job.SourceData.SourceData = dataSource.CreateDataReader();
```

### Events
The `ImportBulkArtifactJob` or `ImageImportBulkArtifactJob` objects expose a number of useful events to obtain document-level errors, completion, fatal exception, and progress.

#### JobReport
This object is exposed by several events and provides useful import details.

| Property                      | Description                                                                                                          |
| ------------------------------| ---------------------------------------------------------------------------------------------------------------------|
| EndTime                       | The import end time.                                                                                                 |
| ErrorRowCount                 | The total number of non-fatal document-level errors that occurred.                                                   |
| ErrorRows                     | The collection of non-fatal document-level errors that occurred.                                                     |
| FatalException                | The exception that resulted in a fatal job error.                                                                    |
| FieldMap                      | The collection of field map entries that map source fields to destination fields in the workspace.                   |
| FileBytes                     | The total number of transferred native file bytes.                                                                   |
| MetadataBytes                 | The total number of transferred metadata bytes.                                                                      |
| StartTime                     | The import start time.                                                                                               |
| TotalRows                     | The total number of processed rows. This value doesn't indicate the number of successful rows.                       |

#### OnComplete
This event is published when an import job is finished. A `JobReport` object is passed with detailed information about the job. A completed job may have errors if the data wasn't imported properly.

```csharp
// This event provides the JobReport object.
job.OnComplete += report =>
{
   Console.WriteLine("The job has completed.");
};
```

#### OnError
This event is published when an error occurs while importing a row of data.

```csharp
// This event provides an IDictionary object with well-known parameters.
job.OnError += row =>
{
    Console.WriteLine(row["Line Number"]);
    Console.WriteLine(row["Identifier"]);
    Console.WriteLine(row["Message"]);
};
```

**Note:** The MaximumErrorCount is a configurable setting available on all import jobs that determines the number of errors to return.

#### OnFatalException
This event is published when an import job encounters a fatal exception caused by invalid import settings or other issues. The fatal exception can be retrieved by the passed `JobReport` object.

```csharp
// This event provides the JobReport object.
job.OnFatalException += report =>
{
    Console.WriteLine("The job experienced a fatal exception: " + report.FatalException);
};
```

#### OnMessage
This event is published throughout the import job life cycle and is similar to the messages displayed in the Relativity Desktop Client.

```csharp
// This event provides the Status object.
job.OnMessage += status =>
{
    Console.WriteLine("Job message: " + status.Message);
};
```

#### OnProcessProgress
This event is published at the same rate as the `OnMessage` event and provides detailed progress information about the import job.

```csharp
// This event provides the FullStatus object.
job.OnProcessProgress += status =>
{
    Console.WriteLine("Job start time: " + status.StartTime);
    Console.WriteLine("Job end time: " + status.EndTime);
    Console.WriteLine("Job process ID: " + status.ProcessID);
    Console.WriteLine("Job total records: " + status.TotalRecords);
    Console.WriteLine("Job total records processed: " + status.TotalRecordsProcessed);
    Console.WriteLine("Job total records processed with warnings: " + status.TotalRecordsProcessedWithWarnings);
    Console.WriteLine("Job total records processed with errors: " + status.TotalRecordsProcessedWithErrors);
    Console.WriteLine("Job total records: " + status.TotalRecordsDisplay);
    Console.WriteLine("Job total records processed: " + status.TotalRecordsProcessedDisplay);
    Console.WriteLine("Job status suffix: " + status.StatusSuffixEntries);
};
```

#### OnProgress
This event is published for each row found in the data set.

```csharp
// This event provides the row number.
job.OnProgress += row =>
{
    Console.WriteLine("Job progress line number: " + row);
};
```

### Execute
Once the job has been fully configured, the API caller simply calls the `Execute` method. As soon as this occurs, events are published and the method returns when the job either completes or a fatal exception occurs.

```csharp
// Wait for the job to complete.
job.Execute();
```

## Sample Jobs
The section below outlines each of the import job test samples.

### Import documents with native files
* The [DocImportTests](Source/Relativity.Import.Client.Samples.NUnit/Tests/DocImportTests.cs "DocImportTests")  imports sample documents.
* The [DocImportFolderTests](Source/Relativity.Import.Client.Samples.NUnit/Tests/DocImportFolderTests.cs "DocImportFolderTests") imports sample documents and also specifies folder paths. 
* The [DocNegativeImportTests](Source/Relativity.Import.Client.Samples.NUnit/Tests/DocNegativeImportTests.cs "DocNegativeImportTests") imports documents that expect job-level import failures.

### Import data into RDOs
* The [ObjectSimpleImportTests](Source/Relativity.Import.Client.Samples.NUnit/Tests/ObjectSimpleImportTests.cs "ObjectSimpleImportTests") imports a custom object with a single-object field.
* The [ObjectAdvancedImportTests](Source/Relativity.Import.Client.Samples.NUnit/Tests/ObjectAdvancedImportTests.cs "ObjectAdvancedImportTests") imports a custom object with a multi-object field.
* The [ObjectNegativeImportTests](Source/Relativity.Import.Client.Samples.NUnit/Tests/ObjectNegativeImportTests.cs "ObjectNegativeImportTests") imports single-object and multi-object fields that expect document-level and job-level import failures.

**Note:** The tests create custom RDO types during the test setup.

### Import images
* The [ImageImportTests](Source/Relativity.Import.Client.Samples.NUnit/Tests/ImageImportTests.cs "ImageImportTests") imports sample images.

### Import productions
* The [ProductionImportTests](Source/Relativity.Import.Client.Samples.NUnit/Tests/ProductionImportTests.cs "ProductionImportTests") imports sample documents, creates a production, and then validates the bates numbers.

**Note:** This test relies upon the [Productions NuGet package](https://www.nuget.org/packages/Relativity.Productions.Client/) to perform all required functionality.