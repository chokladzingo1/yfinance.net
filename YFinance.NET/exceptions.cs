using System;

public class YFException : Exception
{
    public YFException(string description = "") : base(description) { }
}

public class YFDataException : YFException
{
    public YFDataException(string description = "") : base(description) { }
}

public class YFNotImplementedError : NotImplementedException
{
    public YFNotImplementedError(string methodName)
        : base($"Have not implemented fetching '{methodName}' from Yahoo API") { }
}

public class YFTickerMissingError : YFException
{
    public string Rationale { get; }
    public string Ticker { get; }

    public YFTickerMissingError(string ticker, string rationale)
        : base($"${ticker}: possibly delisted; {rationale}")
    {
        Rationale = rationale;
        Ticker = ticker;
    }
}

public class YFTzMissingError : YFTickerMissingError
{
    public YFTzMissingError(string ticker)
        : base(ticker, "no timezone found") { }
}

public class YFPricesMissingError : YFTickerMissingError
{
    public string DebugInfo { get; }

    public YFPricesMissingError(string ticker, string debugInfo)
        : base(ticker, string.IsNullOrEmpty(debugInfo) ? "no price data found" : $"no price data found {debugInfo}")
    {
        DebugInfo = debugInfo;
    }
}

public class YFEarningsDateMissing : YFTickerMissingError
{
    public YFEarningsDateMissing(string ticker)
        : base(ticker, "no earnings dates found") { }
}

public class YFInvalidPeriodError : YFException
{
    public string Ticker { get; }
    public string InvalidPeriod { get; }
    public string ValidRanges { get; }

    public YFInvalidPeriodError(string ticker, string invalidPeriod, string validRanges)
        : base($"{ticker}: Period '{invalidPeriod}' is invalid, must be one of: {validRanges}")
    {
        Ticker = ticker;
        InvalidPeriod = invalidPeriod;
        ValidRanges = validRanges;
    }
}

public class YFRateLimitError : YFException
{
    public YFRateLimitError()
        : base("Too Many Requests. Rate limited. Try after a while.") { }
}