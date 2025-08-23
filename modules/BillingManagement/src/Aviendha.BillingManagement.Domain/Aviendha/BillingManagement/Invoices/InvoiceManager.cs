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
using Aviendha.BillingManagement.Provinces;
using Aviendha.BillingManagement.Taxes;
using Volo.Abp.DependencyInjection;

namespace Aviendha.BillingManagement.Invoices;

[ExposeServices(typeof(IInvoiceManager))]
public class InvoiceManager
    : BillingManagementDomainService, IInvoiceManager
{
    public ICustomerRepository CustomerRepository => LazyServiceProvider.LazyGetRequiredService<ICustomerRepository>();
    public IInvoiceRepository InvoiceRepository => LazyServiceProvider.LazyGetRequiredService<IInvoiceRepository>();
    public ILineItemFactory LineItemFactory => LazyServiceProvider.LazyGetRequiredService<ILineItemFactory>();
    public IItemRepository ItemRepository => LazyServiceProvider.LazyGetRequiredService<IItemRepository>();
    public ITaxCodeRepository TaxCodeRepository => LazyServiceProvider.LazyGetRequiredService<ITaxCodeRepository>();
    public ITaxManager TaxProvider => LazyServiceProvider.LazyGetRequiredService<ITaxManager>();
    public IProvinceRepository ProvinceRepository => LazyServiceProvider.LazyGetRequiredService<IProvinceRepository>();

    public async Task<InvoiceId> CreateInvoiceAsync(CustomerId customerId, IEnumerable<LineItem> lineItems, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(lineItems);

        var customer = await CustomerRepository.GetAsync(customerId, cancellationToken: cancellationToken)
            ?? throw new BillingManagementException(ErrorCodes.NotFound, $"Customer with ID {customerId} not found.");

        var placeOfSupply = await ProvinceRepository.GetAsync(customer.BillingAddress.Province, cancellationToken: cancellationToken)
            ?? throw new BillingManagementException(ErrorCodes.NotFound, $"Province with ID {customer.BillingAddress.Province} not found.");

        var invoice = Invoice.Create(InvoiceId.FromGuid(GuidGenerator.Create()), customer, placeOfSupply, Today);

        foreach (var item in lineItems)
        {
            invoice.AddLineItem(item);
        }

        var inserted = await InvoiceRepository.InsertAsync(invoice, autoSave: true, cancellationToken: cancellationToken);

        return inserted.Id;
    }

    public async Task<Invoice> GetInvoiceAsync(InvoiceId invoiceId, CancellationToken cancellationToken = default)
    {
        var invoice = await InvoiceRepository.GetAsync(invoiceId, cancellationToken: cancellationToken)
            ?? throw new BillingManagementException(ErrorCodes.NotFound, $"Invoice with ID {invoiceId} not found.");

        return invoice;
    }

    public Task<Invoice> UpdateInvoiceAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task ApplyRefundAsync(Invoice invoice, Refund refund, CancellationToken cancellationToken = default)
    {
        var invoiceTotal = invoice.GetGrandTotal();
        if (refund.Amount > invoiceTotal)
        {
            throw new InvalidOperationException($"Refund amount ({refund.Amount:c}) cannot exceed invoice total ({invoiceTotal:c}).");
        }

        var proportion = refund.Amount / invoiceTotal;

        //var subtotalRefund = Math.Round(invoice.GetTotalWithDiscountsAndShipping() * proportion, 2);
        var subtotalRefund = Math.Round((invoice.GetSubtotal() - invoice.GetDiscountAmount() + invoice.GetShippingAmount()) * proportion, 2);
        var taxRefund = Math.Round(invoice.GetTaxAmount() * proportion, 2);
        var shippingRefund = refund.IsShippingRefunded ? Math.Round(invoice.Shipping.ShippingCost * proportion, 2) : Decimal.Zero;

        refund.SetAmounts(
            subtotalRefund,
            taxRefund,
            shippingRefund
        );

        invoice.ProcessRefund(refund);

        await InvoiceRepository.UpdateAsync(invoice, autoSave: true, cancellationToken: cancellationToken);
    }

    public async Task AddLineItem(Invoice invoice, LineItem item, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(invoice);
        ArgumentNullException.ThrowIfNull(item);

        invoice.AddLineItem(item);

        await InvoiceRepository.UpdateAsync(invoice, autoSave: true, cancellationToken: cancellationToken);
    }

    public async Task RemoveLineItemAsync(Invoice invoice, LineItem lineItem, CancellationToken cancellationToken = default)
    {

        ArgumentNullException.ThrowIfNull(invoice);
        ArgumentNullException.ThrowIfNull(lineItem);

        invoice.RemoveLineItem(lineItem);

        await InvoiceRepository.UpdateAsync(invoice, autoSave: true, cancellationToken);
    }

    public async Task ApplyTaxesAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(invoice);

        var rates = await TaxProvider.GetTaxesAsync(invoice.PlaceOfSupplyId, invoice.InvoiceDate, cancellationToken);

        await invoice.ApplyTaxesAsync(TaxProvider, cancellationToken);

        await InvoiceRepository.UpdateAsync(invoice, autoSave: true, cancellationToken: cancellationToken);
    }

    public async Task<LineItem> CreateLineItemAsync(ItemId itemId, DateOnly invoiceDate, CancellationToken cancellationToken = default)
    {
        var item = await ItemRepository.GetAsync(itemId, cancellationToken: cancellationToken)
            ?? throw new BillingManagementException(ErrorCodes.NotFound, $"Item with ID {itemId} not found.");

        return await LineItemFactory.CreateAsync(item, invoiceDate, cancellationToken);
    }
}
