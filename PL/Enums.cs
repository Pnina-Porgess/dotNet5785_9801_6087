using System.Collections;
using System.Collections.Generic;
namespace PL;
internal class CallFieldCollection : IEnumerable
{
    public BO.CallField CallType { get; set; } = BO.CallField.AssignmentId;
    static readonly IEnumerable<BO.CallField> s_enums =
(Enum.GetValues(typeof(BO.CallField)) as IEnumerable<BO.CallField>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

internal class VolunteerFieldCollection : IEnumerable
{
    public BO.TypeOfReading TypeOfReading { get; set; } = BO.TypeOfReading.None;
    static readonly IEnumerable<BO.TypeOfReading> s_enums =
        (Enum.GetValues(typeof(BO.TypeOfReading)) as IEnumerable<BO.TypeOfReading>)!;
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}


internal class OpenCallFieldCollection : IEnumerable
{
    public BO.OpenCallField SortField { get; set; } = BO.OpenCallField.Id;
    static readonly IEnumerable<BO.OpenCallField> s_enums =
        (Enum.GetValues(typeof(BO.OpenCallField)) as IEnumerable<BO.OpenCallField>)!;
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

internal class CallStatusCollection : IEnumerable
{
    public BO.CallStatus Status { get; set; } = BO.CallStatus.Open;
    static readonly IEnumerable<BO.CallStatus> s_enums =
        (Enum.GetValues(typeof(BO.CallStatus)) as IEnumerable<BO.CallStatus>)!;
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}
