using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Godwit.Common.Data.Model;
using Godwit.HandleConfirmEmail.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Godwit.HandleConfirmEmail.Controllers {
    [ApiController]
    [Route("")]
    public class HasuraController : ControllerBase {
        private readonly UserManager<User> _userManager;
        private readonly IValidator<ActionData> _validator;

        public HasuraController(IValidator<ActionData> validator, UserManager<User> userManager) {
            _validator = validator;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ActionData model) {
            var validation = _validator.Validate(model);
            if (!validation.IsValid)
                return Ok(new {
                    Success = false,
                    Errors = validation.Errors.Select(e => e.ErrorMessage).ToArray()
                });
            var user = await _userManager.FindByNameAsync(model.Input.UserName)
                .ConfigureAwait(false);
            if (user == null || user.EmailConfirmed)
                return Ok(new {
                    Success = false,
                    Errors = new[] {"Invalid username or email is already confirmed!"}
                });

            var result = await _userManager.ConfirmEmailAsync(user, model.Input.Token).ConfigureAwait(false);

            if (result != IdentityResult.Success)
                return Ok(new {
                    Success = false,
                    Errors = result.Errors.Select(x => x.Description).ToArray()
                });
            return Ok(new {Success = true});
        }
    }
}