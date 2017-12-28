using System;
using BBTransaction.Factory;
using BBTransaction.Factory.Context;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var transaction = new TransactionFactory().Create<int, string>(options => 
            {
                
            });
        }
    }
}
