using Aviendha.BillingManagement.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Aviendha.BillingManagement;

public abstract class BillingManagementController : AbpControllerBase
{
    protected BillingManagementController()
    {
        LocalizationResource = typeof(BillingManagementResource);
    }
}
