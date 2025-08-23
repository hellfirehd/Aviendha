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

using System.Text.RegularExpressions;

namespace Aviendha.BillingManagement.Provinces;

public partial class ProvinceExtensions
{
    public Boolean IsValidProvince(String? code)
    {
        return TryParse(code, out _);
    }

    public Boolean TryParse(String? input, out String code)
    {
        code = ParseInternal(input, throwOnInvalid: false);
        return !String.IsNullOrWhiteSpace(code);
    }

    public String Parse(String? code)
    {
        return ParseInternal(code, throwOnInvalid: true);
    }

    protected virtual String ParseInternal(String? code, Boolean throwOnInvalid = true)
    {
        if (String.IsNullOrWhiteSpace(code))
        {
            return throwOnInvalid
                ? throw new ArgumentException($"'{nameof(code)}' cannot be null or whitespace.", nameof(code))
                : String.Empty;
        }

        var normalized = Replacer().Replace(code, String.Empty).ToUpper(CultureInfo.InvariantCulture);

        var match = normalized switch
        {
            "AB" => "AB",
            "ALBERA" => "AB",
            "ALBERTA" => "AB",
            "BC" => "BC",
            "BRITISH COLUMBIA" => "BC",
            "MB" => "MB",
            "MANITOBA" => "MB",
            "NB" => "NB",
            "NEW BRUNSWICK" => "NB",
            "NL" => "NL",
            "NEWFOUNDLAND" => "NL",
            "NEWFOUNDLAND AND LABRADOR" => "NL",
            "NS" => "NS",
            "NOVA SCOTIA" => "NS",
            "ON" => "ON",
            "ONTARIO" => "ON",
            "PE" => "PE",
            "PRINCE EDWARD ISLAND" => "PE",
            "QC" => "QC",
            "QUBEC" => "QC",
            "QUÉBEC" => "QC",
            "QUEBEC" => "QC",
            "SK" => "SK",
            "SASKATCHEWAN" => "SK",
            "NT" => "NT",
            "NORTHWEST TERRITORIES" => "NT",
            "NU" => "NU",
            "NUNAVUT" => "NU",
            "YT" => "YT",
            "YUKON" => "YT",
            _ => throwOnInvalid
                ? throw new ArgumentOutOfRangeException(nameof(code), code, "Invalid province code")
                : String.Empty
        };

        return match;
    }

    [GeneratedRegex("[^A-Z ]", RegexOptions.IgnoreCase)]
    private static partial Regex Replacer();
}
