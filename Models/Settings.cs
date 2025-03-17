using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XpenseTrackerWebApp.Models
{
    public class Settings
    {
        [Key]
        public int Id { get; set; }
        public int ExpensesLimit { get; set; }
    }
}
