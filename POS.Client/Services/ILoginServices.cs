namespace POS.Client.Services
{
    public interface ILoginServices
    {
        Task login(string token);
        Task logout();
    }
}
