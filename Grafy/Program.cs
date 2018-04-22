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

            for (int i = 0; i < saturate-n+1; i++) // dzięki "-n" wcześniejsza pętla nie zajmuje dodatkowej złożoności czasowej + zgadza się nasycenie; +1 bo we wczesniejszej pętli odjeliśmy
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
                //Console.Write("-1 \n");
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

        // --- MG ---

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

        static void TransformToGrafMatrix(ref int[,] adjacencyMatrix, ref int[,] grafMatrix, int n) // transformacja na MG
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

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n + 3; j++)
                {
                    Console.Write(grafMatrix[i, j] + ",");
                }
                Console.Write("\n");
            }
        }

        // ------

        static void AdjecencyBFSSort(ref int[,] adjecencyMatrix, int n) // sortowanie BFS z macierzy sąsiedztwa
        {
            int[] amountPredecessor = new int[n]; // tabela ze zliczonymi następnikami dla danego n, czyli węzła
            int[] sorted = new int[n];
            int sortIndex = 0;

            // uzupełnienie tabeli z liczbą poprzedników // Inne dla każdej reprezentacji
            for (int i=0; i<n; i++) 
            {
                for (int j=0; j<n; j++) 
                {
                    if (adjecencyMatrix[j, i] == 1) amountPredecessor[i]++; // iterujemy w kolumnie wiersze, gdy ma poprzednika to zwiększamy liczbę
                }
            }

            //szukanie kolejnych miesjc "0" - bez krawędzi
            for(int i=0; sortIndex < n; i++)
            {
                // wyszukiwanie wierzchołków, bez krawędzi wejściowych
                if (amountPredecessor[i] == 0)
                {
                    sorted[sortIndex] = i; // zapisanie w posortowanej
                    sortIndex++; // przesunięcie indeksu posortowanej tablicy
                    amountPredecessor[i] = -1; // "Usunięcie" z tabeli

                    // obniżenie w tabeli o 1 -> Przeszukanie połączeń // Inne dla każdej reprezentacji
                    for (int j=0; j<n; j++)
                    {
                        if (adjecencyMatrix[i, j] == 1) amountPredecessor[j]--;
                    }
                    i = 0; // po udanym wejsciu zaczynamy od 0
                }
                
                if(i+1 >= n) // jeśli będzie błąd
                {
                    Console.WriteLine("To nie jest graf acykliczny!");
                    break;
                }
            }

            //// --- Wyswietlenie ---

            for (int i = 0; i < n; i++)
            {
                Console.Write(sorted[i] + " ");
            }
            Console.Write("\n");


        }

        static void ComminfBFSSort(ref List<int>[] commingList, int n)
        {
            int[] amountPredecessor = new int[n]; // tabela ze zliczonymi następnikami dla danego n, czyli węzła
            int[] sorted = new int[n];
            int sortIndex = 0;

            // uzupełnienie tabeli z liczbą poprzedników
            for (int i = 0; i < n; i++)
            {
                foreach (int item in commingList[i])
                {
                    if (item != -1) amountPredecessor[item]++;
                }
            }

            //szukanie kolejnych miesjc "0" - bez krawędzi
            for (int i = 0; sortIndex < n; i++)
            {
                // wyszukiwanie wierzchołków, bez krawędzi wejściowych
                if (amountPredecessor[i] == 0)
                {
                    sorted[sortIndex] = i; // zapisanie w posortowanej
                    sortIndex++; // przesunięcie indeksu posortowanej tablicy
                    amountPredecessor[i] = -1; // "Usunięcie" z tabeli

                    // obniżenie w tabeli o 1 -> Przeszukanie połączeń // Inne dla każdej reprezentacji
                    foreach (int item in commingList[i])
                    {
                        if (item != -1) amountPredecessor[item]--;
                    }


                    i = 0; // po udanym wejsciu zaczynamy od 0
                }


                if (i + 1 >= n) // jeśli będzie błąd
                {
                    Console.WriteLine("To nie jest graf acykliczny!");
                    break;
                }
            }

            //// --- Wyswietlenie ---

            for (int i = 0; i < n; i++)
            {
                Console.Write(sorted[i] + " ");
            }
            Console.Write("\n");
        }


        static void EdgeBFSSort(ref int[,] edgeTable, int n, int m) // n- ilość wierzchołków, m - ilość połączeń -> saturate
        {
            int[] amountPredecessor = new int[n]; // tabela ze zliczonymi następnikami dla danego n, czyli węzła
            int[] sorted = new int[n];
            int sortIndex = 0;

            // uzupełnienie tabeli z liczbą poprzedników
            for (int i = 0; i < m; i++)
            {
                amountPredecessor[edgeTable[i, 1]]++;
            }

            //szukanie kolejnych miesjc "0" - bez krawędzi
            for (int i = 0; sortIndex < n; i++)
            {
                // wyszukiwanie wierzchołków, bez krawędzi wejściowych
                if (amountPredecessor[i] == 0)
                {
                    sorted[sortIndex] = i; // zapisanie w posortowanej
                    sortIndex++; // przesunięcie indeksu posortowanej tablicy
                    amountPredecessor[i] = -1; // "Usunięcie" z tabeli

                    // obniżenie w tabeli o 1 -> Przeszukanie połączeń // Inne dla każdej reprezentacji
                    for(int j=0; j<m; j++)
                    {
                        if (edgeTable[j, 0] == i) amountPredecessor[edgeTable[j, 1]]--;
                    }

                    i = 0; // po udanym wejsciu zaczynamy od 0
                }
            }

            // --- Wyswietlenie ---

            for (int i = 0; i < n; i++)
            {
                Console.Write(sorted[i] + " ");
            }
            Console.Write("\n");
        }


        static void GrafMatrixBFSSort(ref int[,] grafMatrix, int n)
        {
            int[] amountPredecessor = new int[n]; // tabela ze zliczonymi następnikami dla danego n, czyli węzła
            int[] sorted = new int[n];
            int sortIndex = 0;
            int helper = 0;

            // uzupełnienie tabeli z liczbą poprzedników na podstawie wartości w macierzy incydencj nakłądanych z listy poprzedników (w skrócie uwaga na ujmne)
            for (int i = 0; i < n; i++) // iteracja po wierszach
            {
                helper = grafMatrix[i, n+1]; // helper pokazuje co będzie następne pokazane 

                while(helper != -1) // sprawdzenie, czy istnieje jakikolwiek poprzednik
                {
                    amountPredecessor[i]++; // zliczenie
                    if (helper == grafMatrix[i, helper] * (-1) ) break; // jeśli trafimy na koniec, to wychodzimy
                    helper = grafMatrix[i, helper] * (-1); // ustawienie nowej wartości helpera na kolejny element ; "-1", bo musimy mieć dodatnią
                }
            }

            //szukanie kolejnych miesjc "0" - bez krawędzi
            for (int i = 0; sortIndex < n; i++)
            {
                // wyszukiwanie wierzchołków, bez krawędzi wejściowych
                if (amountPredecessor[i] == 0)
                {
                    sorted[sortIndex] = i; // zapisanie w posortowanej
                    sortIndex++; // przesunięcie indeksu posortowanej tablicy
                    amountPredecessor[i] = -1; // "Usunięcie" z tabeli

                    // obniżenie w tabeli o 1 -> Przeszukanie połączeń // Inne dla każdej reprezentacji
                    helper = grafMatrix[i, n]; // helper pokazuje co będzie następne pokazane 

                    while (helper != -1)
                    {
                        amountPredecessor[helper]--; // zliczenie
                        if (helper == grafMatrix[i, helper]) break; // jeśli trafimy na koniec, to wychodzimy
                        helper = grafMatrix[i, helper]; // ustawienie nowej wartości helpera na kolejny element
                    }

                    i = 0; // po udanym wejsciu zaczynamy od 0
                }
            }

            // --- Wyswietlenie ---

            for (int i = 0; i < n; i++)
            {
                Console.Write(sorted[i] + " ");
            }

        }
        



        static void DFS(int n, ref int[,] adjacencyMatrix, ref List<int>[] commingList, ref int[,] EdgeTable, ref int[,] grafMatrix)
        {
            //Taki Main dla dfsów, stąd idzie cała lista visited, sorted i wypisywanie testowe; zrobilem tak ze wzgledu na rekurencyjny charakter DFSów

            //inicjujemy tablice elementow posortowanych i odwiedzonych
            int[] sorted = new int[n];
            int[] visited = new int[n];
            int VCounter = 0, Scounter=n-1; //licznik pierwszego wolnego elementu tablicy visited i sorted

            //algorytm dla macierzy sąsiedztwa
            for (int i = 0; i < n; i++) //dla każdego wiezrchołka
            {
             if(!visited.Contains(i))  AdjecencyDFSSort(ref adjacencyMatrix, ref sorted, ref visited, i,ref Scounter, ref VCounter,n); //jeśli w visited nie ma wierzchołka, wywołujemy
            }

            //wyświetlanie
            Console.Write("\n");
            for (int i = 0; i < n; i++)
            {

                Console.Write(sorted[i] + " ");
            }
         
            // Resetujemy zmienne i robimy dokładnie to samo- powtorzyc razy tyle ile konieczne
            sorted = null; visited = null; sorted = new int[n]; visited = new int[n];

            VCounter = 0; Scounter = n - 1;
            //algorytm dla Listy następników
            for (int i = 0; i < n; i++) //dla każdego wiezrchołka
            {
                if (!visited.Contains(i)) ComminfDFSSort(ref commingList, ref sorted, ref visited, i, ref Scounter, ref VCounter, n); //jeśli w visited nie ma wierzchołka, wywołujemy
            }

            //wyświetlanie
            Console.Write("\n");
            for (int i = 0; i < n; i++)
            {

                Console.Write(sorted[i] + " ");
            }
            // Resetujemy zmienne i robimy dokładnie to samo- powtorzyc razy tyle ile konieczne
            sorted = null; visited = null; sorted = new int[n]; visited = new int[n];

            VCounter = 0; Scounter = n - 1;
            //algorytm dla tabeli krawędzi
            for (int i = 0; i < n; i++) //dla każdego wiezrchołka
            {
                if (!visited.Contains(i)) EdgeDFSSort(ref EdgeTable, ref sorted, ref visited, i, ref Scounter, ref VCounter, n); //jeśli w visited nie ma wierzchołka, wywołujemy
            }

            //wyświetlanie
            Console.Write("\n");
            for (int i = 0; i < n; i++)
            {

                Console.Write(sorted[i] + " ");
            }
            // Resetujemy zmienne i robimy dokładnie to samo- powtorzyc razy tyle ile konieczne
            sorted = null; visited = null; sorted = new int[n]; visited = new int[n];

            VCounter = 0; Scounter = n - 1;
            //algorytm dla macierzy grafu
            for (int i = 0; i < n; i++) //dla każdego wiezrchołka
            {
                if (!visited.Contains(i)) GrafMatrixDFSSort(ref grafMatrix, ref sorted, ref visited, i, ref Scounter, ref VCounter, n); //jeśli w visited nie ma wierzchołka, wywołujemy
            }

            //wyświetlanie
            Console.Write("\n");
            for (int i = 0; i < n; i++)
            {

                Console.Write(sorted[i] + " ");
            }
        }
        static void GrafMatrixDFSSort(ref int[,] grafMatrix, ref int[] sorted, ref int[] visited, int i, ref int Scounter, ref int VCounter, int n)
        {

            visited[VCounter] = i; //wstawiamy do odwiedzonych zwiekszamy counter o 1
            VCounter++;
            //sprawdzamy wiersz naszego elementu i, pole n+1 i pola od 0 do n-1 (zastosowana inna kolejnosc elementów
            if(!visited.Contains(grafMatrix[i,n+1])) GrafMatrixDFSSort(ref grafMatrix, ref sorted, ref visited, grafMatrix[i, n+1], ref Scounter, ref VCounter, n);
            for (int j = 0; j < n; j++)//dla wszystkich elementow sprawdzamy czy sa nastepnikami i czy nie ma ich juz w visited- jesli spelnia te warunki, wywolujemy rekurencyjnie
                if (!visited.Contains(grafMatrix[i, j]) && grafMatrix[i, j]<n&& grafMatrix[i, j]>=0 && grafMatrix[i, j]!=j) GrafMatrixDFSSort(ref grafMatrix, ref sorted, ref visited, grafMatrix[i, j], ref Scounter, ref VCounter, n);


            sorted[Scounter] = i; //skoro przeszliśmy już wszystkie następniki to możemy wpisać wierzchołek do tablicy
            Scounter--;



        }
        static void EdgeDFSSort(ref int[,] EdgeTable, ref int[] sorted, ref int[] visited, int i, ref int Scounter, ref int VCounter, int n)
        {

            visited[VCounter] = i; //wstawiamy do odwiedzonych zwiekszamy counter o 1
            VCounter++;
            //dla wszystkich elementow sprawdzamy czy sa nastepnikami i czy nie ma ich juz w visited- jesli spelnia te warunki, wywolujemy rekurencyjnie
            for (int x = 0; x < n; x++) //musimy przeleciec po calej tablicy dwoma petlami
            {
            if(EdgeTable[x,0]==i && !visited.Contains(EdgeTable[x,1]))
                {
                    int j = EdgeTable[x, 1];
                    EdgeDFSSort(ref EdgeTable, ref sorted, ref visited, j, ref Scounter, ref VCounter, n);
                }


            }
               


            sorted[Scounter] = i; //skoro przeszliśmy już wszystkie następniki to możemy wpisać wierzchołek do tablicy
            Scounter--;



        }
        static void ComminfDFSSort(ref List<int>[] commingList,ref int[] sorted, ref int[] visited, int i,ref int Scounter, ref int VCounter, int n)
        {
            visited[VCounter] = i; //wstawiamy do odwiedzonych zwiekszamy counter o 1
            VCounter++;
            foreach (int j in commingList[i])//dla wszystkich elementow sprawdzamy czy sa nastepnikami i czy nie ma ich juz w visited- jesli spelnia te warunki, wywolujemy rekurencyjnie
            {
              
                if (!visited.Contains(j)&&j!=-1) ComminfDFSSort(ref commingList, ref sorted, ref visited, j, ref Scounter, ref VCounter, n);
            }
            


            sorted[Scounter] = i; //skoro przeszliśmy już wszystkie następniki to możemy wpisać wierzchołek do tablicy
            Scounter--;

        }
        static void AdjecencyDFSSort(ref int[,]  adjacencyMatrix, ref int[] sorted, ref int[] visited, int i, ref int Scounter, ref int VCounter, int n)
        {
        
              visited[VCounter] = i; //wstawiamy do odwiedzonych zwiekszamy counter o 1
                VCounter++;
                for (int j = 0; j < n; j++)//dla wszystkich elementow sprawdzamy czy sa nastepnikami i czy nie ma ich juz w visited- jesli spelnia te warunki, wywolujemy rekurencyjnie
                    if (!visited.Contains(j) && adjacencyMatrix[i,j] == 1) AdjecencyDFSSort(ref adjacencyMatrix, ref sorted, ref visited, j, ref Scounter, ref VCounter, n);


                sorted[Scounter] = i; //skoro przeszliśmy już wszystkie następniki to możemy wpisać wierzchołek do tablicy
                Scounter--;
         
        
      
        }

        static void Main()
        {
            int n = 20; // długość boku -> liczba wierzchołków
            int saturate = (n * (n + 1) / 2) / 2; // nasycenie -> ilość połączeń
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

            AdjecencyBFSSort(ref adjacencyMatrix, n);
            ComminfBFSSort(ref commingList, n);
            EdgeBFSSort(ref edgeTable, n, saturate);
            GrafMatrixBFSSort(ref grafMatrix, n);
      
            DFS(n, ref adjacencyMatrix, ref commingList,ref edgeTable, ref grafMatrix);




            Console.Read();

        }
    }
}
