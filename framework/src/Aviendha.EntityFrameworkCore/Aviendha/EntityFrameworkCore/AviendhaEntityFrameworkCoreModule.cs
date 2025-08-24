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

using Aviendha.Application;
using Aviendha.EntityFrameworkCore.Migrations;
using Aviendha.Specifications;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Aviendha.EntityFrameworkCore;

[DependsOn(typeof(AviendhaDddApplicationModule))]
[DependsOn(typeof(AviendhaSpecificationsModule))]
[DependsOn(typeof(AbpEntityFrameworkCoreModule))]
public class AviendhaEntityFrameworkCoreModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        AutoAddDatabaseMigrators(context.Services);
    }

    private static void AutoAddDatabaseMigrators(IServiceCollection services)
    {
        var migrators = new List<Type>();

        services.OnRegistered(context =>
        {
            if (typeof(IDbContextMigrator).IsAssignableFrom(context.ImplementationType))
            {
                migrators.Add(context.ImplementationType);
            }
        });

        services.Configure<AviendhaDbContextMigrationOptions>(options =>
        {
            // Add all found migrators
            options.Migrators.AddIfNotContains(migrators);
        });
    }
}
