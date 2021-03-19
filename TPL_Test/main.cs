using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Numerics;
namespace ParallelTasksExample
{
    //Kod porównuje czas wykonania operacji zaimplementowanych przy użyciu klasycznych wątków oraz przy użyciu biblioteki TPL
    //Wykonana operacja polega na polieczeniu x liczb ciągku fibbon fibonacciego. 
    //Fuknca rwracająca liczbę z ciągu przyjmuje 1 parametr oznaczający numer liczby w ciągu. Każdy wątek wywołuje funkcję 10 razy dla 10 kolejnych liczb.
    class Program
    {
        static void Main(string[] args)
        {
            //Zmienne pomocnicze

            const int maxThreadAllowed = 20; //maksymalna liczba wątków
            var watch = new Stopwatch(); //obiekt do porównania czasów wykonania kodu przez wątki vs bilboteką TPL


            /*-------------------------------------------------------------*/
            /*---------------------Wątki-----------------------------------*/
            /*-------------------------------------------------------------*/
            var mres = new ManualResetEventSlim[maxThreadAllowed];
            for (int i = 0; i < mres.Length; i++)
            {
                mres[i] = new ManualResetEventSlim(false); //ustawiam początkową wartość sygnału na false
            }
            watch.Start();
            
            //Pętla uruchamiające kalsyczne wątki
            for (int i = 0; i < mres.Length; i++)
            {
                var id = i;
                var thread = new Thread(state =>
                {
                for (int j = 0; j < 10; j++)
                {

                        //uruchamiam 10 operacji dla każdego wątku
                        Console.WriteLine(string.Format("{0}, {1} Liczba ciągu Fibbonaciego: {2}", state.ToString(), (id * 10 + j).ToString(), Operat.Fibbonaci(id * 10 + j)));

                    }
                    // wysłamy zdarzenie zmieniające wartoś sygnału
                    mres[id].Set();

                });
                // Uruchamiany wątek
                thread.Start(string.Format("Wątek: {0}", i));
            }

            //oczekiwanie na sygnał dla każego elementu listy
            WaitHandle.WaitAll((from x in mres select x.WaitHandle).ToArray());
            
            //zapisanie czasu wykonania wątkku
            var threadTime = watch.ElapsedMilliseconds;
            watch.Reset();
            foreach (ManualResetEventSlim t in mres)
            {
                //zmiana sygnału powodująca blokowanie wątku
                t.Reset();
            }

            watch.Start();
            /*-------------------------------------------------------------*/
            /*---------------------Task Parallel Library-------------------*/
            /*-------------------------------------------------------------*/

            for (int i = 0; i < mres.Length; i++)
            {
                var id = i;
                Task.Factory.StartNew(state =>
                {
                    for (int j = 0; j < 10; j++)
                    {
                        Console.WriteLine(string.Format("TPL {0}, {1} Liczba ciągu Fibbonaciego: {2}", state.ToString(), (id * 10 + j).ToString(), Operat.Fibbonaci(id * 10 + j)));

                    }
                    mres[id].Set();

                }, string.Format("Zadanie: {0}", i));
            }
            WaitHandle.WaitAll((from x in mres select x.WaitHandle).ToArray());
            // zapisuję czas wykonania zadania
            var taskTime = watch.ElapsedMilliseconds;



            // wypisuję czasy wykonania operacji
            Console.WriteLine("Tradycyjne podejście przy użyciu wątków: {0}ms", threadTime);
            Console.WriteLine("Podejście przy użyciu TPL: {0}ms", taskTime);
            foreach (var item in mres)
            {
                // zmieniamy status zdarzenia
                item.Reset();
            }
            Console.ReadKey();

        }
    }
}
