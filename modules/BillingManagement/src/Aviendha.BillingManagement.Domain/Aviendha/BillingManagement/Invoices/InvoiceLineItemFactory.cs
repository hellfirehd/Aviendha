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

using Aviendha.BillingManagement.Items;
using Volo.Abp.DependencyInjection;

namespace Aviendha.BillingManagement.Invoices;

[ExposeServices(typeof(IInvoiceLineItemFactory))]
public abstract class InvoiceLineItemFactory<T> : IInvoiceLineItemFactory, ITransientDependency
    where T : Item
{
    public Boolean CanCreate(Item item) => item is T;

    protected abstract ItemType ItemType { get; }
    public Task<LineItem> CreateAsync(Item item, DateOnly invoiceDate, CancellationToken cancellationToken = default)
    {
        if (!CanCreate(item))
        {
            throw new ArgumentException($"{nameof(Item.ItemType)} must be of type {ItemType}.", nameof(item));
        }

        OnBeforeCreate((T)item);

        var itemSnapshot = new ItemSnapshot
        {
            ItemId = item.Id,
            SKU = item.SKU,
            Name = item.Name,
            Description = item.Description,
            UnitPrice = item.GetUnitPrice(invoiceDate),
            UnitType = item.UnitType,
            ItemType = item.ItemType,
            ItemCategory = item.ItemCategory,
            TaxCode = item.TaxCode
        };

        var invoiceItem = new LineItem(itemSnapshot, quantity: 1.0m, sortOrder: 0);

        OnAfterCreate((T)item, ref invoiceItem);

        return Task.FromResult(invoiceItem);
    }

    protected virtual Task OnBeforeCreate(T item) => Task.CompletedTask;
    protected virtual Task OnAfterCreate(T item, ref LineItem invoiceItem) => Task.CompletedTask;
}
