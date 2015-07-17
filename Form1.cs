using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace tx_watermark
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private int iNumberOfPages = 1;

        private void insertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertWatermarkImages();
        }

        private void InsertWatermarkImages()
        {
            // remove all existing watermarks
            RemoveWatermarkImages(textControl1);

            foreach (TXTextControl.Page page in textControl1.GetPages())
            {
                // create a new watermark image
                Bitmap bmp = CreateDraftImage();

                // create a new TXTextControl.Image object and mark
                // as watermark
                TXTextControl.Image img = new TXTextControl.Image(bmp);
                img.Name = "Watermark";
                img.Sizeable = false;
                img.Moveable = false;

                // calculate the location to center the image
                Point pImageLocation = new Point(
                    (page.Bounds.Width / 2) - (PixelToTwips(bmp.Size.Width)) / 2,
                    (page.Bounds.Height / 2) - (PixelToTwips(bmp.Size.Height)) / 2);

                // add the image to the page
                textControl1.Images.Add(
                    img,
                    page.Number,
                    pImageLocation,
                    TXTextControl.ImageInsertionMode.BelowTheText);
            }
        }

        // creates a new "DRAFT" sample image
        private Bitmap CreateDraftImage()
        {
            string sText = "DRAFT";

            Bitmap destination = new Bitmap(400, 400);
            using (Graphics g = Graphics.FromImage(destination))
            {
                GraphicsUnit units = GraphicsUnit.Pixel;

                g.Clear(Color.White);
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;

                g.DrawString(sText, new Font(
                    "Arial", 
                    90, 
                    FontStyle.Bold,
                    GraphicsUnit.Pixel), 
                    new SolidBrush(
                        Color.FromArgb(64, Color.Black)),
                        destination.GetBounds(ref units),
                        stringFormat);
            }

            return destination;
        }

        // converts pixels to twips
        static int PixelToTwips(int pixel)
        {
            float DpiX = (float)(1440 / Graphics.FromHwnd(new IntPtr()).DpiX);
            return (int)(pixel * DpiX);
        }

        private void RemoveWatermarkImages(TXTextControl.TextControl textControl)
        {
            // List of images to be deleted
            List<TXTextControl.Image> lImagesToRemove = 
                new List<TXTextControl.Image>();

            // loop through all images and check for the Name
            foreach (TXTextControl.Image img in textControl1.Images)
            {
                // add watermark images to the List
                if (img.Name == "Watermark")
                    lImagesToRemove.Add(img);
            }

            // remove all tagged watermark images
            foreach(TXTextControl.Image img in lImagesToRemove)
            {
                textControl1.Images.Remove(img);
            }
        }

        // set the EditMode to read only when a watermark is selected
        // to avoid the deletion
        private void textControl1_ImageSelected(object sender, TXTextControl.ImageEventArgs e)
        {
            if (e.Image.Name == "Watermark")
                textControl1.EditMode = TXTextControl.EditMode.ReadAndSelect;
        }

        // set back the EditMode 
        private void textControl1_ImageDeselected(object sender, TXTextControl.ImageEventArgs e)
        {
            textControl1.EditMode = TXTextControl.EditMode.Edit;
        }

        // update the watermarks if the number of pages are different
        private void textControl1_Changed(object sender, EventArgs e)
        {
            if (textControl1.Pages > iNumberOfPages)
            {
                InsertWatermarkImages();
                iNumberOfPages = textControl1.Pages;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InsertWatermarkImages();
        }
    }
}
