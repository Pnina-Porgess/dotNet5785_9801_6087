using System.Collections;
using System.Collections.Generic;
namespace PL;
internal class CallFieldCollection : IEnumerable
{
    public BO.CallField Semester { get; set; } = BO.CallField.AssignmentId;
    static readonly IEnumerable<BO.CallField> s_enums =
(Enum.GetValues(typeof(BO.CallField)) as IEnumerable<BO.CallField>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}
