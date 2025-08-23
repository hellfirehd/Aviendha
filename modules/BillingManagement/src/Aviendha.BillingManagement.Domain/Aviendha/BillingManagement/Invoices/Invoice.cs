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
using Aviendha.BillingManagement.Payments;
using Aviendha.BillingManagement.Provinces;
using Aviendha.BillingManagement.Shipping;
using Aviendha.BillingManagement.Taxes;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Entities.Auditing;

namespace Aviendha.BillingManagement.Invoices;

/// <summary>
/// Core invoice aggregate root
/// </summary>
public class Invoice : FullAuditedAggregateRoot<InvoiceId>
{
    private List<LineItem> _lineItems = [];
    private List<AppliedDiscount> _discounts = [];
    private List<AppliedSurcharge> _surcharges = [];
    private List<AppliedPayment> _payments = [];
    private List<AppliedRefund> _refunds = [];

    protected Invoice()
    {
        // Parameterless constructor for EF Core/JSON serialization
    }

    private Invoice(
        InvoiceId invoiceId,
        CustomerId customerId,
        ProvinceId placeOfSupplyId,
        DateOnly invoiceDate) : base(invoiceId)
    {
        InvoiceDate = invoiceDate;  // ToDo: Validate that the date is not insane.
        CustomerId = customerId;
        PlaceOfSupplyId = placeOfSupplyId;
    }

    // Properties
    public virtual String InvoiceNumber { get; protected set; } = String.Empty;
    public virtual DateOnly InvoiceDate { get; protected set; }
    public virtual DateOnly DueDate { get; protected set; }
    public virtual InvoiceStatus Status { get; private set; } = InvoiceStatus.Draft;
    public virtual String ReferenceNumber { get; protected set; } = String.Empty;
    public virtual ShippingInfo Shipping { get; protected set; } = ShippingInfo.Empty;

    public virtual Address BillingAddress { get; protected set; } = Address.Empty;
    public virtual Address ShippingAddress { get; protected set; } = Address.Empty;

    // Relationships
    public CustomerId CustomerId { get; protected set; }
    public ProvinceId PlaceOfSupplyId { get; protected set; }

    [BackingField(nameof(_lineItems))]
    public virtual IReadOnlyList<LineItem> LineItems => _lineItems;
    [BackingField(nameof(_discounts))]
    public virtual IReadOnlyCollection<AppliedDiscount> Discounts => _discounts;
    [BackingField(nameof(_surcharges))]
    public virtual IReadOnlyCollection<AppliedSurcharge> Surcharges => _surcharges;
    [BackingField(nameof(_payments))]
    public virtual IReadOnlyCollection<AppliedPayment> Payments => _payments;
    [BackingField(nameof(_refunds))]
    public virtual IReadOnlyCollection<AppliedRefund> Refunds => _refunds;

    // Methods that modify the invoice

    /// <summary>
    /// Sets the tax rates and recalculates taxes
    /// </summary>
    public async Task ApplyTaxesAsync(ITaxManager taxManager, CancellationToken cancellationToken = default)
    {
        var taxProfile = await taxManager.GetTaxProfileAsync(CustomerId, InvoiceDate, cancellationToken);

        // Apply taxes to each line item
        foreach (var item in LineItems)
        {
            var itemTaxes = await taxManager.GetTaxesAsync(item, taxProfile, InvoiceDate, cancellationToken);

            item.ApplyTaxes(itemTaxes);
        }
    }

    /// <summary>
    /// Adds an item to the invoice
    /// </summary>
    public void AddLineItem(LineItem item)
    {
        var nextSortOrder = _lineItems.Count > 0 ? _lineItems.Max(li => li.SortOrder) + 1 : 0;
        var itemWithSortOrder = item.SetSortOrder(nextSortOrder);
        _lineItems.Add(itemWithSortOrder);
    }

    /// <summary>
    /// Removes a line item by its index in the ordered collection
    /// </summary>
    public void RemoveLineItem(Int32 index)
    {
        if (index < 0 || index >= _lineItems.Count)
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");

        _lineItems.RemoveAt(index);
        RenumberLineItems();
    }

    /// <summary>
    /// Removes a line item by value equality
    /// </summary>
    public void RemoveLineItem(LineItem lineItem)
    {
        var removed = _lineItems.Remove(lineItem);
        if (removed)
            RenumberLineItems();
    }

    /// <summary>
    /// Removes an item from the invoice by item snapshot id
    /// </summary>
    public void RemoveLineItem(ItemId itemSnapshotId)
    {
        var countRemoved = _lineItems.RemoveAll(i => i.Item.ItemId == itemSnapshotId);
        if (countRemoved > 0)
            RenumberLineItems();
    }

    /// <summary>
    /// Updates the quantity of a line item at the specified index
    /// </summary>
    public void UpdateLineItemQuantity(Int32 index, Decimal newQuantity)
    {
        if (index < 0 || index >= _lineItems.Count)
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");

        var oldItem = _lineItems[index];
        var updatedItem = oldItem.SetQuantity(newQuantity);
        _lineItems[index] = updatedItem;
    }

    /// <summary>
    /// Moves a line item up in the order (decreases sort order)
    /// </summary>
    public void MoveLineItemUp(Int32 index)
    {
        if (index <= 0 || index >= _lineItems.Count)
            return; // Can't move up if already at top or invalid index

        var item = _lineItems[index];
        _lineItems.RemoveAt(index);
        _lineItems.Insert(index - 1, item);
        RenumberLineItems();
    }

    /// <summary>
    /// Moves a line item down in the order (increases sort order)
    /// </summary>
    public void MoveLineItemDown(Int32 index)
    {
        if (index < 0 || index >= _lineItems.Count - 1)
            return; // Can't move down if already at bottom or invalid index

        var item = _lineItems[index];
        _lineItems.RemoveAt(index);
        _lineItems.Insert(index + 1, item);
        RenumberLineItems();
    }

    /// <summary>
    /// Moves a line item to a specific position
    /// </summary>
    public void MoveLineItemToPosition(Int32 fromIndex, Int32 toIndex)
    {
        if (fromIndex < 0 || fromIndex >= _lineItems.Count ||
            toIndex < 0 || toIndex >= _lineItems.Count ||
            fromIndex == toIndex)
            return;

        var item = _lineItems[fromIndex];
        _lineItems.RemoveAt(fromIndex);
        _lineItems.Insert(toIndex, item);
        RenumberLineItems();
    }

    /// <summary>
    /// Gets line items ordered by SortOrder
    /// </summary>
    public IReadOnlyList<LineItem> GetOrderedLineItems()
        => _lineItems.OrderBy(li => li.SortOrder).ToList().AsReadOnly();

    /// <summary>
    /// Renumbers all line items to have sequential sort orders starting from 0
    /// </summary>
    private void RenumberLineItems()
    {
        for (int i = 0; i < _lineItems.Count; i++)
        {
            _lineItems[i] = _lineItems[i].SetSortOrder(i);
        }
    }

    /// <summary>
    /// Adds an order-level discount
    /// </summary>
    public void AddDiscount(Discount discount)
    {
        if (discount.Scope == DiscountScope.PerOrder)
        {
            _discounts.Add(new AppliedDiscount(discount.Id, "Some Discount", discount.FixedAmount, discount.Percentage));
        }
        else
        {
            throw new BillingManagementException(ErrorCodes.InvalidDiscountScope, "Only Per-Order discounts can be applied to the invoice.");
        }
    }

    /// <summary>
    /// Adds a surcharge to the invoice
    /// </summary>
    public void AddSurcharge(Surcharge surcharge)
    {
        ArgumentNullException.ThrowIfNull(surcharge);

        _surcharges.Add(new AppliedSurcharge(surcharge.Id, surcharge.Name, surcharge.FixedAmount, surcharge.PercentageRate, surcharge.TaxTreatment));
    }

    public void AddShipping(ShippingInfo shipping) => Shipping = shipping ?? throw new ArgumentNullException(nameof(shipping));

    /// <summary>
    /// Adds a payment to the invoice
    /// </summary>
    public void ProcessPayment(Payment payment)
    {
        var appliedPayment = new AppliedPayment(
            payment.Id,
            payment.Amount);
        _payments.Add(appliedPayment);
        UpdateStatus(payment.PaymentDate);
    }

    /// <summary>
    /// Processes a refund
    /// </summary>
    public void ProcessRefund(Refund refund)
    {
        var balance = GetTotalPaid() - GetTotalRefunded();
        if (refund.Amount > balance)
        {
            throw new BillingManagementException(ErrorCodes.InvalidRefundAmount, $"Refund amount ({refund.Amount:c}) exceeds net payments received: {balance:c}");
        }

        var appliedRefund = new AppliedRefund(
            refund.Id,
            refund.PaymentId,
            refund.Amount,
            refund.SubtotalRefund,
            refund.TaxRefund,
            refund.ShippingRefund);

        _refunds.Add(appliedRefund);
        UpdateStatus(refund.RefundDate);
    }

    /// <summary>
    /// Marks the invoice as sent
    /// </summary>
    public void PostInvoice()
    {
        if (LineItems.Count == 0)
        {
            throw new BillingManagementException(ErrorCodes.CannotPostInvoice, "Cannot post an invoice with no line items.");
        }

        if (Status is InvoiceStatus.Draft or InvoiceStatus.Pending)
        {
            Status = InvoiceStatus.Posted;
        }
    }

    /// <summary>
    /// Cancels the invoice
    /// </summary>
    public void CancelInvoice()
    {
        if (Status != InvoiceStatus.Paid && GetTotalPaid() == 0)
        {
            Status = InvoiceStatus.Cancelled;
        }
        else
        {
            throw new BillingManagementException(ErrorCodes.CannotCancelInvoice, $"This invoice has payments applied: {GetTotalPaid():c}");
        }
    }

    /// <summary>
    /// Updates the invoice status based on payments and balance
    /// </summary>
    public void UpdateStatus(DateOnly effectiveDate)
    {
        var balance = GetBalance();
        var totalRefunded = GetTotalRefunded();
        var total = GetGrandTotal();

        if (totalRefunded >= total)
        {
            Status = InvoiceStatus.Refunded;
        }
        else if (totalRefunded > 0)
        {
            Status = InvoiceStatus.PartiallyRefunded;
        }
        else if (balance <= 0)
        {
            Status = InvoiceStatus.Paid;
        }
        else if (effectiveDate > DueDate)
        {
            Status = InvoiceStatus.Overdue;
        }
        else if (Status == InvoiceStatus.Draft)
        {
            Status = InvoiceStatus.Pending;
        }
    }

    // Methods to calculate amounts

    /// <summary>
    /// Gets the subtotal of all line items before discounts and taxes
    /// </summary>
    public Decimal GetSubtotal()
        => _lineItems.Sum(item => item.GetSubtotal());

    /// <summary>
    /// Gets the total discount amount from line items
    /// </summary>
    public Decimal GetLineItemDiscount()
        => _lineItems.Sum(item => item.GetDiscount());

    /// <summary>
    /// Gets the total order-level discount amount
    /// </summary>
    public Decimal GetOrderDiscount()
        => _discounts.Sum(discount => discount.GetAmount(this));

    /// <summary>
    /// Gets the combined total of order and line item discounts
    /// </summary>
    public Decimal GetDiscountAmount()
        => GetLineItemDiscount() + GetOrderDiscount();

    /// <summary>
    /// Gets the subtotal after applying all discounts
    /// </summary>
    /// <returns></returns>
    public Decimal GetDiscountedSubtotal()
        => GetSubtotal() - GetDiscountAmount();

    /// <summary>
    /// Gets the shipping amount
    /// </summary>
    public Decimal GetShippingAmount()
        => Shipping.ShippingCost;

    /// <summary>
    /// Gets the total tax amount. NOTE: Some surcharges may be taxable, so this is not just the sum of line item taxes.
    /// </summary>
    public Decimal GetTaxAmount()
        => _lineItems.Sum(item => item.GetTaxAmount());

    /// <summary>
    /// Gets the total surcharge amount
    /// </summary>
    public Decimal GetSurchargeAmount()
    {
        var baseAmount = GetDiscountedSubtotal() + GetShippingAmount();

        var amount = 0.00m;
        foreach (var surcharge in _surcharges)
        {
            // All surcharges are calculated based on the base amount (before taxes)
            // The tax treatment will be handled separately when taxes are applied
            amount += surcharge.CalculateSurchargeAmount(baseAmount);
        }

        return amount;
    }

    /// <summary>
    /// Gets the total amount before taxes, including discounts, surcharges, and shipping
    /// </summary>
    /// <returns></returns>
    public Decimal GetTotalAmount()
        => GetSubtotal() - GetDiscountAmount() + GetSurchargeAmount() + GetShippingAmount();

    /// <summary>
    /// Gets the final total amount due
    /// </summary>
    public Decimal GetGrandTotal()
        => GetTotalAmount() + GetTaxAmount();

    /// <summary>
    /// Gets the total amount paid
    /// </summary>
    public Decimal GetTotalPaid() => _payments
        .Sum(p => p.Amount);

    /// <summary>
    /// Gets the total amount refunded
    /// </summary>
    public Decimal GetTotalRefunded()
        => _refunds.Sum(r => r.Amount);

    /// <summary>
    /// Gets the outstanding balance of the invoice
    /// </summary>
    public Decimal GetBalance() => GetGrandTotal() - GetTotalPaid() + GetTotalRefunded();

    /// <summary>
    /// Creates a new <see cref="Invoice"/> instance without setting the Entity Framework navigation
    /// properties so that it can be inserted with out foreign key conflicts with the Customer and
    /// PlaceOfSupply.
    /// </summary>
    /// <param name="invoiceId">The unique identifier for the invoice.</param>
    /// <param name="customer">The customer associated with the invoice. Cannot be <see langword="null"/>.</param>
    /// <param name="placeOfSupply">The province representing the place of supply for the invoice. Cannot be <see langword="null"/>.</param>
    /// <param name="invoiceDate">The date of the invoice.</param>
    /// <returns>A new <see cref="Invoice"/> instance with the specified details.</returns>
    /// <remarks>
    /// We could override the generic repository methods to ignore the navigation properties, which 
    /// might be cleaner, but this method is the simplest thing that works for now. This may change
    /// in the future if we need to do more complex operations during inserts.
    /// </remarks>
    public static Invoice Create(
        InvoiceId invoiceId,
        Customer customer,
        Province placeOfSupply,
        DateOnly invoiceDate)
    {
        var invoice = new Invoice()
        {
            Id = invoiceId,
            CustomerId = customer.Id,
            PlaceOfSupplyId = placeOfSupply.Id,
            InvoiceDate = invoiceDate,
            BillingAddress = customer.BillingAddress ?? Address.Empty,
            ShippingAddress = customer.ShippingAddress ?? Address.Empty
        };

        return invoice;
    }
}
