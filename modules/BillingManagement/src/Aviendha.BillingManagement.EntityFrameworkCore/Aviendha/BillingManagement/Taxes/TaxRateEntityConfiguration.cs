// Aviendha Billing Management
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

//using Aviendha.BillingManagement.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;

//namespace Aviendha.BillingManagement.Taxes;

//public class TaxRateEntityConfiguration : EntityConfiguration<TaxRate>
//{
//    protected override String GetTableName() => "TaxRates";
//    protected override void ConfigureEntity(EntityTypeBuilder<TaxRate> builder)
//    {
//        builder.Property(x => x.Rate)
//            .HasColumnName("Rate")
//            .HasPrecision(16, 6)
//            .IsRequired();

//        builder.Property(x => x.IsActive)
//            .HasColumnName("IsActive")
//            .HasDefaultValue(true)
//            .IsRequired();

//        builder.Property(x => x.EffectiveDate)
//            .HasColumnName("EffectiveDate")
//            .IsRequired();

//        builder.Property(x => x.ExpiryDate)
//            .HasColumnName("ExpiryDate")
//            .IsRequired(false);
//    }
//}
