using System;
using MonoTouch.ObjCRuntime;

[assembly:LinkWith("libnslogger.a", LinkTarget.ArmV6 | LinkTarget.ArmV7 | LinkTarget.Simulator, Frameworks = "CFNetwork SystemConfiguration", ForceLoad = true, IsCxx = false, NeedsGccExceptionHandling = false)]
