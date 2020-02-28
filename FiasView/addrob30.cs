namespace FiasView
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using MySql.Data.EntityFrameworkCore.DataAnnotations;

    [Table("addrob30")]
    [MySqlCharset("utf8")]
    public partial class addrob30
    {
        public int? ACTSTATUS { get; set; }

        [StringLength(128)]
        public string AOGUID { get; set; }

        [Key]
        [StringLength(128)]
        public string AOID { get; set; }

        public int? AOLEVEL { get; set; }

        [StringLength(128)]
        public string AREACODE { get; set; }

        [StringLength(128)]
        public string AUTOCODE { get; set; }

        public int? CENTSTATUS { get; set; }

        [StringLength(128)]
        public string CITYCODE { get; set; }

        [StringLength(128)]
        public string CODE { get; set; }

        public int? CURRSTATUS { get; set; }

        public DateTime? ENDDATE { get; set; }

        [StringLength(128)]
        public string FORMALNAME { get; set; }

        [StringLength(128)]
        public string IFNSFL { get; set; }

        [StringLength(128)]
        public string IFNSUL { get; set; }

        [StringLength(128)]
        public string NEXTID { get; set; }

        [StringLength(128)]
        public string OFFNAME { get; set; }

        [StringLength(128)]
        public string OKATO { get; set; }

        [StringLength(128)]
        public string OKTMO { get; set; }

        public int? OPERSTATUS { get; set; }

        [StringLength(128)]
        public string PARENTGUID { get; set; }

        [StringLength(128)]
        public string PLACECODE { get; set; }

        [StringLength(128)]
        public string PLAINCODE { get; set; }

        [StringLength(128)]
        public string POSTALCODE { get; set; }

        [StringLength(128)]
        public string PREVID { get; set; }

        [StringLength(128)]
        public string REGIONCODE { get; set; }

        [StringLength(128)]
        public string SHORTNAME { get; set; }

        public DateTime? STARTDATE { get; set; }

        [StringLength(128)]
        public string STREETCODE { get; set; }

        [StringLength(128)]
        public string TERRIFNSFL { get; set; }

        [StringLength(128)]
        public string TERRIFNSUL { get; set; }

        public DateTime? UPDATEDATE { get; set; }

        [StringLength(128)]
        public string CTARCODE { get; set; }

        [StringLength(128)]
        public string EXTRCODE { get; set; }

        [StringLength(128)]
        public string SEXTCODE { get; set; }

        public int? LIVESTATUS { get; set; }

        [StringLength(128)]
        public string NORMDOC { get; set; }

        [StringLength(128)]
        public string PLANCODE { get; set; }

        [StringLength(128)]
        public string CADNUM { get; set; }

        public int? DIVTYPE { get; set; }

    }
}
