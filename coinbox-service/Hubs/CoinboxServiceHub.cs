using Microsoft.AspNetCore.SignalR;

namespace SignalRApp
{
    public class CoinboxServiceHub : Hub
    {
        protected IHubContext<CoinboxServiceHub> _context;

        public CoinboxServiceHub(IHubContext<CoinboxServiceHub> context)
        {
            _context = context;
        }
        public async Task BroadcastSend(string message, string userName)
        {
            await _context.Clients.All.SendAsync("Receive", message, userName);
        }
    }
}