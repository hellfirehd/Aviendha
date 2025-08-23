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

using Volo.Abp.Modularity;

namespace Aviendha.BillingManagement.Provinces;

public abstract class ProvinceSeeding_Tests<TStartupModule> : BillingManagementDomainTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    [Fact]
    public async Task Provinces_are_seeded()
    {
        // Arrange
        var repository = GetRequiredService<IProvinceRepository>();

        // Act
        var provinces = await repository.GetListAsync();

        // Assert
        provinces.Count.ShouldBe(13); // Canada has 10 provinces and 3 territories
    }
}
