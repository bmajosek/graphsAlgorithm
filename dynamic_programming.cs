using System;
using System.Collections.Generic;
using System.Text;

namespace Lab02
{
    public class PatternMatching : MarshalByRefObject
    {
        public static void rev(StringBuilder s)
        {
            int i = 0, j = s.Length - 1;
            while(i < j)
            {
                (s[i], s[j]) = (s[j], s[i]);
                ++i;
                --j;
            }
        }
        /// <summary>
        /// Etap 1 - wyznaczenie trasy, zgodnie z którą robot przemieści się z pozycji poczatkowej (0,0) na pozycję docelową (-n-1, m-1)
        /// </summary>
        /// <param name="n">wysokość prostokąta</param>
        /// <param name="m">szerokość prostokąta</param>
        /// <param name="obstacles">tablica ze współrzędnymi przeszkód</param>
        /// <returns>krotka (bool result, string path) - result ma wartość true jeżeli trasa istnieje, false wpp., path to wynikowa trasa</returns>
        public (bool result, string path) Lab02Stage1(int n, int m, (int, int)[] obstacles)
        {
            bool[,] Possible = new bool[n,m];
            int[,] Move = new int[n,m]; // 1 prawo 2 dol 3 przeszkoda
            Possible[0, 0] = true;
            for(int i = 0;i<obstacles.Length;++i)
            {
                Possible[obstacles[i].Item1, obstacles[i].Item2] = false;
                Move[obstacles[i].Item1, obstacles[i].Item2] = 3;
            }
            for(int i = 1;i<n;++i)
            {
                if (Move[i,0] == 3 || Possible[i - 1,0] == false)
                {
                    Possible[i, 0] = false;
                    Move[i, 0] = 3;
                }
                else
                {
                    Possible[i, 0] = true;
                    Move[i, 0] = 2;
                }
            }
            for (int i = 1; i < m; ++i)
            {
                if (Move[0, i] == 3 || Possible[0, i - 1] == false)
                {
                    Possible[0, i] = false;
                    Move[0, i] = 3;
                }
                else
                {
                    Possible[0, i] = true;
                    Move[0, i] = 1;
                }
            }
            for(int i = 1;i<n;++i)
            {
                for(int j = 1;j<m;++j)
                {
                    if ((Possible[i-1,j] == false && Possible[i,j-1] == false) || Move[i, j] == 3)
                    {
                        Possible[i, j] = false;
                        Move[i, j] = 3;
                    }
                    else if (Possible[i - 1,j] == true)
                    {
                        Possible[i, j] = true;
                        Move[i, j] = 2;
                    }
                    else if (Possible[i,j - 1] == true)
                    {
                        Possible[i, j] = true;
                        Move[i, j] = 1;
                    }
                }
            }

            int row = n - 1, col = m - 1;
            StringBuilder road = new StringBuilder();
            bool czyist = false;
            while(row >= 0 || col >= 0)
            {
                if(row < 0 || col < 0)
                {
                    road.Clear();
                    break;
                }
                if (Move[row,col] == 1)
                {
                    road.Append('R');
                    col--;
                }
                else if (Move[row,col] == 2)
                {
                    road.Append('D');
                    row--;
                }
                else if (Move[row,col] == 0)
                {
                    czyist = true;
                    break;
                }
                else
                {
                    road.Clear();
                    break;
                }
            }
            rev(road);
            return (czyist, road.ToString());
        }

        /// <summary>
        /// Etap 2 - wyznaczenie trasy realizującej zadany wzorzec, zgodnie z którą robot przemieści się z pozycji poczatkowej (0,0) na pozycję docelową (-n-1, m-1)
        /// </summary>
        /// <param name="n">wysokość prostokąta</param>
        /// <param name="m">szerokość prostokąta</param>
        /// <param name="pattern">zadany wzorzec</param>
        /// <param name="obstacles">tablica ze współrzędnymi przeszkód</param>
        /// <returns>krotka (bool result, string path) - result ma wartość true jeżeli trasa istnieje, false wpp., path to wynikowa trasa</returns>
        public (bool result, string path) Lab02Stage2(int n, int m, string pattern, (int, int)[] obstacles)
        {
            int k = pattern.Length;
            bool[,,] Possible = new bool[n + 1, m + 1, k];
            int[,,] Move = new int[n + 1, m + 1, k ]; // 1 prawo 2 dol 3 przeszkoda
            bool[,] bools = new bool[n + 1, m + 1];
            for (int i = 0; i < obstacles.Length; ++i)
            {
                Possible[obstacles[i].Item1, obstacles[i].Item2,0] = false;
                Move[obstacles[i].Item1, obstacles[i].Item2, 0] = 3;
            }
            bool czyist = false;
            StringBuilder road = new StringBuilder();
            int col = 0, row = 0;
            Possible[0, 0, 0] = true;
            for (int kk = 0; kk < k; ++kk)
            {
                for (int i = row;i<n;++i)
                {
                    for (int j = col; j < m; ++j)
                    {
                        if (Move[i, j, 0] == 3)
                        {
                            Possible[i, j, kk] = false;
                            continue;
                        }
                        if (kk == 0 && Possible[i, j, kk] == true && bools[i, j] == false)
                        {
                            if (Move[i,j+1,0] != 3 && (pattern[kk] == 'R' || pattern[kk] == '?' || pattern[kk] == '*'))
                            {
                                Possible[i, j + 1, kk] = true;
                                bools[i, j + 1] = true;
                                Move[i, j + 1, kk] = 1;
                            }
                            if (Move[i + 1, j, 0] != 3 && (pattern[kk] == 'D' || pattern[kk] == '?' || pattern[kk] == '*'))
                            {
                                Possible[i + 1, j, kk] = true;
                                bools[i + 1, j] = true;
                                Move[i + 1, j, kk] = 2;
                            }
                        }
                        else if (kk > 0 && Possible[i,j,kk - 1] == true)
                        {
                            if (Move[i, j + 1, 0] != 3 && (pattern[kk] == 'R' || pattern[kk] == '?' || pattern[kk] == '*'))
                            {
                                Possible[i,j+1,kk] = true;
                                Move[i, j + 1, kk] = 1;
                            }
                            if (Move[i + 1, j, 0] != 3 && (pattern[kk] == 'D' || pattern[kk] == '?' || pattern[kk] == '*'))
                            {
                                Possible[i + 1, j, kk] = true;
                                Move[i + 1, j, kk] = 2;
                            }
                        }
                        if(i - 1 >= 0 && Possible[i - 1, j, kk] == true && pattern[kk] == '*')
                        {
                            Possible[i, j, kk] = true;
                            Move[i, j, kk] = 2;
                        }
                        if (j - 1 >= 0 && Possible[i, j - 1, kk] == true && pattern[kk] == '*')
                        {
                            Possible[i, j, kk] = true;
                            Move[i, j, kk] = 1;
                        }
                    }
                }
                if (pattern[kk] == 'D') ++row;
                else if (pattern[kk] == 'R') ++col;
            }
            int r = n - 1, c = m - 1, curr = pattern.Length - 1;
            while((r>=0 || c>=0) && curr>= 0)
            {
                if (Possible[r, c, curr] == false) return (false, "");
                if (pattern[curr] == 'D')
                {
                    road.Append('D');
                    r--;
                    curr--;

                }
                else if (pattern[curr] == 'R')
                {
                    road.Append('R');
                    c--;
                    curr--;
                }
                else if (pattern[curr] == '?')
                {
                    if (Move[r, c, curr] == 1)
                    {
                        road.Append('R');
                        c--;
                        curr--;
                    }
                    else if (Move[r, c, curr] == 2)
                    {
                        road.Append('D');
                        r--;
                        curr--;
                    }
                    else
                    {
                        return (false, "");
                    }
                }
                else
                {
                    if(curr == 0 && Possible[r,c,curr] == true)
                    {
                        while ((r >= 0 && c >= 0))
                        {
                            if (r == 0 && c == 0)
                            {
                                break;
                            }
                            if (Move[r, c, curr] == 1)
                            {
                                road.Append('R');
                                c--;
                            }
                            else if (Move[r, c, curr] == 2)
                            {
                                road.Append('D');
                                r--;
                            }
                        }
                        break;
                    }
                    while ((r>=0 && c>=0 && curr>0 && Possible[r,c,curr - 1] == false) )
                    {
                        if (Move[r,c,curr] == 1)
                        {
                            road.Append('R');
                            c--;
                        }
                        else if (Move[r, c, curr] == 2)
                        {
                            road.Append('D');
                            r--;
                        }
                    }
                    curr--;
                }
            }
            if (r <= 0 && c <= 0 && road.Length == n + m - 2)
            {
                czyist = true;
            }
            else
            {
                czyist = false;
                road.Clear();
            }
            rev(road);
            return (czyist, road.ToString());
        }
    }
}