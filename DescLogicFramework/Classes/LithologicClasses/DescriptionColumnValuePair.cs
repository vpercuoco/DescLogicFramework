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

        [MaxLength(1000)]
        [Column(TypeName = "varchar(5000)")]
        public string Value { get; set; }

        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string ColumnName { get; set; }

    }
}
