using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ExpenceTracker.Modules.Badges.Domain;

namespace ExpenceTracker.Modules.Badges.Application.Commands
{
    public record CreateBadgeCommand(string Name, string Color) : IRequest<Guid>;

    public class CreateBadgeCommandHandler : IRequestHandler<CreateBadgeCommand, Guid>
    {
        private readonly IBadgeRepository _repository;

        public CreateBadgeCommandHandler(IBadgeRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(CreateBadgeCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Name cannot be empty", nameof(request.Name));
            if (request.Name.Length > 100)
                throw new ArgumentException("Name cannot exceed 100 characters", nameof(request.Name));
            if (!Regex.IsMatch(request.Color, "^#[0-9A-Fa-f]{6}$"))
                throw new ArgumentException("Color must be a valid hex color code", nameof(request.Color));

            if (await _repository.ExistsByNameAsync(request.Name))
                throw new InvalidOperationException("A badge with this name already exists");

            var badge = new Badge(request.Name.Trim(), request.Color);
            await _repository.AddAsync(badge);

            return badge.Id;
        }
    }
}
