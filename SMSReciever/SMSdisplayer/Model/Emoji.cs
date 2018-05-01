using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace SMSdisplayer.Model
{
    public class Emoji : UIElement
    {
        protected String chara;
        protected int index;
        protected BitmapImage emojiImage;
        private Rect bBox;
        //protected Image image;

        public BitmapImage EmojiImage { get => emojiImage;}
        public string Chara { get => chara;  }
        public int Index { get => index; }
        public Rect BBox { get => bBox; }

        public Emoji(String chara, int index, String imageFile)
        {
            this.chara = chara;
            this.index = index;

            emojiImage = new BitmapImage(new Uri(imageFile, UriKind.Relative));
            bBox = new Rect();
        }

        public void SetBoundingBox(Rect bBox)
        {
            this.bBox = bBox;
            this.bBox.Width = bBox.Height * 0.9;
            this.bBox.Height = bBox.Height * 0.9;
        }
    }
}
