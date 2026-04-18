using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using ExpenceTracker.Resources;
using ExpenceTracker.Modules.Expenses.Domain;
using ExpenceTracker.Modules.Expenses.Application.Commands;
using ExpenceTracker.Modules.Expenses.Application.Queries;
using ExpenceTracker.Modules.Badges.Application.Queries;
using ExpenceTracker.Modules.Badges.Domain;

namespace ExpenceTracker.Pages.Expenses
{
    public class EditModel : PageModel
    {
        private readonly IMediator _mediator;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public EditModel(IMediator mediator, IStringLocalizer<SharedResource> localizer)
        {
            _mediator = mediator;
            _localizer = localizer;
            AvailableBadges = new List<Badge>();
        }

        [BindProperty]
        public Guid Id { get; set; }

        [BindProperty]
        public string Name { get; set; } = string.Empty;

        [BindProperty]
        public decimal Amount { get; set; }

        [BindProperty]
        public DateTimeOffset Date { get; set; }

        [BindProperty]
        public ImportanceLevel Importance { get; set; } = ImportanceLevel.Normal;

        [BindProperty]
        public string? Notes { get; set; }

        [BindProperty]
        public bool IsPending { get; set; }

        [BindProperty]
        public string BadgeIds { get; set; } = string.Empty;

        public List<Badge> AvailableBadges { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var expense = await _mediator.Send(new GetExpenseByIdQuery(id));
            if (expense == null)
            {
                return NotFound();
            }

            Id = expense.Id;
            Name = expense.Name;
            Amount = expense.Amount;
            Date = expense.Date;
            Importance = expense.Importance;
            Notes = expense.Notes;
            IsPending = expense.IsPending;
            BadgeIds = string.Join(",", expense.BadgeIds);

            AvailableBadges = await _mediator.Send(new GetAllBadgesQuery());
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            AvailableBadges = await _mediator.Send(new GetAllBadgesQuery());

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var badgeIdsList = string.IsNullOrWhiteSpace(BadgeIds)
                ? new List<Guid>()
                : BadgeIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                          .Select(uid => Guid.TryParse(uid, out var guid) ? guid : (Guid?)null)
                          .Where(id => id.HasValue)
                          .Select(id => id!.Value)
                          .ToList();

            try
            {
                await _mediator.Send(new UpdateExpenseCommand(Id, Name, Amount, Date, Importance, Notes, IsPending, badgeIdsList));
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }
    }
}
