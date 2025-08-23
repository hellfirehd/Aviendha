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

using Aviendha.BillingManagement.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Aviendha.BillingManagement.Provinces;

[ExposeServices(typeof(IProvinceRepository))]
public class ProvinceRepository(IDbContextProvider<IBillingDbContext> dbContextProvider)
    : EfCoreRepository<IBillingDbContext, Province, ProvinceId>(dbContextProvider),
    IProvinceRepository,
    ITransientDependency
{
    public async Task<Province> GetAsync(String code, Boolean includeDetails = false, CancellationToken cancellationToken = default)
    {
        return includeDetails
            ? await (await WithDetailsAsync())
                .Where(c => c.Code == code)
                .SingleOrDefaultAsync(GetCancellationToken(cancellationToken))
                ?? throw new BillingManagementException(ErrorCodes.NotFound, $"Province with code '{code}' not found.")
            : await (await GetQueryableAsync())
                .Where(c => c.Code == code)
                .SingleOrDefaultAsync(GetCancellationToken(cancellationToken))
                ?? throw new BillingManagementException(ErrorCodes.NotFound, $"Province with code '{code}' not found.");
    }
}
