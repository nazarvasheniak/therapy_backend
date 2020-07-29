using System;
namespace Domain.ViewModels
{
    public class SpecialistProfileActiveSessionViewModel : BaseViewModel
    {
        public SessionViewModel Session { get; set; }
        public ClientCardViewModel Client { get; set; }
        public int ImagesCount { get; set; }
        public int ResourcesCount { get; set; }
    }
}
