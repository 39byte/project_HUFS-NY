using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace NuriyeApp.Models
{
    [Table("Inventory")]
    public class InventoryItem : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("구분")]
        public string Category { get; set; } = "";

        [Column("카테고리")]
        public string BodyCategory { get; set; } = "";

        [Column("브랜드")]
        public string Brand { get; set; } = "";

        [Column("모델명")]
        public string ModelName { get; set; } = "";

        [Column("규격")]
        public string Format { get; set; } = "";

        [Column("상태")]
        public string Status { get; set; } = "대여가능";
    }
}
