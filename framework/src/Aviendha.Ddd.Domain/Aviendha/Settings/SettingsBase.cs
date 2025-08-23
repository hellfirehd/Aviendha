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

using Volo.Abp;
using Volo.Abp.Settings;

namespace Aviendha.Settings;

public abstract class SettingsBase(ISettingProvider settingProvider)
{
    protected ISettingProvider SettingProvider { get; } = settingProvider;

    /// <summary>
    /// Gets a setting value by checking. Throws <see cref="AbpException"/> if it's null or empty.
    /// </summary>
    /// <param name="name">Name of the setting</param>
    /// <returns>Value of the setting</returns>
    protected async Task<String> GetOrThrow(String name)
    {
        var value = await SettingProvider.GetOrNullAsync(name);

        if (value.IsNullOrEmpty())
        {
            throw new AbpException($"Setting value for '{name}' is null or empty!");
        }

        return value!;
    }
}
