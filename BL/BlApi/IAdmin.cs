

namespace BlApi;
    public interface IAdmin
    {
        void InitializeDB();
        void ResetDB();
       TimeSpan GetRiskRange();
        void SetRiskRange(TimeSpan range);
        DateTime GetClock();
        void ForwardClock(BO.TimeUnit unit);
    }