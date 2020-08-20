using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Godwit.Common.Data.Model;
using Godwit.HandleConfirmEmail.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Godwit.HandleConfirmEmail.Controllers
{
    [ApiController]
    [Route("")]
    public class HasuraController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IValidator<ActionData> _validator;
        private readonly ILogger<HasuraController> _logger;
        public HasuraController(IValidator<ActionData> validator, UserManager<User> userManager, ILogger<HasuraController> logger)
        {
            _validator = validator;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ActionData model)
        {
            _logger.LogInformation($"Call Started by {model?.Session?.UserId ?? "not-provided"} having role {model?.Session?.Role ?? "not-provided"}");
            var validation = _validator.Validate(model);
            if (!validation.IsValid)
            {
                _logger.LogWarning("request validation failed!");
                return Ok(new
                {
                    Success = false,
                    Errors = validation.Errors.Select(e => e.ErrorMessage).ToArray()
                });
            }

            var user = await _userManager.FindByNameAsync(model.Input.UserName)
                .ConfigureAwait(false);
            if (user == null || user.EmailConfirmed)
            {
                _logger.LogWarning("Invalid username or email is already confirmed!");
                return Ok(new
                {
                    Success = false,
                    Errors = new[] { "Invalid username or email is already confirmed!" }
                });
            }

            var result = await _userManager.ConfirmEmailAsync(user, model.Input.Token).ConfigureAwait(false);

            if (result == IdentityResult.Success) return Ok(new { Success = true });
            _logger.LogWarning(result.Errors.First().Description);
            return Ok(new
            {
                Success = false,
                Errors = result.Errors.Select(x => x.Description).ToArray()
            });

        }
    }
}