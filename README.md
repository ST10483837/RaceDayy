# RaceDayy — Full-Stack Event Management System

## Overview
RaceDayy is a comprehensive web-based event management platform designed for the South African road running, walking, and cycling community. This repository contains the final Part 3 implementation, bringing together a RESTful API, an MVC frontend, secure Azure cloud storage, and Docker containerisation.

## Features Implemented
- **Role-Based Access Control:** Distinct dashboards and permissions for Admins, Organizers, and Participants using ASP.NET Core Identity.
- **Event Management:** Organizers can Create, Read, Update, and Delete (CRUD) events.
- **Cloud Image Storage:** Integrated Azure Blob Storage to securely upload and display event banner images.
- **Categories & Enrolments:** Organizers can define race categories (e.g., 42km, 21km), and Participants can enroll in them.
- **Modern UI:** A custom purple, blue, and grey sporting theme with colour-coded status badges (Green for Registered, Amber for Pending).
- **API Documentation:** Fully interactive Swagger UI for testing all backend endpoints.
- **Automated Testing:** xUnit tests covering authentication and event logic.

## Technologies Used
- **Backend:** ASP.NET Core 8.0 (MVC + Web API)
- **Database:** Entity Framework Core (Code-First) with SQL Server
- **Authentication:** ASP.NET Core Identity
- **Cloud Storage:** Azure Blob Storage (`Azure.Storage.Blobs`)
- **Containerisation:** Docker
- **Testing:** xUnit, Moq, EF Core InMemory
- **CI/CD:** GitHub Actions

## Setup Instructions

### Prerequisites
- Visual Studio 2022
- .NET 8.0 SDK
- SQL Server (LocalDB or Express)
- Docker Desktop (for containerisation)

### Running Locally
1. Clone the repository.
2. Open `RaceDayy.sln` in Visual Studio.
3. Update the `DefaultConnection` string in `appsettings.json` to point to your SQL Server.
4. Add your Azure Storage Connection String to the `AzureStorage` section in `appsettings.json`.
5. Run the migrations:
   ```powershell
   Update-Database
6. Press F5 to run the application.
7. Default Test Users
Role	Email	Password
Admin	admin@admin.com	Admin1@
Organizer	organiser@raceday.com	Organizer@1
Participant	participant@raceday.com	Participant@1

Docker Setup

To run this application inside a Docker container, ensure Docker Desktop is running and execute the following commands in the root directory:

1. Build the Docker image:

bash
docker build -t racedayy-app .

2.Run the container:

bash
docker run -d -p 8080:8080 --name racedayy-container racedayy-app

3.Open your browser and navigate to: http://localhost:8080

CI/CD Status
A GitHub Actions workflow is configured to automatically build the project and run unit tests on every push.

<img width="1866" height="868" alt="image" src="https://github.com/user-attachments/assets/0107f504-541e-43ba-a3fa-7b70a5440c79" />


Video Walkthrough
Watch the Part 3 Walkthrough Here

Author
Student Number: ST10483837

Module: PROG6212

Institution: Rosebank International University


