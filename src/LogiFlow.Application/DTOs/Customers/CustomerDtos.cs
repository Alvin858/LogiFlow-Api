namespace LogiFlow.Application.DTOs.Customers;

public record UpdateCustomerProfileRequest(string CompanyName, string ContactPerson, string? TaxNumber, string? PhoneNumber);
public record CreateAddressRequest(string AddressLine1, string? AddressLine2, string City, string State, string PostalCode, string Country, bool IsDefault);
public record UpdateAddressRequest(string AddressLine1, string? AddressLine2, string City, string State, string PostalCode, string Country, bool IsDefault);
public record AddressResponse(int Id, string AddressLine1, string? AddressLine2, string City, string State, string PostalCode, string Country, bool IsDefault);
public record CustomerResponse(int Id, int UserId, string CompanyName, string ContactPerson, string? TaxNumber, string Email, string? PhoneNumber, IReadOnlyCollection<AddressResponse> Addresses);
