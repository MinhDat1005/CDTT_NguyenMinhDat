using System.ComponentModel.DataAnnotations;

namespace Web_CDTT_NguyenMinhDat.Models.ViewModels
{
    public class LoginViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập Username")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập Password")]
    
        public string Password { get; set; }

        public string returnUrl { get; set; }
    }
}
