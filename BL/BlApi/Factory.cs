namespace BlApi
{
    /// <summary>
    /// Factory class for creating instances of the BL (Business Logic) layer.
    /// </summary>
    public static class Factory
    {
        public static IBl Get() => new BlImplementation.Bl();
    }
}

