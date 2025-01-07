namespace Application.Interfaces.ServicesInterfaces
{
    public interface IUrlService
    {
        string GenerateResetPasswordUrl(string token);
    }
}
