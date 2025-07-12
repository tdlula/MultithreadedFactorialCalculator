# Multithreaded Factorial Calculator

A C# console application for efficiently calculating factorials using multithreading. The app features a user-friendly menu, configurable thread limits, batch and single calculations, graceful shutdown, and structured logging.

## Features
- Calculate single or multiple factorials (batch mode)
- Multithreaded computation for performance
- Configurable thread limit
- Graceful shutdown on Ctrl+C
- Console logging using Microsoft.Extensions.Logging

## How It Works
- On startup, the app detects your CPU core count and sets a default thread limit.
- The main menu allows you to:
  - Calculate a single factorial
  - Calculate multiple factorials in batch
  - Change the thread limit
  - Exit the application
- Factorial calculations are performed in parallel using a thread pool.
- Progress and results are displayed in the console.
- Pressing Ctrl+C triggers a graceful shutdown, allowing running tasks to complete and resources to be cleaned up.
- All key events and errors are logged to the console.

## Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)

## How to Build and Run
1. **Clone the repository:**
   ```sh
   git clone <your-repo-url>
   cd MultithreadedFactorialCalculator
   ```
2. **Restore dependencies:**
   ```sh
   dotnet restore
   ```
3. **Build the project:**
   ```sh
   dotnet build
   ```
4. **Run the application:**
   ```sh
   dotnet run --project MultithreadedFactorialCalculator/MultithreadedFactorialCalculator.csproj
   ```

## Example Usage
- **Calculate a single factorial:**
  1. Select option 1 from the menu.
  2. Enter a number between 1 and 170.
- **Batch calculation:**
  1. Select option 2.
  2. Enter numbers separated by commas (e.g., `5,10,15`).
- **Change thread limit:**
  1. Select option 3.
  2. Enter a new thread limit within the recommended range.
- **Exit:**
  - Select option 4 or press Ctrl+C.

## Logging
- The application logs important events and errors to the console with timestamps.

---

Feel free to contribute or open issues for suggestions and improvements!
