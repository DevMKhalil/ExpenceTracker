using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using ExpenceTracker.Resources;
using ExpenceTracker.Modules.Expenses.Application.DTOs;
using ExpenceTracker.Modules.Expenses.Application.Queries;
using ExpenceTracker.Modules.Expenses.Application.Commands;

namespace ExpenceTracker.Pages.Expenses
{
    public class IndexModel : PageModel
    {
        private readonly IMediator _mediator;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public IndexModel(IMediator mediator, IStringLocalizer<SharedResource> localizer)
        {
            _mediator = mediator;
            _localizer = localizer;
        }

        public List<ExpenseDto> Expenses { get; set; } = new List<ExpenseDto>();

        public async Task OnGetAsync()
        {
            Expenses = await _mediator.Send(new GetAllExpensesQuery());
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            await _mediator.Send(new DeleteExpenseCommand(id));
            return RedirectToPage();
        }
    }
}
