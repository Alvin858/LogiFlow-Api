namespace LogiFlow.Application.DTOs.Drivers;

public record CreateDriverRequest(string FirstName, string LastName, string Email, string Password, string? PhoneNumber, string LicenseNumber, DateTime LicenseExpiryDate, int ExperienceYears, bool IsAvailable);
public record UpdateDriverRequest(string FirstName, string LastName, string? PhoneNumber, string LicenseNumber, DateTime LicenseExpiryDate, int ExperienceYears, bool IsAvailable);
public record UpdateDriverAvailabilityRequest(bool IsAvailable);
public record DriverResponse(int Id, int UserId, string FirstName, string LastName, string Email, string? PhoneNumber, string LicenseNumber, DateTime LicenseExpiryDate, int ExperienceYears, bool IsAvailable, bool IsActive);
