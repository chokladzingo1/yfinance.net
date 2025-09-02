using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Linq;
using System.Diagnostics;
using System.Text.Json;

namespace yfinance
{
    public static class Utils
    {
        // Logging (simple version)
        public static void LogDebug(string message)
        {
            Debug.WriteLine("[DEBUG] " + message);
        }

        // ISIN validation
        public static bool IsIsin(string s)
        {
            return Regex.IsMatch(s, "^([A-Z]{2})([A-Z0-9]{9})([0-9])$");
        }

        // Placeholder for ISIN search (requires implementation)
        public static string GetTickerByIsin(string isin)
        {
            // Implement actual lookup logic
            throw new NotImplementedException("ISIN lookup not implemented.");
        }

        // Empty DataTable for OHLCV
        public static DataTable EmptyDf(IEnumerable<DateTime> index = null)
        {
            var dt = new DataTable();
            dt.Columns.Add("Open", typeof(double));
            dt.Columns.Add("High", typeof(double));
            dt.Columns.Add("Low", typeof(double));
            dt.Columns.Add("Close", typeof(double));
            dt.Columns.Add("Adj Close", typeof(double));
            dt.Columns.Add("Volume", typeof(double));
            if (index != null)
            {
                foreach (var date in index)
                {
                    var row = dt.NewRow();
                    dt.Rows.Add(row);
                }
            }
            return dt;
        }

        // Convert snake_case to camelCase
        public static string SnakeCaseToCamelCase(string s)
        {
            var parts = s.Split('_');
            return parts[0] + string.Concat(parts.Skip(1).Select(x => char.ToUpper(x[0]) + x.Substring(1)));
        }

        // Check valid period format (e.g. 1d, 2wk, 3mo, 1y)
        public static bool IsValidPeriodFormat(string period)
        {
            if (period == null) return false;
            return Regex.IsMatch(period, @"^[1-9]\d*(d|wk|mo|y)$");
        }

        // Auto-adjust prices (assumes DataTable with required columns)
        public static DataTable AutoAdjust(DataTable data)
        {
            var df = data.Copy();
            var ratio = new List<double>();
            for (int i = 0; i < df.Rows.Count; i++)
            {
                double adjClose = Convert.ToDouble(df.Rows[i]["Adj Close"]);
                double close = Convert.ToDouble(df.Rows[i]["Close"]);
                ratio.Add(close != 0 ? adjClose / close : 1.0);
            }
            for (int i = 0; i < df.Rows.Count; i++)
            {
                df.Rows[i]["Open"] = Convert.ToDouble(df.Rows[i]["Open"]) * ratio[i];
                df.Rows[i]["High"] = Convert.ToDouble(df.Rows[i]["High"]) * ratio[i];
                df.Rows[i]["Low"] = Convert.ToDouble(df.Rows[i]["Low"]) * ratio[i];
                df.Rows[i]["Close"] = Convert.ToDouble(df.Rows[i]["Adj Close"]);
            }
            df.Columns.Remove("Adj Close");
            return df;
        }

        /// <summary>
        /// Back-adjusted data to mimic true historical prices.
        /// Expects columns: "Open", "High", "Low", "Close", "Adj Close"
        /// </summary>
        public static DataTable BackAdjust(DataTable data)
        {
            // Preserve original column order
            var colOrder = data.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();

            // Copy data
            DataTable df = data.Copy();

            // Compute ratio
            var ratio = new List<double>();
            for (int i = 0; i < df.Rows.Count; i++)
            {
                double adjClose = Convert.ToDouble(df.Rows[i]["Adj Close"]);
                double close = Convert.ToDouble(df.Rows[i]["Close"]);
                ratio.Add(close != 0 ? adjClose / close : double.NaN);
            }

            // Add adjusted columns
            if (!df.Columns.Contains("Adj Open")) df.Columns.Add("Adj Open", typeof(double));
            if (!df.Columns.Contains("Adj High")) df.Columns.Add("Adj High", typeof(double));
            if (!df.Columns.Contains("Adj Low")) df.Columns.Add("Adj Low", typeof(double));

            for (int i = 0; i < df.Rows.Count; i++)
            {
                double r = ratio[i];
                df.Rows[i]["Adj Open"] = Convert.ToDouble(df.Rows[i]["Open"]) * r;
                df.Rows[i]["Adj High"] = Convert.ToDouble(df.Rows[i]["High"]) * r;
                df.Rows[i]["Adj Low"] = Convert.ToDouble(df.Rows[i]["Low"]) * r;
            }

            // Drop original columns
            df.Columns.Remove("Open");
            df.Columns.Remove("High");
            df.Columns.Remove("Low");
            df.Columns.Remove("Adj Close");

            // Rename adjusted columns
            df.Columns["Adj Open"].ColumnName = "Open";
            df.Columns["Adj High"].ColumnName = "High";
            df.Columns["Adj Low"].ColumnName = "Low";

            // Reorder columns as in original if still present
            var finalCols = colOrder.Where(c => df.Columns.Contains(c)).ToList();
            DataTable result = new DataTable();
            foreach (var col in finalCols)
                result.Columns.Add(col, df.Columns[col].DataType);

            foreach (DataRow row in df.Rows)
            {
                var newRow = result.NewRow();
                foreach (var col in finalCols)
                    newRow[col] = row[col];
                result.Rows.Add(newRow);
            }

            return result;
        }

        // Parse quotes from JSON (System.Text.Json)
        public static DataTable ParseQuotes(JsonElement data)
        {
            var timestamps = data.GetProperty("timestamp").EnumerateArray().Select(x => x.GetInt64()).ToList();
            var ohlc = data.GetProperty("indicators").GetProperty("quote")[0];
            var opens = ohlc.GetProperty("open").EnumerateArray().Select(x => x.GetDouble()).ToList();
            var highs = ohlc.GetProperty("high").EnumerateArray().Select(x => x.GetDouble()).ToList();
            var lows = ohlc.GetProperty("low").EnumerateArray().Select(x => x.GetDouble()).ToList();
            var closes = ohlc.GetProperty("close").EnumerateArray().Select(x => x.GetDouble()).ToList();
            var volumes = ohlc.GetProperty("volume").EnumerateArray().Select(x => x.GetInt64()).ToList();

            List<double> adjclose = closes;
            if (data.GetProperty("indicators").TryGetProperty("adjclose", out var adjcloseElem))
            {
                adjclose = adjcloseElem[0].GetProperty("adjclose").EnumerateArray().Select(x => x.GetDouble()).ToList();
            }

            var dt = new DataTable();
            dt.Columns.Add("Date", typeof(DateTime));
            dt.Columns.Add("Open", typeof(double));
            dt.Columns.Add("High", typeof(double));
            dt.Columns.Add("Low", typeof(double));
            dt.Columns.Add("Close", typeof(double));
            dt.Columns.Add("Adj Close", typeof(double));
            dt.Columns.Add("Volume", typeof(long));

            for (int i = 0; i < timestamps.Count; i++)
            {
                var row = dt.NewRow();
                row["Date"] = DateTimeOffset.FromUnixTimeSeconds(timestamps[i]).UtcDateTime;
                row["Open"] = opens[i];
                row["High"] = highs[i];
                row["Low"] = lows[i];
                row["Close"] = closes[i];
                row["Adj Close"] = adjclose[i];
                row["Volume"] = volumes[i];
                dt.Rows.Add(row);
            }
            return dt;
        }

        // Parse actions (dividends, splits, capital gains) from JSON
        public static (DataTable dividends, DataTable splits, DataTable capitalGains) ParseActions(JsonElement data)
        {
            DataTable dividends = null;
            DataTable capitalGains = null;
            DataTable splits = null;

            if (data.TryGetProperty("events", out var events))
            {
                // Dividends
                if (events.TryGetProperty("dividends", out var dividendsElem) && dividendsElem.EnumerateObject().Any())
                {
                    dividends = new DataTable();
                    dividends.Columns.Add("date", typeof(DateTime));
                    dividends.Columns.Add("Dividends", typeof(double));
                    foreach (var entry in dividendsElem.EnumerateObject())
                    {
                        var row = dividends.NewRow();
                        var val = entry.Value;
                        row["date"] = DateTimeOffset.FromUnixTimeSeconds(val.GetProperty("date").GetInt64()).UtcDateTime;
                        row["Dividends"] = val.GetProperty("amount").GetDouble();
                        dividends.Rows.Add(row);
                    }
                    dividends.PrimaryKey = new[] { dividends.Columns["date"] };
                }

                // Capital Gains
                if (events.TryGetProperty("capitalGains", out var gainsElem) && gainsElem.EnumerateObject().Any())
                {
                    capitalGains = new DataTable();
                    capitalGains.Columns.Add("date", typeof(DateTime));
                    capitalGains.Columns.Add("Capital Gains", typeof(double));
                    foreach (var entry in gainsElem.EnumerateObject())
                    {
                        var row = capitalGains.NewRow();
                        var val = entry.Value;
                        row["date"] = DateTimeOffset.FromUnixTimeSeconds(val.GetProperty("date").GetInt64()).UtcDateTime;
                        row["Capital Gains"] = val.GetProperty("amount").GetDouble();
                        capitalGains.Rows.Add(row);
                    }
                    capitalGains.PrimaryKey = new[] { capitalGains.Columns["date"] };
                }

                // Splits
                if (events.TryGetProperty("splits", out var splitsElem) && splitsElem.EnumerateObject().Any())
                {
                    splits = new DataTable();
                    splits.Columns.Add("date", typeof(DateTime));
                    splits.Columns.Add("Stock Splits", typeof(double));
                    foreach (var entry in splitsElem.EnumerateObject())
                    {
                        var row = splits.NewRow();
                        var val = entry.Value;
                        row["date"] = DateTimeOffset.FromUnixTimeSeconds(val.GetProperty("date").GetInt64()).UtcDateTime;
                        double numerator = val.GetProperty("numerator").GetDouble();
                        double denominator = val.GetProperty("denominator").GetDouble();
                        row["Stock Splits"] = numerator / denominator;
                        splits.Rows.Add(row);
                    }
                    splits.PrimaryKey = new[] { splits.Columns["date"] };
                }
            }

            dividends ??= new DataTable();
            if (dividends.Columns.Count == 0) dividends.Columns.Add("Dividends", typeof(double));
            capitalGains ??= new DataTable();
            if (capitalGains.Columns.Count == 0) capitalGains.Columns.Add("Capital Gains", typeof(double));
            splits ??= new DataTable();
            if (splits.Columns.Count == 0) splits.Columns.Add("Stock Splits", typeof(double));

            return (dividends, splits, capitalGains);
        }
        public static DataTable SafeMergeDfs(DataTable dfMain, DataTable dfSub, string interval)
        {
            if (dfSub.Rows.Count == 0)
                throw new Exception("No data to merge");
            if (dfMain.Rows.Count == 0)
                return dfMain;

            // Find the data column in dfSub that is not in dfMain
            var dataCols = dfSub.Columns.Cast<DataColumn>()
                .Select(c => c.ColumnName)
                .Where(c => !dfMain.Columns.Contains(c))
                .ToList();
            if (dataCols.Count != 1)
                throw new Exception("Expected 1 data col");
            string dataCol = dataCols[0];

            // Sort by index (assume "Date" column is the index)
            var mainRows = dfMain.AsEnumerable().OrderBy(r => r.Field<DateTime>("Date")).ToList();
            var subRows = dfSub.AsEnumerable().OrderBy(r => r.Field<DateTime>("Date")).ToList();

            // Determine if intraday
            bool intraday = interval.EndsWith("m") || interval.EndsWith("s");

            // Calculate time delta for interval
            TimeSpan td = IntervalToTimeSpan(interval);

            // Build index arrays
            List<DateTime> mainIndex = mainRows.Select(r => r.Field<DateTime>("Date")).ToList();
            List<DateTime> subIndex = subRows.Select(r => r.Field<DateTime>("Date")).ToList();
            int[] indices = new int[subIndex.Count];

            if (intraday)
            {
                // Use dates only for intraday
                var mainDates = mainIndex.Select(d => d.Date).ToList();
                var subDates = subIndex.Select(d => d.Date).ToList();
                for (int i = 0; i < subDates.Count; i++)
                {
                    int idx = mainDates.FindIndex(d => d >= subDates[i]);
                    indices[i] = idx == -1 ? -1 : idx;
                }
            }
            else
            {
                for (int i = 0; i < subIndex.Count; i++)
                {
                    int idx = mainIndex.FindLastIndex(d => d <= subIndex[i]);
                    indices[i] = idx;
                }
            }

            // Handle out-of-range
            for (int i = 0; i < indices.Length; i++)
            {
                if (indices[i] == -1)
                {
                    // Out-of-range, skip or add empty row as needed
                    // For simplicity, skip in this translation
                    continue;
                }
            }

            // Merge: for each subRow, add its value to the corresponding mainRow
            for (int i = 0; i < subRows.Count; i++)
            {
                int idx = indices[i];
                if (idx == -1) continue;
                var mainRow = mainRows[idx];
                var subRow = subRows[i];
                // Add the new column if not present
                if (!dfMain.Columns.Contains(dataCol))
                    dfMain.Columns.Add(dataCol, subRow.Table.Columns[dataCol].DataType);

                mainRow[dataCol] = subRow[dataCol];
            }

            return dfMain;
        }

        // Helper to convert interval string to TimeSpan
        private static TimeSpan IntervalToTimeSpan(string interval)
        {
            if (interval.EndsWith("m"))
                return TimeSpan.FromMinutes(int.Parse(interval.TrimEnd('m')));
            if (interval.EndsWith("s"))
                return TimeSpan.FromSeconds(int.Parse(interval.TrimEnd('s')));
            if (interval.EndsWith("h"))
                return TimeSpan.FromHours(int.Parse(interval.TrimEnd('h')));
            if (interval.EndsWith("d"))
                return TimeSpan.FromDays(int.Parse(interval.TrimEnd('d')));
            if (interval.EndsWith("wk"))
                return TimeSpan.FromDays(7 * int.Parse(interval.Replace("wk", "")));
            if (interval.EndsWith("mo"))
                return TimeSpan.FromDays(30 * int.Parse(interval.Replace("mo", "")));
            if (interval.EndsWith("y"))
                return TimeSpan.FromDays(365 * int.Parse(interval.TrimEnd('y')));
            throw new ArgumentException("Unknown interval: " + interval);
        }
    }
}