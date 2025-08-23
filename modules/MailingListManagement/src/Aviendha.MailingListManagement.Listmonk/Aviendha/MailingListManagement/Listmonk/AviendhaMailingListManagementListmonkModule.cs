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

using Aviendha.Domain;
using Aviendha.MailingListManagement.Providers;
using Aviendha.OpenApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using Microsoft.Kiota.Http.HttpClientLibrary.Middleware.Options;
using Volo.Abp.Modularity;

namespace Aviendha.MailingListManagement.Listmonk;

[DependsOn(typeof(AviendhaOpenApiModule))]
[DependsOn(typeof(AviendhaDddDomainModule))]
[DependsOn(typeof(AviendhaMailingListManagementApplicationContractsModule))]
public class AviendhaMailingListsListmonkModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        context.Services.Configure<AuthenticationSettings>(configuration.GetSection("Listmonk"));
        context.Services.AddTransient<IMailingListProvider, ListmonkProvider>();

        context.Services.Configure<BodyInspectionHandlerOption>(options =>
        {
#if DEBUG
            options.InspectRequestBody = true;
            options.InspectResponseBody = true;
#endif
        });

        context.Services.AddKiotaHandlers();
        context.Services.AddHttpClient<ListmonkClientFactory>().AttachKiotaHandlers();
        context.Services.AddTransient<IAuthenticationProvider, ListmonkClientAuthenticationProvider>();
    }
}
