using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModels.Request
{
    public class CreateUpdateProblemImageRequest
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Emotion { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public string Characteristic { get; set; }

        [Required]
        public bool IsMine { get; set; }

        [Required]
        public bool IsIDo { get; set; }

        [Required]
        public bool IsForever { get; set; }

        [Required]
        public int LikeScore { get; set; }

        [Required]
        public long ParentImageID { get; set; }
    }
}
