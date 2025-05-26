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
