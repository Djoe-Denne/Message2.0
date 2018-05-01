using SMSdisplayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SMSdisplayer
{
    public class ImageCanvas : Canvas {
       protected override void OnRender(DrawingContext dc)
        {
            foreach (Emoji emoji in Children)
            {
                BitmapImage img = emoji.EmojiImage;
                dc.DrawImage(img, emoji.BBox);
            }
        }
    }
}
