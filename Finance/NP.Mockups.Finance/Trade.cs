using DynamicData.Binding;

namespace NP.Mockups.Finance;

public class Trade : 
    AbstractNotifyPropertyChanged, ITrade
{
    // primary key (used for distinguishing 
    // between the new entries and the 
    // updates_)
    public int TradeId { get; }

    // Stock Symbol
    public Symbol TheSymbol { get; }

    // updatable total trade prices
    decimal _totalTradePrice;
    public decimal TotalTradePrice 
    {
        get => _totalTradePrice; 

        // SetAndRaise fires PropertyChanged event
        // when TotalTradePrice property changes
        set => SetAndRaise(ref _totalTradePrice, value);
    }

    // updatable total trade prices
    bool _isVisible;
    public bool IsVisible
    {
        get => _isVisible;

        // SetAndRaise fires PropertyChanged event
        // when TotalTradePrice property changes
        set => SetAndRaise(ref _isVisible, value);
    }

    public Trade
    (
        int tradeId,
        Symbol symbol,
        decimal totalTradeAmount)
    {
        this.TradeId = tradeId;
        this.TheSymbol = symbol;
        this.TotalTradePrice = totalTradeAmount;
    }

    public override int GetHashCode()
    {
        return TradeId.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        if (obj is Trade trade)
        {
            return this.TradeId.Equals(trade.TradeId) && 
                   (this.TheSymbol == trade.TheSymbol) && 
                   (this.TotalTradePrice == trade.TotalTradePrice);
        }

        return false;
    }

    public override string ToString()
    {
        return $"Symbol='{TheSymbol}', ${TotalTradePrice}";
    }
}