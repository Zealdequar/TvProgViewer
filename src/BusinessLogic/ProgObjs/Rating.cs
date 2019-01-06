﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TVProgViewer.BusinessLogic.ProgObjs
{
    /// <summary>
    /// Контракт (DTO) для рейтинга
    /// </summary>
    [DataContract]
    public class Rating
    {
        /// <summary>
        /// Идентификатор рейтинга
        /// </summary>
        [DataMember]
        public int RatingID { get; set; }

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
        /// Путь к пиктограмме рейтинга
        /// </summary>
        [DataMember]
        public string RatingPath { get; set; }

        /// <summary>
        /// Дата создание рейтинга
        /// </summary>
        [DataMember]
        public DateTimeOffset CreateDate { get; set; }

        /// <summary>
        /// Название рейтинга
        /// </summary>
        [DataMember]
        public string RatingName {get;set;}

        /// <summary>
        /// Видимость рейтинга
        /// </summary>
        [DataMember]
        public bool Visible { get; set; }

        /// <summary>
        /// Дата удаления рейтинга
        /// </summary>
        [DataMember]
        public DateTimeOffset DeleteDate { get; set; }
    }
}
