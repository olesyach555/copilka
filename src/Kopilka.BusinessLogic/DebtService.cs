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

        public async Task AddDebtAsync(DebtContract debt)
        {
            _context.DebtContracts.Add(debt);
            await _context.SaveChangesAsync();

            await GenerateInitialSchedule(debt);
        }

        public async Task UpdateDebtAsync(DebtContract debt)
        {
            _context.Entry(debt).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDebtAsync(int id)
        {
            var debt = await _context.DebtContracts.FindAsync(id);
            if (debt != null)
            {
                // Сначала удаляем график платежей (хотя в OnModelCreating настроен Cascade)
                var schedules = _context.PaymentSchedules.Where(s => s.DebtContractId == id);
                _context.PaymentSchedules.RemoveRange(schedules);

                _context.DebtContracts.Remove(debt);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<PaymentSchedule>> GetScheduleAsync(int debtId)
        {
            return await _context.PaymentSchedules
                .Where(p => p.DebtContractId == debtId)
                .OrderBy(p => p.DueDate)
                .ToListAsync();
        }

        public async Task MarkAsPaidAsync(int scheduleId)
        {
            var item = await _context.PaymentSchedules.FindAsync(scheduleId);
            if (item != null)
            {
                item.IsPaid = true;
                item.PaidDate = DateTime.Now;
                await _context.SaveChangesAsync();

                var debt = await _context.DebtContracts.FindAsync(item.DebtContractId);
                if (debt != null)
                {
                    var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == "Долги" && c.UserId == debt.UserId);
                    if (category == null)
                    {
                        category = new Category { Name = "Долги", IsIncome = false, UserId = debt.UserId };
                        _context.Categories.Add(category);
                        await _context.SaveChangesAsync();
                    }

                    var transaction = new Transaction
                    {
                        UserId = debt.UserId,
                        Amount = item.AmountDue,
                        Date = DateTime.Now,
                        Comment = $"Оплата по долгу: {debt.Title}",
                        CategoryId = category.Id
                    };
                    _context.Transactions.Add(transaction);
                    await _context.SaveChangesAsync();
                }
            }
        }

        private async Task GenerateInitialSchedule(DebtContract debt)
        {
            int months = 12;
            if (debt.EndDate.HasValue)
            {
                months = ((debt.EndDate.Value.Year - debt.StartDate.Year) * 12) + debt.EndDate.Value.Month - debt.StartDate.Month;
                if (months <= 0) months = 1;
            }

            // Аннуитетный платеж
            double rate = (double)debt.InterestRate / 100 / 12;
            decimal monthlyPayment;

            if (rate > 0)
            {
                double factor = (rate * Math.Pow(1 + rate, months)) / (Math.Pow(1 + rate, months) - 1);
                monthlyPayment = (decimal)((double)debt.Principal * factor);
            }
            else
            {
                monthlyPayment = debt.Principal / months;
            }

            decimal remainingPrincipal = debt.Principal;

            for (int i = 1; i <= months; i++)
            {
                decimal interestPayment = remainingPrincipal * (debt.InterestRate / 100 / 12);
                decimal principalPayment = monthlyPayment - interestPayment;

                if (i == months) // Корректировка последнего платежа
                {
                    principalPayment = remainingPrincipal;
                    monthlyPayment = principalPayment + interestPayment;
                }

                var schedule = new PaymentSchedule
                {
                    DebtContractId = debt.Id,
                    DueDate = debt.StartDate.AddMonths(i),
                    PrincipalPayment = principalPayment,
                    InterestPayment = interestPayment,
                    AmountDue = monthlyPayment,
                    IsPaid = false
                };
                _context.PaymentSchedules.Add(schedule);
                remainingPrincipal -= principalPayment;
            }
            await _context.SaveChangesAsync();
        }
    }
}
