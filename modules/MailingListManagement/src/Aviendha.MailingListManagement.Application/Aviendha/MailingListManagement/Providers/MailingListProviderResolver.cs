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

using Aviendha.MailingListManagement.Settings;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Settings;

namespace Aviendha.MailingListManagement.Providers;

public class MailingListProviderResolver(IServiceProvider serviceProvider, ISettingProvider settingProvider)
    : IMailingListProviderResolver, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ISettingProvider _settingProvider = settingProvider;

    public async Task<IMailingListProvider> ResolveProviderAsync(CancellationToken cancellationToken = default)
    {
        // Retrieve the provider name from settings
        var providerName = await _settingProvider.GetOrNullAsync(MailingListsSettings.ProviderName);

        // Use a default provider if none is configured
        if (String.IsNullOrEmpty(providerName))
        {
            providerName = "DefaultProvider"; // Replace with your actual default provider name
        }

        // Resolve the provider from the DI container
        var provider = _serviceProvider
            .GetServices<IMailingListProvider>()
            .FirstOrDefault(p => p.ProviderName == providerName)
            ?? throw new Exception($"Provider '{providerName}' not found.");

        return provider;
    }
}
