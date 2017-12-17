using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureBlobStorage
{
    class Program
    {
        static void Main(string[] args)
        {
            BlobStorage bs = new BlobStorage();
            var data = bs.GetStorage();

            Console.ReadLine();
        }
    }
}
