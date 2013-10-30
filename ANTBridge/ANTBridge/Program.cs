using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ANT_Managed_Library;

namespace ANTBridge
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ANTBridge bridge = new ANTBridge();


                while (true)
                {

                }
            }
            catch (ANT_Exception ex)
            {
                Console.WriteLine("An ANT Exception has occured: " + Environment.NewLine + ex.Message);
            }
            Console.WriteLine("Exiting...");
        }
    }
}
