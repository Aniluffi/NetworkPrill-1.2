using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DOCS2
{
    /// <summary>
    /// класс отвечающий за сервер
    /// </summary>
    class Server
    {
        /// <summary>
        /// поле для хранения клиентов которые онлайн
        /// </summary>
        List<Socket> clients = new List<Socket>();

        /// <summary>
        /// метод для запуска сервера
        /// </summary>
        /// <returns></returns>
        public async Task ServerOn()
        {
            using var tcpLisener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                tcpLisener.Bind(new IPEndPoint(IPAddress.Parse("192.168.1.135"), 8888));
                tcpLisener.Listen();    // запускаем сервер
                Console.WriteLine("Сервер запущен. Ожидание подключений... ");

                while (true)
                {
                    var tcpClient = await tcpLisener.AcceptAsync();
                    lock (clients)
                    {
                        clients.Add(tcpClient); // Добавление клиента в список
                    }
                    Console.WriteLine($"{tcpClient.RemoteEndPoint} подключился");


                    await Task.Run(async () => Client(tcpClient));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        /// <summary>
        /// обработка клиента
        /// </summary>
        /// <param name="tcpClient">клиент</param>
        /// <returns></returns>
        private async Task Client(Socket tcpClient)
        {
            var buffer = new byte[1024];

            await BroadCast(tcpClient, $"\nПодключился {tcpClient.RemoteEndPoint}:  ");

            try
            {
                while (true)
                {
                    var bytesRead = await tcpClient.ReceiveAsync(buffer);

                    var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Сообщение от {tcpClient.RemoteEndPoint}: {message}");

                    await BroadCast(tcpClient, $"\n Сообщение от {tcpClient.RemoteEndPoint}:  " + message);
                }
            }
            catch (SocketException)
            {
                await BroadCast(tcpClient, $"Клиент {tcpClient.RemoteEndPoint} отключился");

                clients.Remove(tcpClient);

                Console.WriteLine($"Клиент {tcpClient.RemoteEndPoint} отключился");
            }

        }

        /// <summary>
        /// метод для широковещания
        /// </summary>
        /// <param name="tcpClient">клиент которой отправляет широковещательный пакет</param>
        /// <param name="message">данные</param>
        /// <returns></returns>
        private async Task BroadCast(Socket tcpClient, string message)
        {
            foreach (var client in clients)
            {
                try
                {

                    var mess = Encoding.UTF8.GetBytes(message);
                    if (tcpClient.RemoteEndPoint != client.RemoteEndPoint)
                        await client.SendAsync(mess);
                }
                catch (SocketException)
                {
                    Console.WriteLine("Ошибка рассылки");
                }
            }
        }
    }
}
