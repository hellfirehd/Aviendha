using Volo.Abp.Application.Services;

namespace Aviendha.BillingManagement.Samples;

public interface ISampleBillingAppService : IApplicationService
{
    Task<SampleDto> GetAsync();

    Task<SampleDto> GetAuthorizedAsync();
}
