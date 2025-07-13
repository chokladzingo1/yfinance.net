using System;
using System.Collections.Generic;

namespace yfinance
{
    /// <summary>
    /// Contains constants and mappings used throughout the Yahoo Finance library.
    /// </summary>
    /// <remarks>
    /// This class provides URLs, column names, sector-industry mappings, and user agents.
    /// It is designed to be used as a static reference for various configurations.
    /// </remarks>
    public static class Consts
    {
        public static readonly string QUERY1_URL = "https://query1.finance.yahoo.com";
        public static readonly string BASE_URL = "https://query2.finance.yahoo.com";
        public static readonly string ROOT_URL = "https://finance.yahoo.com";

        public static readonly List<string> PRICE_COLNAMES = new List<string>
    {
        "Open", "High", "Low", "Close", "Adj Close"
    };

        // Example: SECTOR_INDUSTY_MAPPING
        public static readonly Dictionary<string, HashSet<string>> SECTOR_INDUSTRY_MAPPING = new Dictionary<string, HashSet<string>>
    {
        { "energy", new HashSet<string> {
            "oil-gas-integrated",
            "oil-gas-midstream",
            "oil-gas-e-p",
            "oil-gas-equipment-services",
            "oil-gas-refining-marketing",
            "uranium",
            "oil-gas-drilling",
            "thermal-coal"
        }},
        { "financial-services", new HashSet<string> {
            "banks-diversified",
            "credit-services",
            "asset-management",
            "insurance-diversified",
            "banks-regional",
            "capital-markets",
            "financial-data-stock-exchanges",
            "insurance-property-casualty",
            "insurance-brokers",
            "insurance-life",
            "insurance-specialty",
            "mortgage-finance",
            "insurance-reinsurance",
            "shell-companies",
            "financial-conglomerates"
        }},
        { "healthcare", new HashSet<string> {
            "drug-manufacturers-general",
            "healthcare-plans",
            "biotechnology",
            "medical-devices",
            "diagnostics-research"
            // ...add the rest as needed
        }}
        // ...add other sectors as needed
    };

        public static readonly List<string> USER_AGENTS = new List<string>
    {
        // Chrome
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/133.0.0.0 Safari/537.36",
        "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/133.0.0.0 Safari/537.36",
        "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/133.0.0.0 Safari/537.36",

        // Firefox
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:135.0) Gecko/20100101 Firefox/135.0",
        "Mozilla/5.0 (Macintosh; Intel Mac OS X 14.7; rv:135.0) Gecko/20100101 Firefox/135.0",
        "Mozilla/5.0 (X11; Linux i686; rv:135.0) Gecko/20100101 Firefox/135.0",

        // Safari
        "Mozilla/5.0 (Macintosh; Intel Mac OS X 14_7_4) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/18.3 Safari/605.1.15",

        // Edge
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/133.0.0.0 Safari/537.36 Edg/131.0.2903.86"
    };

        // Add other constants and mappings as needed, following the same pattern.
    }
}