using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ExpenceTracker.Modules.Badges.Domain;

namespace ExpenceTracker.Modules.Badges.Application.Queries
{
    public record GetBadgeByIdQuery(Guid Id) : IRequest<Badge?>;

    public class GetBadgeByIdQueryHandler : IRequestHandler<GetBadgeByIdQuery, Badge?>
    {
        private readonly IBadgeRepository _repository;

        public GetBadgeByIdQueryHandler(IBadgeRepository repository)
        {
            _repository = repository;
        }

        public async Task<Badge?> Handle(GetBadgeByIdQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetByIdAsync(request.Id);
        }
    }
}
