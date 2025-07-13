using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

public class YahooFinanceUtils
{
    public static (DataTable dividends, DataTable splits, DataTable capitalGains) ParseActions(Dictionary<string, object> data)
    {
        DataTable dividends = null;
        DataTable capitalGains = null;
        DataTable splits = null;

        if (data.ContainsKey("events"))
        {
            var events = data["events"] as Dictionary<string, object>;

            // Dividends
            if (events.ContainsKey("dividends") && ((Dictionary<string, object>)events["dividends"]).Count > 0)
            {
                var dividendsDict = (Dictionary<string, object>)events["dividends"];
                dividends = new DataTable();
                dividends.Columns.Add("date", typeof(DateTime));
                dividends.Columns.Add("Dividends", typeof(double));
                if (dividendsDict.Values.First() is Dictionary<string, object> firstDiv && firstDiv.ContainsKey("currency"))
                    dividends.Columns.Add("currency", typeof(string));

                foreach (var entry in dividendsDict.Values)
                {
                    var rowDict = (Dictionary<string, object>)entry;
                    var row = dividends.NewRow();
                    row["date"] = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(rowDict["date"])).UtcDateTime;
                    row["Dividends"] = Convert.ToDouble(rowDict["amount"]);
                    if (rowDict.ContainsKey("currency"))
                        row["currency"] = rowDict["currency"]?.ToString();
                    dividends.Rows.Add(row);
                }

                // Remove currency column if all values are empty
                if (dividends.Columns.Contains("currency") &&
                    dividends.AsEnumerable().All(r => string.IsNullOrEmpty(r.Field<string>("currency"))))
                {
                    dividends.Columns.Remove("currency");
                }

                dividends.DefaultView.Sort = "date ASC";
                dividends = dividends.DefaultView.ToTable();
            }

            // Capital Gains
            if (events.ContainsKey("capitalGains") && ((Dictionary<string, object>)events["capitalGains"]).Count > 0)
            {
                var gainsDict = (Dictionary<string, object>)events["capitalGains"];
                capitalGains = new DataTable();
                capitalGains.Columns.Add("date", typeof(DateTime));
                capitalGains.Columns.Add("Capital Gains", typeof(double));

                foreach (var entry in gainsDict.Values)
                {
                    var rowDict = (Dictionary<string, object>)entry;
                    var row = capitalGains.NewRow();
                    row["date"] = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(rowDict["date"])).UtcDateTime;
                    row["Capital Gains"] = Convert.ToDouble(rowDict["amount"]);
                    capitalGains.Rows.Add(row);
                }

                capitalGains.DefaultView.Sort = "date ASC";
                capitalGains = capitalGains.DefaultView.ToTable();
            }

            // Splits
            if (events.ContainsKey("splits") && ((Dictionary<string, object>)events["splits"]).Count > 0)
            {
                var splitsDict = (Dictionary<string, object>)events["splits"];
                splits = new DataTable();
                splits.Columns.Add("date", typeof(DateTime));
                splits.Columns.Add("Stock Splits", typeof(double));

                foreach (var entry in splitsDict.Values)
                {
                    var rowDict = (Dictionary<string, object>)entry;
                    var row = splits.NewRow();
                    row["date"] = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(rowDict["date"])).UtcDateTime;
                    double numerator = Convert.ToDouble(rowDict["numerator"]);
                    double denominator = Convert.ToDouble(rowDict["denominator"]);
                    row["Stock Splits"] = numerator / denominator;
                    splits.Rows.Add(row);
                }

                splits.DefaultView.Sort = "date ASC";
                splits = splits.DefaultView.ToTable();
            }
        }

        // Ensure non-null DataTables
        if (dividends == null)
        {
            dividends = new DataTable();
            dividends.Columns.Add("Dividends", typeof(double));
        }
        if (capitalGains == null)
        {
            capitalGains = new DataTable();
            capitalGains.Columns.Add("Capital Gains", typeof(double));
        }
        if (splits == null)
        {
            splits = new DataTable();
            splits.Columns.Add("Stock Splits", typeof(double));
        }

        return (dividends, splits, capitalGains);
    }
}
// ...existing code...