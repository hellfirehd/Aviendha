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

using Aviendha.MailingListManagement.IdMapping;

namespace Aviendha.MailingListManagement.Listmonk.IdMapping;

public class ListmonkIdMapper : IIdMapper
{
    public Guid MapToGuid(Object remoteId)
    {
        if (remoteId is not Int32 intId)
        {
            throw new ArgumentException("Remote ID must be an Int32.", nameof(remoteId));
        }

        // Convert the Int32 to a byte array
        var bytes = new Byte[16];
        BitConverter.GetBytes(intId).CopyTo(bytes, 0);

        // Return a new Guid based on the byte array
        return new Guid(bytes);
    }
}