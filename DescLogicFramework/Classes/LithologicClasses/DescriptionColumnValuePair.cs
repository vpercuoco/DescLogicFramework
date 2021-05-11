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
        public int ID { get; set; }

        [MaxLength(5000)]
        [Column(TypeName = "varchar(5000)")]
        public string Value { get; set; }

        [MaxLength(500)]
        [Column(TypeName = "varchar(500)")]
        public string ColumnName { get; set; }

        [MaxLength(500)]
        [Column(TypeName = "varchar(500)")]
        public string CorrectedColumnName { get; set; }

        [MaxLength(5000)]
        [Column(TypeName = "varchar(5000)")]
        public string CorrectedValue { get; set; }

    }
}
