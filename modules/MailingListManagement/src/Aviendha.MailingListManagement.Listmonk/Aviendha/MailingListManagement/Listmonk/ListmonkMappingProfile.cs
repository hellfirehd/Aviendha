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

using AutoMapper;
using CPCA.Listmonk.Api.Models;

namespace Aviendha.MailingListManagement.Listmonk;

public class ListmonkMappingProfile : Profile
{
    const Boolean True = true;

    public ListmonkMappingProfile()
        : base(nameof(ListmonkMappingProfile))
    {

        CreateMap<Subscriber, NewSubscriber>(MemberList.Destination)
            .ForMember(d => d.Lists, o => o.MapFrom(s => s.Lists!.Select(l => l.Id).ToList()))
            .ForMember(d => d.ListUuids, o => o.Ignore())
            .ForMember(d => d.PreconfirmSubscriptions, o => o.Ignore());
        CreateMap<Subscriber_attribs, NewSubscriber_attribs>();

        CreateMap<Subscriber, UpdateSubscriber>(MemberList.Destination)
            .ForMember(d => d.Lists, o => o.MapFrom(s => s.Lists!.Select(l => l.Id).ToList()))
            .ForMember(n => n.ListUuids, o => o.Ignore())
            .ForMember(n => n.PreconfirmSubscriptions, o => o.Ignore());
        CreateMap<Subscriber_attribs, UpdateSubscriber_attribs>();
    }
}
