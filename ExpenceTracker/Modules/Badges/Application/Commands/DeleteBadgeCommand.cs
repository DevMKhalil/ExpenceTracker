using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ExpenceTracker.Modules.Badges.Domain;

namespace ExpenceTracker.Modules.Badges.Application.Commands
{
    public record DeleteBadgeCommand(Guid Id) : IRequest<bool>;

    public class DeleteBadgeCommandHandler : IRequestHandler<DeleteBadgeCommand, bool>
    {
        private readonly IBadgeRepository _repository;

        public DeleteBadgeCommandHandler(IBadgeRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(DeleteBadgeCommand request, CancellationToken cancellationToken)
        {
            var badge = await _repository.GetByIdAsync(request.Id);
            if (badge == null)
                return false;

            badge.IsDeleted = true;
            await _repository.UpdateAsync(badge);
            return true;
        }
    }
}
