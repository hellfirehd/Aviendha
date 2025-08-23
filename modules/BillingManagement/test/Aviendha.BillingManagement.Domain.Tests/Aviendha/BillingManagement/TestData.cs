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
using Bogus;

namespace Aviendha.BillingManagement;

/// <summary>
/// Builder class for creating test data for billing system tests
/// </summary>
public static class TestData
{
    public static readonly DateOnly InvoiceDate = new(2020, 01, 01);

    public static readonly CustomerId AlbertaCustomerId = CustomerId.Parse("01234567-6d1c-4799-b97a-3c280e0329f8");
    public static readonly CustomerId BritishColumbiaCustomerId = CustomerId.Parse("12345678-6d1c-4799-b97a-3c280e0329f8");
    public static readonly CustomerId OntarioCustomerId = CustomerId.Parse("23456789-6d1c-4799-b97a-3c280e0329f8");

    /// <summary>
    /// Represents the unique identifier for a taxable product worth $100.00.
    /// </summary>
    public static readonly ItemId TaxableProductId = ItemId.Parse("b56a2eb1-1bb9-4103-ad37-79c5fa229d79");

    /// <summary>
    /// Represents the unique identifier for a non-taxable <see cref="ItemCategory.BasicGroceries"/> product worth $50.00.
    /// </summary>
    public static readonly ItemId NonTaxableProductId = ItemId.Parse("f3c8b1d2-4e5f-4c6a-9b0d-7c8e1f2a3b4c");

    /// <summary>
    /// Represents the unique identifier for a taxable service worth $500.00.
    /// </summary>
    public static readonly ItemId TaxableServiceId = ItemId.Parse("3f287b83-025d-403f-94b1-2907fa5b186d");

    /// <summary>
    /// Represents the unique identifier for a taxable service worth $75.00.
    /// </summary>
    public static readonly ItemId NonTaxableServiceId = ItemId.Parse("345e10c0-554e-4ecc-8a0f-cf7f6b3ca226");

    public static readonly String[] Provinces = ["AB", "BC", "MB", "NB", "NL", "NS", "NT", "NU", "ON", "PE", "QC", "SK", "YT"];

    public static Customer Customer(Province province, CustomerId? customerId = null)
    {
        var f = new Faker();

        return new Customer(f.Person.FullName, f.Person.Email, [Address(province.Code, isDefault: false, isBilling: true), Address(province.Code, isDefault: false, isShipping: true)])
            .SetId(customerId ?? CustomerId.New());
    }

    public static Address Address(String? provinceCode = null, Boolean isDefault = false, Boolean? isBilling = null, Boolean? isShipping = null)
    {
        var f = new Faker();

        return new Address(
            f.Person.FullName,
            f.Address.StreetAddress(),
            String.Empty, f.Address.City(),
            String.IsNullOrWhiteSpace(provinceCode) ? f.PickRandom(Provinces) : provinceCode,
            f.Address.ZipCode(),
            "Canada",
            new PhoneNumber(f.Person.Phone),
            isDefault,
            isDefault || (isShipping ?? f.Random.Bool()),
            isDefault || (isShipping ?? f.Random.Bool()));
    }

    /// <summary>
    /// Creates a simple invoice for basic testing
    /// </summary>
    public static Invoice EmptyInvoice(Province placeOfSupply)
    {
        var customer = Customer(placeOfSupply);

        return Invoice.Create(InvoiceId.New(), customer, placeOfSupply, InvoiceDate);
    }

    public static Product NewProduct(ItemId id, String sku = "TOM001", String name = "Tomatoes", String taxCode = "ZR-GROCERY", Decimal unitPrice = 50.00m, ItemType itemType = ItemType.Product, ItemCategory itemCategory = ItemCategory.BasicGroceries)
    {
        var product = Product.Create(id, sku, name, taxCode, itemCategory);

        product.AddPrice(unitPrice, DateOnly.FromDateTime(DateTime.UnixEpoch));

        return product;
    }
}
