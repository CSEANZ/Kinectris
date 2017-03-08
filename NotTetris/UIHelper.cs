using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NotTetris
{
    public class UiHelper
    {
        public static Label MakeLabel(string Content, Brush Background, Brush Foreground, FontWeight Fontweight, int Fontsize)
        {
            Label label = new Label();
            label.Content = Content;
            label.Background = Background;
            label.Foreground = Foreground;
            label.FontWeight = Fontweight;
            label.FontSize = Fontsize;

            return label; 
        }
    }
}
