// Aviendha Billing Management
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

using System.ComponentModel;

namespace Aviendha.BillingManagement.Items;

/// <summary>
/// Categories for different types of goods and services under Canadian tax law
/// </summary>
// ToDo: This should be data from a database or configuration file
public enum ItemCategory
{
    None = 0,

    // Goods Categories
    [Description("General Goods")]
    GeneralGoods = 100,

    [Description("Basic Groceries")]
    BasicGroceries = 101,

    [Description("Prepared Foods")]
    PreparedFoods = 102,

    [Description("Books and Magazines")]
    BooksAndMagazines = 103,

    [Description("Prescription Drugs")]
    PrescriptionDrugs = 104,

    [Description("Medical Devices")]
    MedicalDevices = 105,

    [Description("Children's Clothing")]
    ChildrensClothing = 106,

    [Description("Agricultural Products")]
    AgriculturalProducts = 107,

    [Description("Digital Products")]
    DigitalProducts = 108,

    [Description("Office Supplies")]
    OfficeSupplies = 109,

    // Services Categories
    [Description("General Services")]
    GeneralServices = 200,

    [Description("Professional Services")]
    ProfessionalServices = 201,

    [Description("Healthcare Services")]
    HealthcareServices = 202,

    [Description("Educational Services")]
    EducationalServices = 203,

    [Description("Financial Services")]
    FinancialServices = 204,

    [Description("Legal Services")]
    LegalServices = 205,

    [Description("Transportation Services")]
    TransportationServices = 206,

    [Description("Shipping and Delivery")]
    ShippingAndDelivery = 207,

    [Description("Digital Services")]
    DigitalServices = 208,

    // Special Categories
    [Description("Exports")]
    Exports = 300,

    [Description("Imports")]
    Imports = 301,

    [Description("Real Property")]
    RealProperty = 302,
}
