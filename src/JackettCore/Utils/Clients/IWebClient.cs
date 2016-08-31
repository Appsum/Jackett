using System.Threading.Tasks;

namespace JackettCore.Utils.Clients
{
    public interface IWebClient
    {
        Task<WebClientStringResult> GetString(WebRequest request);
        Task<WebClientByteResult> GetBytes(WebRequest request);
        void Init();
    }
}
