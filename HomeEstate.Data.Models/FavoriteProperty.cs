namespace HomeEstate.Models
{
    public class FavoriteProperty
    {
        //mapping таблица
        public int PropertyId { get; set; } // FK към Property
        public Property Property { get; set; } = null!; 

        public string UserId { get; set; } = null!; // FK ApplicationUser 
        public ApplicationUser User { get; set; } = null!; 
    }
}

