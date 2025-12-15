# DndInator Project

## Overview
DndInator is a Blazor WebAssembly application designed to provide an interactive experience for users interested in Dungeons & Dragons. This project is structured to separate the client-side application from shared code, allowing for modular development.

## Project Structure
- **DndInator**: The main Blazor WebAssembly project.
  - **wwwroot**: Contains static files, including the main HTML entry point.
  - **Pages**: Contains Razor components for the application.
  - **Program.cs**: The entry point for the Blazor application.
  - **DndInator.csproj**: Project file defining dependencies and settings.
  
- **DndInator.Shared**: A shared project containing code that can be used by both the client and server.
  - **DndInator.Shared.csproj**: Project file for the shared code.

## Deployment
The application is deployed to GitHub Pages using a GitHub Actions workflow defined in `.github/workflows/deploy.yml`. This workflow automates the build and deployment process to the `gh-pages` branch.

## Getting Started
To run the project locally, ensure you have the .NET SDK installed. You can then build and run the application using the following commands:

```bash
dotnet build DndInator/DndInator.csproj
dotnet run --project DndInator/DndInator.csproj
```

Visit `https://localhost:5001` in your browser to view the application.

## Contributing
Contributions are welcome! Please feel free to submit a pull request or open an issue for any enhancements or bug fixes.

## License
This project is licensed under the MIT License. See the LICENSE file for more details.