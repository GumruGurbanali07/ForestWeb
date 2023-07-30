namespace Forest.Models
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;//update etsende created date yarandigi vaxti gostersin
        public DateTime UpdatedDate { get; set; }
        public DateTime DeletedTime { get; set; }
        
        public bool IsActive { get; set; }
        //meqaleni silsende gedir zibil qutusuna yigilir ve admin silinmis meqalaleri gore bilir
        public bool IsDeleted { get; set; }
    }
}
