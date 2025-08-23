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
using Aviendha.BillingManagement.EntityFrameworkCore;
using Aviendha.BillingManagement.Items;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace Aviendha.BillingManagement;

public class BillingManagementDataSeedContributor(
    ICurrentTenant currentTenant,
    IBillingDbContext dbContext)
    : IDataSeedContributor, ITransientDependency
{
    private readonly ICurrentTenant _currentTenant = currentTenant;
    private readonly IBillingDbContext _dbContext = dbContext;

    private readonly DateOnly _date = DateOnly.FromDateTime(DateTime.UnixEpoch);

    public async Task SeedAsync(DataSeedContext context)
    {
        using (_currentTenant.Change(context?.TenantId))
        {
            await SeedTestCustomersAsync();

            await SeedProductsAsync();
        }
    }

    private async Task SeedTestCustomersAsync()
    {
        await SeedTestCustomerAsync(TestData.AlbertaCustomerId, "AB");
        await SeedTestCustomerAsync(TestData.BritishColumbiaCustomerId, "BC");
        await SeedTestCustomerAsync(TestData.OntarioCustomerId, "ON");
        await _dbContext.SaveChangesAsync();
    }

    private async Task SeedTestCustomerAsync(CustomerId customerId, String provinceCode)
    {
        var province = await _dbContext.Provinces.SingleAsync(p => p.Code == provinceCode);
        var customer = TestData.Customer(province, customerId);
        _dbContext.Customers.Add(customer);
    }

    private async Task SeedProductsAsync()
    {
        var taxableProduct = Product.Create(TestData.TaxableProductId, "TP-001", "A product that is taxable.", "STD-GOODS", itemCategory: ItemCategory.GeneralGoods);
        taxableProduct.AddPrice(100.00m, _date);

        var nonTaxableProduct = Product.Create(TestData.NonTaxableProductId, "NTProduct-001", "A product that is not taxable.", "ZR-GROCERY", itemCategory: ItemCategory.BasicGroceries);
        nonTaxableProduct.AddPrice(50.00M, _date);

        var taxableService = Service.Create(TestData.TaxableServiceId, "Taxable Service", "A service that is taxable.", "STD-PROF", ItemCategory.FinancialServices);
        taxableService.AddPrice(500.00m, _date);

        var nonTaxableService = Service.Create(TestData.NonTaxableServiceId, "Non-Taxable Service", "A service that is not taxable.", "EX-EDUCATION", ItemCategory.EducationalServices);
        nonTaxableService.AddPrice(75.00m, _date);

        var dbSet = _dbContext.Set<Item>();

        dbSet.AddRange([taxableProduct, nonTaxableProduct, taxableService, nonTaxableService]);

        await _dbContext.SaveChangesAsync();
    }
}
