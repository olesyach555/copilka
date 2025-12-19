using CommunityToolkit.Mvvm.ComponentModel;
using Kopilka.BusinessLogic.Services;
using Kopilka.BusinessLogic.ViewModels.Base;
using Kopilka.Shared;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Kopilka.BusinessLogic.ViewModels
{
    /// <summary>
    /// ViewModel для главной страницы (Dashboard).
    /// </summary>
    public partial class HomePageViewModel : ViewModelBase
    {
        private readonly StateService _stateService;
        public readonly User _currentUser;

        /// <summary>
        /// Общий баланс на всех счетах. Свойство напрямую "проксирует" значение из StateService.
        /// </summary>
        public decimal TotalBalance => _stateService.TotalBalance;

        /// <summary>
        /// Коллекция транзакций для отображения. Напрямую "проксирует" коллекцию из StateService.
        /// </summary>
        public ObservableCollection<Transaction> Transactions => _stateService.Transactions;

        public HomePageViewModel(StateService stateService, User currentUser)
        {
            _stateService = stateService;
            _currentUser = currentUser;
            // Подписываемся на изменения свойства TotalBalance в StateService,
            // чтобы обновить наше свойство TotalBalance.
            _stateService.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(StateService.TotalBalance))
                {
                    OnPropertyChanged(nameof(TotalBalance));
                    // Так как транзакции загружаются вместе с балансом,
                    // мы также оповещаем View, что коллекция могла измениться.
                    OnPropertyChanged(nameof(Transactions));
                }
            };
        }

        /// <summary>
        /// Запрашивает у StateService полную перезагрузку данных.
        /// </summary>
        public async Task RefreshDataAsync()
        {
            await _stateService.ReloadAllDataAsync();
        }
    }
}
