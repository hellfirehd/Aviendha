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

namespace Aviendha.MailingListManagement.Providers;

public interface IMailingListProvider
{
    String ProviderName { get; }

    Task<IReadOnlyList<MailingListDto>> GetMailingListsAsync(CancellationToken cancellationToken = default);
    Task<MailingListDto?> GetMailingListAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Result> SubscribeAsync(Guid listId, SubscriberDto subscriber, CancellationToken cancellationToken = default);
    Task<Result> UnsubscribeAsync(Guid listId, SubscriberDto subscriber, CancellationToken cancellationToken = default);

    Task<Result> UpdateSubscriberAsync(SubscriberDto subscriber, CancellationToken cancellationToken = default);
    Task<Result> DeleteSubscriberAsync(SubscriberDto subscriber, CancellationToken cancellationToken = default);
}