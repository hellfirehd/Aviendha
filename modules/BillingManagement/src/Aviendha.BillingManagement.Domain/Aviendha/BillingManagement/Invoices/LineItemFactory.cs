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
using Volo.Abp.Guids;

namespace Aviendha.BillingManagement.Invoices;

[ExposeServices(typeof(ILineItemFactory))]
public class LineItemFactory(ILineItemFactoryProvider manager, IGuidGenerator guidGenerator)
        : ILineItemFactory, ITransientDependency
{
    public ILineItemFactoryProvider Manager { get; } = manager;
    public IGuidGenerator GuidGenerator { get; } = guidGenerator;

    public Boolean CanCreate(Item item) => Manager.Factories.Any(factory => factory.CanCreate(item));

    public async Task<LineItem> CreateAsync(Item item, DateOnly invoiceDate, CancellationToken cancellationToken = default)
    {
        var factory = Manager.Factories.FirstOrDefault(f => f.CanCreate(item))
            ?? throw new InvalidOperationException($"No factory found for item type {item.ItemType}.");

        return await factory.CreateAsync(item, invoiceDate, cancellationToken);
    }
}
