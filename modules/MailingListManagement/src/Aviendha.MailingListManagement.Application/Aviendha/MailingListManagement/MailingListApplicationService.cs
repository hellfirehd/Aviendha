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

using Aviendha.MailingListManagement.Providers;

namespace Aviendha.MailingListManagement;

public class MailingListApplicationService(IMailingListProviderResolver providerResolver) : MailingListManagementApplicationService, IMailingListApplicationService
{
    private readonly IMailingListProviderResolver _providerResolver = providerResolver;

    public String ProviderName => "DynamicProvider"; // Placeholder for dynamic resolution

    public async Task<IReadOnlyList<MailingListDto>> GetMailingListsAsync(CancellationToken cancellationToken = default)
    {
        var provider = await _providerResolver.ResolveProviderAsync(cancellationToken);
        return await provider.GetMailingListsAsync(cancellationToken);
    }

    public async Task<MailingListDto?> GetMailingListAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var provider = await _providerResolver.ResolveProviderAsync(cancellationToken);
        return await provider.GetMailingListAsync(id, cancellationToken);
    }

    public async Task<Result> SubscribeAsync(Guid listId, SubscriberDto subscriber, CancellationToken cancellationToken = default)
    {
        var provider = await _providerResolver.ResolveProviderAsync(cancellationToken);
        return await provider.SubscribeAsync(listId, subscriber, cancellationToken);
    }

    public async Task<Result> UnsubscribeAsync(Guid listId, SubscriberDto subscriber, CancellationToken cancellationToken = default)
    {
        var provider = await _providerResolver.ResolveProviderAsync(cancellationToken);
        return await provider.UnsubscribeAsync(listId, subscriber, cancellationToken);
    }

    public async Task<Result> UpdateSubscriberAsync(SubscriberDto subscriber, CancellationToken cancellationToken = default)
    {
        var provider = await _providerResolver.ResolveProviderAsync(cancellationToken);
        return await provider.UpdateSubscriberAsync(subscriber, cancellationToken);
    }

    public async Task<Result> DeleteSubscriberAsync(SubscriberDto subscriber, CancellationToken cancellationToken = default)
    {
        var provider = await _providerResolver.ResolveProviderAsync(cancellationToken);
        return await provider.DeleteSubscriberAsync(subscriber, cancellationToken);
    }
}