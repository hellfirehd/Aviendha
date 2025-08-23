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

using Aviendha.MailingListManagement.Listmonk.Settings;
using Aviendha.Settings;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Settings;

namespace Aviendha.MailingListManagement.Listmonk;

public class AuthenticationSettings(ISettingProvider settingProvider)
    : SettingsBase(settingProvider), ITransientDependency
{
    public async Task<String> GetBaseUrlAsync()
    {
        return await GetOrThrow(ListmonkSettings.BaseUrl);
    }

    public async Task<String> GetUsernameAsync()
    {
        return await GetOrThrow(ListmonkSettings.Username);
    }

    public async Task<String> GetApiKeyAsync()
    {
        return await GetOrThrow(ListmonkSettings.ApiKey);
    }

    public async Task<String[]> GetAllowedHostsAsync()
    {
        var value = await SettingProvider.GetOrNullAsync(ListmonkSettings.AllowedHosts);
        return value?.Split(',') ?? [];
    }

    public async Task<Boolean> IsRequireHttpsAsync()
    {
        var value = await SettingProvider.GetOrNullAsync(ListmonkSettings.RequireHttps);
        return Boolean.TryParse(value, out var result) && result;
    }
}
