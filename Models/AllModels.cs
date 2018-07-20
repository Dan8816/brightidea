using System;
using System.Collections.Generic;//allows creating of list objects
using System.ComponentModel.DataAnnotations;//dependency for validations compared to models
using System.ComponentModel.DataAnnotations.Schema;//dependency for validations compared to db schema


namespace DojoSecret.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }//primary key
        public string Name { get; set; }//must match the schema name
        public string Alias { get; set; }//must match the schema name
        public string Email { get; set; }//must match the schema name
        public string Password { get; set; }//must match the schema name
        public DateTime Created_At { get; set; }//must match the schema name
        public DateTime Updated_At { get; set; }//must match the schema name
        public List<Message> secrets { get; set; }//this instantiates a list of Message objects from the secrets table connected to a User object for M2M
        public List<Like> likes { get; set; }//this instantiates a list of Like objects from the likes table connected to a User object for M2M
        public User()
        {
            Created_At = DateTime.Now;
            Updated_At = DateTime.Now;
            secrets = new List<Message>();
            likes = new List<Like>();
        }
    }
    public class Message
    {
        [Key]
        public int Id { get; set; }//primary key
        [Required(ErrorMessage = "Content is required.")]//input called Content cannot be blank
        [MinLength(2, ErrorMessage="Must be at least 2 characters.")]
        [MaxLength(255, ErrorMessage="Max 255 characters.")]
        public string Content { get; set; }//must match the schema name
        public DateTime Created_At { get; set;}//must match the schema name
        public DateTime Updated_At { get; set;}//must match the schema name
        public int CreatorId { get; set; }//must match the schema name
        public User Creator { get; set; }//this will refer to an instance of a User attached to the foreign key CreatorId
        public List<Like> likes { get; set; }//this instantiates a list of Like objects from the likes table connected to a User object for M2M
        public Message()
        {
            Created_At = DateTime.Now;
            Updated_At = DateTime.Now;
            likes = new List<Like>();
        }
    }
    public class Like
    {
        [Key]
        public int Id { get; set; }//primary key
        public int LikedId { get; set; }//must match the schema name
        public Message Liked { get; set; }//this will refer to an instance of a Message attached to the foreign key LikedId
        public int PromoterId { get; set; }//must match the schema name
        public User Promoter { get; set; }//this will refer to an instance of a User attached to the foreign key PromoterId
    }
}