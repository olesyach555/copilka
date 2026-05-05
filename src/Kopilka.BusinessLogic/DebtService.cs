using Kopilka.DataAccess;
using Kopilka.Shared;
using Microsoft.EntityFrameworkCore;

namespace Kopilka.BusinessLogic
{
    public class DebtService
    {
        private readonly ApplicationDbContext _context;

        public DebtService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<DebtContract>> GetUserDebtsAsync(int userId)
        {
            return await _context.DebtContracts
                .Where(d => d.UserId == userId)
                .ToListAsync();
        }

        public async Task AddDebtContractAsync(DebtContract contract, bool isAnnuity = true)
        {
            _context.DebtContracts.Add(contract);
            await _context.SaveChangesAsync();

            if (isAnnuity)
                await GenerateAnnuityScheduleAsync(contract);
            else
                await GenerateDifferentiatedScheduleAsync(contract);
        }

        private async Task GenerateAnnuityScheduleAsync(DebtContract contract)
        {
            if (!contract.EndDate.HasValue) return;

            int months = GetMonthsBetween(contract.StartDate, contract.EndDate.Value);
            if (months <= 0) months = 1;

            double monthlyRate = (double)contract.InterestRate / 100 / 12;
            double annuityRatio = (monthlyRate * Math.Pow(1 + monthlyRate, months)) / (Math.Pow(1 + monthlyRate, months) - 1);
            decimal monthlyPayment = contract.Principal * (decimal)annuityRatio;

            for (int i = 1; i <= months; i++)
            {
                var schedule = new PaymentSchedule
                {
                    DebtContractId = contract.Id,
                    DueDate = contract.StartDate.AddMonths(i),
                    AmountDue = monthlyPayment,
                    IsPaid = false
                };
                _context.PaymentSchedules.Add(schedule);
            }
            await _context.SaveChangesAsync();
        }

        private async Task GenerateDifferentiatedScheduleAsync(DebtContract contract)
        {
            if (!contract.EndDate.HasValue) return;

            int months = GetMonthsBetween(contract.StartDate, contract.EndDate.Value);
            if (months <= 0) months = 1;

            decimal monthlyPrincipal = contract.Principal / months;
            decimal remainingPrincipal = contract.Principal;

            for (int i = 1; i <= months; i++)
            {
                decimal interest = remainingPrincipal * (contract.InterestRate / 100 / 12);
                decimal totalPayment = monthlyPrincipal + interest;

                var schedule = new PaymentSchedule
                {
                    DebtContractId = contract.Id,
                    DueDate = contract.StartDate.AddMonths(i),
                    AmountDue = totalPayment,
                    IsPaid = false
                };
                _context.PaymentSchedules.Add(schedule);
                remainingPrincipal -= monthlyPrincipal;
            }
            await _context.SaveChangesAsync();
        }

        private int GetMonthsBetween(DateTime start, DateTime end)
        {
            return ((end.Year - start.Year) * 12) + end.Month - start.Month;
        }

        public List<BankOffer> CompareBankOffers(decimal amount, int months, List<BankParameter> banks)
        {
            var offers = new List<BankOffer>();
            foreach (var bank in banks)
            {
                double monthlyRate = (double)bank.InterestRate / 100 / 12;
                double annuityRatio = (monthlyRate * Math.Pow(1 + monthlyRate, months)) / (Math.Pow(1 + monthlyRate, months) - 1);
                decimal monthlyPayment = amount * (decimal)annuityRatio;
                decimal totalPayout = monthlyPayment * months;

                offers.Add(new BankOffer
                {
                    BankName = bank.Name,
                    MonthlyPayment = monthlyPayment,
                    TotalPayout = totalPayout,
                    Overpayment = totalPayout - amount
                });
            }
            return offers.OrderBy(o => o.TotalPayout).ToList();
        }

        public async Task<List<PaymentSchedule>> GetPaymentScheduleAsync(int contractId)
        {
            return await _context.PaymentSchedules
                .Where(p => p.DebtContractId == contractId)
                .OrderBy(p => p.DueDate)
                .ToListAsync();
        }

        public async Task MarkAsPaidAsync(int scheduleId)
        {
            var schedule = await _context.PaymentSchedules.FindAsync(scheduleId);
            if (schedule != null)
            {
                schedule.IsPaid = true;
                schedule.PaidDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }
    }

    public class BankParameter
    {
        public string Name { get; set; } = string.Empty;
        public decimal InterestRate { get; set; }
    }

    public class BankOffer
    {
        public string BankName { get; set; } = string.Empty;
        public decimal MonthlyPayment { get; set; }
        public decimal TotalPayout { get; set; }
        public decimal Overpayment { get; set; }
    }
}
