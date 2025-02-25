﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DietineWebApp.Models
{
    public class BreakfastFood
    {
        [Key]
        public int BreakfastFoodID { get; set; }
        [Required]

        [DisplayName("Name")]
        public string BFName { get; set; }

        [DisplayName("Calorie Ratio (kcal/100 gram)")]
        public double BFCaloriePerOunce { get; set; }
        [DisplayName("Quantity (gram)")]
        public double BFGram { get; set; }
        [DisplayName("Total (kcal)")]
        public double BFTotalCalorie { get => Math.Round(BFCaloriePerOunce * BFGram / 100, 3); set { } }
        public int BFDbFoodID { get; set; }
        public string BFUserID { get; set; }
        [DisplayName("Date")]
        public string BFDate { get; set; }
    }
}
