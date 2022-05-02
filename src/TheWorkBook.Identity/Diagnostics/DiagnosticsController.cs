using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheWorkBook.Utils.Abstraction;

namespace TheWorkBook.Identity
{
    [SecurityHeaders]
    [Authorize]
    public class DiagnosticsController : Controller
    {
        readonly IEnvVariableHelper _envVariableHelper;

        public DiagnosticsController(IEnvVariableHelper envVariableHelper)
        {
            _envVariableHelper = envVariableHelper;
        }

        public async Task<IActionResult> Index()
        {
            bool enableDiagnostics = _envVariableHelper.GetVariable<bool>("EnableDiagnostics");

            if (!enableDiagnostics)
            {
                return NotFound();
            }    

            var model = new DiagnosticsViewModel(await HttpContext.AuthenticateAsync());
            return View(model);
        }
    }
}