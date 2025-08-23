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

using Aviendha.BillingManagement.Invoices;
using Aviendha.BillingManagement.Items;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace Aviendha.BillingManagement.EntityFrameworkCore;

public static class BillingManagementDbContextModelCreatingExtensions
{
    public static void ConfigureBillingManagement(this ModelBuilder modelBuilder)
    {
        Check.NotNull(modelBuilder, nameof(modelBuilder));

        // Ignore entities that should not be mapped directly to tables
        modelBuilder.Ignore<AppliedDiscount>();
        modelBuilder.Ignore<AppliedSurcharge>();
        modelBuilder.Ignore<AppliedTax>();
        modelBuilder.Ignore<ItemDiscount>();
        modelBuilder.Ignore<ItemSnapshot>();
        modelBuilder.Ignore<ItemUnitPrice>();

        //modelBuilder.ApplyConfiguration(new CustomerEntityTypeConfiguration());
        //modelBuilder.ApplyConfiguration(new InvoiceEntityTypeConfiguration());
        //modelBuilder.ApplyConfiguration(new ItemEntityTypeConfiguration());
        //modelBuilder.ApplyConfiguration(new ProvinceEntityTypeConfiguration());
        //modelBuilder.ApplyConfiguration(new TaxEntityTypeConfiguration());

        var assembly = typeof(BillingManagementDbContextModelCreatingExtensions).Assembly;
        modelBuilder.ApplyConfigurationsFromAssembly(assembly);

        modelBuilder.ConfigurePermissionManagement();
        modelBuilder.ConfigureSettingManagement();
        modelBuilder.ConfigureTenantManagement();
    }
}
