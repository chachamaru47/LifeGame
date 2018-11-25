using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LifeGame
{
    public partial class Form1 : Form
    {
        enum Action
        {
            None,
            Init,
            Restart,
            Step,
        }

        private static readonly int MaxSpeed = 10;
        private static readonly double StartLiveRate = 0.5;

        private System.Timers.Timer mTimer;
        private LifeGame mGame;
        private Action mAction;
        private int mCount;
        private bool mPause;
        private int __speed;
        private int mSpeed
        {
            get { return __speed; }
            set { if (value >= 0 && value <= MaxSpeed) { __speed = value; } }
        }
        private TextLine mText;

        public Form1()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ClientSize = new Size(1280, 960);
            Location = new Point(0, 0);
            Text = "Game of Life";
            DoubleBuffered = true;

            const int rows = 50;
            const int cols = 50;
            const int h = 18;
            const int w = 18;
            mGame = new LifeGame(rows, cols, h, w, false);

            mGame.Init(StartLiveRate);
            mAction = Action.None;
            mCount = 0;
            mPause = false;
            mSpeed = MaxSpeed / 2;

            mText = new TextLine(cols * w, 0);

            mTimer = new System.Timers.Timer();
            mTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnElapsed_TimersTimer);
            mTimer.Interval = 50;
            mTimer.Start();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            mGame.Draw(e.Graphics);
            _DrawInfo(e.Graphics);
        }

        private void _DrawInfo(Graphics g)
        {
            mText.Draw(g, 0, "第" + mGame.mAge + "世代");
            mText.Draw(g, 1, "スピード：" + mSpeed);
            if (mPause)
            {
                mText.Draw(g, 2, "ポーズ中");
            }
            mText.Draw(g, 4, "操作");
            mText.Draw(g, 5, "Space：ポーズ");
            mText.Draw(g, 6, "↑：スピードアップ");
            mText.Draw(g, 7, "↓：スピードダウン");
            mText.Draw(g, 8, "→：１世代進める");
            mText.Draw(g, 9, "←：第１世代に戻す");
            mText.Draw(g, 10, "R：リセット");
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            switch (e.KeyCode)
            {
                case Keys.Space:
                    mPause = !mPause;
                    break;
                case Keys.Up:
                    mSpeed++;
                    break;
                case Keys.Down:
                    mSpeed--;
                    break;
                case Keys.Right:
                    mAction = Action.Step;
                    break;
                case Keys.Left:
                    mAction = Action.Restart;
                    break;
                case Keys.R:
                    mAction = Action.Init;
                    break;
                default:
                    break;
            }
        }

        private void OnElapsed_TimersTimer(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock(this)
            {
                _TickAction();
                _TickStep();
                Invalidate();
            }
        }

        private void _TickStep()
        {
            if (!mPause && mSpeed > 0)
            {
                mCount += (int)(Math.Pow(2, mSpeed) / 2 * mTimer.Interval);
                for (; mCount >= 2000; mCount -= 2000)
                {
                    mGame.Step();
                }
            }
        }

        private void _TickAction()
        {
            switch (mAction)
            {
                case Action.Init:
                    mGame.Init(StartLiveRate);
                    break;
                case Action.Restart:
                    mGame.Restart();
                    break;
                case Action.Step:
                    mGame.Step();
                    break;
                case Action.None:
                default:
                    break;
            }
            mAction = Action.None;
        }
    }
}
