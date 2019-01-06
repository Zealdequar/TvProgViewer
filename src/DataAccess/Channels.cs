//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TVProgViewer.DataAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class Channels
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Channels()
        {
            this.Programmes = new HashSet<Programmes>();
            this.UserChannels = new HashSet<UserChannels>();
        }
    
        public int ChannelID { get; set; }
        public int TVProgProviderID { get; set; }
        public Nullable<int> InternalID { get; set; }
        public Nullable<long> IconID { get; set; }
        public System.DateTimeOffset CreateDate { get; set; }
        public string TitleChannel { get; set; }
        public string IconWebSrc { get; set; }
        public Nullable<System.DateTime> Deleted { get; set; }
    
        public virtual MediaPic MediaPic { get; set; }
        public virtual TVProgProviders TVProgProviders { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Programmes> Programmes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserChannels> UserChannels { get; set; }
    }
}
