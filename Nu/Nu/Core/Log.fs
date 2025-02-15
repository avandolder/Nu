﻿// Nu Game Engine.
// Copyright (C) Bryan Edds, 2013-2023.

namespace Nu
open System
open System.Collections.Concurrent
open System.Diagnostics
open Prime

[<RequireQualifiedAccess>]
module Log =

    let mutable private Initialized = false
#if DEBUG
    let mutable private InfoOnceMessages = ConcurrentDictionary StringComparer.Ordinal
    let mutable private WarnOnceMessages = ConcurrentDictionary StringComparer.Ordinal
    let mutable private ErrorOnceMessages = ConcurrentDictionary StringComparer.Ordinal
#endif

    let private getDateTimeNowStr () =
        let now = DateTimeOffset.Now
        now.ToString "yyyy-MM-dd HH\:mm\:ss.fff zzz"

    /// Log a purely informational message with Trace.TraceInformation.
    /// Thread-safe.
    let info message =
        Trace.TraceInformation (getDateTimeNowStr () + "|Info|" + message)

    /// Log a purely informational message once with Trace.WriteLine.
    /// Thread-safe.
    let infoOnce (message : string) =
#if DEBUG
        if InfoOnceMessages.TryAdd (message, 0) then info message
#else
        ignore message
#endif

    /// Log a warning message with Trace.TraceWarning.
    /// Thread-safe.
    let warn message =
        Trace.TraceWarning (getDateTimeNowStr () + "|Warning|" + message)

    /// Log a warning message once with Trace.WriteLine.
    /// Thread-safe.
    let warnOnce (message : string) =
#if DEBUG
        if WarnOnceMessages.TryAdd (message, 0) then warn message
#else
        ignore message
#endif

    /// Log an error message with Trace.TraceError.
    /// Thread-safe.
    let error message =
        Trace.TraceError (getDateTimeNowStr () + "|Error|" + message)

    /// Log an error message once with Trace.WriteLine.
    /// Thread-safe.
    let errorOnce (message : string) =
#if DEBUG
        if ErrorOnceMessages.TryAdd (message, 0) then warn message
#else
        ignore message
#endif

    /// Log a failure message using Trace.Fail.
    /// Thread-safe.
    let fail message =
        Trace.Fail (getDateTimeNowStr () + "|Fatal|" + message)

    /// Log an custom log type with Trace.TraceInformation.
    /// Thread-safe.
    let custom header message =
        Trace.TraceInformation (getDateTimeNowStr () + "|" + header + "|" + message)

    /// Initialize logging.
    let init (fileNameOpt : string option) =

        // init only once
        if not Initialized then

            // add listener
            let listeners = Trace.Listeners
            listeners.Add (new TextWriterTraceListener (Console.Out)) |> ignore
            match fileNameOpt with
            | Some fileName -> listeners.Add (new TextWriterTraceListener (fileName)) |> ignore
            | None -> ()

            // automatically flush logs
            Trace.AutoFlush <- true

            // mark as Initialized
            Initialized <- true