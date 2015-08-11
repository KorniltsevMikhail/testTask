using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Threading;
namespace testTask
{

    class Program
    {

        static readonly int threadNumbers = Environment.ProcessorCount;
        static readonly byte[] emptyArray = new byte[0];
          

        static int Main(string[] args)
        {
            string file;
            int blockLength;
            ParseArguments(args, out file, out blockLength);

            try
            {
                using (FileStream s = new FileStream(args[0], FileMode.Open))
                {
                
                    new Worker(threadNumbers, blockLength, new MD5Hash(), s)
                        .Work();
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Файл не найден");
                Environment.Exit(1);
            }
            return 0;
        }

        private static void ParseArguments(string[] args, out string file, out int blockLength)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Не задано расположение файла и/или размер блока");
                Environment.Exit(1);                
            }
            try
            {
                file = args[0];
                blockLength = Convert.ToInt32(args[1]);
                if (blockLength == 0)
                {
                    blockLength = 1024 * 1024; // размер блока по умолчанию (1 МБ)
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Неверно задана длина блока");
                Environment.Exit(1);
                file = null;
                blockLength = 0;
            }
            catch (OverflowException)
            {
                Console.WriteLine("Значение длины блока больше максимального");
                Environment.Exit(1);
                file = null;
                blockLength = 0;
            }
            

        }
    }
}
