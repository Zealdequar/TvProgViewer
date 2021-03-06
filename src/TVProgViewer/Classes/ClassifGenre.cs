﻿using System;

namespace TVProgViewer.TVProgApp
{
    public class ClassifGenre
    {
        public string Contain { get; set; }        // - содержит
        public string NonContain { get; set; }     // - не содержит
        public int IdGenre { get; set; }           // - код жанра
        public DateTime TsDeleteAfter { get; set; }  // - удалить после
        
        /// <summary>
        /// Конструктор (по умолчанию)
        /// </summary>
        public ClassifGenre()
        {
            
        }

        /// <summary>
        /// Конструктор (расширенный)
        /// </summary>
        /// <param name="contain">Содержит</param>
        /// <param name="nonContain">Не содержит</param>
        /// <param name="idGnere">Код жанра</param>
        /// <param name="tsDeleteAfter">Удалить после</param>
        public ClassifGenre(string contain, string nonContain, int idGnere, DateTime tsDeleteAfter)
        {
            Contain = contain;
            NonContain = nonContain;
            IdGenre = idGnere;
            TsDeleteAfter = tsDeleteAfter;
        }
    }
}