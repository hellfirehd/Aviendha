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

using Aviendha.BillingManagement;
using Aviendha.BillingManagement.Invoices;
using Microsoft.AspNetCore.Components;

namespace Components;
public partial class InvoiceLineItemsEditor
{
    [Inject] private IInvoiceApplicationService InvoiceService { get; set; } = default!;
    [Parameter] public InvoiceId InvoiceId { get; set; }

    private InvoiceDto? Invoice;
    private decimal[] quantities = [];
    private int? draggingIndex;

    protected override async Task OnInitializedAsync()
    {
        // Load invoice - single query gets everything including LineItems as owned entities!
        Invoice = await InvoiceService.GetInvoiceAsync(InvoiceId);
        quantities = Invoice.LineItems.Select(li => li.Quantity).ToArray();
    }

    private async Task UpdateQuantity(int index, decimal newQuantity)
    {
        await InvoiceService.UpdateLineItemQuantityAsync(new UpdateLineItemQuantity
        {
            InvoiceId = InvoiceId,
            LineItemIndex = index,
            NewQuantity = newQuantity
        });
        await RefreshInvoice();
    }

    private async Task MoveUp(int index)
    {
        await InvoiceService.MoveLineItemUpAsync(new MoveLineItemUp
        {
            InvoiceId = InvoiceId,
            LineItemIndex = index
        });
        await RefreshInvoice();
    }

    private async Task MoveDown(int index)
    {
        await InvoiceService.MoveLineItemDownAsync(new MoveLineItemDown
        {
            InvoiceId = InvoiceId,
            LineItemIndex = index
        });
        await RefreshInvoice();
    }

    private async Task Remove(int index)
    {
        await InvoiceService.RemoveLineItemAsync(new RemoveLineItem
        {
            InvoiceId = InvoiceId,
            LineItemIndex = index
        });
        await RefreshInvoice();
    }

    // Drag and drop support
    private void StartDrag(int index)
    {
        draggingIndex = index;
    }

    private void EndDrag()
    {
        draggingIndex = null;
    }

    private async Task Drop(int targetIndex)
    {
        if (draggingIndex.HasValue && draggingIndex.Value != targetIndex)
        {
            await InvoiceService.MoveLineItemToPositionAsync(new MoveLineItemToPosition
            {
                InvoiceId = InvoiceId,
                FromIndex = draggingIndex.Value,
                ToIndex = targetIndex
            });
            await RefreshInvoice();
        }

        draggingIndex = null;
    }

    private async Task RefreshInvoice()
    {
        Invoice = await InvoiceService.GetInvoiceAsync(InvoiceId);
        quantities = Invoice.LineItems.Select(li => li.Quantity).ToArray();
        StateHasChanged();
    }
}
