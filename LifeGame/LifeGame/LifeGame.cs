using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace LifeGame
{
    class LifeGame
    {
        private int[][,] mCells { get; set; }
        private int mRows { get { return mCells[0].GetLength(0); } }
        private int mCols { get { return mCells[0].GetLength(1); } }
        private int mW { get; set; }
        private int mH { get; set; }
        public int mAge { get; private set; }
        private int mFront { get { return mAge % 2; } }
        private int mBarck { get { return mFront ^ 1; } }
        private bool mAround { get; set; }
        private double mRate { get; set; }
        private int mSeed { get; set; }

        public LifeGame(int rows, int cols, int h, int w, bool around)
        {
            mCells = new int[2][,];
            mCells[0] = new int[rows, cols];
            mCells[1] = new int[rows, cols];
            mW = w;
            mH = h;
            mAround = around;
            Init(0);
        }

        public void Init(double rate)
        {
            mSeed = Environment.TickCount;
            mRate = rate;
            Restart();
        }

        public void Restart()
        {
            Random rnd = new Random(mSeed);
            mAge = 0;
            Array.Clear(mCells[0], 0, mCells[0].Length);
            Array.Clear(mCells[1], 0, mCells[1].Length);
            for (int r = 0; r < mRows; r++)
            {
                for (int c = 0; c < mCols; c++)
                {
                    if (rnd.Next(10000) < (mRate * 10000))
                    {
                        _LiveCell(r, c);
                    }
                }
            }
            mAge++;
        }

        public void Draw(Graphics g)
        {
            g.FillRectangle(Brushes.Black, 0, 0, mCols * mW, mRows * mH);
            for (int r = 0; r < mRows; r++)
            {
                for (int c = 0; c < mCols; c++)
                {
                    int l = _GetCell(r, c);
                    if (l <= 0)
                    {
                        continue;
                    }
                    l--;
                    int len = 5;
                    float alpha = (float)(l % len) / (float)len;
                    int inc = _Lerp(0, 255, alpha);
                    int dec = _Lerp(255, 0, alpha);

                    SolidBrush sb = null;
                    if (l < len * 1)      { sb = new SolidBrush(Color.FromArgb(dec, dec, 255)); }   // 白→青
                    else if (l < len * 2) { sb = new SolidBrush(Color.FromArgb(  0, inc, 255)); }   // 青→水
                    else if (l < len * 3) { sb = new SolidBrush(Color.FromArgb(  0, 255, dec)); }   // 水→緑
                    else if (l < len * 4) { sb = new SolidBrush(Color.FromArgb(inc, 255,   0)); }   // 緑→黄
                    else if (l < len * 5) { sb = new SolidBrush(Color.FromArgb(255, dec,   0)); }   // 黄→赤
                    else                  { sb = new SolidBrush(Color.FromArgb(255,   0,   0)); }   // 赤
                    if (sb != null)
                    {
                        g.FillRectangle(sb, c * mW, r * mH, mW - 1, mH - 1);
                    }
                }
            }
        }

        public void Step()
        {
            int r;
            for (r = 0; r < mRows; r++)
            {
                int c;

                for (c = 0; c < mCols; c++)
                {
                    int live = _GetCellLapLiveNum(r, c);

                    if (_IsLiveCell(r, c))
                    {
                        if (live <= 1 || live >= 4)
                        {
                            /* 過疎：生きているセルに隣接する生きたセルが1つ以下ならば、過疎により死滅する。 */
                            /* 過密：生きているセルに隣接する生きたセルが4つ以上ならば、過密により死滅する。 */
                            _DeadCell(r, c);
                        }
                        else
                        {
                            /* 生存：生きているセルに隣接する生きたセルが2つか3つならば、次の世代でも生存する。 */
                            _LiveCell(r, c);
                        }
                    }
                    else
                    {
                        _DeadCell(r, c);
                        /* 誕生：死んでいるセルに隣接する生きたセルがちょうど3つあれば、次の世代が誕生する。 */
                        if (live == 3)
                        {
                            _LiveCell(r, c);
                        }
                    }
                }
            }
            mAge++;
        }

        private static int _Lerp(int a, int b, float alpha)
        {
            return (int)(a + (b - a) * alpha);
        }

        private static int _Roll(int n, int max)
        {
            return (((n) + (max) * 100) % (max));	/* nを0以上～m未満の範囲に丸める */
        }

        private void _DeadCell(int row, int col)
        {
            mCells[mBarck][row, col] = 0;
        }

        private void _LiveCell(int row, int col)
        {
            mCells[mBarck][row, col] = mCells[mFront][row, col] + 1;
        }

        private int _GetCell(int row, int col)
        {
            return mCells[mFront][row, col];
        }

        private bool _IsLiveCell(int row, int col)
        {
            return (_GetCell(row, col) > 0);
        }

        private int _GetCellLapLiveNum(int row, int col)
        {
            int num = 0;

            if (row < 0 || row >= mRows || col < 0 || col >= mCols)
            {
                return num;
            }

            for (int ofsr = -1; ofsr <= 1; ofsr++)
            {
                int r = row + ofsr;
                int ofsc;

                if (r < 0 || r >= mRows)
                {
                    /* 範囲外 */
                    if (mAround) { r = _Roll(r, mRows); }  /* 回り込み */
                    else { continue; }                  /* 死とみなす */
                }
                for (ofsc = -1; ofsc <= 1; ofsc++)
                {
                    if (ofsr != 0 || ofsc != 0)
                    {
                        int c = col + ofsc;

                        if (c < 0 || c >= mCols)
                        {
                            /* 範囲外 */
                            if (mAround) { c = _Roll(c, mCols); }  /* 回り込み */
                            else { continue; }                  /* 死とみなす */
                        }
                        num += _IsLiveCell(r, c) ? 1 : 0;
                    }
                }
            }
            return num;
        }
    }
}
