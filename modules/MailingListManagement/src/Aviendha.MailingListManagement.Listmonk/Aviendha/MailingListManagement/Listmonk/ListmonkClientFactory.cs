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

using CPCA.Listmonk.Api;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using Volo.Abp.DependencyInjection;

namespace Aviendha.MailingListManagement.Listmonk;

public class ListmonkClientFactory(IAuthenticationProvider authenticationProvider, ClientSettings clientSettings, HttpClient httpClient)
    : ITransientDependency
{
    private readonly IAuthenticationProvider _authenticationProvider = authenticationProvider;
    private readonly ClientSettings _clientSettings = clientSettings;
    private readonly HttpClient _httpClient = httpClient;

    public async Task<ListmonkApi> GetClientAsync()
    {
        //var httpMessageHandler = KiotaClientFactory.ChainHandlersCollectionAndGetFirstLink(KiotaClientFactory.GetDefaultHttpMessageHandler(), handlers.ToArray());
        //var httpClient = new HttpClient(httpMessageHandler!);
        //var adapter = new HttpClientRequestAdapter(authProvider, httpClient: httpClient);
        //var client = new PostsClient(adapter); // the name of the client will vary based on your generation parameters

        var baseUrl = await _clientSettings.GetBaseUrlAsync();

        _httpClient.BaseAddress = new Uri(baseUrl);

        return new ListmonkApi(new HttpClientRequestAdapter(_authenticationProvider, httpClient: _httpClient));
    }
}
