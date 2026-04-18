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
using ExpenceTracker.Modules.Badges.Application.Queries;
using ExpenceTracker.Modules.Badges.Domain;

namespace ExpenceTracker.Pages.Expenses
{
    public class CreateModel : PageModel
    {
        private readonly IMediator _mediator;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public CreateModel(IMediator mediator, IStringLocalizer<SharedResource> localizer)
        {
            _mediator = mediator;
            _localizer = localizer;
            AvailableBadges = new List<Badge>();
        }

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

        public async Task OnGetAsync()
        {
            Date = DateTimeOffset.Now;
            AvailableBadges = await _mediator.Send(new GetAllBadgesQuery());
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
                          .Select(id => Guid.TryParse(id, out var guid) ? guid : (Guid?)null)
                          .Where(id => id.HasValue)
                          .Select(id => id!.Value)
                          .ToList();

            try
            {
                await _mediator.Send(new CreateExpenseCommand(Name, Amount, Date, Importance, Notes, IsPending, badgeIdsList));
                return RedirectToPage("Create", new { saved = true });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }
    }
}
