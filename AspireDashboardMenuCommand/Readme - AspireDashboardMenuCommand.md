# Aspire Dashboard Menu Commands

This documentation explains how to add custom menu commands to your Aspire dashboard services. The functionality is provided by the `AspireDashboardMenuCommand` class located in the `Extensions` folder.

## Prerequisites

- .NET Aspire project
- Service with endpoints (implements `IResourceWithEndpoints`)
- Using statements:
  ```csharp
  using Aspire.Hosting;
  using YourApp.AppHost.Extensions;
  ```

## Overview

The Aspire Dashboard Menu Commands feature allows you to:
- Add clickable menu items to any service in your Aspire dashboard
- Configure icons using the Fluent UI System Icons
- Open URLs when menu items are clicked
- Automatically integrate with Aspire's service discovery to resolve endpoint URLs

## Usage

### Adding Menu Commands

Use the `AddUrlCommand` method to add a menu command to any service:

```csharp
AspireDashboardMenuCommand.AddUrlCommand(
    service,
    name: "swagger-docs",
    displayName: "Swagger Documentation",
    url: "swagger",
    iconName: "Document Table",
    iconVariant: IconVariant.Regular);
```

### Parameters

- `service`: The service to add the command to
- `name`: Unique identifier for the command
- `displayName`: Text shown in the UI
- `url`: URL to open when clicked
- `iconName`: Name of the Fluent UI System icon
- `iconVariant`: Regular or Filled icon style
- `appendToServiceUrl`: Whether to append to service URL or use as absolute

### URL Behavior

- When `appendToServiceUrl = true` (default):
  - The URL is appended to the service's base URL
  - Example: `{service-url}/swagger`
  - Good for endpoints hosted by the service

- When `appendToServiceUrl = false`:
  - The URL is used as-is
  - Example: `https://graphql.myapi.com/docs`
  - Good for external documentation or resources 

## Icons

### Using Fluent UI System Icons

1. Browse available icons at https://fluenticons.co/
2. Copy the icon name (spaces are handled automatically)
3. Use in your code

Example icon names:
- `"Document Table"`
- `"Document Toolbox"`
- `"Document Add"`
- `"Document Text"`
- `"Settings"`
- `"Home"`
- `"Apps"`

Icon names are automatically formatted:
- Spaces are removed
- Each word is capitalized
- Example: `"document table"` becomes `"DocumentTable"`

### Icon Variants

Each icon can be used in two styles:
- `IconVariant.Regular` (outlined version)
- `IconVariant.Filled` (filled version)

## Example

Here's a complete example showing how to add documentation commands to an API service:

```csharp
using Aspire.Hosting;
using YourApp.AppHost.Extensions;

var builder = DistributedApplication.CreateBuilder(args);

// Add API service with documentation commands
var apiService = builder.AddProject<ApiService>("apiservice");

// Add Swagger documentation (appends to service URL)
AspireDashboardMenuCommand.AddUrlCommand(
    apiService,
    name: "swagger-docs",
    displayName: "Swagger Documentation",
    url: "swagger",
    iconName: "Document Table",
    iconVariant: IconVariant.Regular);

// Add Scalar documentation (appends to service URL)
AspireDashboardMenuCommand.AddUrlCommand(
    apiService,
    name: "scalar-docs",
    displayName: "Scalar Documentation",
    url: "scalar/v1",
    iconName: "Document Text",
    iconVariant: IconVariant.Filled);

// Add GraphQL documentation (uses absolute URL)
AspireDashboardMenuCommand.AddUrlCommand(
    apiService,
    name: "graphql-docs",
    displayName: "GraphQL Documentation",
    url: "https://graphql.myapi.com/docs",
    iconName: "Document Toolbox",
    iconVariant: IconVariant.Regular,
    appendToServiceUrl: false);

await builder.Build().RunAsync();
```

## Best Practices

1. **Naming Conventions**
   - Use consistent naming patterns for commands (e.g., `"{type}-docs"`)
   - Keep display names clear and concise
   - Use appropriate icons that match the command's purpose

2. **URL Management**
   - Use `appendToServiceUrl: true` for service-hosted endpoints
   - Use `appendToServiceUrl: false` for external resources
   - Ensure URLs are valid and accessible

3. **Icon Usage**
   - Choose icons that clearly represent the command's function
   - Be consistent with icon variants across similar commands
   - Test icons in both light and dark themes

## Command Behavior

### Service Discovery Integration
Commands automatically leverage Aspire's service discovery to resolve the correct endpoint URLs at runtime. This means you don't need to hardcode service URLs - they are dynamically resolved based on where the service is actually running.

### Health Check Integration
Commands are automatically disabled when the service is unhealthy. This prevents users from accessing endpoints that may not be available.

### Command Display Order
Commands appear in the order they are added to the service. Consider this when organizing related commands.

## Troubleshooting

### Common Issues

1. **Command Not Appearing**
   - Verify the service implements `IResourceWithEndpoints`
   - Check that the using statements are correct
   - Ensure the service is properly registered with the builder

2. **URL Not Opening**
   - Check if the service is healthy
   - Verify the URL is correctly formatted
   - For appended URLs, ensure the service has a valid endpoint

3. **Icon Not Displaying**
   - Verify the icon name matches one from https://fluenticons.co/
   - Check that the icon name is properly capitalized
   - Try both Regular and Filled variants
