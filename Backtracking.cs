using ASD.Graphs;
using ASD.Graphs.Testing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab10
{
    public class Lab10Solution : MarshalByRefObject
    {
        /// <summary>
        /// Wariant 1: Znajdź najtańszy zbiór wierzchołków grafu G 
        /// rozdzielający wszystkie pary wierzchołków z listy fanclubs 
        /// </summary>
        /// <param name="G">Graf prosty</param>
        /// <param name="fanclubs">Lista wierzchołków, które należy rozdzielić</param>
        /// <param name="cost">cost[v] to koszt użycia wierzchołka v; koszty są nieujemne</param>
        /// <param name="maxBudget">Górne ograniczenie na koszt rozwiązania</param>
        /// <returns></returns>
        /// 
        public static bool isCon(Graph G, bool[] stop, bool[] czy, List<int> fans)
        {
            Stack<int> stack = new Stack<int>();
            bool[] visit = new bool[G.VertexCount];
            foreach (var item in fans)
            {
                if (stop[item] == false)
                {
                    stack.Push(item);
                    visit[item] = true;
                    while (stack.Count > 0)
                    {
                        int k = stack.Pop();
                        foreach (int u in G.OutNeighbors(k))
                        {
                            if (stop[u] == false && visit[u] == false)
                            {
                                stack.Push(u);
                                visit[u] = true;
                                if (czy[u] == true)
                                    return false;
                            }
                        }
                    }
                }
            }
            return true;
        }
        public List<int> FindSeparatingSet(Graph G, List<int> fanclubs, int[] cost, int maxBudget)
        {
            List<int> minim = new List<int>();
            List<int> Alll = new List<int>();
            int minimum = int.MaxValue;
            bool[] visited = new bool[G.VertexCount];
            bool[] used = new bool[G.VertexCount];
            bool[] fans = new bool[G.VertexCount];
            bool[] endArr = new bool[G.VertexCount];
            foreach (int i in fanclubs)
                fans[i] = true;
            Stack<int> st = new Stack<int>();
            bool[] tmp = new bool[G.VertexCount];
            List<int> diffs = new List<int>();
            List<int>[] ff = new List<int>[fanclubs.Count];
            List<int> Comps = new List<int>();
            int count = 0;
            for (int i = 0; i < G.VertexCount; i++)
            {
                Comps.Add(i);
            }
            //for (int i = 0; i < fanclubs.Count; i++)
            //{
            //    if (tmp[fanclubs[i]] == false)
            //    {
            //        ff[count] = new List<int> { fanclubs[i] };
            //        Comps[count] = new List<int> { fanclubs[i] };
            //        tmp[fanclubs[i]] = true;
            //        st.Push(fanclubs[i]);
            //        while (st.Count > 0)
            //        {
            //            int k = st.Pop();
            //            foreach (int u in G.OutNeighbors(k))
            //            {
            //                if (tmp[u] == false)
            //                {
            //                    tmp[u] = true;
            //                    st.Push(u);
            //                    Comps[count].Add(u);
            //                    if (fans[u] == true)
            //                    {
            //                        ff[count].Add(u);
            //                    }
            //                }
            //            }
            //        }
            //        count++;
            //    }
            //}
            minim = new List<int>();
            minimum = int.MaxValue;
            Comps.Sort((a, b) => cost[a].CompareTo(cost[b]));
            Rec(used, 0, true, 0);
            foreach (int item in minim)
            {
                endArr[item] = true;
            }
            void Rec(bool[] arr, int actCost, bool check, int act)
            {
                if (act >= Comps.Count)
                    return;
                if (/*check == true &&*/actCost + cost[Comps[act]] < minimum)
                {
                    arr[Comps[act]] = true;
                    if (isCon(G, arr, fans, fanclubs) == true)
                    {
                        minimum = actCost + cost[Comps[act]];
                        minim = new List<int>();
                        for (int i = 0; i < arr.Length; i++)
                        {
                            if (arr[i])
                                minim.Add(i);
                        }
                    }
                    else
                    {
                        Rec(arr, actCost + cost[Comps[act]], true, act + 1);
                    }

                    arr[Comps[act]] = false;
                }
                //if (actCost + cost[Compos[act]] <= maxBudget)
                //{

                //    //foreach (int u in G.OutNeighbors(curr))
                //    //{
                //    //    if (visited[u] == false)
                //    //    {
                //    //        visited[u] = true;
                //    //        arr[curr] = true;
                //    //        Rec(arr, actCost + cost[curr], u, fan, true);
                //    //        arr[curr] = false;
                //    //        Rec(arr, actCost, u, fan, false);
                //    //        visited[u] = false;
                //    //    }
                //    //}
                //}
                Rec(arr, actCost, false, act + 1);
            }

            for (int i = 0; i < endArr.Length; i++)
            {
                if (endArr[i])
                    Alll.Add(i);
            }

            //int[] costs = { 10, 6, 4, 3 };
            //int[] fanclubss = { 0, 1, 2, 3 };
            //Graph Gs = new Graph(4);
            //for(int i = 0;i<Gs.VertexCount;i++)
            //{
            //    for (int j = i + 1; j < Gs.VertexCount; j++)
            //        Gs.AddEdge(i, j);
            //}
            //var kkk = FindConnectedSeparatingSet(Gs, fanclubss.ToList(), costs, 10000);


            return Alll;
        }



        /// <summary>
        /// Wariant 2: Znajdź najtańszy spójny zbiór wierzchołków grafu G 
        /// rozdzielający wszystkie pary wierzchołków z listy fanclubs 
        /// </summary>
        /// <param name="G">Graf prosty</param>
        /// <param name="fanclubs">Lista wierzchołków, które należy rozdzielić</param>
        /// <param name="cost">cost[v] to koszt użycia wierzchołka v; koszty są nieujemne</param>
        /// <param name="maxBudget">Górne ograniczenie na koszt rozwiązania</param>
        /// <returns></returns>
        public List<int> FindConnectedSeparatingSet(Graph G, List<int> fanclubs, int[] cost, int maxBudget)
        {
            bool[] fans = new bool[G.VertexCount];
            foreach (int i in fanclubs)
                fans[i] = true;
            int minimum = int.MaxValue;
            List<int> minim = new List<int>();
            bool[] allVertecies = new bool[G.VertexCount];
            for(int i = 0; i < G.VertexCount; i++)
            {
                allVertecies[i] = true;
            }
            //int count = 0;
            Rec(new bool[G.VertexCount], 0, new List<int>(), allVertecies, new bool[G.VertexCount], 0);

            void Rec(bool[] arr, int actCost, List<int> actList, bool[] available, bool[] actArr, int allSize)
            {
                if (allSize >= G.VertexCount || actCost > maxBudget) return;
                //count++;
                //foreach (int i in actList)
                //{
                //    Console.Write(i + " ");
                //}
                //Console.WriteLine();
                if (actCost < minimum && isCon(G, actArr, fans, fanclubs))
                {
                    minimum = actCost;
                    minim = new List<int>(actList);
                }
                else if(actCost < minimum)
                {
                    for(int u = 0; u < G.VertexCount ; u++)
                    {
                        if (available[u] == false) continue;
                        if (actArr[u] || arr[u] || actCost + cost[u] > maxBudget)
                        {
                            available[u] = false;
                            continue;
                        }
                        bool[] nextAvailable = new bool[G.VertexCount];
                        if (allSize == 0)
                        {
                            foreach(int v in G.OutNeighbors(u))
                            {
                                nextAvailable[v] = true;
                            }
                        }
                        else
                        {
                            foreach (int v in G.OutNeighbors(u))
                            {
                                nextAvailable[v] = true;
                            }
                            for (int v = 0; v < G.VertexCount ; v++)
                            {
                                nextAvailable[v] |= available[v];
                            }
                            nextAvailable[u] = false;
                        }
                        actArr[u] = true;
                        actList.Add(u);
                        Rec(arr, actCost + cost[u], actList, nextAvailable, actArr, allSize + 1);
                        actArr[u] = false;
                        actList.Remove(u);
                        arr[u] = true;
                    }
                    for (int u = 0; u < G.VertexCount; u++)
                    {
                        if (available[u])
                            arr[u] = false;
                    }
                }

            }

            return minim;
        }

    }

}