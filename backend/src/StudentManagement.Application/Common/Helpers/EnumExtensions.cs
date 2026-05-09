using System.ComponentModel;
using System.Reflection;

namespace StudentManagement.Application.Common.Helpers;

public static class EnumExtensions
{
    #region Public Methods

    public static string GetDescription(this Enum objValue)
    {
        MemberInfo? objMember = objValue.GetType().GetMember(objValue.ToString()).FirstOrDefault();
        DescriptionAttribute? objDescription = objMember?.GetCustomAttribute<DescriptionAttribute>();
        return objDescription?.Description ?? objValue.ToString();
    }

    #endregion
}
