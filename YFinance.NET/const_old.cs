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
    public static class Consts_Old
    {
        public static readonly string QUERY1_URL = "https://query1.finance.yahoo.com";
        public static readonly string BASE_URL = "https://query2.finance.yahoo.com";
        public static readonly string ROOT_URL = "https://finance.yahoo.com";
        public static object SENTINEL = new object();

        public static readonly Dictionary<string, List<string>> FUNDAMENTALS_KEYS = new Dictionary<string, List<string>>
        {
            ["financials"] = new List<string>
            {
                "TaxEffectOfUnusualItems", "TaxRateForCalcs", "NormalizedEBITDA", "NormalizedDilutedEPS",
                "NormalizedBasicEPS", "TotalUnusualItems", "TotalUnusualItemsExcludingGoodwill",
                "NetIncomeFromContinuingOperationNetMinorityInterest", "ReconciledDepreciation",
                "ReconciledCostOfRevenue", "EBITDA", "EBIT", "NetInterestIncome", "InterestExpense",
                "InterestIncome", "ContinuingAndDiscontinuedDilutedEPS", "ContinuingAndDiscontinuedBasicEPS",
                "NormalizedIncome", "NetIncomeFromContinuingAndDiscontinuedOperation", "TotalExpenses",
                "RentExpenseSupplemental", "ReportedNormalizedDilutedEPS", "ReportedNormalizedBasicEPS",
                "TotalOperatingIncomeAsReported", "DividendPerShare", "DilutedAverageShares", "BasicAverageShares",
                "DilutedEPS", "DilutedEPSOtherGainsLosses", "TaxLossCarryforwardDilutedEPS",
                "DilutedAccountingChange", "DilutedExtraordinary", "DilutedDiscontinuousOperations",
                "DilutedContinuousOperations", "BasicEPS", "BasicEPSOtherGainsLosses", "TaxLossCarryforwardBasicEPS",
                "BasicAccountingChange", "BasicExtraordinary", "BasicDiscontinuousOperations",
                "BasicContinuousOperations", "DilutedNIAvailtoComStockholders", "AverageDilutionEarnings",
                "NetIncomeCommonStockholders", "OtherunderPreferredStockDividend", "PreferredStockDividends",
                "NetIncome", "MinorityInterests", "NetIncomeIncludingNoncontrollingInterests",
                "NetIncomeFromTaxLossCarryforward", "NetIncomeExtraordinary", "NetIncomeDiscontinuousOperations",
                "NetIncomeContinuousOperations", "EarningsFromEquityInterestNetOfTax", "TaxProvision",
                "PretaxIncome", "OtherIncomeExpense", "OtherNonOperatingIncomeExpenses", "SpecialIncomeCharges",
                "GainOnSaleOfPPE", "GainOnSaleOfBusiness", "OtherSpecialCharges", "WriteOff",
                "ImpairmentOfCapitalAssets", "RestructuringAndMergernAcquisition", "SecuritiesAmortization",
                "EarningsFromEquityInterest", "GainOnSaleOfSecurity", "NetNonOperatingInterestIncomeExpense",
                "TotalOtherFinanceCost", "InterestExpenseNonOperating", "InterestIncomeNonOperating",
                "OperatingIncome", "OperatingExpense", "OtherOperatingExpenses", "OtherTaxes",
                "ProvisionForDoubtfulAccounts", "DepreciationAmortizationDepletionIncomeStatement",
                "DepletionIncomeStatement", "DepreciationAndAmortizationInIncomeStatement", "Amortization",
                "AmortizationOfIntangiblesIncomeStatement", "DepreciationIncomeStatement", "ResearchAndDevelopment",
                "SellingGeneralAndAdministration", "SellingAndMarketingExpense", "GeneralAndAdministrativeExpense",
                "OtherGandA", "InsuranceAndClaims", "RentAndLandingFees", "SalariesAndWages", "GrossProfit",
                "CostOfRevenue", "TotalRevenue", "ExciseTaxes", "OperatingRevenue", "LossAdjustmentExpense",
                "NetPolicyholderBenefitsAndClaims", "PolicyholderBenefitsGross", "PolicyholderBenefitsCeded",
                "OccupancyAndEquipment", "ProfessionalExpenseAndContractServicesExpense", "OtherNonInterestExpense"
            },
            ["balance-sheet"] = new List<string>
            {
                "TreasurySharesNumber", "PreferredSharesNumber", "OrdinarySharesNumber", "ShareIssued", "NetDebt",
                "TotalDebt", "TangibleBookValue", "InvestedCapital", "WorkingCapital", "NetTangibleAssets",
                "CapitalLeaseObligations", "CommonStockEquity", "PreferredStockEquity", "TotalCapitalization",
                "TotalEquityGrossMinorityInterest", "MinorityInterest", "StockholdersEquity",
                "OtherEquityInterest", "GainsLossesNotAffectingRetainedEarnings", "OtherEquityAdjustments",
                "FixedAssetsRevaluationReserve", "ForeignCurrencyTranslationAdjustments",
                "MinimumPensionLiabilities", "UnrealizedGainLoss", "TreasuryStock", "RetainedEarnings",
                "AdditionalPaidInCapital", "CapitalStock", "OtherCapitalStock", "CommonStock", "PreferredStock",
                "TotalPartnershipCapital", "GeneralPartnershipCapital", "LimitedPartnershipCapital",
                "TotalLiabilitiesNetMinorityInterest", "TotalNonCurrentLiabilitiesNetMinorityInterest",
                "OtherNonCurrentLiabilities", "LiabilitiesHeldforSaleNonCurrent", "RestrictedCommonStock",
                "PreferredSecuritiesOutsideStockEquity", "DerivativeProductLiabilities", "EmployeeBenefits",
                "NonCurrentPensionAndOtherPostretirementBenefitPlans", "NonCurrentAccruedExpenses",
                "DuetoRelatedPartiesNonCurrent", "TradeandOtherPayablesNonCurrent",
                "NonCurrentDeferredLiabilities", "NonCurrentDeferredRevenue",
                "NonCurrentDeferredTaxesLiabilities", "LongTermDebtAndCapitalLeaseObligation",
                "LongTermCapitalLeaseObligation", "LongTermDebt", "LongTermProvisions", "CurrentLiabilities",
                "OtherCurrentLiabilities", "CurrentDeferredLiabilities", "CurrentDeferredRevenue",
                "CurrentDeferredTaxesLiabilities", "CurrentDebtAndCapitalLeaseObligation",
                "CurrentCapitalLeaseObligation", "CurrentDebt", "OtherCurrentBorrowings", "LineOfCredit",
                "CommercialPaper", "CurrentNotesPayable", "PensionandOtherPostRetirementBenefitPlansCurrent",
                "CurrentProvisions", "PayablesAndAccruedExpenses", "CurrentAccruedExpenses", "InterestPayable",
                "Payables", "OtherPayable", "DuetoRelatedPartiesCurrent", "DividendsPayable", "TotalTaxPayable",
                "IncomeTaxPayable", "AccountsPayable", "TotalAssets", "TotalNonCurrentAssets",
                "OtherNonCurrentAssets", "DefinedPensionBenefit", "NonCurrentPrepaidAssets",
                "NonCurrentDeferredAssets", "NonCurrentDeferredTaxesAssets", "DuefromRelatedPartiesNonCurrent",
                "NonCurrentNoteReceivables", "NonCurrentAccountsReceivable", "FinancialAssets",
                "InvestmentsAndAdvances", "OtherInvestments", "InvestmentinFinancialAssets",
                "HeldToMaturitySecurities", "AvailableForSaleSecurities",
                "FinancialAssetsDesignatedasFairValueThroughProfitorLossTotal", "TradingSecurities",
                "LongTermEquityInvestment", "InvestmentsinJointVenturesatCost",
                "InvestmentsInOtherVenturesUnderEquityMethod", "InvestmentsinAssociatesatCost",
                "InvestmentsinSubsidiariesatCost", "InvestmentProperties", "GoodwillAndOtherIntangibleAssets",
                "OtherIntangibleAssets", "Goodwill", "NetPPE", "AccumulatedDepreciation", "GrossPPE", "Leases",
                "ConstructionInProgress", "OtherProperties", "MachineryFurnitureEquipment",
                "BuildingsAndImprovements", "LandAndImprovements", "Properties", "CurrentAssets",
                "OtherCurrentAssets", "HedgingAssetsCurrent", "AssetsHeldForSaleCurrent", "CurrentDeferredAssets",
                "CurrentDeferredTaxesAssets", "RestrictedCash", "PrepaidAssets", "Inventory",
                "InventoriesAdjustmentsAllowances", "OtherInventories", "FinishedGoods", "WorkInProcess",
                "RawMaterials", "Receivables", "ReceivablesAdjustmentsAllowances", "OtherReceivables",
                "DuefromRelatedPartiesCurrent", "TaxesReceivable", "AccruedInterestReceivable", "NotesReceivable",
                "LoansReceivable", "AccountsReceivable", "AllowanceForDoubtfulAccountsReceivable",
                "GrossAccountsReceivable", "CashCashEquivalentsAndShortTermInvestments",
                "OtherShortTermInvestments", "CashAndCashEquivalents", "CashEquivalents", "CashFinancial",
                "CashCashEquivalentsAndFederalFundsSold"
            },
            ["cash-flow"] = new List<string>
            {
                "ForeignSales", "DomesticSales", "AdjustedGeographySegmentData", "FreeCashFlow",
                "RepurchaseOfCapitalStock", "RepaymentOfDebt", "IssuanceOfDebt", "IssuanceOfCapitalStock",
                "CapitalExpenditure", "InterestPaidSupplementalData", "IncomeTaxPaidSupplementalData",
                "EndCashPosition", "OtherCashAdjustmentOutsideChangeinCash", "BeginningCashPosition",
                "EffectOfExchangeRateChanges", "ChangesInCash", "OtherCashAdjustmentInsideChangeinCash",
                "CashFlowFromDiscontinuedOperation", "FinancingCashFlow", "CashFromDiscontinuedFinancingActivities",
                "CashFlowFromContinuingFinancingActivities", "NetOtherFinancingCharges", "InterestPaidCFF",
                "ProceedsFromStockOptionExercised", "CashDividendsPaid", "PreferredStockDividendPaid",
                "CommonStockDividendPaid", "NetPreferredStockIssuance", "PreferredStockPayments",
                "PreferredStockIssuance", "NetCommonStockIssuance", "CommonStockPayments", "CommonStockIssuance",
                "NetIssuancePaymentsOfDebt", "NetShortTermDebtIssuance", "ShortTermDebtPayments",
                "ShortTermDebtIssuance", "NetLongTermDebtIssuance", "LongTermDebtPayments", "LongTermDebtIssuance",
                "InvestingCashFlow", "CashFromDiscontinuedInvestingActivities",
                "CashFlowFromContinuingInvestingActivities", "NetOtherInvestingChanges", "InterestReceivedCFI",
                "DividendsReceivedCFI", "NetInvestmentPurchaseAndSale", "SaleOfInvestment", "PurchaseOfInvestment",
                "NetInvestmentPropertiesPurchaseAndSale", "SaleOfInvestmentProperties",
                "PurchaseOfInvestmentProperties", "NetBusinessPurchaseAndSale", "SaleOfBusiness",
                "PurchaseOfBusiness", "NetIntangiblesPurchaseAndSale", "SaleOfIntangibles", "PurchaseOfIntangibles",
                "NetPPEPurchaseAndSale", "SaleOfPPE", "PurchaseOfPPE", "CapitalExpenditureReported",
                "OperatingCashFlow", "CashFromDiscontinuedOperatingActivities",
                "CashFlowFromContinuingOperatingActivities", "TaxesRefundPaid", "InterestReceivedCFO",
                "InterestPaidCFO", "DividendReceivedCFO", "DividendPaidCFO", "ChangeInWorkingCapital",
                "ChangeInOtherWorkingCapital", "ChangeInOtherCurrentLiabilities", "ChangeInOtherCurrentAssets",
                "ChangeInPayablesAndAccruedExpense", "ChangeInAccruedExpense", "ChangeInInterestPayable",
                "ChangeInPayable", "ChangeInDividendPayable", "ChangeInAccountPayable", "ChangeInTaxPayable",
                "ChangeInIncomeTaxPayable", "ChangeInPrepaidAssets", "ChangeInInventory", "ChangeInReceivables",
                "ChangesInAccountReceivables", "OtherNonCashItems", "ExcessTaxBenefitFromStockBasedCompensation",
                "StockBasedCompensation", "UnrealizedGainLossOnInvestmentSecurities", "ProvisionandWriteOffofAssets",
                "AssetImpairmentCharge", "AmortizationOfSecurities", "DeferredTax", "DeferredIncomeTax",
                "DepreciationAmortizationDepletion", "Depletion", "DepreciationAndAmortization",
                "AmortizationCashFlow", "AmortizationOfIntangibles", "Depreciation", "OperatingGainsLosses",
                "PensionAndEmployeeBenefitExpense", "EarningsLossesFromEquityInvestments",
                "GainLossOnInvestmentSecurities", "NetForeignCurrencyExchangeGainLoss", "GainLossOnSaleOfPPE",
                "GainLossOnSaleOfBusiness", "NetIncomeFromContinuingOperations",
                "CashFlowsfromusedinOperatingActivitiesDirect", "TaxesRefundPaidDirect", "InterestReceivedDirect",
                "InterestPaidDirect", "DividendsReceivedDirect", "DividendsPaidDirect", "ClassesofCashPayments",
                "OtherCashPaymentsfromOperatingActivities", "PaymentsonBehalfofEmployees",
                "PaymentstoSuppliersforGoodsandServices", "ClassesofCashReceiptsfromOperatingActivities",
                "OtherCashReceiptsfromOperatingActivities", "ReceiptsfromGovernmentGrants", "ReceiptsfromCustomers"
            }
        };

        public static readonly List<string> QuoteSummaryValidModules = new List<string>
        {
            "summaryProfile",                // contains general information about the company
            "summaryDetail",                 // prices + volume + market cap + etc
            "assetProfile",                  // summaryProfile + company officers
            "fundProfile",
            "price",                         // current prices
            "quoteType",                     // quoteType
            "esgScores",                     // Environmental, social, and governance (ESG) scores, sustainability and ethical performance of companies
            "incomeStatementHistory",
            "incomeStatementHistoryQuarterly",
            "balanceSheetHistory",
            "balanceSheetHistoryQuarterly",
            "cashFlowStatementHistory",
            "cashFlowStatementHistoryQuarterly",
            "defaultKeyStatistics",          // KPIs (PE, enterprise value, EPS, EBITA, and more)
            "financialData",                 // Financial KPIs (revenue, gross margins, operating cash flow, free cash flow, and more)
            "calendarEvents",                // future earnings date
            "secFilings",                    // SEC filings, such as 10K and 10Q reports
            "upgradeDowngradeHistory",       // upgrades and downgrades that analysts have given a company's stock
            "institutionOwnership",          // institutional ownership, holders and shares outstanding
            "fundOwnership",                 // mutual fund ownership, holders and shares outstanding
            "majorDirectHolders",
            "majorHoldersBreakdown",
            "insiderTransactions",           // insider transactions, such as the number of shares bought and sold by company executives
            "insiderHolders",                // insider holders, such as the number of shares held by company executives
            "netSharePurchaseActivity",      // net share purchase activity, such as the number of shares bought and sold by company executives
            "earnings",                      // earnings history
            "earningsHistory",
            "earningsTrend",                 // earnings trend
            "industryTrend",
            "indexTrend",
            "sectorTrend",
            "recommendationTrend",
            "futuresChain"
        };

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