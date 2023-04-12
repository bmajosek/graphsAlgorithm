using ASD;
using ASD.Graphs;
using System;
using System.Collections.Generic;

namespace Lab06
{
    public class HeroesSolver : MarshalByRefObject
    {
        /// <summary>
        /// Etap 1 - stwierdzenie, czy rozwiązanie istnieje
        /// </summary>
        /// <param name="g">graf przedstawiający mapę</param>
        /// <param name="keymasterTents">tablica krotek zawierająca pozycje namiotów klucznika - pierwsza liczba to kolor klucznika, druga to numer skrzyżowania</param>
        /// <param name="borderGates">tablica krotek zawierająca pozycje bram granicznych - pierwsza liczba to kolor bramy, dwie pozostałe to numery skrzyżowań na drodze między którymi znajduje się brama</param>
        /// <param name="p">ilość występujących kolorów (występujące kolory to 1,2,...,p)</param>
        /// <returns>bool - wartość true jeśli rozwiązanie istnieje i false wpp.</returns>
        public bool Lab06Stage1(Graph<int> g, (int color, int city)[] keymasterTents, (int color, int cityA, int cityB)[] borderGates, int p)
        {
            int n = g.VertexCount - 1; // wierzchołek 0 nie występuje w zadaniu
            int curr = 1, lvl;
            int pp = (int)Math.Pow(2, p);
            int[] keys = new int[n + 1];
            int[,] gates = new int[n + 1, n + 1];
            foreach ((int color, int city) in keymasterTents)
            {
                keys[city] |= (1 << (color - 1));
            }
            foreach ((int color, int cityA, int cityB) in borderGates)
            {
                gates[cityA, cityB] |= (1 << (color - 1));
                gates[cityB, cityA] = gates[cityA, cityB];
            }
            bool[,] poss = new bool[n + 1, pp];
            Queue<(int, int)> q = new Queue<(int, int)>();
            q.Enqueue((1, keys[1]));
            poss[1, keys[1]] = true;
            if (n == 1)
                return true;
            while (q.Count > 0)
            {
                (curr, lvl) = q.Dequeue();
                foreach (Edge<int> e in g.OutEdges(curr))
                {
                    if ((lvl & gates[curr, e.To]) != gates[curr, e.To])
                    {
                        continue;
                    }
                    if (poss[e.To, lvl | keys[e.To]] == false)
                    {
                        poss[e.To, lvl | keys[e.To]] = true;
                        if (e.To != n)
                            q.Enqueue((e.To, lvl | keys[e.To]));
                        else
                            return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Etap 2 - stwierdzenie, czy rozwiązanie istnieje
        /// </summary>
        /// <param name="g">graf przedstawiający mapę</param>
        /// <param name="keymasterTents">tablica krotek zawierająca pozycje namiotów klucznika - pierwsza liczba to kolor klucznika, druga to numer skrzyżowania</param>
        /// <param name="borderGates">tablica krotek zawierająca pozycje bram granicznych - pierwsza liczba to kolor bramy, dwie pozostałe to numery skrzyżowań na drodze między którymi znajduje się brama</param>
        /// <param name="p">ilość występujących kolorów (występujące kolory to 1,2,...,p)</param>
        /// <returns>krotka (bool solutionExists, int solutionLength) - solutionExists ma wartość true jeśli rozwiązanie istnieje i false wpp. SolutionLenth zawiera długość optymalnej trasy ze skrzyżowania 1 do n</returns>
        public (bool solutionExists, int solutionLength) Lab06Stage2(Graph<int> g, (int color, int city)[] keymasterTents, (int color, int cityA, int cityB)[] borderGates, int p)
        {
            int n = g.VertexCount - 1; // wierzchołek 0 nie występuje w zadaniu
            int curr = 1, lvl;
            int pp = (int)Math.Pow(2, p);
            int[] keys = new int[n + 1];
            int[,] gates = new int[n + 1, n + 1];
            foreach ((int color, int city) in keymasterTents)
            {
                keys[city] |= (1 << (color - 1));
            }
            foreach ((int color, int cityA, int cityB) in borderGates)
            {
                gates[cityA, cityB] |= (1 << (color - 1));
                gates[cityB, cityA] = gates[cityA, cityB];
            }
            bool[,] poss = new bool[n + 1, pp];
            int[,] dist = new int[n + 1, pp];
            SafePriorityQueue<int, (int, int)> q = new SafePriorityQueue<int, (int, int)>();
            bool[,] isInQueue = new bool[n + 1, pp];
            q.Insert((1, keys[1]), 0);
            dist[1, keys[1]] = 0;
            poss[1, keys[1]] = true;
            int mini = int.MaxValue;
            if (n == 1)
                return (true, 0);
            while (q.Count > 0)
            {
                (curr, lvl) = q.Extract();
                if(curr == n && dist[curr, lvl] < mini)
                { 
                    mini = dist[curr,lvl]; 
                }
                isInQueue[curr, lvl] = false;
                foreach (Edge<int> e in g.OutEdges(curr))
                {
                    if ((lvl & gates[curr, e.To]) != gates[curr, e.To])
                    {
                        continue;
                    }
                    if (poss[e.To, lvl | keys[e.To]] == false || dist[e.To, lvl | keys[e.To]] > dist[curr, lvl] + e.Weight)
                    {
                        poss[e.To, lvl | keys[e.To]] = true;
                        dist[e.To, lvl | keys[e.To]] = dist[curr, lvl] + e.Weight;
                        if (isInQueue[e.To, lvl | keys[e.To]] == false)
                        {
                            q.Insert((e.To, lvl | keys[e.To]), dist[e.To, lvl | keys[e.To]]);
                            isInQueue[e.To, lvl | keys[e.To]] = true;
                        }
                        else
                            q.UpdatePriority((e.To, lvl | keys[e.To]), dist[e.To, lvl | keys[e.To]]);
                    }
                }
            }
            return mini == int.MaxValue ? (false, 0) : (true, mini);
        }
    }
}