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

using Aviendha.BillingManagement.EntityFrameworkCore;
using Aviendha.BillingManagement.Items;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Aviendha.BillingManagement.Taxes;

[ExposeServices(typeof(ITaxCodeRepository))]
public class TaxCodeRepository(IDbContextProvider<IBillingDbContext> dbContextProvider)
    : EfCoreRepository<IBillingDbContext, TaxCode, Guid>(dbContextProvider),
    ITaxCodeRepository,
    ITransientDependency
{
    private readonly Dictionary<String, TaxCode> _taxCodes = new()
    {
        // Standard taxable goods
        ["STD-GOODS"] = new TaxCode
        {
            Code = "STD-GOODS",
            Description = "Standard Taxable Goods",
            TaxTreatment = TaxTreatment.Standard,
            ItemCategory = ItemCategory.GeneralGoods,
            CraReference = "Schedule VI, Part I"
        },

        // Basic groceries (zero-rated)
        ["ZR-GROCERY"] = new TaxCode
        {
            Code = "ZR-GROCERY",
            Description = "Basic Groceries",
            TaxTreatment = TaxTreatment.ZeroRated,
            ItemCategory = ItemCategory.BasicGroceries,
            CraReference = "Schedule VI, Part III",
            Notes = "Meat, fish, poultry, dairy, eggs, vegetables, fruits, cereals, etc."
        },

        // Prescription drugs (zero-rated)
        ["ZR-PRESCRIP"] = new TaxCode
        {
            Code = "ZR-PRESCRIP",
            Description = "Prescription Drugs",
            TaxTreatment = TaxTreatment.ZeroRated,
            ItemCategory = ItemCategory.PrescriptionDrugs,
            CraReference = "Schedule VI, Part II",
            Notes = "Drugs dispensed by prescription"
        },

        // Medical devices (zero-rated)
        ["ZR-MEDICAL"] = new TaxCode
        {
            Code = "ZR-MEDICAL",
            Description = "Medical Devices",
            TaxTreatment = TaxTreatment.ZeroRated,
            ItemCategory = ItemCategory.MedicalDevices,
            CraReference = "Schedule VI, Part II",
            Notes = "Prescribed medical devices and equipment"
        },

        // Healthcare services (exempt)
        ["EX-HEALTH"] = new TaxCode
        {
            Code = "EX-HEALTH",
            Description = "Healthcare Services",
            TaxTreatment = TaxTreatment.Exempt,
            ItemCategory = ItemCategory.HealthcareServices,
            CraReference = "Schedule V, Part II",
            Notes = "Services rendered by healthcare professionals"
        },

        // Educational services (exempt)
        ["EX-EDUCATION"] = new TaxCode
        {
            Code = "EX-EDUCATION",
            Description = "Educational Services",
            TaxTreatment = TaxTreatment.Exempt,
            ItemCategory = ItemCategory.EducationalServices,
            CraReference = "Schedule V, Part III",
            Notes = "Degree, diploma, certificate courses"
        },

        // Financial services (exempt)
        ["EX-FINANCIAL"] = new TaxCode
        {
            Code = "EX-FINANCIAL",
            Description = "Financial Services",
            TaxTreatment = TaxTreatment.Exempt,
            ItemCategory = ItemCategory.FinancialServices,
            CraReference = "Schedule V, Part VII",
            Notes = "Banking, insurance, investment services"
        },

        // Professional services (standard)
        ["STD-PROF"] = new TaxCode
        {
            Code = "STD-PROF",
            Description = "Professional Services",
            TaxTreatment = TaxTreatment.Standard,
            ItemCategory = ItemCategory.ProfessionalServices,
            Notes = "Consulting, accounting, legal services (non-exempt)"
        },

        // Digital services (standard)
        ["STD-DIGITAL"] = new TaxCode
        {
            Code = "STD-DIGITAL",
            Description = "Digital Services",
            TaxTreatment = TaxTreatment.Standard,
            ItemCategory = ItemCategory.DigitalServices,
            Notes = "Software, digital content, online services"
        },

        // Exports (zero-rated)
        ["ZR-EXPORT"] = new TaxCode
        {
            Code = "ZR-EXPORT",
            Description = "Exports",
            TaxTreatment = TaxTreatment.ZeroRated,
            ItemCategory = ItemCategory.Exports,
            CraReference = "Schedule VI, Part V",
            Notes = "Goods and services exported from Canada"
        }
    };

    public async Task<TaxCode> GetAsync(String code, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        // Simulate asynchronous operation (e.g., database call)
        await Task.Delay(100, cancellationToken); // Simulate some async work

        // Retrieve the TaxCode from the dictionary
        if (_taxCodes.TryGetValue(code, out var taxCode))
        {
            return taxCode;
        }

        throw new BillingManagementException(ErrorCodes.NotFound, $"Tax code '{code}' not found.");
    }
}
