using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace testTask
{
    class MD5Hash : IHash
    {



        public static string HashToString(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var v in bytes)
            {
                sb.Append(v.ToString("x2"));
            }
            return sb.ToString();
        }

       // public static void print(long blockCount, IDictionary<int, string> result)
       // {
       //     int number = 0;
       //     while (number < blockCount)
       //     {
       //         //здесь нахй убрать вайл тру и добавить блокировку
       // 
       // 
       //         if (result.ContainsKey(number))
       //         {
       //             Console.WriteLine("{0} : {1}", number + 1, result[number]);
       //             result.Remove(number);
       //             number++;
       //         }
       //     }
       // }

        public string Hash(byte[] buf, int bytesCount)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] hashBytes = md5.ComputeHash(buf, 0, bytesCount);
            return HashToString(hashBytes);
            
        }
    }
}
