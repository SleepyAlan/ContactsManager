using ServiceContracts.Enums;

namespace ServiceContracts.Extensions;

public static class GenderOptionsExtensions
{
    public static string ToString(this GenderOptions genderOption)
    {
        return genderOption switch
        {
            GenderOptions.Male => "Male",
            GenderOptions.Female => "Female",
            GenderOptions.Other => "Other",
            _ => throw new ArgumentOutOfRangeException(nameof(genderOption), genderOption, null)
        };
    }
}