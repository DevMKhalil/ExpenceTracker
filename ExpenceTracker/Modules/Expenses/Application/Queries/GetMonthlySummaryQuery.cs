using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ExpenceTracker.Modules.Expenses.Domain;

namespace ExpenceTracker.Modules.Expenses.Application.Queries
{
    public record GetMonthlySummaryQuery(int Year, int Month) : IRequest<decimal>;

    public class GetMonthlySummaryQueryHandler : IRequestHandler<GetMonthlySummaryQuery, decimal>
    {
        private readonly IExpenseRepository _repository;

        public GetMonthlySummaryQueryHandler(IExpenseRepository repository)
        {
            _repository = repository;
        }

        public async Task<decimal> Handle(GetMonthlySummaryQuery request, CancellationToken cancellationToken)
        {
            var expenses = await _repository.GetAllAsync();
            return expenses
                .Where(e => e.Date.Year == request.Year && e.Date.Month == request.Month && !e.IsPending)
                .Sum(e => e.Amount);
        }
    }
}
