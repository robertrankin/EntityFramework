﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Microsoft.EntityFrameworkCore.Tools
{
    internal static class Exe
    {
        public static int Run(string executable, IReadOnlyList<string> args)
        {
            var arguments = ToArguments(args);

            Reporter.WriteVerbose(executable + " " + arguments);

            var build = Process.Start(
                new ProcessStartInfo
                {
                    FileName = executable,
                    Arguments = arguments,
                    UseShellExecute = false
                });
            build.WaitForExit();

            return build.ExitCode;
        }

        private static string ToArguments(IReadOnlyList<string> args)
        {
            var builder = new StringBuilder();
            for (var i = 0; i < args.Count; i++)
            {
                if (i != 0)
                {
                    builder.Append(" ");
                }

                if (args[i].IndexOf(' ') == -1)
                {
                    builder.Append(args[i]);

                    continue;
                }

                builder.Append("\"");

                var pendingBackslashs = 0;
                for (var j = 0; j < args[i].Length; j++)
                {
                    switch (args[i][j])
                    {
                        case '\"':
                            if (pendingBackslashs != 0)
                            {
                                builder.Append('\\', pendingBackslashs * 2);
                                pendingBackslashs = 0;
                            }
                            builder.Append("\\\"");
                            break;

                        case '\\':
                            pendingBackslashs++;
                            break;

                        default:
                            if (pendingBackslashs != 0)
                            {
                                if (pendingBackslashs == 1)
                                {
                                    builder.Append("\\");
                                }
                                else
                                {
                                    builder.Append('\\', pendingBackslashs * 2);
                                }

                                pendingBackslashs = 0;
                            }

                            builder.Append(args[i][j]);
                            break;
                    }
                }

                if (pendingBackslashs != 0)
                {
                    builder.Append('\\', pendingBackslashs * 2);
                }

                builder.Append("\"");
            }

            return builder.ToString();
        }
    }
}
