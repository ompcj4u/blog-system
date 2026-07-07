using Domain.Entities;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogSystem.UnitTests.ObjectMother;
public static class UserMother
{

    public static User DefaultUser(string password) => new User("user full name",new Email("mohammad@gmail.com"), Password.Create(password));

    

}
