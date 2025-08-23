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

namespace Aviendha.Logging;

public static class LogHeaders
{
    public const String AccountsInstanceHeader = "x-Aviendha-accounts-instance";
    public const String AccountsVersionHeader = "x-Aviendha-accounts-version";
    public const String ApiInstanceHeader = "x-Aviendha-api-instance";
    public const String ApiVersionHeader = "x-Aviendha-api-version";
    public const String ClientInstanceHeader = "x-Aviendha-client-instance";
    public const String ClientVersionHeader = "x-Aviendha-client-version";
    public const String UserAgent = "user-agent";
}
