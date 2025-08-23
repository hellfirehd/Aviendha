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

namespace Aviendha.BillingManagement;

public class InvoiceDto
{
    public Guid Id { get; set; }
    public String InvoiceNumber { get; set; } = String.Empty;
    public DateOnly InvoiceDate { get; set; }
    public DateOnly? DueDate { get; set; }
    public InvoiceStatus Status { get; set; }
    public String CustomerName { get; set; } = String.Empty;
    public String CustomerEmail { get; set; } = String.Empty;
    public String BillingManagementAddress { get; set; } = String.Empty;
    public String ShippingAddress { get; set; } = String.Empty;
    public String State { get; set; } = String.Empty;
    public Decimal Subtotal { get; set; }
    public Decimal TotalTax { get; set; }
    public Decimal TotalDiscounts { get; set; }
    public Decimal TotalSurcharges { get; set; }
    public Decimal ShippingCost { get; set; }
    public Decimal Total { get; set; }
    public Decimal TotalPaid { get; set; }
    public Decimal TotalRefunded { get; set; }
    public Decimal Balance { get; set; }
    public List<LineItemDto> LineItems { get; set; } = [];
    public List<PaymentResponse> Payments { get; set; } = [];
    public List<RefundResponse> Refunds { get; set; } = [];
}
