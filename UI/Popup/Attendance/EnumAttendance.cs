using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnumAttendance : BaseEnum
{
    public enum EAttendance
    {
        None,

        Day,
        Weekly,
        Event,
    }

    private EAttendance attendanceType;
    public EAttendance AttendanceType => attendanceType;

    public void SetAttendanceType(EAttendance type)
    {
        attendanceType = type;
    }

}
