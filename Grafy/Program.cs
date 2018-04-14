using System;
using System.Collections.Generic;
using System.Linq;

namespace Grafy
{
    // Węzły w grafie numeruję od 0


    class Program
    {
        static void GenerateAdjecencyMatrix(ref int[,] adjacencyMatrix, int saturate, int n) // generuje macierz sąsiedztwa
        {
            int x = 0, y = 0;
            Random rand = new Random();

            for(int i=0; i<n-1; i++) // pętla generująca przynajmniej jeden element w każdej kolumnie. Dzięki temu każdy węzeł ma połączenie
            {                          // n-1, bo ostatni wiersz musi być cąły pusty z racji braku 1 na przekątnej i poniżej
                y = i;
                x = rand.Next(y + 1, n); // losowanie kolumny; +1, bo przekątna
                if (adjacencyMatrix[y, x] != 1)
                {
                    adjacencyMatrix[y, x] = 1;
                }
                else
                {
                    i--; // jeśli ten element miał już 1, to zmniejszam i, żeby zgadzało się nasycenie 
                }
            }

            for (int i = 0; i < saturate-n; i++) // dzięki "-n" wcześniejsza pętla nie zajmuje dodatkowej złożoności czasowej + zgadza się nasycenie
            {
                y = rand.Next(0, n-1); // losowanie wiersza; -1, bo przekątna
                x = rand.Next(y + 1, n); // losowanie kolumny; +1, bo przekątna
                if (adjacencyMatrix[y, x] != 1)
                {
                    adjacencyMatrix[y, x] = 1;
                }
                else
                {
                    i--; // jeśli ten element miał już 1, to zmniejszam i, żeby zgadzało się nasycenie 
                }
            }


            // -- wyswietlanie --

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Console.Write(adjacencyMatrix[i, j] + " ");
                }
                Console.Write("\n");
            }
            Console.Write("\n");
        }

        static void TransformToEdgeTable(ref int[,] adjacencyMatrix, ref int[,] edgeTable, int n) // zamienia macierz sąsiedztwa na tabele krawędzi
        {
            int rate = 0; // wskaźnik miejsca w edgeTable (wykorzystywany tylko do zapisu w odpowiednim miejscu w tabeli) 
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (adjacencyMatrix[i, j] == 1)
                    {
                        edgeTable[rate, 0] = i;
                        edgeTable[rate, 1] = j;
                        rate++;
                    }                 
                }
            }

            // wyswietlanie

            //int saturate = (n * (n + 1) / 2) / 2; // nasycenie

            //for (int i = 0; i < saturate; i++)
            //{
            //    for (int j = 0; j < 2; j++)
            //    {
            //        Console.Write(edgeTable[i, j] + " ");
            //    }
            //    Console.Write("\n");
            //}
        }

        static void TransformToCommingList(ref int[,] adjacencyMatrix, ref List<int>[] commingList, int n) // zamiana na listę następników
        {
            for (int i = 0; i < n; i++) // iteracja po wierszach
            {
                //Console.Write(i + " => ");
                for (int j = 0; j < n; j++) // iteracja po kolumnach
                {
                    if (adjacencyMatrix[i, j] == 1) // szukanie zależności
                    {
                        commingList[i].Add(j);
                        //Console.Write(j + " -> ");
                    }
                }
                commingList[i].Add(-1); // znak końca
               // Console.Write("-1 \n");
            }
        }

        static void TransformToPredecessorList(ref int[,] adjacencyMatrix, ref List<int>[] predecessorList, int n) // zamiana na liste poprzedników
        {
            for (int i = 0; i < n; i++) // teraz "i" robi za iterator kolumn
            {
                //Console.Write(i + " => ");
                for (int j = 0; j < n; j++) // iterator wierszy
                {
                    if (adjacencyMatrix[j,i] == 1) // poszukiwanie następnika
                    {
                        predecessorList[i].Add(j);
                        //Console.Write(j + " -> ");
                    }
                }
                predecessorList[i].Add(-1); // znak końca
                //Console.Write("-1 \n");
            }
        }

        static void TransformToLackIncidenceList(ref int[,] adjacencyMatrix, ref List<int>[] lackIncidenceList, int n) // zmiana na listę braku incydencji
        {
            List<int> helper = new List<int>();

            for (int i = 0; i < n; i++) // iterator wezłów
            {
                //Console.Write(i + " => ");
                for (int j = 0; j < n; j++) // sprawdzenie kolumny i wierszy
                {
                    if (adjacencyMatrix[j, i] != 1) // kolumna -> gdy nie ma 1 to dodajemy do listy icydencji
                    {
                        lackIncidenceList[i].Add(j);
                    }

                    if (adjacencyMatrix[i, j] == 1) // wiersz -> gdy okazuje się, że jednak jest incydencja, to usuwamy
                    {
                        lackIncidenceList[i].Remove(j);
                    }
                }
                lackIncidenceList[i].Add(-1); // znak końca

                //foreach (int item in lackIncidenceList[i])
                //{
                //    Console.Write(item + " -> ");
                //}
                //Console.Write("\n");

            }
        }

        static void CommingListCompleteGrafMatrix(ref List<int>[] commingList, ref int[,] grafMatrix, int n) // uzupełnianie MG przy pomocy listy następników
        {
            int lastNode = -1; // będzie zawierał wartość ostatniego węzła

            for (int i = 0; i < n; i++)
            {

                foreach (int item in commingList[i])
                {
                    if (item == commingList[i].First())
                    {
                        grafMatrix[i, n] = lastNode = commingList[i].First(); // Uzupełnienie kolumny nagłówkowej
                    }
                    else if (item == -1)
                    {
                        grafMatrix[i, lastNode] = lastNode; // nadanie ostatniej wartosci
                    }
                    else
                    {
                        grafMatrix[i, lastNode] = item; // dopisanie zaznaczenie kolejnego powiązania w tabeli
                        lastNode = item;
                    }
                }

            }
        }

        static void PredecessorListCompleteGrafMatrix(ref List<int>[] predecessorList, ref int[,] grafMatrix, int n) // uzupełnianie MG przy pomocy listy następników
        {
            int lastNode = -1; // będzie zawierał wartość ostatniego węzła

            for (int i = 0; i < n; i++)
            {
                foreach (int item in predecessorList[i])
                {
                    if (item == predecessorList[i].First())
                    {
                        grafMatrix[i, n+1] = lastNode = predecessorList[i].First(); // Uzupełnienie kolumny nagłówkowej <- przez ten fragment nie mogłem zamknąć wszystkiego w jednej funkcji
                    }
                    else if (item == -1)
                    {
                        grafMatrix[i, lastNode] = lastNode*(-1); // nadanie ostatniej wartosci
                    }
                    else
                    {
                        grafMatrix[i, lastNode] = item*(-1); // dopisanie zaznaczenie kolejnego powiązania w tabeli
                        lastNode = item;
                    }
                }
            }
        }

        static void LackIncidenceListCompleteGrafMatrix(ref List<int>[] predecessorList, ref int[,] grafMatrix, int n) // uzupełnianie MG przy pomocy listy następników
        {
            int lastNode = -1; // będzie zawierał wartość ostatniego węzła

            for (int i = 0; i < n; i++)
            {
                foreach (int item in predecessorList[i])
                {
                    if (item == predecessorList[i].First())
                    {
                        grafMatrix[i, n + 2] = lastNode = predecessorList[i].First(); // Uzupełnienie kolumny nagłówkowej
                    }
                    else if (item == -1)
                    {
                        grafMatrix[i, lastNode] = lastNode + n-1; // nadanie ostatniej wartosci
                    }
                    else
                    {
                        grafMatrix[i, lastNode] = item + n-1; // dopisanie zaznaczenie kolejnego powiązania w tabeli
                        lastNode = item;
                    }
                }
            }
        }



        static void TransformToGrafMatrix(ref int[,] adjacencyMatrix, ref int[,] grafMatrix, int n)
        {
            // Proces tworzenia tablicy z Listami
            List<int>[] commingList = new List<int>[n]; // następnik
            List<int>[] predecessorList = new List<int>[n]; // poprzednik
            List<int>[] lackIncidenceList = new List<int>[n]; // brak incydencji
            for (int i = 0; i < n; i++)
            {
                commingList[i] = new List<int>(); // inicjowanie obiektu
                predecessorList[i] = new List<int>(); // inicjowanie obiektu
                lackIncidenceList[i] = new List<int>(); // inicjowanie obiektu
            }

            // ---- Tworzenie List ----
            TransformToCommingList(ref adjacencyMatrix, ref commingList, n);
            TransformToPredecessorList(ref adjacencyMatrix, ref predecessorList, n);
            TransformToLackIncidenceList(ref adjacencyMatrix, ref lackIncidenceList, n);

            // ---- Tworzenie Macierzy Grafu ----
            CommingListCompleteGrafMatrix(ref commingList, ref grafMatrix, n);
            PredecessorListCompleteGrafMatrix(ref predecessorList, ref grafMatrix, n);
            LackIncidenceListCompleteGrafMatrix(ref lackIncidenceList, ref grafMatrix, n);


            // ---- Wyswietlanie ----

            for (int i=0; i<n; i++)
            {
                for(int j=0; j<n+3; j++)
                {
                    Console.Write(grafMatrix[i, j] + "\t");
                }
                Console.Write("\n");
            }
        }

        static void Main()
        {
            int n = 8; // długość boku -> liczba wierzchołków
            int saturate = (n * (n + 1) / 2) / 2; // nasycenie
            int[,] adjacencyMatrix = new int[n,n]; // macierz sąsiedztwa
            int[,] edgeTable = new int[saturate,2]; // tabela krawędzi

            // Proces tworzenia tablicy z Listami
            List<int>[] commingList = new List<int>[n]; // Lista następników
            for (int i=0; i<n; i++) commingList[i] = new List<int>(); // inicjowanie obiektu

            int[,] grafMatrix = new int[n, n + 3]; // Macierz grafu

            GenerateAdjecencyMatrix(ref adjacencyMatrix, saturate, n);
            TransformToEdgeTable(ref adjacencyMatrix, ref edgeTable, n);
            TransformToCommingList(ref adjacencyMatrix, ref commingList, n);
            TransformToGrafMatrix(ref adjacencyMatrix, ref grafMatrix, n);



        }
    }
}
