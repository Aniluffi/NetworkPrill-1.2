using Client1;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;


class Program
{
    /// <summary>
    /// точка входа
    /// </summary>
    /// <returns></returns>
    public static async Task Main()
    {
       Client client = new Client();
        await client.SearchServer(IPAddress.Parse("192.168.1.135"), 8888);
    }

}