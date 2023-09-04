using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Buffers;
using System.IO.Pipelines;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.WebUtilities;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;

namespace Player
{
    public class MjpegStream
    {
        private readonly HttpClient _client;
        private readonly IStream _sink;
        public MjpegOption Options { get; set; }

        public MjpegStream(HttpClient client, IStream sink, MjpegOption option)
        {
            _sink = sink;
            _client = client;
            Options = option;
        }
        /// <summary>
        /// Поток передачи изображения
        /// </summary>
        /// <param name="token">токен для остановки потока</param>
        /// <returns></returns>
        public async Task StreamAsync(CancellationToken token)
        {
    
            using var request = new HttpRequestMessage(HttpMethod.Get, Options.Url);
            using var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, token);
            //Завершаем задачу если не удалось получить ответ от сервера
            if(response is null || response.StatusCode != HttpStatusCode.OK) return; 

            using var responseStream = await response.Content.ReadAsStreamAsync(token);
            
            //читаем данные
            while (!token.IsCancellationRequested)
            {
                var str = ReadString(responseStream);
                if (str != "--myboundary") break;
                ReadString(responseStream);   //пропускаем тип данных
                
                var timestamp = ReadString(responseStream);
                DateTime time;
                if (!DateTime.TryParse( timestamp.Replace("Timestamp: ", ""), out time))
                {
                    time = DateTime.Now;
                }
                ReadString(responseStream); //пропускаем дату в числовом формате
                ReadString(responseStream); //пропускаем дату с данными о временной зоне

                var length = ReadString(responseStream);
                int imgLen;
                if(!int.TryParse(length.Replace("Content-Length: ", ""), out imgLen))
                {
                    imgLen = 0;
                }
                if (ReadString(responseStream) != "") break;

                _sink.WriteImage(time, await ReadBites(responseStream, imgLen));
               
            }

        }


        /// <summary>
        /// Чтение строки из потока 
        /// </summary>
        /// <param name="stream">поток из которого получаем сообщение</param>
        /// <returns>строка полученная в потоке данных</returns>
        /// <exception cref="Exception"></exception>
        private string ReadString(Stream stream)
        {
            MemoryStream line = new MemoryStream(1024);
            line.SetLength(0);
            line.Position = 0;
            int _byte = stream.ReadByte();
            //читаем пока не будет перехода на следующую строку
            while (_byte != '\n')
            {
                if (_byte == -1)
                {
                    throw new Exception("Невозможно прочитать поток");
                }
                if (_byte != '\r')
                    line.WriteByte((byte)_byte);
                _byte = stream.ReadByte();
            }
            return Encoding.ASCII.GetString(line.ToArray());

        }

        /// <summary>
        /// Получение изображения из потока
        /// </summary>
        /// <param name="stream">поток откуда получаем изображение</param>
        /// <param name="length">длина читаемого изображения</param>
        /// <returns>массив байтов с информацией об изображении</returns>
        private async Task<byte[]> ReadBites(Stream stream, int length)
        {
            MemoryStream jpgms = new MemoryStream(10240);
            jpgms.SetLength(0);
            jpgms.Position = 0;
            int received = 0;
            int readBytes = 0;
            byte[] buff = new byte[4096];
            while (readBytes < length)
            {
                int want = length - received;
                if (want > buff.Length) want = buff.Length;

                readBytes = stream.Read(buff, 0, want);
                if (want == 0 || readBytes == 0) break;

                await jpgms.WriteAsync(buff, 0, readBytes);
                received += readBytes;
            }
            jpgms.Position = 0;
            return jpgms.ToArray();
        }

         
    }
}
