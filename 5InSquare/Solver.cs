using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;


namespace _5InSquare
{
    static class Solver
    {
        private static EventWaitHandle nextSolEvent = new AutoResetEvent(false);
        private static bool shutdownEvent = false;
        public static int DelayTime = 0;
        const int SIZE = 5;
        public delegate void PlaceAct(int x, int y, int num, int style);
        public delegate void PausedAct();
        /// <summary>
        /// x, y, num
        /// </summary>\
        public static event PlaceAct whenPlaced;
        public static event PausedAct whenPaused;
        public enum pieceOptions { Hor, Ver, HorInv, VerInv }
        static List<pieceOptions> options = new pieceOptions[]{
            pieceOptions.Hor, 
            pieceOptions.Ver, 
            pieceOptions.HorInv, 
            pieceOptions.VerInv
        }.ToList();
        static Solver()
        {
            pieces = new List<List<int>>();
            pieces.Add(new int[] { 1, 2 }.ToList());
            pieces.Add(new int[] { 3, 4, 5 }.ToList());
            pieces.Add(new int[] { 4, 2, 1 }.ToList());
            pieces.Add(new int[] { 3, 5, 2 }.ToList());
            pieces.Add(new int[] { 1, 5, 2 }.ToList());
            pieces.Add(new int[] { 2, 3, 4 }.ToList());
            pieces.Add(new int[] { 4, 5, 3 }.ToList());
            pieces.Add(new int[] { 3, 1, 5 }.ToList());
            pieces.Add(new int[] { 1, 4 }.ToList());
            pieceUsed = new Dictionary<List<int>, bool>();
            foreach (var item in pieces)
            {
                pieceUsed.Add(item, false);
            }
            reset();
            sol.IsBackground = true;
            sol.Start();
        }
        public static slot[,] board = new slot[SIZE, SIZE];
        static List<List<int>> pieces;
        static Dictionary<List<int>, bool> pieceUsed;


        public static bool Check()
        {
            if (thS)
            {
                if (whenPaused != null) whenPaused(); 
                nextSolEvent.WaitOne();
                return false;
            }
            return true;
        }
        static bool place(List<int> piece, int x, int y, pieceOptions flag, int pieceNum)
        {
            bool retVal = false;
            List<int> invList = null;
            if (flag == pieceOptions.HorInv || flag == pieceOptions.VerInv)
            {
                invList = new List<int>();
                for (int i = piece.Count - 1; i >= 0; i--)
                {
                    invList.Add(piece[i]);
                }
            }
            switch (flag)
            {
                case pieceOptions.Hor:
                    retVal = horPlace(piece, x, y);
                    break;
                case pieceOptions.Ver:
                    retVal = verPlace(piece, x, y);
                    break;
                case pieceOptions.HorInv:
                    retVal = horPlace(invList, x, y);
                    break;
                case pieceOptions.VerInv:
                    retVal = verPlace(invList, x, y);
                    break;
                default:
                    break;
            }
            if (retVal)
            {
                for (int i = 0; i < piece.Count; i++)
                {
                    if (flag == pieceOptions.Hor || flag == pieceOptions.HorInv)
                    {
                        board[x + i, y].Style = pieceNum;
                    }
                    if (flag == pieceOptions.Ver || flag == pieceOptions.VerInv)
                    {
                        board[x, y + i].Style = pieceNum;
                    }
                }
                pieceUsed[piece] = true;


                if (DelayTime != 0 && !shutdownEvent && whenPlaced != null)
                    System.Threading.Thread.Sleep(DelayTime);
                if (whenPlaced != null)
                {
                    for (int i = 0; i < piece.Count; i++)
                    {
                        if (flag == pieceOptions.Hor || flag == pieceOptions.HorInv)
                        {
                            try { whenPlaced(x + i, y, board[x + i, y].Num, pieceNum); }
                            catch { }
                        }
                        if (flag == pieceOptions.Ver || flag == pieceOptions.VerInv)
                        {
                            try { whenPlaced(x, y + i, board[x, y + i].Num, pieceNum); }
                            catch { }
                        }
                    }
                }
            }
            return retVal;
        }
        static void remove(List<int> piece, int x, int y, pieceOptions flag)
        {
            switch (flag)
            {
                case pieceOptions.Hor:
                    horRemove(piece, x, y);
                    break;
                case pieceOptions.Ver:
                    verRemove(piece, x, y);
                    break;
                case pieceOptions.HorInv:
                    horRemove(piece, x, y);
                    break;
                case pieceOptions.VerInv:
                    verRemove(piece, x, y);
                    break;
                default:
                    break;
            }
            pieceUsed[piece] = false;

            for (int i = 0; i < piece.Count; i++)
            {
                if (flag == pieceOptions.Hor || flag == pieceOptions.HorInv)
                {
                    board[x + i, y].Style = -1;
                }
                if (flag == pieceOptions.Ver || flag == pieceOptions.VerInv)
                {
                    board[x, y + i].Style = -1;
                }
            }

            if (DelayTime != 0 && !shutdownEvent && whenPlaced != null)
                System.Threading.Thread.Sleep(DelayTime);
            if (whenPlaced != null)
            {
                for (int i = 0; i < piece.Count; i++)
                {
                    if (flag == pieceOptions.Hor || flag == pieceOptions.HorInv)
                    {
                        try { whenPlaced(x + i, y, board[x + i, y].Num, -1); }
                        catch { }
                    }
                    if (flag == pieceOptions.Ver || flag == pieceOptions.VerInv)
                    {
                        try { whenPlaced(x, y + i, board[x, y + i].Num, -1); }
                        catch { }
                    }
                }
            }

        }

        static bool horPlace(List<int> piece, int x, int y)
        {
            int size = piece.Count;
            if (x + size > SIZE) return false;
            for (int i = 0; i < size; i++)
            {
                if (board[x + i, y].Num != 0 || board[x + i, y].Taboo[piece[i]] != 0)
                    return false;
            }
            //place
            for (int i = 0; i < size; i++)
            {
                board[x + i, y].Num = piece[i];
                for (int j = 0; j < SIZE; j++)
                {
                    board[x + i, j].Taboo[piece[i]]++;
                    board[j, y].Taboo[piece[i]]++;
                }
            }
            return true;
        }
        static bool verPlace(List<int> piece, int x, int y)
        {
            int size = piece.Count;
            if (y + size > SIZE) return false;
            for (int i = 0; i < size; i++)
            {
                if (board[x, y + i].Num != 0 || board[x, y + i].Taboo[piece[i]] != 0)
                    return false;
            }
            //place
            for (int i = 0; i < size; i++)
            {
                board[x, y + i].Num = piece[i];
                for (int j = 0; j < SIZE; j++)
                {
                    board[x, j].Taboo[piece[i]]++;
                    board[j, y + i].Taboo[piece[i]]++;
                }
            }
            return true;
        }
        static void reset()
        {
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = 0; j < SIZE; j++)
                {
                    board[i, j] = new slot();
                }
            }
            foreach (var item in pieces)
            {
                pieceUsed[item] = false;
            }

                


        }
        public static void Solve()
        {
            thS = false;
            reset();
            recurSolve(0, 0);
        }
        public static void Reset()
        {
            shutdownEvent = true;
            if (pieceUsed.Any(p => p.Value))
            {
                nextSolEvent.Set();
                nextSolEvent.Reset();
            }
            reset();
        }
        static bool thS = false;
        static Thread sol = new Thread(new ThreadStart(delegate { 
            while (true)
            {
                nextSolEvent.WaitOne();
                shutdownEvent = false;
                if (!recurSolve(0, 0)) 
                { 
                    reset(); 
                    if (whenPaused != null) whenPaused(); 
                }
            } 
        }));
        public static void NextSolve()
        {
            thS = true;
            nextSolEvent.Set();
            nextSolEvent.Reset();
        }
        public class slot
        {
            public int Num { get; set; }
            public int[] Taboo { get; private set; }
            public int Style { get; set; }
            public slot()
            {
                Num = 0;
                Taboo = new int[SIZE+1];
                for (int i = 0; i < SIZE+1; i++)
                {
                    Taboo[i] = 0;
                }
                Style = -1;
            }
            public override string ToString()
            {
                return Num.ToString();
            }
        }
        static bool recurSolve(int x, int y)
        {
            int a = x, b = y + 1;
            if (b >= SIZE)
            {
                b = 0;
                a++;
            }
            if (a >= SIZE)
                return Check();
            if (board[x, y].Num != 0)
                return recurSolve(a, b);
            for (int i = 0; i < pieces.Count; i++)
            {
                if (pieceUsed[pieces[i]]) continue;
                for (int j = 0; j < options.Count; j++)
                {
                    if (shutdownEvent) return false;
                    if (place(pieces[i], x, y, options[j], i))
                    {
                        if (recurSolve(a, b)) return true;
                        remove(pieces[i], x, y, options[j]);
                    }
                }
            }
            return false;
        }
        static void horRemove(List<int> piece, int x, int y)
        {
            int size = piece.Count;
            for (int i = 0; i < size; i++)
            {
                int num = board[x + i, y].Num;
                board[x + i, y].Num = 0;
                for (int j = 0; j < SIZE; j++)
                {
                    board[x + i, j].Taboo[num]--;
                    board[j, y].Taboo[num]--;
                }
            }
        }
        static void verRemove(List<int> piece, int x, int y)
        {
            int size = piece.Count;
            for (int i = 0; i < size; i++)
            {
                int num = board[x, y + i].Num;
                board[x, y + i].Num = 0;
                for (int j = 0; j < SIZE; j++)
                {
                    board[x, j].Taboo[num]--;
                    board[j, y + i].Taboo[num]--;
                }
            }
        }
    }
    
}
