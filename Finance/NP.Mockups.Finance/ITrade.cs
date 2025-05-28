using System.ComponentModel;

namespace NP.Mockups.Finance;

public interface ITrade : INotifyPropertyChanged
{
    public int TradeId { get; }

    public Symbol TheSymbol { get; }

    public decimal TotalTradePrice { get; }

    public bool IsVisible { get; }
}
