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
using Aviendha.BillingManagement.Taxes;

namespace Aviendha.BillingManagement.Invoices;

/// <summary>
/// Application service for invoice operations
/// </summary>
public class InvoiceApplicationService(
    ICustomerManager customerManager,
    IInvoiceManager invoiceManager,
    ILineItemFactory lineItemFactory,
    IProvinceRepository provinceManager,
    IItemRepository itemRepository,
    ITaxManager taxProvider)
    : BillingManagementApplicationService, IInvoiceApplicationService
{
    public ICustomerManager CustomerManager { get; } = customerManager;
    public IInvoiceManager InvoiceManager { get; } = invoiceManager;
    public ILineItemFactory LineItemFactory { get; } = lineItemFactory;
    public IProvinceRepository ProvinceRepository { get; } = provinceManager;
    public IItemRepository ItemRepository { get; } = itemRepository;
    private ITaxManager TaxProvider { get; } = taxProvider;

    /// <summary>
    /// Creates a new invoice, returning the ID of the created invoice.
    /// </summary>
    public async Task<Result<InvoiceId>> CreateInvoiceAsync(CreateInvoice command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var customer = await CustomerManager.GetCustomerAsync(command.CustomerId, cancellationToken);

        var lineItems = await CreateLineItemsAsync(command.InvoiceDate, command.Items, cancellationToken);

        var invoiceId = await InvoiceManager.CreateInvoiceAsync(customer.Id, lineItems, cancellationToken);

        return Result.Success(invoiceId);
    }

    /// <summary>
    /// Gets an invoice by ID
    /// </summary>
    public async Task<InvoiceDto> GetInvoiceAsync(InvoiceId invoiceId, CancellationToken cancellationToken = default)
    {
        var invoice = await GetInvoiceOrThrowAsync(invoiceId, cancellationToken);

        var response = ObjectMapper.Map<Invoice, InvoiceDto>(invoice);

        return response;
    }

    public Task<IEnumerable<InvoiceDto>> GetInvoicesAsync(InvoiceFilter filter, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task<Result> AddDiscountsAsync(AddDiscount command, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task<Result> AddSurchargesAsync(AddSurcharge command, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task<Result> CancelInvoiceAsync(CancelInvoice command, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task<Result> PostInvoiceAsync(PostInvoice command, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    /// <summary>
    /// Adds an item to an invoice
    /// </summary>
    public async Task<Result> AddLineItemAsync(AddLineItem command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var invoice = await GetInvoiceOrThrowAsync(command.InvoiceId, cancellationToken);

        var lineItem = await CreateLineItemAsync(invoice.InvoiceDate, command.ItemId, command.Quantity, cancellationToken);

        invoice.AddLineItem(lineItem);

        await InvoiceManager.UpdateInvoiceAsync(invoice, cancellationToken);

        return Result.Success();
    }

    private async Task<Invoice> GetInvoiceOrThrowAsync(InvoiceId invoiceId, CancellationToken cancellationToken = default)
    {
        return await InvoiceManager.GetInvoiceAsync(invoiceId, cancellationToken)
            ?? throw new ArgumentException($"Invoice {invoiceId} not found");
    }

    private async Task<IEnumerable<LineItem>> CreateLineItemsAsync(DateOnly invoiceDate, IEnumerable<CreateLineItem> items, CancellationToken cancellationToken = default)
    {
        var lineItems = new List<LineItem>();

        foreach (var itemRequest in items)
        {
            var item = await CreateLineItemAsync(invoiceDate, itemRequest.ItemId, itemRequest.Quantity, cancellationToken)
                ?? throw new ArgumentException($"Item {itemRequest.ItemId} not found");

            lineItems.Add(item);
        }

        return lineItems;
    }

    private async Task<LineItem> CreateLineItemAsync(DateOnly invoiceDate, ItemId itemId, Decimal quantity, CancellationToken cancellationToken = default)
    {
        var item = await ItemRepository.GetAsync(itemId, cancellationToken: cancellationToken)
            ?? throw new ArgumentException("Invalid item ID.");

        var lineItem = await LineItemFactory.CreateAsync(item, invoiceDate, cancellationToken);

        return lineItem.SetQuantity(quantity);
    }

    /// <summary>
    /// Removes a line item from an invoice by index
    /// </summary>
    public async Task<Result> RemoveLineItemAsync(RemoveLineItem command, CancellationToken cancellationToken = default)
    {
        var invoice = await GetInvoiceOrThrowAsync(command.InvoiceId, cancellationToken);

        if (command.LineItemIndex < 0 || command.LineItemIndex >= invoice.LineItems.Count)
        {
            return Result.Failure("Invalid line item index.");
        }

        invoice.RemoveLineItem(command.LineItemIndex);

        await InvoiceManager.UpdateInvoiceAsync(invoice, cancellationToken);

        return Result.Success();
    }

    /// <summary>
    /// Updates the quantity of a line item
    /// </summary>
    public async Task<Result> UpdateLineItemQuantityAsync(UpdateLineItemQuantity command, CancellationToken cancellationToken = default)
    {
        var invoice = await GetInvoiceOrThrowAsync(command.InvoiceId, cancellationToken);

        if (command.LineItemIndex < 0 || command.LineItemIndex >= invoice.LineItems.Count)
        {
            return Result.Failure("Invalid line item index.");
        }

        invoice.UpdateLineItemQuantity(command.LineItemIndex, command.NewQuantity);

        await InvoiceManager.UpdateInvoiceAsync(invoice, cancellationToken);

        return Result.Success();
    }

    /// <summary>
    /// Moves a line item up in the order
    /// </summary>
    public async Task<Result> MoveLineItemUpAsync(MoveLineItemUp command, CancellationToken cancellationToken = default)
    {
        var invoice = await GetInvoiceOrThrowAsync(command.InvoiceId, cancellationToken);

        invoice.MoveLineItemUp(command.LineItemIndex);

        await InvoiceManager.UpdateInvoiceAsync(invoice, cancellationToken);

        return Result.Success();
    }

    /// <summary>
    /// Moves a line item down in the order
    /// </summary>
    public async Task<Result> MoveLineItemDownAsync(MoveLineItemDown command, CancellationToken cancellationToken = default)
    {
        var invoice = await GetInvoiceOrThrowAsync(command.InvoiceId, cancellationToken);

        invoice.MoveLineItemDown(command.LineItemIndex);

        await InvoiceManager.UpdateInvoiceAsync(invoice, cancellationToken);

        return Result.Success();
    }

    /// <summary>
    /// Moves a line item to a specific position
    /// </summary>
    public async Task<Result> MoveLineItemToPositionAsync(MoveLineItemToPosition command, CancellationToken cancellationToken = default)
    {
        var invoice = await GetInvoiceOrThrowAsync(command.InvoiceId, cancellationToken);

        invoice.MoveLineItemToPosition(command.FromIndex, command.ToIndex);

        await InvoiceManager.UpdateInvoiceAsync(invoice, cancellationToken);

        return Result.Success();
    }

    /// <summary>
    /// Processes a payment for an invoice
    /// </summary>
    public async Task<Result> ProcessPaymentAsync(ProcessPayment command, CancellationToken cancellationToken = default)
    {
        var invoice = await GetInvoiceOrThrowAsync(command.InvoiceId, cancellationToken);

        var payment = new Payment(command.Amount, DateOnly.FromDateTime(DateTime.UtcNow), String.Empty)
        {
            Notes = command.Notes,
            GatewayTransactionId = command.GatewayTransactionId ?? String.Empty,
            GatewayName = command.GatewayName ?? String.Empty
        };

        invoice.ProcessPayment(payment);

        await InvoiceManager.UpdateInvoiceAsync(invoice, cancellationToken);

        return Result.Success();
    }

    /// <summary>
    /// Processes a refund for an invoice
    /// </summary>
    public async Task<Result> ProcessRefundAsync(ProcessRefund command, CancellationToken cancellationToken = default)
    {
        var invoice = await GetInvoiceOrThrowAsync(command.InvoiceId, cancellationToken);

        var refund = new Refund(command.OriginalPaymentId, command.Amount, command.Reason);

        invoice.ProcessRefund(refund);

        await InvoiceManager.UpdateInvoiceAsync(invoice, cancellationToken);

        return Result.Success();
    }

    public Task<Result> UpdateLineItemQuantityAsync(UpdateLineItemQuantity updateLineItemQuantity) => throw new NotImplementedException();
    public Task<Result> MoveLineItemUpAsync(MoveLineItemUp moveLineItemUp) => throw new NotImplementedException();
    public Task<Result> MoveLineItemDownAsync(MoveLineItemDown moveLineItemDown) => throw new NotImplementedException();
    public Task<Result> MoveLineItemToPositionAsync(MoveLineItemToPosition moveLineItemToPosition) => throw new NotImplementedException();
}
