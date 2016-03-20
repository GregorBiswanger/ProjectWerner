using PropertyChanged;

namespace ProjectWerner.Extensions.Face2Speech.ViewModels
{
    [ImplementPropertyChanged]
    public class Word
    {
        public string Text { get; set; }
        public bool IsActive { get; set; }
    }
}