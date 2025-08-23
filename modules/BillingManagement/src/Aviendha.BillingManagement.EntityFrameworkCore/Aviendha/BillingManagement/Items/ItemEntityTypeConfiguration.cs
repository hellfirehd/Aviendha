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

namespace Aviendha.BillingManagement.Items;

public class ItemEntityTypeConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.ConfigureByConvention();

        builder.ToTable(BillingManagementDbProperties.DbTablePrefix + "Items", BillingManagementDbProperties.DbSchema);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("ItemId")
            .HasConversion<ItemIdValueConverter>()
            .IsRequired();

        builder.HasDiscriminator(i => i.ItemType)
            .HasValue<Product>(ItemType.Product)
            .HasValue<Service>(ItemType.Service)
            .IsComplete(false);

        builder.Property(i => i.Name)
            .HasColumnName("Name")
            .HasMaxLength(BillingManagementConsts.MaxNameLength)
            .IsRequired();

        builder.Property(i => i.Description)
            .HasColumnName("Description")
            .HasMaxLength(BillingManagementConsts.MaxDescriptionLength)
            .IsRequired();

        builder.Property(i => i.TaxCode)
            .HasColumnName("TaxCode")
            .HasMaxLength(BillingManagementConsts.MaxTaxCodeLength)
            .IsRequired();

        builder.Property(i => i.UnitType)
            .HasColumnName("UnitType")
            .HasMaxLength(BillingManagementConsts.MaxUnitTypeLength)
            .IsRequired();

        builder.Property(i => i.ItemType)
            .HasColumnName("ItemType")
            .IsRequired();

        builder.Property(i => i.ItemCategory)
            .HasColumnName("ItemCategory")
            .IsRequired();

        builder.Property(i => i.IsActive)
            .HasColumnName("IsActive")
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(q => q.Pricing)
            .HasColumnName("Pricing")
            .HasField("_pricing")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasConversion<ItemUnitPriceListConverter>()
            .IsRequired()
            .Metadata.SetValueComparer(typeof(ItemUnitPriceListValueComparer));
    }
}
