using System.Diagnostics;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Globalization;

namespace YourApp.AppHost.Extensions
{
    /// <summary>
    /// Helper class to add menu commands to any Aspire dashboard service.
    /// Provides functionality to add URL-based commands with icons to the Aspire dashboard's service context menus.
    /// </summary>
    /// <remarks>
    /// This class provides the core functionality for adding individual menu commands to Aspire dashboard services.
    /// Each command can open a URL and display an icon from the Fluent UI System Icons set.
    /// 
    /// Icons:
    /// This class uses Fluent UI System Icons. Browse available icons at https://fluenticons.co/
    /// 
    /// Icon names are automatically formatted:
    /// - Spaces are removed
    /// - Each word is capitalized
    /// Examples:
    /// - "add square" becomes "AddSquare"
    /// - "document table" becomes "DocumentTable"
    /// - "API management" becomes "ApiManagement"
    /// - "text DOCUMENT" becomes "TextDocument"
    /// 
    /// You can copy names directly from the website - they will be formatted automatically.
    /// 
    /// Common Working Icons:
    /// - "Document Table"
    /// - "Document Toolbox"
    /// - "Document Add"
    /// - "Document Text"
    /// - "Settings"
    /// - "Home"
    /// - "Apps"
    /// 
    /// Each icon can be used with:
    /// - IconVariant.Regular (outlined version)
    /// - IconVariant.Filled (filled version)
    /// </remarks>
    /// <example>
    /// Add a simple URL command:
    /// <code>
    /// service.AddUrlCommand(
    ///     name: "swagger-docs",
    ///     displayName: "Swagger Documentation",
    ///     url: "swagger",
    ///     iconName: "Document Table",
    ///     iconVariant: IconVariant.Regular);
    /// </code>
    /// 
    /// Add a command with a full URL:
    /// <code>
    /// service.AddUrlCommand(
    ///     name: "external-docs",
    ///     displayName: "External Documentation",
    ///     url: "https://docs.example.com",
    ///     iconName: "Document Text",
    ///     appendToServiceUrl: false);
    /// </code>
    /// </example>
    public static class AspireDashboardMenuCommand
    {
        /// <summary>
        /// Formats an icon name by removing spaces and ensuring proper capitalization.
        /// </summary>
        private static string FormatIconName(string iconName)
        {
            if (string.IsNullOrWhiteSpace(iconName)) return "Document";

            // Split the string into words and capitalize each word
            var words = iconName.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                              .Select(word => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(word.ToLower()));

            // Join the words back together
            return string.Concat(words);
        }

        /// <summary>
        /// Adds a URL-based command to the service's context menu in the Aspire dashboard.
        /// </summary>
        /// <typeparam name="T">The type of resource builder, must implement IResourceWithEndpoints.</typeparam>
        /// <param name="service">The service to add the command to.</param>
        /// <param name="name">Unique identifier for the command.</param>
        /// <param name="displayName">User-friendly name shown in the UI.</param>
        /// <param name="url">The URL to open when the command is executed.</param>
        /// <param name="iconName">Name of the Fluent UI System icon to display (spaces are automatically removed).</param>
        /// <param name="iconVariant">Whether to use the Regular or Filled version of the icon.</param>
        /// <param name="appendToServiceUrl">If true, appends the URL to the service's base URL. If false, uses the URL as-is.</param>
        /// <returns>The service builder for chaining.</returns>
        public static IResourceBuilder<T> AddUrlCommand<T>(
            this IResourceBuilder<T> service,
            string name,
            string displayName,
            string url,
            string iconName = "Document",
            IconVariant iconVariant = IconVariant.Filled,
            bool appendToServiceUrl = true) where T : IResourceWithEndpoints
        {
            // Format the icon name
            var formattedIconName = FormatIconName(iconName);

            service.WithCommand(
                name: name,
                displayName: displayName,
                executeCommand: async context =>
                {
                    try
                    {
                        string finalUrl = url;
                        if (appendToServiceUrl)
                        {
                            var endpoint = service.GetEndpoint("https");
                            if (endpoint?.Url == null)
                            {
                                return new ExecuteCommandResult { Success = false, ErrorMessage = "Service URL not found" };
                            }
                            finalUrl = $"{endpoint.Url}/{url}";
                        }

                        await Task.Run(() => Process.Start(new ProcessStartInfo(finalUrl) { UseShellExecute = true }));
                        return new ExecuteCommandResult { Success = true };
                    }
                    catch (Exception ex)
                    {
                        return new ExecuteCommandResult { Success = false, ErrorMessage = $"Failed to open URL: {ex.Message}" };
                    }
                },
                updateState: context => context.ResourceSnapshot.HealthStatus == HealthStatus.Healthy
                    ? ResourceCommandState.Enabled
                    : ResourceCommandState.Disabled,
                iconName: formattedIconName,
                iconVariant: iconVariant);

            return service;
        }
    }
} 