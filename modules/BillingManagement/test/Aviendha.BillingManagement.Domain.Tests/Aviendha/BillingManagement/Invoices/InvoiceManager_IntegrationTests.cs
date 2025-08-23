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

public abstract class InvoiceManager_IntegrationTests<TStartupModule> : BillingManagementDomainTestBase<TStartupModule, InvoiceManager>
    where TStartupModule : IAbpModule
{
    private readonly IDbContextProvider<IBillingDbContext> _dbContextProvider;

    protected InvoiceManager_IntegrationTests()
    {
        _dbContextProvider = GetRequiredService<IDbContextProvider<IBillingDbContext>>();
    }

    [Fact]
    public async Task InvoiceManager_CreateInvoice_ShouldSucceed()
    {
        // Act
        var invoiceId = await SUT.CreateInvoiceAsync(TestData.OntarioCustomerId, []);

        // Assert
        invoiceId.ShouldNotBe(InvoiceId.Empty);
        var created = await SUT.GetInvoiceAsync(invoiceId);
        created.ShouldNotBeNull();
    }

    [Fact]
    public async Task InvoiceManager_AddLineItem_ShouldSucceed()
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
            var invoice = await SUT.GetInvoiceAsync(invoiceId);
            var taxableProduct = await TaxableProductAsync(date); // $100
            await SUT.AddLineItem(invoice, taxableProduct);
        });

        // Assert
        await WithUnitOfWorkAsync(async () =>
        {
            var dbContext = await _dbContextProvider.GetDbContextAsync();
            var updated = await dbContext.Invoices.SingleAsync(i => i.Id == invoiceId);
            updated.ShouldNotBeNull();
            updated.LineItems.Count.ShouldBe(1);
            updated.LineItems[0].Item.ItemId.ShouldBe(TestData.TaxableProductId);
        });
    }
}
