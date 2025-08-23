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
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Aviendha.BillingManagement.Customers;

public class CustomerEntityTypeConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ConfigureByConvention();

        builder.ToTable(BillingManagementDbProperties.DbTablePrefix + "Customers", BillingManagementDbProperties.DbSchema);
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("CustomerId")
            .HasConversion<CustomerIdValueConverter>()
            .IsRequired();

        builder.Property(c => c.Name).HasColumnName("Name").HasMaxLength(256).IsRequired();

        builder.Property(q => q.Addresses)
            .HasField("_addresses")
            .HasColumnName("Addresses")
            .UsePropertyAccessMode(PropertyAccessMode.PreferField)
            .HasConversion<AddressListValueConverter>()
            .IsRequired()
            .Metadata.SetValueComparer(typeof(AddressListValueComparer));

        builder.HasMany(customer => customer.Invoices)
            .WithOne()
            .HasForeignKey(invoice => invoice.CustomerId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
