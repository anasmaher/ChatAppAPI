namespace Application.DTOs.ResultsDTOs
{
    public class ServiceResult
    {
        public readonly string? Error;
        public readonly object? data;
        public readonly bool success;

        public ServiceResult(bool success, string? Error = null, object? data = null)
        {
            this.success = success;
            this.Error = Error;
            this.data = data;
        }
    }
}
