## Web API Tester

## Overview

Web API Tester is a C# application designed to simplify the process of testing web APIs. It provides a user-friendly interface for sending requests to web services and validating the responses. This tool is ideal for developers, QA engineers, and testers who need to automate and streamline API testing workflows.

## Features

- **Support for RESTful APIs**: Easily send GET, POST, PUT, DELETE, PATCH, and other HTTP requests.
- **Custom Headers and Parameters**: Add custom headers, query parameters, and body content to your requests.
- **Response Validation**: Automatically check HTTP content and validate.
- **Authentication Support**: Handle API authentication using Bearer Tokens.
- **Export and Import Test Results**: Export test results in TXT format for reporting.
- **Automated Testing**: Schedule API tests to run at specified intervals.
- **Environment Management**: Define and manage multiple environments (e.g., Development, Staging, Production).
- **Error Handling and Logs**: Detailed error logs to troubleshoot failed requests.
- **Integration with CI/CD**: Integrate the tool with your continuous integration and deployment pipelines.

## Installation

## Prerequisites

- .NET SDK (version 6.0 or later)
- A supported operating system (Windows, Linux, macOS)


## Steps to Install

1. Clone the repository:
   ```bash
   git clone https://github.com/Gor903/WEB-API-Tester.git
   ```
2. Navigate to project directory.
   ```bash
   cd WEB-API-Tester
   ```
3. Restore packages manually (optional but recommended).
   ```bash
   dotnet restore
   ```
4. Build the project.
   ```bash
   dotnet build
   ```
5. Run the project.
   ```bash
   dotnet run $<LogLevel>(default info)
   ```
   ### Logging levels
      - **Trace**: trce
      - **Debug**: dbug
      - **Information**: info
      - **Warning**: warn
      - **Error**: fail
      - **Critical**: crit


## Configuration

You can configure default headers, validation methods, and other preferences by modifying the `Tester/<filename>.json` file in the **Tests** directory.

### Creating a Configuration File

Create a `config.json` file in the root directory of the project with the following content:

```json
{
  "LogDirectory": "Global logs directory",
  "TestConfigsPath": "Test files' directory",
  "TestGroups": [
    {
      "Group": "Group1",
      "LogDirectory": "Group logs directory", // if null -> Global logs directory
      "Url": "Base url", // example http://127.0.0.1:8000
      "Ignore": "False", // True or False
      "Files": [
        "CreateUser",    // Test file name (without .json)
        "Login"
      ]
    },
    {
      "Group": "Group2",
      "Url": "Base url",
      "Ignore": "False",
      "Files": [
        "CreateCourse",
        "GetCourseByID"
      ]
    }
  ]
}
```


### Creating a Test File

Create a `<filename>.json` file in the Tests directory of the project with the following content:

```json
{
  "Method": "POST", // GET, POST, PUT, PATCH, DELETE
  "Endpoint": "Url's endpoint", // users
  "Content": {
    "username": "admin",
    "email": "admin@example.com",
    "password": "My$up3rpass",
    "full_name": "Admin"
  },
  "Headers": {
    "key": "value"
  },
  "Query": {
    "key": "value"
  }
  "Expected": {
    "*id": "id",
    "username": "admin",
    "email": "admin@example.com",
    "full_name": "Admin"
  }
}
```
