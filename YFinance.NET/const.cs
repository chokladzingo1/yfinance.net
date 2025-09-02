using System;
using System.Collections.Generic;

namespace yfinance
{
    public static class Consts
    {
        // URLs
        public const string QUERY1_URL = "https://query1.finance.yahoo.com";
        public const string BASE_URL = "https://query2.finance.yahoo.com";
        public const string ROOT_URL = "https://finance.yahoo.com";

        // Sentinel object (use a unique object instance)
        public static readonly object SENTINEL = new object();

        // Fundamentals keys
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

        // Price column names
        public static readonly List<string> PriceColNames = new List<string>
        {
            "Open", "High", "Low", "Close", "Adj Close"
        };

        // Quote summary valid modules
        public static readonly List<string> QuoteSummaryValidModules = new List<string>
        {
            "summaryProfile",
            "summaryDetail",
            "assetProfile",
            "fundProfile",
            "price",
            "quoteType",
            "esgScores",
            "incomeStatementHistory",
            "incomeStatementHistoryQuarterly",
            "balanceSheetHistory",
            "balanceSheetHistoryQuarterly",
            "cashFlowStatementHistory",
            "cashFlowStatementHistoryQuarterly",
            "defaultKeyStatistics",
            "financialData",
            "calendarEvents",
            "secFilings",
            "upgradeDowngradeHistory",
            "institutionOwnership",
            "fundOwnership",
            "majorDirectHolders",
            "majorHoldersBreakdown",
            "insiderTransactions",
            "insiderHolders",
            "netSharePurchaseActivity",
            "earnings",
            "earningsHistory",
            "earningsTrend",
            "industryTrend",
            "indexTrend",
            "sectorTrend",
            "recommendationTrend",
            "futuresChain"
        };

        // User agents
        public static readonly List<string> UserAgents = new List<string>
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

        // Price column names
        //public static readonly List<string> PriceColNames = new List<string>
        //{
        //    "Open", "High", "Low", "Close", "Adj Close"
        //};

        //// Quote summary valid modules
        //public static readonly List<string> QuoteSummaryValidModules = new List<string>
        //{
        //    "summaryProfile", "summaryDetail", "assetProfile", "fundProfile", "price", "quoteType", "esgScores",
        //    "incomeStatementHistory", "incomeStatementHistoryQuarterly", "balanceSheetHistory", "balanceSheetHistoryQuarterly",
        //    "cashFlowStatementHistory", "cashFlowStatementHistoryQuarterly", "defaultKeyStatistics", "financialData",
        //    "calendarEvents", "secFilings", "upgradeDowngradeHistory", "institutionOwnership", "fundOwnership",
        //    "majorDirectHolders", "majorHoldersBreakdown", "insiderTransactions", "insiderHolders", "netSharePurchaseActivity",
        //    "earnings", "earningsHistory", "earningsTrend", "industryTrend", "indexTrend", "sectorTrend", "recommendationTrend",
        //    "futuresChain"
        //};


        // --- Screener Maps and Fields ---

        // EQUITY_SCREENER_EQ_MAP (simplified for demonstration; fill out as needed)
        public static readonly Dictionary<string, object> EQUITY_SCREENER_EQ_MAP = new Dictionary<string, object>
        {
            // Only a few keys shown for brevity; fill out all as needed
            ["exchange"] = new Dictionary<string, HashSet<string>>
            {
                ["us"] = new HashSet<string> { "ASE", "BTS", "CXI", "NCM", "NGM", "NMS", "NYQ", "OEM", "OQB", "OQX", "PCX", "PNK", "YHD" },
                ["ca"] = new HashSet<string> { "CNQ", "NEO", "TOR", "VAN" },
                // ... add all other regions ...
            },
            ["sector"] = new HashSet<string>
            {
                "Basic Materials", "Industrials", "Communication Services", "Healthcare",
                "Real Estate", "Technology", "Energy", "Utilities", "Financial Services",
                "Consumer Defensive", "Consumer Cyclical"
            },
            ["peer_group"] = new HashSet<string>
            {
                "US Fund Equity Energy", "US CE Convertibles", "EAA CE UK Large-Cap Equity"
                // ... add all peer groups as needed ...
            },
            ["region"] = new HashSet<string>
            {
                "us", "ca", "gb", "de", "fr", "jp", "au", "in", "cn", "hk"
                // ... add all regions as needed ...
            }
        };

        // FUND_SCREENER_EQ_MAP (simplified)
        public static readonly Dictionary<string, object> FUND_SCREENER_EQ_MAP = new Dictionary<string, object>
        {
            ["exchange"] = new Dictionary<string, HashSet<string>>
            {
                ["us"] = new HashSet<string> { "NAS" }
            }
        };

        // COMMON_SCREENER_FIELDS
        public static readonly Dictionary<string, HashSet<string>> COMMON_SCREENER_FIELDS = new Dictionary<string, HashSet<string>>
        {
            ["price"] = new HashSet<string>
            {
                "eodprice", "intradaypricechange", "intradayprice"
            },
            ["eq_fields"] = new HashSet<string>
            {
                "exchange"
            }
        };

        // FUND_SCREENER_FIELDS
        public static readonly Dictionary<string, HashSet<string>> FUND_SCREENER_FIELDS = new Dictionary<string, HashSet<string>>
        {
            ["eq_fields"] = new HashSet<string>
            {
                "categoryname", "performanceratingoverall", "initialinvestment",
                "annualreturnnavy1categoryrank", "riskratingoverall"
            }
        };

        // Merge FUND_SCREENER_FIELDS with COMMON_SCREENER_FIELDS
        static Consts()
        {
            foreach (var kv in COMMON_SCREENER_FIELDS)
            {
                if (FUND_SCREENER_FIELDS.ContainsKey(kv.Key))
                    FUND_SCREENER_FIELDS[kv.Key].UnionWith(kv.Value);
                else
                    FUND_SCREENER_FIELDS[kv.Key] = new HashSet<string>(kv.Value);
            }
            foreach (var kv in COMMON_SCREENER_FIELDS)
            {
                if (EQUITY_SCREENER_FIELDS.ContainsKey(kv.Key))
                    EQUITY_SCREENER_FIELDS[kv.Key].UnionWith(kv.Value);
                else
                    EQUITY_SCREENER_FIELDS[kv.Key] = new HashSet<string>(kv.Value);
            }
        }

        // EQUITY_SCREENER_FIELDS (partial, fill out as needed)
        public static readonly Dictionary<string, HashSet<string>> EQUITY_SCREENER_FIELDS = new Dictionary<string, HashSet<string>>
        {
            ["eq_fields"] = new HashSet<string>
            {
                "region", "sector", "peer_group"
            },
            ["price"] = new HashSet<string>
            {
                "lastclosemarketcap.lasttwelvemonths", "percentchange", "lastclose52weekhigh.lasttwelvemonths",
                "fiftytwowkpercentchange", "lastclose52weeklow.lasttwelvemonths", "intradaymarketcap"
            },
            ["trading"] = new HashSet<string>
            {
                "beta", "avgdailyvol3m", "pctheldinsider", "pctheldinst", "dayvolume", "eodvolume"
            },
            ["short_interest"] = new HashSet<string>
            {
                "short_percentage_of_shares_outstanding.value", "short_interest.value",
                "short_percentage_of_float.value", "days_to_cover_short.value", "short_interest_percentage_change.value"
            },
            ["valuation"] = new HashSet<string>
            {
                "bookvalueshare.lasttwelvemonths", "lastclosemarketcaptotalrevenue.lasttwelvemonths",
                "lastclosetevtotalrevenue.lasttwelvemonths", "pricebookratio.quarterly", "peratio.lasttwelvemonths",
                "lastclosepricetangiblebookvalue.lasttwelvemonths", "lastclosepriceearnings.lasttwelvemonths", "pegratio_5y"
            },
            ["profitability"] = new HashSet<string>
            {
                "consecutive_years_of_dividend_growth_count", "returnonassets.lasttwelvemonths",
                "returnonequity.lasttwelvemonths", "forward_dividend_per_share", "forward_dividend_yield",
                "returnontotalcapital.lasttwelvemonths"
            },
            ["leverage"] = new HashSet<string>
            {
                "lastclosetevebit.lasttwelvemonths", "netdebtebitda.lasttwelvemonths", "totaldebtequity.lasttwelvemonths",
                "ltdebtequity.lasttwelvemonths", "ebitinterestexpense.lasttwelvemonths", "ebitdainterestexpense.lasttwelvemonths",
                "lastclosetevebitda.lasttwelvemonths", "totaldebtebitda.lasttwelvemonths"
            },
            ["liquidity"] = new HashSet<string>
            {
                "quickratio.lasttwelvemonths", "altmanzscoreusingtheaveragestockinformationforaperiod.lasttwelvemonths",
                "currentratio.lasttwelvemonths", "operatingcashflowtocurrentliabilities.lasttwelvemonths"
            },
            ["income_statement"] = new HashSet<string>
            {
                "totalrevenues.lasttwelvemonths", "netincomemargin.lasttwelvemonths", "grossprofit.lasttwelvemonths",
                "ebitda1yrgrowth.lasttwelvemonths", "dilutedepscontinuingoperations.lasttwelvemonths",
                "quarterlyrevenuegrowth.quarterly", "epsgrowth.lasttwelvemonths", "netincomeis.lasttwelvemonths",
                "ebitda.lasttwelvemonths", "dilutedeps1yrgrowth.lasttwelvemonths", "totalrevenues1yrgrowth.lasttwelvemonths",
                "operatingincome.lasttwelvemonths", "netincome1yrgrowth.lasttwelvemonths", "grossprofitmargin.lasttwelvemonths",
                "ebitdamargin.lasttwelvemonths", "ebit.lasttwelvemonths", "basicepscontinuingoperations.lasttwelvemonths",
                "netepsbasic.lasttwelvemonths", "netepsdiluted.lasttwelvemonths"
            },
            ["balance_sheet"] = new HashSet<string>
            {
                "totalassets.lasttwelvemonths", "totalcommonsharesoutstanding.lasttwelvemonths", "totaldebt.lasttwelvemonths",
                "totalequity.lasttwelvemonths", "totalcurrentassets.lasttwelvemonths", "totalcashandshortterminvestments.lasttwelvemonths",
                "totalcommonequity.lasttwelvemonths", "totalcurrentliabilities.lasttwelvemonths", "totalsharesoutstanding"
            },
            ["cash_flow"] = new HashSet<string>
            {
                "forward_dividend_yield", "leveredfreecashflow.lasttwelvemonths", "capitalexpenditure.lasttwelvemonths",
                "cashfromoperations.lasttwelvemonths", "leveredfreecashflow1yrgrowth.lasttwelvemonths",
                "unleveredfreecashflow.lasttwelvemonths", "cashfromoperations1yrgrowth.lasttwelvemonths"
            },
            ["esg"] = new HashSet<string>
            {
                "esg_score", "environmental_score", "governance_score", "social_score", "highest_controversy"
            }
        };
    }
}