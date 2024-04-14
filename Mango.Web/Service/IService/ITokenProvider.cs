namespace Mango.Web.Service.IService
{
    public interface ITokenProvider
    {
        public void AddToken(string token);
        public void RemoveToken();
        public string? GetToken();
    }
}
