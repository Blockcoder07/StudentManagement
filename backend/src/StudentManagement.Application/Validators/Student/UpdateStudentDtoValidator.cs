using FluentValidation;
using StudentManagement.Application.DTOs.Student;

namespace StudentManagement.Application.Validators.Student;

public class UpdateStudentDtoValidator : AbstractValidator<UpdateStudentDto>
{
    public UpdateStudentDtoValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("A valid student id is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100)
            .Matches(@"^[a-zA-Z\s.'-]+$").WithMessage("Name can only contain letters, spaces, dots, apostrophes and hyphens.");

        RuleFor(x => x.Email)
            .NotEmpty().EmailAddress().MaximumLength(150);

        RuleFor(x => x.Age).InclusiveBetween(5, 120);

        RuleFor(x => x.Course).NotEmpty().MaximumLength(100);
    }
}
