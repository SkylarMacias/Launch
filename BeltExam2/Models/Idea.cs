using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using BeltExam.Models;

namespace BeltExam.Models
{
    public class Idea
    {
        [Key]
        public int IdeaId {get;set;}


        [Required(ErrorMessage="*Required")]
        [MinLength(5, ErrorMessage="Idea must be at least 5 characters")]
        public string Content {get;set;}



        public int UserId {get;set;}



        public User Creator {get;set;}


        public List<Like> UsersWhoLiked {get;set;}

    
    }
}