﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace TVProgViewer.TVProgApp
{
    public class Genre
    {
        private string _genreName;
        private Image _image;
        private string _fileName;
        private bool _visible;

        public Genre()
        {
            
        }

        public Genre(bool visible, Image image, string fileName, string genreName )
        {
            _genreName = genreName;
            _image = image;
            _fileName = fileName;
            _visible = visible;
        }

        public string GenreName
        {
            get { return _genreName; }
            set { _genreName = value; }
        }

        public Image GenreImage
        {
            get { return _image; }
            set { _image = value; }
        }

        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }
    }
}
