using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Management;
using System.Diagnostics;

namespace PAKtA
{
    class Program
    {
        static int Main(string[] args)
        {

            string[] argsset = { "-T", "-I", "-X" };
            string okkey = "";
            string result = "";
            bool xt = false;
            Process pp = getparent();
            

            // Check arguments
            if (args.Length == 0 || args[0] == "-?" || !args.Contains(argsset[0]) )
            {
                WriteHelp();
                return(-1);
            }

            int timeout = Int32.Parse(args[(Array.FindIndex(args, tmp => args.Contains(argsset[0]))) + 1]);

            if (args.Contains(argsset[1]))
            {
                okkey = args[(Array.FindIndex(args, tmp => tmp.Contains(argsset[1]))) + 1];
                if (okkey.Length > 1)
                {
                    WriteHelp();
                    return (-1);
                }
            }

            if (args.Contains(argsset[2]))
            {
                xt = true;
                //pp = getparent();

            }

            Thread cntThread;
            cntThread = new Thread(() => revcount(timeout)); 
            cntThread.Start();

            try
            {
                Console.WriteLine("Waiting for interruption, otherwise will continue");
                result = Reader.ReadLine(timeout);
                
            }
            catch (TimeoutException)
            {

                if (!xt) { return (0); } //cntThread.Abort(); 
                else { pp.Kill(); } //parent.Kill(); 
                
            }

            cntThread.Abort();
            Console.ResetColor();
            
            if (!xt)
            {
                if (okkey != "" && okkey.Equals(result, StringComparison.OrdinalIgnoreCase))
                { return (1); }
                else { return (0); }
            }
            else
            {
                if (okkey != "" && okkey.Equals(result, StringComparison.OrdinalIgnoreCase))
                { return (1); } //Environment.Exit(0);
                else { pp.Kill(); }
            }
            
          

            //general failure. It shouldn't happen
            return (-2);
        }

        private static void WriteHelp()
        {
            String assemblyName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
            Console.WriteLine("PAKtA = Press Any Key to Abort");
            Console.WriteLine("Press any key to INTERRUPT (not continue) batch task while timeout goes.");
            Console.WriteLine("If no key is pressed during timeout, batch will continue normally.");
            Console.WriteLine();
            Console.WriteLine(@"{0} -T timeout ms (-I key to continue) (-X KILL PARENT PROCESS instead of generating errorlevel)", assemblyName);
        }

        private static void revcount(int tmot)
        {
            
            for (int i = 0; i < tmot; ++i )
            {
                Thread.Sleep(1);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("\r{0} ", ((tmot-i)/1000).ToString());
                Console.ResetColor();
                Console.Write(((tmot - i) % 1000).ToString());
            }
        }

        private static System.Diagnostics.Process getparent()
        {

            var myId = Process.GetCurrentProcess().Id;
            var query = string.Format("SELECT ParentProcessId FROM Win32_Process WHERE ProcessId = {0}", myId);
            var search = new ManagementObjectSearcher("root\\CIMV2", query);
            var results = search.Get().GetEnumerator();
            results.MoveNext();
            var queryObj = results.Current;
            var parentId = (uint)queryObj["ParentProcessId"];
            var parent = Process.GetProcessById((int)parentId);
            return parent;
        }

    }

    class Reader
    {
        private static Thread inputThread;
        private static AutoResetEvent getInput, gotInput;
        private static string input;

        static Reader()
        {
            getInput = new AutoResetEvent(false);
            gotInput = new AutoResetEvent(false);
            inputThread = new Thread(reader);
            inputThread.IsBackground = true;
            inputThread.Start();
        }

        private static void reader()
        {
            while (true)
            {
                getInput.WaitOne();
                input = Console.ReadKey().Key.ToString();
                gotInput.Set();
            }
        }

        // omit the parameter to read a line without a timeout
        public static string ReadLine(int timeOutMillisecs = Timeout.Infinite)
        {
            getInput.Set();
            bool success = gotInput.WaitOne(timeOutMillisecs);
            if (success)
                return input;
            else
                throw new TimeoutException("User did not provide input within the timelimit.");
        }
    }
}
