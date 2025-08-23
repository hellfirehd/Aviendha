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

//namespace Aviendha.BillingManagement.Invoices;

//public class DiscountEntityConfiguration : EntityConfiguration<Discount>
//{
//    protected override String GetTableName() => "Dicounts";

//    protected override void ConfigureEntity(EntityTypeBuilder<Discount> builder)
//    {
//        builder.Property(d => d.Name)
//            .HasColumnName("Name")
//            .HasMaxLength(BillingManagementConsts.MaxNameLength)
//            .IsRequired();

//        builder.Property(d => d.Description)
//            .HasColumnName("Description")
//            .HasMaxLength(BillingManagementConsts.MaxDescriptionLength)
//            .IsRequired(false);

//        builder.Property(d => d.MinimumAmount)
//            .HasColumnName("MinimumAmount")
//            .HasColumnType("MONEY")
//            .IsRequired();

//        builder.Property(d => d.MaximumDiscount)
//            .HasColumnName("MaximumDiscount")
//            .HasColumnType("MONEY")
//            .IsRequired();

//        builder.Property(d => d.Percentage)
//            .HasColumnName("Percentage")
//            .HasColumnType("MONEY")
//            .IsRequired();

//        builder.Property(d => d.FixedAmount)
//            .HasColumnName("FixedAmount")
//            .HasColumnType("MONEY")
//            .IsRequired();
//    }
//}
