using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testTask
{
    class Task
    {

        public readonly Buffer buffer;
        public readonly int number;
        public readonly string hashStr;

        public Task(Buffer buffer, int number) {
            this.buffer = buffer;
            this.number = number;
        }
        public Task(string bufferStr, int number)
        {
            this.hashStr = bufferStr;
            this.number = number;
        }


    }
}
