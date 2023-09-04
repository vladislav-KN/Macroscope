using System.Buffers;

namespace Player
{

    public interface IStream
    {
        /// <summary>
        /// Функция записи изображения
        /// </summary>
        /// <param name="time">время отправки или получения изображение</param>
        /// <param name="imageBytes">массив байтов с изображением</param>
        public void WriteImage(System.DateTime time, byte[] imageBytes);
    }
}