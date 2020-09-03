using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DescLogicFramework
{
   public class DescriptionColumnValuePair
    {

        [Key]
        public Guid ID { get; set; }

        [MaxLength(1000)]
        [Column(TypeName = "varchar(1000)")]
        public string Value { get; set; }

        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string ColumnName { get; set; }



    }

    public class MeasurementColumnValuePair
    {
        [Key]
        public Guid ID { get; set; }

        [MaxLength(500)]
        [Column(TypeName = "varchar(500)")]
        public string Value { get; set; }

        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string ColumnName { get; set; }

        public int? LithologicSubID { get; set; }

        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string LithologicID { get; set; }

    }
}
