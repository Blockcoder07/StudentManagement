using FluentValidation;
using StudentManagement.Application.DTOs.Student;

namespace StudentManagement.Application.Validators.Student;

public class CreateStudentDtoValidator : AbstractValidator<CreateStudentDto>
{
    public CreateStudentDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.")
            .Matches(@"^[a-zA-Z\s.'-]+$").WithMessage("Name can only contain letters, spaces, dots, apostrophes and hyphens.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email format is invalid.")
            .MaximumLength(150).WithMessage("Email must not exceed 150 characters.");

        RuleFor(x => x.Age)
            .InclusiveBetween(5, 120).WithMessage("Age must be between 5 and 120.");

        RuleFor(x => x.Course)
            .NotEmpty().WithMessage("Course is required.")
            .MaximumLength(100).WithMessage("Course must not exceed 100 characters.");
    }
}
