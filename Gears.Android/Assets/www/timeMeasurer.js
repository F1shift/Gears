function CreateTimeMeasurer() {
    var StartTime;
    var LastTime;
    var EndTime;
    var TotalTime;
    

    var StartOrReset = () => {
        TotalTime = 0;
        StartTime = LastTime = new Date();
    };

    var Report = (timeTag = "") =>
    {
        EndTime = new Date();
        if (StartTime == null)
        {
            console.log("[TimeMeasurer]: StartTime is null! StartOrReset(); should be runned before Report();");
            return;
        }
        var timeSpan = EndTime.getTime() - LastTime.getTime();
        TotalTime += timeSpan;
        console.log("[" + timeTag + "]: Since Last reportation:" + timeSpan + " ms; Since start: " + TotalTime + " ms");
        LastTime = new Date();
    };

    var ReportAndReset = (timeTag = "") =>
    {
        Report(timeTag);
        StartOrReset();
    };

    var TimeMeasurer = {};
    Object.defineProperty(TimeMeasurer, "StartTime", { get() { return StartTime; } });
    Object.defineProperty(TimeMeasurer, "LastTime", { get() { return LastTime; } });
    Object.defineProperty(TimeMeasurer, "EndTime", { get() { return EndTime; } });
    Object.defineProperty(TimeMeasurer, "TotalTime", { get() { return TotalTime; } });
    Object.defineProperty(TimeMeasurer, "StartOrReset", { get() { return StartOrReset; } });
    Object.defineProperty(TimeMeasurer, "Report", { get() { return Report; } });
    Object.defineProperty(TimeMeasurer, "ReportAndReset", { get() { return ReportAndReset; } });
    return TimeMeasurer;
}
