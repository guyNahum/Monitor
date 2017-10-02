using Agent.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agent.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            Comunicator comunicator = new Comunicator();
            comunicator.StartListen();
        }
    }
}
