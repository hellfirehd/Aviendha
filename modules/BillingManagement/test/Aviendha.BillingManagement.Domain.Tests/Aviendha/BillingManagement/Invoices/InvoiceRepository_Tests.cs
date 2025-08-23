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
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Aviendha.BillingManagement.Invoices;

/* Write your custom repository tests in this project as abstract classes.
 * Then inherit these abstract classes from EF Core & MongoDB test projects.
 * In this way, both database providers are tested with the same set tests.
 */
public abstract class InvoiceRepository_Tests<TStartupModule> : BillingManagementDomainTestBase<TStartupModule, EfCoreInvoiceRepository>
    where TStartupModule : IAbpModule
{
    private readonly IDbContextProvider<IBillingDbContext> _dbContextProvider;

    protected InvoiceRepository_Tests()
    {
        _dbContextProvider = GetRequiredService<IDbContextProvider<IBillingDbContext>>();
    }

    [Fact]
    public async Task InvoiceRepository_InsertInvoice_Succeeds()
    {
        // Arrange
        var date = DateOnly.FromDateTime(DateTime.Now);
        var invoiceId = InvoiceId.FromGuid(GuidGenerator.Create());
        var customer = await CustomerRepository.GetAsync(TestData.OntarioCustomerId);
        var province = await ProvinceRepository.GetAsync(customer.ShippingAddress.Province);
        var invoice = Invoice.Create(invoiceId, customer, province, date);

        // Act
        var result = await SUT.InsertAsync(invoice);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(invoiceId);
        result.InvoiceDate.ShouldBe(date);
        result.CustomerId.ShouldBe(customer.Id);
        result.PlaceOfSupplyId.ShouldBe(province.Id);
        result.BillingAddress.ShouldBe(customer.BillingAddress);
        result.ShippingAddress.ShouldBe(customer.ShippingAddress);
        result.Status.ShouldBe(InvoiceStatus.Draft);
    }

    [Fact]
    public async Task InvoiceRepository_InsertInvoice_WithLineItem_Succeeds()
    {
        // Arrange
        var date = DateOnly.FromDateTime(DateTime.Now);
        var invoiceId = InvoiceId.FromGuid(GuidGenerator.Create());
        var customer = await CustomerRepository.GetAsync(TestData.OntarioCustomerId);
        var province = await ProvinceRepository.GetAsync(customer.ShippingAddress.Province);
        var invoice = Invoice.Create(invoiceId, customer, province, date);
        var lineItem = await TaxableProductAsync(date); // $100.00 item
        invoice.AddLineItem(lineItem);

        // Act
        await SUT.InsertAsync(invoice);

        // Assert
        var inserted = await SUT.GetAsync(invoiceId);
        inserted.ShouldNotBeNull();
        inserted.LineItems.Count.ShouldBe(1);
    }

    [Fact]
    public async Task InvoiceRepository_UpdateInvoice_Succeeds()
    {
        // Arrange
        var date = DateOnly.FromDateTime(DateTime.Now);
        var invoiceId = InvoiceId.FromGuid(GuidGenerator.Create());

        await WithUnitOfWorkAsync(async () =>
        {
            var customer = await CustomerRepository.GetAsync(TestData.OntarioCustomerId);
            var province = await ProvinceRepository.GetAsync(customer.ShippingAddress.Province);
            var invoice = Invoice.Create(invoiceId, customer, province, date);
            var dbContext = await _dbContextProvider.GetDbContextAsync();
            dbContext.Invoices.Add(invoice);
            await dbContext.SaveChangesAsync();
        });

        // Act
        await WithUnitOfWorkAsync(async () =>
        {
            var invoice = await SUT.GetAsync(invoiceId);
            var lineItem = await TaxableProductAsync(date); // $100.00 item
            invoice.AddLineItem(lineItem);
            await SUT.UpdateAsync(invoice, autoSave: true);
        });

        // Assert
        await WithUnitOfWorkAsync(async () =>
        {
            var dbContext = await _dbContextProvider.GetDbContextAsync();
            var updated = await dbContext.Invoices.SingleAsync(i => i.Id == invoiceId);
            updated.ShouldNotBeNull();
            updated.LineItems.Count.ShouldBe(1);
        });
    }
}
