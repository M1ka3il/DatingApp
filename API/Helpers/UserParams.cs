namespace API.Helpers;

public class UserParams : PaginationParams
{
  // Free-text filter matched against the username.
  public string? Search { get; set; }

  // "username" (default) — sort field. OrderBy is "asc" or "desc".
  public string OrderBy { get; set; } = "username";
  public string Direction { get; set; } = "asc";
}
