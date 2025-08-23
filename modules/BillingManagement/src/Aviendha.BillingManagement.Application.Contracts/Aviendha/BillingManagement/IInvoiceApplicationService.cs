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

using Aviendha.BillingManagement.Invoices;
using Volo.Abp.Application.Services;

namespace Aviendha.BillingManagement;

/// <summary>
/// Application service contract for invoice operations
/// </summary>
public interface IInvoiceApplicationService : IApplicationService
{
    /// <summary>
    /// Gets an invoice by ID
    /// </summary>
    Task<InvoiceDto> GetInvoiceAsync(InvoiceId invoiceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all invoices with optional filtering
    /// </summary>
    Task<IEnumerable<InvoiceDto>> GetInvoicesAsync(InvoiceFilter filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new invoice
    /// </summary>
    Task<Result<InvoiceId>> CreateInvoiceAsync(CreateInvoice command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a line item to an invoice
    /// </summary>
    Task<Result> AddLineItemAsync(AddLineItem command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes an item from an invoice
    /// </summary>
    Task<Result> RemoveLineItemAsync(RemoveLineItem command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Applies discounts to an invoice
    /// </summary>
    Task<Result> AddDiscountsAsync(AddDiscount command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Applies surcharges to an invoice
    /// </summary>
    Task<Result> AddSurchargesAsync(AddSurcharge command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancels an invoice
    /// </summary>
    Task<Result> PostInvoiceAsync(PostInvoice command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancels an invoice
    /// </summary>
    Task<Result> CancelInvoiceAsync(CancelInvoice command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Processes a payment for an invoice
    /// </summary>
    Task<Result> ProcessPaymentAsync(ProcessPayment command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Processes a refund for an invoice
    /// </summary>
    Task<Result> ProcessRefundAsync(ProcessRefund command, CancellationToken cancellationToken = default);
    Task<Result> UpdateLineItemQuantityAsync(UpdateLineItemQuantity updateLineItemQuantity);
    Task<Result> MoveLineItemUpAsync(MoveLineItemUp moveLineItemUp);
    Task<Result> MoveLineItemDownAsync(MoveLineItemDown moveLineItemDown);
    Task<Result> MoveLineItemToPositionAsync(MoveLineItemToPosition moveLineItemToPosition);
}
