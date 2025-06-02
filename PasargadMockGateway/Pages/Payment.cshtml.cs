using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PasargadMockGateway.Pages;

public class PaymentModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string Token { get; set; } = string.Empty;

    [BindProperty(SupportsGet = true)]
    public string InvoiceNumber { get; set; } = string.Empty;

    [BindProperty(SupportsGet = true)]
    public string TerminalCode { get; set; } = string.Empty;

    [BindProperty(SupportsGet = true)]
    public decimal Amount { get; set; }

    [BindProperty(SupportsGet = true)]
    public string RedirectUrl { get; set; } = string.Empty;

    public void OnGet()
    {
        // در اینجا می‌توانید اطلاعات تراکنش را از دیتابیس بخوانید
        // و در صورت نیاز اعتبارسنجی انجام دهید
    }
} 