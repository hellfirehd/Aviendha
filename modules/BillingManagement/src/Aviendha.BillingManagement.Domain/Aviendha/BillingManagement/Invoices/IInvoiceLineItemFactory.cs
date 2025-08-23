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

using Aviendha.BillingManagement.Items;

namespace Aviendha.BillingManagement.Invoices;

public interface IInvoiceLineItemFactory
{
    /// <summary>
    /// Determines whether the specified item can be created by this instance.
    /// </summary>
    /// <param name="item">The item to evaluate for creation eligibility. Cannot be null.</param>
    /// <returns><see langword="true"/> if the item meets the criteria for creation; otherwise, <see langword="false"/>.</returns>
    Boolean CanCreate(Item item);

    /// <summary>
    /// Creates an invoice item based on the provided request.
    /// </summary>
    /// <param name="item">The request containing details for the invoice item.</param>
    /// <param name="invoiceDate">The date used to determine the unit price.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>An instance of <see cref="LineItem"/>.</returns>
    Task<LineItem> CreateAsync(Item item, DateOnly invoiceDate, CancellationToken cancellationToken = default);
}
