using System.ComponentModel.DataAnnotations;

namespace Downcast.SessionManager.Jwt;

public class JwtOptions : IValidatableObject
{
    public const string OptionsSection = "JwtOptions";

    [Required]
    public TimeSpan Duration { get; init; }

    [Required(ErrorMessage = "The key is required so the jwt can be securely signed")]
    public string Key { get; init; } = null!;

    [Required]
    public string Issuer { get; init; } = null!;

    [Required]
    public string Audience { get; init; } = null!;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Duration == default)
        {
            yield return new ValidationResult("Please define a duration higher than zero, e.g., 01:00:00",
                                              new[] { nameof(Duration) });
        }
    }
}