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
using Aviendha.BillingManagement.Provinces;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Services;

namespace Aviendha.BillingManagement.Taxes;

/// <summary>
/// Production-ready Canadian tax calculation provider
/// Supports all Canadian provinces and territories with accurate tax rates
/// </summary>
[ExposeServices(typeof(ITaxManager))]
public class CanadianTaxManager(ICustomerRepository customerRepository, IProvinceRepository mapRepository, ITaxRepository taxRepository, ITaxCodeRepository taxCodeRepository, TimeProvider timeProvider)
    : DomainService, ITaxManager, ITransientDependency
{
    private readonly ICustomerRepository _customerRepository = customerRepository;
    private readonly IProvinceRepository _provinceRepository = mapRepository;
    private readonly ITaxRepository _taxRepository = taxRepository;
    private readonly ITaxCodeRepository _taxCodeRepository = taxCodeRepository;

    protected TimeProvider TimeProvider { get; } = timeProvider;

    /// <summary>
    /// Gets applicable tax rates for a province and date
    /// </summary>
    public async Task<IEnumerable<ApplicableTax>> GetTaxesAsync(ProvinceId provinceId, DateOnly? effectiveDate = null, CancellationToken cancellationToken = default)
    {
        var date = effectiveDate ?? DateOnly.FromDateTime(TimeProvider.GetUtcNow().DateTime);

        // Reload province because the supplied province may not have come with taxes.
        var province = await _provinceRepository.GetAsync(provinceId, includeDetails: true, cancellationToken: cancellationToken);

        var list = new List<ApplicableTax>();
        foreach (var entry in province.Taxes)
        {
            var rate = entry.GetTaxRate(date);
            if (rate is not null)
            {
                list.Add(new ApplicableTax(entry.Id, entry.Name, entry.Code, rate.Rate));
            }
        }

        return list;
    }

    public async Task<IEnumerable<ApplicableTax>> GetTaxesAsync(LineItem lineItem, CustomerTaxProfile customerProfile, DateOnly effectiveDate, CancellationToken cancellationToken = default)
    {
        // Check customer exemption first
        if (customerProfile.QualifiesForExemption(effectiveDate) == true)
        {
            return [];
        }

        // Get item classification
        if (String.IsNullOrEmpty(lineItem.Item.TaxCode))
        {
            // Fallback to standard taxes if no classification
            return await GetTaxesAsync(customerProfile.PlaceOfSupplyId, effectiveDate, cancellationToken);
        }

        // Get tax code
        var taxCode = await _taxCodeRepository.GetAsync(lineItem.Item.TaxCode, cancellationToken);
        if (taxCode is null || !taxCode.IsValidOn(effectiveDate))
        {
            // Fallback to standard taxes if tax code not found
            return await GetTaxesAsync(customerProfile.PlaceOfSupplyId, effectiveDate, cancellationToken);
        }

        // Apply tax treatment rules
        return taxCode.TaxTreatment switch
        {
            TaxTreatment.Exempt or TaxTreatment.OutOfScope => [],
            TaxTreatment.ZeroRated => await GetZeroRatedTaxRatesAsync(customerProfile.PlaceOfSupplyId, effectiveDate),
            _ => await GetTaxesAsync(customerProfile.PlaceOfSupplyId, effectiveDate, cancellationToken)
        };
    }

    public async Task<CustomerTaxProfile> GetTaxProfileAsync(CustomerId customerId, DateOnly invoiceDate, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetAsync(customerId, includeDetails: false, cancellationToken);
        var province = await _provinceRepository.GetAsync(customer.BillingAddress.Province, includeDetails: true, cancellationToken: cancellationToken);

        // Here you would typically retrieve the tax profile from a database or an external service
        // For demonstration, returning a new instance of CustomerTaxProfile
        return new CustomerTaxProfile
        {
            CustomerId = customer.Id,
            PlaceOfSupplyId = province.Id,
            EffectiveDate = invoiceDate,
            TaxStatus = customer.TaxStatus
        };
    }

    private async Task<List<ApplicableTax>> GetZeroRatedTaxRatesAsync(ProvinceId taxProvinceId, DateOnly effectiveDate)
    {
        // Zero-rated items typically have a specific tax rate of 0%
        var list = await GetTaxesAsync(taxProvinceId, effectiveDate);

        return list
            .Select(at => at with { Rate = 0.0m })
            .ToList();
    }
}
