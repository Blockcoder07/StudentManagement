using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using StudentManagement.Application.Common.Helpers;
using StudentManagement.Application.Common.Responses;
using StudentManagement.Domain.Enums;

namespace StudentManagement.API.Filters;

public class ValidationActionFilter : IAsyncActionFilter
{
    #region Fields

    private readonly IServiceProvider _objServiceProvider;

    #endregion

    #region Constructor

    public ValidationActionFilter(IServiceProvider objServiceProvider)
    {
        _objServiceProvider = objServiceProvider;
    }

    #endregion

    #region Public Methods

    public async Task OnActionExecutionAsync(ActionExecutingContext objContext, ActionExecutionDelegate objNext)
    {
        foreach (object? objArgument in objContext.ActionArguments.Values.Where(a => a is not null))
        {
            Type objValidatorType = typeof(IValidator<>).MakeGenericType(objArgument!.GetType());
            IValidator? objValidator = _objServiceProvider.GetService(objValidatorType) as IValidator;
            if (objValidator is null)
            {
                continue;
            }

            Type objValidationContextType = typeof(ValidationContext<>).MakeGenericType(objArgument.GetType());
            IValidationContext objValidationContext = (IValidationContext)Activator.CreateInstance(objValidationContextType, objArgument)!;
            ValidationResult objResult = await objValidator.ValidateAsync(objValidationContext);

            if (!objResult.IsValid)
            {
                List<string> lstErrors = objResult.Errors.Select(e => e.ErrorMessage).ToList();
                ApiResponse objResponse = ApiResponse.Fail(
                    ResponseMessage.ValidationFailed.GetDescription(),
                    StatusCodes.Status400BadRequest,
                    lstErrors);

                objContext.Result = new BadRequestObjectResult(objResponse);
                return;
            }
        }

        await objNext();
    }

    #endregion
}
