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

namespace Aviendha.BillingManagement.Invoices;

public class InvoiceEntityTypeConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ConfigureByConvention();

        builder.ToTable(BillingManagementDbProperties.DbTablePrefix + "Invoices", BillingManagementDbProperties.DbSchema);
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("InvoiceId")
            .HasConversion<InvoiceIdValueConverter>()
            .IsRequired();

        builder.Property(invoice => invoice.InvoiceNumber)
            .HasColumnName("InvoiceNumber")
            .HasMaxLength(BillingManagementConsts.MaxInvoiceNumberLength)
            .IsRequired();

        builder.Property(invoice => invoice.InvoiceDate)
            .HasColumnName("InvoiceDate")
            .IsRequired();

        builder.Property(invoice => invoice.DueDate)
            .HasColumnName("DueDate")
            .IsRequired();

        builder.Property(invoice => invoice.Status)
            .HasColumnName("Status")
            .HasConversion<String>()
            .IsRequired();

        builder.Property(invoice => invoice.CustomerId)
            .HasColumnName("CustomerId")
            .IsRequired();

        builder.Property(invoice => invoice.ReferenceNumber)
            .HasColumnName("ReferenceNumber")
            .HasMaxLength(BillingManagementConsts.MaxReferenceNumberLength)
            .IsRequired();

        builder.Property(invoice => invoice.PlaceOfSupplyId)
            .HasColumnName("PlaceOfSupplyId")
            .IsRequired();

        builder.Property(invoice => invoice.BillingAddress)
            .HasColumnName("BillingAddress")
            .HasConversion<AddressConverter>()
            .IsRequired();

        builder.Property(invoice => invoice.ShippingAddress)
            .HasColumnName("ShippingAddress")
            .HasConversion<AddressConverter>()
            .IsRequired();

        builder.Property(invoice => invoice.Shipping)
            .HasColumnName("Shipping")
            .HasConversion<ShippingInfoConverter>()
            .IsRequired();

        builder.OwnsMany(invoice => invoice.LineItems, lineItem =>
        {
            lineItem.ToJson();
            lineItem.WithOwner().HasForeignKey("InvoiceId");
        });

        builder.Navigation(invoice => invoice.LineItems)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasField("_lineItems");

        builder.OwnsMany(invoice => invoice.Discounts, discount =>
        {
            discount.ToJson();
            discount.WithOwner().HasForeignKey("InvoiceId");
        });

        builder.Navigation(invoice => invoice.Discounts)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasField("_discounts");

        builder.OwnsMany(invoice => invoice.Surcharges, surcharge =>
        {
            surcharge.ToJson();
            surcharge.WithOwner().HasForeignKey("InvoiceId");
        });

        builder.Navigation(invoice => invoice.Surcharges)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasField("_surcharges");

        builder.OwnsMany(invoice => invoice.Payments, payment =>
        {
            payment.ToJson();
            payment.WithOwner().HasForeignKey("InvoiceId");
        });

        builder.Navigation(invoice => invoice.Payments)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasField("_payments");

        builder.OwnsMany(invoice => invoice.Refunds, refund =>
        {
            refund.ToJson();
            refund.WithOwner().HasForeignKey("InvoiceId");
        });

        builder.Navigation(invoice => invoice.Refunds)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasField("_refunds");
    }
}
