using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XpenseTrackerWebApp.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        [Required(ErrorMessage = "This field is required.")]
        public string Title { get; set; }

        [Column(TypeName = "nvarchar(5)")]
        public string Icon { get; set; } = "";

        //public string Color { get; set; }

        [Column(TypeName = "nvarchar(10)")]
        public string Type { get; set; } = "Expense";
        [NotMapped]
        public string? TitleWithIcon
        {
            get
            {
                return this.Icon + " " + this.Title;
            }
        }
    }

    public static class IconList
    {
        [Column(TypeName = "nvarchar(5)")]
        public static readonly List<string> Icons =
        [
            "📈", "💰", "🛒", "🏠", "🍴", "🚗", "🎉", "✈️", "📚", "💡",
        ];
    }

    public static class DoughnutChartRadius
    {
        public static readonly List<string> Radius =
        [
            "100", "118.7", "124.6", "137.5", "150.8", "155.5", "160.6", "165.8", "170"
        ];
    }
}
