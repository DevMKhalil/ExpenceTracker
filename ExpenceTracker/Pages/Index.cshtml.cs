using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using ExpenceTracker.Resources;
using ExpenceTracker.Modules.Expenses.Application.DTOs;
using ExpenceTracker.Modules.Expenses.Application.Queries;

namespace ExpenceTracker.Pages
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

        public decimal DailyTotal { get; set; }
        public decimal MonthlyTotal { get; set; }
        public decimal PendingTotal { get; set; }
        public List<BadgeBreakdownItem> BadgeBreakdown { get; set; } = new List<BadgeBreakdownItem>();

        public async Task OnGetAsync()
        {
            DailyTotal = await _mediator.Send(new GetDailySummaryQuery(DateTimeOffset.Now));
            MonthlyTotal = await _mediator.Send(new GetMonthlySummaryQuery(DateTime.Now.Year, DateTime.Now.Month));
            PendingTotal = await _mediator.Send(new GetPendingTotalQuery());
            BadgeBreakdown = await _mediator.Send(new GetBadgeSummaryQuery());
            
            // Limit to top 5
            if (BadgeBreakdown.Count > 5)
            {
                BadgeBreakdown = BadgeBreakdown.GetRange(0, 5);
            }
        }
    }
}
