using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Timers;
using IWshRuntimeLibrary;

namespace ASIC_TEST
{
    internal class Program
    {
        public static void Main()
        {
            string path = "BadApple_ZOV.gif";
            string fullPath = System.IO.Path.GetFullPath(path);
            Bitmap image = new Bitmap(fullPath);
            FrameDimension dimension = new FrameDimension(image.FrameDimensionsList[0]);
            StringBuilder sb;
            int left = Console.WindowLeft, top = Console.WindowTop, frameCount = image.GetFrameCount(dimension), time = 0;
            Stopwatch stopwatch = new Stopwatch();

            for (int z = 0; ; z = (z + 1) % frameCount)
            {

                stopwatch.Restart(); // Запускаем таймер для измерения времени обработки кадра
                sb = new StringBuilder();
                image.SelectActiveFrame(dimension, z);

                BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                unsafe
                {
                    byte* scan0 = (byte*)bitmapData.Scan0.ToPointer();

                    for (int i = 0; i < image.Height; i++)
                    {
                        for (int j = 0; j < image.Width; j++)
                        {
                            int index = i * bitmapData.Stride + j * 4;
                            byte blue = scan0[index];
                            if (blue < 90)
                            {
                                sb.Append(' ');
                            }
                            else
                            {
                                sb.Append("#");
                            }
                            sb.Append(' ');
                            //j = j + 6;
                        }
                        sb.Append('\n');
                        //i = i + 11;
                    }
                }
                image.UnlockBits(bitmapData);

                stopwatch.Stop(); // Останавливаем таймер
                time++;
                Console.SetCursorPosition(left, top);
                Console.Write(sb.ToString());
               
                // Вычисляем FPS и выводим в консоль
                double fps = 1000.0 / stopwatch.ElapsedMilliseconds;
                Console.SetCursorPosition(left, top + image.Height);
                Console.Write($"FPS: {fps:F2}");
                
            }
        }

    }

}

