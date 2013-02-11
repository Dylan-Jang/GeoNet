using System;

namespace GeoNetConsole
{
    class Program
    {

        /*
         * **************** nic locations **********************
         * 
        APNIC
        ftp://ftp.apnic.net/pub/stats/apnic/delegated-apnic-latest
        ARIN
        ftp://ftp.arin.net/pub/stats/arin/delegated-arin-latest
        LACNIC
        ftp://ftp.lacnic.net/pub/stats/lacnic/delegated-lacnic-latest
        RIPE NCC
        ftp://ftp.ripe.net/pub/stats/ripencc/delegated-ripencc-latest
        AFRINIC
        ftp://ftp.afrinic.net/pub/stats/afrinic/delegated-afrinic-latest
         * 
         */

        
        static void Main()
        {


            string line;
            Console.WriteLine("Enter '1' to generate the binary file, or '2' to test current implementation:");
            Console.WriteLine();
            do
            {
                
                line = Console.ReadLine();
                if (line != null)
                {
                    switch (line)
                    {
                        case "1" :
                            BuildFactory.BuildLists();
                            goto exit;
                          

                        case "2" :

                            TestFactory.TestGeoNet();
                            goto exit;
                        

                        default:
                            Console.WriteLine("Sorry, 1 or 2 please!");
                            break;
                    }

                }
            } while (line != null);
            exit:
            Console.ReadLine();


        }





    }
}
    

