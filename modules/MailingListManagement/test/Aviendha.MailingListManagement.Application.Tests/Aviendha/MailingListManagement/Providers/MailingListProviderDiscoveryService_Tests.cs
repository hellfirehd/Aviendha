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

using Microsoft.Extensions.DependencyInjection;

namespace Aviendha.MailingListManagement.Providers;

public class MailingListProviderDiscoveryService_Tests : MailingListManagementApplicationTestBase<MailingListManagementApplicationTestModule>
{
    [Fact]
    public void Should_Return_All_Registered_Providers()
    {
        // Arrange
        var mockProvider1 = new Mock<IMailingListProvider>();
        mockProvider1.Setup(p => p.ProviderName).Returns("Provider1");

        var mockProvider2 = new Mock<IMailingListProvider>();
        mockProvider2.Setup(p => p.ProviderName).Returns("Provider2");

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient(_ => mockProvider1.Object);
        serviceCollection.AddTransient(_ => mockProvider2.Object);

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var discoveryService = new MailingListProviderDiscoveryService(serviceProvider);

        // Act
        var providers = discoveryService.GetAvailableProviders();

        // Assert
        providers.ShouldContain("Provider1");
        providers.ShouldContain("Provider2");
        providers.Count.ShouldBe(2);
    }
}