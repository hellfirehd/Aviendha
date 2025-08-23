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

namespace Aviendha.Microservices;

public static class AviendhaMicroserviceKeys
{
    public static String AspireRedis { get; set; } = AviendhaMicroserviceKeyDefaults.AspireRedis;
    public static String Authority { get; set; } = AviendhaMicroserviceKeyDefaults.Authority;
    public static String CorsOrigins { get; set; } = AviendhaMicroserviceKeyDefaults.CorsOrigins;
    public static String DataProtectionKey { get; set; } = AviendhaMicroserviceKeyDefaults.DataProtectionKey;
    public static String DistributedCacheKeyPrefix { get; set; } = AviendhaMicroserviceKeyDefaults.DistributedCacheKeyPrefix;
    public static String RedisConfiguration { get; set; } = AviendhaMicroserviceKeyDefaults.RedisConfiguration;
    public static String RequireHttpsMetadata { get; set; } = AviendhaMicroserviceKeyDefaults.RequireHttpsMetadata;
}
