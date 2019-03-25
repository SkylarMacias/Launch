using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace BeltExam.Models
{
    public class ViewModel
    {
        public List<User> DisplayUsers {get;set;}

        public List<Idea> DisplayIdeas {get;set;}

        public List<Like> DisplayLikes {get;set;}

        public LoginUser LoginUser {get;set;}

        public User User {get;set;}

        public Idea Idea {get;set;}

        public Like Like {get;set;}

        public List<int> LikeIds {get;set;}

    }
}