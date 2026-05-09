using System.ComponentModel;

namespace StudentManagement.Domain.Enums;

public enum ResponseMessage
{
    [Description("Operation completed successfully.")]
    Success,

    [Description("Login successful.")]
    LoginSuccess,

    [Description("Invalid username or password.")]
    InvalidCredentials,

    [Description("Your account has been deactivated. Please contact the administrator.")]
    AccountInactive,

    [Description("You are not authorised to perform this action.")]
    UnauthorizedAccess,

    [Description("Student created successfully.")]
    StudentCreated,

    [Description("Student updated successfully.")]
    StudentUpdated,

    [Description("Student deleted successfully.")]
    StudentDeleted,

    [Description("Students fetched successfully.")]
    StudentsFetched,

    [Description("Student fetched successfully.")]
    StudentFetched,

    [Description("Student not found.")]
    StudentNotFound,

    [Description("A student with the same email already exists.")]
    StudentEmailExists,

    [Description("Validation failed. Please verify the highlighted fields.")]
    ValidationFailed,

    [Description("An unexpected error occurred while processing your request.")]
    InternalServerError,

    [Description("The database operation could not be completed.")]
    DatabaseError
}
