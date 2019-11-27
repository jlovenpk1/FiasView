namespace FiasView
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("fias.house30")]
    public partial class house30
    {
        
        [StringLength(255)]
        public string AOGUID { get; set; }

        [StringLength(255)]
        public string BUILDNUM { get; set; }

        public DateTime? ENDDATE { get; set; }

        public int? ESTSTATUS { get; set; }

        [StringLength(255)]
        public string HOUSEGUID { get; set; }

        [Key]
        [StringLength(255)]
        public string HOUSEID { get; set; }

        [StringLength(255)]
        public string HOUSENUM { get; set; }

        public int? STATSTATUS { get; set; }

        [StringLength(255)]
        public string IFNSFL { get; set; }

        [StringLength(255)]
        public string IFNSUL { get; set; }

        [StringLength(255)]
        public string OKATO { get; set; }

        [StringLength(255)]
        public string OKTMO { get; set; }

        [StringLength(255)]
        public string POSTALCODE { get; set; }

        public DateTime? STARTDATE { get; set; }

        [StringLength(255)]
        public string STRUCNUM { get; set; }

        public int? STRSTATUS { get; set; }

        [StringLength(255)]
        public string TERRIFNSFL { get; set; }

        [StringLength(255)]
        public string TERRIFNSUL { get; set; }

        public DateTime? UPDATEDATE { get; set; }

        [StringLength(255)]
        public string NORMDOC { get; set; }

        public int? COUNTER { get; set; }

        [StringLength(255)]
        public string CADNUM { get; set; }

        public int? DIVTYPE { get; set; }
    }
}
