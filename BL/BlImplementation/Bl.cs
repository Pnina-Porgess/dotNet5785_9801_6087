namespace BlImplementation;
using BlApi;
internal class Bl : IBl
{
    public IVolunteer? Volunteer
    {
        get { return new VolunteerImplementation(); }
    }
    public ICall? Call
    {
        get
        {
            return new CallImplementation();
        }
    }

    public IAdmin Admin { get; } = new AdminImplementation();

}

