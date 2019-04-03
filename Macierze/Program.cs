using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Macierze
{
    class Program
    {
        unsafe static void Main(string[] args)
        {
            int n = 2048;
            int* A = (int*)Marshal.AllocHGlobal(sizeof(int) * n * n);
            int* B = (int*)Marshal.AllocHGlobal(sizeof(int) * n * n);

            /* tylko do testow poprawnosci
            int[,] a = new int[,] { { 5, -2, 7, 3 }, { 4, 6, -5, 1 }, { -2, 4, -3, 1 }, { 7, 9, 2, 8 } };
            int[,] b = new int[,] { { 4, 1, -1, -2 }, { 3, 9, 6, -8 }, { -7, 2, 1, 1 }, { -4, 6, -1, -5 } };
            int iter = 0;
            foreach (int x in a)
            {
                A[iter++] = x;
            }
            iter = 0;
            foreach (int y in b)
            {
                B[iter++] = y;
            }
            */

            Random rnd = new Random();
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    A[i * n + j] = rnd.Next(-n * n / 2, n * n / 2 + 1);
                    B[i * n + j] = rnd.Next(-n * n / 2, n * n / 2 + 1);
                }
            }

            /* wyświetlanie macierzy - czasochłonne => do testów poprawności
            Console.WriteLine("Macierz A:");
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Console.Write("{0}\t", A[i * n + j]);
                }
                Console.Write("\n");
            }

            Console.WriteLine("\nMacierz B:");
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Console.Write("{0}\t", B[i * n + j]);
                }
                Console.Write("\n");
            }
            */

            long* C = (long*)Marshal.AllocHGlobal(sizeof(long) * n * n);
            int* BT = (int*)Marshal.AllocHGlobal(sizeof(int) * n * n);

            
            Stopwatch watch = new Stopwatch();
            watch.Start();
            //mnozenie przez macierz transponowana
            //for (int i = 0; i < n; i++)
            //{
            //    for (int j = 0; j < n; j++)
            //    {
            //        BT[i * n + j] = B[i + j * n];
            //    }
            //}
            //for (int i = 0; i < n; i++) // zrownoleglenie na 2 petli Cij
            //{
            //    Parallel.For(0, n, (j) =>
            //    {
            //        C[i + j * n] = 0;
            //        for (int k = 0; k < n; k++)
            //        {
            //            C[i + j * n] += A[j * n + k] * BT[i * n + k];
            //        }
            //    });
            //}

            //for (int i = 0; i < n; i++) // zrownoleglenie na 2 petli Cji
            //{
            //    Parallel.For(0, n, (j) =>
            //    {
            //        C[i * n + j] = 0;
            //        for (int k = 0; k < n; k++)
            //        {
            //            C[i * n + j] += A[i * n + k] * BT[j * n + k];
            //        }
            //    });
            //}

            //Parallel.For(0, n, (i) => // zrownoleglenie na 1 petli Cij
            //{
            //    for (int j = 0; j < n; j++)
            //    {
            //        C[i + j * n] = 0;
            //        for (int k = 0; k < n; k++)
            //        {
            //            C[i + j * n] += A[j * n + k] * BT[i * n + k];
            //        }
            //    }
            //});

            //Parallel.For(0, n, (i) => // zrownoleglenie na 1 petli Cji
            //{
            //    for (int j = 0; j < n; j++)
            //    {
            //        C[i * n + j] = 0;
            //        for (int k = 0; k < n; k++)
            //        {
            //            C[i * n + j] += A[i * n + k] * BT[j * n + k];
            //        }
            //    }
            //});




            // mnozenie przez macierz poczatkowa

            //for (int i = 0; i < n; i++) // mnozenie przez macierz poczatkowa zrownoleglenie na 2 petli Cij
            //{
            //    Parallel.For(0, n, (j) =>
            //    {
            //        C[i + j * n] = 0;
            //        for (int k = 0; k < n; k++)
            //        {
            //            C[i + j * n] += A[j * n + k] * B[i + k * n];
            //        }
            //    });
            //}

            //for (int i = 0; i < n; i++) // mnozenie przez macierz poczatkowa zrownoleglenie na 2 petli Cji
            //{
            //    Parallel.For(0, n, (j) =>
            //    {
            //        C[i * n + j] = 0;
            //        for (int k = 0; k < n; k++)
            //        {
            //            C[i * n + j] += A[i * n + k] * B[j + k * n];
            //        }
            //    });
            //}


            Parallel.For(0, n, (i) => // mnozenie przez macierz poczatkowa zrownoleglenie na 1 petli Cij
            {
                for (int j = 0; j < n; j++)
                {
                    C[i + j * n] = 0;
                    for (int k = 0; k < n; k++)
                    {
                        C[i + j * n] += A[j * n + k] * B[i + k * n];
                    }
                }
            });

            //Parallel.For(0, n, (i) => // mnozenie przez macierz poczatkowa zrownoleglenie na 1 petli Cji
            //{
            //    for (int j = 0; j < n; j++)
            //    {
            //        C[i * n + j] = 0;
            //        for (int k = 0; k < n; k++)
            //        {
            //            C[i * n + j] += A[i * n + k] * B[j + k * n];
            //        }
            //    }
            //});

            watch.Stop();

            /* wyświetlanie macierzy wynikowej czasochłonne => do testów poprawności
            Console.WriteLine("\nMacierz wynikowa C:");
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Console.Write("{0}\t", C[i * n + j]);
                }
                Console.Write("\n");
            }
            */

            Console.WriteLine("Czas przetwarzania: {0}ms", watch.ElapsedMilliseconds);
            Console.ReadKey();
        }
    }
}
