
namespace PageAccessCap.Models
{
    public class ErrorViewModel
    {
        public int PageHitCount { get; set; }
        public string Message { get; set; }

        public ErrorViewModel() { }
        public ErrorViewModel(string message)
            : this()
        {
            this.Message = message;
        }
    }
}