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

using Aviendha.BillingManagement.Customers;
using Aviendha.BillingManagement.Invoices;
using Aviendha.BillingManagement.Provinces;
using Volo.Abp.Domain.Services;

namespace Aviendha.BillingManagement.Taxes;

/// <summary>
/// Interface for tax calculation providers
/// </summary>
public interface ITaxManager : IDomainService
{
    /// <summary>
    /// Gets the tax rates for <paramref name="province"/> that were in effect on <paramref name="effectiveDate"/>.
    /// </summary>
    /// <param name="province"></param>
    /// <param name="effectiveDate"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>List of applicable taxes</returns>
    Task<IEnumerable<ApplicableTax>> GetTaxesAsync(ProvinceId provinceId, DateOnly? effectiveDate = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all applicable tax rates for a line item and customer profile on a specific date
    /// </summary>
    /// <param name="item"></param>
    /// <param name="customerProfile"></param>
    /// <param name="effectiveDate"></param>
    /// <returns></returns>
    Task<IEnumerable<ApplicableTax>> GetTaxesAsync(LineItem item, CustomerTaxProfile customerProfile, DateOnly effectiveDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the tax profile for a customer, which includes applicable tax codes and rates.
    /// </summary>
    /// <param name="customer"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The tax profile for the specified customer.</returns>
    Task<CustomerTaxProfile> GetTaxProfileAsync(CustomerId customerId, DateOnly invoiceDate, CancellationToken cancellationToken = default);
}
