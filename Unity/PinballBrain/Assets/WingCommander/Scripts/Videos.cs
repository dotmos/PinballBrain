using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Videos {
    public class WC2 {
        public static string Intro {
            get { return Application.streamingAssetsPath + "/WC2/Videos/Intro.mp4"; }
        }

        public static string Death {
            get { return Application.streamingAssetsPath + "/WC2/Videos/Death.mp4"; }
        }

        public static string Flyby {
            get { return Application.streamingAssetsPath + "/WC2/Videos/Flyby.mp4"; }
        }

        public static string Eject {
            get { return Application.streamingAssetsPath + "/WC2/Videos/Eject.mp4"; }
        }
    }
}
