using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CertManager;

[ApiExplorerSettings(IgnoreApi = true)]
public class ExceptionHandlerController : ControllerBase
{
	[Route("/error")]
	public IActionResult HandleError()
	{
		var exceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerFeature>()!;

		if (exceptionHandlerFeature.Error is KeyNotFoundException) return NotFound();
		return Problem();
	}
}