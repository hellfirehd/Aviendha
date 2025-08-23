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

namespace Aviendha.BillingManagement.Provinces;

public class ProvinceEntityTypeConfiguration : IEntityTypeConfiguration<Province>
{
    public void Configure(EntityTypeBuilder<Province> builder)
    {
        builder.ConfigureByConvention();

        builder.ToTable(BillingManagementDbProperties.DbTablePrefix + "Provinces", BillingManagementDbProperties.DbSchema);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("ProvinceId")
            .HasConversion<ProvinceIdValueConverter>()
            .IsRequired();

        builder.Property(p => p.Code)
            .HasColumnName("Code")
            .HasMaxLength(BillingManagementConsts.MaxProvinceCodeLength)
            .IsRequired();

        builder.Property(p => p.Name)
            .HasColumnName("Name")
            .HasMaxLength(BillingManagementConsts.MaxProvinceNameLength)
            .IsRequired();

        //builder.Property(p => p.Taxes)
        //    .HasColumnName("Taxes")
        //    .HasField("_taxes")
        //    .UsePropertyAccessMode(PropertyAccessMode.Field);
        //.HasConversion<TaxRateListValueConverter>()
        //.IsRequired()
        //.Metadata.SetValueComparer(typeof(TaxRateListValueComparer));

        builder.HasMany(province => province.Taxes)
            .WithMany();
    }
}
