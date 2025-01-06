namespace Application.DTOs.ResultsDTOs
{
    public class ServiceResult
    {
        public readonly List<string>? Errors;
        public readonly object? data;
        public readonly bool success;

        public ServiceResult(bool success, List<string>? Errors = null, object? data = null)
        {
            this.success = success;
            this.Errors = Errors;
            this.data = data;
        }
    }
}
