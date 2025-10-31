using System;
using System.Net.Sockets;
using System.Text;

namespace ChatClient
{
    class Program
    {
        private const string ServerIP = "127.0.0.1"; // Yerel makine
        private const int Port = 13000;

        static void Main(string[] args)
        {
            try
            {
                // 1. TcpClient nesnesini oluştur ve sunucuya bağlan
                using (TcpClient client = new TcpClient())
                {
                    client.Connect(ServerIP, Port);
                    Console.WriteLine($"Sunucuya ({ServerIP}:{Port}) bağlandı!");

                    // 2. Veri Akışını (NetworkStream) Al
                    using (NetworkStream stream = client.GetStream())
                    {
                        string message = string.Empty;

                        while (true)
                        {
                            Console.Write("Mesajınızı girin (çıkmak için 'exit'): ");
                            message = Console.ReadLine();

                            if (string.IsNullOrEmpty(message) || message.ToLower() == "exit")
                            {
                                break;
                            }

                            // 3. Veriyi Bayt Dizisine Çevir ve Gönder
                            byte[] dataToSend = Encoding.UTF8.GetBytes(message);
                            stream.Write(dataToSend, 0, dataToSend.Length);
                            Console.WriteLine("Mesaj gönderildi.");

                            // 4. Sunucudan Gelen Cevabı Oku
                            byte[] buffer = new byte[1024];
                            int bytesRead = stream.Read(buffer, 0, buffer.Length);

                            // Baytları okunabilir metne çevir
                            string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                            Console.WriteLine($"Sunucudan Cevap: {response}");
                        }
                    } // NetworkStream otomatik kapatılır
                } // TcpClient otomatik kapatılır (Dispose)
            }
            catch (SocketException e)
            {
                Console.WriteLine($"Bağlantı Hatası: Sunucuya bağlanılamadı veya bağlantı koptu. Hata: {e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Genel Hata: {e.Message}");
            }
            finally
            {
                Console.WriteLine("İstemci kapatılıyor. Çıkmak için bir tuşa basın.");
                Console.ReadKey();
            }
        }
    }
}