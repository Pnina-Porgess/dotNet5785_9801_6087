using DalApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dal;

sealed public class DalXml : IDal
{

public IAssignment Assignment => throw new NotImplementedException();

    public IVolunteer Volunteer => throw new NotImplementedException();

    public ICall Call => throw new NotImplementedException();

    public IConfig Config => throw new NotImplementedException();

    public void ResetDB()
    {
        throw new NotImplementedException();
    }
}

