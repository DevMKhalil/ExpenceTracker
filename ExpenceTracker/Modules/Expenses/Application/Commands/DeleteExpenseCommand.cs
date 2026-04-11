using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ExpenceTracker.Modules.Expenses.Domain;

namespace ExpenceTracker.Modules.Expenses.Application.Commands
{
    public record DeleteExpenseCommand(Guid Id) : IRequest<bool>;

    public class DeleteExpenseCommandHandler : IRequestHandler<DeleteExpenseCommand, bool>
    {
        private readonly IExpenseRepository _repository;

        public DeleteExpenseCommandHandler(IExpenseRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(DeleteExpenseCommand request, CancellationToken cancellationToken)
        {
            var expense = await _repository.GetByIdAsync(request.Id);
            if (expense == null)
            {
                return false;
            }

            await _repository.DeleteAsync(request.Id);
            return true;
        }
    }
}
