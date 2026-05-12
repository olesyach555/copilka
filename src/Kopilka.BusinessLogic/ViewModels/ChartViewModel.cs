using CommunityToolkit.Mvvm.ComponentModel;
using Kopilka.Shared;
using System.Collections.ObjectModel;

namespace Kopilka.BusinessLogic.ViewModels
{
    public partial class ChartViewModel : ViewModelBase
    {
        private readonly TransactionService _transactionService;
        private readonly int _userId;
        private readonly DateTime _startDate;
        private readonly DateTime _endDate;

        [ObservableProperty]
        private List<ChartPoint> _chartPoints = new();

        public ChartViewModel(TransactionService transactionService, int userId, DateTime startDate, DateTime endDate)
        {
            _transactionService = transactionService;
            _userId = userId;
            _startDate = startDate;
            _endDate = endDate;
        }

        public async Task LoadDataAsync()
        {
            ChartPoints = await _transactionService.GetChartDataAsync(_userId, _startDate, _endDate);
        }
    }
}
