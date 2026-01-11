using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Domain.Lookups
{

    public enum Sex : int
    {
        Unknown = 1,
        Male = 2,
        Female = 3
    }

    public enum UserRole
    {
        Admin = 0,
        Registrar = 1,
        Doctor = 2
    }


    public enum AppointmentStatus
    {
        Planned = 0,    
        Completed = 1,   
        Canceled = 2    
    }


    public enum PaymentMethod : int
    {
        Cash = 1,
        Card = 2,
        Insurance = 3,
        BankTransfer = 4
    }

    public enum DocumentType : int
    {
        Passport = 1,
        IDCard = 2,
        BirthCertificate = 3,
        Other = 4
    }
}
