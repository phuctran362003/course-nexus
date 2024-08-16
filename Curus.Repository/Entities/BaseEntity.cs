using System.ComponentModel.DataAnnotations;

namespace Curus.Repository.Entities
{
    public abstract class BaseEntity<TId>
    {
        [Key]
        public TId? Id { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string? ModifiedBy { get; set; }

        public DateTime? DeletedDate { get; set; }

        public string? DeletedBy { get; set; }

        public bool? IsDelete { get; set; } = false;
    }
}

