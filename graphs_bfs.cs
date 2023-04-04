using System;
using ASD.Graphs;
using System.Collections.Generic;
using System.Collections;

namespace ASD
{
    public class Lab04 : MarshalByRefObject
    {
        /// <summary>
        /// Etap 1 - wyznaczanie numerów grup, które jest w stanie odwiedzić Karol, zapisując się na początku do podanej grupy
        /// </summary>
        /// <param name="graph">Ważony graf skierowany przedstawiający zasady dołączania do grup</param>
        /// <param name="start">Numer grupy, do której początkowo zapisuje się Karol</param>
        /// <returns>Tablica numerów grup, które może odwiedzić Karol, uporządkowana rosnąco</returns>
        public int[] Lab04Stage1(DiGraph<int> graph, int start)
        {

            DiGraph newGraph = new DiGraph((graph.VertexCount + 1) * graph.VertexCount);
            Stack<int> sta = new Stack<int>();
            for (int i = 0; i < graph.VertexCount; i++)
            {
                foreach (int j in graph.OutNeighbors(i))
                {
                    int k = graph.GetEdgeWeight(i, j);
                    if (k == -1 && start == i)
                    {
                        sta.Push(j * graph.VertexCount + start);
                        continue;
                    }
                    if (k == -1)
                    {
                        continue;
                    }
                    else
                    {
                        newGraph.AddEdge(i * graph.VertexCount + k, j * graph.VertexCount + i);
                    }
                }
            }
            bool[] visited = new bool[newGraph.VertexCount];
            bool[] news = new bool[graph.VertexCount];
            news[start] = true;
            List<int> path = new List<int>();
            while (sta.Count > 0)
            {
                int curr = sta.Pop();
                if (visited[curr] == false)
                {
                    visited[curr] = true;
                    news[curr / graph.VertexCount] = true;
                    foreach (int neigh in newGraph.OutNeighbors(curr))
                    {
                        if (visited[neigh] == false)
                        {
                            sta.Push(neigh);
                        }
                    }
                }
            }
            for (int i = 0; i < graph.VertexCount; i++)
            {
                if (news[i] == true)
                {
                    path.Add(i);
                }
            }
            return path.ToArray();
        }

        /// <summary>
        /// Etap 2 - szukanie możliwości przejścia z jednej z grup z `starts` do jednej z grup z `goals`
        /// </summary>
        /// <param name="graph">Ważony graf skierowany przedstawiający zasady dołączania do grup</param>
        /// <param name="starts">Tablica z numerami grup startowych (trasę należy zacząć w jednej z nich)</param>
        /// <param name="goals">Tablica z numerami grup docelowych (trasę należy zakończyć w jednej z nich)</param>
        /// <returns>(possible, route) - `possible` ma wartość true gdy istnieje możliwość przejścia, wpp. false,
        /// route to tablica z numerami kolejno odwiedzanych grup (pierwszy numer to numer grupy startowej, ostatni to numer grupy docelowej),
        /// jeżeli possible == false to route ustawiamy na null</returns>
        public (bool possible, int[] route) Lab04Stage2(DiGraph<int> graph, int[] starts, int[] goals)
        {
            DiGraph newGraph = new DiGraph((graph.VertexCount + 1) * graph.VertexCount);
            Stack<int> sta = new Stack<int>();
            bool[] visited = new bool[newGraph.VertexCount];
            bool[] news = new bool[graph.VertexCount];
            int[] news2 = new int[graph.VertexCount];
            int[] prev = new int[newGraph.VertexCount];
            List<int> path = new List<int>();
            for (int i = 0; i < goals.Length; i++)
            {
                news2[goals[i]] = -2;
            }
            for (int i = 0;i < starts.Length;i++)
            {
                visited[starts[i]] = true;
                prev[starts[i]] = -1;
                if (news2[starts[i]] == -2)
                {
                    path.Add(starts[i]);
                    return (true, path.ToArray());
                }
                foreach (int j in graph.OutNeighbors(starts[i]))
                {
                    int k = graph.GetEdgeWeight(starts[i], j);
                    if (k == -1)
                    {
                        sta.Push(j * graph.VertexCount + starts[i]);
                        prev[j * graph.VertexCount + starts[i]] = starts[i];
                    }
                    else
                    {
                        newGraph.AddEdge(starts[i] * graph.VertexCount + k, j * graph.VertexCount + starts[i]);
                    }
                }
            }
            for (int i = 0; i < graph.VertexCount; i++)
            {
                foreach (int j in graph.OutNeighbors(i))
                {
                    int k = graph.GetEdgeWeight(i, j);
                    if (k == -1 || visited[i] == true)
                    {
                        continue;
                    }
                    else
                    {
                        newGraph.AddEdge(i * graph.VertexCount + k, j * graph.VertexCount + i);
                    }
                }
            }
            int kk = -5;
            while (sta.Count > 0)
            {
                int curr = sta.Pop();
                if (visited[curr] == false)
                {
                    news[curr / graph.VertexCount] = true;
                    if (news2[curr / graph.VertexCount] == -2) 
                    {
                        kk = curr;
                    }
                    news2[curr / graph.VertexCount] = curr;
                    visited[curr] = true;
                    foreach (int neigh in newGraph.OutNeighbors(curr))
                    {
                        if (visited[neigh] == false)
                        {
                            sta.Push(neigh);
                            prev[neigh] = curr;
                        }
                    }
                }
            }
            if (kk == -5) return (false, null);
            while (prev[kk] != -1)
            {
                path.Add(kk / graph.VertexCount);
                kk = prev[kk];
            }
            path.Add(kk);
            path.Reverse();
            return (true, path.ToArray());

        }
    }
}
