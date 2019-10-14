using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;

namespace ChatServer
{
    public class Server
    {
        static TcpListener tcpListener; // сервер для прослушивания
        List<Client> clients = new List<Client>(); // все подключения
 
        protected internal void AddConnection(Client clientObject)
        {
            clients.Add(clientObject);
        }
        protected internal void RemoveConnection(string id)
        {
        Client client = clients.FirstOrDefault(c => c.Id == id);  // получаем по id закрытое подключение
                if (client != null)
                clients.Remove(client); // и удаляем его из списка подключений
        }
         protected internal void Listen() // прослушивание входящих подключений
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, 61354);
                tcpListener.Start();
                Console.WriteLine("Сервер запущен. Ожидание подключений...");
 
                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();
 
                    Client clientObject = new Client(tcpClient, this);
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }
        protected internal void BroadcastMessage(string message, string id) // трансляция сообщения подключенным клиентам
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].Id!= id) // если id клиента не равно id отправляющего
                {
                    clients[i].Stream.Write(data, 0, data.Length); //передача данных
                }
            }
        }
        protected internal void Disconnect() // отключение всех клиентов
        {
            tcpListener.Stop(); //остановка сервера
 
            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Close(); //отключение клиента
            }
            Environment.Exit(0); //завершение процесса
        }
    }
}
