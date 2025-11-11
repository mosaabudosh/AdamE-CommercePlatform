using Adam.Generic.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Model.Authorisation
{
    public class CustomerModel
    {
        public string? CustomerId { get; set; }
        public string FirstName { get; set; }
        public string? SecondName { get; set; }
        public string LastName { get; set; }
        public string? FirstNameAr { get; set; }
        public string? SecondNameAr { get; set; }
        public string? LastNameAr { get; set; }
        public string? UserName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string? Password { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? ProfileImage { get; set; }
        public string? Adresse { get; set; }
        public bool? IsBlock { get; set; }
        public int? GenderId { get; set; }
        public string? Gender { get; set; }
        public string? Age { get; set; }
        public int? CityId { get; set; }
        public bool? IsActive { get; set; }
        public FileCreationModel? FileCreationModel { get; set; }
        public List<CustomerDeliveryAddressModel>? CustomerDeliveryAddressModels { get; set; }
        public List<CustomerCardInfoModel>? CustomerCardInfoModels { get; set; }
    }
}
