using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ChatServer
{
    class Program
    {
        // Sunucu portu
        private const int Port = 13000;

        static void Main(string[] args)
        {
            TcpListener listener = null;

            try
            {
                // 1. TcpListener nesnesini oluştur ve tüm yerel IP'leri dinle
                listener = new TcpListener(IPAddress.Any, Port);

                // 2. Dinlemeye Başla
                listener.Start();
                Console.WriteLine($"Sunucu {Port} portunda dinlemede. Bağlantı bekleniyor...");

                // 3. İstemciyi Kabul Et (Bağlantı gelene kadar bloke olur)
                // Bu, sadece tek bir istemciyi kabul edebilen basit bir sunucu örneğidir.
                using (TcpClient client = listener.AcceptTcpClient())
                {
                    Console.WriteLine("İstemci bağlandı!");

                    // 4. Veri Akışını (NetworkStream) Al
                    using (NetworkStream stream = client.GetStream())
                    {
                        byte[] buffer = new byte[1024];
                        int bytesRead;

                        // 5. İstemciden gelen veriyi sürekli oku
                        // stream.Read, veri gelene kadar bloke olur.
                        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            // Baytları okunabilir metne çevir
                            string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                            Console.WriteLine($"İstemciden gelen: {data}");

                            // 6. İstemciye Cevap Gönder
                            string responseMessage = $"Sunucu aldı: {data} ({DateTime.Now.ToShortTimeString()})";
                            byte[] response = Encoding.UTF8.GetBytes(responseMessage);
                            stream.Write(response, 0, response.Length);
                        }
                    } // NetworkStream otomatik kapatılır
                } // TcpClient otomatik kapatılır (Dispose)
            }
            catch (SocketException e)
            {
                Console.WriteLine($"Socket Hatası: {e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Genel Hata: {e.Message}");
            }
            finally
            {
                // 7. Dinleyiciyi kapat
                if (listener != null)
                {
                    listener.Stop();
                }
                Console.WriteLine("Sunucu durduruldu. Çıkmak için bir tuşa basın.");
                Console.ReadKey();
            }
        }
    }
}