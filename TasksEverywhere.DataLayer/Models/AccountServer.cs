﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksEverywhere.DataLayer.Models
{
    public class AccountServer
    {
        [Key]
        [Column(Order = 1)]
        public long AccountID { get; set; }
        [Key]
        [Column(Order = 2)]
        public long ServerID { get; set; }
    }
}
