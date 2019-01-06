﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace TVProgViewer.TVProgApp
{
    public class Favorite
    {
        public Favorite()
        {
            
        }

        public Favorite(bool visible, Image image, string fileName, string favoriteName )
        {
            FavoriteName = favoriteName;
            FavoriteImage = image;
            FileName = fileName;
            Visible = visible;
        }

        public string FavoriteName { get; set; }

        public Image FavoriteImage { get; set; }

        public bool Visible { get; set; }

        public string FileName { get; set; }
    }
}
