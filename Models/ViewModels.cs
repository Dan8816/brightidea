using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DojoSecret.Models
{
    public class CustomDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)//this is the validation for future dates
        {
            DateTime date = (DateTime)value;
            return date > DateTime.Now;//this implies the converted string to date must be later than the DateTime.Now, so in the future, inversely return < DateTime.Now would be a past date
        }
    }
    public class LoginUser//this class is for instantiating a user in the Login route within User controller
    {
        public string LogEmail { get; set; }//their will be a string input from a form called LogEmail for asp-validation

        [DataType(DataType.Password)]
        public string LogPassword { get; set; }//their will be a string input from a form called LogPassword for asp-validation
    }

    public class RegisterUser//this class is for validation when creating a user
    {
        [Key]
        public int Id { get; set; }//Primary Key

        [Required(ErrorMessage = "Name field must not be empty.")]//the input called First_Name cannot be blank
        [RegularExpression("^[a-zA-Z_ ]+$", ErrorMessage = "Name must be non-numerical.")]//one string no spaces and only letters
        [MinLength(2, ErrorMessage = "Min 2 characters.")]
        [MaxLength(45, ErrorMessage = "Max 45 characters.")]
        public string Name { get; set; }//this must match the table field name and case wise

        [Required(ErrorMessage = "Alias field must not be empty.")]//the input called Last_Name cannot be blank
        [RegularExpression("^[a-zA-Z_ ]+$", ErrorMessage = "Alias must be non-numerical.")]//one string no spaces and only letters
        [MinLength(2,ErrorMessage = "Min 2 characters.")]
        [MaxLength(45, ErrorMessage = "Max 45 characters.")]
        public string Alias { get; set; }//this must match the table field name and case wise

        [Required(ErrorMessage = "Email field must not be empty.")]//the input called  Email cannot be blank
        [EmailAddress(ErrorMessage = "Email field must be valid email.")]//ASP.Net has a built in regex for valid email strings
        //[RegularExpression(@"^\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}$")]//if you wanted your own email regex
        [MaxLength(45, ErrorMessage = "Max 45 characters." )]//this only validated the input string length, the table allows 255 chars to account for hashing
        public string Email { get; set; }//this must match the table field name and case wise

        [Required(ErrorMessage = "Password field must not be empty.")]//the input called Password cannot be blank
        [MinLength(8, ErrorMessage = "Password must be 8 or more characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }//this must match the table field name and case wise

        [Required(ErrorMessage = "Password confirm field must not be empty.")]
        [NotMapped]//not looked at a field in the db, this is just for validation
        [Compare("Password", ErrorMessage = "Passwords do not match.")]//compares to a form input called "Password"
        public string confirm { get; set; }//form input called confirm
        public DateTime Created_At { get; set; }//this must match the table field name and case wise
        public DateTime Updated_At { get; set; }//this must match the table field name and case wise
    }

    public class NewMessage//this class is for validation when creating a new message
    {
        [Key]
        public int Id { get; set; }//Primary Key
        [Required(ErrorMessage = "Content is required.")]//input called Content cannot be blank
        [MinLength(2, ErrorMessage="Must be at least 2 characters.")]
        [MaxLength(255, ErrorMessage="Max 255 characters.")]
        public string Content {get; set;}//this must match the table field name and case wise
        public int CreatorId { get; set; }//must match the schema name
        public User Creator { get; set; }//this will refer to an instance of a User attached to the foreign key CreatorId
        public DateTime Created_At { get; set; }//this must match the table field name and case wise
        public DateTime Updated_At { get; set; }//this must match the table field name and case wise
    }

    public class DashboardModel//this is to bring everything into the dashboard cshtml
    {
        public User ninjas {get; set;}
        public Message secrets {get; set;}
        public Like likes {get; set;}
        public List<User> NinjaList {get;set;}
        public List<Message> MessageList {get;set;}
    }
}