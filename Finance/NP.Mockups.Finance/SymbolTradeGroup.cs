using System.ComponentModel.DataAnnotations;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;

namespace NP.Mockups.Finance;

// class representing trades grouping
// by Symbol
public class SymbolTradeGroup : 
    AbstractNotifyPropertyChanged, IDisposable, ITrade
{
    public static int _currentId = -1;

    // Dynamic Data group
    IGroup<Trade, int, Symbol> _group;

    public int TradeId { get; }

    // trades within the group
    public IEnumerable<Trade> ChildTrades => _group.Cache.Items;

    ISourceCache<ITrade, int> _aggregationAndTrades =
        new SourceCache<ITrade, int>(t => t.TradeId);

    public IObservableCache<ITrade, int> AggregationAndChildTrades => 
        _aggregationAndTrades;

    // group key
    public Symbol TheSymbol => _group.Key;

    // sum of TotalTradePrice across all
    // the trades within the group
    public decimal TotalTradePrice { get; private set; }

    // updatable total trade prices
    bool _isVisible = true;
    public bool IsVisible
    {
        get => _isVisible;

        // SetAndRaise fires PropertyChanged event
        // when IsVisible property changes
        set => SetAndRaise(ref _isVisible, value);
    }

    bool _isOpen = true;
    public bool IsOpen
    {
        get => _isOpen;

        // SetAndRaise fires PropertyChanged event
        // when IsOpen property changes
        set
        {
            SetAndRaise(ref _isOpen, value);

            foreach (var childTrade in ChildTrades)
            {
                childTrade.IsVisible = _isOpen;
            }
        }
    }

    public override string ToString()
    {
        return $"Symbol='{TheSymbol}', ${TotalTradePrice}" 
               + "\n-------------------------------------------";
    }

    private IDisposable? _disposableSubscription;
    public void Dispose()
    {
        // destroy the group cache
        _group.Cache.Dispose();

        // remove the aggregation subscription
        _disposableSubscription?.Dispose();
        _disposableSubscription = null;
    }


    public SymbolTradeGroup(IGroup<Trade, int, Symbol> group)
    {
        _group = group;

        TradeId = _currentId--;

        var groupObservableChangeSet =
            _group
                .Cache
                .Connect();

        // set up the TotalPrice to be 
        // dynamically calculated when the group
        // or individual trades are changed
        var disposable1 =
            groupObservableChangeSet
                .ToCollection()
                .Select(collection => collection.Sum(t => t.TotalTradePrice))
                .Subscribe(sum => this.TotalTradePrice = sum);

        _aggregationAndTrades.AddOrUpdate(this);

        var disposable2 =
            groupObservableChangeSet
                .Cast<Trade, int, ITrade>(t => t)
                .PopulateInto(_aggregationAndTrades);

        _disposableSubscription =
            new CompositeDisposable(disposable1, disposable2);
    }
}