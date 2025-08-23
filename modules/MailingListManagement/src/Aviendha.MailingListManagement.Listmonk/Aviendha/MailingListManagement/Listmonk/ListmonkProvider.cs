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

using Aviendha.MailingListManagement.Listmonk.IdMapping;
using Aviendha.MailingListManagement.Providers;
using CPCA.Listmonk.Api;
using CPCA.Listmonk.Api.Lists;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Domain.Services;

namespace Aviendha.MailingListManagement.Listmonk;

public class ListmonkProvider(ListmonkIdMapper idMapper) : DomainService, IMailingListProvider
{
    private readonly ListmonkIdMapper _idMapper = idMapper;
    private ListmonkApi? _api;

    public String ProviderName => "Listmonk";

    public async Task<IReadOnlyList<MailingListDto>> GetMailingListsAsync(CancellationToken cancellationToken = default)
    {
        var api = await GetApiAsync(cancellationToken);

        var response = await api.Lists.GetAsync(q =>
        {
            q.QueryParameters.PerPage = 100;
            q.QueryParameters.OrderBy = GetOrder_byQueryParameterType.Name;
        }, cancellationToken: cancellationToken);

        var results = response?.Data?.Results?.Select(list => new MailingListDto
        {
            Id = list.Id.HasValue ? _idMapper.MapToGuid(list.Id.Value) : Guid.Empty,
            Name = list.Name!,
        }).ToList() ?? [];

        return results;
    }

    protected async Task<ListmonkApi> GetApiAsync(CancellationToken cancellationToken = default)
    {
        return _api ??= await LazyServiceProvider.GetRequiredService<ListmonkClientFactory>().GetClientAsync();
    }

    public Task<MailingListDto?> GetMailingListAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // Implementation for fetching a specific mailing list from Listmonk
        throw new NotImplementedException();
    }

    public Task<Result> SubscribeAsync(Guid listId, SubscriberDto subscriber, CancellationToken cancellationToken = default)
    {
        // Implementation for subscribing a user to a mailing list in Listmonk
        throw new NotImplementedException();
    }

    public Task<Result> UnsubscribeAsync(Guid listId, SubscriberDto subscriber, CancellationToken cancellationToken = default)
    {
        // Implementation for unsubscribing a user from a mailing list in Listmonk
        throw new NotImplementedException();
    }

    public Task<Result> UpdateSubscriberAsync(SubscriberDto subscriber, CancellationToken cancellationToken = default)
    {
        // Implementation for updating a subscriber in Listmonk
        throw new NotImplementedException();
    }

    public Task<Result> DeleteSubscriberAsync(SubscriberDto subscriber, CancellationToken cancellationToken = default)
    {
        // Implementation for deleting a subscriber in Listmonk
        throw new NotImplementedException();
    }
}