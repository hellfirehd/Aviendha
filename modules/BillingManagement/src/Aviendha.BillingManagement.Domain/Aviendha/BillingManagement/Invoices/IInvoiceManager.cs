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
using Aviendha.BillingManagement.Items;
using Volo.Abp.Domain.Services;

namespace Aviendha.BillingManagement.Invoices;

public interface IInvoiceManager : IDomainService
{
    Task<InvoiceId> CreateInvoiceAsync(CustomerId customerId, IEnumerable<LineItem> lineItems, CancellationToken cancellationToken = default);
    Task<LineItem> CreateLineItemAsync(ItemId itemId, DateOnly invoiceDate, CancellationToken cancellationToken = default);
    Task<Invoice> GetInvoiceAsync(InvoiceId invoiceId, CancellationToken cancellationToken = default);
    Task<Invoice> UpdateInvoiceAsync(Invoice invoice, CancellationToken cancellationToken = default);
}
