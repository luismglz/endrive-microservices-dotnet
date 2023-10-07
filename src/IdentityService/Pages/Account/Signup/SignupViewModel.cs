using System.ComponentModel.DataAnnotations;

namespace IdentityService;
public class SignupViewModel
{
  [Required]
  public string Email { get; set; }

  [Required]
  public string Password { get; set; }

  [Required]
  public string Username { get; set; }

  [Required]
  public string FirstName { get; set; }

  [Required]
  public string LastName { get; set; }
  public string ReturnUrl { get; set; }
  public string Button { get; set; }
}
