namespace EmployeeManagement.DTO.Response
{
    public class ApiErrorResponse
    {
        public string Message { get; set; } = string.Empty;
        public string? Details { get; set; }
    }
}
