using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client1
{
    class Client
    {
        /// <summary>
        /// поле для отмены ввода сообщения
        /// </summary>
        CancellationTokenSource _stop = new CancellationTokenSource();
        /// <summary>
        /// буфер для хранения сообщений
        /// </summary>
        List<string> _BufferMessege = new List<string>();

        /// <summary>
        /// метод для поиска сервера
        /// </summary>
        /// <param name="iP">айпи сервера</param>
        /// <param name="port">порт</param>
        /// <returns></returns>
        public async Task SearchServer(IPAddress iP,int port)
        {
            using var tcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                await tcpClient.ConnectAsync(iP, port);

                Task.Run(async () => SendMessege(tcpClient), _stop.Token);

                await Task.Run(async () => await AcceptMessege(tcpClient));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// метод для принятия сообщений
        /// </summary>
        /// <param name="tcpClient">клиент</param>
        /// <returns></returns>
        private async Task AcceptMessege(Socket tcpClient)
        {
            while (true)
            {
                if (tcpClient.Poll(10, SelectMode.SelectRead))
                {
                    var buf = new byte[1024];
                    int acceptBytes = await tcpClient.ReceiveAsync(buf);

                    string acceptWord = Encoding.UTF8.GetString(buf, 0, acceptBytes);

                    _stop.Cancel();

                    UpdayteChat(acceptWord);

                    Console.WriteLine("Send:");
                }
            }
        }

        /// <summary>
        /// метод для обновления чата
        /// </summary>
        /// <param name="saveUp">сообщения на сохранение</param>
        private void UpdayteChat(string saveUp)
        {
            Console.Clear();
            _BufferMessege.Add(saveUp);

            Console.WriteLine(ToString());
        }

        /// <summary>
        /// преобразование в строку сообщений
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string messege = "";
            foreach (var item in _BufferMessege) messege += item + "\n";
            return messege;
        }

        /// <summary>
        /// метод для отправки сообщений
        /// </summary>
        /// <param name="tcpClient"></param>
        /// <returns></returns>
        private async Task SendMessege(Socket tcpClient)
        {
            while (true)
            {
                Console.WriteLine("Send:");
                string word = Console.ReadLine();
                byte[] data = Encoding.UTF8.GetBytes(word);

                await tcpClient.SendAsync(data);

                UpdayteChat("Я: " + word + "\n");
            }
        }
    }
}

