// Canadian Professional Counsellors Association Application Suite
// Copyright (C) 2024 Doug Wilson
//
// This program is free software: you can redistribute it and/or modify it under the terms of
// the GNU Affero General Public License as published by the Free Software Foundation, either
// version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this
// program. If not, see <https://www.gnu.org/licenses/>.

using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Kiota.Http.HttpClientLibrary;

/// <summary>
/// Service collection extensions for Kiota handlers.
/// </summary>
public static class KiotaServiceCollectionExtensions
{
    /// <summary>
    /// Adds the Kiota handlers to the service collection.
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/> to add the services to</param>
    /// <returns><see cref="IServiceCollection"/> as per convention</returns>
    /// <remarks>The handlers are added to the http client by the <see cref="AttachKiotaHandlers(IHttpClientBuilder)"/> call, which requires them to be pre-registered in DI</remarks>
    public static IServiceCollection AddKiotaHandlers(this IServiceCollection services)
    {
        // Dynamically load the Kiota handlers from the Client Factory
        var kiotaHandlers = KiotaClientFactory.GetDefaultHandlerActivatableTypes();
        // And register them in the DI container
        foreach (var handler in kiotaHandlers)
        {
            services.AddTransient(handler);
        }

        return services;
    }

    /// <summary>
    /// Adds the Kiota handlers to the http client builder.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    /// <remarks>
    /// Requires the handlers to be registered in DI by <see cref="AddKiotaHandlers(IServiceCollection)"/>.
    /// The order in which the handlers are added is important, as it defines the order in which they will be executed.
    /// </remarks>
    public static IHttpClientBuilder AttachKiotaHandlers(this IHttpClientBuilder builder)
    {
        // Dynamically load the Kiota handlers from the Client Factory
        var kiotaHandlers = KiotaClientFactory.GetDefaultHandlerActivatableTypes();
        // And attach them to the http client builder
        foreach (var handler in kiotaHandlers)
        {
            builder.AddHttpMessageHandler((sp) => (DelegatingHandler)sp.GetRequiredService(handler));
        }

        return builder;
    }
}
