// Aviendha ABP Framework Extensions
// Copyright (C) 2025 Doug Wilson
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

using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
using System.Diagnostics;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace Aviendha.MailingListManagement.Listmonk;

public class ListmonkClientAuthenticationProvider(ICurrentTenant currentTenant, AuthenticationSettings settings)
    : IAuthenticationProvider, ITransientDependency
{
    private static readonly ActivitySource ActivitySource = new(typeof(RequestInformation).Namespace!);

    private readonly ICurrentTenant _currentTenant = currentTenant;
    private readonly AuthenticationSettings _settings = settings;

    /// <inheritdoc />
    public async Task AuthenticateRequestAsync(RequestInformation request, Dictionary<String, Object>? additionalAuthenticationContext = default, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var span = ActivitySource?.StartActivity(nameof(AuthenticateRequestAsync));

        var validator = new AllowedHostsValidator(await _settings.GetAllowedHostsAsync());

        if (!validator.IsUrlHostValid(request.URI))
        {
            span?.SetTag("com.microsoft.kiota.authentication.is_url_valid", false);
            return;
        }

        var uri = request.URI;
        if (!uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
        {
            span?.SetTag("com.microsoft.kiota.authentication.is_url_valid", false);
            throw new ArgumentException("Only https is supported");
        }

        // Authorization: token api_user:token
        request.Headers.Add("Authorization", $"token {await _settings.GetUsernameAsync()}:{await _settings.GetApiKeyAsync()}");

        span?.SetTag("com.microsoft.kiota.authentication.is_url_valid", true);
    }
}
