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

using Volo.Abp.Modularity;

namespace Aviendha.MailingListManagement.Samples;

/* Write your custom repository tests like that, in this project, as abstract classes.
 * Then inherit these abstract classes from EF Core & MongoDB test projects.
 * In this way, both database providers are tests with the same set tests.
 */
public abstract class SampleRepository_Tests<TStartupModule> : MailingListManagementTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    //private readonly ISampleRepository _sampleRepository;

    protected SampleRepository_Tests()
    {
        //_sampleRepository = GetRequiredService<ISampleRepository>();
    }

    [Fact]
    public Task Method1Async()
    {
        return Task.CompletedTask;
    }
}