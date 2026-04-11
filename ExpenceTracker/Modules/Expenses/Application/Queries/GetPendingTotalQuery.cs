using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ExpenceTracker.Modules.Expenses.Domain;

namespace ExpenceTracker.Modules.Expenses.Application.Queries
{
    public record GetPendingTotalQuery() : IRequest<decimal>;

    public class GetPendingTotalQueryHandler : IRequestHandler<GetPendingTotalQuery, decimal>
    {
        private readonly IExpenseRepository _repository;

        public GetPendingTotalQueryHandler(IExpenseRepository repository)
        {
            _repository = repository;
        }

        public async Task<decimal> Handle(GetPendingTotalQuery request, CancellationToken cancellationToken)
        {
            var expenses = await _repository.GetAllAsync();
            return expenses.Where(e => e.IsPending).Sum(e => e.Amount);
        }
    }
}
