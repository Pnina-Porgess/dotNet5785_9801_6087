namespace BlImplementation
{
    using BlApi;

    /// <summary>
    /// Implementation of the IBl interface providing access to various business logic services.
    /// </summary>
    internal class Bl : IBl
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        /// <summary>
        /// Gets an instance of the IVolunteer service.
        /// </summary>
        public IVolunteer? Volunteer
        {
            get { return new VolunteerImplementation(); }
        }

        /// <summary>
        /// Gets an instance of the ICall service.
        /// </summary>
        public ICall? Call
        {
            get
            {
                return new CallImplementation();
            }
        }

        /// <summary>
        /// Gets an instance of the IAdmin service for administrative functionality.
        /// </summary>
        public IAdmin Admin { get; } = new AdminImplementation();
    }
}
