using System;
using System.Drawing.Printing;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace FRAUD_UI_ANALIZATOR.SCRIPTS
{
    public class Animations
    {
        public static void ShakeAnimation(double from, Image image)
        {
            DoubleAnimation doubleShake1 = new()
            {
                Duration = TimeSpan.FromSeconds(.5)
            };
            DoubleAnimation doubleShake2 = new()
            {
                Duration = TimeSpan.FromSeconds(.5)
            };
        }
    }
}