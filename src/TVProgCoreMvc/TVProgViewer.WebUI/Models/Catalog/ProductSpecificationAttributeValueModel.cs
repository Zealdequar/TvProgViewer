﻿using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Models.Catalog
{
    /// <summary>
    /// Represents a product specification attribute value model
    /// </summary>
    public record ProductSpecificationAttributeValueModel : BaseTvProgModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the attribute type id
        /// </summary>
        public int AttributeTypeId { get; set; }

        /// <summary>
        /// Gets or sets the value raw. This value is already HTML encoded
        /// </summary>
        public string ValueRaw { get; set; }

        /// <summary>
        /// Gets or sets the option color (if specified). Used to display color squares
        /// </summary>
        public string ColorSquaresRgb { get; set; }

        #endregion
    }
}
