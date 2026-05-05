using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Kopilka.BusinessLogic.ViewModels
{
    public partial class ChartViewModel : ViewModelBase
    {
        private readonly TransactionService _transactionService;
        private readonly int _userId;
        private readonly DateTime _start;
        private readonly DateTime _end;

        [ObservableProperty]
        private List<ChartPoint> _chartPoints = new();

        public ChartViewModel(TransactionService transactionService, int userId, DateTime start, DateTime end)
        {
            _transactionService = transactionService;
            _userId = userId;
            _start = start;
            _end = end;
        }

        [RelayCommand]
        public async Task LoadDataAsync()
        {
            ChartPoints = await _transactionService.GetChartDataAsync(_userId, _start, _end);
        }
    }
}
