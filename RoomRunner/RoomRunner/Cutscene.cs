using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoomRunner
{
    public class Cutscene
    {
        public int Frame;
        public Cutscene(CutsceneIds id)
        {
            Frame = 0;
        }

        public void Update()
        {

        }
        public void Start()
        {

        }
        public void Cancel()
        {

        }
    }
    public enum CutsceneIds
    {

    }
}
