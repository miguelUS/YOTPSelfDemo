using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace YOTPSelfDemo.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }
        [BindProperty]
        public string Username { get; set; }
        [BindProperty]
        public string Serialnumber { get; set; }
        [BindProperty]
        public string YOTPcode { get; set; }
        
        public string Result = "";

        public void OnGet()
        {

        }

        public IActionResult OnPost()
        {
            YOTPSelfService selfService = new YOTPSelfService();
            Result = selfService.AssignYubiKey(Username, Serialnumber, YOTPcode);

            if (ModelState.IsValid == false)
            {
                return Page();
            }
            else
                return Page();
        }
    }
}
