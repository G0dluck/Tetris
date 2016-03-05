using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    struct ElementArray
    {
        public bool Status { get; set; }
        public System.Drawing.Brush Brush { get { return brush; } set { brush = value; } }
        private System.Drawing.Brush brush;
    }
}
