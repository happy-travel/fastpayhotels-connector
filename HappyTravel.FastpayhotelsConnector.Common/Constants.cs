﻿namespace HappyTravel.FastpayhotelsConnector.Common;

public static class Constants
{
    public const string HeaderGrandtype = "grant_type";
    public const string HeaderClientId = "client_id";
    public const string HeaderClientSecret = "client_secret";
    public const string HeaderVersion = "version";
    public const string HeaderUser = "username";
    public const string HeaderPassword = "password";
    public const string ApiVersion = "1";
    public const string DefaultLanguageCode = "en";
    public static readonly string[] Languages = new[] { "ar", "en", "ru", "bg", "de", "el", "es", "fr", "it", "hu", "pl", "pt", "ro", "sr", "tr" };

    public static readonly Dictionary<string, string> CountryCodes = new Dictionary<string, string>()
    {
        { "Afghanistan", "AF" },
        { "Åland Islands", "AX" },
        { "Albania", "AL" },
        { "Algeria", "DZ" },
        { "American Samoa", "AS" },
        { "Andorra", "AD" },
        { "Angola", "AO" },
        { "Anguilla", "AI" },
        { "Antarctica", "AQ" },
        { "Antigua and Barbuda", "AG" },
        { "Argentina", "AR" },
        { "Armenia", "AM" },
        { "Aruba", "AW" },
        { "Australia", "AU" },
        { "Austria", "AT" },
        { "Azerbaijan", "AZ" },
        { "Bahamas", "BS" },
        { "Bahrain", "BH" },
        { "Bangladesh", "BD" },
        { "Barbados", "BB" },
        { "Belarus", "BY" },
        { "Belgium", "BE" },
        { "Belize", "BZ" },
        { "Benin", "BJ" },
        { "Bermuda", "BM" },
        { "Bhutan", "BT" },
        { "Bolivia", "BO" },
        { "\"Bonaire, Sint Eustatius and Saba\"", "BQ" },
        { "Bosnia Herzegovina", "BA" },
        { "Botswana", "BW" },
        { "Bouvet Island", "BV" },
        { "Brazil", "BR" },
        { "British Indian Ocean Territory", "IO" },
        { "BRUNEI", "BN" },
        { "Brunei Darussalam", "BN" },
        { "Bulgaria", "BG" },
        { "Burkina Faso", "BF" },
        { "Burundi", "BI" },
        { "CAPE VERDE", "CV" },
        { "Cambodia", "KH" },
        { "Cameroon", "CM" },
        { "Canada", "CA" },
        { "Cayman Islands", "KY" },
        { "Central African Republic", "CF" },
        { "Chad", "TD" },
        { "Chile", "CL" },
        { "China", "CN" },
        { "Christmas Island", "CX" },
        { "Cocos (Keeling) Islands", "CC" },
        { "Colombia", "CO" },
        { "Comoros", "KM" },
        { "DEMOCRATIC REPUBLIC OF CONGO", "CD" },
        { "Congo (Democratic Republic)", "CD" },
        { "Congo (Republic of)", "CG" },
        { "Cook Islands", "CK" },
        { "Costa Rica", "CR" },
        { "COTE D`IVOIRE(IVORY COAST)", "CI" },
        { "Ivory Coast", "CI" },
        { "Croatia", "HR" },
        { "Cuba", "CU" },
        { "Curacao", "CW" },
        { "North Cyprus", "CY" },
        { "CYPRUS", "CY" },
        { "Czech Republic", "CZ" },
        { "Denmark", "DK" },
        { "Djibouti", "DJ" },
        { "Dominica", "DM" },
        { "Dominican Republic", "DO" },
        { "Ecuador", "EC" },
        { "Egypt", "EG" },
        { "El Salvador", "SV" },
        { "Equatorial Guinea", "GQ" },
        { "Eritrea", "ER" },
        { "Estonia", "EE" },
        { "Eswatini", "SZ" },
        { "Ethiopia", "ET" },
        { "Falkland Islands(Malvinas)", "FK" },
        { "Faroe Islands", "FO" },
        { "Fiji", "FJ" },
        { "Finland", "FI" },
        { "France", "FR" },
        { "French Guiana", "GF" },
        { "French Polynesia", "PF" },
        { "French Southern Territories", "TF" },
        { "Gabon", "GA" },
        { "Gambia", "GM" },
        { "Georgia", "GE" },
        { "Germany", "DE" },
        { "Ghana", "GH" },
        { "Gibraltar", "GI" },
        { "Greece", "GR" },
        { "Greenland", "GL" },
        { "Grenada", "GD" },
        { "Guadeloupe", "GP" },
        { "Guam", "GU" },
        { "Guatemala", "GT" },
        { "Guernsey", "GG" },
        { "Guinea", "GN" },
        { "Guinea-Bissau", "GW" },
        { "Guyana", "GY" },
        { "Haiti", "HT" },
        { "Heard Island and McDonald Islands", "HM" },
        { "Holy See", "VA" },
        { "Honduras", "HN" },
        { "Hong Kong SAR", "HK" },
        { "Hong Kong", "HK" },
        { "Hungary", "HU" },
        { "Iceland", "IS" },
        { "India", "IN" },
        { "Indonesia", "ID" },
        { "Iran", "IR" },
        { "Iraq", "IQ" },
        { "Ireland", "IE" },
        { "Isle of Man", "IM" },
        { "Israel", "IL" },
        { "Italy", "IT" },
        { "Jamaica", "JM" },
        { "Japan", "JP" },
        { "Jersey", "JE" },
        { "Jordan", "JO" },
        { "Kazakhstan", "KZ" },
        { "Kenya", "KE" },
        { "Kiribati", "KI" },
        { "Korea (the Democratic People's Republic of)", "KP" },
        { "South Korea", "KR" },
        { "Kuwait", "KW" },
        { "Kyrgyzstan", "KG" },
        { "Laos", "LA" },
        { "Latvia", "LV" },
        { "Lebanon", "LB" },
        { "Lesotho", "LS" },
        { "Liberia", "LR" },
        { "Libya", "LY" },
        { "Liechtenstein", "LI" },
        { "Lithuania", "LT" },
        { "Luxembourg", "LU" },
        { "Macau", "MO" },
        { "Macedonia", "MK" },
        { "F.Y.R.O. Macedonia", "MK" },
        { "Madagascar", "MG" },
        { "Malawi", "MW" },
        { "Malaysia", "MY" },
        { "Maldives", "MV" },
        { "Mali", "ML" },
        { "Malta", "MT" },
        { "Marshall Islands", "MH" },
        { "Martinique", "MQ" },
        { "Mauritania", "MR" },
        { "Mauritius", "MU" },
        { "Mayotte", "YT" },
        { "Mexico", "MX" },
        { "Micronesia", "FM" },
        { "Federated States of Micronesia", "FM" },
        { "Moldova", "MD" },
        { "Monaco", "MC" },
        { "Mongolia", "MN" },
        { "Montenegro", "ME" },
        { "Montserrat", "MS" },
        { "Morocco", "MA" },
        { "Mozambique", "MZ" },
        { "Myanmar", "MM" },
        { "Namibia", "NA" },
        { "Nauru", "NR" },
        { "Nepal", "NP" },
        { "Netherlands", "NL" },
        { "New Caledonia", "NC" },
        { "New Zealand", "NZ" },
        { "Nicaragua", "NI" },
        { "Niger", "NE" },
        { "Nigeria", "NG" },
        { "Niue", "NU" },
        { "Norfolk Island", "NF" },
        { "Northern Mariana Islands", "MP" },
        { "Norway", "NO" },
        { "Oman", "OM" },
        { "Pakistan", "PK" },
        { "Palau", "PW" },
        { "Palestine, State of", "PS" },
        { "Panama", "PA" },
        { "Papua New Guinea", "PG" },
        { "Paraguay", "PY" },
        { "Peru", "PE" },
        { "Philippines", "PH" },
        { "Pitcairn", "PN" },
        { "Poland", "PL" },
        { "Portugal", "PT" },
        { "Puerto Rico", "PR" },
        { "Qatar", "QA" },
        { "Reunion", "RE" },
        { "Romania", "RO" },
        { "Russia", "RU" },
        { "Rwanda", "RW" },
        { "Saint Barthelemy", "BL" },
        { "Saint Helena Ascension Island Tristan da Cunha", "SH" },
        { "St. Kitts and Nevis", "KN" },
        { "Saint Kitts and Nevis", "KN" },
        { "ST LUCIA", "LC" },
        { "Saint Lucia", "LC" },
        { "Sint Maarten", "MF" },
        { "Saint Pierre and Miquelon", "PM" },
        { "Saint Vincent and the Grenadines", "VC" },
        { "Samoa", "WS" },
        { "San Marino", "SM" },
        { "Sao Tome and Principe", "ST" },
        { "Saudi Arabia", "SA" },
        { "Senegal", "SN" },
        { "Serbia", "RS" },
        { "Seychelles", "SC" },
        { "Sierra Leone", "SL" },
        { "Singapore", "SG" },
        { "Sint Maarten (Dutch part)", "SX" },
        { "Slovakia", "SK" },
        { "Slovenia", "SI" },
        { "Solomon Islands", "SB" },
        { "Somalia", "SO" },
        { "South Africa", "ZA" },
        { "South Georgia and the South Sandwich Islands", "GS" },
        { "South Sudan", "SS" },
        { "Spain", "ES" },
        { "Sri Lanka", "LK" },
        { "Sudan", "SD" },
        { "Suriname", "SR" },
        { "Svalbard", "SJ" },
        { "Sweden", "SE" },
        { "Swaziland", "CH" },
        { "Switzerland", "CH" },
        { "Syrian Arab Republic", "SY" },
        { "Taiwan", "TW" },
        { "Tajikistan", "TJ" },
        { "Tanzania", "TZ" },
        { "Thailand", "TH" },
        { "Timor-Leste", "TL" },
        { "Togo", "TG" },
        { "Tokelau", "TK" },
        { "Tonga", "TO" },
        { "Trinidad and Tobago", "TT" },
        { "Tunisia", "TN" },
        { "Turkey", "TR" },
        { "Turkmenistan", "TM" },
        { "TURKS / CAICOS", "TC" },
        { "Turks and Caicos Islands", "TC" },
        { "Tuvalu", "TV" },
        { "Uganda", "UG" },
        { "Ukraine", "UA" },
        { "United Arab Emirates", "AE" },
        { "United Kingdom", "GB" },
        { "United States Minor Outlying Islands", "UM" },
        { "United States", "US" },
        { "Uruguay", "UY" },
        { "Uzbekistan", "UZ" },
        { "Vanuatu", "VU" },
        { "Venezuela", "VE" },
        { "Vietnam", "VN" },
        { "British Virgin Islands", "VG" },
        { "U.S. VIRGIN ISLANDS", "VI" },
        { "Virgin Islands (USA)", "VI" },
        { "Wallis and Futuna", "WF" },
        { "Western Sahara", "EH" },
        { "Yemen", "YE" },
        { "Zambia", "ZM" },
        { "Zimbabwe", "ZW" }
    };
}
