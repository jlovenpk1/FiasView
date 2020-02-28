namespace FiasView
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("fias.addrcorrect")]
    public partial class addrcorrect
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        [Required]
        [StringLength(128)]
        public string shortName { get; set; }

        [Required]
        [StringLength(128)]
        public string fullName { get; set; }

        [StringLength(45)]
        public string FiasID { get; set; }
    }
}
