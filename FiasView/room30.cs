namespace FiasView
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("room30")]
    public partial class room30
    {
        [Key]
        [StringLength(128)]
        public string ROOMID { get; set; }

        [StringLength(128)]
        public string ROOMGUID { get; set; }

        [StringLength(128)]
        public string HOUSEGUID { get; set; }

        [StringLength(128)]
        public string REGIONCODE { get; set; }

        [StringLength(128)]
        public string FLATNUMBER { get; set; }

        public int? FLATTYPE { get; set; }

        [StringLength(128)]
        public string ROOMNUMBER { get; set; }

        [StringLength(128)]
        public string ROOMTYPE { get; set; }

        [StringLength(128)]
        public string CADNUM { get; set; }

        [StringLength(128)]
        public string ROOMCADNUM { get; set; }

        [StringLength(128)]
        public string POSTALCODE { get; set; }

        public DateTime? UPDATEDATE { get; set; }

        [StringLength(128)]
        public string PREVID { get; set; }

        [StringLength(128)]
        public string NEXTID { get; set; }

        public int? OPERSTATUS { get; set; }

        public DateTime? STARTDATE { get; set; }

        public DateTime? ENDDATE { get; set; }

        public int? LIVESTATUS { get; set; }

        [StringLength(128)]
        public string NORMDOC { get; set; }
    }
}
