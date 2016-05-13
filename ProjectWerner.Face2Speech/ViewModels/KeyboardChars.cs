using PropertyChanged;

namespace ProjectWerner.Face2Speech.ViewModels
{
    [ImplementPropertyChanged]
    public class KeyboardChars
    {
        public string Text { get; set; }
        public string Type { get; set; }
        public bool IsActive { get; set; }
    }
}