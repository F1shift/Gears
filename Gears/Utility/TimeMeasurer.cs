using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Gears.Utility
{
    class TimeMeasurer
    {
        public class ARecord
        {
            public String TimeTag;
            public DateTime RecordedTime;
            public TimeSpan TimSpane;
            public TimeSpan CurrentTotalTims;
        }

        public DateTime StartTime { get; set; }
        public DateTime LastTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan TotalTime { get; set; }
        public List<ARecord> RecordList { get; set; }

        public static Func<TimeSpan, String> DefautTimeSpanFormatter = (time) => { return time.TotalMilliseconds.ToString() + " ms"; };

        public void StartOrReset()
        {
            TotalTime = new TimeSpan();
            StartTime  = LastTime = DateTime.Now;
            RecordList = new List<ARecord>();
        }
        public void Report(
            String timeTag = "",
            Func<TimeSpan, String> timeSpanFormatter = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string callerPath = "",
              [CallerLineNumber] int lineNumber = -1)
        {
            if (timeSpanFormatter == null)
            {
                timeSpanFormatter = DefautTimeSpanFormatter;
            }
            EndTime = DateTime.Now;
            var timeSpan = EndTime - LastTime;
#if DEBUG
            if (StartTime == null)
            {
                Debug.WriteLine($"[{nameof(TimeMeasurer)}] (Caller:{callerPath}//{memberName} in line:{lineNumber}) : {nameof(StartTime)} is null! StartOrReset(); should be runned before Report();");
                return;
            }
            TotalTime += timeSpan;
            Debug.WriteLine($"[{timeTag}] : Since Last reportation : {timeSpanFormatter(timeSpan)}; Since start {timeSpanFormatter(TotalTime)}");
#endif
            LastTime = DateTime.Now;
            RecordList.Add(new ARecord()
            {
                TimeTag = timeTag,
                RecordedTime = EndTime,
                CurrentTotalTims = TotalTime,
                TimSpane = timeSpan
            });
        }

        public void ReportAndReset(
            String timeTag = "",
            Func<TimeSpan, String> timeSpanFormatter = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string callerPath = "",
              [CallerLineNumber] int lineNumber = -1)
        {
            Report(timeTag, timeSpanFormatter, memberName, callerPath, lineNumber);
            StartOrReset();
        }

        public void PrintAllRecord(Func<TimeSpan, String> timeSpanFormatter = null) 
        {
            if (timeSpanFormatter == null)
            {
                timeSpanFormatter = DefautTimeSpanFormatter;
            }
            foreach (var item in RecordList)
            {
                Debug.WriteLine($"[{item.RecordedTime.TimeOfDay}:{item.TimeTag}] : Tim Span : {timeSpanFormatter(item.TimSpane)} ms, Total Time : {timeSpanFormatter(item.CurrentTotalTims)} ms");
            }
        }
    }
}
