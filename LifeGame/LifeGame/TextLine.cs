using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace LifeGame
{
    class TextLine
    {
        private Font mFont;
        private int mInfoX;
        private int mInfoY;

        public TextLine(int x, int y)
        {
            mFont = new Font("MS UI Gothic", 20);
            mInfoX = x;
            mInfoY = y;
        }

        public void Draw(Graphics g, int line, string text)
        {
            TextRenderer.DrawText(g, text, mFont, new Point(mInfoX, mInfoY + line * 30), Color.Black);
        }
    }
}
