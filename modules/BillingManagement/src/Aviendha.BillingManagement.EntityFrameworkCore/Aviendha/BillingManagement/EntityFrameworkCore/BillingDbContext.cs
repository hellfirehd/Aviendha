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

using Aviendha.BillingManagement.Customers;
using Aviendha.BillingManagement.Invoices;
using Aviendha.BillingManagement.Items;
using Aviendha.BillingManagement.Provinces;
using Aviendha.BillingManagement.Shipping;
using Aviendha.BillingManagement.Taxes;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Aviendha.BillingManagement.EntityFrameworkCore;

[ConnectionStringName(BillingManagementDbProperties.ConnectionStringName)]
public class BillingDbContext(DbContextOptions<BillingDbContext> options)
    : AbpDbContext<BillingDbContext>(options), IBillingDbContext
{
    public DbSet<Customer> Customers { get; set; } = default!;
    public DbSet<Invoice> Invoices { get; set; } = default!;
    public DbSet<Item> Items { get; set; } = default!;
    public DbSet<Province> Provinces { get; set; } = default!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
#if DEBUG
        optionsBuilder.EnableSensitiveDataLogging();
#endif
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<CustomerId>().HaveConversion<CustomerIdValueConverter>();
        configurationBuilder.Properties<InvoiceId>().HaveConversion<InvoiceIdValueConverter>();
        configurationBuilder.Properties<ItemId>().HaveConversion<ItemIdValueConverter>();
        configurationBuilder.Properties<ProvinceId>().HaveConversion<ProvinceIdValueConverter>();
        configurationBuilder.Properties<TaxId>().HaveConversion<TaxIdValueConverter>();

        configurationBuilder.Properties<DateOnly>().HaveConversion<DateOnlyConverter>();
        configurationBuilder.Properties<Address>().HaveConversion<AddressConverter>();
        configurationBuilder.Properties<AppliedTax>().HaveConversion<AppliedTaxValueConverter>();
        configurationBuilder.Properties<Email>().HaveConversion<EmailConverter>();
        configurationBuilder.Properties<ItemSnapshot>().HaveConversion<ItemSnapshotValueConverter>();
        configurationBuilder.Properties<ItemUnitPrice>().HaveConversion<ItemUnitPriceConverter>();
        configurationBuilder.Properties<ShippingInfo>().HaveConversion<ShippingInfoConverter>();
        configurationBuilder.Properties<TaxRate>().HaveConversion<TaxRateValueConverter>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ConfigureBillingManagement();

        base.OnModelCreating(modelBuilder);
    }
}
