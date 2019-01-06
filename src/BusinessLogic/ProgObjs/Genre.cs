﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TVProgViewer.BusinessLogic.ProgObjs
{
    /// <summary>
    /// Контракт (DTO) для жанра
    /// </summary>
    [DataContract]
    public class Genre
    {
        /// <summary>
        /// Идентификатор жанра
        /// </summary>
        [DataMember]
        public int GenreID { get; set; }

        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        [DataMember]
        public long UID { get; set; }

        /// <summary>
        /// Идентификатор пиктограммы
        /// </summary>
        [DataMember]
        public int IconID { get; set; }

        /// <summary>
        /// Путь к пиктограмме жанра
        /// </summary>
        [DataMember]
        public string GenrePath { get; set; }

        /// <summary>
        /// Дата создания жанра
        /// </summary>
        [DataMember]
        public DateTimeOffset CreateDate { get; set; }

        /// <summary>
        /// Название жанра
        /// </summary>
        [DataMember]
        public string GenreName { get; set; }

        /// <summary>
        /// Видимость жанра
        /// </summary>
        [DataMember]
        public bool Visible { get; set; }

        /// <summary>
        /// Дата удаления жанра
        /// </summary>
        [DataMember]
        public DateTimeOffset DeleteDate { get; set; }
    }
}
