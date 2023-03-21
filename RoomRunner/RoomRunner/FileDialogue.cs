using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace RoomRunner
{
    class FileDialogue
    {
        public bool done;
        public FileDialogue()
        {
            done = false;
        }
        public String Show()
        {
            done = false;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select Music";
            ofd.Filter = "WAV|*.wav";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                done = true;
                return ofd.FileName;
            }
            done = true;
            return "";
            
        }
        
    }
}
