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

using Aviendha.BillingManagement.Provinces;
using Aviendha.BillingManagement.Taxes;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace Aviendha.BillingManagement;

public class BillingManagementDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IProvinceRepository _provinceRepository;

    private readonly Dictionary<String, Province> provinces;
    private readonly Dictionary<String, Tax> taxes;

    public BillingManagementDataSeedContributor(
        IProvinceRepository provinceRepository,
        ITaxRepository taxRepository)
    {
        _provinceRepository = provinceRepository;

        taxes = new Dictionary<String, Tax>
        {
            { "GST", new Tax(TaxId.Parse("a3b1e7a0-8a1e-11ee-b9d1-0242ac120002"), "GST", "GST", isGstHst: true)
                .AddTaxRate(0.05M, new DateOnly(2008, 1, 1), DateOnly.MaxValue)
                .AddTaxRate(0.06M, new DateOnly(2006, 7, 1), new DateOnly(2008, 1, 1))
                .AddTaxRate(0.07M, new DateOnly(1991, 1, 1), new DateOnly(2006, 7, 1)) },

            { "BC-PST", new Tax(TaxId.Parse("a3b1e7a1-8a1e-11ee-b9d1-0242ac120002"), "PST", "BC-PST", isGstHst: false)
                .AddTaxRate(0.07M, new DateOnly(2013, 4, 1), DateOnly.MaxValue) },

            { "MB-RST", new Tax(TaxId.Parse("a3b1e7a2-8a1e-11ee-b9d1-0242ac120002"), "RST", "MB-RST", isGstHst: false)
                .AddTaxRate(0.07M, new DateOnly(2019, 7, 1), DateOnly.MaxValue) },

            { "NB-HST", new Tax(TaxId.Parse("a3b1e7a3-8a1e-11ee-b9d1-0242ac120002"), "HST", "NB-HST", isGstHst: true)
                .AddTaxRate(0.15M, new DateOnly(2016, 7, 1), DateOnly.MaxValue) },

            { "NL-HST", new Tax(TaxId.Parse("a3b1e7a4-8a1e-11ee-b9d1-0242ac120002"), "HST", "NL-HST", isGstHst: true)
                .AddTaxRate(0.15M, new DateOnly(2016, 7, 1), DateOnly.MaxValue) },

            { "NS-HST", new Tax(TaxId.Parse("a3b1e7a5-8a1e-11ee-b9d1-0242ac120002"), "HST", "NS-HST", isGstHst: true)
                .AddTaxRate(0.15M, new DateOnly(2010, 7, 1), new DateOnly(2025, 4, 1))
                .AddTaxRate(0.14M, new DateOnly(2025, 4, 1), DateOnly.MaxValue) },

            { "ON-HST", new Tax(TaxId.Parse("a3b1e7a6-8a1e-11ee-b9d1-0242ac120002"), "HST", "ON-HST", isGstHst: true)
                .AddTaxRate(0.13M, new DateOnly(2010, 7, 1), DateOnly.MaxValue) },

            { "PE-HST", new Tax(TaxId.Parse("a3b1e7a7-8a1e-11ee-b9d1-0242ac120002"), "HST", "PE-HST", isGstHst: true)
                .AddTaxRate(0.15M, new DateOnly(2016, 10, 1), DateOnly.MaxValue) },

            { "QC-QST", new Tax(TaxId.Parse("a3b1e7a8-8a1e-11ee-b9d1-0242ac120002"), "QST", "QC-QST", isGstHst: false)
                .AddTaxRate(0.09975M, new DateOnly(2013, 1, 1), DateOnly.MaxValue) },

            { "SK-PST", new Tax(TaxId.Parse("a3b1e7a9-8a1e-11ee-b9d1-0242ac120002"), "PST", "SK-PST", isGstHst: false)
                .AddTaxRate(0.06M, new DateOnly(2017, 3, 23), DateOnly.MaxValue) }
        };

        provinces = new()
        {
            { "AB", new Province(ProvinceId.Parse("f3b1e7a0-8a1e-11ee-b9d1-0242ac120002"), "Alberta", "AB", [taxes["GST"]]) },
            { "BC", new Province(ProvinceId.Parse("f3b1e7a1-8a1e-11ee-b9d1-0242ac120002"), "British Columbia", "BC", [taxes["GST"],taxes["BC-PST"]]) },
            { "MB", new Province(ProvinceId.Parse("f3b1e7a2-8a1e-11ee-b9d1-0242ac120002"), "Manitoba", "MB", [taxes["GST"],taxes["MB-RST"]]) },
            { "NB", new Province(ProvinceId.Parse("f3b1e7a3-8a1e-11ee-b9d1-0242ac120002"), "New Brunswick", "NB", [taxes["NB-HST"]]) },
            { "NL", new Province(ProvinceId.Parse("f3b1e7a4-8a1e-11ee-b9d1-0242ac120002"), "Newfoundland and Labrador", "NL", [taxes["NL-HST"]]) },
            { "NS", new Province(ProvinceId.Parse("f3b1e7a5-8a1e-11ee-b9d1-0242ac120002"), "Nova Scotia", "NS", [taxes["NS-HST"]]) },
            { "NT", new Province(ProvinceId.Parse("f3b1e7a6-8a1e-11ee-b9d1-0242ac120002"), "Northwest Territories", "NT", [taxes["GST"]]) },
            { "NU", new Province(ProvinceId.Parse("f3b1e7a7-8a1e-11ee-b9d1-0242ac120002"), "Nunavut", "NU", [taxes["GST"]]) },
            { "ON", new Province(ProvinceId.Parse("f3b1e7a8-8a1e-11ee-b9d1-0242ac120002"), "Ontario", "ON", [taxes["ON-HST"]]) },
            { "PE", new Province(ProvinceId.Parse("f3b1e7a9-8a1e-11ee-b9d1-0242ac120002"), "Prince Edward Island", "PE", [taxes["PE-HST"]]) },
            { "QC", new Province(ProvinceId.Parse("f3b1e7aa-8a1e-11ee-b9d1-0242ac120002"), "Quebec", "QC", [taxes["GST"], taxes["QC-QST"]]) },
            { "SK", new Province(ProvinceId.Parse("f3b1e7ab-8a1e-11ee-b9d1-0242ac120002"), "Saskatchewan", "SK", [taxes["GST"], taxes["SK-PST"]]) },
            { "YT", new Province(ProvinceId.Parse("f3b1e7ac-8a1e-11ee-b9d1-0242ac120002"), "Yukon Territory", "YT", [taxes["GST"]]) }
        };
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        if (await _provinceRepository.GetCountAsync() == 0)
        {
            await _provinceRepository.InsertManyAsync(provinces.Values, autoSave: true);
        }
    }
}
